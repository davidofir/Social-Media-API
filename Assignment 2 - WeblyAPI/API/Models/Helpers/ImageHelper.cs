using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;

namespace API.Models.Helpers
{
    public static class ImageHelper
    {
        public static IEnumerable<string> GetTags(string imageUrl)
        {
            string apiKey = "acc_cb5b8ba463d08bb";
            string apiSecret = "fbe920ade3ea685ca2e3f979b2b25885";
            string basicAuthValue = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(String.Format("{0}:{1}", apiKey, apiSecret)));
            var client = new RestClient("https://api.imagga.com/v2/tags");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddParameter("image_url", imageUrl);
            request.AddParameter("limit", "5");
            request.AddHeader("Authorization", String.Format("Basic {0}", basicAuthValue));
            IRestResponse response = client.Execute(request);
            string[] lines = response.Content.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<string> results = lines.Where(l => l.Contains("\"tag\""));
            var result = new List<string>();
            foreach (var line in results)
                yield return line.Split(':')[2].Replace("}", string.Empty).Replace("\"", string.Empty);
        }
    }
}