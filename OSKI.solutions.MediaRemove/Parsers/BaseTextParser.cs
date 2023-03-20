using System.Collections.Generic;
using System.Linq;
using MediaRemove.Interfaces.Nexu;
using MediaRemove.Models.Nexu;

namespace MediaRemove.Parsers
{
    public abstract class BaseTextParser : IPropertyValueParser
    {
        public abstract bool IsParserFor(string propertyEditorAlias);

        public IEnumerable<IRelatedEntity> GetRelatedEntities(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Enumerable.Empty<IRelatedEntity>();
            }

            var relatedEntities = new List<IRelatedEntity>();

            foreach (var documentUdi in ParserUtilities.GetDocumentUdiFromText(value).ToList())
            {
                relatedEntities.Add(new RelatedDocumentEntity
                {
                    RelatedEntityUdi = documentUdi
                });
            }

            foreach (var mediaUdi in ParserUtilities.GetMediaUdiFromText(value).ToList())
            {
                relatedEntities.Add(new RelatedMediaEntity()
                {
                    RelatedEntityUdi = mediaUdi
                });
            }

            return relatedEntities;
        }
    }
}
