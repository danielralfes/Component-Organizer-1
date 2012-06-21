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

            lastQuery = query;

            string octopartQueryURL = "http://octopart.com/api/v2/bom/match?lines=[{%22q%22%3A+%22" + query + "%22}]";

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

        internal JToken GetPartAttribute(JToken parent, string fieldname)
        {
            foreach (JToken child in parent.Children())
            {
                JToken attributeToken = child.SelectToken("attribute.fieldname");
                if (attributeToken.Value<string>() == "fieldname")
                {
                    return child.SelectToken("values[0]");
                }
            }

            return null;
        }

        internal int? GetPinCount(string query)
        {
            JObject part = performQuery(query);
            JToken specsToken = part.SelectToken("results[0].items[0].specs"); // [attribute.fieldname = number_of_pins].values[0]
            JToken pinCountToken = GetPartAttribute(specsToken, "number_of_pins");
            if (pinCountToken != null)
                return pinCountToken.Value<int?>();
            return 0;
        }

        internal string GetPackage(string query)
        {

            JObject part = performQuery(query);
            JToken specsToken = part.SelectToken("results[0].items[0].specs"); // [attribute.fieldname = case_package].values[0]
            JToken packageToken = GetPartAttribute(specsToken, "case_package");
            if (packageToken != null)
                return packageToken.Value<string>();
            return "";
        }

        internal string GetMountingType(string query)
        {

            JObject part = performQuery(query);
            JToken specsToken = part.SelectToken("results[0].items[0].specs"); // [attribute.fieldname = mounting_type].values[0]
            JToken mountingTypeToken = GetPartAttribute(specsToken, "mounting_type");
            if (mountingTypeToken != null)
                return mountingTypeToken.Value<string>();
            return "";
        }

        internal float? GetAveragePrice(string query)
        {
            JObject part = performQuery(query);
            JToken avgPriceToken = part.SelectToken("results[0].items[0].avg_price[0]");
            float? avgPrice = avgPriceToken.Value<float?>();
            return avgPrice;
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
