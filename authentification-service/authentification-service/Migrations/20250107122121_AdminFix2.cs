using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace authentification_service.Migrations
{
    /// <inheritdoc />
    public partial class AdminFix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            UPDATE Users
            SET PasswordHash = '$2a$12$EkvUZbfke.K2o2NgCsJAquUk1Ci2Fa/YW0YmScpfQLX62xWnWIbiu'
            WHERE Id = 4 AND Username = 'admin' AND PasswordHash = '$2a$12$abcdefghijklmnopqrstuv';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
