using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LBQuiz.Migrations
{
    /// <inheritdoc />
    public partial class profilepicture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_Question_QuestionMultipleId",
                table: "Question");

            migrationBuilder.DropTable(
                name: "MultipleOptions");

            migrationBuilder.DropIndex(
                name: "IX_Question_QuestionMultipleId",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "QuestionMultipleId",
                table: "Question");

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePicture",
                table: "AspNetUsers",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "QuestionMultipleId",
                table: "Question",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MultipleOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColorString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorrectFalse = table.Column<bool>(type: "bit", nullable: false),
                    MultipleChoiceAnswerId = table.Column<int>(type: "int", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultipleOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MultipleOptions_Question_MultipleChoiceAnswerId",
                        column: x => x.MultipleChoiceAnswerId,
                        principalTable: "Question",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Question_QuestionMultipleId",
                table: "Question",
                column: "QuestionMultipleId");

            migrationBuilder.CreateIndex(
                name: "IX_MultipleOptions_MultipleChoiceAnswerId",
                table: "MultipleOptions",
                column: "MultipleChoiceAnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_Question_QuestionMultipleId",
                table: "Question",
                column: "QuestionMultipleId",
                principalTable: "Question",
                principalColumn: "Id");
        }
    }
}
