using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3.Data; 
using PBL3.Models;

namespace PBL3.Controllers
{
    [ApiController]
    [Route("api/SinhVien")]
    public class SVController : ControllerBase
    {
        private readonly AppDbContext _context;
        public SVController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSV()
        {
            var SinhVien = await _context.SinhViens.ToListAsync();
            return Ok(SinhVien);
        }

    }

    [ApiController]
    [Route("api/Account")]
    public class ACCController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ACCController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Account request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Thiếu thông tin đăng nhập." });

            var user = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserName == request.UserName);

            if (user == null || user.Password != request.Password)
            {
                return Unauthorized(new { message = "Sai tài khoản hoặc mật khẩu." });
            }

            return Ok(new { message = "Đăng nhập thành công!", userId = user.Id, userRole = user.Role });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromQuery] int id)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.Id == id);
            if (account == null)
                return NotFound("Không tìm thấy Tài Khoản.");

            account.Password = "1";
            _context.SaveChanges();

            return Ok("Password đã được reset.");
        }

        [HttpPost("CreateAccount")]
        public IActionResult CreateAccount([FromBody] Account account)
        {
            if (_context.Accounts.Any(a => a.UserName == account.UserName))
            {
                return BadRequest("Tài khoản đã tồn tại.");
            }

            _context.Accounts.Add(account);
            _context.SaveChanges();

            return Ok(account);
        }

        public class DeleteStudentRequest
        {
            public int mssv { get; set; }
        }

        [HttpPost("DeleteStudentAndAccount")]
        public IActionResult DeleteStudentAndAccount([FromBody] DeleteStudentRequest request)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.Id == request.mssv);
            var sv = _context.SinhViens.FirstOrDefault(s => s.MSSV == request.mssv);

            if (account != null)
                _context.Accounts.Remove(account);

            if (sv != null)
                _context.SinhViens.Remove(sv);

            _context.SaveChanges();

            return Ok("Đã xoá thành công");
        }

        [HttpPost("DeleteGiangVienAndAccount")]
        public IActionResult DeleteGiangVienAndAccount([FromBody] DeleteStudentRequest request)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.Id == request.mssv);
            var gv = _context.GiangViens.FirstOrDefault(s => s.MaGiangVien == request.mssv);

            if (account != null)
                _context.Accounts.Remove(account);

            if (gv != null)
                _context.GiangViens.Remove(gv);

            _context.SaveChanges();

            return Ok("Đã xoá thành công");
        }

        public class ChangePasswordRequest
        {
            public int Id { get; set; }
            public string OldPassword { get; set; } = string.Empty;
            public string NewPassword { get; set; } = string.Empty;
        }

        [HttpPut("ChangePassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.OldPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest("Vui lòng nhập đầy đủ mật khẩu cũ và mới.");

            var account = _context.Accounts.FirstOrDefault(a => a.Id == request.Id);
            if (account == null)
                return NotFound("Không tìm thấy tài khoản.");

            if (account.Password != request.OldPassword)
                return Unauthorized("Mật khẩu cũ không đúng.");

            account.Password = request.NewPassword;
            _context.SaveChanges();

            return Ok("Đổi mật khẩu thành công.");
        }
    }

    [ApiController]
    [Route("api/User")]
    public class TTSVController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TTSVController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetUserByMSSV")]
        public async Task<IActionResult> GetUserByMSSV([FromQuery] int mssv)
        {
            var user = await _context.SinhViens
                .Where(u => u.MSSV == mssv)
                .Select(u => new
                {
                    HoTen = u.HoTen,
                    NgaySinh = u.NgaySinh,
                    Khoa = u.Khoa,
                    LopHocPhan = u.LopHocPhan,
                    MSSV = u.MSSV,
                    Email = u.Email,
                    SDT = u.SDT,
                    NienKhoa = u.NienKhoa
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "Không tìm thấy người dùng." });

            return Ok(user);
        }

        [HttpGet]
        public IActionResult GetAllSV()
        {
            var accounts = _context.SinhViens.Select(a => new
            {
                a.SinhVienId,
                a.HoTen,
                a.NgaySinh,
                a.Khoa,
                a.LopHocPhan,
                a.MSSV,
                a.Email,
                a.SDT,
                a.NienKhoa
            }).ToList();

            return Ok(accounts);
        }

        [HttpPost]
        public async Task<IActionResult> PostSinhVien([FromBody] SinhVien sinhVien)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing = _context.SinhViens.FirstOrDefault(sv => sv.MSSV == sinhVien.MSSV);
            if (existing != null)
            {
                return Conflict("Sinh viên đã tồn tại.");
            }

            _context.SinhViens.Add(sinhVien);
            await _context.SaveChangesAsync();

            return Ok(sinhVien);
        }

        [HttpPut("UpdateSinhVien/{mssv}")]
        public async Task<IActionResult> UpdateSinhVien(string mssv, [FromBody] SinhVien sinhVienDto)
        {

            if (mssv != sinhVienDto.MSSV.ToString())
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(sv => sv.MSSV == int.Parse(mssv));
            if (sinhVien == null)
            {
                return NotFound("Không tìm thấy sinh viên.");
            }

            sinhVien.HoTen = sinhVienDto.HoTen;
            sinhVien.NgaySinh = sinhVienDto.NgaySinh;
            sinhVien.Khoa = sinhVienDto.Khoa;
            sinhVien.LopHocPhan = sinhVienDto.LopHocPhan;
            sinhVien.Email = sinhVienDto.Email;
            sinhVien.SDT = sinhVienDto.SDT;

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Cập nhật sinh viên thành công.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }
    }

    [ApiController]
    [Route("api/DoAn")]
    public class DAController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DAController(AppDbContext context)
        {
            _context = context;
        }

        public class DoAnDTO
        {
            public int ID { get; set; }
            public string? Ten { get; set; }
            public string? Start { get; set; }
            public string? End { get; set; }
            public string? Status { get; set; }
            public string? GVHD { get; set; }
            public string? Detail { get; set; }
            public string? Requirement { get; set; }
            public int MSSV { get; set; }
            public int NamThucHien { get; set; }
        }

        [HttpGet("GetDoAn")]
        public async Task<IActionResult> GetDoAn([FromQuery] int userId)
        {
            var user = await _context.DoAns
                .Where(u => u.MSSV == userId)
                .Select(u => new DoAnDTO
                {
                    ID = u.DoAnId,
                    Ten = u.Ten,
                    Start = u.NgayBatDau,
                    End = u.NgayKetThuc,
                    Status = u.Status,
                    GVHD = u.GVHD,
                    Detail = u.MoTa,
                    Requirement = u.YCGV,
                    MSSV = u.MSSV,
                    NamThucHien = u.NamThucHien
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "Không tìm thấy đồ án." });

            return Ok(user);
        }

        [HttpGet("GetByGVHD")]
        public async Task<IActionResult> GetDoAnByGVHD([FromQuery] string tenGV)
        {
            var list = await _context.DoAns
                .Where(d => d.GVHD == tenGV && d.Status == "Đang thực hiện")
                .Select(d => new
                {
                    TenDoAn = d.Ten,
                    TenSinhVien = d.SinhVien.HoTen,
                    Id = d.DoAnId
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("GetByGVHD1")]
        public async Task<IActionResult> GetDoAnByGVHD1([FromQuery] string tenGV)
        {
            var list = await _context.DoAns
                .Where(d => d.GVHD == tenGV && d.Status == "Đã Hoàn Thành")
                .Select(d => new
                {
                    TenDoAn = d.Ten,
                    TenSinhVien = d.SinhVien.HoTen,
                    Id = d.DoAnId
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoAnById(int id)
        {
            var doAn = await _context.DoAns
                .Where(u => u.DoAnId == id)
                .Select(u => new DoAnDTO
                {
                    ID = u.DoAnId,
                    Ten = u.Ten,
                    Start = u.NgayBatDau,
                    End = u.NgayKetThuc,
                    Status = u.Status,
                    GVHD = u.GVHD,
                    Detail = u.MoTa,
                    Requirement = u.YCGV,
                    MSSV = u.MSSV
                })
                .FirstOrDefaultAsync();

            if (doAn == null)
                return NotFound(new { message = "Không tìm thấy đồ án theo ID." });

            return Ok(doAn);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDoAn()
        {
            var doAnList = await _context.DoAns
                .Include(d => d.SinhVien)
                .Select(d => new
                {
                    id = d.DoAnId,
                    mssv = d.MSSV,
                    tenSinhVien = d.SinhVien,
                    tenGiangVien = d.GVHD,
                    tenDoAn = d.Ten,
                    ngayBatDau = d.NgayBatDau,
                    ngayKetThuc = d.NgayKetThuc,
                    trangThai = d.Status,
                    NamThucHien = d.NamThucHien,
                })
                .ToListAsync();

            return Ok(doAnList);
        }

        [HttpPut("UpdateDoAn/{mssv}")]
        public async Task<IActionResult> UpdateDoAnByMssv(string mssv, [FromBody] DoAn updatedDoAn)
        {
            try
            {
                // Tìm đồ án theo MSSV
                var doAn = await _context.DoAns
                    .Include(d => d.SinhVien)
                    .FirstOrDefaultAsync(d => d.SinhVien.MSSV == int.Parse(mssv));

                if (doAn == null)
                {
                    return NotFound($"Không tìm thấy đồ án cho sinh viên có MSSV = {mssv}");
                }

                doAn.Ten = updatedDoAn.Ten;
                doAn.NgayBatDau = updatedDoAn.NgayBatDau;
                doAn.NgayKetThuc = updatedDoAn.NgayKetThuc;
                doAn.Status = updatedDoAn.Status;
                doAn.GVHD = updatedDoAn.GVHD;
                doAn.MoTa = updatedDoAn.MoTa;

                await _context.SaveChangesAsync();

                return Ok("Cập nhật thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        public class DeleteStudentRequest
        {
            public int mssv { get; set; }
        }

        [HttpPost("DeleteDoAn")]
        public IActionResult DeleteGiangVienAndAccount([FromBody] DeleteStudentRequest request)
        {
            var doan = _context.DoAns.FirstOrDefault(a => a.MSSV == request.mssv);
            if (doan == null)
            {
                return NotFound("Không tìm thấy đồ án với MSSV này");
            }
            var tienDos = _context.TienDos.Where(t => t.DoAnId == doan.DoAnId).ToList();
            if (tienDos.Any())
            {
                _context.TienDos.RemoveRange(tienDos);
            }
            _context.DoAns.Remove(doan);
            _context.SaveChanges();
            return Ok("Đã xoá thành công");
        }
    }

    [ApiController]
    [Route("api/GiangVien")]
    public class TTGVController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TTGVController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetGiangVienByMSGV")]
        public async Task<IActionResult> GetUserByMSSV([FromQuery] int msgv)
        {
            try
            {
                var user = await _context.GiangViens
                    .Where(u => u.MaGiangVien == msgv)
                    .Select(u => new
                    {
                        HoTen = u.HoTen,
                        NgaySinh = u.NgaySinh,
                        Khoa = u.Khoa,
                        SDT = u.SoDienThoai,
                        MGV = u.MaGiangVien,
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                    return NotFound(new { message = "Không tìm thấy người dùng." });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAllGV()
        {
            var accounts = _context.GiangViens.Select(a => new
            {
                a.Id,
                a.HoTen,
                a.NgaySinh,
                a.Khoa,
                a.MaGiangVien,
                a.SoDienThoai
            }).ToList();

            return Ok(accounts);
        }

        [HttpPut("UpdateGiangVien/{mgv}")]
        public async Task<IActionResult> UpdateGiangVien(int mgv, [FromBody] GiangVien updatedGiangVien)
        {
            if (mgv != updatedGiangVien.MaGiangVien)
                return BadRequest("ID giảng viên không khớp.");

            var giangVien = await _context.GiangViens.FirstOrDefaultAsync(gv => gv.MaGiangVien == mgv);
            if (giangVien == null)
                return NotFound();

            giangVien.HoTen = updatedGiangVien.HoTen;
            giangVien.NgaySinh = updatedGiangVien.NgaySinh;
            giangVien.Khoa = updatedGiangVien.Khoa;
            giangVien.SoDienThoai = updatedGiangVien.SoDienThoai;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        public IActionResult CreateGiangVien([FromBody] GiangVien gv)
        {
            if (_context.GiangViens.Any(g => g.MaGiangVien == gv.MaGiangVien))
            {
                return BadRequest("Mã giảng viên đã tồn tại.");
            }

            _context.GiangViens.Add(gv);
            _context.SaveChanges();

            return Ok(gv);
        }
    }

    [ApiController]
    [Route("api/TienDo")]
    public class TienDoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TienDoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetTienDoByDoAnId")]
        public async Task<IActionResult> GetTienDoByDoAnId(int doAnId)
        {
            var tienDoList = await _context.TienDos
                .Where(td => td.DoAnId == doAnId)
                .OrderBy(td => td.ThuTu)
                .Select(td => new
                {
                    td.TienDoId,
                    td.ThuTu,
                    td.NoiDung,
                    td.NgayCapNhat,
                    td.Khoa,
                    td.Status
                })
                .ToListAsync();

            if (!tienDoList.Any())
            {
                return NotFound("Không tìm thấy tiến độ cho đồ án này");
            }

            return Ok(tienDoList);
        }
    }

    [ApiController]
    [Route("api/QLDoAn")]
    public class DoAnController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DoAnController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("by-gvhd")]
        public async Task<IActionResult> GetDoAnByGVHD(string gvhd)
        {
            if (string.IsNullOrWhiteSpace(gvhd))
                return BadRequest("Tên giảng viên không hợp lệ");

            var result = await _context.DoAns
                .Where(d => d.GVHD == gvhd)
                .Select(d => new
                {
                    d.DoAnId,
                    TenDoAn = d.Ten,
                    d.Status,
                    d.GVHD,
                    d.MSSV,
                    d.SinhVien.HoTen,
                })
                .ToListAsync();

            return Ok(result);
        }
    }

    [ApiController]
    [Route("api/QLDoAnTheoMSSV")]
    public class QLDoAnTheoMSSVController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QLDoAnTheoMSSVController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetByDoAnId")]
        public async Task<IActionResult> GetByDoAnId([FromQuery] int doAnId)
        {
            if (doAnId <= 0)
                return BadRequest("DoAnId không hợp lệ");

            var doAn = await _context.DoAns
                .Where(d => d.DoAnId == doAnId)
                .Select(d => new
                {
                    d.DoAnId,
                    TenDoAn = d.Ten,
                    d.Status,
                    d.GVHD,
                    d.MSSV,
                    d.NgayBatDau,
                    d.NgayKetThuc,
                    d.MoTa
                })
                .FirstOrDefaultAsync();

            if (doAn == null)
                return NotFound("Không tìm thấy đồ án");

            var sinhVien = await _context.SinhViens
                .Where(sv => sv.MSSV == doAn.MSSV)
                .Select(sv => new
                {
                    sv.MSSV,
                    sv.HoTen,
                    NgaySinh = sv.NgaySinh.ToString("dd/MM/yyyy"),
                    sv.Khoa,
                    sv.LopHocPhan,
                    sv.Email,
                    sv.SDT
                })
                .FirstOrDefaultAsync();

            return Ok(new { doAn, sinhVien });
        }
    }

    [Route("api/TienDoCS")]
    [ApiController]

    public class TienDoCSController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TienDoCSController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{doAnId}")]
        public async Task<IActionResult> GetTienDoByDoAn(int doAnId)
        {
            var tienDoList = await _context.TienDos
                .Where(td => td.DoAnId == doAnId)
                .OrderBy(td => td.ThuTu)
                .Select(td => new
                {
                    td.TienDoId,
                    td.ThuTu,
                    td.Status,
                    td.NoiDung,
                    NgayCapNhat = td.NgayCapNhat
                })
                .ToListAsync();

            return Ok(tienDoList);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> UpdateTienDo([FromBody] TienDo dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tienDo = await _context.TienDos
                .FirstOrDefaultAsync(td => td.DoAnId == dto.DoAnId && td.ThuTu == dto.ThuTu);

            if (tienDo == null)
            {
                tienDo = new TienDo
                {
                    DoAnId = dto.DoAnId,
                    ThuTu = dto.ThuTu,
                    Status = dto.Status,
                    NoiDung = dto.NoiDung,
                    NgayCapNhat = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                };
                _context.TienDos.Add(tienDo);
            }
            else
            {
                tienDo.Status = dto.Status;
                tienDo.NoiDung = dto.NoiDung;
                tienDo.NgayCapNhat = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                _context.TienDos.Update(tienDo);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật tiến độ thành công" });
        }
    }

    [Route("api/DiemSoLoad")]
    [ApiController]
    public class DiemSoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DiemSoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostDiemSo([FromBody] DiemSo diemSo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.DiemSos.Add(diemSo);
            await _context.SaveChangesAsync();
            return Ok(diemSo);
        }
    }

    [ApiController]
    [Route("api/ExcelImport")]

    public class ExcelImportController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExcelImportController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> ImportSinhViens([FromBody] List<ImportSinhVienDto> dtos)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { Errors = errors });
            }
            foreach (var dto in dtos)
            {
                // 1. Tạo sinh viên
                var sv = new SinhVien
                {
                    MSSV = dto.MSSV,
                    HoTen = dto.HoTen,
                    NgaySinh = dto.NgaySinh,
                    Khoa = dto.Khoa,
                    LopHocPhan = dto.LopHocPhan,
                    Email = dto.Email,
                    SDT = dto.SDT,
                    NienKhoa = dto.NienKhoa
                };
                _context.SinhViens.Add(sv);
                await _context.SaveChangesAsync();

                // 2. Tạo đồ án
                var da = new DoAn
                {
                    Ten = dto.TenDoAn,
                    NgayBatDau = dto.NgayBatDau,
                    NgayKetThuc = dto.NgayKetThuc,
                    Status = dto.Status,
                    GVHD = dto.GVHD,
                    MoTa = dto.MoTa,
                    YCGV = dto.YCGV,
                    MSSV = dto.MSSV,
                    NamThucHien = dto.NamThucHien,
                    SinhVien = sv
                };
                _context.DoAns.Add(da);
                await _context.SaveChangesAsync();

                // 3. Tạo tiến độ mặc định
                var now = DateTime.Now.ToString("yyyy-MM-dd");
                var tienDos = new List<TienDo>
                {
                    new TienDo { ThuTu = 1, Status = "NO", NoiDung = "Giai đoạn 1", NgayCapNhat = now, Khoa = dto.Khoa, DoAnId = da.DoAnId },
                    new TienDo { ThuTu = 2, Status = "NO", NoiDung = "Giai đoạn 2", NgayCapNhat = now, Khoa = dto.Khoa, DoAnId = da.DoAnId },
                    new TienDo { ThuTu = 3, Status = "NO", NoiDung = "Giai đoạn 3", NgayCapNhat = now, Khoa = dto.Khoa, DoAnId = da.DoAnId }
                };
                _context.TienDos.AddRange(tienDos);

                // 4. Tạo tài khoản
                var acc = new Account
                {
                    UserName = dto.MSSV.ToString(),
                    Password = "1",
                    Role = "SV",
                    Id = sv.SinhVienId
                };
                _context.Accounts.Add(acc);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Import thành công" });
        }
    }

    public class ImportSinhVienDto
    {
        public int MSSV { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public string Khoa { get; set; }
        public string LopHocPhan { get; set; }
        public string Email { get; set; }
        public string SDT { get; set; }
        public int NienKhoa { get; set; }
        public string TenDoAn { get; set; }
        public string NgayBatDau { get; set; }
        public string? NgayKetThuc { get; set; }
        public string Status { get; set; }
        public string GVHD { get; set; }
        public string MoTa { get; set; }
        public string YCGV { get; set; }
        public int NamThucHien { get; set; }
    }

    [ApiController]
    [Route("api/DiemSo")]
    public class SinhVienController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SinhVienController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{mssv}")]
        public async Task<IActionResult> GetDiemSoByMSSV(string mssv)
        {
            var doAn = await _context.DoAns
                .FirstOrDefaultAsync(d => d.MSSV == int.Parse(mssv));

            if (doAn == null)
                return NotFound(new { message = "Không tìm thấy đồ án của sinh viên này." });

            var diemSo = await _context.DiemSos
                .FirstOrDefaultAsync(ds => ds.DoAnId == doAn.DoAnId);

            if (diemSo == null)
                return NotFound(new { message = "Không tìm thấy điểm số cho đồ án này." });

            return Ok(new
            {
                idDoAn = doAn.DoAnId,
                diemHoiDong = diemSo.DiemHoiDong,
                diemHuongDan = diemSo.DiemHuongDan,
                diemPhanBien = diemSo.DiemPhanBien,
                diemTrungBinh = diemSo.DiemTrungBinh
            });
        }
    }
}