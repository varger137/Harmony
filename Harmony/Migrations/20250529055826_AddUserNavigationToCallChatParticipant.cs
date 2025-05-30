using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Harmony.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNavigationToCallChatParticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CallChatParticipant_CallChats_CallChatId",
                table: "CallChatParticipant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CallChatParticipant",
                table: "CallChatParticipant");

            migrationBuilder.RenameTable(
                name: "CallChatParticipant",
                newName: "CallChatParticipants");

            migrationBuilder.RenameIndex(
                name: "IX_CallChatParticipant_CallChatId",
                table: "CallChatParticipants",
                newName: "IX_CallChatParticipants_CallChatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CallChatParticipants",
                table: "CallChatParticipants",
                columns: new[] { "UserId", "CallChatId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CallChatParticipants_CallChats_CallChatId",
                table: "CallChatParticipants",
                column: "CallChatId",
                principalTable: "CallChats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CallChatParticipants_Users_UserId",
                table: "CallChatParticipants",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CallChatParticipants_CallChats_CallChatId",
                table: "CallChatParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_CallChatParticipants_Users_UserId",
                table: "CallChatParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CallChatParticipants",
                table: "CallChatParticipants");

            migrationBuilder.RenameTable(
                name: "CallChatParticipants",
                newName: "CallChatParticipant");

            migrationBuilder.RenameIndex(
                name: "IX_CallChatParticipants_CallChatId",
                table: "CallChatParticipant",
                newName: "IX_CallChatParticipant_CallChatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CallChatParticipant",
                table: "CallChatParticipant",
                columns: new[] { "UserId", "CallChatId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CallChatParticipant_CallChats_CallChatId",
                table: "CallChatParticipant",
                column: "CallChatId",
                principalTable: "CallChats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
