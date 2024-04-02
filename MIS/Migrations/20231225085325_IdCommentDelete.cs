using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIS.Migrations
{
    /// <inheritdoc />
    public partial class IdCommentDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultationModel_InspectionCommentModel_CommentId",
                table: "ConsultationModel");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsultationModel_Inspections_InspectionId",
                table: "ConsultationModel");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsultationModel_Specialiti_SpecialityId",
                table: "ConsultationModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConsultationModel",
                table: "ConsultationModel");

            migrationBuilder.DropIndex(
                name: "IX_ConsultationModel_CommentId",
                table: "ConsultationModel");

            migrationBuilder.DropIndex(
                name: "IX_ConsultationModel_SpecialityId",
                table: "ConsultationModel");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "ConsultationModel");

            migrationBuilder.DropColumn(
                name: "SpecialityId",
                table: "ConsultationModel");

            migrationBuilder.RenameTable(
                name: "ConsultationModel",
                newName: "ConsultationModels");

            migrationBuilder.RenameIndex(
                name: "IX_ConsultationModel_InspectionId",
                table: "ConsultationModels",
                newName: "IX_ConsultationModels_InspectionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConsultationModels",
                table: "ConsultationModels",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultationModels_Inspections_InspectionId",
                table: "ConsultationModels",
                column: "InspectionId",
                principalTable: "Inspections",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultationModels_Inspections_InspectionId",
                table: "ConsultationModels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConsultationModels",
                table: "ConsultationModels");

            migrationBuilder.RenameTable(
                name: "ConsultationModels",
                newName: "ConsultationModel");

            migrationBuilder.RenameIndex(
                name: "IX_ConsultationModels_InspectionId",
                table: "ConsultationModel",
                newName: "IX_ConsultationModel_InspectionId");

            migrationBuilder.AddColumn<Guid>(
                name: "CommentId",
                table: "ConsultationModel",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SpecialityId",
                table: "ConsultationModel",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConsultationModel",
                table: "ConsultationModel",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationModel_CommentId",
                table: "ConsultationModel",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationModel_SpecialityId",
                table: "ConsultationModel",
                column: "SpecialityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultationModel_InspectionCommentModel_CommentId",
                table: "ConsultationModel",
                column: "CommentId",
                principalTable: "InspectionCommentModel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultationModel_Inspections_InspectionId",
                table: "ConsultationModel",
                column: "InspectionId",
                principalTable: "Inspections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultationModel_Specialiti_SpecialityId",
                table: "ConsultationModel",
                column: "SpecialityId",
                principalTable: "Specialiti",
                principalColumn: "Id");
        }
    }
}
