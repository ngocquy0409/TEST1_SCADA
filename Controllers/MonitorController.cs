using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TEST1_SCADA.Data;
using TEST1_SCADA.Models;
using TEST1_SCADA.Services;

namespace TEST1_SCADA.Controllers
{
    [Route("monitor")]
    public class MonitorController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly PlcService _plc;

        public MonitorController(ApplicationDbContext db, PlcService plc)
        {
            _db = db;
            _plc = plc;
        }

        private async Task<MonitorContextDto> BuildContext(int line, int machine, string ca)
        {
            var tenDayChuyen = $"Dây chuyền {line}";
            var tenMay = $"Máy {machine}";

            var ps = await _db.ParameterSettings
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(x =>
                    x.TenDayChuyen == tenDayChuyen &&
                    x.TenMay == tenMay &&
                    x.Ca == ca
                );

            // fallback: lấy bản mới nhất
            ps ??= await _db.ParameterSettings
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            // 2) Lấy trưởng ca theo Ca từ ShiftConfigs -> TruongCa
            int caSo = ca.Contains("2") ? 2 : ca.Contains("3") ? 3 : 1;

            var sc = await _db.ShiftConfigs
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(x => x.CaSo == caSo);

            TruongCa? tc = null;
            if (sc != null)
            {
                tc = await _db.TruongCa.FirstOrDefaultAsync(x => x.Id == sc.TruongCaId);
            }

            return new MonitorContextDto
            {
                Success = true,
                TenDayChuyen = tenDayChuyen,
                TenMay = tenMay,
                Ca = ca,

                SanPhamId = ps?.SanPhamId,
                MaSanPham = ps?.MaSanPham ?? "",
                TenSanPham = ps?.TenSanPham ?? "",

                MaTruongCa = tc?.MaTruongCa ?? "",
                
                TenTruongCa = tc?.HovaTen ?? ""
            };
        }

        [HttpGet("context")]
        public async Task<IActionResult> GetContext([FromQuery] int line = 1, [FromQuery] int machine = 1, [FromQuery] string ca = "Ca 1")
        {
            try
            {
                var ctx = await BuildContext(line, machine, ca);
                return Json(ctx);
            }
            catch (Exception ex)
            {
                return Json(new MonitorContextDto { Success = false, Message = ex.Message });
            }
        }

        //  API: Read PLC + Save SQL
        [HttpGet("read-save")]
        public async Task<IActionResult> ReadAndSave([FromQuery] int line = 1, [FromQuery] int machine = 1, [FromQuery] string ca = "Ca 1")
        {
            try
            {
                var ctx = await BuildContext(line, machine, ca);
                if (!ctx.Success)
                    return Json(new { success = false, message = ctx.Message });

                // đọc PLC
                _plc.EnsureConnected();
                var data = ReadOneMachineFromPlc(line, machine);

                // lưu SQL 
                var row = new MonitorData2
                {
                    TimeStamp = DateTime.Now,
                    TenDayChuyen = ctx.TenDayChuyen,
                    TenMay = ctx.TenMay,
                    Ca = ctx.Ca,

                    SanPhamId = ctx.SanPhamId,
                    MaSanPham = ctx.MaSanPham,
                    TenSanPham = ctx.TenSanPham,

                    MaTruongCa = ctx.MaTruongCa,
                    TenTruongCa = ctx.TenTruongCa,

                    Speed = data.Speed,
                    Oee = data.Oee,
                    Stop5s = data.Stop5s,
                    Stop5m = data.Stop5m,
                    EmptyPct = data.EmptyPct,
                    Total = data.Total,
                    Good = data.Good,
                    Jam = data.Jam,
                    Empty = data.Empty
                };

                _db.MonitorDatas.Add(row);
                await _db.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    timeStamp = row.TimeStamp,
                    context = ctx,
                    machineData = data
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private MachineLiveDto ReadOneMachineFromPlc(int line, int machine)
        {
            // ví dụ mỗi line tương ứng 1 DB
            int dbNumber = 1;

            // ví dụ mỗi máy chiếm 40 byte
            int machineBlockSize = 40;
            int baseOffset = (machine - 1) * machineBlockSize;

            string DBW(int offset) => $"DB{dbNumber}.DBW{offset}";

            return new MachineLiveDto
            {
                Speed = _plc.ReadInt16(DBW(baseOffset + 0)),
                Oee = _plc.ReadInt16(DBW(baseOffset + 2)),
                Stop5s = _plc.ReadInt16(DBW(baseOffset + 4)),
                Stop5m = _plc.ReadInt16(DBW(baseOffset + 6)),
                EmptyPct = _plc.ReadInt16(DBW(baseOffset + 8)),
                Total = _plc.ReadInt16(DBW(baseOffset + 10)),
                Good = _plc.ReadInt16(DBW(baseOffset + 12)),
                Jam = _plc.ReadInt16(DBW(baseOffset + 14)),
                Empty = _plc.ReadInt16(DBW(baseOffset + 16)),
            };
        }
    }
}
