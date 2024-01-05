using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class evetn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Events",
            //    columns: table => new
            //    {
            //        EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        When = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Owner = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        MemberLimit = table.Column<int>(type: "int", nullable: false),
            //        Tag = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        GroupId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Events", x => x.EventId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "EventsMembers",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        GroupId = table.Column<int>(type: "int", nullable: false),
            //        EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_EventsMembers", x => x.Id);
            //    });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "Events");

            //migrationBuilder.DropTable(
            //    name: "EventsMembers");
        }
    }
}
