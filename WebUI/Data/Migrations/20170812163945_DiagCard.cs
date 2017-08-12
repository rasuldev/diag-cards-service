using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebUI.Data.Migrations
{
    public partial class DiagCard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRemoved",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "DiagnosticCards",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosticCards_UserId",
                table: "DiagnosticCards",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiagnosticCards_AspNetUsers_UserId",
                table: "DiagnosticCards",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiagnosticCards_AspNetUsers_UserId",
                table: "DiagnosticCards");

            migrationBuilder.DropIndex(
                name: "IX_DiagnosticCards_UserId",
                table: "DiagnosticCards");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsRemoved",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DiagnosticCards");
        }
    }
}
