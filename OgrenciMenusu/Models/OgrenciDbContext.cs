using Microsoft.EntityFrameworkCore;

namespace OgrenciMenusu.Models
{
    public class OgrenciDbContext : DbContext
    {
        public OgrenciDbContext(DbContextOptions<OgrenciDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ogrenci> Ogrenciler { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Ders> Dersler { get; set; }
        public DbSet<DersProgrami> DersProgramlari { get; set; }
        public DbSet<Konu> Konular { get; set; }
        public DbSet<Odev> Odevler { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Kullanici>()
                .HasIndex(k => k.KullaniciAdi)
                .IsUnique();

            modelBuilder.Entity<Kullanici>()
                .Property(k => k.KullaniciAdi)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Kullanici>()
                .Property(k => k.SifreHash)
                .HasMaxLength(500)
                .IsRequired();
        }
    }
}
