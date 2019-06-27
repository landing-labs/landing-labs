using System;
using System.Collections.Generic;

namespace Test.DomainModel
{
    public class User : Entity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<Application> Applications { get; set; }


        public User()
        {
            Roles = new List<Role>();
            Applications = new List<Application>();
        }
    }
}