using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataAccess
{
    /// <summary>
    /// DB context for database.
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
    public partial class TheaterDb : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TheaterDb"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public TheaterDb(DbContextOptions<TheaterDb> options)
            : base(options)
        {
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shows.Show>(entity =>
            {
                entity.ToTable("Shows", "shows");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.Subtitle).HasMaxLength(500);

                entity.Property(e => e.CreatedUtc).IsRequired();
                entity.Property(e => e.CreatedBy).HasMaxLength(50).IsRequired();
                entity.Property(e => e.ModifiedUtc).IsRequired();
                entity.Property(e => e.ModifiedBy).HasMaxLength(50).IsRequired();
                entity.Property(e => e.IsDeleted).IsRequired();
            });

            modelBuilder.Entity<Shows.ShowProperty>(entity =>
            {
                entity.ToTable("ShowProperties", "shows");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Value)
                    .HasMaxLength(500);

                entity.Property(e => e.IsDeleted).IsRequired();

                entity.HasOne(d => d.Show)
                    .WithMany(p => p.ShowProperties)
                    .HasForeignKey(d => d.ShowId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ShowProperties_Shows");
            });
        }

        /// <summary>
        /// Gets or sets the shows.
        /// </summary>
        public virtual DbSet<Shows.Show> Shows { get; set; }

        /// <summary>
        /// Gets or sets the show properties.
        /// </summary>
        public virtual DbSet<Shows.ShowProperty> ShowProperties { get; set; }
    }
}