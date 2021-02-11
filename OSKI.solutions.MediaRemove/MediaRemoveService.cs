using Our.Umbraco.Nexu.Common.Constants;
using Umbraco.Core.Services;

namespace OSKI.solutions.MediaRemove
{
    public class MediaRemoveService
    {
        private static readonly MediaRemoveService service;
        private readonly IRelationService relationService;

        public MediaRemoveService(IRelationService relationService)
        {
            this.relationService = relationService;
        }

        public bool IsBuilt()
        {
            var docToDoc = relationService.GetRelationTypeById(RelationTypes.DocumentToDocument);
            var docToMedia = relationService.GetRelationTypeById(RelationTypes.DocumentToMedia);
            if (docToDoc == null || docToMedia == null)
            {
                return false;
            }
            var built = relationService.HasRelations(docToDoc);
            built = built || relationService.HasRelations(docToMedia);
            return built;
        }
    }
}
