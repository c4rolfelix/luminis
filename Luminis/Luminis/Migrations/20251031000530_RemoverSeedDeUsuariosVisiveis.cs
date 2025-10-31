using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Luminis.Migrations
{
    /// <inheritdoc />
    public partial class RemoverSeedDeUsuariosVisiveis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a18c66e9-0260-466d-a789-21800f123456",
                column: "ConcurrencyStamp",
                value: "06834646-9f73-4852-9cad-05de52e3c3e8");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b18c66e9-0260-466d-a789-21800f123457",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "106a916f-9286-4aeb-92d8-ca76730f7b9f", "AQAAAAIAAYagAAAAEEQuN+iNa+ePwbjMAl0BMSJC/CBk9CefKSGpPDBMnZ9uO6VovGidCiUA5hv/X9FrbQ==", "662b4133-966b-4867-b3e0-e4d66384b7df" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c18c66e9-0260-466d-a789-21800f123458",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "129582f6-e910-472c-b7fd-44453305f744", "AQAAAAIAAYagAAAAEHkTvPpTuKlScledzt/+A6wveCZR3wimHaZmKaMAx8PQd0wZC3P5F81vPXfIvy2Olg==", "0d1f1ad8-f826-4041-86e1-798bd9461c00" });

            migrationBuilder.UpdateData(
                table: "Psicologos",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataCadastro",
                value: new DateTime(2025, 10, 30, 21, 5, 30, 123, DateTimeKind.Local).AddTicks(1028));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a18c66e9-0260-466d-a789-21800f123456",
                column: "ConcurrencyStamp",
                value: "7e052b19-a43e-4367-89cc-084017905fce");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b18c66e9-0260-466d-a789-21800f123457",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ec923c10-16bb-4a4c-9c82-cd75d5244ded", "AQAAAAIAAYagAAAAEKKWk6cGRiXxNvgwJWqCQF3ZMnqL2sn3/HhH9dQhMcqBU7G429lmDpAej5msXFuUYQ==", "32a642cc-9dcc-4448-8e66-9969af1f5eb3" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c18c66e9-0260-466d-a789-21800f123458",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7d715558-071f-461e-91b7-a488e8c915b8", "AQAAAAIAAYagAAAAEEJ0PuNQCyyNRhCiQSBveFtJQN1k3wq8RfxEnu9M9J9niuAVwYPqDhVKqsC5rZVK8Q==", "de7a4b54-75cf-4624-abd0-1d2797359366" });

            migrationBuilder.UpdateData(
                table: "Psicologos",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataCadastro",
                value: new DateTime(2025, 10, 30, 20, 56, 13, 843, DateTimeKind.Local).AddTicks(8251));
        }
    }
}
