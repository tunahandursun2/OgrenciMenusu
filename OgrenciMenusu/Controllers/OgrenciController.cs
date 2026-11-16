using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OgrenciMenusu.Models;

namespace OgrenciMenusu.Controllers
{
    [Authorize]
    public class OgrenciController : Controller
    {
        private readonly OgrenciDbContext _context;

        public OgrenciController(OgrenciDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DersEkle(string dersAdi)
        {
            if (string.IsNullOrWhiteSpace(dersAdi))
            {
                return Json(new
                {
                    success = false,
                    message = "Ders adı boş olamaz."
                });
            }

            var kullaniciAdi = User.Identity?.Name;

            if (string.IsNullOrEmpty(kullaniciAdi))
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == kullaniciAdi);

            if (kullanici == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var ders = new Ders
            {
                DersAdi = dersAdi.Trim(),
                KullaniciId = kullanici.Id
            };

            _context.Dersler.Add(ders);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                id = ders.Id,
                dersAdi = ders.DersAdi
            });
        }

        [HttpGet]
        public async Task<IActionResult> DersleriGetir()
        {
            var kullaniciAdi = User.Identity?.Name;

            if (string.IsNullOrEmpty(kullaniciAdi))
            {
                return Json(new
                {
                    success = false,
                    dersler = new List<object>()
                });
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == kullaniciAdi);

            if (kullanici == null)
            {
                return Json(new
                {
                    success = false,
                    dersler = new List<object>()
                });
            }

            var dersler = await _context.Dersler
                .Where(d => d.KullaniciId == kullanici.Id)
                .OrderBy(d => d.DersAdi)
                .Select(d => new
                {
                    id = d.Id,
                    dersAdi = d.DersAdi
                })
                .ToListAsync();

            return Json(new
            {
                success = true,
                dersler
            });
        }

        [HttpPost]
        public async Task<IActionResult> DersSil(int id)
        {
            var kullaniciAdi = User.Identity?.Name;

            if (string.IsNullOrEmpty(kullaniciAdi))
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == kullaniciAdi);

