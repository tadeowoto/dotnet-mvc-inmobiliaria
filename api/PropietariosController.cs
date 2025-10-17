
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

        [HttpGet]
        [Route("api/helloworld")]
        public IActionResult Get()
        {

            try
            {
                return Ok(new
                {
                    mensaje = "Hola mundo desde Web API",
                    Error = false,
                    Fecha = DateTime.Now,
                    Propietario = new
                    {
                        Nombre = "Pedro",
                        Apellido = "Picapiedra",
                        Email = "TtT9o@example.com"
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("api/Propietarios/{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var propietario = contexto.Propietarios.Find(id);
                if (propietario == null)
                {
                    return NotFound();
                }
                return Ok(propietario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //metodos reales

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
                return Ok(propietario);
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