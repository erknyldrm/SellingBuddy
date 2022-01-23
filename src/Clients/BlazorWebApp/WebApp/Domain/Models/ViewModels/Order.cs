using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Domain.Models.ViewModels
{
    public class Order
    {
        public string OrderNumber { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
        public string Description { get; set; }

        [Required]
        public string City { get; set; }
        
        [Required]
        public string Street { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Country { get; set; }

    }
}
