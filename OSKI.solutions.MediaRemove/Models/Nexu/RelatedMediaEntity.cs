using System;
using MediaRemove.Constants;
using MediaRemove.Interfaces.Nexu;
using Umbraco.Cms.Core;

namespace MediaRemove.Models.Nexu
{
    public class RelatedMediaEntity : IRelatedEntity
    {
        public Udi RelatedEntityUdi { get; set; }

        public Guid RelationType => RelationTypes.DocumentToMedia;       
    }
}
