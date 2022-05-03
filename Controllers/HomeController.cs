using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WEB_Shop_Ajax.Data;
using WEB_Shop_Ajax.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Xml.Linq;
using WEB_Shop_Ajax.HomeFunction;

namespace WEB_Shop_Ajax.Controllers
{   
    public class HomeController : Controller
    {
        WEB_Shop_AjaxContext db;

        public HomeController(WEB_Shop_AjaxContext context)
        {
            db = context;
        }
        public async Task<IActionResult> Index(string? jsonfilter, int page = 1,
            SortState sortOrder = SortState.BrandAsc)
        {
            IQueryable<Product> products = db.Product.Include(x => x.Images);

            // Фильтрация
            IQueryable<string>? brandFilter = products.Select(x => x.Brand).Distinct();
          
            FilterViewModel filterViewModel = new FilterViewModel();
            BrandFilter productFilter = new ();
            productFilter.ProductFilter(jsonfilter, brandFilter, ref products, ref filterViewModel);

            // сортировка
            FullSort fullSort = new ();
            fullSort.FullSortVoid(ref sortOrder, ref products);

            //Пагинация
            int pageSize = 5;

            var count = await products.CountAsync();
            var items = await products.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            IndexViewModel viewModel = new IndexViewModel
            
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel(sortOrder),   
                FilterViewModel = filterViewModel,
                Product = items
            };
            
            return View(viewModel);
        }
    }
}

