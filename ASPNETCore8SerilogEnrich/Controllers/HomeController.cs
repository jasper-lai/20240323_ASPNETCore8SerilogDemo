namespace ASPNETCore8SerilogEnrich.Controllers
{
    using ASPNETCore8SerilogEnrich.Models;
    using Microsoft.AspNetCore.Mvc;
    using Serilog;
    using System.Diagnostics;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("這是 Enrich 首頁");

            // 實測一下有無前置 @ 的差異
            var position = new { Latitude = 25, Longitude = 134 };
            var elapsedMs = 34;
            _logger.LogInformation("Processed {@Position} in {Elapsed} ms", position, elapsedMs);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
