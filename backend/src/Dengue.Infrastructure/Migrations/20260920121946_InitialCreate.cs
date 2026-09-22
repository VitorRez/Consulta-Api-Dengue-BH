using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dengue.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DengueAlerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ew = table.Column<int>(type: "int", nullable: false),
                    Ey = table.Column<int>(type: "int", nullable: false),
                    DataIniSE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CasosEst = table.Column<double>(type: "float(18)", precision: 18, scale: 4, nullable: false),
                    Casos = table.Column<int>(type: "int", nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    PRt1 = table.Column<double>(type: "float(18)", precision: 18, scale: 6, nullable: true),
                    PInc100k = table.Column<double>(type: "float(18)", precision: 18, scale: 6, nullable: true),
                    Rt = table.Column<double>(type: "float(18)", precision: 18, scale: 6, nullable: true),
                    Pop = table.Column<double>(type: "float(18)", precision: 18, scale: 4, nullable: true),
                    Receptivo = table.Column<int>(type: "int", nullable: true),
                    Transmissao = table.Column<int>(type: "int", nullable: true),
                    NivelInc = table.Column<int>(type: "int", nullable: true),
                    NotifAccumYear = table.Column<int>(type: "int", nullable: true),
                    ImportedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DengueAlerts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DengueAlerts_Ew_Ey",
                table: "DengueAlerts",
                columns: new[] { "Ew", "Ey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DengueAlerts");
        }
    }
}
