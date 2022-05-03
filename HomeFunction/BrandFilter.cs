using WEB_Shop_Ajax.Models;
using System.Text.Json;

namespace WEB_Shop_Ajax.HomeFunction
{
    public class BrandFilter
    {
        public void ProductFilter(string? jsonfilter, IQueryable<string> brandFilter, ref IQueryable<Product> products, ref FilterViewModel filterViewModel)
        {
            filterViewModel.Brand = brandFilter;
            filterViewModel.jsonfilter = jsonfilter;

            string[] masFilter = Array.Empty<string>();
            try
            {
                masFilter = JsonSerializer.Deserialize<string[]>(jsonfilter).ToArray();
            }
            catch (Exception)
            {

            }
            if (masFilter.Length != 0)
            {
                var models = masFilter.ToList();
                if (models.Count != 0)
                {
                    products = products.Where(x => models.Contains(x.Brand));
                }
            }
            return;
        }
    }
}
