using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIS.Migrations.ApplicationDbContextForMISMigrations
{
    /// <inheritdoc />
    public partial class addmis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Records",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    ACTUAL = table.Column<int>(type: "integer", nullable: false),
                    MKB_CODE = table.Column<string>(type: "text", nullable: false),
                    MKB_NAME = table.Column<string>(type: "text", nullable: false),
                    REC_CODE = table.Column<string>(type: "text", nullable: false),
                    ID_PARENT = table.Column<string>(type: "text", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Records", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Records");
        }
    }
}
