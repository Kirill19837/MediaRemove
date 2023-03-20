using System.Collections.Generic;
using MediaRemove.Models.Nexu;
using Umbraco.Cms.Core;

namespace MediaRemove.Interfaces.Nexu
{
    public interface IEntityRelationService
    {
        IList<NexuRelationDisplayModel> GetRelationsForItem(Udi udi);

        IList<NexuRelationDisplayModel> GetUsedItemsFromList(IList<Udi> udis);

        bool CheckLinksInDescendants(GuidUdi rootId);
    }
}
