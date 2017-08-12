using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebUI.Data.Migrations
{
    public partial class AddedSomeFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OId",
                table: "DiagnosticCards",
                newName: "СardId");

            migrationBuilder.AlterColumn<bool>(
                name: "IsApproved",
                table: "AspNetUsers",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "DiagnosticCards",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisteredDate",
                table: "DiagnosticCards",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "DiagnosticCards");

            migrationBuilder.DropColumn(
                name: "RegisteredDate",
                table: "DiagnosticCards");

            migrationBuilder.RenameColumn(
                name: "СardId",
                table: "DiagnosticCards",
                newName: "OId");

            migrationBuilder.AlterColumn<bool>(
                name: "IsApproved",
                table: "AspNetUsers",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);
        }
    }
}
