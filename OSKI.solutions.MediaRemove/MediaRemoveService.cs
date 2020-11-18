using Our.Umbraco.Nexu.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
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

        public static MediaRemoveService Current => service ?? new MediaRemoveService(ApplicationContext.Current.Services.RelationService);

        public bool IsBuilt()
        {
            var docToDoc = relationService.GetRelationTypeByAlias(RelationTypes.DocumentToDocumentAlias);
            var docToMedia = relationService.GetRelationTypeByAlias(RelationTypes.DocumentToMediaAlias);
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
