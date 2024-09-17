namespace grade_service.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Category { get; set; } 
        public required string Title { get; set; }
        public required double Mark {  get; set; }
        public required double Weight {  get; set; }
    }
}
