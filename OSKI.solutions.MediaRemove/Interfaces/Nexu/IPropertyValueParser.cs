using System.Collections.Generic;
using Umbraco.Cms.Core.Composing;

namespace MediaRemove.Interfaces.Nexu
{
    public interface IPropertyValueParser : IDiscoverable
    {
        bool IsParserFor(string propertyEditorAlias);

        IEnumerable<IRelatedEntity> GetRelatedEntities(string value);
    }
}
