using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoPart.Model
{
    public class ParameterFilter
    {
        public string query  { get; set; }
        public string apiKey { get; set; }
        public string start  { get; set; }
        public string limit  { get; set; }

        //TODO: Criar modelo todos parametros
        //https://octopart.com/api/docs/v3/rest-api#response-schemas-searchrequest
    }
}
