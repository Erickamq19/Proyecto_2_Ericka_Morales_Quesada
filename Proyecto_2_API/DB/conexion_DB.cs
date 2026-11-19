using Microsoft.EntityFrameworkCore;
using Proyecto_2_API.Modelos;

namespace Proyecto_2_API.DB
{
    public class conexion_DB : DbContext
    {

        public conexion_DB(DbContextOptions<conexion_DB> options) : base (options)
        {

     

        }

        public DbSet<Estados> Estados { get; set; }
        public DbSet<Prioridades> Prioridades { get; set; }
        public DbSet<Proyectos> Proyectos { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Tareas> Tareas { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Estados>().ToTable("Estados");
            modelBuilder.Entity<Prioridades>().ToTable("Prioridades");
            modelBuilder.Entity<Proyectos>().ToTable("Proyectos");
            modelBuilder.Entity<Roles>().ToTable("Roles");
            modelBuilder.Entity<Tareas>().ToTable("Tareas");
            modelBuilder.Entity<Usuarios>().ToTable("Usuarios");
            //Claves foraneas
            modelBuilder.Entity<Usuarios>().HasOne<Roles>().WithMany().HasForeignKey(u => u.IdRol).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Proyectos>().HasOne<Usuarios>().WithMany().HasForeignKey(i => i.IdUsuarioCreador).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Tareas>().HasOne<Proyectos>().WithMany().HasForeignKey(a => a.IdProyecto).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Tareas>().HasOne<Usuarios>().WithMany().HasForeignKey(o => o.IdUsuarioCreador).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Tareas>().HasOne<Usuarios>().WithMany().HasForeignKey(t => t.IdUsuarioAsignado).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Tareas>().HasOne<Estados>().WithMany().HasForeignKey(l => l.IdEstado).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Tareas>().HasOne<Prioridades>().WithMany().HasForeignKey(j => j.IdPrioridad).OnDelete(DeleteBehavior.Restrict);
            //Valores de cada tabla
            //Usuarios
            modelBuilder.Entity<Usuarios>().Property(f => f.IdRol).HasDefaultValue(2);
            modelBuilder.Entity<Usuarios>().Property(m => m.FechaRegistro).HasDefaultValueSql("SYSDATETIME()");
            modelBuilder.Entity<Usuarios>().Property(z => z.Activo).HasDefaultValue(true);
            //Proyectos
            modelBuilder.Entity<Proyectos>().Property(c => c.FechaCreacion).HasDefaultValueSql("SYSDATETIME()");
            modelBuilder.Entity<Proyectos>().Property(v => v.Activo).HasDefaultValue(true);
            modelBuilder.Entity<Proyectos>().HasOne<Usuarios>().WithMany().HasForeignKey(s => s.IdResponsable).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Proyectos>().HasOne<Estados>().WithMany().HasForeignKey(g => g.IdEstado).OnDelete(DeleteBehavior.Restrict);
            //Tareas
            modelBuilder.Entity<Tareas>().Property(k => k.IdEstado).HasDefaultValue(1);
            modelBuilder.Entity<Tareas>().Property(b => b.IdPrioridad).HasDefaultValue(2);
            modelBuilder.Entity<Tareas>().Property(x => x.FechaCreacion).HasDefaultValueSql("SYSDATETIME()"); 




        }

    }

   
}
