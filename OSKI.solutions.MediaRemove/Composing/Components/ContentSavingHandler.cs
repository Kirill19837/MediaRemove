using System.Diagnostics;
using MediaRemove.Interfaces.Nexu;
using NPoco.fastJSON;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace MediaRemove.Composing.Components
{
    internal class ContentSavingHandler : INotificationHandler<ContentSavingNotification>
    {
        private readonly IEntityParsingService _entityParsingService;

        public ContentSavingHandler(IEntityParsingService entityParsingService)
        {
            _entityParsingService = entityParsingService;
        }

        public void Handle(ContentSavingNotification notification)
        {
            Debug.WriteLine(JSON.ToJSON(notification.SavedEntities));
            foreach (var contentItem in notification.SavedEntities)
            {
                _entityParsingService.ParseContent(contentItem);
            }
        }
    }
}
