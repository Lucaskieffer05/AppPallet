using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AppPallet.Models;

public partial class PalletContext : DbContext
{
    public PalletContext()
    {
    }

    public PalletContext(DbContextOptions<PalletContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<Cheque> Cheques { get; set; }

    public virtual DbSet<ContactosEmpresa> ContactosEmpresas { get; set; }

    public virtual DbSet<CostoPorCamion> CostoPorCamions { get; set; }

    public virtual DbSet<CostoPorPallet> CostoPorPallets { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<FichaTecnica> FichaTecnicas { get; set; }

    public virtual DbSet<GastosFijo> GastosFijos { get; set; }

    public virtual DbSet<Lote> Lotes { get; set; }

    public virtual DbSet<Mes> Mes { get; set; }

    public virtual DbSet<Pallet> Pallets { get; set; }

    public virtual DbSet<PalletComponente> PalletComponentes { get; set; }

    public virtual DbSet<PalletDimensione> PalletDimensiones { get; set; }

    public virtual DbSet<PalletEspecificacione> PalletEspecificaciones { get; set; }

    public virtual DbSet<PalletHumedad> PalletHumedads { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LUCAS\\SQLEXPRESS;Initial Catalog=Pallet;Persist Security Info=True;User ID=sa;Password=42559251;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Area>(entity =>
        {
            entity.ToTable("Area");

            entity.Property(e => e.AreaId).HasColumnName("AreaID");
            entity.Property(e => e.NomArea)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Cheque>(entity =>
        {
            entity.ToTable("Cheque");

            entity.Property(e => e.ChequeId).HasColumnName("ChequeID");
            entity.Property(e => e.FechaEmision).HasColumnType("datetime");
            entity.Property(e => e.FechaPago).HasColumnType("datetime");
            entity.Property(e => e.Monto).HasColumnType("money");
            entity.Property(e => e.NumCheque)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Proveedor)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ContactosEmpresa>(entity =>
        {
            entity.ToTable("ContactosEmpresa");

            entity.Property(e => e.ContactosEmpresaId).HasColumnName("ContactosEmpresaID");
            entity.Property(e => e.AreaId).HasColumnName("AreaID");
            entity.Property(e => e.Comentario)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Contacto)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EmpresaId).HasColumnName("EmpresaID");
            entity.Property(e => e.Mail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Pallet)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Sello)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Area).WithMany(p => p.ContactosEmpresas)
                .HasForeignKey(d => d.AreaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContactosEmpresa_Area");

            entity.HasOne(d => d.Empresa).WithMany(p => p.ContactosEmpresas)
                .HasForeignKey(d => d.EmpresaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContactosEmpresa_Empresa");
        });

        modelBuilder.Entity<CostoPorCamion>(entity =>
        {
            entity.ToTable("CostoPorCamion");

            entity.Property(e => e.CostoPorCamionId).HasColumnName("CostoPorCamionID");
            entity.Property(e => e.CostoPorPalletId).HasColumnName("CostoPorPalletID");
            entity.Property(e => e.Monto).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.NombreCosto)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.CostoPorPallet).WithMany(p => p.CostoPorCamions)
                .HasForeignKey(d => d.CostoPorPalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CostoPorCamion_CostoPorPallet");
        });

        modelBuilder.Entity<CostoPorPallet>(entity =>
        {
            entity.ToTable("CostoPorPallet");

            entity.Property(e => e.CostoPorPalletId).HasColumnName("CostoPorPalletID");
            entity.Property(e => e.EmpresaId).HasColumnName("EmpresaID");
            entity.Property(e => e.MesId).HasColumnName("MesID");
            entity.Property(e => e.NombrePalletCliente)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PalletId).HasColumnName("PalletID");
            entity.Property(e => e.PrecioPallet).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Empresa).WithMany(p => p.CostoPorPallets)
                .HasForeignKey(d => d.EmpresaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CostoPorPallet_Empresa1");

            entity.HasOne(d => d.Mes).WithMany(p => p.CostoPorPallets)
                .HasForeignKey(d => d.MesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CostoPorPallet_Mes");

            entity.HasOne(d => d.Pallet).WithMany(p => p.CostoPorPallets)
                .HasForeignKey(d => d.PalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CostoPorPallet_Pallet");
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.EmpresaId).HasName("PK_ClienteProveedor");

            entity.ToTable("Empresa");

            entity.Property(e => e.EmpresaId).HasColumnName("EmpresaID");
            entity.Property(e => e.Cuit)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUIT");
            entity.Property(e => e.Domicilio)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NomEmpresa)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FichaTecnica>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("FichaTecnica");

            entity.Property(e => e.Asdasd)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("asdasd");
        });

        modelBuilder.Entity<GastosFijo>(entity =>
        {
            entity.HasKey(e => e.GastosFijosId);

            entity.Property(e => e.GastosFijosId).HasColumnName("GastosFijosID");
            entity.Property(e => e.MesId).HasColumnName("MesID");
            entity.Property(e => e.Monto).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.NombreGastoFijo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Mes).WithMany(p => p.GastosFijos)
                .HasForeignKey(d => d.MesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GastosFijos_Mes");
        });

        modelBuilder.Entity<Lote>(entity =>
        {
            entity.ToTable("Lote");

            entity.Property(e => e.LoteId).HasColumnName("LoteID");
            entity.Property(e => e.NomCamionero)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NomProveedor)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NumFacturaProveedor)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PedidoId).HasColumnName("PedidoID");
            entity.Property(e => e.PrecioCamionero).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.PrecioProveedor).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Pedido).WithMany(p => p.Lotes)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lote_Pedido");
        });

        modelBuilder.Entity<Mes>(entity =>
        {
            entity.HasKey(e => e.MesId);

            entity.Property(e => e.MesId).HasColumnName("MesID");
        });

        modelBuilder.Entity<Pallet>(entity =>
        {
            entity.HasKey(e => e.PalletId).HasName("PK__Pallet__C049FE5C4C49F8BC");

            entity.ToTable("Pallet");

            entity.Property(e => e.PalletId).HasColumnName("PalletID");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Estructura)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Peso).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.Sello)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ToleranciaPeso).HasColumnType("decimal(3, 1)");
            entity.Property(e => e.Tratamiento)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PalletComponente>(entity =>
        {
            entity.HasKey(e => e.ComponenteId).HasName("PK__PalletDe__CDF27190DC82A8F5");

            entity.Property(e => e.ComponenteId).HasColumnName("ComponenteID");
            entity.Property(e => e.Ancho).HasColumnType("decimal(4, 1)");
            entity.Property(e => e.Espesor).HasColumnType("decimal(4, 1)");
            entity.Property(e => e.Largo).HasColumnType("decimal(4, 1)");
            entity.Property(e => e.PalletId).HasColumnName("PalletID");
            entity.Property(e => e.TipoComponente)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UnidadMedida)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("CM");

            entity.HasOne(d => d.Pallet).WithMany(p => p.PalletComponentes)
                .HasForeignKey(d => d.PalletId)
                .HasConstraintName("FK__PalletDet__Palle__4D94879B");
        });

        modelBuilder.Entity<PalletDimensione>(entity =>
        {
            entity.HasKey(e => e.DimensionId).HasName("PK__PalletDi__1F7D4F3135770694");

            entity.Property(e => e.DimensionId).HasColumnName("DimensionID");
            entity.Property(e => e.PalletId).HasColumnName("PalletID");
            entity.Property(e => e.TipoDimension)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Tolerancia)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UnidadMedida)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("CM");
            entity.Property(e => e.Valor).HasColumnType("decimal(6, 1)");

            entity.HasOne(d => d.Pallet).WithMany(p => p.PalletDimensiones)
                .HasForeignKey(d => d.PalletId)
                .HasConstraintName("FK__PalletDim__Palle__48CFD27E");
        });

        modelBuilder.Entity<PalletEspecificacione>(entity =>
        {
            entity.HasKey(e => e.EspecificacionId).HasName("PK__PalletEs__0F3870DFF1A4C923");

            entity.Property(e => e.EspecificacionId).HasColumnName("EspecificacionID");
            entity.Property(e => e.OtrasEspecificaciones)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PalletId).HasColumnName("PalletID");
            entity.Property(e => e.TipoClavo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Pallet).WithMany(p => p.PalletEspecificaciones)
                .HasForeignKey(d => d.PalletId)
                .HasConstraintName("FK__PalletEsp__Palle__5441852A");
        });

        modelBuilder.Entity<PalletHumedad>(entity =>
        {
            entity.HasKey(e => e.HumedadId).HasName("PK__PalletHu__68F9900577A9C033");

            entity.ToTable("PalletHumedad");

            entity.Property(e => e.HumedadId).HasColumnName("HumedadID");
            entity.Property(e => e.HumedadMaxima).HasColumnType("decimal(4, 1)");
            entity.Property(e => e.PalletId).HasColumnName("PalletID");
            entity.Property(e => e.TipoComponente)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Tolerancia)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Pallet).WithMany(p => p.PalletHumedads)
                .HasForeignKey(d => d.PalletId)
                .HasConstraintName("FK__PalletHum__Palle__5165187F");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.ToTable("Pedido");

            entity.Property(e => e.PedidoId).HasColumnName("PedidoID");
            entity.Property(e => e.EmpresaId).HasColumnName("EmpresaID");
            entity.Property(e => e.PalletId).HasColumnName("PalletID");

            entity.HasOne(d => d.Empresa).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.EmpresaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pedido_Empresa");

            entity.HasOne(d => d.Pallet).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.PalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pedido_Pallet");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
