namespace HttpClientLoging.Sample
{
    public static class CustomeHttpClientFactory
    {
        //
        // Summary:
        //     Creates a new System.Net.Http.HttpClient instance configured with the handlers
        //     provided and with an System.Net.Http.HttpClientHandler as the innermost handler.
        //
        // Parameters:
        //   handlers:
        //     An ordered list of System.Net.Http.DelegatingHandler instances to be invoked
        //     as an System.Net.Http.HttpRequestMessage travels from the System.Net.Http.HttpClient
        //     to the network and an System.Net.Http.HttpResponseMessage travels from the network
        //     back to System.Net.Http.HttpClient. The handlers are invoked in a top-down fashion.
        //     That is, the first entry is invoked first for an outbound request message but
        //     last for an inbound response message.
        //
        // Returns:
        //     An System.Net.Http.HttpClient instance with the configured handlers.
        public static HttpClient Create(params DelegatingHandler[] handlers)
        {
            return Create(new HttpClientHandler(), handlers);
        }

        //
        // Summary:
        //     Creates a new System.Net.Http.HttpClient instance configured with the handlers
        //     provided and with the provided innerHandler as the innermost handler.
        //
        // Parameters:
        //   innerHandler:
        //     The inner handler represents the destination of the HTTP message channel.
        //
        //   handlers:
        //     An ordered list of System.Net.Http.DelegatingHandler instances to be invoked
        //     as an System.Net.Http.HttpRequestMessage travels from the System.Net.Http.HttpClient
        //     to the network and an System.Net.Http.HttpResponseMessage travels from the network
        //     back to System.Net.Http.HttpClient. The handlers are invoked in a top-down fashion.
        //     That is, the first entry is invoked first for an outbound request message but
        //     last for an inbound response message.
        //
        // Returns:
        //     An System.Net.Http.HttpClient instance with the configured handlers.
        public static HttpClient Create(HttpMessageHandler innerHandler, params DelegatingHandler[] handlers)
        {
            return new HttpClient(CreatePipeline(innerHandler, handlers));
        }

        //
        // Summary:
        //     Creates an instance of an System.Net.Http.HttpMessageHandler using the System.Net.Http.DelegatingHandler
        //     instances provided by handlers. The resulting pipeline can be used to manually
        //     create System.Net.Http.HttpClient or System.Net.Http.HttpMessageInvoker instances
        //     with customized message handlers.
        //
        // Parameters:
        //   innerHandler:
        //     The inner handler represents the destination of the HTTP message channel.
        //
        //   handlers:
        //     An ordered list of System.Net.Http.DelegatingHandler instances to be invoked
        //     as part of sending an System.Net.Http.HttpRequestMessage and receiving an System.Net.Http.HttpResponseMessage.
        //     The handlers are invoked in a top-down fashion. That is, the first entry is invoked
        //     first for an outbound request message but last for an inbound response message.
        //
        // Returns:
        //     The HTTP message channel.
        public static HttpMessageHandler CreatePipeline(HttpMessageHandler innerHandler, IEnumerable<DelegatingHandler> handlers)
        {
            if (innerHandler == null)
            {
                throw new ArgumentNullException("innerHandler");
            }

            if (handlers == null)
            {
                return innerHandler;
            }

            HttpMessageHandler httpMessageHandler = innerHandler;
            foreach (DelegatingHandler item in handlers.Reverse())
            {
                if (item == null)
                {
                    throw new ArgumentNullException($"not null handlers in {typeof(DelegatingHandler).Name}");
                   
                }

                if (item.InnerHandler != null)
                {
                    throw new ArgumentNullException($"InnerHandler Duplicate Instace {typeof(DelegatingHandler).Name}");
                   
                }

                item.InnerHandler = httpMessageHandler;
                httpMessageHandler = item;
            }

            return httpMessageHandler;
        }

    }
}
