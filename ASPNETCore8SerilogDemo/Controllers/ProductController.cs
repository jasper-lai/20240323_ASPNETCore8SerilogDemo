namespace ASPNETCore8SerilogDemo.Controllers
{
    using ASPNETCore8Filter.Controllers;
    using ASPNETCore8Filter.Models;
    using ASPNETCore8Filter.Utilities;
    using ASPNETCore8SerilogDemo.Models;
    using ASPNETCore8SerilogDemo.Services;
    using Microsoft.AspNetCore.Mvc;

    public class ProductController : BaseController
    {
        private readonly IProductService _service;
        private readonly ILogger<ProductController> _logger;
        private readonly ILoggingService<ProductController> _loggingService;

        public ProductController(IProductService service, ILogger<ProductController> logger, ILoggingService<ProductController> loggingService)
        {
            _service = service;
            _logger = logger;
            _loggingService = loggingService;
        }

        //public ProductController(IProductService service, ILoggingService<ProductController> loggingService)
        //{
        //    _service = service;
        //    _loggingService = loggingService;
        //}


        [HttpGet]
        public IActionResult Index()
        {
            var products = new List<ProductViewModel>()
            {   new() {Id=1, Name="布丁", OrderQty=1, UnitPrice=50 },
                new() {Id=2, Name="蛋塔", OrderQty=1, UnitPrice=40 },
            };

            // 比較有沒有 @ 解構子的影響
            _logger.LogInformation("產品清單(_logger): {myId} {products}", "001", products);
            _logger.LogInformation("產品清單(_logger): {@myId} {@products}", "002", products);
            _loggingService.LogInformation("產品清單(_loggingService): {products}", products);
            _loggingService.LogInformation("產品清單(_loggingService): {@products}", products);
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var product = new ProductViewModel() { Id = 1, Name = "豆塔", OrderQty = 1, UnitPrice = 30 };
            return View(product);
        }

        [HttpPost]
        public IActionResult Create(ProductViewModel product)
        {
            var result = _service.Create(product);
            //_logger.LogInformation("處理結果: {result}", result);
            _loggingService.LogInformation($"處理結果: {result}");
            return View(product);
        }

        [HttpPost]
        public IActionResult CreateAjaxForm(ProductViewModel product)
        {
            // -----------------
            // 以下已經移到 Action Filter 作處理了
            // -----------------
            //// 處理 validation attribute (model binding) 檢核未過的錯誤
            //if (!ModelState.IsValid)
            //{
            //    var description = this.ModelErrorToString();
            //    throw new MyClientException(description);
            //    //return BadRequest(ModelState);
            //}

            var result = _service.Create(product);
            //_logger.LogInformation("處理結果: {result}", result);
            _loggingService.LogInformation($"處理結果: {result}");

            return View("Create", product);
        }


        [HttpPost]
        public IActionResult CreateAjaxJson([FromBody] ProductViewModel product)
        {

            // -----------------
            // 以下已經移到 Action Filter 作處理了
            // -----------------
            //// 處理 validation attribute (model binding) 檢核未過的錯誤
            //if (!ModelState.IsValid)
            //{
            //    var description = this.ModelErrorToString();
            //    throw new MyClientException(description); 
            //    //return BadRequest(ModelState);
            //}

            var result = _service.Create(product);
            // _logger.LogInformation("處理結果: {result}", result);
            _loggingService.LogInformation($"處理結果: {result}");
            return View("Create", product);
        }

        [HttpPost]
        public IActionResult CreateAjaxJsonSuccess([FromBody] ProductViewModel product)
        {
            var result = $"新增成功(Success) {product.Name}";
            //_logger.LogInformation("處理結果: {result}", result);
            _loggingService.LogInformation($"處理結果: {result}");
            return Json(new { Result = result });
        }

        [HttpPost]
        public IActionResult OccursParamNullException([FromBody] ProductViewModel product)
        {
            throw new MyParamNullException("產品名稱");
        }

        [HttpPost]
        public IActionResult OccursOutRangeException([FromBody] ProductViewModel product)
        {
            var result = _service.OccursOutRangeException(product);
            return Json(new { Result = result });
        }

        [HttpPost]
        public IActionResult OccursDataExistException([FromBody] ProductViewModel product)
        {
            throw new MyDataExistException("豆塔");
        }

        [HttpPost]
        public IActionResult OccursDataNotExistException([FromBody] ProductViewModel product)
        {
            throw new MyDataNotExistException("費南雪");
        }

        [HttpPost]
        public IActionResult OccursIOException([FromBody] ProductViewModel product)
        {
            throw new System.IO.IOException("費南雪武功密笈 不存在");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AntiForgoryOkByAjaxForm(ProductViewModel product)
        {
            throw new MyClientException("AntiForgory OK, but bad request 01");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AntiForgoryOkByAjaxJson([FromBody] ProductViewModel product)
        {
            throw new MyClientException("AntiForgory OK, but bad request 02");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AntiForgoryNgByAjaxJson([FromBody] ProductViewModel product)
        {
            throw new MyClientException("lacking form hidden field: __RequestVerificationToken");
        }
    }
}
