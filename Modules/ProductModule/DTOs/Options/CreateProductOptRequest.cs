namespace Food_Market_BE.Modules.ProductModule.DTOs.Options
{
    public class CreateProductOptRequest
    {
        public string Name { get; set; } = default!;

        public bool IsRequired { get; set; } = false;

        public bool IsMultiple { get; set; } = false;

        public List<CreateProductOptValueRequest> Values { get; set; } = new();
    }
}