using System;
using NPoco;
using Umbraco.Cms.Core;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace MediaRemove.Models.Nexu
{
    [TableName(DatabaseConstants.TableName)]
    [PrimaryKey(DatabaseConstants.IdColumn, AutoIncrement = false)]
    public class NexuRelation
    {
        private Udi _udi;

        public NexuRelation()
        {
            Id = Guid.NewGuid();
        }

        [Column(DatabaseConstants.IdColumn)]
        [PrimaryKeyColumn(AutoIncrement = false, Clustered = false, Name = DatabaseConstants.PrimaryKey)]
        public Guid Id { get; set; }

        [Column(DatabaseConstants.ParentUdiColumn)]
        public string ParentUdi { get; set; }

        [Column(DatabaseConstants.ChildUdiColumn)]
        public string ChildUdi { get; set; }

        [Column(DatabaseConstants.RelationTypeColumn)]
        public Guid RelationType { get; set; }

        [Column(DatabaseConstants.PropertyAlias)]
        public string PropertyAlias { get; set; }

        [Column(DatabaseConstants.CultureColumn)]
        public string Culture { get; set; }

        [Ignore]
        public Udi Udi
        {
            get
            {
                if (_udi != null)
                {
                    return _udi;
                }

                _udi = new GuidUdi("nexurelation", Id);

                return _udi;
            }
        }
    }
}
