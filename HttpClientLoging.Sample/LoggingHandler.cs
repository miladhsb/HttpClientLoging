using Microsoft.Extensions.DependencyInjection;

namespace HttpClientLoging.Sample
{
    public class LoggingHandler : DelegatingHandler
    {
     
        private readonly ILogger<LoggingHandler> _logger;
       
        public LoggingHandler(ILogger<LoggingHandler> logger)
        {
            _logger = logger;
        }


      

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Microsoft.Extensions.Http.Logging.LoggingHttpMessageHandler

            //if (InnerHandler == null)
            //{
            //    InnerHandler = new HttpClientHandler();
            //}


            _logger.LogInformation("Request:");
            //_logger.LogInformation(request.ToString());
            if (request.Content != null)
            {
                _logger.LogInformation(await request.Content.ReadAsStringAsync());
            }
           

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            _logger.LogInformation("Response:");
            //_logger.LogInformation(response.ToString());
            //if (response.Content != null)
            //{
            //    _logger.LogInformation(await response.Content.ReadAsStringAsync());
            //}
          

            return response;
        }
    }
}
