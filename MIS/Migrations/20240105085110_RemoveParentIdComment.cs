using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIS.Migrations
{
    /// <inheritdoc />
    public partial class RemoveParentIdComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "InspectionCommentModel");

            migrationBuilder.AddColumn<Guid>(
                name: "Parent",
                table: "InspectionCommentModel",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Parent",
                table: "InspectionCommentModel");

            migrationBuilder.AddColumn<string>(
                name: "ParentId",
                table: "InspectionCommentModel",
                type: "text",
                nullable: true);
        }
    }
}
