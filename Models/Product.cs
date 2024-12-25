namespace perfume.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Brand { get; set; }
        public decimal Price { get; set; }
        public decimal WithDiscount { get; set; }
        public string? Description { get; set; }
        public int Stock { get; set; }
        public string? ImageURL { get; set; }

        // Relationships
        public ICollection<OrderProduct>? OrderProducts { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}
