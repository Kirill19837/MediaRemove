using MediaRemove.Constants;
using MediaRemove.Interfaces;
using Umbraco.Cms.Core.Services;

namespace MediaRemove.Services
{

    public class MediaRemoveService : IMediaRemoveService
    {
        private readonly IRelationService _relationService;

        public MediaRemoveService(IRelationService relationService)
        {
            _relationService = relationService;
        }

        public bool IsBuilt()
        {
            var docToDoc = _relationService.GetRelationTypeById(RelationTypes.DocumentToDocument);
            var docToMedia = _relationService.GetRelationTypeById(RelationTypes.DocumentToMedia);
            if (docToDoc == null || docToMedia == null)
            {
                return false;
            }
            var built = _relationService.HasRelations(docToDoc);
            built = built || _relationService.HasRelations(docToMedia);
            return built;
        }
    }
}
