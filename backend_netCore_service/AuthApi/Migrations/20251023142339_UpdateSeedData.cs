using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEIoGFYRderrQD75lDIaUlNXn0Pq9vAwjBPnPArFKYO1Ugmw1ChnIqKd4n1emh/bHvQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEDe/roRwj4kx6zqoCbxhrjvrQp1OX7/tsgsOhY36G9Zaw1mIoNoN/tzqxFTA7QhPpg==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEBqHeXkNBPiGUg2Tc2/fg8xpogvczcP2bSdwKVqV17VhgWUP3BLsh2xTk5bivMRDDw==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEOGUn0W6GDAH7QXzBt61XVHE/BKlE+6MktvM1Df/VR9Q/z+6PwdvARsWc7D/Kg/LDA==");
        }
    }
}
