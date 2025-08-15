using MediaRemove.Constants;
using System.IO;
using System.Text.Json;
using Umbraco.Cms.Core.Models;

namespace MediaRemove.Models
{
    public class MediaItemWrapper
    {
        public IMedia Media { get; set; }
        public UnusedMedia Model { get; set; }
        public MediaItemWrapper(IMedia media, MediaItemWrapper previous)
        {
            var source = GetMediaSource(media);

            Media = media;
            Model = new UnusedMedia
            {
                Name = media.Name,
                Path = $"{previous.Model.Path}/{media.Name}",
                Id = media.Id,
                Source = source,
                MediaType = GetMediaType(media, source),
                BackofficeLink = GetBackOfficeLink(media.Id)
            };
        }

        public MediaItemWrapper(IMedia media)
        {
            var source = GetMediaSource(media);

            Media = media;
            Model = new UnusedMedia
            {
                Name = media.Name,
                Path = $"{media.Name}",
                Source = source,
                MediaType = GetMediaType(media, source),
                BackofficeLink = GetBackOfficeLink(media.Id)
            };
        }

        private string GetMediaSource(IMedia media)
        {
            if (!media.HasProperty("umbracoFile")) return null;

            var rawValue = media.GetValue<string>("umbracoFile");

            if (IsJson(rawValue))
            {
                try
                {
                    using var jsonDoc = JsonDocument.Parse(rawValue);
                    if (jsonDoc.RootElement.TryGetProperty(PluginConstants.Props.MediaSourceProp, out var src)) return src.GetString();
                }
                catch { /* fallback to raw string */ }
            }

            return rawValue;
        }

        private string GetMediaType(IMedia media, string source)
        {
            if (media == null) return null;

            var contentType = media.ContentType.Alias;

            if (!string.IsNullOrWhiteSpace(source))
            {
                var ext = Path.GetExtension(source)?.TrimStart('.').ToLower();
                if (!string.IsNullOrEmpty(ext))
                {
                    return $"{contentType} / .{ext}";
                }
            }

            return contentType;
        }

        private string GetBackOfficeLink(int mediaId)
        {
            return string.Format(PluginConstants.MediaEditDirectLinkTemplate, mediaId);
        }

        private bool IsJson(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue)) return false;

            rawValue = rawValue.Trim();

            try
            {
                using var doc = JsonDocument.Parse(rawValue);
                return true;
            }
            catch { return false; }
        }
    }
}
