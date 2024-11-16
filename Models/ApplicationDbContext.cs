using Microsoft.EntityFrameworkCore;
using _123.Dtos;

namespace _123.Models
{
        public class ApplicationDbContext : DbContext
        {
                public DbSet<Usuario> Usuarios { get; set; }
                public DbSet<Producto> Productos { get; set; }

                public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                    : base(options)
                {
                }

                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                        base.OnModelCreating(modelBuilder);

                        modelBuilder.Entity<Usuario>(entity =>
                        {
                                entity.HasKey(e => e.IdUsuario);
                                entity.Property(e => e.Nombre)
                  .IsRequired()
                  .HasMaxLength(100);
                                entity.Property(e => e.Correo)
                  .IsRequired()
                  .HasMaxLength(256);
                                entity.HasIndex(e => e.Correo)
                  .IsUnique();
                                entity.Property(e => e.Clave)
                  .IsRequired()
                  .HasMaxLength(256);
                                entity.Property(e => e.Confirmado)
                  .IsRequired()
                  .HasDefaultValue(false);
                                entity.Property(e => e.Token)
                  .HasMaxLength(256)
                  .IsRequired(false);
                                entity.Property(e => e.RestablecerTokenExpira)
                  .IsRequired(false);

                                entity.Property(e => e.FechaCreacion)
                  .IsRequired()
                  .HasDefaultValueSql("GETDATE()");
                                entity.Property(e => e.FechaActualizacion)
                  .IsRequired(false);
                        });

                        modelBuilder.Entity<Producto>(entity =>
      {
              entity.HasKey(e => e.Id);
              entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);
              entity.Property(e => e.Precio)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
              entity.Property(e => e.Descripcion)
                    .HasMaxLength(1000) 
                    .IsRequired(false);
              entity.Property(e => e.Archivo)
                              .HasMaxLength(1000)
                              .IsRequired(false);
      });
                }

        }
}