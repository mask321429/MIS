using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIS.Migrations
{
    /// <inheritdoc />
    public partial class addForeginKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PatientModelId",
                table: "Inspections",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_PatientModelId",
                table: "Inspections",
                column: "PatientModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspections_patientModels_PatientModelId",
                table: "Inspections",
                column: "PatientModelId",
                principalTable: "patientModels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inspections_patientModels_PatientModelId",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_PatientModelId",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "PatientModelId",
                table: "Inspections");
        }
    }
}
