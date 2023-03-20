using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using MediaRemove.Interfaces;
using MediaRemove.Interfaces.Nexu;
using MediaRemove.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Extensions;

namespace MediaRemove
{
    public class MediaRemoveController : UmbracoApiController
    {
        private readonly IMediaRemoveService _mediaRemoveService;
        private readonly IEntityRelationService _nexuService;
        private readonly IEntityParsingService _entityParsingService;
        private readonly IContentService _contentService;
        private readonly IMediaService _mediaService;
        private readonly ILogger _logger;

        private readonly MediaRemoveContext _mediaRemoveContext;

        public MediaRemoveController(IMediaRemoveService mediaRemoveService, IEntityRelationService nexuService, IEntityParsingService entityParsingService, IContentService contentService, IMediaService mediaService, ILogger logger)
        {
            _mediaRemoveService = mediaRemoveService;
            _nexuService = nexuService;
            _entityParsingService = entityParsingService;
            _contentService = contentService;
            _mediaService = mediaService;
            _logger = logger;
            
            _mediaRemoveContext = MediaRemoveContext.Current;
        }

        [HttpGet]
        public IActionResult GetUnusedMedia()
        {
            var backgroundGetMedia = new Thread(FindUnusedMedia)
            {
                IsBackground = true,
                Name = "MediaRemove GetUnusedMedia"
            };
            backgroundGetMedia.Start();

            return Ok();
        }

        [HttpGet]
        public IActionResult GetUnusedMediaStatus()
        {
            return Ok(new
            {
                _mediaRemoveContext.IsProcessingMedia,
                Data = _mediaRemoveContext.UnusedMedia.Select(x => x.Model),
                TotalCount = _mediaRemoveContext.TotalAmountOfMedia
            });
        }

        [HttpGet]
        public IActionResult DeleteUnusedMediaStatus()
        {
            return Ok(new
            {
                _mediaRemoveContext.IsProcessingDeleting,
                ItemsToProcess = _mediaRemoveContext.ItemsToProcessDeleting,
                ItemsProcessed = _mediaRemoveContext.DeletedItemsProcessed
            });
        }

        [HttpPost]
        public IActionResult DeleteUnusedMedia(int[] ids)
        {
            var backgroundDelete = new Thread(DeleteMedia)
            {
                IsBackground = true,
                Name = "MediaRemove DeleteMedia"
            };
            backgroundDelete.Start(ids);

            return Ok();
        }

        [HttpGet]
        public IActionResult IsBuilt()
        {
            return Ok(new
            {
                IsBuilt = _mediaRemoveService.IsBuilt()
            });
        }

        [HttpGet]
        public IActionResult GetRebuildStatus()
        {
            return Ok(new 
            {
                NexuContext.Current.IsProcessing,
                NexuContext.Current.ItemsProcessed,
                ItemName = NexuContext.Current.ItemInProgress
            });
        }

        [HttpGet]
        public IActionResult Rebuild()
        {
            ThreadPool.QueueUserWorkItem(RebuildJob);

            return Ok();
        }

        public void RebuildJob(object state)
        {
            try
            {
                NexuContext.Current.IsProcessing = true;

                var rootLevelItems = _contentService.GetRootContent().ToList();

                foreach (var item in rootLevelItems)
                {
                    ParseContent(item);
                }

                long totalRecords = 0;
                rootLevelItems = _contentService.GetPagedChildren(
                    Umbraco.Cms.Core.Constants.System.RecycleBinContent,
                    0,
                    int.MaxValue,
                    out totalRecords).ToList();

                foreach (var item in rootLevelItems)
                {
                    ParseContent(item);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("An unhandled exception occurred while parsing content", ex);
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
            _entityParsingService.ParseContent(item);
            NexuContext.Current.ItemsProcessed++;

            var children = _contentService.GetPagedChildren(item.Id, 0, int.MaxValue, out _).ToList();

            foreach (var child in children)
            {
                // parse the content
                ParseContent(child);
            }
        }

        private void GetUnusedMediaItems(MediaItemWrapper root)
        {
            var children = _mediaService.GetPagedChildren(root.Media.Id, 0, int.MaxValue, out _).ToList();

            var found = false;
            _mediaRemoveContext.TotalAmountOfMedia += children.Count;
            foreach (var mediaItem in children)
            {
                found = true;
                GetUnusedMediaItems(new MediaItemWrapper(mediaItem, root));
            }

            if (found)
            {
                return;
            }

            var relations = _nexuService.GetRelationsForItem(root.Media.GetUdi());
            if (!relations.Any())
                _mediaRemoveContext.UnusedMedia.Add(root);
        }

        internal void FindUnusedMedia()
        {
            _mediaRemoveContext.UnusedMedia = new ConcurrentBag<MediaItemWrapper>();
            _mediaRemoveContext.IsProcessingMedia = true;
            _mediaRemoveContext.TotalAmountOfMedia = 0;
            var mediaItems = _mediaService.GetRootMedia();
            _mediaRemoveContext.TotalAmountOfMedia += mediaItems.Count();

            foreach (var mediaItem in mediaItems)
            {
                GetUnusedMediaItems(new MediaItemWrapper(mediaItem));
            }
            _mediaRemoveContext.IsProcessingMedia = false;
        }

        private void DeleteMedia(object packed)
        {
            var ids = (int[])packed;

            _mediaRemoveContext.IsProcessingDeleting = true;
            _mediaRemoveContext.ItemsToProcessDeleting = ids.Length;
            _mediaRemoveContext.DeletedItemsProcessed = 0;
            _mediaRemoveContext.TotalAmountOfMedia = 0;

            foreach (var id in ids)
            {
                var media = _mediaService.GetById(id);
                if (media == null) continue;

                _mediaService.Delete(media);
                _mediaRemoveContext.DeletedItemsProcessed++;
            }

            _mediaRemoveContext.IsProcessingDeleting = false;
            _mediaRemoveContext.UnusedMedia = new ConcurrentBag<MediaItemWrapper>();
        }
    }
}
