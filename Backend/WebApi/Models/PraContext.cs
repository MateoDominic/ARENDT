using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Models;

public partial class PraContext : DbContext
{
    public PraContext()
    {
    }

    public PraContext(DbContextOptions<PraContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<Picture> Pictures { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Quiz> Quizzes { get; set; }

    public virtual DbSet<QuizHistory> QuizHistories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=ConnectionStrings:PRAcs");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Answers__3214EC273BAA1768");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AnswerText).HasMaxLength(150);
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

            entity.HasOne(d => d.Question).WithMany(p => p.Answers)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Answers__Questio__208CD6FA");
        });

        modelBuilder.Entity<Picture>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pictures__3214EC2784B0625A");

            entity.Property(e => e.Id).HasColumnName("ID");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3214EC275F1AF93E");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.PictureId).HasColumnName("PictureID");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");

            entity.HasOne(d => d.Picture).WithMany(p => p.Questions)
                .HasForeignKey(d => d.PictureId)
                .HasConstraintName("FK__Questions__Pictu__1CBC4616");

            entity.HasOne(d => d.Quiz).WithMany(p => p.Questions)
                .HasForeignKey(d => d.QuizId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Questions__QuizI__1DB06A4F");
        });

        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Quizzes__3214EC27CBC8B783");

            entity.HasIndex(e => e.Title, "UQ__Quizzes__2CB664DC60357144").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AuthorId).HasColumnName("AuthorID");
            entity.Property(e => e.Title).HasMaxLength(75);

            entity.HasOne(d => d.Author).WithMany(p => p.Quizzes)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Quizzes__AuthorI__19DFD96B");
        });

        modelBuilder.Entity<QuizHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__QuizHist__3214EC277EF7A2BB");

            entity.ToTable("QuizHistory");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.PlayedAt).HasColumnType("datetime");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.WinnerId).HasColumnName("WinnerID");
            entity.Property(e => e.WinnerName).HasMaxLength(75);

            entity.HasOne(d => d.Quiz).WithMany(p => p.QuizHistories)
                .HasForeignKey(d => d.QuizId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__QuizHisto__QuizI__2739D489");

            entity.HasOne(d => d.Winner).WithMany(p => p.QuizHistories)
                .HasForeignKey(d => d.WinnerId)
                .HasConstraintName("FK__QuizHisto__Winne__282DF8C2");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC27189EAD6B");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E435CF729F").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.JoinDate).HasColumnType("date");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.PasswordSalt).HasMaxLength(256);
            entity.Property(e => e.ProfilePictureId).HasColumnName("ProfilePictureID");
            entity.Property(e => e.Username).HasMaxLength(75);

            entity.HasOne(d => d.ProfilePicture).WithMany(p => p.Users)
                .HasForeignKey(d => d.ProfilePictureId)
                .HasConstraintName("FK__Users__ProfilePi__160F4887");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
