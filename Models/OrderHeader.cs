﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingWeb.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }

        public DateTime? PaymentDate { get; set; }
        public double? OrderTotal { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? SessionId { get; set; }

        public string? PaymentIntentId { get; set; }

        [Display(Name = "Ngày đặt phòng")]
        public DateTime? CheckInDate { get; set; }
        [Display(Name = "Ngày trả phòng")]
        public DateTime? CheckOutDate { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        [NotMapped]
        public Hotel Hotel { get; set; }

    }
}
