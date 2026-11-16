namespace OgrenciMenusu.Models
{
    public class DersProgrami
    {
        public int Id { get; set; }

        public int KullaniciId { get; set; }

        public int DersId { get; set; }

        public int Gun { get; set; }

        public int BaslangicSaat { get; set; }

        public int BaslangicDakika { get; set; }

        public int BitisSaat { get; set; }

        public int BitisDakika { get; set; }
    }
}