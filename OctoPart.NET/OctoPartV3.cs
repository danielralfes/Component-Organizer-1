using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;



namespace OctoPart
{
    public class OctoPartFetcherV3
    {
        public OctoPartFetcherV3()
        {
        }
        // TODO: Create a OctoPartFetcher.FillInInfo(Part p) method
        /*
        Description    
        Pins           
        Package        
        Price          
        ManufacturerURL 
        */


        ////string lastQuery;
        ////JObject lastObject;
        ////private JObject performQuery(string query)
        ////{
        ////    if (lastObject != null)
        ////        if (query.Equals(lastQuery))
        ////            return lastObject;

        ////    lastQuery = query;

        ////    string octopartQueryURL = "http://octopart.com/api/v3/parts/search?q=" + HttpUtility.UrlEncode(query) + "&apikey=69d07868";

        ////    string jsonData = client.DownloadString(octopartQueryURL);
        ////    lastObject = JObject.Parse(jsonData);
        ////    return lastObject;
        ////}

        //private JToken pickBestPart(JObject queryResults)
        //{
        //    return queryResults.SelectToken("");
        //}

        /////
        public void BOMMatching()
        {
            // -- your search query --
            var query = new List<dynamic>()
            {
                new Dictionary<string, string>()
                { { "mpn", "SN74S74N" },
                  { "reference", "line1" } },
                new Dictionary<string, string>()
                { { "sku", "67K1122" },
                  { "reference", "line2" } },
                new Dictionary<string, string>()
                { { "mpn_or_sku", "SN74S74N" },
                  { "reference", "line3" } },
                new Dictionary<string, string>()
                { { "brand", "Texas Instruments" },
                  { "mpn", "SN74S74N" },
                  { "reference", "line4" } }
            };

            string octopartUrlBase = "http://octopart.com/api/v3";
            string octopartUrlEndpoint = "parts/match";
            string apiKey = APIKEY;

            // Create the search request
            string queryString = (new JavaScriptSerializer()).Serialize(query);
            var client = new RestClient(octopartUrlBase);
            var req = new RestRequest(octopartUrlEndpoint, Method.GET)
                        .AddParameter("apikey", apiKey)
                        .AddParameter("queries", queryString);

            // Perform the search and obtain results
            var data = client.Execute(req).Content;
            var response = JsonConvert.DeserializeObject<dynamic>(data);

            // Print request time (in milliseconds)
            Console.WriteLine(response["msec"]);

            // Print mpn's
            foreach (var result in response["results"])
            {
                Console.WriteLine("Reference: " + result["reference"]);
                foreach (var item in result["items"])
                {
                    Console.WriteLine(item["mpn"]);
                }
            }
        }

        public void ParametricSearch()
        {
            // -- your search query --
            string query = "solid state relay";

            string octopartUrlBase = "http://octopart.com/api/v3";
            string octopartUrlEndpoint = "parts/search";
            string apiKey = APIKEY;

            // Create the search request
            var client = new RestClient(octopartUrlBase);
            var req = new RestRequest(octopartUrlEndpoint, Method.GET)
                        .AddParameter("apikey", apiKey)
                        .AddParameter("q", query)
                        .AddParameter("start", "0")
                        .AddParameter("limit", "10");

            // Perform the search and obtain results
            var data = client.Execute(req).Content;
            var search_response = JsonConvert.DeserializeObject<dynamic>(data);
            // Print number of hits
            Console.WriteLine(search_response["hits"]);
            // Print results
            foreach (var result in search_response["results"])
            {
                var part = result["item"];

                // Print matched part
                Console.WriteLine(part["brand"]["name"] + " - " + part["mpn"]);
            }
        }


        // -- your API key -- (https://octopart.com/api/register)
        private const string APIKEY = "69d07868";
    }
}
