using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CrmDb.Models;

public partial class CrmDbContext : DbContext
{
    public CrmDbContext()
    {
    }

    public CrmDbContext(DbContextOptions<CrmDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<ContactPerson> ContactPersons { get; set; }

    public virtual DbSet<DataSource> DataSources { get; set; }

    public virtual DbSet<MatchLog> MatchLogs { get; set; }

    public virtual DbSet<ScoreHistory> ScoreHistories { get; set; }

    public virtual DbSet<ScoreRule> ScoreRules { get; set; }

    public virtual DbSet<SourceRecord> SourceRecords { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=CrmDb;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.ActivityId).HasName("PK__Activiti__45F4A7914EF7AADB");

            entity.Property(e => e.Tarih)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Tip)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.Company).WithMany(p => p.Activities)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Activities_Companies");

            entity.HasOne(d => d.User).WithMany(p => p.Activities)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Activities_Users");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__Companie__2D971CAC8C9DDDC3");

            entity.HasIndex(e => e.Sehir, "IX_Companies_Sehir");

            entity.HasIndex(e => e.Skor, "IX_Companies_Skor").IsDescending();

            entity.HasIndex(e => e.VergiNo, "IX_Companies_VergiNo");

            entity.Property(e => e.Adres).HasMaxLength(500);
            entity.Property(e => e.Durum)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Yeni");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Sehir).HasMaxLength(50);
            entity.Property(e => e.SektorKodu)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Skor).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.SonGuncelleme)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TahminiBuyukluk).HasMaxLength(100);
            entity.Property(e => e.UnvanResmi).HasMaxLength(250);
            entity.Property(e => e.VergiNo)
                .HasMaxLength(11)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ContactPerson>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PK__ContactP__5C66259BA35288E9");

            entity.Property(e => e.AdSoyad).HasMaxLength(100);
            entity.Property(e => e.Eposta)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Telefon)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Unvan).HasMaxLength(100);
            entity.Property(e => e.VeriKaynagi).HasMaxLength(100);

            entity.HasOne(d => d.Company).WithMany(p => p.ContactPeople)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_ContactPersons_Companies");
        });

        modelBuilder.Entity<DataSource>(entity =>
        {
            entity.HasKey(e => e.DataSourceId).HasName("PK__DataSour__28EECD6C82500AD3");

            entity.Property(e => e.Ad).HasMaxLength(100);
            entity.Property(e => e.HukukiDurum).HasMaxLength(100);
            entity.Property(e => e.SonIceAktarim).HasColumnType("datetime");
            entity.Property(e => e.Tip)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MatchLog>(entity =>
        {
            entity.HasKey(e => e.MatchLogId).HasName("PK__MatchLog__0497CA6A643EA968");

            entity.HasIndex(e => e.Decision, "IX_MatchLogs_Decision");

            entity.HasIndex(e => e.MatchedCompanyId, "IX_MatchLogs_MatchedCompanyId");

            entity.HasIndex(e => e.SourceRecordId, "IX_MatchLogs_SourceRecordId");

            entity.Property(e => e.AppliedThreshold).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Decision).HasMaxLength(50);
            entity.Property(e => e.MatchType).HasMaxLength(50);
            entity.Property(e => e.ReviewedByUserId).HasMaxLength(450);
            entity.Property(e => e.SimilarityScore).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.MatchedCompany).WithMany(p => p.MatchLogs)
                .HasForeignKey(d => d.MatchedCompanyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_MatchLogs_Companies");

            entity.HasOne(d => d.SourceRecord).WithMany(p => p.MatchLogs)
                .HasForeignKey(d => d.SourceRecordId)
                .HasConstraintName("FK_MatchLogs_SourceRecords");
        });

        modelBuilder.Entity<ScoreHistory>(entity =>
        {
            entity.HasKey(e => e.ScoreHistoryId).HasName("PK__ScoreHis__5E27B4E4F1395407");

            entity.Property(e => e.HesaplamaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.KriterVersiyonu)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SkorDegeri).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Company).WithMany(p => p.ScoreHistories)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_ScoreHistories_Companies");
        });

        modelBuilder.Entity<ScoreRule>(entity =>
        {
            entity.HasKey(e => e.RuleId).HasName("PK__ScoreRul__110458E2115FC415");

            entity.Property(e => e.Aciklama).HasMaxLength(250);
            entity.Property(e => e.AgirlikPuani).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.AktifMi).HasDefaultValue(true);
            entity.Property(e => e.GuncellemeTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.KriterAdi).HasMaxLength(100);
        });

        modelBuilder.Entity<SourceRecord>(entity =>
        {
            entity.HasKey(e => e.SourceRecordId).HasName("PK__SourceRe__E205D4DBCC076501");

            entity.Property(e => e.EslestirmeDurumu)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Yeni Kayıt");
            entity.Property(e => e.IceAktarmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Company).WithMany(p => p.SourceRecords)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_SourceRecords_Companies");

            entity.HasOne(d => d.DataSource).WithMany(p => p.SourceRecords)
                .HasForeignKey(d => d.DataSourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SourceRecords_DataSources");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CEF48C6C0");

            entity.HasIndex(e => e.Eposta, "UQ__Users__03ABA391A2B5687E").IsUnique();

            entity.Property(e => e.AdSoyad).HasMaxLength(100);
            entity.Property(e => e.AktifMi).HasDefaultValue(true);
            entity.Property(e => e.Eposta)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Rol)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Satis");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
