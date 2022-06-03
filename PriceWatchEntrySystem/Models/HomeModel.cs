using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PriceWatchEntrySystem.Models
{
    public class HomeModel
    {
       

    }

    public class LoginViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterViewModel
    { 
        public string Username { get; set;  }
        public string Password { get; set; }
        public string SelectedRole { get; set; }
        public string SelectedSupermarket { get; set; }
    }
    
}