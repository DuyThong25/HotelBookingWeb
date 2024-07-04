using HotelBookingWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HotelBookingWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDataProvince()
        {
            // Replace "API_URL" with the actual URL of the third-party API you want to access
            var apiUrl = "https://vn-public-apis.fpo.vn/provinces/getAll?limit=-1";

            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                // Process the data or return it as-is
                return Json(new { data });
            }
            else
            {
                // Handle the error condition
                return StatusCode((int)response.StatusCode);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
