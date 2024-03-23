namespace ASPNETCore8Filter.Services
{
    using ASPNETCore8Filter.Utilities;

    public abstract class BaseService<T>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ILoggingService<T> _loggingService;

        public BaseService(IHttpContextAccessor httpContextAccessor, ILoggingService<T> loggingService)
        {
            _httpContextAccessor = httpContextAccessor;
            _loggingService = loggingService;
        }
    }
}
