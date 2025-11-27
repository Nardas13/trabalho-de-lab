using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AutoHubProjeto.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Administrador> Administradors { get; set; }

    public virtual DbSet<Anuncio> Anuncios { get; set; }

    public virtual DbSet<AnuncioImagem> AnuncioImagems { get; set; }

    public virtual DbSet<Compra> Compras { get; set; }

    public virtual DbSet<Comprador> Compradors { get; set; }

    public virtual DbSet<LogAdministrativo> LogAdministrativos { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Utilizador> Utilizadors { get; set; }

    public virtual DbSet<Veiculo> Veiculos { get; set; }

    public virtual DbSet<Vendedor> Vendedors { get; set; }

    public virtual DbSet<Visitum> Visita { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=S\\SQLEXPRESS;Database=Marketplace;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>(entity =>
        {
            entity.HasKey(e => e.IdAdmin).HasName("PK__Administ__4C3F97F471DF33AB");

            entity.ToTable("Administrador");

            entity.Property(e => e.IdAdmin).ValueGeneratedNever();

            entity.HasOne(d => d.IdAdminNavigation).WithOne(p => p.Administrador)
                .HasForeignKey<Administrador>(d => d.IdAdmin)
                .HasConstraintName("FK__Administr__IdAdm__3E52440B");
        });

        modelBuilder.Entity<Anuncio>(entity =>
        {
            entity.HasKey(e => e.IdAnuncio).HasName("PK__Anuncio__BD6A7622F40512F1");

            entity.ToTable("Anuncio");

            entity.HasIndex(e => e.IdVeiculo, "UQ__Anuncio__CAC4F347BC1C1231").IsUnique();

            entity.Property(e => e.DataAtualizacao).HasPrecision(0);
            entity.Property(e => e.DataPublicacao)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Estado)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("ativo");
            entity.Property(e => e.Preco).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Titulo).HasMaxLength(200);

            entity.HasOne(d => d.IdVeiculoNavigation).WithOne(p => p.Anuncio)
                .HasForeignKey<Anuncio>(d => d.IdVeiculo)
                .HasConstraintName("FK__Anuncio__IdVeicu__5629CD9C");

            entity.HasOne(d => d.IdVendedorNavigation).WithMany(p => p.Anuncios)
                .HasForeignKey(d => d.IdVendedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Anuncio__IdVende__5535A963");
        });

        modelBuilder.Entity<AnuncioImagem>(entity =>
        {
            entity.HasKey(e => e.IdImagem).HasName("PK__AnuncioI__B42D8F15C1B3A80F");

            entity.ToTable("AnuncioImagem");

            entity.Property(e => e.Url).HasMaxLength(400);

            entity.HasOne(d => d.IdAnuncioNavigation).WithMany(p => p.AnuncioImagems)
                .HasForeignKey(d => d.IdAnuncio)
                .HasConstraintName("FK__AnuncioIm__IdAnu__5CD6CB2B");
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.IdCompra).HasName("PK__Compra__0A5CDB5CC3F0906D");

            entity.ToTable("Compra");

            entity.Property(e => e.DataCompra)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.EstadoPagamento)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("pendente");
            entity.Property(e => e.ReferenciaPagamento).HasMaxLength(80);
            entity.Property(e => e.Valor).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.IdAnuncioNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.IdAnuncio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Compra__IdAnunci__60A75C0F");

            entity.HasOne(d => d.IdCompradorNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.IdComprador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Compra__IdCompra__5FB337D6");
        });

        modelBuilder.Entity<Comprador>(entity =>
        {
            entity.HasKey(e => e.IdComprador).HasName("PK__Comprado__324FC82048A45058");

            entity.ToTable("Comprador");

            entity.Property(e => e.IdComprador).ValueGeneratedNever();
            entity.Property(e => e.FiltroFavorito).HasMaxLength(400);
            entity.Property(e => e.MarcaFavorita).HasMaxLength(100);
            entity.Property(e => e.NotificacoesAtivas).HasDefaultValue(true);

            entity.HasOne(d => d.IdCompradorNavigation).WithOne(p => p.Comprador)
                .HasForeignKey<Comprador>(d => d.IdComprador)
                .HasConstraintName("FK__Comprador__IdCom__47DBAE45");
        });

        modelBuilder.Entity<LogAdministrativo>(entity =>
        {
            entity.HasKey(e => e.IdLog).HasName("PK__LogAdmin__0C54DBC6A81E41B3");

            entity.ToTable("LogAdministrativo");

            entity.Property(e => e.Acao).HasMaxLength(300);
            entity.Property(e => e.DataHora)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Detalhes).HasMaxLength(1000);
            entity.Property(e => e.Entidade).HasMaxLength(100);
            entity.Property(e => e.IdEntidade).HasMaxLength(64);

            entity.HasOne(d => d.IdAdminNavigation).WithMany(p => p.LogAdministrativos)
                .HasForeignKey(d => d.IdAdmin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LogAdmini__IdAdm__4BAC3F29");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.IdReserva).HasName("PK__Reserva__0E49C69D8A6D5410");

            entity.ToTable("Reserva");

            entity.Property(e => e.DataReserva)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Estado)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("ativada");
            entity.Property(e => e.ExpiraEm).HasPrecision(0);

            entity.HasOne(d => d.IdAnuncioNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdAnuncio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reserva__IdAnunc__68487DD7");

            entity.HasOne(d => d.IdCompradorNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdComprador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reserva__IdCompr__6754599E");
        });

        modelBuilder.Entity<Utilizador>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Utilizad__3214EC07E93A86E5");

            entity.ToTable("Utilizador");

            entity.HasIndex(e => e.Email, "UQ_Utilizador_Email").IsUnique();

            entity.HasIndex(e => e.Username, "UQ_Utilizador_Username").IsUnique();

            entity.Property(e => e.CodigoConfirmacao).HasMaxLength(50);
            entity.Property(e => e.DataCriacao)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.EstadoConta)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("ativo");
            entity.Property(e => e.Morada).HasMaxLength(250);
            entity.Property(e => e.Nome).HasMaxLength(150);
            entity.Property(e => e.PasswordHash).HasMaxLength(128);
            entity.Property(e => e.ResetPasswordCode).HasMaxLength(50);
            entity.Property(e => e.Telefone).HasMaxLength(30);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<Veiculo>(entity =>
        {
            entity.HasKey(e => e.IdVeiculo).HasName("PK__Veiculo__CAC4F346239F6C57");

            entity.ToTable("Veiculo");

            entity.Property(e => e.Caixa).HasMaxLength(50);
            entity.Property(e => e.Categoria).HasMaxLength(80);
            entity.Property(e => e.Combustivel).HasMaxLength(50);
            entity.Property(e => e.Descricao).HasMaxLength(1000);
            entity.Property(e => e.Localizacao).HasMaxLength(150);
            entity.Property(e => e.Marca).HasMaxLength(100);
            entity.Property(e => e.Modelo).HasMaxLength(100);
            entity.Property(e => e.PrecoSugerido).HasColumnType("decimal(12, 2)");
        });

        modelBuilder.Entity<Vendedor>(entity =>
        {
            entity.HasKey(e => e.IdVendedor).HasName("PK__Vendedor__16D6C7CA033831D8");

            entity.ToTable("Vendedor");

            entity.HasIndex(e => e.Nif, "UQ_Vendedor_NIF").IsUnique();

            entity.Property(e => e.IdVendedor).ValueGeneratedNever();
            entity.Property(e => e.DadosFaturacao).HasMaxLength(300);
            entity.Property(e => e.DataAprovacao).HasPrecision(0);
            entity.Property(e => e.Nif)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("NIF");
            entity.Property(e => e.Tipo)
                .HasMaxLength(12)
                .IsUnicode(false);

            entity.HasOne(d => d.IdAdminAprovadorNavigation).WithMany(p => p.Vendedors)
                .HasForeignKey(d => d.IdAdminAprovador)
                .HasConstraintName("FK__Vendedor__IdAdmi__440B1D61");

            entity.HasOne(d => d.IdVendedorNavigation).WithOne(p => p.Vendedor)
                .HasForeignKey<Vendedor>(d => d.IdVendedor)
                .HasConstraintName("FK__Vendedor__IdVend__4222D4EF");
        });

        modelBuilder.Entity<Visitum>(entity =>
        {
            entity.HasKey(e => e.IdVisita).HasName("PK__Visita__020AC8276F4BA839");

            entity.Property(e => e.DataHora).HasPrecision(0);
            entity.Property(e => e.Estado)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("marcada");

            entity.HasOne(d => d.IdAnuncioNavigation).WithMany(p => p.Visita)
                .HasForeignKey(d => d.IdAnuncio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Visita__IdAnunci__6EF57B66");

            entity.HasOne(d => d.IdCompradorNavigation).WithMany(p => p.Visita)
                .HasForeignKey(d => d.IdComprador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Visita__IdCompra__6E01572D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
