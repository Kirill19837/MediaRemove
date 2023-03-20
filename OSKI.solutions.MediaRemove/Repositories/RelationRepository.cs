using System.Collections.Generic;
using System.Linq;
using MediaRemove.Interfaces.Nexu;
using MediaRemove.Models.Nexu;
using NPoco;
using Umbraco.Cms.Core;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace MediaRemove.Repositories
{
    public class RelationRepository : IRelationRepository
    {
        private readonly IScopeProvider _scopeProvider;

        public RelationRepository(IScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public void PersistRelationsForContentItem(Udi contentItemUdi, IEnumerable<NexuRelation> relations)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete:true);
            using var transaction = scope.Database.GetTransaction();
            var db = scope.Database;

            var udiString = contentItemUdi.ToString();

            var deleteSql = new Sql<ISqlContext>(scope.SqlContext);
            deleteSql.Where<NexuRelation>(x => x.ParentUdi == udiString);

            db.Delete<NexuRelation>(deleteSql);

            db.BulkInsertRecords(relations);

            transaction.Complete();
        }

        public IEnumerable<NexuRelation> GetIncomingRelationsForItem(Udi udi)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var db = scope.Database;

            var udiString = udi.ToString();

            var sql = new Sql<ISqlContext>(scope.SqlContext);
            sql.Where<NexuRelation>(x => x.ChildUdi == udiString);

            return db.Fetch<NexuRelation>(sql);
        }

        public IList<KeyValuePair<string,string>> GetUsedItemsFromList(IList<Udi> udis)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var db = scope.Database;

            var udiStrings = udis.Select(x => x.ToString()).ToList();

            var sql = new Sql<ISqlContext>(scope.SqlContext);
            sql.Where<NexuRelation>(x => udiStrings.Contains(x.ChildUdi));

            var relations = db.Fetch<NexuRelation>(sql).ToList();

            return relations.Select(rel => new KeyValuePair<string, string>(rel.ChildUdi, rel.Culture)).ToList();

        }
    }
}
