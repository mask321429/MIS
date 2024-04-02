using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIS.Migrations
{
    /// <inheritdoc />
    public partial class IdDelet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inspections_Users_DoctorId",
                table: "Inspections");

            migrationBuilder.DropForeignKey(
                name: "FK_Inspections_patientModels_PatientId",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_DoctorId",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_PatientId",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Inspections");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId",
                table: "Inspections",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PatientId",
                table: "Inspections",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_DoctorId",
                table: "Inspections",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_PatientId",
                table: "Inspections",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspections_Users_DoctorId",
                table: "Inspections",
                column: "DoctorId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspections_patientModels_PatientId",
                table: "Inspections",
                column: "PatientId",
                principalTable: "patientModels",
                principalColumn: "Id");
        }
    }
}
