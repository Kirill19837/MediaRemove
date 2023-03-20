using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;

namespace MediaRemove
{
    internal class ServerRoutes : INotificationHandler<ServerVariablesParsingNotification>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;

        public ServerRoutes(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
        {
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
        }

        public void Handle(ServerVariablesParsingNotification notification)
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return;
            }

            var urlDictionary = new Dictionary<string, object>
            {
                {
                    "GetUnusedMedia",
                    _linkGenerator.GetUmbracoApiService<MediaRemoveController>(
                        nameof(MediaRemoveController.GetUnusedMedia))
                },
                {
                    "DeleteUnusedMedia",
                    _linkGenerator.GetUmbracoApiService<MediaRemoveController>(
                        nameof(MediaRemoveController.DeleteUnusedMedia))
                },
                {
                    "GetUnusedMediaStatus",
                    _linkGenerator.GetUmbracoApiService<MediaRemoveController>(
                        nameof(MediaRemoveController.GetUnusedMediaStatus))
                },
                {
                    "IsBuilt",
                    _linkGenerator.GetUmbracoApiService<MediaRemoveController>(nameof(MediaRemoveController.IsBuilt))
                },
                {
                    "DeleteUnusedMediaStatus",
                    _linkGenerator.GetUmbracoApiService<MediaRemoveController>(
                        nameof(MediaRemoveController.DeleteUnusedMediaStatus))
                },
                {
                    "RebuildApi",
                    _linkGenerator.GetPathByAction(action: nameof(MediaRemoveController.GetRebuildStatus), controller: "MediaRemove")
                }
            };
            if (!notification.ServerVariables.ContainsKey("MediaRemove"))
            {
                notification.ServerVariables.Add("MediaRemove", urlDictionary);
            }
        }
    }
}
