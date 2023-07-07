using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Net.NetworkInformation;

namespace HttpClientLoging.Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class httpClientTest : ControllerBase
    {
       

      
        
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly LoggingHandler _loggingHandler;

        public httpClientTest(IHttpClientFactory httpClientFactory, LoggingHandler loggingHandler)
        {
         
   
            this._httpClientFactory = httpClientFactory;
            this._loggingHandler = loggingHandler;
        }



        [HttpGet("TestHttpCientFactory")]
        public async Task<IActionResult> TestHttpCientFactory()
        {
            var _httpClient = _httpClientFactory.CreateClient("TestClient");
            var res = await _httpClient.GetAsync("https://www.google.com/");

            return Ok("ok");
        }

        //تزریق _loggingHandler به صورت پایپ لاین به http client
        [HttpGet("TestHttpCientNewInstance")]
        public async Task<IActionResult> TestHttpCientNewInstance()
        {

           // var client = new HttpClient(_loggingHandler);

            var client = new HttpClient(CustomeHttpClientFactory.CreatePipeline(new HttpClientHandler(),new DelegatingHandler[] { _loggingHandler }));
            client.DefaultRequestHeaders.Add("User-Agent", "TestApp");


            var res = await client.GetAsync("https://www.google.com/");

            return Ok("ok");
        }

        //تزریق Httpclient factory به رست شارپ
        [HttpGet("TestRestSharpHttpCientFactory")]
        public async Task<IActionResult> TestRestSharpHttpCientFactory()
        {
            //  IRestClient client = new RestClient(options);
            var _httpClient = _httpClientFactory.CreateClient("TestClient");
            var client = new RestClient(_httpClient);

            var request = new RestRequest("https://www.google.com/", Method.Get);
            var response = client.Execute(request);

            return Ok("ok");
        }
        [HttpGet("TestRestSharp")]
        public async Task<IActionResult> TestRestSharp()
        {
            //دادن logginghandler  به صورت مستقیم به رست شارپ
            //var options = new RestClientOptions("https://www.google.com/")
            //{

            //    ConfigureMessageHandler = p =>
            //    {
            //        p = _loggingHandler;
            //        return p;
            //    }

            //};

            //or

            //دادن  _loggingHandler به صورت ایجاد پایپ بایت به رست شارپ
            var options = new RestClientOptions("https://www.google.com/")
            {

                ConfigureMessageHandler = p =>
                {
                    p = CustomeHttpClientFactory.CreatePipeline(new HttpClientHandler(), new DelegatingHandler[] { _loggingHandler });
                    return p;
                }

            };

            IRestClient client = new RestClient(options);

            var request = new RestRequest("", Method.Get);
            var response =await client.ExecuteAsync(request);
            return Ok("ok");
        }

        //در این تست به دلیل استفاده عادی از رست شارپ پورتهاس زیادی اشغال میشود
        [HttpGet("PortCountRestSharp")]
        public async Task<IActionResult> PortCountRestSharp()
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();


            
            for (int i = 0; i < 100; i++)
            {
                
                var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
                var client = new RestClient("https://www.google.com/");

                var request = new RestRequest("",Method.Get);
                var response = client.Execute(request);
                Console.WriteLine(tcpConnInfoArray.Count());
                await  Task.Delay(100);
            }

            return Ok();
        }

        //در این تست به دلیل تزریق httpclient factory  در رست شارپ پورتها مدیریت میشود
        [HttpGet("PortCountRestSharpCientFactory")]
        public async Task<IActionResult> PortCountRestSharpCientFactory()
 
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();


            for (int i = 0; i < 100; i++)
            {
                var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
                var _httpClient = _httpClientFactory.CreateClient("TestClient");
                var client = new RestClient(_httpClient);

                var request = new RestRequest("https://www.google.com/", Method.Get);
                var response = client.Execute(request);
                Console.WriteLine(tcpConnInfoArray.Count());
                await Task.Delay(10);
            }

            return Ok();
        }


        //در این تست به جهت استفاده از HttpClient factory پورتها مدیریت میشود
        [HttpGet("PortCountHttpCientFactory")]
        public async Task<IActionResult> PortCountHttpCientFactory()

        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();


            for (int i = 0; i < 100; i++)
            {
                var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
                var _httpClient = _httpClientFactory.CreateClient("TestClient");
                var res = await _httpClient.GetAsync("https://www.google.com/");

                Console.WriteLine(tcpConnInfoArray.Count());
                await Task.Delay(10);
            }

            return Ok();
        }

        //در این تست چون از http client  نمونه جدید ساخته میشود پورتهای زیادی اشغال میشود
        [HttpGet("PortCountHttpCientNewInstance")]
        public async Task<IActionResult> PortCountHttpCientNewInstance()

        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

           // var client = new HttpClient(CustomeHttpClientFactory.CreatePipeline(new HttpClientHandler(), new DelegatingHandler[] { _loggingHandler }));
            for (int i = 0; i < 100; i++)
            {
                var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

                //دریافت نمونه جدید از loggingHandler به ازای هر اسکوپ
                using var ServiceScope = HttpContext.RequestServices.CreateScope();
                var NewloggingHandler = ServiceScope.ServiceProvider.GetRequiredService<LoggingHandler>();
                var client = new HttpClient(CustomeHttpClientFactory.CreatePipeline(new HttpClientHandler(), new DelegatingHandler[] { NewloggingHandler }));
             
                
                client.DefaultRequestHeaders.Add("User-Agent", "TestApp");


                var res = await client.GetAsync("https://www.google.com/");

                Console.WriteLine(tcpConnInfoArray.Count());
                await Task.Delay(10);
            }

            return Ok();
        }


        //در ایتفاده از HttpClientFactory حتی اگر نمونه جدید هم ایجاد شود باز هم پورتها را مدیریت میکند
        [HttpGet("PortCountHttpCientFactoryNewScope")]
        public async Task<IActionResult> PortCountHttpCientFactoryNewScope()

        {

         
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

           

            for (int i = 0; i < 100; i++)
            {
                using var ServiceScope = HttpContext.RequestServices.CreateScope();
                var httpClientfactory=  ServiceScope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                var cLient= httpClientfactory.CreateClient("TestClient");

                var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

                var res = await cLient.GetAsync("https://www.google.com/");

                Console.WriteLine(tcpConnInfoArray.Count());
                await Task.Delay(10);
            }

            return Ok();
        }
    }
}