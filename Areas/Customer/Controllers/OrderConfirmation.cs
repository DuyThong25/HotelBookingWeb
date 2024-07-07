using HotelBookingWeb.Data;
using HotelBookingWeb.Models;
using HotelBookingWeb.Models.ViewModel;
using HotelBookingWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBookingWeb.Areas.Customer.Controllers
{
    [Area(StaticDetail.Role_Customer)]
    [Authorize]
    public class OrderConfirmation : Controller
    {
        private readonly ApplicationDbContext _db;

        public OrderConfirmation(ApplicationDbContext db)
        {
            _db = db;
        }

        // Summary Order
        public IActionResult Index()
        {
            
           return View();
        }
    }
}
