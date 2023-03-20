using System;
using MediaRemove.Constants;
using MediaRemove.Interfaces.Nexu;
using Umbraco.Cms.Core;

namespace MediaRemove.Models.Nexu
{
    public class RelatedDocumentEntity : IRelatedEntity
    {
        /// <inheritdoc />
        public Udi RelatedEntityUdi { get; set; }

        /// <inheritdoc />
        public Guid RelationType => RelationTypes.DocumentToDocument;       
    }
}
