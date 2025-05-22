using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SuperShop.Migrations
{
    public partial class initDb : Migration // Classe gerada automaticamente após executar o comando `add-migration InitialDB` no Package Manager Console.
                                            // OBS: "InitialDB" é só o nome dado à migration.
    {
        /// Define as alterações a serem aplicadas no banco de dados (ex: criação de tabelas).
        protected override void Up(MigrationBuilder migrationBuilder) // Código C# gerado pelo EF Core para definir a estrutura da tabela.
                                                                      // Para aplicar essa estrutura no banco de dados real (gerar SQL e executar),
                                                                      // é necessário rodar o comando `update-database` no Package Manager Console.
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastPurchase = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSale = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    Stock = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder) // meétodo que apaga a tabela
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
