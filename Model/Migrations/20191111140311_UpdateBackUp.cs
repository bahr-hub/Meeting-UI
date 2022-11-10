using Microsoft.EntityFrameworkCore.Migrations;

namespace Model.Migrations
{
    public partial class UpdateBackUp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModulePrivilege",
                columns: table => new
                {
                    FK_PrivilegeID = table.Column<int>(nullable: false),
                    FK_ModuleID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModulePrivilege", x => new { x.FK_PrivilegeID, x.FK_ModuleID });
                    table.ForeignKey(
                        name: "FK_Privilege_Module",
                        column: x => x.FK_ModuleID,
                        principalTable: "Module",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Privilege_Privilege",
                        column: x => x.FK_PrivilegeID,
                        principalTable: "Privilege",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModulePrivilege_FK_ModuleID",
                table: "ModulePrivilege",
                column: "FK_ModuleID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModulePrivilege");
        }
    }
}
