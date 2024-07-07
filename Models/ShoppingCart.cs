using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingWeb.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public int RoomId { get; set; }
        [ForeignKey("RoomId")]
        [ValidateNever]
        public Room Room { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
