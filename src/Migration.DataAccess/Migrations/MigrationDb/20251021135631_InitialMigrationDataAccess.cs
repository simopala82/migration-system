#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Migration.DataAccess.Migrations.MigrationDb
{
    /// <inheritdoc />
    public partial class InitialMigrationDataAccess : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MigrationStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LegacyUserId = table.Column<int>(type: "int", nullable: false),
                    NewUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    ErrorDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminActionBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MigrationStatus", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MigrationStatus_LegacyUserId",
                table: "MigrationStatus",
                column: "LegacyUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MigrationStatus");
        }
    }
}
