using HotelBookingWeb.Data;
using HotelBookingWeb.Data.IRepository;
using HotelBookingWeb.Models;
using HotelBookingWeb.Models.ViewModel;
using HotelBookingWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;

namespace HotelBookingWeb.Areas.Customer.Controllers
{
    [Area(StaticDetail.Role_Customer)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IMomoSetting _momoSetting;
        public DetailHotelVM DetailHotelVM { get; set; }

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, IMomoSetting momoSetting)
        {
            _momoSetting = momoSetting;
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            //// Kiểm tra có ai đặt phòng vào hôm nay không
            //var orderDetailFormDB = _db.OrderDetails.Include("Room").Include("OrderHeader").Where(x => x.OrderHeader.CheckInDate == DateTime.Now).ToList();
            //if (orderDetailFormDB.Count > 0 && orderDetailFormDB != null)
            //{
            //    // Có người đặt phòng vào hôm nay -> cap nhat trang thai phong
            //    foreach (var orderDetail in orderDetailFormDB)
            //    {
            //        // QUa ngay thue phong
            //        if (orderDetail.Room.CheckIn < DateTime.Now && orderDetail.Room.CheckOut < DateTime.Now)
            //        {
            //            orderDetail.Room.CheckIn = null;
            //            orderDetail.Room.CheckOut = null;
            //            orderDetail.Room.Status = StaticDetail.RoomStatus_Available;
            //        }
            //        if (orderDetail.Room.Status == StaticDetail.RoomStatus_Available)
            //        {
            //            // Neu phong dang trong ma co lich check in vao hom nay thi cap nhat
            //            orderDetail.Room.CheckIn = orderDetail.OrderHeader.CheckInDate;
            //            orderDetail.Room.CheckOut = orderDetail.OrderHeader.CheckOutDate;
            //            orderDetail.Room.Status = StaticDetail.RoomStatus_Unavailable;
            //        }

            //    }
            //}


            string checkIn = DateTime.Now.ToString("yyyy/MM/dd");
            string checkOut = DateTime.Now.ToString("yyyy/MM/dd");
            DetailHotelVM = new()
            {
                ListHotel = _db.Hotels.Include("HotelImages").Include("Rooms").ToList(),
                CheckInDate = checkIn,
                CheckOutDate = checkOut
            };
            foreach (var hotel in DetailHotelVM.ListHotel)
            {
                if (hotel.Rooms != null && hotel.Rooms.Count > 0)
                {
                    var listRoom = hotel.Rooms.Where(x => x.HotelId == @hotel.Id
                                    && x.CheckIn?.ToString("yyyy/MM/dd") != checkIn
                                    && x.CheckOut?.ToString("yyyy/MM/dd") != checkOut).ToList();
                    foreach (var room in listRoom)
                    {
                        var roomCaterogy = _db.RoomCategories.Where(x => x.Id == room.CategoryId).FirstOrDefault();
                        if (roomCaterogy != null)
                        {
                            room.CategoryRoom = roomCaterogy;
                        }
                    }
                }
            }
            ViewData["CheckInDate"] = DetailHotelVM.CheckInDate;
            ViewData["CheckOutDate"] = DetailHotelVM.CheckOutDate;
            return View(DetailHotelVM);
        }

        [HttpPost]
        public IActionResult Index([FromForm] string cityInput, string inOutDate)
        {
            string checkIn = inOutDate.Split(" - ")[0].Trim();
            string checkOut = inOutDate.Split(" - ")[1].Trim();
            DetailHotelVM = new()
            {
                ListHotel = _db.Hotels.Include("HotelImages").Include("Rooms").Where(x => x.City.Contains(cityInput)).ToList(),
                CheckInDate = checkIn,
                CheckOutDate = checkOut
            };
            foreach (var hotel in DetailHotelVM.ListHotel)
            {
                if (hotel.Rooms != null && hotel.Rooms.Count > 0)
                {
                    var listRoom = hotel.Rooms.Where(x => x.HotelId == @hotel.Id
                                    && x.CheckIn?.ToString("yyyy/MM//dd") != checkIn
                                    && x.CheckOut?.ToString("yyyy/MM//dd") != checkOut).ToList();
                    foreach (var room in listRoom)
                    {
                        var roomCaterogy = _db.RoomCategories.Where(x => x.Id == room.CategoryId).FirstOrDefault();
                        if (roomCaterogy != null)
                        {
                            room.CategoryRoom = roomCaterogy;
                        }
                    }
                }
            }
            ViewData["CheckInDate"] = DetailHotelVM.CheckInDate;
            ViewData["CheckOutDate"] = DetailHotelVM.CheckOutDate;
            return View(DetailHotelVM);
        }

