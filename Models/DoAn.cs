using System.ComponentModel.DataAnnotations;

namespace PBL3.Models
{
    public class DoAn
    {
        [Key]
        public int DoAnId { get; set; }
        public string? Ten { get; set; }
        public string? NgayBatDau { get; set; }
        public string? NgayKetThuc { get; set; }
        public string? Status { get; set; }
        public string? GVHD { get; set; }
        public string? MoTa { get; set; }
        public string? YCGV { get; set; }
        public int MSSV { get; set; }
        public int NamThucHien { get; set; }
        public SinhVien? SinhVien { get; set; }
        public ICollection<TienDo>? TienDo { get; set; }
    }
}