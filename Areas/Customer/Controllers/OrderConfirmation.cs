using HotelBookingWeb.Data;
using HotelBookingWeb.Models;
using HotelBookingWeb.Models.ViewModel;
using HotelBookingWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBookingWeb.Areas.Customer.Controllers
{
    [Area(StaticDetail.Role_Customer)]
    [Authorize]
    public class OrderConfirmation : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderConfirmation(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _db = db;
        }

        // Summary Order
        public IActionResult Index()
        {

            var query = _httpContextAccessor.HttpContext.Request.Query;
            var orderId = query["orderId"].ToString().Split('-')[1];
            var resultCode = query["resultCode"].ToString();

            var orderHeaderFromDB = _db.OrderHeaders.FirstOrDefault(x => x.Id == int.Parse(orderId));
            if (resultCode == "0" && orderHeaderFromDB != null)
            {

                orderHeaderFromDB.OrderStatus = StaticDetail.OrderStatus_Approved;
                orderHeaderFromDB.PaymentStatus = StaticDetail.PaymentStatus_Approved;
                orderHeaderFromDB.PaymentIntentId = query["transId"];
                orderHeaderFromDB.SessionId = query["requestId"];
                _db.OrderHeaders.Update(orderHeaderFromDB);
                _db.SaveChanges();

                // Lấy các phòng được chọn
                var listRoomID = _db.OrderDetails.Where(x => x.OrderHeaderId == orderHeaderFromDB.Id).Select(x => x.RoomId).ToList();
                foreach (var roomId in listRoomID)
                {
                    var roomSelected = _db.Rooms.FirstOrDefault(x => x.Id == roomId);
                    roomSelected.CheckIn = orderHeaderFromDB.CheckInDate;
                    roomSelected.CheckOut = orderHeaderFromDB.CheckOutDate;
                    roomSelected.Status = StaticDetail.RoomStatus_Unavailable;

                    _db.Rooms.Update(roomSelected);
                    _db.SaveChanges();
                }
                TempData["success"] = "Thanh Toán thành công";
                return View(orderHeaderFromDB);

            }
            else
            {
                if (orderHeaderFromDB != null)
                {
                    _db.OrderHeaders.Remove(orderHeaderFromDB);
                    _db.SaveChanges();
                }
                TempData["error"] = "Thanh Toán thất bại";
                orderHeaderFromDB = new()
                {
                    OrderStatus = StaticDetail.OrderStatus_Pending,
                    PaymentStatus = StaticDetail.PaymentStatus_Pending
                };
                return View(orderHeaderFromDB);

            }

        }
    }
}
