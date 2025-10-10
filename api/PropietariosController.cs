
using inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria.Api.Controllers
{



    public class PropietariosController : ControllerBase
    {

        private readonly DataContext contexto;

        public PropietariosController(DataContext context)
        {
            this.contexto = context;
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
    }

}