using BlazorWeb.Server.Helpers;
using BlazorWeb.Shared.Objetos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWeb.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductosController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public ProductosController(ApplicationDbContext context)
        {
            this.context = context;
        }


        [HttpGet("mostrar")]
        public async Task<ActionResult<List<Producto>>> Get([FromQuery] Paginar paginacion, [FromQuery] string nombre)
        { 
            var queryable = context.Productos.Include(x=> x.Categoria).OrderBy(x => x.Nombre).AsQueryable();
            if (!string.IsNullOrEmpty(nombre))
            {
                queryable = queryable.Where(x => x.Nombre.ToLower().Contains(nombre.ToLower()));
            }
            await HttpContext.InsertarParametrosPaginacionEnRespuesta(queryable, paginacion.CantidadRegistros);
            return await queryable.Paginar(paginacion).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> Get(int id)
        {
            var producto = await context.Productos.Include(x => x.Categoria).Include(x=> x.Proveedor).FirstOrDefaultAsync(x => x.ProductoId == id);

            if (producto == null) { return NotFound(); }

            return producto;
        }


        [HttpGet("mostrartodos")]
        public async Task<ActionResult<List<Producto>>> Get2()
        {

            return await context.Productos.Include(x=> x.Categoria).Where(x => x.Discontinuado == "no").AsNoTracking().OrderBy(x => x.Nombre).ToListAsync();
        }

 
        [HttpGet("mostrarhistorial")]              // AGREGADO
        public async Task<ActionResult<List<HistorialProducto>>> Get5([FromQuery] Paginar paginacion, [FromQuery] string nombre)
        {
            var queryable = context.HistorialProductos.OrderByDescending(x => x.FechaEdicion).AsQueryable();
            if (!string.IsNullOrEmpty(nombre))
            {
                queryable = queryable.Where(x => x.Nombre.ToLower().Contains(nombre.ToLower()));
            }
            await HttpContext.InsertarParametrosPaginacionEnRespuesta(queryable, paginacion.CantidadRegistros);
            return await queryable.Paginar(paginacion).ToListAsync();
        }

 

        [HttpPost("{name}")]       //        INSERTAR
        public async Task<ActionResult<int>> Post([FromBody] Producto producto, string name)
        {
            try
            {
                var existe = context.Productos.Where(x => x.Codigo == producto.Codigo || x.CodigoBarra == producto.CodigoBarra).FirstOrDefault();
                if (existe != null)
                {
                    return BadRequest("ERROR el codigo de barras o el codigo interno seleccionado ya se encuentra cargado");
                }
    
                HistorialProducto historial = new HistorialProducto()
                {
                    Movimiento = "Insertar",
                    FechaEdicion = DateTime.Now,
                    UsuarioId = context.Users.Where(x => x.UserName == name).Select(x => x.Id).FirstOrDefault(),
                    NomUsusario = name,
                    ProductoId = producto.ProductoId,
                    Nombre = producto.Nombre,
                    Ant_Stock = context.Productos.Where(x => x.ProductoId == producto.ProductoId).Select(x => x.Stock).FirstOrDefault(),
                    Stock = producto.Stock,
                    Precio_compra = producto.Precio_compra,
                    Ant_Precio_venta = context.Productos.Where(x => x.ProductoId == producto.ProductoId).Select(x => x.Precio_venta).FirstOrDefault(),
                    Precio_venta = producto.Precio_venta,
                    Codigo = producto.Codigo
                };
                context.Entry(producto.Categoria).State = EntityState.Unchanged;
                context.Entry(producto.Proveedor).State = EntityState.Unchanged;
                context.Productos.Add(producto);
                var result = await context.SaveChangesAsync();
                historial.ProductoId = producto.ProductoId;
                context.HistorialProductos.Add(historial);                 
                    await context.SaveChangesAsync();
                if (result > 0)
                {
                    return producto.ProductoId;
                }
                else
                {
                    return BadRequest("ERROR producto no fue Agregado");
                }
            }

            catch (Exception ex)
            {
                string error_ = ex.Message;
                return BadRequest("ERROR producto no fue creado");
            }
        }


        [HttpPut("{name}")]       //  EDITAR
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Encargado, Stock")]
        public async Task<ActionResult> Put([FromBody] Producto producto, string name)          // juan
        {
            try
            {
                var existe = context.Productos.Where(x => (x.Codigo == producto.Codigo || x.CodigoBarra == producto.CodigoBarra) && x.ProductoId != producto.ProductoId).FirstOrDefault();
                if (existe != null)
                {
                    return BadRequest("ERROR el codigo de barras o el codigo interno seleccionado ya se encuentra cargado");
                }
                // juan                                     <--------------------------------------<<<<
                HistorialProducto historial = new HistorialProducto()
                {
                    Movimiento = "Editar",
                    FechaEdicion = DateTime.Now,
                    UsuarioId = context.Users.Where(x => x.UserName == name).Select(x => x.Id).FirstOrDefault(),
                    NomUsusario = name,
                    ProductoId = producto.ProductoId,
                    Nombre = producto.Nombre,
                    Ant_Stock = context.Productos.Where(x => x.ProductoId == producto.ProductoId).Select(x => x.Stock).FirstOrDefault(),   // juan
                    Stock = producto.Stock,
                    Precio_compra = producto.Precio_compra,
                    Ant_Precio_venta = context.Productos.Where(x => x.ProductoId == producto.ProductoId).Select(x => x.Precio_venta).FirstOrDefault(), //juan
                    Precio_venta = producto.Precio_venta,
                    Codigo = producto.Codigo
                };
                context.Entry(producto.Categoria).State = EntityState.Unchanged;
                context.Entry(producto.Proveedor).State = EntityState.Unchanged;
                context.Add(historial);                                         // juan
                context.Entry(producto).State = EntityState.Modified;
                var result = await context.SaveChangesAsync();

                if (result > 0)
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest("ERROR producto no modificado");
                }
            }
            catch (Exception ex)
            {
                string error_ = ex.Message;
                return BadRequest("ERROR producto no modificado");
            }
        }

        //           DELETE
        [HttpDelete("{id},{name}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Encargado, Stock")]
        public async Task<ActionResult> Delete(int id, string name) 
        {
            try
            {

                var producto = await context.Productos.FirstOrDefaultAsync(x => x.ProductoId == id);
                HistorialProducto historial = new HistorialProducto()
                {
                    Movimiento = "Eliminado",
                    FechaEdicion = DateTime.Now,
                    UsuarioId = context.Users.Where(x => x.UserName == name).Select(x => x.Id).FirstOrDefault(),
                    NomUsusario = name,
                    ProductoId = producto.ProductoId,
                    Nombre = producto.Nombre,
                    Ant_Stock = producto.Stock,                    // juan
                    Stock = producto.Stock,
                    Precio_compra = producto.Precio_compra,
                    Ant_Precio_venta = producto.Precio_venta,      // juan
                    Precio_venta = producto.Precio_venta,
                    Codigo = producto.Codigo
                     
                };

                context.HistorialProductos.Add(historial);           // juan
                context.Productos.Remove(producto);
                var result = await context.SaveChangesAsync();
                if (result > 0)
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest("ERROR producto no fue borrado");
                }
            }
            catch (Exception ex)
            {
                string error_ = ex.Message;
                return BadRequest("ERROR producto no fue borrado");
            }


        }
    }


}
