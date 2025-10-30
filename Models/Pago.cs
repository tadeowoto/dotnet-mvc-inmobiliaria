
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace inmobiliaria.Models
{
    [Table("pago")]
    public class Pago
    {
        [Key]
        public int id_pago { get; set; }

        [Required(ErrorMessage = "El número de pago es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe ingresar un número válido.")]
        [Column("nro_pago")]
        public int numero_pago { get; set; }

        [Required(ErrorMessage = "El detalle del pago es obligatorio.")]
        [StringLength(200, ErrorMessage = "El detalle no puede superar los 200 caracteres.")]
        [Column("detalle")]
        [JsonPropertyName("detalle")]
        public String detalle_pago { get; set; }

        [Required(ErrorMessage = "La fecha de pago es obligatoria.")]
        [Column("fecha_pago")]
        public DateTime fecha_de_pago { get; set; }

        [Required(ErrorMessage = "El monto del pago es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ingrese un monto válido.")]
        [Column("importe_pago")]
        [JsonPropertyName("monto")]
        public Double monto_pago { get; set; }


        [Column("estado")]
        [JsonPropertyName("estado")]
        public Boolean esta_anulado { get; set; }
        [Column("id_alquiler")]

        public int contratoId { get; set; }
        [NotMapped]
        public Contrato? Contrato { get; set; }


        public Pago() { }

        public Pago(int id_pago, int numero_pago, String detalle_pago, DateTime fecha_de_pago, Double monto_pago, Boolean esta_anulado, int contratoId, Contrato contrato)
        {
            this.id_pago = id_pago;
            this.numero_pago = numero_pago;
            this.detalle_pago = detalle_pago;
            this.fecha_de_pago = fecha_de_pago;
            this.monto_pago = monto_pago;
            this.esta_anulado = esta_anulado;
            this.contratoId = contratoId;
            this.Contrato = contrato;
        }

    }
}

