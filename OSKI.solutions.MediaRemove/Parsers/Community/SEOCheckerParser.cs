using MediaRemove.Interfaces.Nexu;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MediaRemove.Models.Nexu;

namespace MediaRemove.Parsers.Community
{
    public class SEOCheckerParser : IPropertyValueParser
    {
        /// <inheritdoc />
        public bool IsParserFor(string propertyEditorAlias)
        {
            return propertyEditorAlias.Equals("SEOChecker.SEOCheckerSocialPropertyEditor");
        }

        /// <inheritdoc />
        public IEnumerable<IRelatedEntity> GetRelatedEntities(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var entities = new List<IRelatedEntity>();
                var model = XDocument.Parse(value).Root;
                if (model != null)
                {
                    try
                    {
                        var socialImage = model.Descendants("socialImage").FirstOrDefault();
                        if (socialImage == null)
                        {
                            return Enumerable.Empty<IRelatedEntity>();
                        }

                        if (string.IsNullOrEmpty(socialImage.Value))
                        {
                            return Enumerable.Empty<IRelatedEntity>();
                        }

                        foreach (var documentUdi in ParserUtilities.GetMediaUdiFromText(socialImage.Value).ToList())
                        {
                            entities.Add(new RelatedMediaEntity
                            {
                                RelatedEntityUdi = documentUdi,
                            });
                        }

                        return entities;
                    }
                    catch { }
                }
            }

            return Enumerable.Empty<IRelatedEntity>();
        }
    }
}
