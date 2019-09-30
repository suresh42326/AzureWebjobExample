using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureWebjobExample.Models
{
    public class User
    {
        public string PKUserId { get; set; }
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string IsActive { get; set; }
        public string Notes { get; set; }
    }
}