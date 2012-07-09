using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OctoPart
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

            string octopartQueryURL = "http://octopart.com/api/v2/parts/search?q=" + HttpUtility.UrlEncode(query) + "&apikey=a59f7ef3";

            string jsonData = client.DownloadString(octopartQueryURL);
            lastObject = JObject.Parse(jsonData);
            return lastObject;
        }

        private JToken pickBestPart(JObject queryResults)
        {

            return queryResults.SelectToken("");
        }

        private JToken GetPartAttribute(JToken parent, string fieldname)
        {
            foreach (JToken child in parent.Children())
            {
                JToken attributeToken = child.SelectToken("attribute.fieldname");
                if (attributeToken.Value<string>() == fieldname)
                {
                    return child.SelectToken("values[0]");
                }
            }

            return null;
        }

        public string GetDescription(string query)
        {
            JObject part = performQuery(query);
            JToken descriptionToken = part.SelectToken("results[0].item.short_description.text");
            string description = null;
            if (descriptionToken != null)
                description = descriptionToken.Value<string>();

            int shortest = int.MaxValue;
            if ((description == null) || (description.Length > 64))
            {
                JToken descriptionsToken = part.SelectToken("results[0].item.descriptions");
                foreach (JToken child in descriptionsToken.Children())
                {
                    string thisDesc = child.SelectToken("text").Value<string>();
                    if (thisDesc.Length > 16) // Minimum description length?
                        if (thisDesc.Length < shortest)
                        {
                            shortest = thisDesc.Length;
                            description = thisDesc;
                        }
                }
            }
            return description;
        }

        public int? GetPinCount(string query)
        {
            JObject part = performQuery(query);
            JToken specsToken = part.SelectToken("results[0].item.specs"); // [attribute.fieldname = number_of_pins].values[0]
            JToken pinCountToken = GetPartAttribute(specsToken, "number_of_pins");
            if (pinCountToken != null)
                return pinCountToken.Value<int?>();
            return 0;
        }

        public string GetPackage(string query)
        {

            JObject part = performQuery(query);
            JToken specsToken = part.SelectToken("results[0].item.specs"); // [attribute.fieldname = case_package].values[0]
            JToken packageToken = GetPartAttribute(specsToken, "case_package");
            if (packageToken != null)
                return packageToken.Value<string>();
            return "";
        }

        public string GetMountingType(string query)
        {

            JObject part = performQuery(query);
            JToken specsToken = part.SelectToken("results[0].item.specs"); // [attribute.fieldname = mounting_type].values[0]
            JToken mountingTypeToken = GetPartAttribute(specsToken, "mounting_type");
            if (mountingTypeToken != null)
                return mountingTypeToken.Value<string>();
            return "";
        }

        public float? GetAveragePrice(string query)
        {
            JObject part = performQuery(query);
            JToken avgPriceToken = part.SelectToken("results[0].item.avg_price[0]");
            float? avgPrice = avgPriceToken.Value<float?>();
            return avgPrice;
        }

        public string GetManufacturer(string query)
        {
            JObject part = performQuery(query);
            JToken manufacturerToken = part.SelectToken("results[0].item.manufacturer.displayname");
            string manufacturer = manufacturerToken.Value<string>();
            return manufacturer;
        }
    }
}
