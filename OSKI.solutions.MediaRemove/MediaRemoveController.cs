using OSKI.solutions.MediaRemove.Models;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using Our.Umbraco.Nexu.Common;
using Our.Umbraco.Nexu.Common.Interfaces.Services;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Editors;

namespace OSKI.solutions.MediaRemove
{
    public class MediaRemoveController : UmbracoAuthorizedJsonController
    {
        private readonly MediaRemoveService mediaRemoveService;
        private readonly IEntityRelationService nexuService;
        private readonly IEntityParsingService entityParsingService;
        private readonly IContentService contentService;
        private readonly IMediaService mediaService;
        private readonly ILogger logger;

        private readonly MediaRemoveContext mediaRemoveContext;

        public MediaRemoveController(MediaRemoveService mediaRemoveService, IEntityRelationService nexuService, IEntityParsingService entityParsingService, IContentService contentService, IMediaService mediaService, ILogger logger)
        {
            this.mediaRemoveService = mediaRemoveService;
            this.nexuService = nexuService;
            this.entityParsingService = entityParsingService;
            this.contentService = contentService;
            this.mediaService = mediaService;
            this.logger = logger;
            
            mediaRemoveContext = MediaRemoveContext.Current;
        }

        [HttpGet]
        public HttpResponseMessage GetUnusedMedia()
        {
            Thread backgroundGetMedia = new Thread(FindUnusedMedia);
            backgroundGetMedia.IsBackground = true;
            backgroundGetMedia.Name = "MediaRemove GetUnusedMedia";
            backgroundGetMedia.Start();

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpGet]
        public HttpResponseMessage GetUnusedMediaStatus()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new { mediaRemoveContext.IsProcessingMedia, Data = mediaRemoveContext.UnusedMedia.Select(x => x.Model), TotalCount = mediaRemoveContext.TotalAmountOfMedia });
        }

        [HttpGet]
        public HttpResponseMessage DeleteUnusedMediaStatus()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new { mediaRemoveContext.IsProcessingDeleting, ItemsToProcess = mediaRemoveContext.ItemsToProcessDeleting, ItemsProcessed = mediaRemoveContext.DeletedItemsProcessed, });
        }

        [HttpPost]
        public HttpResponseMessage DeleteUnusedMedia(int[] ids)
        {
            Thread backgroundDelete = new Thread(new ParameterizedThreadStart(DeleteMedia))
            {
                IsBackground = true,
                Name = "MediaRemove DeleteMedia"
            };
            backgroundDelete.Start(ids);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpGet]
        public HttpResponseMessage IsBuilt()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new { IsBuilt = mediaRemoveService.IsBuilt() });
        }

        [HttpGet]
        public HttpResponseMessage GetRebuildStatus()
        {
            var model = new 
            {
                NexuContext.Current.IsProcessing,
                NexuContext.Current.ItemsProcessed,
                ItemName = NexuContext.Current.ItemInProgress
            };

            return this.Request.CreateResponse(HttpStatusCode.OK, model);
        }

        [HttpGet]
        public HttpResponseMessage Rebuild()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.RebuildJob));

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public void RebuildJob(object state)
        {
            try
            {
                NexuContext.Current.IsProcessing = true;

                var rootLevelItems = this.contentService.GetRootContent().ToList();

                foreach (var item in rootLevelItems)
                {
                    this.ParseContent(item);
                }

                long totalRecords = 0;
                rootLevelItems = this.contentService.GetPagedChildren(
                    Constants.System.RecycleBinContent,
                    0,
                    int.MaxValue,
                    out totalRecords).ToList();

                foreach (var item in rootLevelItems)
                {
                    this.ParseContent(item);
                }
            }
            catch (Exception e)
            {
                this.logger.Error<MediaRemoveController>("An unhandled exception occurred while parsing content", e);
            }
            finally
            {
                NexuContext.Current.IsProcessing = false;
                NexuContext.Current.ItemsProcessed = 0;
                NexuContext.Current.ItemInProgress = string.Empty;
            }
        }

        private void ParseContent(IContent item)
        {
            NexuContext.Current.ItemInProgress = item.Name;
            this.entityParsingService.ParseContent(item);
            NexuContext.Current.ItemsProcessed++;

            long totalRecords = 0;
            var children = this.contentService.GetPagedChildren(item.Id, 0, int.MaxValue, out totalRecords).ToList();

            foreach (var child in children)
            {
                // parse the content
                this.ParseContent(child);
            }
        }

        private void GetUnusedMediaItems(MediaItemWrapper root)
        {
            var children = this.mediaService.GetPagedChildren(root.Media.Id, 0, int.MaxValue, out long totalRecords).ToList();
            bool found = false;
            mediaRemoveContext.TotalAmountOfMedia += children.Count();
            foreach (var mediaItem in children)
            {
                found = true;
                GetUnusedMediaItems(new MediaItemWrapper(mediaItem, root));
            }

            if (found)
            {
                return;
            }

            var relations = nexuService.GetRelationsForItem(root.Media.GetUdi());
            if (!relations.Any())
                mediaRemoveContext.UnusedMedia.Add(root);
        }

        internal void FindUnusedMedia()
        {
            mediaRemoveContext.UnusedMedia = new ConcurrentBag<MediaItemWrapper>();
            mediaRemoveContext.IsProcessingMedia = true;
            mediaRemoveContext.TotalAmountOfMedia = 0;
            var mediaItems = mediaService.GetRootMedia();
            mediaRemoveContext.TotalAmountOfMedia += mediaItems.Count();
            foreach (var mediaItem in mediaItems)
            {
                GetUnusedMediaItems(new MediaItemWrapper(mediaItem));
            }
            mediaRemoveContext.IsProcessingMedia = false;
        }

        private void DeleteMedia(object packed)
        {
            int[] ids = (int[])packed;

            mediaRemoveContext.IsProcessingDeleting = true;
            mediaRemoveContext.ItemsToProcessDeleting = ids.Length;
            mediaRemoveContext.DeletedItemsProcessed = 0;
            mediaRemoveContext.TotalAmountOfMedia = 0;

            foreach (var id in ids)
            {
                mediaService.Delete(mediaService.GetById(id));
                mediaRemoveContext.DeletedItemsProcessed++;
            }

            mediaRemoveContext.IsProcessingDeleting = false;
            mediaRemoveContext.UnusedMedia = new ConcurrentBag<MediaItemWrapper>();
        }
    }
}
