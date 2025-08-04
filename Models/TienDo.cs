using System.ComponentModel.DataAnnotations;

namespace PBL3.Models
{
    public class TienDo
    {
        [Key]
        public int TienDoId { get; set; }
        public int ThuTu { get; set; }
        public string? Status { get; set; }
        public string? NoiDung { get; set; }
        public string? NgayCapNhat { get; set; }
        public string? Khoa { get; set; }
        public int DoAnId { get; set; }
        public DoAn? DoAn { get; set; }
    }
}