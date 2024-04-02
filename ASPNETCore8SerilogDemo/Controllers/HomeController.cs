namespace ASPNETCore8SerilogDemo.Controllers
{
    using ASPNETCore8Filter.Utilities;
    using ASPNETCore8SerilogDemo.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;

    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly ILoggingService<HomeController> _loggingService;

        //public HomeController(ILogger<HomeController> logger, ILoggingService<HomeController> loggingService)
        //{
        //    _logger = logger;
        //    _loggingService = loggingService;
        //}

        public HomeController(ILoggingService<HomeController> loggingService)
        {
            _loggingService = loggingService;
        }

        public IActionResult Index()
        {
            //_logger.LogInformation("ASPNETCore8SerilogDemo­º­¶");
            HttpContext.Session.SetString("UserId", "user001");
            _loggingService.LogInformation("ASPNETCore8SerilogDemo­º­¶");
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
