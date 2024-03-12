using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectcomp1640be.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatedatabaserole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Marketing_Comments_Contributions_comments_contribution_id",
                table: "Marketing_Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Marketing_Comments_Users_comments_user_id",
                table: "Marketing_Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Faculties_user_faculty_id",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_user_role_id",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "role_name",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Marketing_Comments_Contributions_comments_contribution_id",
                table: "Marketing_Comments",
                column: "comments_contribution_id",
                principalTable: "Contributions",
                principalColumn: "contribution_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Marketing_Comments_Users_comments_user_id",
                table: "Marketing_Comments",
                column: "comments_user_id",
                principalTable: "Users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Faculties_user_faculty_id",
                table: "Users",
                column: "user_faculty_id",
                principalTable: "Faculties",
                principalColumn: "faculty_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_user_role_id",
                table: "Users",
                column: "user_role_id",
                principalTable: "Roles",
                principalColumn: "role_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Marketing_Comments_Contributions_comments_contribution_id",
                table: "Marketing_Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Marketing_Comments_Users_comments_user_id",
                table: "Marketing_Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Faculties_user_faculty_id",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_user_role_id",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "role_name",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Marketing_Comments_Contributions_comments_contribution_id",
                table: "Marketing_Comments",
                column: "comments_contribution_id",
                principalTable: "Contributions",
                principalColumn: "contribution_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Marketing_Comments_Users_comments_user_id",
                table: "Marketing_Comments",
                column: "comments_user_id",
                principalTable: "Users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Faculties_user_faculty_id",
                table: "Users",
                column: "user_faculty_id",
                principalTable: "Faculties",
                principalColumn: "faculty_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_user_role_id",
                table: "Users",
                column: "user_role_id",
                principalTable: "Roles",
                principalColumn: "role_id");
        }
    }
}
