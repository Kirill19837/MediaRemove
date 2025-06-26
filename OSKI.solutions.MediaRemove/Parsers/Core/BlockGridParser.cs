using MediaRemove.Parsers;

namespace OSKI.solutions.MediaRemove.Parsers.Core
{
    public class BlockGridParser : BaseTextParser
    {
        public override bool IsParserFor(string propertyEditorAlias)
        {
            return propertyEditorAlias.Equals(Umbraco.Cms.Core.Constants.PropertyEditors.Aliases.BlockGrid);
        }
    }
}
