using System;
using System.Collections.Generic;
using System.Text;

namespace VTA.Buckets.Models
{
    public class User
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Type => typeof(User).Name;
    }
}
