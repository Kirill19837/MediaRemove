﻿using OSKI.solutions.MediaRemove.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKI.solutions.MediaRemove
{
    public class MediaRemoveContext
    {
        private static MediaRemoveContext instance;
        public MediaRemoveContext()
        {
            this.UnusedMedia = new ConcurrentBag<MediaItemWrapper>();
            this.IsProcessingMedia = false;
            instance = this;
        }

        public static MediaRemoveContext Current => instance ?? new MediaRemoveContext();
        public ConcurrentBag<MediaItemWrapper> UnusedMedia { get; set; }
        public bool IsProcessingMedia { get; set; }
        public bool IsProcessingDeleting { get; set; }
        public int ItemsToProcessDeleting { get; set; }
        public int DeletedItemsProcessed { get; set; }
        public int TotalAmountOfMedia { get; set; }
    }
}
