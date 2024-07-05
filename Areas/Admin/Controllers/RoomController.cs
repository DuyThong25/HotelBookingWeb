using HotelBookingWeb.Data;
using HotelBookingWeb.Models;
using HotelBookingWeb.Models.ViewModel;
using HotelBookingWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingWeb.Areas.Admin.Controllers
{
    [Area(StaticDetail.Role_Admin)]
    [Authorize(Roles = StaticDetail.Role_Admin)]
    public class RoomController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        [BindProperty]
        public RoomVM RoomVM { get; set; }
        public RoomController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Upsert(int? id, int hotelId)
        {
            List<string> listStatus = new List<string>()
            {
                {StaticDetail.RoomStatus_Available },
                {StaticDetail.RoomStatus_Unavailable }
            };
            IEnumerable<SelectListItem> selectListItems =
                _db.RoomCategories.ToList().Select(x => new SelectListItem
                {
                    Text = $"{x.Name} ({x.Capacity} người)",
                    Value = x.Id.ToString()
                });
            IEnumerable<SelectListItem> selectListStatus =
                listStatus.Select(x => new SelectListItem
                {
                    Text = x,
                    Value = x
                });
            var hotelFromDB = _db.Hotels.FirstOrDefault(x => x.Id == hotelId);

            RoomVM = new()
            {
                Room = new()
                {
                    HotelId = hotelId,
                    Hotel = hotelFromDB
                },
                CategoryRoomList = selectListItems,
                StatusRoomList = selectListStatus
            };

            if (id != null && id != 0) // update
            {
                RoomVM.Room = _db.Rooms.Include("RoomImages").FirstOrDefault(x => x.Id == id && x.HotelId == hotelId);
            }
            return View(RoomVM);
        }

        [HttpPost]
        public IActionResult Upsert(List<IFormFile>? files)
        {
            if (ModelState.IsValid)
            {
                if (RoomVM.Room.Id == 0) // Create
                {
                    _db.Add(RoomVM.Room);
                    _db.SaveChanges();
                    TempData["success"] = "Thêm thành công.";
                }
                else // update
                {
                    _db.Update(RoomVM.Room);
                    _db.SaveChanges();
                    TempData["success"] = "Chỉnh sửa thành công.";
                }

                // Handle image
                if (files != null)
                {
                    RoomVM.Room.RoomImages = HandleToGetFileImage(RoomVM.Room, files);
                    _db.Update(RoomVM.Room);
                    _db.SaveChanges();
                }
                return RedirectToAction(nameof(Upsert), new { id = RoomVM.Room.Id, hotelId = RoomVM.Room.HotelId });
            }
            else
            {
                TempData["error"] = "Có gì đó không đúng..";
                return RedirectToAction(nameof(Upsert), new { id = RoomVM.Room.Id, hotelId = RoomVM.Room.HotelId });
            }
        }

        public List<RoomImage> HandleToGetFileImage(Room room, List<IFormFile>? files)
        {
            List<RoomImage> reuslt = new();

            foreach (IFormFile file in files)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + "-" + Path.GetFileName(file.FileName);
                string productPath = Path.Combine("images", "products", "product-" + room.Id.ToString());
                string finalPath = Path.Combine(wwwRootPath, productPath);

                // If folder are not created -> create
                if (!Directory.Exists(finalPath))
                {
                    Directory.CreateDirectory(finalPath);
                }
                // copy hinh vao folder
                using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                RoomImage roomImage = new()
                {
                    ImageUrl = @"\" + productPath + @"\" + fileName,
                    RoomId = room.Id,
                };

                if (roomImage.ImageUrl == null)
                {
                    reuslt = new List<RoomImage>();
                }
                else
                {
                    reuslt.Add(roomImage);
                }
            }
            return reuslt;
        }

        public IActionResult DeleteImage(int imageId, int hotelId)
        {
            var imageToBeDeleted = _db.RoomImages.FirstOrDefault(x => x.ID == imageId);
            int roomId = imageToBeDeleted.RoomId;
            if (imageToBeDeleted != null)
            {
                if (!String.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    string fileDelete = Path.Combine(_webHostEnvironment.WebRootPath, imageToBeDeleted.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(fileDelete))
                    {
                        System.IO.File.Delete(fileDelete);
                    }
                }

                _db.RoomImages.Remove(imageToBeDeleted);
                _db.SaveChanges();
                TempData["success"] = "Deleted Succesfully.";
            }
            return RedirectToAction(nameof(Upsert), new { id = roomId, hotelId = hotelId });
        }


        #region API
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            Room? room = _db.Rooms.FirstOrDefault(x => x.Id == id);

            if (room != null)
            {
                _db.Rooms.Remove(room);
                _db.SaveChanges();
                return Json(new { success = true, message = "Xóa thành công." });
            }
            else
            {
                return Json(new { success = false, message = "Có gì đó không đúng.." });
            }
        }
        #endregion
    }
}