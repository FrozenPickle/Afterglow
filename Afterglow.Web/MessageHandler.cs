using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking;
using Griffin.Networking.Http.Messages;
using System.Net;
using System.IO;
using System.Threading;

namespace Afterglow.Web
{
    public class MessageHandler : IUpstreamHandler
    {
        /// <summary>
        /// Handle a message
        /// </summary>
        /// <param name="context">Context unique for this handler instance</param>
        /// <param name="message">Message to process</param>
        /// <remarks>
        /// All messages that can't be handled MUST be send up the chain using <see cref="IPipelineHandlerContext.SendUpstream"/>.
        /// </remarks>
        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            var msg = message as ReceivedHttpRequest;
            if (msg == null)
                return;

            var request = msg.HttpRequest;
            var response = request.CreateResponse(HttpStatusCode.OK, "OK");

            // TODO: implement handler

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine("Welcome to Afterglow " + Thread.CurrentPrincipal.Identity.Name);
            writer.WriteLine("the time is: " + DateTime.Now);

            writer.Flush();

            stream.Position = 0;
            response.Body = stream;
            context.SendDownstream(new SendHttpResponse(request, response));
        }
    }

}
