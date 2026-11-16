namespace OgrenciMenusu.Models
{
    public class Odev
    {
        public int Id { get; set; }

        public int KullaniciId { get; set; }

        public int? DersId { get; set; }

        public string DersAdi { get; set; } = "";

        public string OdevAdi { get; set; } = "";

        public DateTime TeslimTarihi { get; set; }

        public bool Tamamlandi { get; set; }

        public DateTime? TamamlanmaTarihi { get; set; }
    }
}