        public IActionResult Detail(int id, string checkIn, string checkOut)
        {
            Hotel hotel = _db.Hotels.Include("HotelImages").Include("Rooms").FirstOrDefault(x => x.Id == id);
            foreach (var room in hotel.Rooms)
            {
                var roomCaterogy = _db.RoomCategories.Where(x => x.Id == room.CategoryId).FirstOrDefault();
                if (roomCaterogy != null)
                {
                    room.CategoryRoom = roomCaterogy;
                }
            }

            DetailHotelVM = new()
            {
                Hotel = hotel,
                CheckInDate = checkIn,
                CheckOutDate = checkOut
            };
            return View(DetailHotelVM);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Detail(DetailHotelVM detailHotelVM, List<int> selectedRooms, int totalNightStay, DateTime checkInDate, DateTime checkOutDate)
        {
            if (selectedRooms != null && selectedRooms.Count > 0)
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                // Xóa hết cart cũ
                List<ShoppingCart> cartFromDB = _db.ShoppingCarts.Where(x => x.ApplicationUserId == userId).ToList();
                _db.RemoveRange(cartFromDB);
                _db.SaveChanges();

                // Lap qua tat ca phong duoc chon
                foreach (var roomSelectedId in selectedRooms)
                {
                    // Ading new room 
                    ShoppingCart shoppingCart = new()
                    {
                        RoomId = roomSelectedId,
                        ApplicationUserId = userId,
                        HotelId = detailHotelVM.Hotel.Id
                    };
                    _db.ShoppingCarts.Add(shoppingCart);
                    _db.SaveChanges();
                }
                return RedirectToAction(actionName: nameof(Summary), new { totalNightStay, checkInDate, checkOutDate });
            }
            else
            {
                detailHotelVM.Hotel = _db.Hotels.Include("HotelImages").Include("Rooms").FirstOrDefault(x => x.Id == detailHotelVM.Hotel.Id);
                foreach (var room in detailHotelVM.Hotel.Rooms)
                {
                    var roomCaterogy = _db.RoomCategories.Where(x => x.Id == room.CategoryId).FirstOrDefault();
                    if (roomCaterogy != null)
                    {
                        room.CategoryRoom = roomCaterogy;
                    }
                }
                TempData["error"] = "Bạn chưa chọn phòng nào.";
                return View(detailHotelVM);
            }
        }

        public IActionResult Summary(int totalNightStay, DateTime checkInDate, DateTime checkOutDate)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM shoppingCartVM = new()
            {
                OrderHeader = new(),
                RoomCartSelectedList = _db.ShoppingCarts.Include("Room").Where(x => x.ApplicationUserId == userId).ToList()
            };

            ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(x => x.Id == userId);
            Hotel hotelFormDB = _db.Hotels.FirstOrDefault(x => x.Id == shoppingCartVM.RoomCartSelectedList.Select(x => x.Room.HotelId).FirstOrDefault());

            shoppingCartVM.OrderHeader.ApplicationUser = applicationUser;
            shoppingCartVM.OrderHeader.ApplicationUserId = applicationUser.Id;
            shoppingCartVM.OrderHeader.Name = applicationUser.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            shoppingCartVM.OrderHeader.CheckInDate = checkInDate;
            shoppingCartVM.OrderHeader.CheckOutDate = checkOutDate;
            shoppingCartVM.OrderHeader.OrderTotal = 0;
            shoppingCartVM.OrderHeader.OrderStatus = StaticDetail.OrderStatus_Pending;
            shoppingCartVM.OrderHeader.PaymentStatus = StaticDetail.PaymentStatus_Pending;
            shoppingCartVM.OrderHeader.Hotel = hotelFormDB;
            foreach (var room in shoppingCartVM.RoomCartSelectedList)
            {
                room.Room.totalNightStay = totalNightStay;
                room.Room.totalPriceRoom = room.Room.PricePerNight * totalNightStay;
                shoppingCartVM.OrderHeader.OrderTotal += (room.Room.PricePerNight * totalNightStay);
            }
            return View(shoppingCartVM);
        }

        [HttpPost]
        public IActionResult Summary(ShoppingCartVM shoppingCartVM, int totalNightStay)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCartVM.RoomCartSelectedList = _db.ShoppingCarts.Include("Room").Where(x => x.ApplicationUserId == userId).ToList();

            shoppingCartVM.OrderHeader.PaymentDate = DateTime.Now;
            shoppingCartVM.OrderHeader.OrderStatus = StaticDetail.OrderStatus_Pending;
            shoppingCartVM.OrderHeader.PaymentStatus = StaticDetail.PaymentStatus_Pending;

            _db.OrderHeaders.Add(shoppingCartVM.OrderHeader);
            _db.SaveChanges();

            // Save in table OrderDetail
            foreach (var cart in shoppingCartVM.RoomCartSelectedList)
            {
                OrderDetail orderDetail = new()
                {
                    OrderHeaderId = shoppingCartVM.OrderHeader.Id,
                    RoomId = cart.RoomId,
                    pricePerNight = cart.Room.PricePerNight,
                    totalNightStay = totalNightStay
                };
                _db.OrderDetails.Add(orderDetail);
                _db.SaveChanges();
            }


            var response = _momoSetting.CreatePaymentAsync(shoppingCartVM);

            try
            {
                return Redirect(response.payUrl.Value);
            }
            catch
            {
                TempData["Error"] = "Lỗi xảy ra.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }



        //[HttpGet]
        //public async Task<IActionResult> GetDataProvince()
        //{
        //    Replace "API_URL" with the actual URL of the third - party API you want to access
        //   var apiUrl = "https://vn-public-apis.fpo.vn/provinces/getAll?limit=-1";

        //    HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        string data = await response.Content.ReadAsStringAsync();
        //        Process the data or return it as-is
        //        return Json(new { data });
        //    }
        //    else
        //    {
        //        Handle the error condition
        //        return StatusCode((int)response.StatusCode);
        //    }
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
