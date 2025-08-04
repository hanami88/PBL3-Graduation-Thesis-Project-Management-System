using System.ComponentModel.DataAnnotations;

namespace PBL3.Models
{
    public class SinhVien
    {
        [Key]
        public int SinhVienId { get; set; }
        public string? HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public string? Khoa { get; set; }
        public string? LopHocPhan { get; set; }
        public int MSSV { get; set; }
        public string? Email { get; set; }
        public string? SDT { get; set; }
        public int NienKhoa { get; set; }
        public ICollection<DoAn>? DoAn { get; set; }
    }
}
