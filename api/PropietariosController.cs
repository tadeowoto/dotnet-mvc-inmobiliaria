using inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria.lib;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace inmobiliaria.Api.Controllers
{
    [ApiController]
    public class PropietariosController : ControllerBase
    {

        private readonly DataContext contexto;
        private readonly HashPasswordService service;
        private readonly IConfiguration _config;

        public PropietariosController(DataContext context, IConfiguration config, HashPasswordService service)
        {
            this.contexto = context;
            this.service = service;
            _config = config;

        }


        /// <summary>
        /// Inicia sesión para un propietario y genera un token JWT.
        /// </summary>
        /// <remarks>
        /// Espera datos (email y password) enviados como form-data o x-www-form-urlencoded.
        /// </remarks>
        /// <param name="data">Objeto LoginData con email y password.</param>
        /// <response code="200">Login exitoso. Devuelve el token JWT (string).</response>
        /// <response code="400">Usuario o contraseña incorrectos, o excepción.</response>
        [HttpPost("api/Propietarios/login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public IActionResult Login([FromForm] LoginData data)
        {
            try
            {
                String hashedPassword = service.HashPassword(data.password);
                var propietario = contexto.Propietarios
                    .FirstOrDefault(p => p.email_propietario == data.email);

                if (propietario != null && propietario.password_propietario == hashedPassword)
                {
                    var key = new SymmetricSecurityKey(
                     Encoding.UTF8.GetBytes(_config["Jwt:Key"])
                    );
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var claims = new[]
                    //info que se va a guardar en el token
                    {
                        new Claim("Id", propietario.id_propietario.ToString()),
                        new Claim("FullName", propietario.nombre_propietario + " " + propietario.apellido_propietario),
                        new Claim("Email", propietario.email_propietario)
                    };

                    var token = new JwtSecurityToken(
                        issuer: _config["Jwt:Issuer"],
                        audience: _config["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(90),
                        signingCredentials: creds
                    );
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Usuario o contraseña incorrectos");

                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Actualiza el perfil del propietario logueado.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación JWT. Espera datos como form-data.
        /// El ID en los datos debe coincidir con el ID del token.
        /// </remarks>
        /// <param name="data">Objeto UpdateProfileData con los datos a modificar.</param>
        /// <response code="200">Perfil actualizado correctamente (string).</response>
        /// <response code="401">No autorizado (No tiene permisos o el ID no coincide).</response>
        /// <response code="404">Propietario no encontrado.</response>
        /// <response code="400">Error en la solicitud (Excepción).</response>
        [HttpPut("api/Propietarios/updateProfile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public IActionResult UpdateProfile([FromForm] UpdateProfileData data)
        {
            try
            {
                //Busco el propietario logueado
                var id = int.Parse(User.Claims.First(c => c.Type == "Id").Value);
                var propietario = contexto.Propietarios.Find(id);
                if (propietario == null)
                {
                    return NotFound("Propietario no encontrado");
                }
                if (propietario.id_propietario != data.id_propietario)
                {
                    return Unauthorized("No tiene permiso para actualizar este perfil");
                }
                propietario.nombre_propietario = data.nombre_propietario;
                propietario.apellido_propietario = data.apellido_propietario;
                propietario.email_propietario = data.email_propietario;
                propietario.telefono_propietario = data.telefono_propietario;

                contexto.Propietarios.Update(propietario);
                contexto.SaveChanges();
                return Ok("Perfil actualizado correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Cambia la contraseña del propietario logueado.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación JWT. Espera datos como form-data.
        /// Valida que la contraseña antigua (OldPassword) sea correcta.
        /// </remarks>
        /// <param name="data">Objeto ChangePasswordData con OldPassword y NewPassword.</param>
        /// <response code="200">Contraseña actualizada correctamente (string).</response>
        /// <response code="400">La contraseña actual es incorrecta, o excepción.</response>
        /// <response code="404">Propietario no encontrado.</response>
        [HttpPut("api/Propietarios/changePassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public IActionResult ChangePassword([FromForm] ChangePasswordData data)
        {
            try
            {
                //Busco el propietario logueado
                var id = int.Parse(User.Claims.First(c => c.Type == "Id").Value);
                var propietario = contexto.Propietarios.Find(id);
                if (propietario == null)
                {
                    return NotFound("Propietario no encontrado");
                }
                //Hasheo la contraseña vieja y la comparo con la que tiene el propietario en la bd
                String hashedOldPassword = service.HashPassword(data.OldPassword);
                if (propietario.password_propietario != hashedOldPassword)
                {
                    return BadRequest("La contraseña actual es incorrecta");
                }
                //Si ya esta aca, es decir que la contraseña vieja es correcta
                //Hasheo la nueva contraseña y la actualizo en la bd
                String hashedNewPassword = service.HashPassword(data.NewPassword);
                propietario.password_propietario = hashedNewPassword;
                contexto.Propietarios.Update(propietario);
                contexto.SaveChanges();
                return Ok("Contraseña actualizada correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        /// <summary>
        /// Obtiene los datos del propietario actualmente logueado (desde el token).
        /// </summary>
        /// <remarks>
        /// Requiere autenticación JWT.
        /// Devuelve el objeto Propietario sin la contraseña.
        /// </remarks>
        /// <response code="200">Devuelve el objeto `Propietario` (sin password).</response>
        /// <response code="401">No autorizado (Token JWT inválido o ausente).</response>
        /// <response code="404">Propietario no encontrado.</response>
        /// <response code="400">Error en la solicitud (Excepción).</response>
        [HttpGet("api/Propietarios/logged")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Propietario))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public IActionResult getPropietarioLogged()
        {
            try
            {
                var id = int.Parse(User.Claims.First(c => c.Type == "Id").Value);
                var propietario = contexto.Propietarios.Find(id);
                if (propietario == null)
                {
                    return NotFound("Propietario no encontrado");
                }
                Propietario propietarioSinPassword = new Propietario
                {
                    id_propietario = propietario.id_propietario,
                    dni_propietario = propietario.dni_propietario,
                    nombre_propietario = propietario.nombre_propietario,
                    apellido_propietario = propietario.apellido_propietario,
                    email_propietario = propietario.email_propietario,
                    telefono_propietario = propietario.telefono_propietario,
                };
                return Ok(propietarioSinPassword);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene todos los inmuebles del propietario logueado.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación JWT.
        /// </remarks>
        /// <response code="200">Devuelve la lista de inmuebles (IEnumerable&lt;Inmueble&gt;).</response>
        /// <response code="401">No autorizado (Token JWT inválido o ausente).</response>
        /// <response code="400">Error en la solicitud (Excepción).</response>
        [HttpGet("/api/Propietarios/inmuebles")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Inmueble>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public IActionResult getInmueblesPropietario()
        {
            try
            {
                // traigo el id de las claims del usuario logueado
                var id = int.Parse(User.Claims.First(c => c.Type == "Id").Value);
                //busco en la bd todos los inmuebles que tengan ese id de propietario
                var inmuebles = contexto.Inmuebles
                    .Where(i => i.PropietarioId == id)
                    .ToList();

                return Ok(inmuebles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
