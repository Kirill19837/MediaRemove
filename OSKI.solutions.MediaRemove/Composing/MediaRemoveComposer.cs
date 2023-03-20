using MediaRemove.Composing.Collections;
using MediaRemove.Composing.Components;
using MediaRemove.Interfaces;
using MediaRemove.Interfaces.Nexu;
using MediaRemove.Repositories;
using MediaRemove.Services;
using MediaRemove.Services.Nexu;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace MediaRemove.Composing
{
    public class MediaRemoveComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient<IRelationRepository, RelationRepository>();
            builder.Services.AddTransient<IMediaRemoveService, MediaRemoveService>();
            builder.Services.AddTransient<IEntityParsingService, EntityParsingService>();
            builder.Services.AddTransient<IEntityRelationService, EntityRelationService>();
            builder.WithCollectionBuilder<PropertyValueParserCollectionBuilder>().Append(
                builder.TypeLoader.GetTypes<IPropertyValueParser>());

            builder.AddComponent<MigrationComponent>();
            builder.AddNotificationHandler<ContentSavingNotification, ContentSavingHandler>();
        }
    }
}
