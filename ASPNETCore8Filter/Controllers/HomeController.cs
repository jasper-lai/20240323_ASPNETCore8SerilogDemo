namespace ASPNETCore8Filter.Controllers
{
    using ASPNETCore8Filter.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;

    [Route("ASPNETCore8Filter/[controller]/[action]")]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("ASPNETCore8Filter­º­¶");
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
