using HotelBookingWeb.Data;
using HotelBookingWeb.Models;
using HotelBookingWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace HotelBookingWeb.Areas.Admin.Controllers
{
    [Area(StaticDetail.Role_Admin)]
    [Authorize(Roles = StaticDetail.Role_Admin)]
    public class RoomCategoryController : Controller
    {
        public readonly ApplicationDbContext _db;
        public RoomCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View(_db.RoomCategories.ToList());
        }

        public IActionResult Upsert(int? id)
        {
            RoomCategory roomCategory = new RoomCategory();
            if (id != null && id != 0) // update
            {
                roomCategory = _db.RoomCategories.FirstOrDefault(x => x.Id == id);
            }
            return View(roomCategory);
        }

        [HttpPost]
        public IActionResult Upsert(RoomCategory roomCategory)
        {
            if (ModelState.IsValid)
            {
                if (roomCategory.Id == 0) // Create
                {
                    _db.Add(roomCategory);
                    _db.SaveChanges();
                    TempData["success"] = "Thêm thành công.";
                }
                else // update
                {
                    _db.Update(roomCategory);
                    _db.SaveChanges();
                    TempData["success"] = "Chỉnh sửa thành công.";
                }
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Có gì đó không đúng..";
                return View(roomCategory);
            }
        }

        #region api
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            RoomCategory? roomCategory = _db.RoomCategories.FirstOrDefault(x => x.Id == id);

            if (roomCategory != null)
            {
                _db.RoomCategories.Remove(roomCategory);
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
