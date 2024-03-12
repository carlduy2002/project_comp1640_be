using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectcomp1640be.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatedatabasewithgender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "account_gender",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "account_gender",
                table: "Users");
        }
    }
}
