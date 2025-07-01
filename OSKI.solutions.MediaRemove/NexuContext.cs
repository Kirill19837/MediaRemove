using Microsoft.Extensions.Options;
using OSKI.solutions.MediaRemove.Models.Nexu;

namespace MediaRemove
{
    public class NexuContext
    {
        private readonly NexuSettings _settings;

        private static readonly object Padlock = new();

        private bool _isProcessing;

        private string _itemInProgress;

        private int _itemsProcessed;

        public NexuContext(IOptions<NexuSettings> options)
        {
            _settings = options.Value;
            _isProcessing = false;
            _itemInProgress = string.Empty;
            _itemsProcessed = 0;
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                lock (Padlock)
                {
                    _isProcessing = value;
                }
            }
        }

        public string ItemInProgress
        {
            get => _itemInProgress;
            set
            {
                lock (Padlock)
                {
                    _itemInProgress = value;
                }
            }
        }

        public int ItemsProcessed
        {
            get => _itemsProcessed;
            set
            {
                lock (Padlock)
                {
                    _itemsProcessed = value;
                }
            }
        }

        public bool PreventDelete => _settings.PreventDelete;

        public bool PreventUnPublish => _settings.PreventUnpublish;
    }
}
