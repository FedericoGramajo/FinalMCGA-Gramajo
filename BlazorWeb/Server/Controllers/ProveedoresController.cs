using BlazorWeb.Server.Helpers;
using BlazorWeb.Shared.Objetos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProveedoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
       

        public ProveedoresController(
            ApplicationDbContext context)
        {
            this.context = context;
          
        }


        [HttpGet("mostrar")]
        public async Task<ActionResult<List<Proveedor>>> Get([FromQuery] Paginar paginacion, [FromQuery] string nombre)
        {
            var queryable = context.Proveedores.OrderBy(x => x.Nombre).AsQueryable();
            if (!string.IsNullOrEmpty(nombre))
            {
                queryable = queryable.Where(x => x.Nombre.ToLower().Contains(nombre.ToLower()));
            }
            await HttpContext.InsertarParametrosPaginacionEnRespuesta(queryable, paginacion.CantidadRegistros);
            return await queryable.Paginar(paginacion).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Proveedor>> Get(int id)
        {
            var proveedore = await context.Proveedores.FirstOrDefaultAsync(x => x.Id == id);

            if (proveedore == null) { return NotFound(); }

            return proveedore;
        }

        [HttpGet("mostrartodos")]
        public async Task<ActionResult<List<Proveedor>>> Get2()
        {

            return await context.Proveedores.OrderBy(x => x.Nombre).ToListAsync(); ;

        }



        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] Proveedor proveedore)
        {
            try
            {
                context.Add(proveedore);
                await context.SaveChangesAsync();
                return proveedore.Id;

            }

            catch (Exception ex)
            {
                string error_ = ex.Message;
                return BadRequest("ERROR proveedor no fue creado");
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] Proveedor proveedore)
        {
            try
            {
                context.Entry(proveedore).State = EntityState.Modified;
                var result = await context.SaveChangesAsync();

                if (result > 0)
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest("ERROR proveedor no modificado");
                }
            }
            catch (Exception ex)
            {
                string error_ = ex.Message;
                return BadRequest("ERROR proveedor no modificado");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var proveedore = await context.Proveedores.FirstOrDefaultAsync(x => x.Id == id); ;
                context.Proveedores.Remove(proveedore);
                var result = await context.SaveChangesAsync();
                if (result > 0)
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest("ERROR proveedor no fue borrado");
                }
            }
            catch (Exception ex)
            {
                string error_ = ex.Message;
                return BadRequest("ERROR proveedor no fue borrado");
            }


        }


    }

}
        
 