using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.UI.JavaScript;

namespace OSKI.solutions.MediaRemove
{
    internal class UmbracoEventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            using (ApplicationContext.Current.ProfilingLogger.TraceDuration<UmbracoEventHandler>("Begin ApplicationStarted", "End ApplicationStarted"))
            {
                // setup server variables
                ServerVariablesParser.Parsing += this.ServerVariablesParserParsing;
            }
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
                {"DeleteUnusedMediaStatus", urlHelper.GetUmbracoApiService<MediaRemoveController>("DeleteUnusedMediaStatus", null) }
            };

            if (!e.Keys.Contains("MediaRemove"))
            {
                e.Add("MediaRemove", urlDictionairy);
            }
        }
    }
}
