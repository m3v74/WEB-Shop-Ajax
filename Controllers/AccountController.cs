using WEB_Shop_Ajax.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using WEB_Shop_Ajax.Models;
using WEB_Shop_Ajax.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WEB_Shop_Ajax.ViewModels;
using System.Text.Json;


namespace WEB_Shop_Ajax.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly WEB_Shop_AjaxContext _context;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, WEB_Shop_AjaxContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email };
                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //добавление роли при регистрации
                    await _userManager.AddToRoleAsync(user, "user");
                    // установка куки
                    await _signInManager.SignInAsync(user, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {           
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                       // _signInManager.UpdateExternalAuthenticationTokensAsync(_signInManager.ExternalLoginSignInAsync);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ArrangeOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArrangeOrder(User user)
        {
            if (ModelState.IsValid)
            {
                
            
                var FirstName = user.UserFirstName;
                var SurName = user.UserSurName;
                var Adress = user.HomeAdress;
                string? HistoryJson = user.History;
                var manager = _context.Users.Where(x => x.UserName == user.Email).FirstOrDefault();
                if (manager != null)
                {
                    string? oldhistoryjson = manager.History;
                    if (oldhistoryjson != null)
                    {
                        HistoryCartJson oldhistory = JsonSerializer.Deserialize<HistoryCartJson>(oldhistoryjson);
                        HistoryCartJson History = JsonSerializer.Deserialize<HistoryCartJson>(HistoryJson);
                        for(int i = 0; i < History.History.Count; i++)
                        {
                            oldhistory.History.Add(History.History[i]);
                        }

                        
                        var newHistory = JsonSerializer.Serialize(oldhistory);
                        manager.History = newHistory;
                    }
                    else
                    {
                        manager.History = HistoryJson;
                    }
                    
                    if (Adress != null)manager.HomeAdress = Adress;
                    if (FirstName != null) manager.UserFirstName = FirstName;
                    if (SurName != null) manager.UserSurName = SurName;
                    
                    _context.SaveChanges();

                }
            }
            
            return RedirectToAction("Cart", "Products");
        }

        /*[HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User? user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(user); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }*/
        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                //new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id), new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddDays(3650) });
        }

    }
}