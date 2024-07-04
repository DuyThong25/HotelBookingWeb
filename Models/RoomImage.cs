using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingWeb.Models
{
    public class RoomImage
    {
        public int ID { get; set; }
        [Required]
        public string ImageUrl { get; set; }

        public int RoomId { get; set; }
        [ForeignKey("RoomId")]
        [ValidateNever]
        public Room Room { get; set; }
    }
}
