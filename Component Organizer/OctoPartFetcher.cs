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

        internal int? GetPinCount(string query)
        {
            return 0;
            JObject part = performQuery(query);
            JToken pinCountToken = part.SelectToken("results[0].items[0].specs.attribute[attribute.fieldname = number_of_pins].values[0]");
            int? pinCount = pinCountToken.Value<int?>();
            return pinCount;
        }

        internal string GetPackage(string query)
        {
            return "";
            JObject part = performQuery(query);
            JToken packageToken = part.SelectToken("results[0].items[0].specs.attribute[attribute.fieldname = case_package].values[0]");
            string package = packageToken.Value<string>();
            return package;
        }

        internal float? GetAveragePrice(string query)
        {
            return 0;
            throw new NotImplementedException();
        }

        public string GetManufacturer(string query)
        {
            JObject part = performQuery(query);
            JToken manufacturerToken = part.SelectToken("results[0].items[0].manufacturer.displayname");
            string manufacturer = manufacturerToken.Value<string>();
            return manufacturer;
        }
    }
}
