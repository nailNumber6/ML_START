using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MLSTART_GUI.Migrations
{
    /// <inheritdoc />
    public partial class InitMigrationAttempt1000 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Login = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Password = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Authorizationdate = table.Column<DateTime>(name: "Authorization date", type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Login);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
