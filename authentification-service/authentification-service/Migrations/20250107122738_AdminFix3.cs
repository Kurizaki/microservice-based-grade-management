using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace authentification_service.Migrations
{
    /// <inheritdoc />
    public partial class AdminFix3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            INSERT INTO Users (Id, Username, PasswordHash, IsAdmin)
            SELECT 4, 'admin', '$2a$12$EkvUZbfke.K2o2NgCsJAquUk1Ci2Fa/YW0YmScpfQLX62xWnWIbiu', 1
            WHERE NOT EXISTS (
            SELECT 1 FROM Users WHERE Id = 4 AND Username = 'admin');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
