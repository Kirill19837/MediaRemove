using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;

namespace OSKI.solutions.MediaRemove.Models
{
    public class MediaItemWrapper
    {
        public IMedia Media { get; set; }
        public UnusedMedia Model { get; set; }

        public MediaItemWrapper(IMedia media, MediaItemWrapper previous)
        {
            Media = media;
            Model = new UnusedMedia
            {
                Name = media.Name,
                Path = $"{previous.Model.Path}/{media.Name}",
                Id = media.Id,
                Source = media.GetValue<string>("umbracoFile")
            };
        }

        public MediaItemWrapper(IMedia media)
        {
            Media = media;
            Model = new UnusedMedia
            {
                Name = media.Name,
                Path = $"{media.Name}",
                Id = media.Id,
                Source = media.GetValue<string>("umbracoFile")
            };
        }
    }
}
