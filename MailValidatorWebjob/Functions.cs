using Microsoft.Azure.WebJobs;
using Newtonsoft.Json.Linq;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Net;

namespace MailValidatorWebjob
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger("queue1")] string message, TextWriter log)
        {
            Console.WriteLine(message);
            string con = "Server=SURESH-DECCANSO;Database=Test;Trusted_Connection=True;";
            string query = $"select EmailId from UserInfo where PKUserId = '{message}'";
            // create connection and command
            using (SqlConnection cn = new SqlConnection(con))

            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                // open connection, execute INSERT, close connection
                cn.Open();
                string emailid = cmd.ExecuteScalar().ToString();
                string url = $"http://apilayer.net/api/check?access_key=d5b2e1999ac6e9ee023dafa83c1c8611&email={emailid}&smtp=1&format=1";
                var client = new WebClient { Encoding = System.Text.Encoding.UTF8 };
                var res = client.DownloadString(url);
                JObject json = JObject.Parse(res);
                bool format_valid = Convert.ToBoolean(json.GetValue("format_valid"));
                bool smtp_check = Convert.ToBoolean(json.GetValue("smtp_check"));
                bool mx_found = Convert.ToBoolean(json.GetValue("mx_found"));
                if (format_valid && smtp_check && mx_found)
                {
                    query = $"update UserInfo set IsActive = 'true' and Notes = 'Email verified!' where PKUserId = '{message}'";
                    using (SqlCommand cmdUpdate = new SqlCommand(query, cn))
                    {
                        cmdUpdate.ExecuteNonQuery();
                    }
                }
                else
                {
                    query = $"update UserInfo set IsActive = 'false', Notes = 'Invalid Email!' where PKUserId = '{message}'";
                    using (SqlCommand cmdUpdate = new SqlCommand(query, cn))
                    {
                        cmdUpdate.ExecuteNonQuery();
                    }
                }
                cn.Close();
            }

        }
    }
}
