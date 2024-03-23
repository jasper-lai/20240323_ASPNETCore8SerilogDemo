namespace ASPNETCore8Filter.Models
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// 自行定義的 ProblemDatails (衍生自 ProblemDetails)
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ProblemDetails" />
    public class MyProblemDetails : ProblemDetails
    {
        /// <summary>
        /// Http Request 的追蹤代號 (GUID)
        /// </summary>
        /// <value>
        /// The trace identifier.
        /// </value>
        public string? TraceId { get; set; } = string.Empty;

        /// <summary>
        /// Controller 全名
        /// </summary>
        /// <value>
        /// The name of the controller.
        /// </value>
        public string? ControllerName { get; set; }  = string.Empty;

        /// <summary>
        /// Action 名稱
        /// </summary>
        /// <value>
        /// The name of the action.
        /// </value>
        public string? ActionName { get; set; } = string.Empty;

        /// <summary>
        /// 使用者代碼
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public string? UserId { get; set; } = string.Empty;
    }
}
