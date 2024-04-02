using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIS.Migrations.ApplicationDbContextForMISMigrations
{
    /// <inheritdoc />
    public partial class addRootMIS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ROOT",
                table: "Records",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ROOT",
                table: "Records");
        }
    }
}
