using BlazorWeb.Server.Helpers;
using BlazorWeb.Shared.Objetos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
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
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public UsuariosController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
        {
            this.context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

      
       
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserToken>> Login([FromBody] UserInfo userInfo)
        {
            // capa seguridad del lado cliente Autorize, esta capa deserealiza el passworrd
            var result = await _signInManager.PasswordSignInAsync(userInfo.UserName, 
                userInfo.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {

                var usuario = await _userManager.FindByNameAsync(userInfo.UserName);
                var roles = await _userManager.GetRolesAsync(usuario);

                return BuildToken(usuario, roles);
            }
            else
            {
                return BadRequest("Usuario o Password incorrecto");
            }
        }

        private UserToken BuildToken(IdentityUser usuario, IList<string> roles)
        {
            var claims = new List<Claim>()
            { // claim permisos de la pagina
        new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Id),
        new Claim(ClaimTypes.Name, usuario.UserName),
        new Claim(ClaimTypes.NameIdentifier, usuario.Id),
        // new Claim("miValor", "Lo que yo quiera"),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            foreach (var rol in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //tiempo que dura en expirar el tocken
            var expiration = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken token = new JwtSecurityToken(
               issuer: null,
               audience: null,
               claims: claims,
               expires: expiration,
               signingCredentials: creds);

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }


        [HttpGet("RenovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserToken>> Renovar()
        {
  
            var usuario = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var roles = await _userManager.GetRolesAsync(usuario);

            return BuildToken(usuario, roles);
        }

        [HttpGet("mostrar")]
        public async Task<ActionResult<List<Usuario>>> Get([FromQuery] Paginar paginacion, [FromQuery] string nombre)
        {
            var queryable = context.Users.OrderBy(x => x.UserName).AsQueryable();
            if (!string.IsNullOrEmpty(nombre))
            {
                queryable = queryable.Where(x => x.UserName.ToLower().Contains(nombre.ToLower()));
            }
            await HttpContext.InsertarParametrosPaginacionEnRespuesta(queryable, paginacion.CantidadRegistros);
            var resultado = await queryable.Paginar(paginacion).ToListAsync();
            var usuarios = (from IdentityUser user in resultado
                            select new Usuario() { ID = user.Id, Email = user.Email, UserName = user.UserName, Password = user.PasswordHash }).ToList();
            return usuarios;
        }


        [HttpGet("roles")]
        public async Task<ActionResult<List<Rol>>> Get()
        {
            return await context.Roles
                .Select(x => new Rol { Nombre = x.Name, RoleId = Convert.ToInt32(x.Id) }).OrderBy(x => x.Nombre).ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> Get(string id)
        {
            var user = await context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
            var usuario = new Usuario { ID = user.Id, Email = user.Email, UserName = user.UserName, Password = user.PasswordHash };
            var rol = context.Roles.Where(x => x.Id == context.UserRoles.Where(u => u.UserId == usuario.ID).Select(xx => xx.RoleId).FirstOrDefault()).FirstOrDefault();
            if (rol != null)
            {
                usuario.rol = new Rol() { RoleId = Convert.ToInt32(rol.Id), Nombre = rol.Name };
                usuario.RoleId = usuario.rol.RoleId;
            }
   
            if (usuario == null) { return NotFound(); }

            return usuario;
        }


        [HttpPost("Crear")]
        public async Task<ActionResult<string>> CreateUser([FromBody] Usuario model)
        {
            var user = new IdentityUser { UserName = model.UserName, Email = model.Email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, model.Password);
            await _userManager.UpdateAsync(user);
            await _userManager.AddToRoleAsync(user, model.rol.Nombre );
            if (result.Succeeded)
            {
                return user.Id;
            }
            else
            {
                return BadRequest("Usuario o Password invalido, Password debe contener letras y numeros");
            }
        }




        [HttpPut("Editar")]
        public async Task<ActionResult<IdentityResult>> UpdateUser([FromBody] Usuario model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.ID);
                user.UserName = model.UserName;
                user.Email = model.Email;
                var hasher = new PasswordHasher<IdentityUser>();
                user.PasswordHash = hasher.HashPassword(user, model.Password);
       
                var result = await _userManager.UpdateAsync(user);
                if (model.rol != null)
                {
                var rol = context.Roles.Where(x => x.Id == model.RoleId.ToString()).FirstOrDefault();
                if (rol != null)
                { 
                await _userManager.RemoveFromRoleAsync(user, rol.Name); //borro el rol anterior 
                }
                await _userManager.AddToRoleAsync(user, model.rol.Nombre);
                }



                if (result.Succeeded)
                {
                    return result;
                }
                else
                {
                    return BadRequest("ERROR Usuario no modificado, tenga en cuenta: el Password debe contener letras y numeros");
                }
            }
            catch (Exception ex)
            {
                string error_ = ex.Message;
                return BadRequest("ERROR Usuario no modificado");
            }

        }
        [HttpDelete("Eliminar/{idUsuario}")]
        public async Task<ActionResult<IdentityResult>> DeleteUser(string Idusuario)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(Idusuario);
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return result;
                }
                else
                {
                    return BadRequest("ERROR Usuario no fue borrado");
                }
            }
            catch (Exception ex)
            {
                string error_ = ex.Message;
                return BadRequest("ERROR Usuario no fue borrado");
            }



        }
    }
}