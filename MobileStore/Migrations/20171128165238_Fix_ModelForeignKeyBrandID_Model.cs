using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MobileStore.Migrations
{
    public partial class Fix_ModelForeignKeyBrandID_Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Model_Brand_ModelID",
                table: "Model");

            migrationBuilder.AlterColumn<int>(
                name: "ModelID",
                table: "Model",
                type: "int",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.CreateIndex(
                name: "IX_Model_BrandID",
                table: "Model",
                column: "BrandID");

            migrationBuilder.AddForeignKey(
                name: "FK_Model_Brand_BrandID",
                table: "Model",
                column: "BrandID",
                principalTable: "Brand",
                principalColumn: "BrandID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Model_Brand_BrandID",
                table: "Model");

            migrationBuilder.DropIndex(
                name: "IX_Model_BrandID",
                table: "Model");

            migrationBuilder.AlterColumn<int>(
                name: "ModelID",
                table: "Model",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_Model_Brand_ModelID",
                table: "Model",
                column: "ModelID",
                principalTable: "Brand",
                principalColumn: "BrandID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
