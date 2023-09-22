namespace $rootnamespace$.Areas.Admin
{
    using System;
    using System.Web;
    using System.Web.Mvc;

    using Elmah;

    internal class ElmahResult : ActionResult
    {
        private readonly string resouceType;

        public ElmahResult()
            : this(null)
        {
        }

        public ElmahResult(string resouceType)
        {
            this.resouceType = resouceType;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var factory = new ErrorLogPageFactory();

            if (!string.IsNullOrEmpty(this.resouceType))
            {
                var pathInfo = "/" + this.resouceType;
                context.HttpContext.RewritePath(this.FilePath(context), pathInfo, context.HttpContext.Request.QueryString.ToString());
            }

            var currentContext = GetCurrentContextAsHttpContext(context);

            var httpHandler = factory.GetHandler(currentContext, null, null, null);
            var httpAsyncHandler = httpHandler as IHttpAsyncHandler;

            if (httpAsyncHandler != null)
            {
                httpAsyncHandler.BeginProcessRequest(currentContext, r => { }, null);
                return;
            }

            httpHandler.ProcessRequest(currentContext);
        }

        private static HttpContext GetCurrentContextAsHttpContext(ControllerContext context)
        {
            return context.HttpContext.ApplicationInstance.Context;
        }

        private string FilePath(ControllerContext context)
        {
            return this.resouceType != "stylesheet" ? context.HttpContext.Request.Path.Replace(string.Format("/{0}", this.resouceType), string.Empty) : context.HttpContext.Request.Path;
        }
    }
}