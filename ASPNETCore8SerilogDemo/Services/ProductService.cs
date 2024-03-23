namespace ASPNETCore8SerilogDemo.Services
{
    using ASPNETCore8Filter.Models;
    using ASPNETCore8Filter.Services;
    using ASPNETCore8Filter.Utilities;
    using ASPNETCore8SerilogDemo.Models;

    public class ProductService : BaseService<ProductService>, IProductService
    {
        public ProductService(IHttpContextAccessor httpContextAccessor, ILoggingService<ProductService> loggingService)
            : base(httpContextAccessor, loggingService)
        {
        }

        public int Create(ProductViewModel product)
        {
            _loggingService.LogInformation("產品建立完成");
            return 1;
        }

        public int OccursOutRangeException(ProductViewModel product)
        {
            // _loggingService.LogError(new MyOutRangeException("Product price out of range"), "Error creating product");
            throw new MyOutRangeException("產品單價");
        }
    }
}
