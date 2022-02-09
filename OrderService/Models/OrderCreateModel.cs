using System.Text.Json.Serialization;

namespace OrderService.Models
{
    public class OrderCreateModel
    {
        public Currency Currency { get; set; }
        public string[] Goods { get; set; }

        public string Number { get; set; }
        public decimal TotalAmount { get; set; }

        public InnerNumber InnerNumber { get; set; }

    }

    public class InnerNumber
    {
        public string Number { get; set; }
    }
}