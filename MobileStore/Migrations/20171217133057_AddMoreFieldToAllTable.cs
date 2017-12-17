using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MobileStore.Migrations
{
    public partial class AddMoreFieldToAllTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Period",
                table: "WarrantyCard");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedDate",
                table: "WarrantyDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsPrinted",
                table: "WarrantyDetail",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnedDate",
                table: "WarrantyDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "WarrantyDate",
                table: "WarrantyDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "WarrantyCard",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrinted",
                table: "WarrantyCard",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrinted",
                table: "Order",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "period",
                table: "ModelFromSupplier",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PictureOneUrl",
                table: "Model",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PictureThreeUrl",
                table: "Model",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PictureTwoUrl",
                table: "Model",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Constant",
                columns: table => new
                {
                    ConstantID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Parameter = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Constant", x => x.ConstantID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Constant");

            migrationBuilder.DropColumn(
                name: "ExpectedDate",
                table: "WarrantyDetail");

            migrationBuilder.DropColumn(
                name: "IsPrinted",
                table: "WarrantyDetail");

            migrationBuilder.DropColumn(
                name: "ReturnedDate",
                table: "WarrantyDetail");

            migrationBuilder.DropColumn(
                name: "WarrantyDate",
                table: "WarrantyDetail");

            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "WarrantyCard");

            migrationBuilder.DropColumn(
                name: "IsPrinted",
                table: "WarrantyCard");

            migrationBuilder.DropColumn(
                name: "IsPrinted",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "period",
                table: "ModelFromSupplier");

            migrationBuilder.DropColumn(
                name: "PictureOneUrl",
                table: "Model");

            migrationBuilder.DropColumn(
                name: "PictureThreeUrl",
                table: "Model");

            migrationBuilder.DropColumn(
                name: "PictureTwoUrl",
                table: "Model");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "Period",
                table: "WarrantyCard",
                nullable: false,
                defaultValue: 0);
        }
    }
}
