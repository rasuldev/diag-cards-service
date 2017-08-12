using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebUI.Data.Migrations
{
    public partial class DiagCardAndUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

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

            migrationBuilder.CreateTable(
                name: "DiagnosticCards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AllowedMaxWeight = table.Column<int>(nullable: false),
                    BodyNumber = table.Column<string>(maxLength: 50, nullable: true),
                    BrakeType = table.Column<int>(nullable: false),
                    Category = table.Column<int>(nullable: false),
                    CategoryCommon = table.Column<int>(nullable: false),
                    DocumentIssueDate = table.Column<DateTime>(nullable: false),
                    DocumentIssuer = table.Column<string>(maxLength: 50, nullable: false),
                    DocumentNumber = table.Column<string>(maxLength: 6, nullable: false),
                    DocumentSeries = table.Column<string>(maxLength: 4, nullable: false),
                    DocumentType = table.Column<int>(nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: false),
                    Firstname = table.Column<string>(maxLength: 30, nullable: false),
                    FrameNumber = table.Column<string>(maxLength: 50, nullable: true),
                    FuelType = table.Column<int>(nullable: false),
                    IsForeigner = table.Column<bool>(nullable: false),
                    IssueYear = table.Column<int>(nullable: false),
                    Lastname = table.Column<string>(maxLength: 30, nullable: false),
                    Manufacturer = table.Column<string>(maxLength: 30, nullable: false),
                    Model = table.Column<string>(maxLength: 30, nullable: false),
                    Note = table.Column<string>(maxLength: 250, nullable: true),
                    OId = table.Column<string>(maxLength: 30, nullable: true),
                    Patronymic = table.Column<string>(maxLength: 30, nullable: true),
                    RegNumber = table.Column<string>(maxLength: 10, nullable: true),
                    Running = table.Column<int>(nullable: false),
                    TyreManufacturer = table.Column<string>(maxLength: 30, nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    VIN = table.Column<string>(maxLength: 17, nullable: true),
                    Weight = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiagnosticCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiagnosticCards_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosticCards_BodyNumber",
                table: "DiagnosticCards",
                column: "BodyNumber");

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosticCards_DocumentNumber",
                table: "DiagnosticCards",
                column: "DocumentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosticCards_DocumentSeries",
                table: "DiagnosticCards",
                column: "DocumentSeries");

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosticCards_FrameNumber",
                table: "DiagnosticCards",
                column: "FrameNumber");

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosticCards_RegNumber",
                table: "DiagnosticCards",
                column: "RegNumber");

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosticCards_UserId",
                table: "DiagnosticCards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosticCards_VIN",
                table: "DiagnosticCards",
                column: "VIN");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiagnosticCards");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsRemoved",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");
        }
    }
}
