using AzureWebjobExample.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;

namespace AzureWebjobExample.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            string con = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            string query = "select  PKUserId,UserName,EmailId,Password,IsActive,Notes from UserInfo";
            // create connection and command
            List<User> userInfos = new List<User>();
            using (SqlConnection cn = new SqlConnection(con))
            {
                // open connection, execute INSERT, close connection
                cn.Open();
                SqlDataAdapter sde = new SqlDataAdapter(query, con);
                DataSet ds = new DataSet();
                sde.Fill(ds);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    User user = new User();
                    user.EmailId = dr["EmailId"].ToString();
                    user.Password = dr["Password"].ToString();
                    user.PKUserId = dr["PKUserId"].ToString();
                    user.Notes = dr["Notes"].ToString();
                    userInfos.Add(user);
                   
                }
                sde.Fill(ds);
                cn.Close();
            }
            return View(userInfos);
        }

        // GET: User/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        public ActionResult Create(User user)
        {
            try
            {
                string con = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
                string userId = Guid.NewGuid().ToString();
                string query = "INSERT INTO dbo.UserInfo (PKUserId, UserName, EmailId, Password, IsActive) " +
                    "VALUES (@PKUserId, @UserName, @EmailId, @Password, @IsActive) ";

                // create connection and command
                using (SqlConnection cn = new SqlConnection(con))

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    // define parameters and their values
                    cmd.Parameters.Add("@PKUserId", SqlDbType.VarChar, 50).Value = userId;
                    cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 50).Value = user.UserName;
                    cmd.Parameters.Add("@EmailId", SqlDbType.VarChar, 50).Value = user.EmailId;
                    cmd.Parameters.Add("@Password", SqlDbType.VarChar, 50).Value = user.Password;
                    cmd.Parameters.Add("@IsActive", SqlDbType.VarChar, 50).Value = true;
                    // open connection, execute INSERT, close connection
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                    //inserting queue message
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageCon"].ConnectionString);
                    CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                    CloudQueue queue = queueClient.GetQueueReference("queue1");
                    CloudQueueMessage message = new CloudQueueMessage(userId);
                    queue.AddMessage(message);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: User/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
