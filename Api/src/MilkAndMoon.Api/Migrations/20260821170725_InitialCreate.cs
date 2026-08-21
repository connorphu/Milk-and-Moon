using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilkAndMoon.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    email = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "babies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_babies", x => x.id);
                    table.ForeignKey(
                        name: "fk_babies_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "diaper_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    diaper_type = table.Column<string>(type: "text", nullable: false),
                    pee_color = table.Column<string>(type: "text", nullable: true),
                    stool_color = table.Column<string[]>(type: "text[]", nullable: false, defaultValueSql: "'{}'"),
                    stool_texture = table.Column<string[]>(type: "text[]", nullable: false, defaultValueSql: "'{}'"),
                    rash = table.Column<string>(type: "text", nullable: true),
                    rash_location = table.Column<string[]>(type: "text[]", nullable: false, defaultValueSql: "'{}'"),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    baby_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    timezone = table.Column<string>(type: "text", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_diaper_logs", x => x.id);
                    table.CheckConstraint("diaper_logs_diaper_types", "diaper_type IN ('wet','poopy','both')");
                    table.CheckConstraint("diaper_logs_pee_colors", "pee_color IN ('clear','light yellow','dark yellow','orange')");
                    table.CheckConstraint("diaper_logs_rash", "rash IN ('mild','moderate','severe')");
                    table.CheckConstraint("diaper_logs_rash_locations", "rash_location <@ ARRAY['front','back','folds','aroundAnus','other']::text[]");
                    table.CheckConstraint("diaper_logs_stool_colors", "stool_color <@ ARRAY['black','yellow','green','brown','red','white','other']::text[]");
                    table.CheckConstraint("diaper_logs_stool_textures", "stool_texture <@ ARRAY['seedy','soft','loose','watery','mucus','hard','other']::text[]");
                    table.ForeignKey(
                        name: "fk_diaper_logs_babies_baby_id",
                        column: x => x.baby_id,
                        principalTable: "babies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "feed_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    bottle_size = table.Column<decimal>(type: "numeric(3,1)", precision: 3, scale: 1, nullable: false, defaultValue: 0m),
                    feed_type = table.Column<string>(type: "text", nullable: false),
                    breast_side = table.Column<string[]>(type: "text[]", nullable: false, defaultValueSql: "'{}'"),
                    milk_type = table.Column<string[]>(type: "text[]", nullable: false, defaultValueSql: "'{}'"),
                    milk_consumed = table.Column<decimal>(type: "numeric(3,1)", precision: 3, scale: 1, nullable: false, defaultValue: 0m),
                    end_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    baby_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    timezone = table.Column<string>(type: "text", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_feed_logs", x => x.id);
                    table.CheckConstraint("feed_logs_breast_side", "breast_side <@ ARRAY['left','right']::text[]");
                    table.CheckConstraint("feed_logs_feed_type", "feed_type IN ('bottle', 'breast')");
                    table.CheckConstraint("feed_logs_milk_type", "milk_type <@ ARRAY['breastmilk','formula']::text[]");
                    table.ForeignKey(
                        name: "fk_feed_logs_babies_baby_id",
                        column: x => x.baby_id,
                        principalTable: "babies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pump_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    left_amount = table.Column<decimal>(type: "numeric(3,1)", precision: 3, scale: 1, nullable: false, defaultValue: 0m),
                    right_amount = table.Column<decimal>(type: "numeric(3,1)", precision: 3, scale: 1, nullable: false, defaultValue: 0m),
                    end_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    baby_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    timezone = table.Column<string>(type: "text", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pump_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_pump_logs_babies_baby_id",
                        column: x => x.baby_id,
                        principalTable: "babies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sleep_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    location = table.Column<string>(type: "text", nullable: true),
                    wake_reasons = table.Column<string[]>(type: "text[]", nullable: false, defaultValueSql: "'{}'"),
                    end_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    baby_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    timezone = table.Column<string>(type: "text", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sleep_logs", x => x.id);
                    table.CheckConstraint("sleep_logs_locations", "location IN ('bassinet','crib','contactNap','stroller','car','other')");
                    table.CheckConstraint("sleep_logs_wake_reasons", "wake_reasons <@ ARRAY['hungry','diaper','naturally','noise','moved','unknown']::text[]");
                    table.ForeignKey(
                        name: "fk_sleep_logs_babies_baby_id",
                        column: x => x.baby_id,
                        principalTable: "babies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_babies_user_id",
                table: "babies",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_diaper_logs_baby_id",
                table: "diaper_logs",
                column: "baby_id");

            migrationBuilder.CreateIndex(
                name: "ix_feed_logs_baby_id",
                table: "feed_logs",
                column: "baby_id");

            migrationBuilder.CreateIndex(
                name: "ix_pump_logs_baby_id",
                table: "pump_logs",
                column: "baby_id");

            migrationBuilder.CreateIndex(
                name: "ix_sleep_logs_baby_id",
                table: "sleep_logs",
                column: "baby_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "diaper_logs");

            migrationBuilder.DropTable(
                name: "feed_logs");

            migrationBuilder.DropTable(
                name: "pump_logs");

            migrationBuilder.DropTable(
                name: "sleep_logs");

            migrationBuilder.DropTable(
                name: "babies");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
