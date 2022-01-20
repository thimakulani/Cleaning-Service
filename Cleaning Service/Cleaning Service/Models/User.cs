using Plugin.CloudFirestore.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cleaning_Service.Models
{
    public class User
    {
        [Id]
        public string Id { get; set; }
        [MapTo("FirstName")]
        public string FirstName { get; set; }
        [MapTo("LastName")]
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public string Latitude { get; set; } 
        public string Longitude { get; set; }
        public string Status { get; set; }
    }
}
