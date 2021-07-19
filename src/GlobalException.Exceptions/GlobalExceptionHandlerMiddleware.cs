using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace GlobalException.Exceptions
{
    public class GlobalExceptionHandlerMiddleware : IMiddleware
    {
        //private readonly IMediatorHandler _mediatorHandler;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            //_mediatorHandler = mediatorHandler;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            //var obj = new EventException(exception);
            //_mediatorHandler.RaiseEvent(obj);

            var json = new
            {
                context.Response.StatusCode,
                //Message = string.Format("Ops, ocorreu um erro enesperado, entre em contato com o suporte e informe o código do erro: {0}", obj.AggregateId),
                Message = "An error occurred whilst processing your request",
                Detailed = exception
            };

            return context.Response.WriteAsync(JsonConvert.SerializeObject(json));
        }
    }

}
