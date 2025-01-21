namespace perfume.Models
{
    public class BasePerfume
    {
        public int Id { get; set; } // Primary Key
        public string? UserId { get; set; } // Foreign Key
        public int? Size { get; set; }
         public int? Quantity { get; set; }
        public string? Status { get; set; }
        public ApplicationUser User { get; set; }

        public string? SelectedBases { get; set; } 

    }
}
