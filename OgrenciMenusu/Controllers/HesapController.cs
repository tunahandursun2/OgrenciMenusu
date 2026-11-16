using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OgrenciMenusu.Models;

namespace OgrenciMenusu.Controllers
{
    public class HesapController : Controller
    {
        private readonly OgrenciDbContext _context;
        private readonly PasswordHasher<Kullanici> _passwordHasher = new();

        public HesapController(OgrenciDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Giris() => View(new GirisViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Giris(GirisViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var kullanici = await _context.Kullanicilar
                .SingleOrDefaultAsync(k => k.KullaniciAdi == model.KullaniciAdi);

            if (kullanici is null || _passwordHasher.VerifyHashedPassword(
                    kullanici, kullanici.SifreHash, model.Sifre) == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı.");
                return View(model);
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),
                new Claim(ClaimTypes.Name, kullanici.KullaniciAdi)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Ogrenci");
        }

        [HttpGet]
        public IActionResult Kayit() => View(new KayitViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Kayit(KayitViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var kullaniciAdi = model.KullaniciAdi.Trim();

            if (await _context.Kullanicilar
                .AnyAsync(k => k.KullaniciAdi == kullaniciAdi))
            {
                ModelState.AddModelError(
                    nameof(model.KullaniciAdi),
                    "Bu kullanıcı adı zaten alınmış.");

                return View(model);
            }

            var kullanici = new Kullanici
            {
                KullaniciAdi = kullaniciAdi
            };

            kullanici.SifreHash =
                _passwordHasher.HashPassword(kullanici, model.Sifre);

            _context.Kullanicilar.Add(kullanici);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Giris));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cikis()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Giris));
        }
    }
}