using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingWeb.Models
{
    public class Hotel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống")]
        [Display(Name = "Tên khách sạn")]
        public string Name { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Không được bỏ trống")]
        [Display(Name = "Địa chỉ")]
        public string Street { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống")]
        [Display(Name = "Quận")]
        public string Distinct { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống")]
        [Display(Name = "Phường")]
        public string Ward { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống")]
        [Display(Name = "Thành phố")]
        public string City { get; set; }

        [ValidateNever]
        [Display(Name = "Danh sách phòng")]
        public List<Room> Rooms { get; set; }

        [ValidateNever]
        [Display(Name = "Hình ảnh khách sạn")]
        public List<HotelImage> HotelImages { get; set; }
    }
}
