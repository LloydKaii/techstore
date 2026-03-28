using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechStore.Migrations
{
    /// <inheritdoc />
    public partial class PCBuild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PCBuilds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CPUId = table.Column<int>(type: "int", nullable: true),
                    MainboardId = table.Column<int>(type: "int", nullable: true),
                    RAMId = table.Column<int>(type: "int", nullable: true),
                    VGAId = table.Column<int>(type: "int", nullable: true),
                    PSUId = table.Column<int>(type: "int", nullable: true),
                    CaseId = table.Column<int>(type: "int", nullable: true),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCBuilds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PCBuilds_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PCBuilds_Products_CPUId",
                        column: x => x.CPUId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PCBuilds_Products_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PCBuilds_Products_MainboardId",
                        column: x => x.MainboardId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PCBuilds_Products_PSUId",
                        column: x => x.PSUId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PCBuilds_Products_RAMId",
                        column: x => x.RAMId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PCBuilds_Products_VGAId",
                        column: x => x.VGAId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PCBuilds_CaseId",
                table: "PCBuilds",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PCBuilds_CPUId",
                table: "PCBuilds",
                column: "CPUId");

            migrationBuilder.CreateIndex(
                name: "IX_PCBuilds_MainboardId",
                table: "PCBuilds",
                column: "MainboardId");

            migrationBuilder.CreateIndex(
                name: "IX_PCBuilds_PSUId",
                table: "PCBuilds",
                column: "PSUId");

            migrationBuilder.CreateIndex(
                name: "IX_PCBuilds_RAMId",
                table: "PCBuilds",
                column: "RAMId");

            migrationBuilder.CreateIndex(
                name: "IX_PCBuilds_UserId",
                table: "PCBuilds",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PCBuilds_VGAId",
                table: "PCBuilds",
                column: "VGAId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PCBuilds");
        }
    }
}
