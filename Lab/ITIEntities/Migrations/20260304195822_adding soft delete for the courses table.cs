using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITIEntities.Migrations
{
    /// <inheritdoc />
    public partial class addingsoftdeleteforthecoursestable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourses_Courses_CrsNo",
                table: "StudentCourses");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Courses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourses_Courses_CrsNo",
                table: "StudentCourses",
                column: "CrsNo",
                principalTable: "Courses",
                principalColumn: "CrsId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourses_Courses_CrsNo",
                table: "StudentCourses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Courses");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourses_Courses_CrsNo",
                table: "StudentCourses",
                column: "CrsNo",
                principalTable: "Courses",
                principalColumn: "CrsId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
