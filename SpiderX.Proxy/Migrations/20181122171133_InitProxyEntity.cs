using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SpiderX.Proxy.Migrations
{
	public partial class InitProxyEntity : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "ProxyEntity",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
					Host = table.Column<string>(type: "VARCHAR(32)", nullable: false),
					Port = table.Column<int>(nullable: false),
					Location = table.Column<string>(nullable: true),
					Category = table.Column<byte>(nullable: false),
					AnonymityDegree = table.Column<byte>(nullable: false),
					ResponseMilliseconds = table.Column<int>(nullable: false),
					UpdateTime = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "GETDATE()"),
					Source = table.Column<string>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ProxyEntity", x => x.Id);
				});

			migrationBuilder.CreateIndex(
				name: "IX_ProxyEntity_Host_Port",
				table: "ProxyEntity",
				columns: new[] { "Host", "Port" },
				unique: true);

			//Add a trigger to keep UpdateTime latest on UPDATE.
			migrationBuilder.Sql(
				@"CREATE TRIGGER [dbo].[ProxyEntity_UPDATE] ON [dbo].[ProxyEntity]
				AFTER UPDATE
				AS
				BEGIN
					SET NOCOUNT ON;

					IF((SELECT TRIGGER_NESTLEVEL()) > 1) RETURN;

					DECLARE @Id INT
					SELECT @Id = INSERTED.Id
					FROM INSERTED

					UPDATE dbo.ProxyEntity
					SET UpdateTime = GETUTCDATE()
					WHERE Id = @Id
				END");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "ProxyEntity");
		}
	}
}