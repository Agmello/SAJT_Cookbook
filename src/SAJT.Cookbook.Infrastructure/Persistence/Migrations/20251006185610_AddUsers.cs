using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAJT.Cookbook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.Sql(@"INSERT INTO [Users] ([Id], [Name], [CreatedAtUtc], [UpdatedAtUtc])
SELECT DISTINCT r.[AuthorId], CONVERT(NVARCHAR(200), CONCAT('Legacy Author ', RIGHT(CONVERT(NVARCHAR(36), r.[AuthorId]), 8))), SYSUTCDATETIME(), SYSUTCDATETIME()
FROM [Recipes] r
WHERE r.[AuthorId] <> '00000000-0000-0000-0000-000000000000'
  AND NOT EXISTS (SELECT 1 FROM [Users] u WHERE u.[Id] = r.[AuthorId]);");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Users_AuthorId",
                table: "Recipes",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Users_AuthorId",
                table: "Recipes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
