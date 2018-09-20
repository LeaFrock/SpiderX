using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SpiderX.Proxy.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProxyEntities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Host = table.Column<string>(type: "VARCHAR(32)", nullable: true),
                    Port = table.Column<int>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    Category = table.Column<int>(nullable: false),
                    AnonymityDegree = table.Column<int>(nullable: false),
                    ResponseMilliseconds = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProxyEntities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProxyEntities_Host_Port",
                table: "ProxyEntities",
                columns: new[] { "Host", "Port" },
                unique: true,
                filter: "[Host] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProxyEntities");
        }
    }
}
