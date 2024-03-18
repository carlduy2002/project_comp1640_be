using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectcomp1640be.Migrations
{
    /// <inheritdoc />
    public partial class updatedatabasewithforeignkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Faculties_facultiesfaculty_id",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_role_id",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "Users",
                newName: "user_role_id");

            migrationBuilder.RenameColumn(
                name: "facultiesfaculty_id",
                table: "Users",
                newName: "user_faculty_id");

            migrationBuilder.RenameIndex(
                name: "IX_Users_role_id",
                table: "Users",
                newName: "IX_Users_user_role_id");

            migrationBuilder.RenameIndex(
                name: "IX_Users_facultiesfaculty_id",
                table: "Users",
                newName: "IX_Users_user_faculty_id");

            migrationBuilder.AlterColumn<string>(
                name: "user_status",
                table: "Users",
                type: "varchar(10)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "user_confirm_password",
                table: "Users",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "user_avatar",
                table: "Users",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "token",
                table: "Users",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "reset_password_token",
                table: "Users",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "reset_password_exprytime",
                table: "Users",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "refesh_token_exprytime",
                table: "Users",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "refesh_token",
                table: "Users",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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
                name: "FK_Users_Faculties_user_faculty_id",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_user_role_id",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "user_role_id",
                table: "Users",
                newName: "role_id");

            migrationBuilder.RenameColumn(
                name: "user_faculty_id",
                table: "Users",
                newName: "facultiesfaculty_id");

            migrationBuilder.RenameIndex(
                name: "IX_Users_user_role_id",
                table: "Users",
                newName: "IX_Users_role_id");

            migrationBuilder.RenameIndex(
                name: "IX_Users_user_faculty_id",
                table: "Users",
                newName: "IX_Users_facultiesfaculty_id");

            migrationBuilder.AlterColumn<int>(
                name: "user_status",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)");

            migrationBuilder.AlterColumn<string>(
                name: "user_confirm_password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "user_avatar",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AlterColumn<string>(
                name: "token",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "reset_password_token",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "reset_password_exprytime",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "refesh_token_exprytime",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "refesh_token",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Faculties_facultiesfaculty_id",
                table: "Users",
                column: "facultiesfaculty_id",
                principalTable: "Faculties",
                principalColumn: "faculty_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_role_id",
                table: "Users",
                column: "role_id",
                principalTable: "Roles",
                principalColumn: "role_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
