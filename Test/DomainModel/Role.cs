using System.Collections.Generic;

namespace Test.DomainModel
{
    public class Role : Entity
    {
        public string Name { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}