using System.Collections.Generic;

namespace OrderService.Domain.Models
{
    internal class CustomerBasket
    {
        public string BuyerId { get; set; }
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();

        public CustomerBasket()
        {

        }

        public CustomerBasket(string customerId) => BuyerId = customerId;   
    }
}
