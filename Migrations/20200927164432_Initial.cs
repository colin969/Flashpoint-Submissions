using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace website.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Meta",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: true),
                    AlternateTitles = table.Column<string>(nullable: true),
                    Library = table.Column<string>(nullable: true),
                    Series = table.Column<string>(nullable: true),
                    Developer = table.Column<string>(nullable: true),
                    Publisher = table.Column<string>(nullable: true),
                    PlayMode = table.Column<string>(nullable: true),
                    ReleaseDate = table.Column<string>(nullable: true),
                    Version = table.Column<string>(nullable: true),
                    Languages = table.Column<string>(nullable: true),
                    Extreme = table.Column<string>(nullable: true),
                    Tags = table.Column<string>(nullable: true),
                    TagCategories = table.Column<string>(nullable: true),
                    Source = table.Column<string>(nullable: true),
                    Platform = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    ApplicationPath = table.Column<string>(nullable: true),
                    LaunchCommand = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    OriginalDescription = table.Column<string>(nullable: true),
                    CurationNotes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meta", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    fileName = table.Column<string>(nullable: true),
                    logoUrl = table.Column<string>(nullable: true),
                    size = table.Column<long>(nullable: false),
                    authorId = table.Column<string>(nullable: true),
                    submissionDate = table.Column<DateTime>(nullable: false),
                    statusUpdated = table.Column<DateTime>(nullable: false),
                    updatedById = table.Column<string>(nullable: true),
                    status = table.Column<string>(nullable: true),
                    metaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_Submissions_Meta_metaId",
                        column: x => x.metaId,
                        principalTable: "Meta",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionNote",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    submissionid = table.Column<int>(nullable: true),
                    authorId = table.Column<string>(nullable: true),
                    dateAdded = table.Column<DateTime>(nullable: false),
                    note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionNote", x => x.id);
                    table.ForeignKey(
                        name: "FK_SubmissionNote_Submissions_submissionid",
                        column: x => x.submissionid,
                        principalTable: "Submissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionNote_submissionid",
                table: "SubmissionNote",
                column: "submissionid");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_metaId",
                table: "Submissions",
                column: "metaId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubmissionNote");

            migrationBuilder.DropTable(
                name: "Submissions");

            migrationBuilder.DropTable(
                name: "Meta");
        }
    }
}
