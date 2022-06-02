using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using MySql.Data.MySqlClient;
using PriceWatchEntrySystem.Models;
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
            return View();
        }

        public ActionResult DataVisualization()
        {
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


        [HttpGet]
        public ActionResult Register()
        {
            return View();
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