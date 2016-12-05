using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyRent_v0.Models
{
    public class RenterProfileModels
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string FullName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Gender { get; set; }

        [Display(Name = "Date of Brith")]
        public DateTime DOB { get; set; }

        [Display(Name = "Home Phone")]
        public string LandLine { get; set; }

        [Display(Name = "Mobile Phone")]
        public string Mobile { get; set; }
        public string Fax { get; set; }

        [Display(Name = "Unit Number")]
        public string UnitNumber { get; set; }

        [Display(Name = "Street Number")]
        public string StreetNumber { get; set; }
        public string Address { get; set; }
        public string Suburb { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int PostCode { get; set; }

    }
}