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

    readonly string connectionString = Preferences.Get("database_connection_string", "Server=localhost;Database=AppPallet;Trusted_Connection=true;");
    public virtual DbSet<ActivoPasivo> ActivoPasivo { get; set; }

    public virtual DbSet<Area> Area { get; set; }

    public virtual DbSet<Cheque> Cheque { get; set; }

    public virtual DbSet<ContactosEmpresa> ContactosEmpresa { get; set; }

    public virtual DbSet<CostoPorCamion> CostoPorCamion { get; set; }

    public virtual DbSet<CostoPorPallet> CostoPorPallet { get; set; }

    public virtual DbSet<Egreso> Egreso { get; set; }

    public virtual DbSet<Empresa> Empresa { get; set; }

    public virtual DbSet<GastosFijos> GastosFijos { get; set; }

    public virtual DbSet<HistorialHumedad> HistorialHumedad { get; set; }

    public virtual DbSet<Ingreso> Ingreso { get; set; }

    public virtual DbSet<Lote> Lote { get; set; }

    public virtual DbSet<Pallet> Pallet { get; set; }

    public virtual DbSet<PalletComponentes> PalletComponentes { get; set; }

    public virtual DbSet<PalletDimensiones> PalletDimensiones { get; set; }

    public virtual DbSet<PalletEspecificaciones> PalletEspecificaciones { get; set; }

    public virtual DbSet<PalletHumedad> PalletHumedad { get; set; }

    public virtual DbSet<Pedido> Pedido { get; set; }

    public virtual DbSet<Venta> Venta { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer(connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivoPasivo>(entity =>
        {
            entity.Property(e => e.ActivoPasivoId).HasColumnName("ActivoPasivoID");
            entity.Property(e => e.Categoria)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.Mes).HasColumnType("datetime");
            entity.Property(e => e.Monto).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.Property(e => e.AreaId).HasColumnName("AreaID");
            entity.Property(e => e.NomArea)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Cheque>(entity =>
        {
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

            entity.HasOne(d => d.Area).WithMany(p => p.ContactosEmpresa)
                .HasForeignKey(d => d.AreaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContactosEmpresa_Area");

            entity.HasOne(d => d.Empresa).WithMany(p => p.ContactosEmpresa)
                .HasForeignKey(d => d.EmpresaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContactosEmpresa_Empresa");
        });

        modelBuilder.Entity<CostoPorCamion>(entity =>
        {
            entity.Property(e => e.CostoPorCamionId).HasColumnName("CostoPorCamionID");
            entity.Property(e => e.CostoPorPalletId).HasColumnName("CostoPorPalletID");
            entity.Property(e => e.Monto).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.NombreCosto)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.CostoPorPallet).WithMany(p => p.CostoPorCamion)
                .HasForeignKey(d => d.CostoPorPalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CostoPorCamion_CostoPorPallet");
        });

        modelBuilder.Entity<CostoPorPallet>(entity =>
        {
            entity.Property(e => e.CostoPorPalletId).HasColumnName("CostoPorPalletID");
            entity.Property(e => e.EmpresaId).HasColumnName("EmpresaID");
            entity.Property(e => e.FechaDelete).HasColumnType("datetime");
            entity.Property(e => e.GananciaPorCantPallet).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mes).HasColumnType("datetime");
            entity.Property(e => e.NombrePalletCliente)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PalletId).HasColumnName("PalletID");
            entity.Property(e => e.PrecioPallet).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Empresa).WithMany(p => p.CostoPorPallet)
                .HasForeignKey(d => d.EmpresaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CostoPorPallet_Empresa1");

            entity.HasOne(d => d.Pallet).WithMany(p => p.CostoPorPallet)
                .HasForeignKey(d => d.PalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CostoPorPallet_Pallet");
        });

        modelBuilder.Entity<Egreso>(entity =>
        {
            entity.Property(e => e.EgresoId).HasColumnName("EgresoID");
            entity.Property(e => e.Comentario)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.DescripEgreso)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Factura)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.Mes).HasColumnType("datetime");
            entity.Property(e => e.Monto).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SumaIva)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("SumaIVA");
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.EmpresaId).HasName("PK_ClienteProveedor");

            entity.Property(e => e.EmpresaId).HasColumnName("EmpresaID");
            entity.Property(e => e.Cuit)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUIT");
            entity.Property(e => e.Domicilio)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaDelete).HasColumnType("datetime");
            entity.Property(e => e.NomEmpresa)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Cliente");
        });

        modelBuilder.Entity<GastosFijos>(entity =>
        {
            entity.Property(e => e.GastosFijosId).HasColumnName("GastosFijosID");
            entity.Property(e => e.Mes).HasColumnType("datetime");
            entity.Property(e => e.Monto).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.NombreGastoFijo)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HistorialHumedad>(entity =>
        {
            entity.Property(e => e.HistorialHumedadId).HasColumnName("HistorialHumedadID");
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.PedidoId).HasColumnName("PedidoID");
            entity.Property(e => e.PesoAprox)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Promedio).HasColumnType("decimal(18, 1)");

            entity.HasOne(d => d.Pedido).WithMany(p => p.HistorialHumedad)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistorialHumedad_Pedido");
        });

        modelBuilder.Entity<Ingreso>(entity =>
        {
            entity.Property(e => e.IngresoId).HasColumnName("IngresoID");
            entity.Property(e => e.Comentario)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DescripIngreso)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Factura)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.Monto).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Op)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OP");
            entity.Property(e => e.Remito)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Lote>(entity =>
        {
            entity.Property(e => e.LoteId).HasColumnName("LoteID");
            entity.Property(e => e.EmpresaId).HasColumnName("EmpresaID");
            entity.Property(e => e.FechaEntrega).HasColumnType("datetime");
            entity.Property(e => e.FechaSolicitada).HasColumnType("datetime");
            entity.Property(e => e.NomCamionero)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NumFacturaProveedor)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PrecioCamionero).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PrecioProveedor).HasColumnType("decimal(18, 4)");

            entity.HasOne(d => d.Empresa).WithMany(p => p.Lote)
                .HasForeignKey(d => d.EmpresaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lote_Empresa");
        });

        modelBuilder.Entity<Pallet>(entity =>
        {
            entity.HasKey(e => e.PalletId).HasName("PK__Pallet__C049FE5C4C49F8BC");

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
            entity.Property(e => e.FechaEliminacion).HasColumnType("datetime");
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

        modelBuilder.Entity<PalletComponentes>(entity =>
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

        modelBuilder.Entity<PalletDimensiones>(entity =>
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

        modelBuilder.Entity<PalletEspecificaciones>(entity =>
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

            entity.Property(e => e.HumedadId).HasColumnName("HumedadID");
            entity.Property(e => e.HumedadMaxima).HasColumnType("decimal(4, 1)");
            entity.Property(e => e.PalletId).HasColumnName("PalletID");
            entity.Property(e => e.TipoComponente)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Tolerancia)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Pallet).WithMany(p => p.PalletHumedad)
                .HasForeignKey(d => d.PalletId)
                .HasConstraintName("FK__PalletHum__Palle__5165187F");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.Property(e => e.PedidoId).HasColumnName("PedidoID");
            entity.Property(e => e.FechaEFinal)
                .HasColumnType("datetime")
                .HasColumnName("FechaE_Final");
            entity.Property(e => e.FechaEInicio)
                .HasColumnType("datetime")
                .HasColumnName("FechaE_Inicio");
            entity.Property(e => e.LoteId).HasColumnName("LoteID");
            entity.Property(e => e.PalletId).HasColumnName("PalletID");

            entity.HasOne(d => d.Lote).WithMany(p => p.Pedido)
                .HasForeignKey(d => d.LoteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pedido_Lote");

            entity.HasOne(d => d.Pallet).WithMany(p => p.Pedido)
                .HasForeignKey(d => d.PalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pedido_Pallet");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.Property(e => e.VentaId).HasColumnName("VentaID");
            entity.Property(e => e.Comentario)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.CostoPorPalletId).HasColumnName("CostoPorPalletID");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaCobroEstimada).HasColumnType("datetime");
            entity.Property(e => e.FechaEntrega).HasColumnType("datetime");
            entity.Property(e => e.FechaEntregaEstimada).HasColumnType("datetime");
            entity.Property(e => e.FechaVenta).HasColumnType("datetime");

            entity.HasOne(d => d.CostoPorPallet).WithMany(p => p.Venta)
                .HasForeignKey(d => d.CostoPorPalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Venta_CostoPorPallet");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
