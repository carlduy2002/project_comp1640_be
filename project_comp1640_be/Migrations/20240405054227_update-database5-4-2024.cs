using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectcomp1640be.Migrations
{
    /// <inheritdoc />
    public partial class updatedatabase542024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "last_login",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "total_work_duration",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Page_Views",
                columns: table => new
                {
                    pageviewid = table.Column<int>(name: "page_view_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pageviewname = table.Column<string>(name: "page_view_name", type: "varchar(100)", nullable: false),
                    browsername = table.Column<string>(name: "browser_name", type: "varchar(100)", nullable: false),
                    timestamp = table.Column<DateTime>(name: "time_stamp", type: "datetime2", nullable: false),
                    totaltimeaccess = table.Column<int>(name: "total_time_access", type: "int", nullable: false),
                    pageviewuserid = table.Column<int>(name: "page_view_user_id", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Page_Views", x => x.pageviewid);
                    table.ForeignKey(
                        name: "FK_Page_Views_Users_page_view_user_id",
                        column: x => x.pageviewuserid,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Page_Views_page_view_user_id",
                table: "Page_Views",
                column: "page_view_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Page_Views");

            migrationBuilder.DropColumn(
                name: "last_login",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "total_work_duration",
                table: "Users");
        }
    }
}
