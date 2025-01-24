using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_Project.Migrations
{
    /// <inheritdoc />
    public partial class TaskDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "DueTime",
                table: "Task");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueAt",
                table: "Task",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueAt",
                table: "Task");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DueDate",
                table: "Task",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "DueTime",
                table: "Task",
                type: "time",
                nullable: true);
        }
    }
}
