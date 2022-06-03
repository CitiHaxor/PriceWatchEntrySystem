using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Globalization;
using MySql.Data.MySqlClient;
using PriceWatchEntrySystem.Models;
using CsvHelper;
using System.Xml.Linq;
using System.IO;

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
                        var records = csvReader.GetRecords<dynamic>().ToList();
                    }
                }

            }
            return RedirectToAction("Index");

        }

        [HttpGet]
        public ActionResult DataVisualization(string Catagory)
        {
            if (IsUserLoggedIn() == false)
                return RedirectToAction("Login");

            // Build the view model
            
            
            if (Catagory != null)
            {
                PriceTableViewModel priceTable = new PriceTableViewModel();
                priceTable.CatagoryName = Catagory;




                PricesTableRows pricesTableRows = new PricesTableRows();
            }
            


            return View();
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