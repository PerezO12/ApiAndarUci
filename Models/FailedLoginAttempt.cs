using System;
using System.ComponentModel.DataAnnotations;

namespace ApiUCI.Models
{
    public class FailedLoginAttempt
    {
        [Key]
        [Required(ErrorMessage = "El Id es obligatorio.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "La dirección IP es obligatoria.")]
        [RegularExpression(@"^(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)\.((25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)\.){2}(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)$", 
            ErrorMessage = "La dirección IP no tiene un formato válido.")]
        [MaxLength(45, ErrorMessage = "La dirección IP no puede exceder los 45 caracteres.")]
        public string IPAddress { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "El número de intentos debe ser un valor positivo.")]
        public int AttemptCount { get; set; } = 0;

        [Required(ErrorMessage = "La fecha del último intento es obligatoria.")]
        public DateTime LastAttempt { get; set; } = DateTime.UtcNow;

        public DateTime? LockoutEnd { get; set; }
    }
}
