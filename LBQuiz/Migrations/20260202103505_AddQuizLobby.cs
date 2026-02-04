using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LBQuiz.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizLobby : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'[QuizLobby]') IS NULL
                BEGIN
                    CREATE TABLE [QuizLobby] (
                        [Id] int NOT NULL IDENTITY,
                        [QuizId] int NOT NULL,
                        [JoinCode] nvarchar(6) NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        [IsActive] bit NOT NULL,
                        CONSTRAINT [PK_QuizLobby] PRIMARY KEY ([Id])
                    );
                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizLobby");
        }
    }
}
