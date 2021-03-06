﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi.Core.Injectinvi;
using Tweetinvi.Models;
using Tweetinvi.Streaming.Webhooks;
using Tweetinvi.Streams;

namespace Tweetinvi.AspNet
{
    public class WebhookMiddlewareMessageHandler : DelegatingHandler
    {
        private readonly IAccountActivityRequestHandler _accountActivityRequestHandler;

        public WebhookMiddlewareMessageHandler(IAccountActivityRequestHandler accountActivityRequestHandler)
        {
            _accountActivityRequestHandler = accountActivityRequestHandler;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestHandler = new WebhooksRequestHandlerForWebApi(request);

            if (await _accountActivityRequestHandler.IsRequestManagedByTweetinviAsync(requestHandler).ConfigureAwait(false))
            {
                var routeHandled = await _accountActivityRequestHandler.TryRouteRequestAsync(requestHandler).ConfigureAwait(false);
                if (routeHandled)
                {
                    var response = requestHandler.GetHttpResponseMessage();

                    var tsc = new TaskCompletionSource<HttpResponseMessage>();
                    tsc.SetResult(response);

                    return await tsc.Task.ConfigureAwait(false);
                }
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

    }
}