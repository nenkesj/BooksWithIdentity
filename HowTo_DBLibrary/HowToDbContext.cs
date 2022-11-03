using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HowTo_DBLibrary
{
    public partial class HowToDBContext : DbContext
    {
        public HowToDBContext()
        {
        }

        public HowToDBContext(DbContextOptions<HowToDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Attempt> Attempts { get; set; } = null!;
        public virtual DbSet<Code> Codes { get; set; } = null!;
        public virtual DbSet<HowTo> HowTos { get; set; } = null!;
        public virtual DbSet<Info> Infos { get; set; } = null!;
        public virtual DbSet<Key> Keys { get; set; } = null!;
        public virtual DbSet<Node> Nodes { get; set; } = null!;
        public virtual DbSet<Note> Notes { get; set; } = null!;
        public virtual DbSet<Observation> Observations { get; set; } = null!;
        public virtual DbSet<Picture> Pictures { get; set; } = null!;
        public virtual DbSet<Problem> Problems { get; set; } = null!;
        public virtual DbSet<Summary> Summaries { get; set; } = null!;
        public virtual DbSet<Task> Tasks { get; set; } = null!;
        public virtual DbSet<Tree> Trees { get; set; } = null!;
        public virtual DbSet<Type> Types { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=HowToDB;Trusted_Connection=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attempt>(entity =>
            {
                entity.Property(e => e.AttemptId).HasColumnName("AttemptID");

                entity.Property(e => e.Attempt1)
                    .IsUnicode(false)
                    .HasColumnName("Attempt")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.CompletedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('January 1, 1753')");

                entity.Property(e => e.Outcome)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.Succeeded)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.TaskId).HasColumnName("TaskID");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.Attempts)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Attempt_Task_fk");
            });

            modelBuilder.Entity<Code>(entity =>
            {
                entity.ToTable("Code");

                entity.Property(e => e.CodeId).HasColumnName("CodeID");

                entity.Property(e => e.Code1)
                    .HasColumnType("text")
                    .HasColumnName("Code")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.NodeId).HasColumnName("NodeID");

                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.HasOne(d => d.Node)
                    .WithMany(p => p.Codes)
                    .HasForeignKey(d => d.NodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Code_Node_fk");
            });

            modelBuilder.Entity<HowTo>(entity =>
            {
                entity.ToTable("HowTo");

                entity.Property(e => e.HowToId).HasColumnName("HowToID");

                entity.Property(e => e.Client)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.NodeId).HasColumnName("NodeID");

                entity.Property(e => e.Topic)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(' ')");

                entity.HasOne(d => d.Node)
                    .WithMany(p => p.HowTos)
                    .HasForeignKey(d => d.NodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HowTo_Node_fk");
            });

            modelBuilder.Entity<Info>(entity =>
            {
                entity.ToTable("Info");

                entity.Property(e => e.InfoId).HasColumnName("InfoID");

                entity.Property(e => e.Heading)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.InfoText)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.NodeId).HasColumnName("NodeID");

                entity.Property(e => e.TreeId).HasColumnName("TreeID");

                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.HasOne(d => d.Node)
                    .WithMany(p => p.Infos)
                    .HasForeignKey(d => d.NodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Info_Node_fk");

                entity.HasOne(d => d.Tree)
                    .WithMany(p => p.Infos)
                    .HasForeignKey(d => d.TreeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Info_Tree_fk");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Infos)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Info_Type_fk");
            });

            modelBuilder.Entity<Key>(entity =>
            {
                entity.Property(e => e.KeyId).HasColumnName("KeyID");

                entity.Property(e => e.KeyText)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.NodeId).HasColumnName("NodeID");

                entity.Property(e => e.TreeId).HasColumnName("TreeID");

                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.HasOne(d => d.Node)
                    .WithMany(p => p.Keys)
                    .HasForeignKey(d => d.NodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Key_Node_fk");

                entity.HasOne(d => d.Tree)
                    .WithMany(p => p.Keys)
                    .HasForeignKey(d => d.TreeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Key_Tree_fk");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Keys)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Key_Type_fk");
            });

            modelBuilder.Entity<Node>(entity =>
            {
                entity.Property(e => e.NodeId).HasColumnName("NodeID");

                entity.Property(e => e.Heading)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.NodeText)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.ParentNodeId).HasColumnName("ParentNodeID");

                entity.Property(e => e.TreeId).HasColumnName("TreeID");

                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.HasOne(d => d.Tree)
                    .WithMany(p => p.Nodes)
                    .HasForeignKey(d => d.TreeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Node_Tree_fk");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Nodes)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Node_Type_fk");
            });

            modelBuilder.Entity<Note>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Details)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.NodeId).HasColumnName("NodeID");

                entity.Property(e => e.NoteId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("NoteID");

                entity.Property(e => e.Text)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(' ')");

                entity.HasOne(d => d.Node)
                    .WithMany()
                    .HasForeignKey(d => d.NodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Note_Node_fk");
            });

            modelBuilder.Entity<Observation>(entity =>
            {
                entity.Property(e => e.ObservationId).HasColumnName("ObservationID");

                entity.Property(e => e.Comment)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.Observation1)
                    .IsUnicode(false)
                    .HasColumnName("Observation")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.ProblemId).HasColumnName("ProblemID");

                entity.HasOne(d => d.Problem)
                    .WithMany(p => p.Observations)
                    .HasForeignKey(d => d.ProblemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Observation_Problem_fk");
            });

            modelBuilder.Entity<Picture>(entity =>
            {
                entity.Property(e => e.PictureId).HasColumnName("PictureID");

                entity.Property(e => e.InfoId).HasColumnName("InfoID");

                entity.Property(e => e.NodeId).HasColumnName("NodeID");

                entity.Property(e => e.Picture1)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("Picture");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.HasOne(d => d.Node)
                    .WithMany(p => p.Pictures)
                    .HasForeignKey(d => d.NodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Picture_Node_fk");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Pictures)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Picture_Type_fk");
            });

            modelBuilder.Entity<Problem>(entity =>
            {
                entity.Property(e => e.ProblemId).HasColumnName("ProblemID");

                entity.Property(e => e.Client)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Details)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.Impacts)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.Lpar)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NodeId).HasColumnName("NodeID");

                entity.Property(e => e.Occurred)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('January 1, 1753')");

                entity.Property(e => e.ProblemSystem)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(' ')");

                entity.HasOne(d => d.Node)
                    .WithMany(p => p.Problems)
                    .HasForeignKey(d => d.NodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Problem_Node_fk");
            });

            modelBuilder.Entity<Summary>(entity =>
            {
                entity.Property(e => e.SummaryId).HasColumnName("SummaryID");

                entity.Property(e => e.NodeId).HasColumnName("NodeID");

                entity.Property(e => e.Summary1)
                    .HasColumnType("text")
                    .HasColumnName("Summary")
                    .HasDefaultValueSql("(' ')");

                entity.HasOne(d => d.Node)
                    .WithMany(p => p.Summaries)
                    .HasForeignKey(d => d.NodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Summary_Node_fk");
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.Property(e => e.TaskId).HasColumnName("TaskID");

                entity.Property(e => e.Client)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.CompletedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('January 1, 1753')");

                entity.Property(e => e.Instructions)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.NodeId).HasColumnName("NodeID");

                entity.Property(e => e.RequestSystem)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.StartedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('January 1, 1753')");

                entity.Property(e => e.WhereAt)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(' ')");

                entity.HasOne(d => d.Node)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.NodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Task_Node_fk");
            });

            modelBuilder.Entity<Tree>(entity =>
            {
                entity.Property(e => e.TreeId).HasColumnName("TreeID");

                entity.Property(e => e.Heading)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Trees)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Tree_Type_fk");
            });

            modelBuilder.Entity<Type>(entity =>
            {
                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.Property(e => e.Category)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.Label)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(' ')");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
