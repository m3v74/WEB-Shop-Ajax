using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WEB_Shop_Ajax.Data;
using WEB_Shop_Ajax.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Text;

namespace WEB_Shop_Ajax.Controllers
{
    public class ProductsController : Controller
    {
        private readonly WEB_Shop_AjaxContext _context;
        IWebHostEnvironment _appEnvironment;
        public ProductsController(WEB_Shop_AjaxContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }
        // GET: Products
        public IActionResult Index()
        {
            try
            {
                return new OkObjectResult(_context.Product.ToList());
            }
            catch (Exception)
            {
                return null;
            }
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var img = _context.Images.Where(x => x.ProductId == id).ToList();
            var product = await _context.Product.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            product.Images = img;
            return View(product);
        }

        // Cart
        public async Task<IActionResult> Cart()
        {
           
            return View();
        }

       

        // GET: Products/ImagesAdd
        public IActionResult ImagesAdd()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImagesAdd(Product product, IFormFileCollection uploadedFiles)
        {
            
            if (ModelState.IsValid)
            {
                product.Brand = product.Brand.Trim();
                //product.Brand = product.Brand.Replace(" ", "_");
                product.Model = product.Model.Trim();
                //product.Model = product.Model.Replace(" ", "_");
                var path = Directory.GetCurrentDirectory() + @"\wwwroot\Images\" + product.Brand + @"\" + product.Model;
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }

                foreach (var uploadedFile in uploadedFiles)
                {
                    if (uploadedFiles != null)
                    {
                        // путь к папке Files
                        string path2 = "/Images/" + product.Brand + @"\" + product.Model + @"\" + uploadedFile.FileName;
                        // сохраняем файл в папку Files в каталоге wwwroot
                        using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path2, FileMode.Create))
                        {
                            await uploadedFile.CopyToAsync(fileStream);
                        }
                        _context.SaveChanges();
                    }
                }

                _context.Add(product);
                await _context.SaveChangesAsync();

                foreach (var uploadedFile in uploadedFiles)
                {
                    if (uploadedFile != null)
                    {
                        // images.Name = uploadedFile.FileName;
                        int prod = product.Id;
                        string name = uploadedFile.FileName;
                        _context.Database.ExecuteSqlRaw("INSERT INTO Images (ProductId, Name) VALUES ({0},{1})", prod, name);
                    }

                }
                _context.SaveChanges();

                return RedirectToRoute(new { controller = "Home", action = "Index" });
            }
            return View(product);

        }


        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var img = _context.Images.Where(x => x.ProductId == id).ToList();
            var product = await _context.Product.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            product.Images = img;
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, Image image, IFormFileCollection uploadedFiles, string? delimg)
        {
            if (id != product.Id)
            {
                return NotFound();
            }


            if (delimg != null)
            {
                string[] delimgmas = JsonSerializer.Deserialize<string[]>(delimg).ToArray();

                foreach (var img in delimgmas)
                {
                    if (delimg != null)
                    {
                        string path = Directory.GetCurrentDirectory() + @"\wwwroot\Images\" + product.Brand + @"\" + product.Model + @"\" + img;
                        FileInfo fileInf = new FileInfo(path);
                        if (fileInf.Exists)
                        {
                            fileInf.Delete();
                        }
                        _context.SaveChanges();
                    }
                }
                string imgdel = "DELETE FROM Images WHERE ";
                foreach (var img in delimgmas)
                {
                    if (delimg != null)
                    {
                        var idprod = product.Id;
                        string newimgdel = $"(Name= \'{img}\' AND ProductId = {idprod}) OR ";
                        imgdel = imgdel + newimgdel;
                    }
                }
                imgdel = imgdel.Remove(imgdel.Length - 4);
                _context.Database.ExecuteSqlRaw(imgdel);
                _context.SaveChanges();
            }

            foreach (var uploadedFile in uploadedFiles)
            {
                if (uploadedFiles != null)
                {
                    // путь к папке Files
                    string path2 = "/Images/" + product.Brand + @"\" + product.Model + @"\" + uploadedFile.FileName;
                    // сохраняем файл в папку Files в каталоге wwwroot
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path2, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }
                    _context.SaveChanges();
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (uploadedFiles.Count != 0)
                    {
                        string query = "INSERT INTO Images (ProductId, Name) VALUES ";
                        foreach (var uploadedFile in uploadedFiles)
                        {
                            if (uploadedFile != null)
                            {
                                int prod = product.Id;
                                string name = uploadedFile.FileName;
                                string newQuery = $"({prod},\'{name}\'),";
                                query = query + newQuery;
                            }
                        }
                        query = query.Remove(query.Length - 1);
                        //Тут нужно из строки query удалить последнюю запятую
                        _context.Database.ExecuteSqlRaw(query);
                        _context.SaveChanges();
                    }


                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                string path = Directory.GetCurrentDirectory() + @"\wwwroot\Images\" + product.Brand;
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                if (dirInfo.Exists)
                {

                    dirInfo.Delete(true);
                }
            }

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToRoute(new { controller = "Home", action = "Index" });
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
