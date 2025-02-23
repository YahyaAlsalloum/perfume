﻿using System.ComponentModel;

namespace perfume.Models
{
    public class OrderProduct
    {
        public int Id { get; set; }
        public Order? Order { get; set; }
        public int? OrderId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [DefaultValue(1)]
        public int? Quantity { get; set; }

    }
}
