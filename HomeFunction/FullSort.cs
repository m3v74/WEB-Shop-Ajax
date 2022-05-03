using WEB_Shop_Ajax.Models;
namespace WEB_Shop_Ajax.HomeFunction
{
    public class FullSort
    {
        public void FullSortVoid(ref SortState sortOrder, ref IQueryable<Product> products)
        {
            switch (sortOrder)
            {
                case SortState.BrandDesc:
                    products = products.OrderByDescending(s => s.Brand);
                    break;
                case SortState.PriceAsc:
                    products = products.OrderBy(s => s.Price);
                    break;
                case SortState.PriceDesc:
                    products = products.OrderByDescending(s => s.Price);
                    break;
                case SortState.ModelAsc:
                    products = products.OrderBy(s => s.Model);
                    break;
                case SortState.ModelDesc:
                    products = products.OrderByDescending(s => s.Model);
                    break;
                default:
                    products = products.OrderBy(s => s.Brand);
                    break;
            }
            return;
        }
    }
}
