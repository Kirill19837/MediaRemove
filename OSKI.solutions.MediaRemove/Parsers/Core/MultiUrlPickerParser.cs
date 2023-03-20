namespace MediaRemove.Parsers.Core
{
    public class MultiUrlPickerParser : BaseTextParser
    {
        /// <inheritdoc />
        public override bool IsParserFor(string propertyEditorAlias)
        {
            return propertyEditorAlias.Equals(Umbraco.Cms.Core.Constants.PropertyEditors.Aliases.MultiUrlPicker);
        }
    }
}
