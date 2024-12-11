namespace perfume.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }

        // Relationships
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
