using System;
using System.Collections.Generic;
using MediaRemove.Interfaces.Nexu;
using Umbraco.Cms.Core.Composing;

namespace MediaRemove.Composing.Collections
{
    public class PropertyValueParserCollection : BuilderCollectionBase<IPropertyValueParser>
    {
        public PropertyValueParserCollection(Func<IEnumerable<IPropertyValueParser>> items)
            : base(items)
        {
        }
    }
}
