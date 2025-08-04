using PBL3.Models;

namespace PBL3.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<DoAn>? DoAns { get; set; }
        public DbSet<GiangVien>? GiangViens { get; set; }
        public DbSet<SinhVien>? SinhViens { get; set; }
        public DbSet<TienDo>? TienDos { get; set; }
        public DbSet<Account>? Accounts { get; set; }
        public DbSet<DiemSo>? DiemSos { get; set; }
    }
}