using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingWeb.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        
        public int? totalNightStay { get; set; }
        public double? pricePerNight { get; set; }

        [Required]
        public int OrderHeaderId { get; set; }

        [ForeignKey("OrderHeaderId")]
        [ValidateNever]
        public OrderHeader OrderHeader { get; set; }

        [Required]
        public int RoomId { get; set; }

        [ForeignKey("RoomId")]
        [ValidateNever]
        public Room Room { get; set; }
    }
}
