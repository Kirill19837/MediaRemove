using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MediaRemove.Interfaces.Nexu;
using MediaRemove.Models.Nexu;
using NPoco.fastJSON;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace MediaRemove.Services.Nexu
{
    public class EntityRelationService : IEntityRelationService
    {
        private readonly IRelationRepository _relationRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IContentService _contentService;
        private readonly IContentTypeService _contentTypeService;
        private readonly IMediaService _mediaService;

        public EntityRelationService(IRelationRepository relationRepository, ILocalizationService localizationService, IContentService contentService, IContentTypeService contentTypeService, IMediaService mediaService)
        {
            _relationRepository = relationRepository;
            _localizationService = localizationService;
            _contentService = contentService;
            _contentTypeService = contentTypeService;
            _mediaService = mediaService;
        }
        
         public IList<NexuRelationDisplayModel> GetRelationsForItem(Udi udi)
        {
            var nexuRelationDisplayModels = new List<NexuRelationDisplayModel>();

            var contentTypes = new Dictionary<int, IContentType>();

            var relations = _relationRepository.GetIncomingRelationsForItem(udi).ToList();

            Debug.WriteLine(JSON.ToJSON(relations), "relations");
            if (!relations.Any()) return nexuRelationDisplayModels;

            var defaultLanguage = _localizationService.GetDefaultLanguageIsoCode().ToLowerInvariant();

            var contentItems = _contentService
                .GetByIds(relations.Select(x => new GuidUdi(new Uri(x.ParentUdi))).Distinct()).ToList();

            Debug.WriteLine(JSON.ToJSON(contentItems), "contentItems");
            foreach (var relation in relations)
            {
                var culture = !string.IsNullOrWhiteSpace(relation.Culture) ? relation.Culture : defaultLanguage;
                var content = contentItems.Find(x => x.GetUdi().ToString() == relation.ParentUdi);

                if (content == null) continue;

                var contentName = content.Name;
                var published = content.Published;

                if (content.AvailableCultures.Any())
                {
                    contentName = content.GetCultureName(culture);
                    published = content.IsCulturePublished(culture);
                }

                var model = nexuRelationDisplayModels.FirstOrDefault(
                    x => x.Culture == culture && x.Id == content.Id);

                if (model == null)
                {
                    model = new NexuRelationDisplayModel
                    {
                        IsTrashed = content.Trashed,
                        Key = content.Key,
                        Id = content.Id,
                        Culture = culture,
                        IsPublished = published,
                        Name = contentName
                    };

                    nexuRelationDisplayModels.Add(model);
                }

                var prop = content.Properties.FirstOrDefault(x => x.Alias == relation.PropertyAlias);

                if (prop == null) continue;

                if (contentTypes.ContainsKey(content.ContentTypeId) == false)
                    contentTypes.Add(content.ContentTypeId, _contentTypeService.Get(content.ContentTypeId));

                var currentContentType = contentTypes[content.ContentTypeId];

                var tabName = currentContentType.CompositionPropertyGroups.FirstOrDefault(
                    cpg => cpg.PropertyTypes.Any(pt => pt.Alias == relation.PropertyAlias))?.Name;

                model.Properties.Add(new NexuRelationPropertyDisplay
                {
                    PropertyName = prop.PropertyType.Name,
                    TabName = tabName
                });
            }

            return nexuRelationDisplayModels;
        }

        /// <inheritdoc />
        public IList<NexuRelationDisplayModel> GetUsedItemsFromList(IList<Udi> udis)
        {
            var nexuRelationDisplayModels = new List<NexuRelationDisplayModel>();

            if (udis?.Any() != true)
            {
               return nexuRelationDisplayModels;
            }

            var relations = _relationRepository.GetUsedItemsFromList(udis);

            if (!relations.Any())
            {
                return nexuRelationDisplayModels;
            }
          
            foreach (var relation in relations.Distinct())
            {
                var guidUdi = UdiParser.Parse(relation.Key);

                switch (guidUdi.EntityType)
                {
                    case Umbraco.Cms.Core.Constants.UdiEntityType.Document:
                    {
                        var content = _contentService.GetById(guidUdi.AsGuid());

                        if (content == null) continue;

                        var contentName = content.Name;
                        var published = content.Published;

                        if (content.AvailableCultures.Any())
                        {
                            contentName = content.GetCultureName(relation.Value);
                            published = content.IsCulturePublished(relation.Value);
                        }

                        nexuRelationDisplayModels.Add(new NexuRelationDisplayModel
                        {
                            Culture = relation.Value,
                            Id = content.Id,
                            IsPublished = published,
                            IsTrashed = content.Trashed,
                            Key = content.Key,
                            Name = contentName
                        });
                        break;
                    }
                    case Umbraco.Cms.Core.Constants.UdiEntityType.Media:
                    {
                        var media = _mediaService.GetById(guidUdi.AsGuid());

                        if (media == null) continue;

                        if (nexuRelationDisplayModels.All(x => x.Key != media.Key))
                        {
                            nexuRelationDisplayModels.Add(new NexuRelationDisplayModel
                            {
                                Culture = string.Empty,
                                Id = media.Id,
                                IsPublished = false,
                                IsTrashed = media.Trashed,
                                Key = media.Key,
                                Name = media.Name
                            });
                        }

                        break;
                    }
                }
            }

            return nexuRelationDisplayModels;
        }

        public bool CheckLinksInDescendants(GuidUdi rootId)
        {
            return rootId.EntityType == "media" ? CheckMediaDescendants(rootId) : CheckContentDescendants(rootId);
        }

        private bool CheckContentDescendants(GuidUdi rootId)
        {
            var content = _contentService.GetById(rootId.Guid);

            if (content == null) return false;

            var descendants = _contentService.GetPagedDescendants(content.Id, 0, int.MaxValue, out _).ToList();

            if (!descendants.Any()) return false;

            var udis = descendants.Select(x => (Udi)x.GetUdi()).ToList();

            var relations = _relationRepository.GetUsedItemsFromList(udis);

            return relations.Any();
        }

        private bool CheckMediaDescendants(GuidUdi rootId)
        {
            var media = _mediaService.GetById(rootId.Guid);

            if (media == null) return false;

            var descendants = _mediaService.GetPagedDescendants(media.Id, 0, int.MaxValue, out _).ToList();

            if (!descendants.Any()) return false;

            var udis = descendants.Select(x => (Udi)x.GetUdi()).ToList();

            var relations = _relationRepository.GetUsedItemsFromList(udis);

            return relations.Any();
        }
    }
}
