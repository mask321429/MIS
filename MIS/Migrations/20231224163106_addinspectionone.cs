using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIS.Migrations
{
    /// <inheritdoc />
    public partial class addinspectionone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InspectionCommentModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ParentId = table.Column<string>(type: "text", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Author = table.Column<string>(type: "text", nullable: false),
                    AuthorID = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionCommentModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InspectionDoctor",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    idDoctor = table.Column<Guid>(type: "uuid", nullable: false),
                    idInspection = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionDoctor", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "InspectionPatient",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    idPatient = table.Column<Guid>(type: "uuid", nullable: false),
                    idInspection = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionPatient", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Inspections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Anamnesis = table.Column<string>(type: "text", nullable: true),
                    Complaints = table.Column<string>(type: "text", nullable: true),
                    Treatment = table.Column<string>(type: "text", nullable: true),
                    Conclusion = table.Column<int>(type: "integer", nullable: true),
                    NextVisitDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeathDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BaseInspectionId = table.Column<Guid>(type: "uuid", nullable: true),
                    PreviousInspectionId = table.Column<Guid>(type: "uuid", nullable: true),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: true),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inspections_Users_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Inspections_patientModels_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patientModels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PatientDiagnoses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    idPatient = table.Column<Guid>(type: "uuid", nullable: false),
                    idDiagnosis = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientDiagnoses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ConsultationModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InspectionId = table.Column<Guid>(type: "uuid", nullable: true),
                    SpecialityId = table.Column<Guid>(type: "uuid", nullable: true),
                    CommentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsultationModel_InspectionCommentModel_CommentId",
                        column: x => x.CommentId,
                        principalTable: "InspectionCommentModel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ConsultationModel_Inspections_InspectionId",
                        column: x => x.InspectionId,
                        principalTable: "Inspections",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ConsultationModel_Specialiti_SpecialityId",
                        column: x => x.SpecialityId,
                        principalTable: "Specialiti",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DiagnosisModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    InspectionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiagnosisModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiagnosisModel_Inspections_InspectionId",
                        column: x => x.InspectionId,
                        principalTable: "Inspections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationModel_CommentId",
                table: "ConsultationModel",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationModel_InspectionId",
                table: "ConsultationModel",
                column: "InspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationModel_SpecialityId",
                table: "ConsultationModel",
                column: "SpecialityId");

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosisModel_InspectionId",
                table: "DiagnosisModel",
                column: "InspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_DoctorId",
                table: "Inspections",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_PatientId",
                table: "Inspections",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsultationModel");

            migrationBuilder.DropTable(
                name: "DiagnosisModel");

            migrationBuilder.DropTable(
                name: "InspectionDoctor");

            migrationBuilder.DropTable(
                name: "InspectionPatient");

            migrationBuilder.DropTable(
                name: "PatientDiagnoses");

            migrationBuilder.DropTable(
                name: "InspectionCommentModel");

            migrationBuilder.DropTable(
                name: "Inspections");
        }
    }
}
