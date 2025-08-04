using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PBL3.Migrations
{
    public partial class PBL3Fix3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiemSos",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DiemHoiDong = table.Column<int>(type: "int", nullable: false),
                    DiemHuongDan = table.Column<int>(type: "int", nullable: false),
                    DiemPhanBien = table.Column<int>(type: "int", nullable: false),
                    DiemTrungBinh = table.Column<int>(type: "int", nullable: false),
                    DoAnId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiemSos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DiemSos_DoAns_DoAnId",
                        column: x => x.DoAnId,
                        principalTable: "DoAns",
                        principalColumn: "DoAnId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DiemSos_DoAnId",
                table: "DiemSos",
                column: "DoAnId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiemSos");
        }
    }
}
