using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OgrenciMenusu.Migrations
{
    public partial class DersProgramiEklendi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DersProgramlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    KullaniciId = table.Column<int>(type: "int", nullable: false),

                    DersId = table.Column<int>(type: "int", nullable: false),

                    Gun = table.Column<int>(type: "int", nullable: false),

                    BaslangicSaat = table.Column<int>(type: "int", nullable: false),

                    BaslangicDakika = table.Column<int>(type: "int", nullable: false),

                    BitisSaat = table.Column<int>(type: "int", nullable: false),

                    BitisDakika = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DersProgramlari", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DersProgramlari");
        }
    }
}