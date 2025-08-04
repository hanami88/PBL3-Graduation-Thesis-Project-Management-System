namespace PBL3.Models
{
    public class DiemSo
    {
        [Key]
        public int ID { get; set; }
        public int DiemHoiDong { get; set; }
        public int DiemHuongDan { get; set; }
        public int DiemPhanBien { get; set; }
        public int DiemTrungBinh { get; set; }
        public DoAn? DoAn { get; set; }
        public int DoAnId { get; set; }
    }
}