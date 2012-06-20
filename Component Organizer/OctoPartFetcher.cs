using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Component_Organizer
{
    public class OctoPartFetcher
    {
        WebClient client;
        string lastQuery;
        JObject lastObject;

        public OctoPartFetcher()
        {
            client = new WebClient();
        }

        private JObject performQuery(string query)
        {
            if (lastObject != null)
                if (query.Equals(lastQuery))
                    return lastObject;

            string octopartQueryURL = "http://octopart.com/api/v2/bom/match?lines=[{%22q%22%3A+%22" + query + "%22}]" + "&pretty_print=true";

            string jsonData = client.DownloadString(octopartQueryURL);
            lastObject = JObject.Parse(jsonData);
            return lastObject;
        }
        
        public string GetDescription(string query)
        {
            JObject part = performQuery(query);
            JToken descriptionToken = part.SelectToken("results[0].items[0].short_description");
            string description = descriptionToken.Value<string>();
            return description;
        }
    }
}
