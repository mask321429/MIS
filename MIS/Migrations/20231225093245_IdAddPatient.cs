using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIS.Migrations
{
    /// <inheritdoc />
    public partial class IdAddPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Patient",
                table: "Inspections",
                newName: "PatientId");

            migrationBuilder.RenameColumn(
                name: "Doctor",
                table: "Inspections",
                newName: "DoctorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "Inspections",
                newName: "Patient");

            migrationBuilder.RenameColumn(
                name: "DoctorId",
                table: "Inspections",
                newName: "Doctor");
        }
    }
}
