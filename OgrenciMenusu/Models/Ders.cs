namespace OgrenciMenusu.Models
{
    public class Ders
    {
        public int Id { get; set; }

        public int KullaniciId { get; set; }

        public string DersAdi { get; set; } = string.Empty;
    }
}