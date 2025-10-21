
using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace inmobiliaria.Api.Controllers
{

    public class InmueblesController : ControllerBase
    {
        private readonly DataContext contexto;

        public InmueblesController(DataContext context)
        {
            this.contexto = context;
        }

        [HttpGet("/api/Inmuebles/GetContratoVigente")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult getInmueblesConContratoVigente()
        {
            try
            {
                // traigo el id de las claims del usuario logueado
                var id = int.Parse(User.Claims.First(c => c.Type == "Id").Value);
                //busco en la bd todos los inmuebles que tengan ese id de propietario
                var inmuebles = contexto.Inmuebles
                    .Where(i => i.PropietarioId == id)
                    .ToList();

                //tengo que buscar los inmuebles que tengan true en contrato vigente
                var inmueblesConContratoVigente = inmuebles.Where(
                    i => i.tieneContratoVigente == true
                );
                if (inmueblesConContratoVigente == null)
                {
                    return NotFound("No se encontraron inmuebles con contrato vigente");
                }
                else if (inmueblesConContratoVigente.Count() == 0)
                {
                    return NotFound("No se encontraron inmuebles con contrato vigente");
                }
                return Ok(inmueblesConContratoVigente);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("/api/inmuebles/actualizar")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult actualizarDisponibilidad([FromBody] Inmueble inmuebleActualizado)
        {
            try
            {
                Console.WriteLine("Disponibilidad a actualizar: " + inmuebleActualizado.disponibilidad_inmueble);

                var id = int.Parse(User.Claims.First(c => c.Type == "Id").Value);
                var propietario = contexto.Propietarios.Find(id);
                if (propietario == null)
                {
                    return NotFound("Propietario no encontrado");
                }
                if (id != propietario.id_propietario)
                {
                    return Unauthorized("No tiene permisos para actualizar este inmueble");
                }
                var inmueble = contexto.Inmuebles.Find(inmuebleActualizado.id_inmueble);
                if (inmueble == null)
                {
                    return NotFound("Inmueble no encontrado");
                }

                inmueble.disponibilidad_inmueble = inmuebleActualizado.disponibilidad_inmueble;
                Console.WriteLine("Disponibilidad actualizada a: " + inmueble.disponibilidad_inmueble);

                contexto.Inmuebles.Update(inmueble);
                contexto.SaveChanges();
                return Ok("Disponibilidad del inmueble actualizada correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}