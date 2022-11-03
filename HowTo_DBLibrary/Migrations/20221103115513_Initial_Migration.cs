using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HowTo_DBLibrary.Migrations
{
    public partial class Initial_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.CreateTable(
                name: "Types",
                columns: table => new
                {
                    TypeID = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false, defaultValueSql: "(' ')"),
                    Category = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false, defaultValueSql: "(' ')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Types", x => x.TypeID);
                });

            migrationBuilder.CreateTable(
                name: "Trees",
                columns: table => new
                {
                    TreeID = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Heading = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValueSql: "(' ')"),
                    TypeID = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trees", x => x.TreeID);
                    table.ForeignKey(
                        name: "Tree_Type_fk",
                        column: x => x.TypeID,
                        principalTable: "Types",
                        principalColumn: "TypeID");
                });

            migrationBuilder.CreateTable(
                name: "Nodes",
                columns: table => new
                {
                    NodeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TreeID = table.Column<short>(type: "smallint", nullable: false),
                    TypeID = table.Column<short>(type: "smallint", nullable: false),
                    ParentNodeID = table.Column<int>(type: "int", nullable: false),
                    TreeLevel = table.Column<short>(type: "smallint", nullable: false),
                    Heading = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValueSql: "(' ')"),
                    NodeText = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false, defaultValueSql: "(' ')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nodes", x => x.NodeID);
                    table.ForeignKey(
                        name: "Node_Tree_fk",
                        column: x => x.TreeID,
                        principalTable: "Trees",
                        principalColumn: "TreeID");
                    table.ForeignKey(
                        name: "Node_Type_fk",
                        column: x => x.TypeID,
                        principalTable: "Types",
                        principalColumn: "TypeID");
                });

            migrationBuilder.CreateTable(
                name: "Code",
                columns: table => new
                {
                    CodeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NodeID = table.Column<int>(type: "int", nullable: false),
                    TypeID = table.Column<short>(type: "smallint", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false, defaultValueSql: "(' ')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Code", x => x.CodeID);
                    table.ForeignKey(
                        name: "Code_Node_fk",
                        column: x => x.NodeID,
                        principalTable: "Nodes",
                        principalColumn: "NodeID");
                });

            migrationBuilder.CreateTable(
                name: "HowTo",
                columns: table => new
                {
                    HowToID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NodeID = table.Column<int>(type: "int", nullable: false),
                    Topic = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValueSql: "(' ')"),
                    Client = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValueSql: "(' ')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HowTo", x => x.HowToID);
                    table.ForeignKey(
                        name: "HowTo_Node_fk",
                        column: x => x.NodeID,
                        principalTable: "Nodes",
                        principalColumn: "NodeID");
                });

            migrationBuilder.CreateTable(
                name: "Info",
                columns: table => new
                {
                    InfoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NodeID = table.Column<int>(type: "int", nullable: false),
                    TreeID = table.Column<short>(type: "smallint", nullable: false),
                    TypeID = table.Column<short>(type: "smallint", nullable: false),
                    Heading = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValueSql: "((0))"),
                    InfoText = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Info", x => x.InfoID);
                    table.ForeignKey(
                        name: "Info_Node_fk",
                        column: x => x.NodeID,
                        principalTable: "Nodes",
                        principalColumn: "NodeID");
                    table.ForeignKey(
                        name: "Info_Tree_fk",
                        column: x => x.TreeID,
                        principalTable: "Trees",
                        principalColumn: "TreeID");
                    table.ForeignKey(
                        name: "Info_Type_fk",
                        column: x => x.TypeID,
                        principalTable: "Types",
                        principalColumn: "TypeID");
                });

            migrationBuilder.CreateTable(
                name: "Keys",
                columns: table => new
                {
                    KeyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TreeID = table.Column<short>(type: "smallint", nullable: false),
                    NodeID = table.Column<int>(type: "int", nullable: false),
                    TypeID = table.Column<short>(type: "smallint", nullable: false),
                    KeyText = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, defaultValueSql: "(' ')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keys", x => x.KeyID);
                    table.ForeignKey(
                        name: "Key_Node_fk",
                        column: x => x.NodeID,
                        principalTable: "Nodes",
                        principalColumn: "NodeID");
                    table.ForeignKey(
                        name: "Key_Tree_fk",
                        column: x => x.TreeID,
                        principalTable: "Trees",
                        principalColumn: "TreeID");
                    table.ForeignKey(
                        name: "Key_Type_fk",
                        column: x => x.TypeID,
                        principalTable: "Types",
                        principalColumn: "TypeID");
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    NoteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NodeID = table.Column<int>(type: "int", nullable: false),
                    Details = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false, defaultValueSql: "(' ')"),
                    Text = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false, defaultValueSql: "(' ')")
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "Note_Node_fk",
                        column: x => x.NodeID,
                        principalTable: "Nodes",
                        principalColumn: "NodeID");
                });

            migrationBuilder.CreateTable(
                name: "Pictures",
                columns: table => new
                {
                    PictureID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NodeID = table.Column<int>(type: "int", nullable: false),
                    TypeID = table.Column<short>(type: "smallint", nullable: false),
                    Picture = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValueSql: "(' ')"),
                    PictureSize = table.Column<int>(type: "int", nullable: false),
                    DisplayAt = table.Column<short>(type: "smallint", nullable: false),
                    DisplayStopAt = table.Column<short>(type: "smallint", nullable: false),
                    InfoID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pictures", x => x.PictureID);
                    table.ForeignKey(
                        name: "Picture_Node_fk",
                        column: x => x.NodeID,
                        principalTable: "Nodes",
                        principalColumn: "NodeID");
                    table.ForeignKey(
                        name: "Picture_Type_fk",
                        column: x => x.TypeID,
                        principalTable: "Types",
                        principalColumn: "TypeID");
                });

            migrationBuilder.CreateTable(
                name: "Problems",
                columns: table => new
                {
                    ProblemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NodeID = table.Column<int>(type: "int", nullable: false),
                    ProblemSystem = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValueSql: "(' ')"),
                    ProblemNo = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValueSql: "(' ')"),
                    Occurred = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "('January 1, 1753')"),
                    Impacts = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValueSql: "(' ')"),
                    Details = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false, defaultValueSql: "(' ')"),
                    Client = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Lpar = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Problems", x => x.ProblemID);
                    table.ForeignKey(
                        name: "Problem_Node_fk",
                        column: x => x.NodeID,
                        principalTable: "Nodes",
                        principalColumn: "NodeID");
                });

            migrationBuilder.CreateTable(
                name: "Summaries",
                columns: table => new
                {
                    SummaryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NodeID = table.Column<int>(type: "int", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: false, defaultValueSql: "(' ')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Summaries", x => x.SummaryID);
                    table.ForeignKey(
                        name: "Summary_Node_fk",
                        column: x => x.NodeID,
                        principalTable: "Nodes",
                        principalColumn: "NodeID");
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    TaskID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NodeID = table.Column<int>(type: "int", nullable: false),
                    RequestSystem = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValueSql: "(' ')"),
                    RequestNo = table.Column<int>(type: "int", nullable: false),
                    StartedOn = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "('January 1, 1753')"),
                    CompletedOn = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "('January 1, 1753')"),
                    WhereAt = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValueSql: "(' ')"),
                    Client = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValueSql: "(' ')"),
                    Instructions = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false, defaultValueSql: "(' ')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.TaskID);
                    table.ForeignKey(
                        name: "Task_Node_fk",
                        column: x => x.NodeID,
                        principalTable: "Nodes",
                        principalColumn: "NodeID");
                });

            migrationBuilder.CreateTable(
                name: "Observations",
                columns: table => new
                {
                    ObservationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProblemID = table.Column<int>(type: "int", nullable: false),
                    Observation = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false, defaultValueSql: "(' ')"),
                    Comment = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false, defaultValueSql: "(' ')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observations", x => x.ObservationID);
                    table.ForeignKey(
                        name: "Observation_Problem_fk",
                        column: x => x.ProblemID,
                        principalTable: "Problems",
                        principalColumn: "ProblemID");
                });

            migrationBuilder.CreateTable(
                name: "Attempts",
                columns: table => new
                {
                    AttemptID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskID = table.Column<int>(type: "int", nullable: false),
                    CompletedOn = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "('January 1, 1753')"),
                    Succeeded = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    Attempt = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false, defaultValueSql: "(' ')"),
                    Outcome = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false, defaultValueSql: "(' ')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attempts", x => x.AttemptID);
                    table.ForeignKey(
                        name: "Attempt_Task_fk",
                        column: x => x.TaskID,
                        principalTable: "Tasks",
                        principalColumn: "TaskID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attempts_TaskID",
                table: "Attempts",
                column: "TaskID");

            migrationBuilder.CreateIndex(
                name: "IX_Code_NodeID",
                table: "Code",
                column: "NodeID");

            migrationBuilder.CreateIndex(
                name: "IX_HowTo_NodeID",
                table: "HowTo",
                column: "NodeID");

            migrationBuilder.CreateIndex(
                name: "IX_Info_NodeID",
                table: "Info",
                column: "NodeID");

            migrationBuilder.CreateIndex(
                name: "IX_Info_TreeID",
                table: "Info",
                column: "TreeID");

            migrationBuilder.CreateIndex(
                name: "IX_Info_TypeID",
                table: "Info",
                column: "TypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Keys_NodeID",
                table: "Keys",
                column: "NodeID");

            migrationBuilder.CreateIndex(
                name: "IX_Keys_TreeID",
                table: "Keys",
                column: "TreeID");

            migrationBuilder.CreateIndex(
                name: "IX_Keys_TypeID",
                table: "Keys",
                column: "TypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_TreeID",
                table: "Nodes",
                column: "TreeID");

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_TypeID",
                table: "Nodes",
                column: "TypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_NodeID",
                table: "Notes",
                column: "NodeID");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_ProblemID",
                table: "Observations",
                column: "ProblemID");

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_NodeID",
                table: "Pictures",
                column: "NodeID");

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_TypeID",
                table: "Pictures",
                column: "TypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_NodeID",
                table: "Problems",
                column: "NodeID");

            migrationBuilder.CreateIndex(
                name: "IX_Summaries_NodeID",
                table: "Summaries",
                column: "NodeID");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_NodeID",
                table: "Tasks",
                column: "NodeID");

            migrationBuilder.CreateIndex(
                name: "IX_Trees_TypeID",
                table: "Trees",
                column: "TypeID");*/
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //Down method should likely never be run on an existing database for the first migration.
        }
    }
}
