using System.Configuration;
using MediaRemove.Constants;
using Umbraco.Extensions;

namespace MediaRemove
{
    public class NexuContext
    {
        private static NexuContext _instance;

        private static readonly object Padlock = new();

        private bool _isProcessing;

        private string _itemInProgress;

        private int _itemsProcessed;

        private NexuContext()
        {
            _isProcessing = false;
            _itemInProgress = string.Empty;
            _itemsProcessed = 0;
            PreventDelete = GetAppSetting(AppSettings.PreventDelete, false);
            PreventUnPublish = GetAppSetting(AppSettings.PreventUnpublish, false);
            _instance = this;
        }

        public static NexuContext Current
        {
            get
            {
                lock (Padlock)
                {
                    return _instance ??= new NexuContext();
                }
            }
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

        public bool PreventDelete { get; }

        public bool PreventUnPublish { get; set; }

        private static T GetAppSetting<T>(string key, T defaultValue)
        {
            var value = defaultValue;

            var setting = ConfigurationManager.AppSettings[key];

            if (setting == null) return value;

            var attempConvert = setting.TryConvertTo<T>();

            if (attempConvert.Success) value = attempConvert.Result;

            return value;
        }
    }
}
