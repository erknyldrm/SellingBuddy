using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Domain.Models.ViewModels
{
    public class Basket
    {
        public List<BasketItem> Items { get; init; } = new List<BasketItem>();

        public string BuyerId { get; set; }

        public decimal Total()
        {
            return Math.Round(Items.Sum(p =>p.UnitPrice * p.Quantity), 2);
        }
    }
}
