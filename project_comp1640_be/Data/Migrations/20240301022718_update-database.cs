using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectcomp1640be.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatedatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Academic_Years",
                columns: table => new
                {
                    academicyearid = table.Column<int>(name: "academic_year_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    academicyeartitle = table.Column<string>(name: "academic_year_title", type: "nvarchar(max)", nullable: false),
                    academicYearstartClosureDate = table.Column<DateTime>(name: "academic_Year_startClosureDate", type: "datetime2", nullable: false),
                    academicYearendClosureDate = table.Column<DateTime>(name: "academic_Year_endClosureDate", type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Academic_Years", x => x.academicyearid);
                });

            migrationBuilder.CreateTable(
                name: "Faculties",
                columns: table => new
                {
                    facultyid = table.Column<int>(name: "faculty_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    facultyname = table.Column<string>(name: "faculty_name", type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faculties", x => x.facultyid);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    roleid = table.Column<int>(name: "role_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rolename = table.Column<string>(name: "role_name", type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.roleid);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    userid = table.Column<int>(name: "user_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userusername = table.Column<string>(name: "user_username", type: "nvarchar(20)", maxLength: 20, nullable: false),
                    useremail = table.Column<string>(name: "user_email", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    userpassword = table.Column<string>(name: "user_password", type: "nvarchar(max)", nullable: false),
                    userconfirmpassword = table.Column<string>(name: "user_confirm_password", type: "nvarchar(max)", nullable: false),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    refeshtoken = table.Column<string>(name: "refesh_token", type: "nvarchar(max)", nullable: true),
                    refeshtokenexprytime = table.Column<DateTime>(name: "refesh_token_exprytime", type: "datetime2", nullable: false),
                    resetpasswordtoken = table.Column<string>(name: "reset_password_token", type: "nvarchar(max)", nullable: true),
                    resetpasswordexprytime = table.Column<DateTime>(name: "reset_password_exprytime", type: "datetime2", nullable: false),
                    accountstatus = table.Column<int>(name: "account_status", type: "int", nullable: false),
                    useravatar = table.Column<string>(name: "user_avatar", type: "nvarchar(max)", nullable: true),
                    roleid = table.Column<int>(name: "role_id", type: "int", nullable: false),
                    facultiesfacultyid = table.Column<int>(name: "facultiesfaculty_id", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.userid);
                    table.ForeignKey(
                        name: "FK_Users_Faculties_facultiesfaculty_id",
                        column: x => x.facultiesfacultyid,
                        principalTable: "Faculties",
                        principalColumn: "faculty_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Roles_role_id",
                        column: x => x.roleid,
                        principalTable: "Roles",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contributions",
                columns: table => new
                {
                    contributionid = table.Column<int>(name: "contribution_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    contributiontitle = table.Column<string>(name: "contribution_title", type: "nvarchar(max)", nullable: false),
                    contributioncontent = table.Column<string>(name: "contribution_content", type: "nvarchar(max)", nullable: false),
                    contributionimage = table.Column<string>(name: "contribution_image", type: "nvarchar(max)", nullable: false),
                    contributionsubmitiondate = table.Column<DateTime>(name: "contribution_submition_date", type: "datetime2", nullable: false),
                    IsEnabled = table.Column<int>(type: "int", nullable: false),
                    IsSelected = table.Column<int>(type: "int", nullable: false),
                    IsView = table.Column<int>(type: "int", nullable: false),
                    usersuserid = table.Column<int>(name: "usersuser_id", type: "int", nullable: true),
                    facultiesfacultyid = table.Column<int>(name: "facultiesfaculty_id", type: "int", nullable: true),
                    academicyearsacademicyearid = table.Column<int>(name: "academic_yearsacademic_year_id", type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contributions", x => x.contributionid);
                    table.ForeignKey(
                        name: "FK_Contributions_Academic_Years_academic_yearsacademic_year_id",
                        column: x => x.academicyearsacademicyearid,
                        principalTable: "Academic_Years",
                        principalColumn: "academic_year_id");
                    table.ForeignKey(
                        name: "FK_Contributions_Faculties_facultiesfaculty_id",
                        column: x => x.facultiesfacultyid,
                        principalTable: "Faculties",
                        principalColumn: "faculty_id");
                    table.ForeignKey(
                        name: "FK_Contributions_Users_usersuser_id",
                        column: x => x.usersuserid,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Marketing_Comments",
                columns: table => new
                {
                    commentid = table.Column<int>(name: "comment_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    commentdate = table.Column<DateTime>(name: "comment_date", type: "datetime2", nullable: false),
                    usersuserid = table.Column<int>(name: "usersuser_id", type: "int", nullable: true),
                    contributionscontributionid = table.Column<int>(name: "contributionscontribution_id", type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marketing_Comments", x => x.commentid);
                    table.ForeignKey(
                        name: "FK_Marketing_Comments_Contributions_contributionscontribution_id",
                        column: x => x.contributionscontributionid,
                        principalTable: "Contributions",
                        principalColumn: "contribution_id");
                    table.ForeignKey(
                        name: "FK_Marketing_Comments_Users_usersuser_id",
                        column: x => x.usersuserid,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_academic_yearsacademic_year_id",
                table: "Contributions",
                column: "academic_yearsacademic_year_id");

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_facultiesfaculty_id",
                table: "Contributions",
                column: "facultiesfaculty_id");

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_usersuser_id",
                table: "Contributions",
                column: "usersuser_id");

            migrationBuilder.CreateIndex(
                name: "IX_Marketing_Comments_contributionscontribution_id",
                table: "Marketing_Comments",
                column: "contributionscontribution_id");

            migrationBuilder.CreateIndex(
                name: "IX_Marketing_Comments_usersuser_id",
                table: "Marketing_Comments",
                column: "usersuser_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_facultiesfaculty_id",
                table: "Users",
                column: "facultiesfaculty_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_role_id",
                table: "Users",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Marketing_Comments");

            migrationBuilder.DropTable(
                name: "Contributions");

            migrationBuilder.DropTable(
                name: "Academic_Years");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Faculties");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
