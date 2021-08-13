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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CategoriasController : ControllerBase
    {
        private readonly ApplicationDbContext context;


        public CategoriasController(
            ApplicationDbContext context)
        {
            this.context = context;

        }


        [HttpGet("mostrar")]
        public async Task<ActionResult<List<Categoria>>> Get([FromQuery] Paginar paginacion, [FromQuery] string nombre)
        {
            var queryable = context.Categorias.OrderBy(x => x.Rubro).AsQueryable();
            if (!string.IsNullOrEmpty(nombre))
            {
                queryable = queryable.Where(x => x.Rubro.ToLower().Contains(nombre.ToLower()));
            }
            await HttpContext.InsertarParametrosPaginacionEnRespuesta(queryable, paginacion.CantidadRegistros);
            return await queryable.Paginar(paginacion).ToListAsync();
        }



        [HttpGet("mostrartodos")]
        public async Task<ActionResult<List<Categoria>>> Get2()
        {
            var queryable = context.Categorias.OrderBy(x => x.Rubro).AsQueryable();
            return await queryable.ToListAsync();
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> Get(int id)
        {
            var categoria = await context.Categorias.FirstOrDefaultAsync(x => x.CategoriaId == id);

            if (categoria == null) { return NotFound(); }

            return categoria;
        }



       



        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] Categoria categoria)
        {
            try
            {
                context.Add(categoria);
                await context.SaveChangesAsync();
                return categoria.CategoriaId;

            }
            catch (Exception ex)
            {
                string error_ = ex.Message;
                return BadRequest("ERROR categoria no fue creado");
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] Categoria categoria)
        {
            try
            {
                context.Entry(categoria).State = EntityState.Modified;
                var result = await context.SaveChangesAsync();

                if (result > 0)
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest("ERROR categoria no modificado");
                }
            }
            catch (Exception ex)
            {
                string error_ = ex.Message;
                return BadRequest("ERROR categoria no modificado");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var categoria = await context.Categorias.FirstOrDefaultAsync(x => x.CategoriaId == id); ;
                context.Categorias.Remove(categoria);
                var result = await context.SaveChangesAsync();
                if (result > 0)
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest("ERROR categoria no fue borrado");
                }
            }
            catch (Exception ex)
            {
                string error_ = ex.Message;
                return BadRequest("ERROR categoria no fue borrado");
            }


        }


    }

}
