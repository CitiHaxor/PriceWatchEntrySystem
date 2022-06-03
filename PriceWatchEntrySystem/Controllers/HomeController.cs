using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using MySql.Data.MySqlClient;
using PriceWatchEntrySystem.Models;
using CsvHelper;
using System.Xml.Linq;
using System.IO;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace PriceWatchEntrySystem.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            if (IsUserLoggedIn() == false)
                return RedirectToAction("Login");
            else
                return View();
        }

        [HttpGet]
        public ActionResult DataEntry()
        {
            if (IsUserLoggedIn() == false)
                return RedirectToAction("Login");

            return View();
        }

        [HttpPost]
        public ActionResult DataEntry(HttpPostedFileBase postedFile)
        {
            if (postedFile == null) return View();
                //Check file type
                
            string sFileName = System.IO.Path.GetFileName(postedFile.FileName);
            string sFileExt = System.IO.Path.GetExtension(sFileName);

            if (sFileExt == ".csv")
            {
                // Read the contents of the csv file, update the Product Price table
                using (var streamReader = new StreamReader(postedFile.InputStream))
                {
                    using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                    {
                        var records = csvReader.GetRecords<ProductInformation>().ToList();
                        DataTable qr= ExecuteSQLQuery(String.Format("select supermarket_id from user where Username = '{0}' " , Request.Cookies["CurUsername"].Value));
                        object o = qr.Rows[0][0].ToString();
                        int Supermarket = Int16.Parse(o.ToString());
                        try {
                        
                                string server = "35.220.178.165";
                                string uid = "peter";
                                string password = "123456";
                                string database = "CityHaxor";
                                string mycon = "server =" + server + "; Uid = " + uid + "; password = " + password + "; persistsecurityinfo = True; database = " + database + "; SslMode = none";
                                MySqlConnection con = new MySqlConnection(mycon);
                                MySqlCommand cmd = null;
                                
                                    con.Open();
                                    foreach (var VARIABLE in records)
                                    {
                                            DateTime oDate = Convert.ToDateTime(VARIABLE.Date);
                                        String SQLStatement = String.Format("Call insertPrice({0},'{1}',{2},'{3}');", Supermarket,
                                            VARIABLE.Barcode
                                            , VARIABLE.Price,oDate.ToString("yyyy-M-d"));
                                        
                                        Debug.Print(SQLStatement);
                                        cmd = new MySqlCommand(SQLStatement, con);
                                        cmd.ExecuteNonQuery();
                                    }
                                    con.Close();

                                
                        }
                        catch (Exception e) {
                            Debug.Print(e.ToString());
                        }

                    }
                }

            }
            return RedirectToAction("Index");
        }

        public class ProductInformation
        {
            [Name("price")] public float Price { get; set; }
            [Name("barcode")] public string Barcode{ get; set; }
            [Name("date")] public string Date{ get; set; }
        }

        [HttpGet]
        public ActionResult DataVisualization(string Catagory)
        {
            if (IsUserLoggedIn() == false)
                return RedirectToAction("Login");

            // Build the view model

            PriceTableViewModel priceTableViewModel = new PriceTableViewModel();

            if (Catagory != null)
            {
              
                priceTableViewModel.CatagoryName = Catagory;

                string SQLQueryString = @"SELECT b.barcode, b.brand, b.Name, p.Date, s.supermarket, ROUND(AVG(p.price), 1) AS p_avg, b.categories
                    FROM barcode_mapping AS b
                    LEFT JOIN product_prices AS p
                    ON p.barcode = b.barcode
                    LEFT JOIN supermarket AS s
                    ON s.supermarket_id = p.supermarket_id
                    WHERE b.categories = '"+ Catagory + @"' AND p.Date IS NOT NULL
                    GROUP BY p.Date,b.barcode,p.supermarket_id
                    ORDER BY p.Date DESC
                    ";

                // Get all product ids that matches the catagory
                DataTable listOfProductIDs = ExecuteSQLQuery(SQLQueryString);

                //
                for (int i = 0; i < listOfProductIDs.Rows.Count; i++)
                {
                    PricesTableRows pricesTableRows = new PricesTableRows();
                    pricesTableRows.Barcode = listOfProductIDs.Rows[i]["barcode"].ToString();
                    pricesTableRows.Brand = listOfProductIDs.Rows[i]["brand"].ToString();
                    pricesTableRows.Name = listOfProductIDs.Rows[i]["Name"].ToString();
                    pricesTableRows.Date = listOfProductIDs.Rows[i]["Date"].ToString();
                    int j = 0;
                    while (j < 7)
                    {
                        string curSupermarketName = listOfProductIDs.Rows[i]["supermarket"].ToString();
                        string curPrice = listOfProductIDs.Rows[i]["p_avg"].ToString();
                        if (curSupermarketName == "Wellcome")
                            pricesTableRows.Wellcome_Price = curPrice;
                        else if (curSupermarketName == "PARKnSHOP")
                            pricesTableRows.PARNnShop_Price = curPrice;
                        else if (curSupermarketName == "Market Place")
                            pricesTableRows.MarketPlace_Price = curPrice;
                        else if (curSupermarketName == "Watsons")
                            pricesTableRows.Watsons_Price = curPrice;
                        else if (curSupermarketName == "Mannings")
                            pricesTableRows.Mannings_Price = curPrice;
                        else if (curSupermarketName == "AEON")
                            pricesTableRows.AEON_Price = curPrice;
                        else if (curSupermarketName == "DCH Food Mart")
                            pricesTableRows.DCHFoodMart_Price = curPrice;

                        j++;
                        if (i < listOfProductIDs.Rows.Count - 1)
                            i++;
                    }
                    priceTableViewModel.listPricesTableRows.Add(pricesTableRows);
                }

            }
            


            return View(priceTableViewModel);
        }


        [HttpGet]
        public ActionResult Login()
        {
            LoginViewModel loginViewModel = new LoginViewModel();

            return View(loginViewModel);
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel loginViewModel)
        {
            string user = loginViewModel.Username;
            string pass = loginViewModel.Password;
            string SQLQuery = "SELECT * FROM user WHERE Username = '" + user + "' AND Password = '" + pass + "'";
            DataTable result = ExecuteSQLQuery(SQLQuery);


            if (result.Rows.Count == 1)
            {
                Response.Cookies["CurUsername"].Value = user;
                Response.Cookies["cookie"].Expires = DateTime.Now.AddMinutes(30);

                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Invalid username and password");

            }


            return View(loginViewModel);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Response.Cookies["CurUsername"].Expires = DateTime.Now.AddDays(-1);

            return View();
        }


        [HttpPost]
        public ActionResult Register(RegisterViewModel registerViewModel)
        {
            string registerUsername = registerViewModel.Username;
            string registerPassword = registerViewModel.Password;
            string registerRole = registerViewModel.SelectedRole;
            string registerSupermarket = registerViewModel.SelectedSupermarket;

            // Check if fields are empty
            if (registerUsername == null || registerPassword == null || registerRole == null || registerSupermarket == null)
            {
                ModelState.AddModelError("", "You need to fill in all the information");
                return View(registerViewModel);
            }
            
            // Check if username exists
            string SQLQuery = "SELECT `Username` FROM `user` WHERE `Username` = '" + registerUsername + "'";
            DataTable result = ExecuteSQLQuery(SQLQuery);
            if (result.Rows.Count != 0)
            {
                ModelState.AddModelError("", "This username already exists");
                return View(registerViewModel);
            }

            // Get role ID
            SQLQuery = "SELECT `role_id` FROM `role` WHERE `Role` = '" + registerRole + "'";
             result = ExecuteSQLQuery(SQLQuery);
            string registerRoleID = result.Rows[0]["role_id"].ToString();

            // Get supermarket ID
            SQLQuery = "SELECT `supermarket_id` FROM `supermarket` WHERE `supermarket` = '" + registerSupermarket + "'";
            result = ExecuteSQLQuery(SQLQuery);
            string registerSupermarketID = result.Rows[0]["supermarket_id"].ToString();

            string SQLUpdate = "INSERT INTO `user` (supermarket_id, `Password`, role_id, `Username`) VALUES (" + registerSupermarketID + ", '"+ registerPassword +"'," + registerRoleID + ",'" + registerUsername + "')";

            try
            {
                ExecuteSQLUpdate(SQLUpdate);
            }
            catch (Exception exception)
            {
                ModelState.AddModelError("", "A DB error occured");
                return View(registerViewModel);
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Register()
        {
            RegisterViewModel registerViewModel = new RegisterViewModel();



            return View(registerViewModel);
        }

        [HttpGet]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult DataGraphs()
        {

            return View();
        }


        public bool IsUserLoggedIn()
        {

            if (Request.Cookies["CurUsername"] != null)
                return true;
            else
                return false;
        }

        public DataTable ExecuteSQLQuery(string queryString)
        {
            string server = "35.220.178.165";
            string uid = "peter";
            string password = "123456";
            string database = "CityHaxor";
            string mycon = "server =" + server + "; Uid = " + uid + "; password = " + password + "; persistsecurityinfo = True; database = " + database + "; SslMode = none";
            MySqlConnection con = new MySqlConnection(mycon);
            DataTable dt = new DataTable();
            MySqlCommand cmd = null;
            try
            {
                cmd = new MySqlCommand(queryString, con);
                con.Open();
                dt.Load(cmd.ExecuteReader());
                con.Close();

            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "')</script>");
                con.Close();
            }

            //foreach (DataRow row in dt.Rows)
            //{
            //    string id = row["id"].ToString();
            //    string supermarket_id = row["supermarket_id"].ToString();
            //    string passout = row["Password"].ToString();
            //    string userout = row["Username"].ToString();

            //    System.Diagnostics.Debug.WriteLine(id + " " + supermarket_id + " " + passout + " " + userout);
            //}

            return dt;
        }

        public void ExecuteSQLUpdate(string updateString)
        {
            string server = "35.220.178.165";
            string uid = "peter";
            string password = "123456";
            string database = "CityHaxor";
            string mycon = "server =" + server + "; Uid = " + uid + "; password = " + password + "; persistsecurityinfo = True; database = " + database + "; SslMode = none";
            MySqlConnection con = new MySqlConnection(mycon);
            DataTable dt = new DataTable();
            MySqlCommand cmd = null;
            try
            {
                cmd = new MySqlCommand(updateString, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "')</script>");
                con.Close();
                return;
            }

            //foreach (DataRow row in dt.Rows)
            //{
            //    string id = row["id"].ToString();
            //    string supermarket_id = row["supermarket_id"].ToString();
            //    string passout = row["Password"].ToString();
            //    string userout = row["Username"].ToString();

            //    System.Diagnostics.Debug.WriteLine(id + " " + supermarket_id + " " + passout + " " + userout);
            //}

           

        }
    }
}