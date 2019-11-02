using System;
using System.Collections.Generic;
using System.Text;

namespace modstaz.Libraries
{
    public class UserRole
    {
        public List<Role> Roles { get; set; }

        public UserRole()
        {
            Roles = new List<Role>()
            {
                new Role() {Id = 1, Name = "Super User"},
                new Role() {Id = 2, Name = "Creator"},
                new Role() {Id = 3, Name = "Delete"},
                new Role() {Id = 4, Name = "Edit"},
                new Role() {Id = 5, Name = "Add"},
                new Role() {Id = 6, Name = "View"}
            };
        }


        public class Role
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
