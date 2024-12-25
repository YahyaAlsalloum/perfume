﻿namespace perfume.Models
{
    public class Order
    {
        public int Id { get; set; }
       
        public string UserId { get; set; }

        // Navigation :
        public ApplicationUser User { get; set; }

        // Relationship :
        public ICollection<OrderProduct>? OrderProducts { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
    }
}
