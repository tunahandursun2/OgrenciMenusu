namespace OgrenciMenusu.Models
{
    public class Konu
    {
        public int Id { get; set; }

        public int KullaniciId { get; set; }

        public int DersId { get; set; }

        public string KonuAdi { get; set; } = "";

        public bool Tamamlandi { get; set; }
    }
}