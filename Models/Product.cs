using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WEB_Shop_Ajax.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required (ErrorMessage = "Поле должно быть установлено")]
        public string? Model { get; set; }
        [Required (ErrorMessage = "Поле должно быть установлено")]
        public string? Brand { get; set; }
        [Required (ErrorMessage = "Поле должно быть установлено")]
        public double? Price { get; set; }
        public string? Description { get; set; }
        public List<Image>? Images { get; set; }
        [Required (ErrorMessage = "Поле должно быть установлено")]
        public int? Count { get; set; }
        
        public Product()
        {
            Images = new List<Image>();

        }
        
    }
}
