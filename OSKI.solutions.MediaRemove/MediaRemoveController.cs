using AutoMapper;
using OSKI.solutions.MediaRemove.Models;
using Our.Umbraco.Nexu.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Editors;

namespace OSKI.solutions.MediaRemove
{
    public class MediaRemoveController: UmbracoAuthorizedJsonController
    {
        private readonly IMediaService mediaService;
        private readonly NexuService nexuService;
        private readonly MediaRemoveService mediaRemoveService;
        private readonly MediaRemoveContext mediaRemoveContext;

        public MediaRemoveController()
        {
            this.mediaService = this.Services.MediaService;
            this.nexuService = NexuService.Current;
            mediaRemoveService = MediaRemoveService.Current;
            mediaRemoveContext = MediaRemoveContext.Current;
        }

        [HttpGet]
        public HttpResponseMessage GetUnusedMedia()
        {
            Thread backgroundRebuild = new Thread(FindUnusedMedia);
            backgroundRebuild.IsBackground = true;
            backgroundRebuild.Name = "MediaRemove GetUnusedMedia";
            backgroundRebuild.Start();

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpGet]
        public HttpResponseMessage GetUnusedMediaStatus()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new { mediaRemoveContext.IsProcessingMedia, Data = mediaRemoveContext.UnusedMedia.Select(x => x.Model) });
        }

        [HttpPost]
        public HttpResponseMessage DeleteUnusedMedia(int[] ids)
        {
            foreach (var id in ids)
            {
                mediaService.Delete(mediaService.GetById(id));
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpGet]
        public HttpResponseMessage IsBuilt()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new { IsBuilt = mediaRemoveService.IsBuilt() });
        }
        private void GetUnusedMediaItems(MediaItemWrapper root)
        {
            var childs = root.Media.Children();
            bool found = false;
            foreach (var mediaItem in childs)
            {
                found = true;
                GetUnusedMediaItems(new MediaItemWrapper(mediaItem, root));
            }

            if (found)
            {
                return;
            }

            var relations = nexuService.GetNexuRelationsForContent(root.Media.Id, false);
            if (!relations.Any())
                mediaRemoveContext.UnusedMedia.Add(root);
        }

        internal void FindUnusedMedia()
        {
            mediaRemoveContext.UnusedMedia = new ConcurrentBag<MediaItemWrapper>();
            mediaRemoveContext.IsProcessingMedia = true;
            var mediaItems = mediaService.GetRootMedia();
            foreach (var mediaItem in mediaItems)
            {
                GetUnusedMediaItems(new MediaItemWrapper(mediaItem));
            }
            mediaRemoveContext.IsProcessingMedia = false;
        }
    }
}