            if (kullanici == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var ders = await _context.Dersler
                .FirstOrDefaultAsync(d =>
                    d.Id == id &&
                    d.KullaniciId == kullanici.Id);

            if (ders == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Ders bulunamadı."
                });
            }

            var programDersleri = await _context.DersProgramlari
                .Where(p =>
                    p.DersId == id &&
                    p.KullaniciId == kullanici.Id)
                .ToListAsync();

            _context.DersProgramlari.RemoveRange(programDersleri);

            _context.Dersler.Remove(ders);

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true
            });
        }

        [HttpGet]
        public async Task<IActionResult> DersPrograminiGetir()
        {
            var kullaniciAdi = User.Identity?.Name;

            if (string.IsNullOrEmpty(kullaniciAdi))
            {
                return Json(new
                {
                    success = false,
                    dersler = new List<object>()
                });
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == kullaniciAdi);

            if (kullanici == null)
            {
                return Json(new
                {
                    success = false,
                    dersler = new List<object>()
                });
            }

            var program = await _context.DersProgramlari
                .Where(p => p.KullaniciId == kullanici.Id)
                .Join(
                    _context.Dersler,
                    p => p.DersId,
                    d => d.Id,
                    (p, d) => new
                    {
                        id = p.Id,
                        dersId = d.Id,
                        dersAdi = d.DersAdi,
                        gun = p.Gun,
                        saat = p.BaslangicSaat,
                        dakika = p.BaslangicDakika,
                        bitisSaat = p.BitisSaat,
                        bitisDakika = p.BitisDakika
                    }
                )
                .ToListAsync();

            return Json(new
            {
                success = true,
                dersler = program
            });
        }

        [HttpPost]
        public async Task<IActionResult> DersProgramiEkle(
            int dersId,
            int gun,
            int baslangicSaat,
            int baslangicDakika,
            int bitisSaat,
            int bitisDakika)
        {
            var kullaniciAdi = User.Identity?.Name;

            if (string.IsNullOrEmpty(kullaniciAdi))
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == kullaniciAdi);

            if (kullanici == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var ders = await _context.Dersler
                .FirstOrDefaultAsync(d =>
                    d.Id == dersId &&
                    d.KullaniciId == kullanici.Id);

            if (ders == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Ders bulunamadı."
                });
            }

            var programDersi = new DersProgrami
            {
                KullaniciId = kullanici.Id,
                DersId = dersId,
                Gun = gun,
                BaslangicSaat = baslangicSaat,
                BaslangicDakika = baslangicDakika,
                BitisSaat = bitisSaat,
                BitisDakika = bitisDakika
            };

            _context.DersProgramlari.Add(programDersi);

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                id = programDersi.Id
            });
        }

        [HttpPost]
        public async Task<IActionResult> DersProgramiSil(int id)
        {
            var kullaniciAdi = User.Identity?.Name;

            if (string.IsNullOrEmpty(kullaniciAdi))
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == kullaniciAdi);

            if (kullanici == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var programDersi = await _context.DersProgramlari
                .FirstOrDefaultAsync(p =>
                    p.Id == id &&
                    p.KullaniciId == kullanici.Id);

            if (programDersi == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Program dersi bulunamadı."
                });
            }

            _context.DersProgramlari.Remove(programDersi);

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true
            });
        }

        [HttpPost]
        public async Task<IActionResult> KonuEkle(int dersId, string konuAdi)
        {
            if (string.IsNullOrWhiteSpace(konuAdi))
            {
                return Json(new
                {
                    success = false,
                    message = "Konu adı boş olamaz."
                });
            }

            var kullaniciAdi = User.Identity?.Name;

            if (string.IsNullOrEmpty(kullaniciAdi))
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == kullaniciAdi);

            if (kullanici == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var ders = await _context.Dersler
                .FirstOrDefaultAsync(d =>
                    d.Id == dersId &&
                    d.KullaniciId == kullanici.Id);

            if (ders == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Ders bulunamadı."
                });
            }

            var konu = new Konu
            {
                KullaniciId = kullanici.Id,
                DersId = dersId,
                KonuAdi = konuAdi.Trim(),
                Tamamlandi = false
            };

            _context.Konular.Add(konu);

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                id = konu.Id
            });
        }

        [HttpGet]
        public async Task<IActionResult> KonulariGetir()
        {
            var kullaniciAdi = User.Identity?.Name;

            if (string.IsNullOrEmpty(kullaniciAdi))
            {
                return Json(new
                {
                    success = false,
                    konular = new List<object>()
                });
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == kullaniciAdi);

            if (kullanici == null)
            {
                return Json(new
                {
                    success = false,
                    konular = new List<object>()
                });
            }

            var konular = await _context.Konular
                .Where(k => k.KullaniciId == kullanici.Id)
                .Join(
                    _context.Dersler,
                    k => k.DersId,
                    d => d.Id,
                    (k, d) => new
                    {
                        id = k.Id,
                        dersId = d.Id,
                        dersAdi = d.DersAdi,
                        konuAdi = k.KonuAdi,
                        tamamlandi = k.Tamamlandi
                    }
                )
                .OrderBy(k => k.dersAdi)
                .ThenBy(k => k.id)
                .ToListAsync();

            return Json(new
            {
                success = true,
                konular
            });
        }

        [HttpPost]
        public async Task<IActionResult> KonuTamamlandiDegistir(int id)
        {
            var kullaniciAdi = User.Identity?.Name;

            if (string.IsNullOrEmpty(kullaniciAdi))
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == kullaniciAdi);

            if (kullanici == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var konu = await _context.Konular
                .FirstOrDefaultAsync(k =>
                    k.Id == id &&
                    k.KullaniciId == kullanici.Id);

            if (konu == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Konu bulunamadı."
                });
            }

            konu.Tamamlandi = !konu.Tamamlandi;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                tamamlandi = konu.Tamamlandi
            });
        }

        [HttpPost]
        public async Task<IActionResult> KonuSil(int id)
        {
            var kullaniciAdi = User.Identity?.Name;

            if (string.IsNullOrEmpty(kullaniciAdi))
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == kullaniciAdi);

            if (kullanici == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var konu = await _context.Konular
                .FirstOrDefaultAsync(k =>
                    k.Id == id &&
                    k.KullaniciId == kullanici.Id);

            if (konu == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Konu bulunamadı."
                });
            }

            _context.Konular.Remove(konu);

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true
            });
        }

        /* ÖDEVLER */

        [HttpPost]
        public async Task<IActionResult> OdevEkle(
            int? dersId,
            string dersAdi,
            string odevAdi,
            DateTime teslimTarihi)
        {
            var kullaniciAdi = User.Identity?.Name;

            if (string.IsNullOrEmpty(kullaniciAdi))
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == kullaniciAdi);

            if (kullanici == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            if (string.IsNullOrWhiteSpace(dersAdi))
            {
                return Json(new
                {
                    success = false,
                    message = "Ders adı boş olamaz."
                });
            }

            if (string.IsNullOrWhiteSpace(odevAdi))
            {
                return Json(new
                {
                    success = false,
                    message = "Ödev adı boş olamaz."
                });
            }

            var odev = new Odev
            {
                KullaniciId = kullanici.Id,
                DersId = dersId,
                DersAdi = dersAdi.Trim(),
                OdevAdi = odevAdi.Trim(),
                TeslimTarihi = teslimTarihi,
                Tamamlandi = false,
                TamamlanmaTarihi = null
            };

            _context.Odevler.Add(odev);

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Ödev başarıyla eklendi."
            });
        }

        [HttpGet]
        public async Task<IActionResult> OdevleriGetir()
        {
            var kullaniciAdi = User.Identity?.Name;

            if (string.IsNullOrEmpty(kullaniciAdi))
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == kullaniciAdi);

            if (kullanici == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var odevler = await _context.Odevler
                .Where(o => o.KullaniciId == kullanici.Id)
                .OrderBy(o => o.TeslimTarihi)
                .Select(o => new
                {
                    o.Id,
                    o.DersId,
                    o.DersAdi,
                    o.OdevAdi,
                    o.TeslimTarihi,
                    o.Tamamlandi,
                    o.TamamlanmaTarihi
                })
                .ToListAsync();

            return Json(new
            {
                success = true,
                odevler
            });
        }

        [HttpPost]
        public async Task<IActionResult> OdevTamamla(int id)
        {
            var kullaniciAdi = User.Identity?.Name;

            if (string.IsNullOrEmpty(kullaniciAdi))
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == kullaniciAdi);

            if (kullanici == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var odev = await _context.Odevler
                .FirstOrDefaultAsync(o =>
                    o.Id == id &&
                    o.KullaniciId == kullanici.Id);

            if (odev == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Ödev bulunamadı."
                });
            }

            odev.Tamamlandi = true;
            odev.TamamlanmaTarihi = DateTime.Now;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true
            });
        }

        [HttpPost]
        public async Task<IActionResult> OdevSil(int id)
        {
            var kullaniciAdi = User.Identity?.Name;

            if (string.IsNullOrEmpty(kullaniciAdi))
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == kullaniciAdi);

            if (kullanici == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var odev = await _context.Odevler
                .FirstOrDefaultAsync(o =>
                    o.Id == id &&
                    o.KullaniciId == kullanici.Id);

            if (odev == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Ödev bulunamadı."
                });
            }

            _context.Odevler.Remove(odev);

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true
            });
        }


        [HttpGet]
        public async Task<IActionResult> YaklasanOdevleriGetir()
        {
            var kullaniciAdi = User.Identity?.Name;

            if (string.IsNullOrEmpty(kullaniciAdi))
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == kullaniciAdi);

            if (kullanici == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var bugun = DateTime.Today;
            var ucGunSonra = bugun.AddDays(3);

            var aktifOdevler = await _context.Odevler
                .Where(o =>
                    o.KullaniciId == kullanici.Id &&
                    !o.Tamamlandi)
                .OrderBy(o => o.TeslimTarihi)
                .ToListAsync();

            if (aktifOdevler.Count == 0)
            {
                return Json(new
                {
                    success = true,
                    odevler = new List<object>()
                });
            }

            var yaklasanOdevler = aktifOdevler
                .Where(o =>
                    o.TeslimTarihi.Date >= bugun &&
                    o.TeslimTarihi.Date <= ucGunSonra)
                .ToList();

            if (yaklasanOdevler.Count == 0)
            {
                var enYakinTarih = aktifOdevler
                    .Where(o => o.TeslimTarihi.Date >= bugun)
                    .Select(o => o.TeslimTarihi.Date)
                    .FirstOrDefault();

                if (enYakinTarih != default)
                {
                    yaklasanOdevler = aktifOdevler
                        .Where(o => o.TeslimTarihi.Date == enYakinTarih)
                        .ToList();
                }
            }

            return Json(new
            {
                success = true,
                odevler = yaklasanOdevler.Select(o => new
                {
                    o.Id,
                    o.DersAdi,
                    o.OdevAdi,
                    o.TeslimTarihi
                })
            });
        }
        [HttpGet]
        public async Task<IActionResult> OdevIstatistikleriniGetir()
        {
            var kullaniciAdi = User.Identity?.Name;

            if (string.IsNullOrEmpty(kullaniciAdi))
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == kullaniciAdi);

            if (kullanici == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Kullanıcı bulunamadı."
                });
            }

            var bugun = DateTime.Today;

            var haftaninBaslangici = bugun.AddDays(
                -(int)bugun.DayOfWeek + 1
            );

            if (bugun.DayOfWeek == DayOfWeek.Sunday)
            {
                haftaninBaslangici = bugun.AddDays(-6);
            }

            var haftaninBitisi = haftaninBaslangici.AddDays(7);

            var ayinBaslangici = new DateTime(
                bugun.Year,
                bugun.Month,
                1
            );

            var gelecekAy = ayinBaslangici.AddMonths(1);

            var toplamTamamlanan = await _context.Odevler
                .CountAsync(o =>
                    o.KullaniciId == kullanici.Id &&
                    o.Tamamlandi);

            var buHaftaTamamlanan = await _context.Odevler
                .CountAsync(o =>
                    o.KullaniciId == kullanici.Id &&
                    o.Tamamlandi &&
                    o.TamamlanmaTarihi >= haftaninBaslangici &&
                    o.TamamlanmaTarihi < haftaninBitisi);

            var buAyTamamlanan = await _context.Odevler
                .CountAsync(o =>
                    o.KullaniciId == kullanici.Id &&
                    o.Tamamlandi &&
                    o.TamamlanmaTarihi >= ayinBaslangici &&
                    o.TamamlanmaTarihi < gelecekAy);

            return Json(new
            {
                success = true,
                toplam = toplamTamamlanan,
                haftalik = buHaftaTamamlanan,
                aylik = buAyTamamlanan
            });
        }
    }
}   