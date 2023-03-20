using Umbraco.Cms.Core.Models;

namespace MediaRemove.Interfaces.Nexu
{
    public interface IEntityParsingService
    {
        void ParseContent(IContent content);
    }
}
