
using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace inmobiliaria.Api.Controllers
{

    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ContratoController : ControllerBase
    {

        private readonly DataContext contexto;

        public ContratoController(DataContext context)
        {
            this.contexto = context;
        }


        [HttpGet("/api/contratos/inmueble/{id_inmueble}")]
        public IActionResult getContratosPorInmueble(int id_inmueble)
        {
            try
            {
                var iduser = int.Parse(User.Claims.First(c => c.Type == "Id").Value);
                var user = contexto.Propietarios.Find(iduser);
                if (user == null)
                {
                    return NotFound("Propietario no encontrado");
                }
                //verificar si el inmueble pertenece al propietario logueado
                var inmueble = contexto.Inmuebles.Find(id_inmueble);
                if (inmueble == null || inmueble.PropietarioId != iduser)
                {
                    return Forbid("Inmueble no encontrado o no pertenece al propietario.");
                }

                var contratos = contexto.Contratos
                    .Where(c => c.idInmueble == id_inmueble)
                    .ToList();

                if (contratos == null || contratos.Count == 0)
                {
                    return NotFound("No se encontraron contratos para el inmueble especificado.");
                }
                return Ok(contratos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpGet("/api/pagos/contrato/{id_contrato}")]
        public IActionResult getPagosPorContrato(int id_contrato)
        {
            try
            {
                var iduser = int.Parse(User.Claims.First(c => c.Type == "Id").Value);
                var user = contexto.Propietarios.Find(iduser);
                if (user == null)
                {
                    return NotFound("Propietario no encontrado");
                }

                var contrato = contexto.Contratos.Find(id_contrato);
                if (contrato == null)
                {
                    return NotFound("Contrato no encontrado.");
                }
                var inmueble = contexto.Inmuebles.Find(contrato.idInmueble);
                if (inmueble == null || inmueble.PropietarioId != iduser)
                {
                    return Forbid("El contrato no pertenece a un inmueble del propietario.");
                }

                var pagos = contexto.Set<Pago>()
                    .Where(p => p.contratoId == id_contrato)
                    .ToList();
                return Ok(pagos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }






    }

}