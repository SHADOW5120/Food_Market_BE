using System.ComponentModel.DataAnnotations;

namespace Food_Market_BE.Modules.CartModule.Dtos
{
    public class SelectedOption
    {
        [Required]
        public string OptionId { get; set; } = default!;

        [Required]
        public string ValueId { get; set; } = default!;
    }
}
