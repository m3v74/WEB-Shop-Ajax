using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace WEB_Shop_Ajax.Models
{
    public class User : IdentityUser
    {        
        public bool Enabled { get; set; }
        public string UserFirstName { get; set; }
        public string UserSurName { get; set; }
        public string HomeAdress { set; get; }
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public string? History { get; set; }
       
    }
}
