namespace ASPNETCore8SerilogDemo.Services
{
    using ASPNETCore8SerilogDemo.Models;

    public interface IProductService
    {
        int Create(ProductViewModel product);
        int OccursOutRangeException(ProductViewModel product);
    }
}
