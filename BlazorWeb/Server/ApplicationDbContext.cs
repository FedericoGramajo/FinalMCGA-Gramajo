using BlazorWeb.Shared.Objetos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWeb.Server
{
   
    public class ApplicationDbContext : IdentityDbContext
    {
        //le paso la cadena de conexion al objeto con la opcion configurada en el start  up
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<IdentityUser>().ToTable("Usuarios");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("RolesUsuarios");


        }


        public DbSet<Producto> Productos { get; set; }
   
        public DbSet<Categoria> Categorias { get; set; }
  
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<HistorialProducto> HistorialProductos { get; set; }
      
    }
    
}
