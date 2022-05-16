using Microsoft.EntityFrameworkCore.Migrations;

namespace ResumeApi.Migrations
{
    public partial class AddEduatn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "Educations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Educations_PersonId",
                table: "Educations",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_Persons_PersonId",
                table: "Educations",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Educations_Persons_PersonId",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_Educations_PersonId",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Educations");
        }
    }
}
