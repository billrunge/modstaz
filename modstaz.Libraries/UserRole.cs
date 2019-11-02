using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modstaz.Libraries
{
    public class UserRole
    {
        public List<Role> RolesList { get; set; }

        public UserRole()
        {
            RolesList = new List<Role>()
            {
                new Role() {Id = (int)Roles.SuperUser, Name = "Super User"},
                new Role() {Id = (int)Roles.Creator, Name = "Creator"},
                new Role() {Id = (int)Roles.Delete, Name = "Delete"},
                new Role() {Id = (int)Roles.Edit, Name = "Edit"},
                new Role() {Id = (int)Roles.Add, Name = "Add"},
                new Role() {Id = (int)Roles.View, Name = "View"}
            };
        }
        public enum Roles
        {
            SuperUser = 1,
            Creator = 2,
            Delete = 3,
            Edit = 4,
            Add = 5,
            View = 6
        }
        public class Role
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
