
using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace inmobiliaria.Api.Controllers
{

    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InmueblesController : ControllerBase
    {
        private readonly DataContext contexto;
        private readonly IWebHostEnvironment environment;

        public InmueblesController(DataContext context, IWebHostEnvironment environment)
        {
            this.contexto = context;
            this.environment = environment;
        }

        /// <summary>
        /// Obtiene los inmuebles del propietario logueado que tienen un contrato vigente.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación JWT.
        /// </remarks>
        /// <response code="200">Devuelve la lista de inmuebles con contrato vigente (IEnumerable&lt;Inmueble&gt;).</response>
        /// <response code="401">No autorizado (Token JWT inválido o ausente).</response>
        /// <response code="404">No se encontraron inmuebles con contrato vigente o el propietario no existe.</response>
        /// <response code="400">Error en la solicitud (Excepción).</response>
        [HttpGet("/api/Inmuebles/GetContratoVigente")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Inmueble>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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


        /// <summary>
        /// Actualiza el estado de disponibilidad de un inmueble.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación JWT. Solo el propietario puede modificar su inmueble.
        /// Espera un JSON con el id_inmueble y la disponibilidad_inmueble.
        /// </remarks>
        /// <param name="inmuebleActualizado">Objeto JSON con los datos del inmueble (importa `id_inmueble` y `disponibilidad_inmueble`).</param>
        /// <response code="200">Actualización exitosa (devuelve un string).</response>
        /// <response code="401">No autorizado (el inmueble no pertenece al propietario o el token es inválido).</response>
        /// <response code="404">Propietario o inmueble no encontrado.</response>
        /// <response code="400">Error en la solicitud (Excepción).</response>
        [HttpPut("/api/inmuebles/actualizar")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult actualizarDisponibilidad([FromBody] Inmueble inmuebleActualizado)
        {
            try
            {
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

        /// <summary>
        /// Crea un nuevo inmueble para el propietario logueado (incluye foto).
        /// </summary>
        /// <remarks>
        /// Requiere JWT. Este endpoint espera `multipart/form-data` para poder subir la foto.
        /// Enviar todos los campos del inmueble como form-data y el archivo de imagen en el campo 'foto_form'.
        /// </remarks>
        /// <param name="inmueble">Datos del inmueble enviados como form-data.</param>
        /// <response code="200">Inmueble creado exitosamente (devuelve el objeto `Inmueble` creado).</response>
        /// <response code="401">No autorizado (Token JWT inválido o ausente).</response>
        /// <response code="404">Propietario no encontrado.</response>
        /// <response code="400">Error en la solicitud (ej. 'Out of range' en la BD, o datos inválidos).</response>
        [HttpPost("/api/inmuebles/crear")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Inmueble))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult crearInmueble([FromForm] Inmueble inmueble)
        {
            try
            {
                var id = int.Parse(User.Claims.First(c => c.Type == "Id").Value);
                var propietario = contexto.Propietarios.Find(id);
                if (propietario == null)
                {
                    return NotFound("Propietario no encontrado");
                }
                inmueble.PropietarioId = id;
                if (inmueble.foto_form != null && inmueble.foto_form.Length > 0)
                {

                    string uploadsFolder = Path.Combine(environment.WebRootPath, "uploads/inmuebles");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(inmueble.foto_form.FileName);
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        inmueble.foto_form.CopyTo(stream);
                    }

                    // Guarda la ruta de acceso PÚBLICA
                    inmueble.foto_inmueble = "/uploads/inmuebles/" + fileName;
                }
                inmueble.disponibilidad_inmueble = false;
                contexto.Inmuebles.Add(inmueble);
                contexto.SaveChanges();

                return Ok(inmueble);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + " | InnerException: " + ex.InnerException?.Message);
            }
        }

        /// <summary>
        /// Obtiene un inmueble específico por su ID.
        /// </summary>
        /// <remarks>
        /// Requiere JWT. Solo el propietario del inmueble puede verlo.
        /// </remarks>
        /// <param name="id_inmueble">El ID (int) del inmueble a consultar.</param>
        /// <response code="200">Devuelve el objeto `Inmueble`.</response>
        /// <response code="401">No autorizado (Token JWT inválido o ausente).</response>
        /// <response code="403">Acceso prohibido (el inmueble no pertenece al propietario).</response>
        /// <response code="404">Propietario o inmueble no encontrado.</response>
        /// <response code="400">Error en la solicitud (Excepción).</response>
        [HttpGet("/api/inmuebles/{id_inmueble}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Inmueble))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult getInmueblePorId(int id_inmueble)
        {
            try
            {

                var iduser = int.Parse(User.Claims.First(c => c.Type == "Id").Value);
                var user = contexto.Propietarios.Find(iduser);
                if (user == null)
                {
                    return NotFound("Propietario no encontrado");
                }
                var inmueble = contexto.Inmuebles.Find(id_inmueble);
                if (inmueble == null)
                {
                    return NotFound("Inmueble no encontrado");
                }
                if (inmueble.PropietarioId != iduser)
                {
                    return Forbid("No tiene permisos para ver este inmueble.");
                }
                return Ok(inmueble);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}