using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectcomp1640be.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatedatabasewithusergender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "account_gender",
                table: "Users",
                newName: "user_gender");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "user_gender",
                table: "Users",
                newName: "account_gender");
        }
    }
}
