using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAJT.Cookbook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeIngredientNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE i
SET
    [Name] = LOWER(LTRIM(RTRIM([Name]))),
    [PluralName] = CASE
        WHEN [PluralName] IS NULL THEN NULL
        WHEN LTRIM(RTRIM([PluralName])) = '' THEN NULL
        ELSE LOWER(LTRIM(RTRIM([PluralName])))
    END
FROM [Ingredients] AS i;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE i
SET
    [Name] = LOWER(LTRIM(RTRIM([Name]))),
    [PluralName] = CASE
        WHEN [PluralName] IS NULL THEN NULL
        WHEN LTRIM(RTRIM([PluralName])) = '' THEN NULL
        ELSE LOWER(LTRIM(RTRIM([PluralName])))
    END
FROM [Ingredients] AS i;");
        }
    }
}
