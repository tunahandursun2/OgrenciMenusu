using System.ComponentModel.DataAnnotations;

namespace OgrenciMenusu.Models
{
    public class KayitViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3-50 karakter olmalıdır.")]
        [Display(Name = "Kullanıcı adı")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Sifre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre tekrarını girin.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Sifre), ErrorMessage = "Şifreler eşleşmiyor.")]
        [Display(Name = "Şifre tekrar")]
        public string SifreTekrar { get; set; } = string.Empty;
    }
}
