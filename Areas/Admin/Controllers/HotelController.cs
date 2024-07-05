using HotelBookingWeb.Data;
using HotelBookingWeb.Models;
using HotelBookingWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingWeb.Areas.Admin.Controllers
{
    [Area(StaticDetail.Role_Admin)]
    [Authorize(Roles = StaticDetail.Role_Admin)]
    public class HotelController : Controller
    {
        public readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HotelController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;

        }
        public IActionResult Index()
        {
            return View(_db.Hotels.Include("HotelImages").Include("Rooms").ToList());
        }

        public IActionResult Upsert(int? id)
        {
            Hotel hotel = new Hotel();

            if (id != null && id != 0) // update
            {
                hotel = _db.Hotels.Include("HotelImages").Include("Rooms").FirstOrDefault(x => x.Id == id);
                
            }
            return View(hotel);
        }

        [HttpPost]
        public IActionResult Upsert(Hotel hotel, List<IFormFile>? files)
        {
            if (ModelState.IsValid)
            {
                if (hotel.Id == 0) // Create
                {
                    _db.Add(hotel);
                    _db.SaveChanges();
                    TempData["success"] = "Thêm thành công.";
                }
                else // update
                {
                    _db.Update(hotel);
                    _db.SaveChanges();
                    TempData["success"] = "Chỉnh sửa thành công.";
                }

                // Handle image
                if (files != null)
                {
                    hotel.HotelImages = HandleToGetFileImage(hotel, files);
                    _db.Hotels.Update(hotel);
                    _db.SaveChanges();
                }
                return RedirectToAction(nameof(Upsert), new { id = hotel.Id });
            }
            else
            {
                TempData["error"] = "Có gì đó không đúng..";
                return View(hotel); 
            }
        }

        public List<HotelImage> HandleToGetFileImage(Hotel hotel, List<IFormFile>? files)
        {
            List<HotelImage> reuslt = new();

            foreach (IFormFile file in files)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + "-" + Path.GetFileName(file.FileName);
                string productPath = Path.Combine("images", "products", "product-" + hotel.Id.ToString());
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

                HotelImage productImage = new()
                {
                    ImageUrl = @"\" + productPath + @"\" + fileName,
                    HotelId = hotel.Id,
                };

                if (productImage.ImageUrl == null)
                {
                    reuslt = new List<HotelImage>();
                }
                else
                {
                    reuslt.Add(productImage);
                }
                //HandleDeleteFileImage(hotel.Product, wwwRootPath);
            }
            return reuslt;
        }

        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = _db.HotelImages.FirstOrDefault(x => x.ID == imageId);
            int hotelId = imageToBeDeleted.HotelId;
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

                _db.HotelImages.Remove(imageToBeDeleted);
                _db.SaveChanges();
                TempData["success"] = "Deleted Succesfully.";
            }
            return RedirectToAction(nameof(Upsert), new { id = hotelId });
        }


        #region API
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            Hotel? hotel = _db.Hotels.FirstOrDefault(x => x.Id == id);

            if (hotel != null)
            {
                _db.Hotels.Remove(hotel);
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
