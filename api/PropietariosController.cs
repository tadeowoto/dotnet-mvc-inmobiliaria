
using inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria.lib;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace inmobiliaria.Api.Controllers
{
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



        [HttpPost("api/Propietarios/login")]
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

        [HttpPut("api/Propietarios/changePassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
                Console.WriteLine("La contraseña actual es correcta");
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





        [HttpGet("api/Propietarios/logged")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [HttpGet("/api/Propietarios/inmuebles")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
