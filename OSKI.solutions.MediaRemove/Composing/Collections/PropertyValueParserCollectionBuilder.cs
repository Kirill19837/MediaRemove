using MediaRemove.Interfaces.Nexu;
using Umbraco.Cms.Core.Composing;

namespace MediaRemove.Composing.Collections
{
    public class PropertyValueParserCollectionBuilder : OrderedCollectionBuilderBase<PropertyValueParserCollectionBuilder, PropertyValueParserCollection, IPropertyValueParser>
    {
        /// <inheritdoc />
        protected override PropertyValueParserCollectionBuilder This => this;
    }
}
