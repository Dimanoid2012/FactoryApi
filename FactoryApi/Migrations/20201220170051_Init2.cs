using Microsoft.EntityFrameworkCore.Migrations;

namespace FactoryApi.Migrations
{
    public partial class Init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Images",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "41e5abbc-202c-4b65-bc48-a8ae8a14722f",
                column: "ConcurrencyStamp",
                value: "07364805-b6b5-49fd-8c47-aa626cfd4610");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "84f1aac4-d856-4839-853b-e62c49867d7e",
                column: "ConcurrencyStamp",
                value: "3f4857ec-5896-49cb-90da-695d21f33b76");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "88b5386c-1904-4326-97f7-14a497549c49",
                column: "ConcurrencyStamp",
                value: "7ef61046-db40-41dc-ae21-4c78e722b0a0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b5a01fe8-225a-4bc8-aa91-cb17305b80f9",
                column: "ConcurrencyStamp",
                value: "f13a9186-1bac-4884-b6e9-307e71f72999");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c5872579-9861-4889-a89e-dbce38c0134d",
                column: "ConcurrencyStamp",
                value: "875a734f-5b08-42ef-a1ba-ea9d1f15a7d0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f8c8bc95-23f8-45ba-8b2d-88352bfd3289",
                column: "ConcurrencyStamp",
                value: "2d64bc21-9f70-41b6-b8fe-8af59df380d9");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Images");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "41e5abbc-202c-4b65-bc48-a8ae8a14722f",
                column: "ConcurrencyStamp",
                value: "975dcc26-15ee-4e31-b2b2-1426180fe5f3");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "84f1aac4-d856-4839-853b-e62c49867d7e",
                column: "ConcurrencyStamp",
                value: "44d16119-3fee-4cfc-843e-7c3cf85f647e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "88b5386c-1904-4326-97f7-14a497549c49",
                column: "ConcurrencyStamp",
                value: "e6d44412-ecf0-4840-9f42-a77690e04de3");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b5a01fe8-225a-4bc8-aa91-cb17305b80f9",
                column: "ConcurrencyStamp",
                value: "2ca0f484-26a9-4450-887b-40c1947703bd");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c5872579-9861-4889-a89e-dbce38c0134d",
                column: "ConcurrencyStamp",
                value: "82ba9fc8-4423-4cb5-adf9-5667928ea42f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f8c8bc95-23f8-45ba-8b2d-88352bfd3289",
                column: "ConcurrencyStamp",
                value: "df5b17ff-0f2b-4f79-a323-79095b718110");
        }
    }
}
