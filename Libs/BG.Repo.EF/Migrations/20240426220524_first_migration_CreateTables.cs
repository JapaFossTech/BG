using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BG.Repo.EF.Migrations
{
    /// <inheritdoc />
    public partial class first_migration_CreateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoardGames",
                columns: table => new
                {
                    BoardGameID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(127)", maxLength: 127, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    MinPlayers = table.Column<int>(type: "int", nullable: false),
                    MaxPlayers = table.Column<int>(type: "int", nullable: false),
                    PlayTime = table.Column<int>(type: "int", nullable: false),
                    MinAge = table.Column<int>(type: "int", nullable: false),
                    UsersRated = table.Column<int>(type: "int", nullable: false),
                    RatingAverage = table.Column<double>(type: "float", nullable: false),
                    BGGRank = table.Column<int>(type: "int", nullable: false),
                    ComplexityAverage = table.Column<double>(type: "float", nullable: false),
                    OwnedUsers = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardGames", x => x.BoardGameID);
                });

            migrationBuilder.CreateTable(
                name: "Domains",
                columns: table => new
                {
                    DomainID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DomainDesc = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Domains", x => x.DomainID);
                });

            migrationBuilder.CreateTable(
                name: "Mechanics",
                columns: table => new
                {
                    MechanicID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MechanicDesc = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mechanics", x => x.MechanicID);
                });

            migrationBuilder.CreateTable(
                name: "BoardGamesDomains",
                columns: table => new
                {
                    BoardGamesDomainID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoardGameID = table.Column<int>(type: "int", nullable: false),
                    DomainID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardGamesDomains", x => x.BoardGamesDomainID);
                    table.ForeignKey(
                        name: "FK_BoardGamesDomains_BoardGames_BoardGameID",
                        column: x => x.BoardGameID,
                        principalTable: "BoardGames",
                        principalColumn: "BoardGameID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoardGamesDomains_Domains_DomainID",
                        column: x => x.DomainID,
                        principalTable: "Domains",
                        principalColumn: "DomainID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BoardGamesMechanics",
                columns: table => new
                {
                    BoardGamesMechanicID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoardGameID = table.Column<int>(type: "int", nullable: false),
                    MechanicID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardGamesMechanics", x => x.BoardGamesMechanicID);
                    table.ForeignKey(
                        name: "FK_BoardGamesMechanics_BoardGames_BoardGameID",
                        column: x => x.BoardGameID,
                        principalTable: "BoardGames",
                        principalColumn: "BoardGameID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoardGamesMechanics_Mechanics_MechanicID",
                        column: x => x.MechanicID,
                        principalTable: "Mechanics",
                        principalColumn: "MechanicID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoardGamesDomains_BoardGameID",
                table: "BoardGamesDomains",
                column: "BoardGameID");

            migrationBuilder.CreateIndex(
                name: "IX_BoardGamesDomains_DomainID",
                table: "BoardGamesDomains",
                column: "DomainID");

            migrationBuilder.CreateIndex(
                name: "IX_BoardGamesMechanics_BoardGameID",
                table: "BoardGamesMechanics",
                column: "BoardGameID");

            migrationBuilder.CreateIndex(
                name: "IX_BoardGamesMechanics_MechanicID",
                table: "BoardGamesMechanics",
                column: "MechanicID");

            migrationBuilder.CreateIndex(
                name: "IX_Domains_DomainDesc",
                table: "Domains",
                column: "DomainDesc",
                unique: true)
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Mechanics_MechanicDesc",
                table: "Mechanics",
                column: "MechanicDesc",
                unique: true)
                .Annotation("SqlServer:Clustered", false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoardGamesDomains");

            migrationBuilder.DropTable(
                name: "BoardGamesMechanics");

            migrationBuilder.DropTable(
                name: "Domains");

            migrationBuilder.DropTable(
                name: "BoardGames");

            migrationBuilder.DropTable(
                name: "Mechanics");
        }
    }
}
