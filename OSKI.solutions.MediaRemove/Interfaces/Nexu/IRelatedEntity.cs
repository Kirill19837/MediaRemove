using System;
using Umbraco.Cms.Core;

namespace MediaRemove.Interfaces.Nexu
{
    public interface IRelatedEntity
    {
        Udi RelatedEntityUdi { get; set; }

        Guid RelationType { get; }       
    }
}
