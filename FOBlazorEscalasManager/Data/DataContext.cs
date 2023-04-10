using Microsoft.EntityFrameworkCore;
using FOBlazorEscalasManager.Models;

namespace FOBlazorEscalasManager.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        
        public DbSet<Capitan> Capitanes { get; set; }
        public DbSet<CalendarioCapitan> CalendarioCapitanes { get; set; }
        public DbSet<Barco> Barcos { get; set; }
        public DbSet<Escala> Escalas { get; set; }
        public DbSet<EstadoEscala> EstadoEscalas { get; set; }
        public DbSet<Puerto> Puertos { get; set; }
        public DbSet<Muelle> Muelles { get; set; }
        public DbSet<Atraque> Atraques { get; set; }
        public DbSet<TipoAtraque> TiposAtraque { get; set; }
        public DbSet<TipoOperacionPortuaria> TiposOperaciones { get; set; }
        public DbSet<Pais> Paises { get; set; }
        public DbSet<Manifiesto> Manifiestos { get; set; }
        public DbSet<TipoPartida> TiposPartida { get; set; }
        public DbSet<Error> Errores { get; set; }
        public DbSet<TipoVehiculos> TiposVehiculo { get; set; }
        public DbSet<UserProfile> UsersProfile { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {


            builder.Entity<Capitan>()
                .ToTable("Capitanes")
                .Property(p => p.Id_Capitan)
                .HasColumnType("decimal(18,0)")
                ;
           
            builder.Entity<Capitan>(e =>
            {
                e.HasOne<Pais>(x => x.Pais)
                    .WithMany(x => x.Capitan)
                    .HasForeignKey(x => x.Nacionalidad);

            });

            builder.Entity<CalendarioCapitan>()

                .ToTable("CalendarioCapitanes")
                .Property(p => p.Id)
                .HasColumnType("decimal(18,0)");
                //.ValueGeneratedNever(); 
                
            builder.Entity<Barco>()
                .ToTable("Barcos")
                .Property(p => p.ID_Barco)
                .HasColumnType("decimal(18,0)");

            builder.Entity<Barco>(e =>
            {
                e.HasMany<Escala>(x => x.Escala)
                    .WithOne(x => x.Barco)
                    .HasForeignKey(p => new { p.Buque })
                    .HasPrincipalKey(p => new { p.Codigo });
            });

            builder.Entity<Escala>()
               .ToTable("Escalas")             
               .HasKey(p => new { p.Puerto, p.NumeroEscala, p.AnnoEscala });

            //builder.Entity<Escala>(e =>
            //{
                //e.ToTable("Escalas");
                //e.HasKey(p => new { p.Puerto, p.NumeroEscala, p.AnnoEscala });
                //e.HasMany(x => x.Atraques)
                //    .WithOne(x => x.Escala)
                //    .HasForeignKey(x => x.ID)
                //    .HasPrincipalKey(x => x.ID_Atraque);


            //});


            builder.Entity<Atraque>()
               .ToTable("Atraques")
               .Property(p => p.ID_Atraque)
               .HasColumnType("decimal(18,0)");

           //builder.Entity<Atraque>(e =>
                //{
                    //e.HasOne(x => x.Escala)
                    //    .WithMany(x => x.Atraques)
                    //    .HasForeignKey(x => x.ID_Atraque)
                    //    .HasPrincipalKey(x => x.ID_Atraque);

                //}
                //);

            builder.Entity<EstadoEscala>()
              .ToTable("Estados")
              .HasNoKey();

            builder.Entity<Puerto>()
             .ToTable("Puertos")
              .HasKey(p => new { p.Codigo });

            builder.Entity<Puerto>(e =>
            {
                e.HasMany<Escala>(x => x.Escala)
                    .WithOne(x => x.PuertoOperacion)
                    .HasForeignKey(p => new { p.Puerto });                                 
            });

            builder.Entity<Muelle>()
               .ToTable("Muelles")
               .Property(p => p.ID)
               .HasColumnType("decimal(18,0)");

            builder.Entity<TipoAtraque>()
              .ToTable("TiposAtraque")
              .HasKey(p => new { p.Codigo });

            builder.Entity<TipoOperacionPortuaria>()
             .ToTable("TipoOperacionesPortuarias")
             .HasKey(p => new { p.Codigo });

            builder.Entity<Pais>()
            .ToTable("Paises")
            .HasKey(p => new { p.Codigo });

            builder.Entity<Manifiesto>(e =>
            {
                e.ToTable("Manifiesto");
                e.HasKey(p => new { p.Puerto, p.NumeroEscala, p.AnnoEscala, p.Orden, p.Actividad });
                e.HasOne(p => p.Escala)
                    .WithMany(p => p.Manifiesto)
                    .HasForeignKey(p => new { p.Puerto, p.NumeroEscala, p.AnnoEscala });                              

            });
            builder.Entity<TipoPartida>()
           .ToTable("TiposPartida")
           .HasKey(p => new { p.Tipo });

            builder.Entity<Error>(e =>
            {
                e.ToTable("Errores");
                e.HasKey(p => new { p.ERR_NUMSECUENCIAL });
                e.HasOne(p => p.Escala)
                    .WithMany(p => p.Error)
                    .HasForeignKey(p => new { p.Puerto, p.NumeroEscala, p.AnnoEscala })
                    .HasPrincipalKey(p => new { p.Puerto, p.NumeroEscala, p.AnnoEscala }); 
                    
            });

            builder.Entity<TipoVehiculos>()
            .ToTable("TablaTiposVehiculo")
            .HasKey(p => new { p.IdVehiculo });


            builder.Entity<UserProfile>()
              .ToTable("UserProfile")
              .Property(p => p.UserId);
             

            base.OnModelCreating(builder);
        }

    }
}
