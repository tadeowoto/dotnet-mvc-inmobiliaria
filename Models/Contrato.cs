

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace inmobiliaria.Models
{
    [Table("contrato")]
    public class Contrato
    {
        [Key]
        [Column("id_contrato")]
        public int id_contrato { get; set; }
        [Column("fecha_inicio")]
        public DateTime fechaInicio_contrato { get; set; }
        [Column("fecha_finalizacion")]
        public DateTime fechaFin_contrato { get; set; }

        [Required(ErrorMessage = "El monto del contrato es obligatorio.")]
        [Column("monto_alquiler")]
        public int monto_contrato { get; set; }
        [Column("id_inmueble")]
        public int idInmueble { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un inquilino.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un inquilino válido.")]
        [Column("id_inquilino")]
        public int idInquilino { get; set; }
        [NotMapped]
        public Inmueble? Inmueble { get; set; }
        [NotMapped]
        public Inquilino? Inquilino { get; set; }

        [Column("estado")]
        public bool anulado_contrato { get; set; } = false;

        public Contrato() { }

        public Contrato(int id_contrato, DateTime fechaInicio_contrato, DateTime fechaFin_contrato, int monto_contrato, int idInmueble, int idInquilino, Inmueble? Inmueble, Inquilino? Inquilino)
        {
            this.id_contrato = id_contrato;
            this.fechaInicio_contrato = fechaInicio_contrato;
            this.fechaFin_contrato = fechaFin_contrato;
            this.idInmueble = idInmueble;
            this.idInquilino = idInquilino;
            this.monto_contrato = monto_contrato;
            this.Inmueble = Inmueble;
            this.Inquilino = Inquilino;
        }

    }


}