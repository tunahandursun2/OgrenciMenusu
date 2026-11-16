using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using OgrenciMenusu.Models;

var builder = WebApplication.CreateBuilder(args);

// MVC servislerini ekle
builder.Services.AddControllersWithViews();

// Giriş sistemi
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Hesap/Giris";
        options.AccessDeniedPath = "/Hesap/Giris";
    });

// SQL Server bağlantısı
builder.Services.AddDbContext<OgrenciDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("OgrenciDB")
    ));

var app = builder.Build();

// Uygulamanın çalışma ayarları
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// MVC yönlendirmesi
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Veritabanı tablolarını oluştur
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OgrenciDbContext>();

    await db.Database.ExecuteSqlRawAsync("""
        IF OBJECT_ID(N'dbo.Kullanicilar', N'U') IS NULL
        BEGIN
            CREATE TABLE dbo.Kullanicilar (
                Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Kullanicilar PRIMARY KEY,
                KullaniciAdi nvarchar(50) NOT NULL,
                SifreHash nvarchar(500) NOT NULL
            );

            CREATE UNIQUE INDEX IX_Kullanicilar_KullaniciAdi
            ON dbo.Kullanicilar(KullaniciAdi);
        END

        IF OBJECT_ID(N'dbo.Dersler', N'U') IS NULL
        BEGIN
            CREATE TABLE dbo.Dersler (
                Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Dersler PRIMARY KEY,
                KullaniciId int NOT NULL,
                DersAdi nvarchar(100) NOT NULL
            );
        END
        """);
}

app.Run();