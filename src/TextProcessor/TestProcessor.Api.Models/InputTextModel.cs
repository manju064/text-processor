using System.ComponentModel.DataAnnotations;

namespace TextProcessor.Api.Models
{
    /// <summary>
    /// Input text model for Api end point
    /// </summary>
    public class InputTextModel
    {
        [Required]
        [MinLength(1)]
        public string Text { get; set; }
    }
}
