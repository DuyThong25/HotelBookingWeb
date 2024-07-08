using HotelBookingWeb.Data;
using HotelBookingWeb.Models;
using HotelBookingWeb.Models.ViewModel;
using HotelBookingWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HotelBookingWeb.Areas.Customer.Controllers
{
    [Area(StaticDetail.Role_Customer)]
    [Authorize]
    public class OrderConfirmation : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OrderConfirmation(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
            _db = db;
        }

        // Summary Order
        public IActionResult Index()
        {

            var query = _httpContextAccessor.HttpContext.Request.Query;
            var orderId = query["orderId"].ToString().Split('-')[1];
            var resultCode = query["resultCode"].ToString();

            var orderHeaderFromDB = _db.OrderHeaders.Include("ApplicationUser").FirstOrDefault(x => x.Id == int.Parse(orderId));
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

                // Gửi mail sau khi thanh toán thành công
                string teamplate = TemplateEmailConfirmOrder(orderHeaderFromDB);
                _emailSender.SendEmailAsync(orderHeaderFromDB.ApplicationUser.Email
                    , $"Cảm ơn vì đã lựa chọn chúng tôi - Mã hóa đơn: {orderHeaderFromDB.Id}"
                    , teamplate);
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
        private string TemplateEmailConfirmOrder(OrderHeader orderHeader)
        {
            var orderDetailFormDb = _db.OrderDetails.Include("Room").Where(x => x.OrderHeaderId == orderHeader.Id).ToList();
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            string roomList = string.Empty;
            double totalPrice = 0.0;
            double TotalPriceAfterSurcharge = 0.0; // The Final Total Price
            string CustomerAddress = string.Empty;
            foreach (var item in orderDetailFormDb)
            {
                roomList += "<tr>";
                roomList += "<td>" + item.Room.Name + "</td>";
                roomList += "<td>" + item.totalNightStay + "</td>";
                roomList += "<td>" + StaticDetail.VndCurrency(item.Room.PricePerNight) + "</td>";
                roomList += "</tr>";

                totalPrice += item.Room.PricePerNight * item.Room.totalNightStay;
            }

            // Neu co giam gia thi se lay totalPrice - Discount và cuoi cung thi gan
            // TotalPriceAfterSurcharge = totalPrice
            TotalPriceAfterSurcharge = totalPrice;
            CustomerAddress = $"Khách hàng: {orderHeader.Name} - Số điện thoại: {orderHeader.PhoneNumber}";

            string contentEmailConfirmOrder = System.IO.File.ReadAllText(Path.Combine(wwwRootPath, "template\\emails\\EmailConfirmOrder.html"));
            contentEmailConfirmOrder = contentEmailConfirmOrder.Replace("{{OrderID}}", orderHeader.Id.ToString());
            contentEmailConfirmOrder = contentEmailConfirmOrder.Replace("{{TrangThaiDon}}", orderHeader.PaymentStatus);
            contentEmailConfirmOrder = contentEmailConfirmOrder.Replace("{{OrderDate}}", orderHeader.PaymentDate.ToString());
            contentEmailConfirmOrder = contentEmailConfirmOrder.Replace("{{CustomerName}}", orderHeader.Name);
            contentEmailConfirmOrder = contentEmailConfirmOrder.Replace("{{CustomerPhone}}", orderHeader.PhoneNumber);
            contentEmailConfirmOrder = contentEmailConfirmOrder.Replace("{{CustomerAddress}}", CustomerAddress);
            contentEmailConfirmOrder = contentEmailConfirmOrder.Replace("{{CustomerEmail}}", orderHeader.ApplicationUser.Email);
            contentEmailConfirmOrder = contentEmailConfirmOrder.Replace("{{Note}}", string.Empty);
            contentEmailConfirmOrder = contentEmailConfirmOrder.Replace("{{ProductList}}", roomList);
            contentEmailConfirmOrder = contentEmailConfirmOrder.Replace("{{TotalPrice}}", StaticDetail.VndCurrency(totalPrice));
            contentEmailConfirmOrder = contentEmailConfirmOrder.Replace("{{Discount}}", "0");
            contentEmailConfirmOrder = contentEmailConfirmOrder.Replace("{{TotalPriceAfterSurcharge}}", StaticDetail.VndCurrency(TotalPriceAfterSurcharge));

            return contentEmailConfirmOrder;
        }
    }
}
