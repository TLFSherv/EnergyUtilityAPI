using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EnergyUtilityApi;

public partial class EnergyUtilityDbContext : DbContext
{
    public EnergyUtilityDbContext()
    {
    }

    public EnergyUtilityDbContext(DbContextOptions<EnergyUtilityDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AllPostcodeDno> AllPostcodeDnos { get; set; }

    public virtual DbSet<Dno> Dnos { get; set; }

    public virtual DbSet<DnoNeedRegion> DnoNeedRegions { get; set; }

    public virtual DbSet<DnoPriceCapRate> DnoPriceCapRates { get; set; }

    public virtual DbSet<ElecConsPostcode> ElecConsPostcodes { get; set; }

    public virtual DbSet<RegionalWeight> RegionalWeights { get; set; }

    public virtual DbSet<WeightCategory> WeightCategories { get; set; }

    //     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //         => optionsBuilder.UseNpgsql("Host=localhost;Database=energy_utility_db;Username=postgres;Password=password");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AllPostcodeDno>(entity =>
        {
            entity.HasKey(e => e.Postcode).HasName("postcodes_with_dno_pkey");

            entity.ToTable("all_postcode_dnos");

            entity.Property(e => e.Postcode)
                .HasMaxLength(8)
                .IsFixedLength()
                .HasColumnName("postcode");
            entity.Property(e => e.DnoId).HasColumnName("dno_id");
        });

        modelBuilder.Entity<Dno>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("dno_pkey");

            entity.ToTable("dno");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Operator)
                .HasMaxLength(40)
                .HasColumnName("operator");
            entity.Property(e => e.Region)
                .HasMaxLength(40)
                .HasColumnName("region");
            entity.Property(e => e.RegionCode)
                .HasMaxLength(1)
                .HasColumnName("region_code");
        });

        modelBuilder.Entity<DnoNeedRegion>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("dno_need_regions");

            entity.Property(e => e.DnoId).HasColumnName("dno_id");
            entity.Property(e => e.NeedRegionSourceId).HasColumnName("need_region_source_id");

            entity.HasOne(d => d.Dno).WithMany()
                .HasForeignKey(d => d.DnoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dno_fk");
        });

        modelBuilder.Entity<DnoPriceCapRate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("regional_price_cap_rates_pkey");

            entity.ToTable("dno_price_cap_rates");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnnualStandingCharge)
                .HasPrecision(5, 4)
                .HasColumnName("annual_standing_charge");
            entity.Property(e => e.DnoId).HasColumnName("dno_id");
            entity.Property(e => e.MeterTypeId).HasColumnName("meter_type_id");
            entity.Property(e => e.MeterType)
                .HasMaxLength(20)
                .HasColumnName("meter_type");
            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(20)
                .HasColumnName("payment_method");
            entity.Property(e => e.UnitRatePence)
                .HasPrecision(5, 4)
                .HasColumnName("unit_rate_pence");
        });

        modelBuilder.Entity<ElecConsPostcode>(entity =>
        {
            entity.HasKey(e => e.Postcode).HasName("postcode_cons_pkey");

            entity.ToTable("elec_cons_postcodes");

            entity.HasIndex(e => e.Postcode, "idx_postcode_lookup");

            entity.Property(e => e.Postcode)
                .HasMaxLength(20)
                .HasColumnName("postcode");
            entity.Property(e => e.MeanCons)
                .HasPrecision(8, 3)
                .HasColumnName("mean_cons");
            entity.Property(e => e.MedianCons)
                .HasPrecision(8, 3)
                .HasColumnName("median_cons");
            entity.Property(e => e.NumMeters).HasColumnName("num_meters");
        });

        modelBuilder.Entity<RegionalWeight>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("regional_weights_pkey");

            entity.ToTable("regional_weights");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Multiplier)
                .HasPrecision(4, 3)
                .HasColumnName("multiplier");
            entity.Property(e => e.Region)
                .HasMaxLength(40)
                .HasColumnName("region");
            entity.Property(e => e.RegionId).HasColumnName("region_id");
            entity.Property(e => e.Value)
                .HasMaxLength(40)
                .HasColumnName("value");
            entity.Property(e => e.ValueId).HasColumnName("value_id");

            entity.HasOne(d => d.Category).WithMany(p => p.RegionalWeights)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("category_fk");
        });

        modelBuilder.Entity<WeightCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("weight_categories_pkey");

            entity.ToTable("weight_categories");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(30)
                .HasColumnName("category_name");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
