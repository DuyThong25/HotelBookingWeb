using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingWeb.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tên phòng")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Mã phòng")]
        public string Code { get; set; }

        [Required]
        [Display(Name ="Mô tả")]
        public string Description { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Giá phòng/ đêm")]
        public double PricePerNight { get; set; }

        [Required]
        [Display(Name = "Trạng thái phòng")]
        public string Status { get; set; }

        [Display(Name ="Ngày đặt phòng")]
        public DateTime? CheckIn {  get; set; }
        [Display(Name = "Ngày trả phòng")]
        public DateTime? CheckOut {  get; set; }

        public int HotelId { get; set; }
        [ForeignKey("HotelId")]
        [ValidateNever]
        public Hotel Hotel { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public RoomCategory CategoryRoom { get; set; }

        [ValidateNever]
        [Display(Name = "Hình ảnh phòng")]
        public List<RoomImage> RoomImages { get; set; }


    }
}
