using nanoFramework.WebServer;
using System;
using System.Net;
using System.Text;

namespace DemoWebServer.Controller
{
    [Authentication("Basic:user password")]
    class ControllerAuth
    {
        [Route("authbasic")]
        public void Basic(WebServerEventArgs e)
        {
            WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.OK);
        }

        [Route("authbasicspecial")]
        [Authentication("Basic:user2 password")]
        public void Special(WebServerEventArgs e)
        {
            WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.OK);
        }

        [Authentication("ApiKey:superKey1234")]
        [Route("authapi")]
        public void Key(WebServerEventArgs e)
        {
            WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.OK);
        }

        [Route("authnone")]
        [Authentication("None")]
        public void None(WebServerEventArgs e)
        {
            WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.OK);
        }

        [Authentication("ApiKey")]
        [Route("authdefaultapi")]
        public void DefaultApi(WebServerEventArgs e)
        {
            WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.OK);
        }
    }
}
