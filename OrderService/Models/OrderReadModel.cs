using System;

namespace OrderService.Models
{
    public class OrderReadModel
    {
        public Guid OrderId { get; set; }
        public string[] Goods { get; set; }
        
        public Currency Currency { get; set; }

        public decimal Cost { get; set; }
        public DateTime ArriveAt { get; set; }
    }
}