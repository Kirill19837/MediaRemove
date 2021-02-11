using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Web.JavaScript;

namespace OSKI.solutions.MediaRemove
{
    internal class ServerRoutes : IComponent
    {
        public void Initialize()
        {
            ServerVariablesParser.Parsing += this.ServerVariablesParserParsing;
        }

        public void Terminate()
        {
            ServerVariablesParser.Parsing -= this.ServerVariablesParserParsing;
        }

        private void ServerVariablesParserParsing(object sender, Dictionary<string, object> e)
        {
            if (HttpContext.Current == null)
            {
                return;
            }

            var urlHelper = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));

            var urlDictionairy = new Dictionary<string, object>
            {
                { "GetUnusedMedia", urlHelper.GetUmbracoApiService<MediaRemoveController>("GetUnusedMedia", null) },
                { "DeleteUnusedMedia", urlHelper.GetUmbracoApiService<MediaRemoveController>("DeleteUnusedMedia", null) },
                { "GetUnusedMediaStatus", urlHelper.GetUmbracoApiService<MediaRemoveController>("GetUnusedMediaStatus", null) },
                { "IsBuilt", urlHelper.GetUmbracoApiService<MediaRemoveController>("IsBuilt", null) },
                { "DeleteUnusedMediaStatus", urlHelper.GetUmbracoApiService<MediaRemoveController>("DeleteUnusedMediaStatus", null) },
                { "RebuildApi", urlHelper.GetUmbracoApiServiceBaseUrl<MediaRemoveController>(c => c.GetRebuildStatus()) }
            };
            if (!e.Keys.Contains("MediaRemove"))
            {
                e.Add("MediaRemove", urlDictionairy);
            }
        }
    }
}
