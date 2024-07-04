using Microsoft.AspNetCore.Mvc;

namespace HotelBookingWeb.Areas.Admin.Controllers
{
    public class CategoryRoomController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
