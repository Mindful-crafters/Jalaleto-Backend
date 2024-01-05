using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "GroupMembers",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        GroupId = table.Column<int>(type: "int", nullable: false),
            //        UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        Mail = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_GroupMembers", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Groups",
            //    columns: table => new
            //    {
            //        GroupId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Owner = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Groups", x => x.GroupId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Users",
            //    columns: table => new
            //    {
            //        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        Mail = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Birthday = table.Column<DateTime>(type: "Date", nullable: false),
            //        PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
            //        PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
            //        CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Users", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Reminders",
            //    columns: table => new
            //    {
            //        ReminderId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        LastModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        DaysBeforeToRemind = table.Column<int>(type: "int", nullable: false),
            //        RemindByEmail = table.Column<bool>(type: "bit", nullable: false),
            //        PriorityLevel = table.Column<int>(type: "int", nullable: false),
            //        Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Status = table.Column<int>(type: "int", nullable: false),
            //        UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Reminders", x => x.ReminderId);
            //        table.ForeignKey(
            //            name: "FK_Reminders_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Reminders_UserId",
            //    table: "Reminders",
            //    column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "GroupMembers");

            //migrationBuilder.DropTable(
            //    name: "Groups");

            //migrationBuilder.DropTable(
            //    name: "Reminders");

            //migrationBuilder.DropTable(
            //    name: "Users");
        }
    }
}
