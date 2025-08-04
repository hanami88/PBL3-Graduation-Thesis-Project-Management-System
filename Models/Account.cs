namespace PBL3.Models
{
    public class Account
    {
        [Key]
        public int IDs { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public int Id { get; set; }
    }
}