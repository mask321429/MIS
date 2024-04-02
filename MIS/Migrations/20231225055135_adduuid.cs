using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIS.Migrations
{
    /// <inheritdoc />
    public partial class adduuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiagnosisModel_Inspections_InspectionId",
                table: "DiagnosisModel");

            migrationBuilder.DropColumn(
                name: "idDiagnosis",
                table: "PatientDiagnoses");

            migrationBuilder.AlterColumn<Guid>(
                name: "InspectionId",
                table: "DiagnosisModel",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_DiagnosisModel_Inspections_InspectionId",
                table: "DiagnosisModel",
                column: "InspectionId",
                principalTable: "Inspections",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiagnosisModel_Inspections_InspectionId",
                table: "DiagnosisModel");

            migrationBuilder.AddColumn<int>(
                name: "idDiagnosis",
                table: "PatientDiagnoses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "InspectionId",
                table: "DiagnosisModel",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DiagnosisModel_Inspections_InspectionId",
                table: "DiagnosisModel",
                column: "InspectionId",
                principalTable: "Inspections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
