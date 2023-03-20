namespace MediaRemove.Parsers.Core
{
    public class MultiNodeTreePickerParser : BaseTextParser
    {
        /// <inheritdoc />
        public override bool IsParserFor(string propertyEditorAlias)
        {
            return propertyEditorAlias.Equals(Umbraco.Cms.Core.Constants.PropertyEditors.Aliases.MultiNodeTreePicker);
        }       
    }
}
