using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SingleWebApi
{
    public class UserController : ApiController
    {
        public UserModel getAdmin()
        {
            return new UserModel() { UserID = "000", UserName = "Admin" };
        }

        public bool add(UserModel user)
        {
            return user != null;
        }
    }
}