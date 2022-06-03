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

    public class PricesTableRows
    {
        public string Barcode { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Wellcome_Price { get; set; }
        public string PARNnShop_Price { get; set; }
        public string MarketPlace_Price { get; set;  }
        public string Watsons_Price { get; set; }
        public string Mannings_Price { get; set; }
        public string AEON_PRice { get; set; }
        public string DCHFoodMart_Price { get; set; }
    }

    public class PriceTableViewModel
    {
        public string CatagoryName { get; set; }
        public List<PricesTableRows> listPricesTableRows { get; set; }
    }
    
}