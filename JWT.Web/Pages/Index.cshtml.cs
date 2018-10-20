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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace JWT.Web.Pages
{
    public class IndexModel : PageModel
    {

        public class ResponseData
        {
            public string token { get; set; }

        }
        private HttpClient client = new HttpClient();
        private readonly ILogger<IndexModel> _logger;
        private string _baseUrl = string.Empty;

        public string SomeResultFromBusinessAction { get; set; }
        public IndexModel(IConfiguration configuration, ILogger<IndexModel> logger)
        {
            _baseUrl = configuration["WebAPIUrl"];
            _logger = logger;
        }
        public IActionResult OnGet()
        {
            return Page();
        }
        public async void OnPost(string username, string password)
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                client.BaseAddress = new Uri(_baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.PostAsJsonAsync("api/login", new { Username = username, Password = password });
                string tokenResult = await response.Content.ReadAsStringAsync();
                ResponseData tokenData = JsonConvert.DeserializeObject<ResponseData>(tokenResult);
                _logger.LogInformation($"TOKEN FROM API: {tokenData.token}");
                //For example you may keep it in a cookie
                //Now we have token let's do another web api call
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenData.token);

                //This time let's execute a GET method 
                response = await client.GetAsync("api/somebusiness/2018");

                SomeResultFromBusinessAction = await response.Content.ReadAsStringAsync();

                 _logger.LogInformation($"Business Action Result: {SomeResultFromBusinessAction}");
            }

       

        }
    }
}
