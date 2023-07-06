using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace HttpClientLoging.Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class httpClientTest : ControllerBase
    {
       

      
        private readonly HttpClient _httpClient;
        private readonly LoggingHandler _loggingHandler;

        public httpClientTest(IHttpClientFactory httpClientFactory, LoggingHandler loggingHandler)
        {
         
            this._httpClient = httpClientFactory.CreateClient("TestClient");
            this._loggingHandler = loggingHandler;
        }

        [HttpGet("TestHttpCientFactory")]
        public async Task<IActionResult> TestHttpCientFactory()
        {

            var res = await _httpClient.GetAsync("https://www.google.com/");

            return Ok("ok");
        }


        [HttpGet("TestHttpCientNewInstance")]
        public async Task<IActionResult> TestHttpCientNewInstance()
        {

            var client = new HttpClient(_loggingHandler);
            client.DefaultRequestHeaders.Add("User-Agent", "TestApp");


            var res = await client.GetAsync("https://www.google.com/");

            return Ok("ok");
        }

        [HttpGet("TestRestSharpHttpCientFactory")]
        public async Task<IActionResult> TestRestSharpHttpCientFactory()
        {
            //  IRestClient client = new RestClient(options);
            var client = new RestClient(_httpClient);

            var request = new RestRequest("https://www.google.com/", Method.Get);
            var response = client.Execute(request);

            return Ok("ok");
        }
        [HttpGet("TestRestSharp")]
        public async Task<IActionResult> TestRestSharp()
        {

            var options = new RestClientOptions("https://www.google.com/")
            {

                ConfigureMessageHandler = p =>
                {
                    p = _loggingHandler;
                    return p;
                }

            };


            IRestClient client = new RestClient(options);

            var request = new RestRequest("", Method.Get);
            var response =await client.ExecuteAsync(request);
            return Ok("ok");
        }
    }
}