using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ApiFinancas.Src.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // AJUSTE MANUAL: o EF gerou um AlterColumn de integer para uuid, mas o PostgreSQL
            // não converte int em uuid. Como category_id ainda não apontava para nenhuma tabela
            // (os valores antigos não têm significado), a coluna é apagada e recriada já como uuid.
            migrationBuilder.DropColumn(
                name: "category_id",
                schema: "finances",
                table: "transactions");

            migrationBuilder.AddColumn<Guid>(
                name: "category_id",
                schema: "finances",
                table: "transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "categories",
                schema: "finances",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.id);
                    table.ForeignKey(
                        name: "FK_categories_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "finances",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "finances",
                table: "categories",
                columns: new[] { "id", "name", "user_id" },
                values: new object[,]
                {
                    { new Guid("8b1f6c1e-3f0a-4c2d-9a51-0c6f2a1d7e01"), "Alimentação", null },
                    { new Guid("8b1f6c1e-3f0a-4c2d-9a51-0c6f2a1d7e02"), "Moradia", null },
                    { new Guid("8b1f6c1e-3f0a-4c2d-9a51-0c6f2a1d7e03"), "Transporte", null },
                    { new Guid("8b1f6c1e-3f0a-4c2d-9a51-0c6f2a1d7e04"), "Saúde", null },
                    { new Guid("8b1f6c1e-3f0a-4c2d-9a51-0c6f2a1d7e05"), "Educação", null },
                    { new Guid("8b1f6c1e-3f0a-4c2d-9a51-0c6f2a1d7e06"), "Lazer", null },
                    { new Guid("8b1f6c1e-3f0a-4c2d-9a51-0c6f2a1d7e07"), "Salário", null },
                    { new Guid("8b1f6c1e-3f0a-4c2d-9a51-0c6f2a1d7e08"), "Outros", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_transactions_category_id",
                schema: "finances",
                table: "transactions",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_categories_user_id_name",
                schema: "finances",
                table: "categories",
                columns: new[] { "user_id", "name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_categories_category_id",
                schema: "finances",
                table: "transactions",
                column: "category_id",
                principalSchema: "finances",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_categories_category_id",
                schema: "finances",
                table: "transactions");

            migrationBuilder.DropTable(
                name: "categories",
                schema: "finances");

            migrationBuilder.DropIndex(
                name: "IX_transactions_category_id",
                schema: "finances",
                table: "transactions");

            // AJUSTE MANUAL: mesmo motivo do Up (não há conversão de uuid para integer)
            migrationBuilder.DropColumn(
                name: "category_id",
                schema: "finances",
                table: "transactions");

            migrationBuilder.AddColumn<int>(
                name: "category_id",
                schema: "finances",
                table: "transactions",
                type: "integer",
                nullable: true);
        }
    }
}
