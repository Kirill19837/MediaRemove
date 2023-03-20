using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MediaRemove.Composing.Collections;
using MediaRemove.Interfaces.Nexu;
using MediaRemove.Models.Nexu;
using NPoco.fastJSON;
using Serilog;
using Umbraco.Cms.Core.Models;
using Umbraco.Extensions;

namespace MediaRemove.Services.Nexu
{
    public class EntityParsingService : IEntityParsingService
    {
        private readonly PropertyValueParserCollection _propertyValueParserCollection;

        private readonly ILogger _logger;

        private readonly IRelationRepository _relationRepository;

        public EntityParsingService(PropertyValueParserCollection propertyValueParserCollection, ILogger logger, IRelationRepository relationRepository)
        {
            _propertyValueParserCollection = propertyValueParserCollection;
            _logger = logger;
            _relationRepository = relationRepository;
        }        
        
        public void ParseContent(IContent content)
        {
            Debug.WriteLine(JSON.ToJSON(content), "ParseContent - content");
            if (content.Blueprint)
            {
                return;
            }

            var relations = new List<NexuRelation>();

            try
            {
                relations.AddRange(GetRelatedEntitiesFromContent(content));
                Debug.WriteLine(JSON.ToJSON(relations), "ParseContent - relations");
            }
            catch (Exception ex)
            {
                _logger.Error($"Something went wrong parsing content with id {content.Id}", ex);
                return;
            }

            try
            {
                SaveRelationsForContentItem(content, relations);
            }
            catch (Exception ex)
            {
                _logger.Error($"Something went wrong saving relations for content with id {content.Id}", ex);
            }
        }

        public virtual IPropertyValueParser GetParserForPropertyEditor(string propertyEditorAlias)
        {
            return _propertyValueParserCollection.FirstOrDefault(x => x.IsParserFor(propertyEditorAlias));
        }

        public virtual IEnumerable<IRelatedEntity> GetRelatedEntitiesFromPropertyEditorValue(string propertyEditorAlias, object propertyValue)
        {
            if (string.IsNullOrWhiteSpace(propertyValue?.ToString())) return Enumerable.Empty<IRelatedEntity>();

            var parser = GetParserForPropertyEditor(propertyEditorAlias);

            return parser != null ? parser.GetRelatedEntities(propertyValue.ToString()).DistinctBy(x => x.RelatedEntityUdi.ToString()) : Enumerable.Empty<IRelatedEntity>();
        }

        public virtual IDictionary<string, IEnumerable<IRelatedEntity>> GetRelatedEntitiesFromProperty(IProperty property)
        {
            var relationsByCulture = new Dictionary<string, IEnumerable<IRelatedEntity>>();

            var editorAlias = property.PropertyType.PropertyEditorAlias;

            foreach (var cultureValue in property.Values)
            {
                var entities = GetRelatedEntitiesFromPropertyEditorValue(
                    editorAlias,
                    cultureValue.EditedValue).ToList();

                if (entities.Any())
                {
                    relationsByCulture.Add(cultureValue.Culture ?? "invariant", entities);
                }                
            }

            return relationsByCulture;
        }

        public virtual IEnumerable<NexuRelation> GetRelatedEntitiesFromContent(IContent content)
        {
            var entities = new List<NexuRelation>();

            foreach (var prop in content.Properties.ToList())
            {
                var relatedEntities = GetRelatedEntitiesFromProperty(prop);

                foreach (var language in relatedEntities.Keys.ToList())
                {
                    entities.AddRange(relatedEntities[language]
                    .ToList()
                    .Select(entity => new NexuRelation
                    {
                        ParentUdi = content.GetUdi().ToString(),
                        ChildUdi = entity.RelatedEntityUdi.ToString(),
                        RelationType = entity.RelationType,
                        Culture = language,
                        PropertyAlias = prop.Alias
                    }));
                }
            }

            return entities;
        }

        public virtual void SaveRelationsForContentItem(IContent content, IEnumerable<NexuRelation> relations)
        {
            var relationList = relations.ToList();
            Debug.WriteLine(JSON.ToJSON(content), "SaveRelationsForContentItem - relationList");
            foreach (var relation in relationList.Where(relation => relation.Culture == "invariant"))
            {
                relation.Culture = null;
            }

            _relationRepository.PersistRelationsForContentItem(content.GetUdi(), relationList);
        }
    }
}
