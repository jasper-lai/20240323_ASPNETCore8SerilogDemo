namespace ASPNETCore8Filter.Utilities
{
    using ASPNETCore8Filter.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using System.Reflection.Metadata.Ecma335;

    public class LoggingService<T> : ILoggingService<T>
    {
        private readonly ILogger<T> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public LoggingService(ILogger<T> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 取得 HttpContext 的重要欄位至 MyProblemDetails
        /// </summary>
        /// <returns></returns>
        private MyProblemDetails SetProblemDetailsFromHttpContext()
        {
            var context = _httpContextAccessor.HttpContext;
            var traceId = context?.TraceIdentifier;
            string? controllerName = null;
            string? actionName = null;
            var endpoint = context?.GetEndpoint();
            if (endpoint != null)
            {
                var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

                if (actionDescriptor != null)
                {
                    controllerName = actionDescriptor.ControllerTypeInfo.FullName;
                    actionName = actionDescriptor.ActionName;
                }
            }

            var result = new MyProblemDetails()
            {
                TraceId = traceId,
                ControllerName = controllerName,
                ActionName = actionName,
                Instance = context?.Request.Path
            };

            return result;
        }


        /// <summary>
        /// Log Information
        /// </summary>
        /// <param name="message"></param>
        /// <param name="structuredData"></param>
        /// <remarks>
        /// 呼叫方式:
        /// (1) _loggingService.LogInformation($"處理結果: {result}"); 
        /// :: structuredData is null 
        /// :: 程式會自行補上 MyProblemDetails
        /// (2) _loggingService.LogInformation("產品清單: {@products}", products);
        /// :: structuredData is not null (且不是 MyProblemDetails) 
        /// :: 程式會自行補上 MyProblemDetails
        /// (3) _loggingService.LogInformation("資訊 {@unifiedOutput}", response);
        /// :: structuredData is not null (且是 MyProblemDetails) 
        /// :: 程式不會自行補上 MyProblemDetails
        /// </remarks>
        public void LogInformation(string message, object? structuredData = null)
        {
            var problemDetails = SetProblemDetailsFromHttpContext();
            problemDetails.Status = 200;
            problemDetails.Title = "OK";
            problemDetails.Detail = message;
            problemDetails.UserId = "jasper";

            if (structuredData == null)
            {
                _logger.LogInformation("{Message}: {@unifiedOutput}", message, problemDetails);
            }
            else
            {
                if (structuredData.GetType() == typeof(MyProblemDetails))
                {
                    _logger.LogInformation("{Message}: {@unifiedOutput}", message, structuredData);
                }
                else
                {
                    _logger.LogInformation("{Message}: {@structuredData}: {@unifiedOutput}", message, structuredData, problemDetails);
                }
            }
        }

        /// <summary>
        /// Log Warning
        /// </summary>
        /// <param name="message"></param>
        /// <param name="structuredData"></param>
        /// <remarks>
        /// 呼叫方式:
        /// (1) _loggingService.LogWarning($"處理結果: {result}"); 
        /// :: structuredData is null 
        /// :: 程式會自行補上 MyProblemDetails
        /// (2) _loggingService.LogWarning("產品清單: {@products}", products);
        /// :: structuredData is not null (且不是 MyProblemDetails) 
        /// :: 程式會自行補上 MyProblemDetails
        /// (3) _loggingService.LogWarning("警告 {@unifiedOutput}", response);
        /// :: structuredData is not null (且是 MyProblemDetails) 
        /// :: 程式不會自行補上 MyProblemDetails
        /// </remarks>
        public void LogWarning(string message, object? structuredData = null)
        {
            var problemDetails = SetProblemDetailsFromHttpContext();
            problemDetails.Status = 400;
            problemDetails.Title = "BadRequest";
            problemDetails.Detail = message;
            problemDetails.UserId = "jasper";

            if (structuredData == null)
            {
                _logger.LogInformation("{Message}: {@unifiedOutput}", message, problemDetails);
            }
            else
            {
                if (structuredData.GetType() == typeof(MyProblemDetails))
                {
                    _logger.LogWarning("{Message}: {@unifiedOutput}", message, structuredData);
                }
                else
                {
                    _logger.LogWarning("{Message}: {@structuredData}: {@unifiedOutput}", message, structuredData, problemDetails);
                }
            }
        }


        /// <summary>
        /// Log Error
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="structuredData"></param>
        /// <remarks>
        /// 呼叫方式:
        /// (1) _loggingService.LogError(exception, $"處理結果: {result}"); 
        /// :: structuredData is null 
        /// :: 程式會自行補上 MyProblemDetails
        /// (2) _loggingService.LogError(exception, "產品清單: {@products}", products);
        /// :: structuredData is not null (且不是 MyProblemDetails) 
        /// :: 程式會自行補上 MyProblemDetails
        /// (3) _loggingService.LogError(exception, "錯誤 {@unifiedOutput}", response);
        /// :: structuredData is not null (且是 MyProblemDetails) 
        /// :: 程式不會自行補上 MyProblemDetails
        /// </remarks>
        public void LogError(Exception exception, string message, object?    structuredData = null)
        {
            var problemDetails = SetProblemDetailsFromHttpContext();
            problemDetails.Status = 500;
            problemDetails.Title = "InternalServerError";
            problemDetails.Detail = message;
            problemDetails.UserId = "jasper";

            if (structuredData == null)
            {
                _logger.LogError(exception, "{Message}: {@unifiedOutput}", message, problemDetails);
            }
            else
            {
                if (structuredData.GetType() == typeof(MyProblemDetails))
                {
                    _logger.LogError(exception, "{Message}: {@unifiedOutput}", message, structuredData);
                }
                else
                {
                    _logger.LogError(exception, "{Message}: {@structuredData}: {@unifiedOutput}", message, structuredData, problemDetails);
                }
            }
        }
    }
}
