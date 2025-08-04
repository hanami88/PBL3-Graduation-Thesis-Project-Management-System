using System.ComponentModel.DataAnnotations;

namespace PBL3.Models
{
    public class GiangVien
    {
        [Key]
        public int Id { get; set; }
        public string? HoTen { get; set; }
        public string? NgaySinh { get; set; }
        public string? Khoa { get; set; }
        public int MaGiangVien { get; set; }
        public string? SoDienThoai { get; set; }
    }
}