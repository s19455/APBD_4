namespace VetClinicApi.Models
{
    public class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double Mass { get; set; }
        public string FurColor { get; set; } = string.Empty;
    }
}