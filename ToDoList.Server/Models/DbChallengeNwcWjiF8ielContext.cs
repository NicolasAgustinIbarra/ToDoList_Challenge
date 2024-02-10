using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ToDoList.Server.Models;

public partial class DbChallengeNwcWjiF8ielContext : DbContext
{
    public DbChallengeNwcWjiF8ielContext()
    {
    }

    public DbChallengeNwcWjiF8ielContext(DbContextOptions<DbChallengeNwcWjiF8ielContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Efmigrationshistory> Efmigrationshistories { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("Server=DESKTOP-EKQV02T;Database=db_challenge_nwc_wjiF8IEl;User=root;Password=123456;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("categories");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Efmigrationshistory>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PRIMARY");

            entity.ToTable("__efmigrationshistory");

            entity.Property(e => e.MigrationId).HasMaxLength(150);
            entity.Property(e => e.ProductVersion).HasMaxLength(32);
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tasks");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Completed).HasColumnName("completed");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Deadline)
                .HasColumnType("date")
                .HasColumnName("deadline");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasMany(d => d.Categories).WithMany(p => p.Tasks)
                .UsingEntity<Dictionary<string, object>>(
                    "CategoryTask",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("category_task_ibfk_2"),
                    l => l.HasOne<Task>().WithMany()
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("category_task_ibfk_1"),
                    j =>
                    {
                        j.HasKey("TaskId", "CategoryId").HasName("PRIMARY");
                        j.ToTable("category_task");
                        j.HasIndex(new[] { "CategoryId" }, "category_id");
                        j.IndexerProperty<int>("TaskId").HasColumnName("task_id");
                        j.IndexerProperty<int>("CategoryId").HasColumnName("category_id");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
