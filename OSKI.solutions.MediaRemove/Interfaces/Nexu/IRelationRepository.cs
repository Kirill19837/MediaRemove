using System.Collections.Generic;
using MediaRemove.Models.Nexu;
using Umbraco.Cms.Core;

namespace MediaRemove.Interfaces.Nexu
{
    public interface IRelationRepository
    {
        void PersistRelationsForContentItem(Udi contentItemUdi, IEnumerable<NexuRelation> relations);
        IEnumerable<NexuRelation> GetIncomingRelationsForItem(Udi udi);
        IList<KeyValuePair<string, string>> GetUsedItemsFromList(IList<Udi> udis);
    }
}
