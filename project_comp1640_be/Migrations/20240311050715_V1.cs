using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectcomp1640be.Migrations
{
    /// <inheritdoc />
<<<<<<<< HEAD:project_comp1640_be/Migrations/20240305054904_update-database.cs
    public partial class updatedatabase : Migration
========
    public partial class V1 : Migration
>>>>>>>> 58d28babae6d54cfb0f4dc3c545e2d82bfbed28e:project_comp1640_be/Migrations/20240311050715_V1.cs
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
<<<<<<<< HEAD:project_comp1640_be/Migrations/20240305054904_update-database.cs
                    academicyeartitle = table.Column<string>(name: "academic_year_title", type: "nvarchar(max)", nullable: false),
                    academicYearstartClosureDate = table.Column<DateTime>(name: "academic_Year_startClosureDate", type: "datetime2", nullable: false),
========
                    academicyeartitle = table.Column<string>(name: "academic_year_title", type: "nvarchar(max)", nullable: true),
                    academicYearstartClosureDate = table.Column<DateTime>(name: "academic_Year_startClosureDate", type: "datetime2", nullable: true),
>>>>>>>> 58d28babae6d54cfb0f4dc3c545e2d82bfbed28e:project_comp1640_be/Migrations/20240311050715_V1.cs
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
<<<<<<<< HEAD:project_comp1640_be/Migrations/20240305054904_update-database.cs
                    facultyname = table.Column<string>(name: "faculty_name", type: "nvarchar(max)", nullable: false)
========
                    facultyname = table.Column<string>(name: "faculty_name", type: "nvarchar(max)", nullable: true)
>>>>>>>> 58d28babae6d54cfb0f4dc3c545e2d82bfbed28e:project_comp1640_be/Migrations/20240311050715_V1.cs
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
<<<<<<<< HEAD:project_comp1640_be/Migrations/20240305054904_update-database.cs
                    rolename = table.Column<string>(name: "role_name", type: "nvarchar(max)", nullable: false)
========
                    rolename = table.Column<string>(name: "role_name", type: "nvarchar(max)", nullable: true)
>>>>>>>> 58d28babae6d54cfb0f4dc3c545e2d82bfbed28e:project_comp1640_be/Migrations/20240311050715_V1.cs
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
<<<<<<<< HEAD:project_comp1640_be/Migrations/20240305054904_update-database.cs
                    userusername = table.Column<string>(name: "user_username", type: "nvarchar(20)", maxLength: 20, nullable: false),
                    useremail = table.Column<string>(name: "user_email", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    userpassword = table.Column<string>(name: "user_password", type: "nvarchar(max)", nullable: false),
                    userconfirmpassword = table.Column<string>(name: "user_confirm_password", type: "nvarchar(max)", nullable: false),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    refeshtoken = table.Column<string>(name: "refesh_token", type: "nvarchar(max)", nullable: true),
                    refeshtokenexprytime = table.Column<DateTime>(name: "refesh_token_exprytime", type: "datetime2", nullable: false),
                    resetpasswordtoken = table.Column<string>(name: "reset_password_token", type: "nvarchar(max)", nullable: true),
                    resetpasswordexprytime = table.Column<DateTime>(name: "reset_password_exprytime", type: "datetime2", nullable: false),
========
                    userusername = table.Column<string>(name: "user_username", type: "nvarchar(max)", nullable: true),
                    useremail = table.Column<string>(name: "user_email", type: "nvarchar(max)", nullable: true),
                    userpassword = table.Column<string>(name: "user_password", type: "nvarchar(max)", nullable: true),
                    userconfirmpassword = table.Column<string>(name: "user_confirm_password", type: "nvarchar(max)", nullable: true),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    refeshtoken = table.Column<string>(name: "refesh_token", type: "nvarchar(max)", nullable: true),
                    refeshtokenexprytime = table.Column<DateTime>(name: "refesh_token_exprytime", type: "datetime2", nullable: true),
                    resetpasswordtoken = table.Column<string>(name: "reset_password_token", type: "nvarchar(max)", nullable: true),
                    resetpasswordexprytime = table.Column<DateTime>(name: "reset_password_exprytime", type: "datetime2", nullable: true),
>>>>>>>> 58d28babae6d54cfb0f4dc3c545e2d82bfbed28e:project_comp1640_be/Migrations/20240311050715_V1.cs
                    userstatus = table.Column<int>(name: "user_status", type: "int", nullable: false),
                    useravatar = table.Column<string>(name: "user_avatar", type: "nvarchar(max)", nullable: true),
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
                        principalColumn: "faculty_id");
                    table.ForeignKey(
                        name: "FK_Users_Roles_user_role_id",
                        column: x => x.userroleid,
                        principalTable: "Roles",
                        principalColumn: "role_id");
                });

            migrationBuilder.CreateTable(
                name: "Contributions",
                columns: table => new
                {
                    contributionid = table.Column<int>(name: "contribution_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
<<<<<<<< HEAD:project_comp1640_be/Migrations/20240305054904_update-database.cs
                    contributiontitle = table.Column<string>(name: "contribution_title", type: "nvarchar(max)", nullable: false),
                    contributioncontent = table.Column<string>(name: "contribution_content", type: "nvarchar(max)", nullable: false),
                    contributionimage = table.Column<string>(name: "contribution_image", type: "nvarchar(max)", nullable: false),
                    contributionsubmitiondate = table.Column<DateTime>(name: "contribution_submition_date", type: "datetime2", nullable: false),
                    IsEnabled = table.Column<int>(type: "int", nullable: false),
                    IsSelected = table.Column<int>(type: "int", nullable: false),
                    IsView = table.Column<int>(type: "int", nullable: false),
                    contributionuserid = table.Column<int>(name: "contribution_user_id", type: "int", nullable: false),
                    contributionacademicid = table.Column<int>(name: "contribution_academic_id", type: "int", nullable: false)
========
                    contributiontitle = table.Column<string>(name: "contribution_title", type: "nvarchar(max)", nullable: true),
                    contributioncontent = table.Column<string>(name: "contribution_content", type: "nvarchar(max)", nullable: true),
                    contributionimage = table.Column<string>(name: "contribution_image", type: "nvarchar(max)", nullable: true),
                    contributionsubmitiondate = table.Column<DateTime>(name: "contribution_submition_date", type: "datetime2", nullable: true),
                    IsEnabled = table.Column<int>(type: "int", nullable: true),
                    IsSelected = table.Column<int>(type: "int", nullable: true),
                    IsView = table.Column<int>(type: "int", nullable: true),
                    contributionuserid = table.Column<int>(name: "contribution_user_id", type: "int", nullable: false),
                    contributionacademicyearsid = table.Column<int>(name: "contribution_academic_years_id", type: "int", nullable: false)
>>>>>>>> 58d28babae6d54cfb0f4dc3c545e2d82bfbed28e:project_comp1640_be/Migrations/20240311050715_V1.cs
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contributions", x => x.contributionid);
                    table.ForeignKey(
<<<<<<<< HEAD:project_comp1640_be/Migrations/20240305054904_update-database.cs
                        name: "FK_Contributions_Academic_Years_contribution_academic_id",
                        column: x => x.contributionacademicid,
========
                        name: "FK_Contributions_Academic_Years_contribution_academic_years_id",
                        column: x => x.contributionacademicyearsid,
>>>>>>>> 58d28babae6d54cfb0f4dc3c545e2d82bfbed28e:project_comp1640_be/Migrations/20240311050715_V1.cs
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
                name: "Marketing_Comments",
                columns: table => new
                {
                    commentid = table.Column<int>(name: "comment_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
<<<<<<<< HEAD:project_comp1640_be/Migrations/20240305054904_update-database.cs
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    commentdate = table.Column<DateTime>(name: "comment_date", type: "datetime2", nullable: false),
                    commentsuserid = table.Column<int>(name: "comments_user_id", type: "int", nullable: false),
                    commentscontributionid = table.Column<int>(name: "comments_contribution_id", type: "int", nullable: false)
========
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    commentdate = table.Column<DateTime>(name: "comment_date", type: "datetime2", nullable: true),
                    commentuserid = table.Column<int>(name: "comment_user_id", type: "int", nullable: false),
                    commentcontributionid = table.Column<int>(name: "comment_contribution_id", type: "int", nullable: false)
>>>>>>>> 58d28babae6d54cfb0f4dc3c545e2d82bfbed28e:project_comp1640_be/Migrations/20240311050715_V1.cs
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marketing_Comments", x => x.commentid);
                    table.ForeignKey(
<<<<<<<< HEAD:project_comp1640_be/Migrations/20240305054904_update-database.cs
                        name: "FK_Marketing_Comments_Contributions_comments_contribution_id",
                        column: x => x.commentscontributionid,
                        principalTable: "Contributions",
                        principalColumn: "contribution_id");
                    table.ForeignKey(
                        name: "FK_Marketing_Comments_Users_comments_user_id",
                        column: x => x.commentsuserid,
========
                        name: "FK_Marketing_Comments_Contributions_comment_contribution_id",
                        column: x => x.commentcontributionid,
                        principalTable: "Contributions",
                        principalColumn: "contribution_id");
                    table.ForeignKey(
                        name: "FK_Marketing_Comments_Users_comment_user_id",
                        column: x => x.commentuserid,
>>>>>>>> 58d28babae6d54cfb0f4dc3c545e2d82bfbed28e:project_comp1640_be/Migrations/20240311050715_V1.cs
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
<<<<<<<< HEAD:project_comp1640_be/Migrations/20240305054904_update-database.cs
                name: "IX_Contributions_contribution_academic_id",
                table: "Contributions",
                column: "contribution_academic_id");
========
                name: "IX_Contributions_contribution_academic_years_id",
                table: "Contributions",
                column: "contribution_academic_years_id");
>>>>>>>> 58d28babae6d54cfb0f4dc3c545e2d82bfbed28e:project_comp1640_be/Migrations/20240311050715_V1.cs

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_contribution_user_id",
                table: "Contributions",
                column: "contribution_user_id");

            migrationBuilder.CreateIndex(
<<<<<<<< HEAD:project_comp1640_be/Migrations/20240305054904_update-database.cs
                name: "IX_Marketing_Comments_comments_contribution_id",
                table: "Marketing_Comments",
                column: "comments_contribution_id");

            migrationBuilder.CreateIndex(
                name: "IX_Marketing_Comments_comments_user_id",
                table: "Marketing_Comments",
                column: "comments_user_id");
========
                name: "IX_Marketing_Comments_comment_contribution_id",
                table: "Marketing_Comments",
                column: "comment_contribution_id");

            migrationBuilder.CreateIndex(
                name: "IX_Marketing_Comments_comment_user_id",
                table: "Marketing_Comments",
                column: "comment_user_id");
>>>>>>>> 58d28babae6d54cfb0f4dc3c545e2d82bfbed28e:project_comp1640_be/Migrations/20240311050715_V1.cs

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
