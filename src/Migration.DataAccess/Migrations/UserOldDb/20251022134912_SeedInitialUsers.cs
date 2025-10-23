using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Migration.DataAccess.Migrations.UserOldDb
{
    /// <inheritdoc />
    public partial class SeedInitialUsers : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UsersOld",
                columns: new[] { "LegacyUserId", "DocumentType", "FirstName", "LastName", "LegacyEmailAddress", "Status" },
                values: new object[,]
                {
                    { 1, "FACTURE", "Mario", "Bianchi", "mario.bianchi@email.com", "ACTIVE" },
                    { 2, "FACTURE", "Marco", "Verdi", "marco.verdi@email.com", "ACTIVE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UsersOld",
                keyColumn: "LegacyUserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UsersOld",
                keyColumn: "LegacyUserId",
                keyValue: 2);
        }
    }
}
