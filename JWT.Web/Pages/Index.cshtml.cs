using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace JWT.Web.Pages
{
    public class IndexModel : PageModel
    {
        private HttpClient client = new HttpClient();
        private string baseUrl = string.Empty;
        public IndexModel(IConfiguration configuration)
        {
            baseUrl = configuration["WebAPIUrl"];

        }
        public void OnGet()
        {
            t();
        }

        private void t()
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Task<HttpResponseMessage> response = client.PostAsJsonAsync("api/login", new { Username = "ArdaCetinkaya", Password = "SomeFancySecurePassword" });
            Task<string> tokenResult = response.Result.Content.ReadAsAsync<string>();

            dynamic token = JsonConvert.DeserializeObject(tokenResult.Result);
            //Call protected "Get" action using the token from above
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Token);
            response = client.GetAsync("api/somebusiness");

            //Write result from protected action
            Task<string> values = response.Result.Content.ReadAsStringAsync();

        }
    }
}
