using System.Collections.Concurrent;
using MediaRemove.Models;

namespace MediaRemove
{
    public class MediaRemoveContext
    {
        private static MediaRemoveContext _instance;
        public MediaRemoveContext()
        {
            UnusedMedia = new ConcurrentBag<MediaItemWrapper>();
            IsProcessingMedia = false;
            _instance = this;
        }

        public static MediaRemoveContext Current => _instance ?? new MediaRemoveContext();
        public ConcurrentBag<MediaItemWrapper> UnusedMedia { get; set; }
        public bool IsProcessingMedia { get; set; }
        public bool IsProcessingDeleting { get; set; }
        public int ItemsToProcessDeleting { get; set; }
        public int DeletedItemsProcessed { get; set; }
        public int TotalAmountOfMedia { get; set; }
    }
}
