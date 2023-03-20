using System;
using System.Collections.Generic;
using System.Linq;
using MediaRemove.Interfaces.Nexu;
using MediaRemove.Models.Nexu;
using Umbraco.Cms.Core;

namespace MediaRemove.Parsers.Core
{
    public class ContentPickerParser : IPropertyValueParser
    {
        public bool IsParserFor(string propertyEditorAlias)
        {
            return propertyEditorAlias.Equals(Umbraco.Cms.Core.Constants.PropertyEditors.Aliases.ContentPicker);
        }

        public IEnumerable<IRelatedEntity> GetRelatedEntities(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var relatedEntities = new List<IRelatedEntity>
                {
                    new RelatedDocumentEntity
                    {
                        RelatedEntityUdi = new StringUdi(new Uri(value))
                    }
                };

                return relatedEntities;
            }

            return Enumerable.Empty<IRelatedEntity>();
        }
    }
}
