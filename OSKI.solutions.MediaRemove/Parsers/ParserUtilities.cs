using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Umbraco.Cms.Core;

namespace MediaRemove.Parsers
{
    internal static class ParserUtilities
    {
        public static IEnumerable<Udi> GetDocumentUdiFromText(string text)
        {
            return GetUdiFromText(text, "umb://document/(.{32})");
        }

        public static IEnumerable<Udi> GetMediaUdiFromText(string text)
        {
            return GetUdiFromText(text, "umb://media/(.{32})");
        }

        private static IEnumerable<Udi> GetUdiFromText(string text, string regex)
        {
            var udiList = new List<Udi>();

            var udiMatches = Regex.Matches(text, regex);

            foreach (Match match in udiMatches)
            {
                udiList.Add(new StringUdi(new Uri(match.Value)));
            }

            return udiList.DistinctBy(x => x.ToString());
        }
    }
}
