using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v2groups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupMembers",
                table: "GroupMembers");

            migrationBuilder.RenameTable(
                name: "Groups",
                newName: "Groups2");

            migrationBuilder.RenameTable(
                name: "GroupMembers",
                newName: "GroupMembers2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups2",
                table: "Groups2",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupMembers2",
                table: "GroupMembers2",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups2",
                table: "Groups2");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupMembers2",
                table: "GroupMembers2");

            migrationBuilder.RenameTable(
                name: "Groups2",
                newName: "Groups");

            migrationBuilder.RenameTable(
                name: "GroupMembers2",
                newName: "GroupMembers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupMembers",
                table: "GroupMembers",
                column: "Id");
        }
    }
}
