namespace MediaRemove.Parsers.Core
{
    public class NestedContentParser : BaseTextParser
    {
        public override bool IsParserFor(string propertyEditorAlias)
        {
            return propertyEditorAlias.Equals(Umbraco.Cms.Core.Constants.PropertyEditors.Aliases.NestedContent);
        }      
    }
}
