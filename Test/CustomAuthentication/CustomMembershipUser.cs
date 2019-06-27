using System;
using System.Collections.Generic;
using System.Web.Security;
using Test.DomainModel;

namespace Test.CustomAuthentication
{
    public class CustomMembershipUser : MembershipUser
    {
        #region User Properties

        public int Id { get; set; }
       
        public ICollection<Role> Roles { get; set; }

        #endregion

        public CustomMembershipUser(User user) : base("CustomMembership", user.Username, user.Id, user.Email, string.Empty, string.Empty, true, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now)
        {
            Id = user.Id;
            Roles = user.Roles;
        }
    }
}