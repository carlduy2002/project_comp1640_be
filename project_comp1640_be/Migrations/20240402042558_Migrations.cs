using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectcomp1640be.Migrations
{
    /// <inheritdoc />
    public partial class Migrations : Migration
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
                    academicyeartitle = table.Column<string>(name: "academic_year_title", type: "varchar(255)", nullable: false),
                    academicyearClosureDate = table.Column<DateTime>(name: "academic_year_ClosureDate", type: "datetime2", nullable: false),
                    academicyearFinalClosureDate = table.Column<DateTime>(name: "academic_year_FinalClosureDate", type: "datetime2", nullable: false)
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
                    facultyname = table.Column<string>(name: "faculty_name", type: "varchar(255)", nullable: false)
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
                    rolename = table.Column<string>(name: "role_name", type: "varchar(100)", nullable: false)
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
                    userusername = table.Column<string>(name: "user_username", type: "varchar(255)", nullable: false),
                    useremail = table.Column<string>(name: "user_email", type: "varchar(255)", nullable: false),
                    userpassword = table.Column<string>(name: "user_password", type: "varchar(255)", nullable: false),
                    userconfirmpassword = table.Column<string>(name: "user_confirm_password", type: "varchar(255)", nullable: true),
                    token = table.Column<string>(type: "varchar(255)", nullable: true),
                    refeshtoken = table.Column<string>(name: "refesh_token", type: "varchar(255)", nullable: true),
                    refeshtokenexprytime = table.Column<DateTime>(name: "refesh_token_exprytime", type: "datetime2", nullable: true),
                    resetpasswordtoken = table.Column<string>(name: "reset_password_token", type: "varchar(255)", nullable: true),
                    resetpasswordexprytime = table.Column<DateTime>(name: "reset_password_exprytime", type: "datetime2", nullable: true),
                    lastlogin = table.Column<DateTime>(name: "last_login", type: "datetime2", nullable: true),
                    totalworkduration = table.Column<int>(name: "total_work_duration", type: "int", nullable: true),
                    userstatus = table.Column<string>(name: "user_status", type: "varchar(10)", nullable: false),
                    useravatar = table.Column<string>(name: "user_avatar", type: "varchar(255)", nullable: true),
                    userroleid = table.Column<int>(name: "user_role_id", type: "int", nullable: false),
                    userfacultyid = table.Column<int>(name: "user_faculty_id", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.userid);
                    table.ForeignKey(
                        name: "FK_Users_Faculties_user_faculty_id",
                        column: x => x.userfacultyid,
                        principalTable: "Faculties",
                        principalColumn: "faculty_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Roles_user_role_id",
                        column: x => x.userroleid,
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
                    contributiontitle = table.Column<string>(name: "contribution_title", type: "varchar(255)", nullable: false),
                    contributioncontent = table.Column<string>(name: "contribution_content", type: "varchar(255)", nullable: false),
                    contributionimage = table.Column<string>(name: "contribution_image", type: "varchar(255)", nullable: false),
                    contributionsubmitiondate = table.Column<DateTime>(name: "contribution_submition_date", type: "datetime2", nullable: false),
                    IsEnabled = table.Column<string>(type: "varchar(10)", nullable: false),
                    IsSelected = table.Column<string>(type: "varchar(10)", nullable: false),
                    IsView = table.Column<string>(type: "varchar(10)", nullable: false),
                    IsPublic = table.Column<string>(type: "varchar(10)", nullable: false),
                    contributionuserid = table.Column<int>(name: "contribution_user_id", type: "int", nullable: false),
                    contributionacademicyearsid = table.Column<int>(name: "contribution_academic_years_id", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contributions", x => x.contributionid);
                    table.ForeignKey(
                        name: "FK_Contributions_Academic_Years_contribution_academic_years_id",
                        column: x => x.contributionacademicyearsid,
                        principalTable: "Academic_Years",
                        principalColumn: "academic_year_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contributions_Users_contribution_user_id",
                        column: x => x.contributionuserid,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "Marketing_Comments",
                columns: table => new
                {
                    commentid = table.Column<int>(name: "comment_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    commentcontent = table.Column<string>(name: "comment_content", type: "varchar(2000)", nullable: false),
                    commentdate = table.Column<DateTime>(name: "comment_date", type: "datetime2", nullable: false),
                    commentuserid = table.Column<int>(name: "comment_user_id", type: "int", nullable: false),
                    commentcontributionid = table.Column<int>(name: "comment_contribution_id", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marketing_Comments", x => x.commentid);
                    table.ForeignKey(
                        name: "FK_Marketing_Comments_Contributions_comment_contribution_id",
                        column: x => x.commentcontributionid,
                        principalTable: "Contributions",
                        principalColumn: "contribution_id");
                    table.ForeignKey(
                        name: "FK_Marketing_Comments_Users_comment_user_id",
                        column: x => x.commentuserid,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_contribution_academic_years_id",
                table: "Contributions",
                column: "contribution_academic_years_id");

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_contribution_user_id",
                table: "Contributions",
                column: "contribution_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Marketing_Comments_comment_contribution_id",
                table: "Marketing_Comments",
                column: "comment_contribution_id");

            migrationBuilder.CreateIndex(
                name: "IX_Marketing_Comments_comment_user_id",
                table: "Marketing_Comments",
                column: "comment_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Page_Views_page_view_user_id",
                table: "Page_Views",
                column: "page_view_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_user_faculty_id",
                table: "Users",
                column: "user_faculty_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_user_role_id",
                table: "Users",
                column: "user_role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Marketing_Comments");

            migrationBuilder.DropTable(
                name: "Page_Views");

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
