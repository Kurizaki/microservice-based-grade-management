using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace authentification_service.Migrations
{
    /// <inheritdoc />
    public partial class AdminFix4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert admin user if not exists
            migrationBuilder.Sql(@"
            INSERT INTO Users (Username, PasswordHash, IsAdmin)
            SELECT 'admin', '$2a$12$EkvUZbfke.K2o2NgCsJAquUk1Ci2Fa/YW0YmScpfQLX62xWnWIbiu', 1
            WHERE NOT EXISTS (
            SELECT 1 FROM Users WHERE Username = 'admin'
            );");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
