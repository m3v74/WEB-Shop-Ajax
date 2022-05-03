using System.Collections.Generic;
using WEB_Shop_Ajax.Models;

namespace WEB_Shop_Ajax.Models
{
    public class IndexViewModel
    {
        public IEnumerable<Product> Product { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public FilterViewModel FilterViewModel { get; set; }    
    }
}