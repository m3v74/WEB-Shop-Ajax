namespace WEB_Shop_Ajax.Models
{
    public class Image
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public Product? Product { get; set; }
    }
}
