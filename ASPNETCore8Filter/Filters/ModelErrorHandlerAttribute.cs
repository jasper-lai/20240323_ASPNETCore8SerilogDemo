namespace ASPNETCore8Filter.Filters
{
    using ASPNETCore8Filter.Models;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class ModelErrorHandlerAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                // 統一集中例外處理
                var description = ModelErrorToString(context.ModelState);
                throw new MyClientException(description);
            }
        }

        /// <summary>
        /// 將未通過 Model Validation 資料檢核的清單, 轉為字串
        /// </summary>
        /// <returns></returns>
        private static string ModelErrorToString(ModelStateDictionary modelState)
        {
            // Your implementation to convert model state errors to a single string
            var errors = modelState.Values.SelectMany(v => v.Errors)
               .Select(e => e.ErrorMessage)
               .ToList();
            var description = string.Join(Environment.NewLine, errors);
            description = "輸入的資料有誤: " + Environment.NewLine + description;
            return description;
        }

    }
}
