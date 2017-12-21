using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MobileStore.Migrations
{
    public partial class ChangeFieldName_Period_ModelFromSupplier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "period",
                table: "ModelFromSupplier",
                newName: "Period");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Period",
                table: "ModelFromSupplier",
                newName: "period");
        }
    }
}
