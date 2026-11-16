using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OgrenciMenusu.Migrations
{
    /// <inheritdoc />
    public partial class OdevEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Odevler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    DersId = table.Column<int>(type: "int", nullable: true),
                    DersAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OdevAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeslimTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tamamlandi = table.Column<bool>(type: "bit", nullable: false),
                    TamamlanmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Odevler", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Odevler");
        }
    }
}
