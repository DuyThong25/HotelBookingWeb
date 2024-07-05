using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingWeb.Models
{
    public class HotelImage
    {
        public int ID { get; set; }
        [Required]
        public string ImageUrl { get; set; }

        public int HotelId { get; set; }
        [ForeignKey("HotelId")]
        [ValidateNever]
        public Hotel Hotel { get; set; }

        [ValidateNever]
        [Display(Name = "Hình ảnh khách sạn")]
        public List<HotelImage> HotelImages { get; set; }

    }
}
