using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fileManagerApi.ViewModel
{
    public class userModel
    {
        public string userId { get; set; }
        public string userName { get; set; }
        public string userMail { get; set; }
        public string userGroup { get; set; }
        public string userPassword { get; set; }
        public byte[] userProfilImg { get; set; }
    }
}