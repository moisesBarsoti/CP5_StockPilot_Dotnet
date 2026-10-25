using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockPilot.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_STOCKPILOT_SUPPLIER",
                columns: table => new
                {
                    SUPPLIER_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    ZIP_CODE = table.Column<string>(type: "NVARCHAR2(8)", maxLength: 8, nullable: false),
                    CITY = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    STATE = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_STOCKPILOT_SUPPLIER", x => x.SUPPLIER_ID);
                });

            migrationBuilder.CreateTable(
                name: "TB_STOCKPILOT_PRODUCT",
                columns: table => new
                {
                    PRODUCT_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    PRICE = table.Column<decimal>(type: "DECIMAL(12,2)", precision: 12, scale: 2, nullable: false),
                    STOCK_QUANTITY = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SUPPLIER_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_STOCKPILOT_PRODUCT", x => x.PRODUCT_ID);
                    table.ForeignKey(
                        name: "FK_TB_STOCKPILOT_PRODUCT_TB_STOCKPILOT_SUPPLIER_SUPPLIER_ID",
                        column: x => x.SUPPLIER_ID,
                        principalTable: "TB_STOCKPILOT_SUPPLIER",
                        principalColumn: "SUPPLIER_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_STOCKPILOT_PRODUCT_SUPPLIER_ID",
                table: "TB_STOCKPILOT_PRODUCT",
                column: "SUPPLIER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TB_STOCKPILOT_SUPPLIER_EMAIL",
                table: "TB_STOCKPILOT_SUPPLIER",
                column: "EMAIL",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_STOCKPILOT_PRODUCT");

            migrationBuilder.DropTable(
                name: "TB_STOCKPILOT_SUPPLIER");
        }
    }
}
