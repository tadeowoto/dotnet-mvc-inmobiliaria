
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

        /// <summary>
        /// Obtiene todos los contratos de un inmueble específico.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación JWT.
        /// Solo el propietario del inmueble puede ver esta información.
        /// </remarks>
        /// <param name="id_inmueble">El ID (int) del inmueble a consultar.</param>
        /// <response code="200">Devuelve la lista de contratos (IEnumerable&lt;Contrato&gt;).</response>
        /// <response code="401">No autorizado (Token JWT inválido o ausente).</response>
        /// <response code="403">Acceso prohibido (El inmueble no pertenece al propietario).</response>
        /// <response code="404">No se encontró el propietario, el inmueble, o contratos asociados.</response>
        /// <response code="400">Error en la solicitud (Excepción).</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Contrato>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

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

        /// <summary>
        /// Obtiene todos los pagos asociados a un contrato específico.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación JWT.
        /// El propietario debe ser dueño del inmueble asociado al contrato.
        /// </remarks>
        /// <param name="id_contrato">El ID (int) del contrato a consultar.</param>
        /// <response code="200">Devuelve la lista de pagos (IEnumerable&lt;Pago&gt;).</response>
        /// <response code="401">No autorizado (Token JWT inválido o ausente).</response>
        /// <response code="403">Acceso prohibido (El contrato no pertenece a un inmueble del propietario).</response>
        /// <response code="404">No se encontró el propietario o el contrato.</response>
        /// <response code="400">Error en la solicitud (Excepción).</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Pago>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

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