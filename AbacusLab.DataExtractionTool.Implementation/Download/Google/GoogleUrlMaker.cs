using System;

namespace AbacusLab.DataExtractionTool.Implementation.Download.Google
{
    internal class GoogleUrlMaker
    {
        public string PrepareUrl(string searchText, bool isFullTextSearch, string apiKey, string nextPageToken = "")
        {
            var searchFormate = isFullTextSearch ? "query={0}" : "reference={0}";
            const string searchUrlFormate = @"https://maps.googleapis.com/maps/api/place/{0}/json?{1}&sensor=false&key={2}";
            if (isFullTextSearch)
                searchText = searchText.Replace(" ", "+");
            searchFormate = string.Format(searchFormate, searchText);
            var requestUrl = string.Format(searchUrlFormate, isFullTextSearch ? "textsearch" : "details", searchFormate, apiKey);
            if (String.IsNullOrEmpty(nextPageToken)) return requestUrl;
            var nextPageTokenFormate = String.Format("&pagetoken={0}", nextPageToken);
            requestUrl = requestUrl + nextPageTokenFormate;
            return requestUrl;
        }
    }
}
