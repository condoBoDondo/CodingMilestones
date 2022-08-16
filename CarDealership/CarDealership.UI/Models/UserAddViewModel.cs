using CarDealership.Models.Queries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace CarDealership.UI.Models
{
    public class UserAddViewModel : IValidatableObject
    {
        // need user to be added role dropdown, and passwords
        public UserShort User { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            // first name, last name, email must be filled out

            if (string.IsNullOrEmpty(User.FirstName))
                errors.Add(new ValidationResult("First Name is required."));

            if (string.IsNullOrEmpty(User.LastName))
                errors.Add(new ValidationResult("Last Name is required."));

            if (string.IsNullOrEmpty(User.Email))
                errors.Add(new ValidationResult("Email is required."));

            // must enter password of adequate format, must match confirm
            if (string.IsNullOrEmpty(Password))
                errors.Add(new ValidationResult("Password is required."));
            else
            {
                // alphanumeric + specials, 8-20 chars, at least 1 letter/num
                Regex pattern = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,30}$");

                if (Password.Length < 8 || !pattern.IsMatch(Password))
                    errors.Add(new ValidationResult("Password must be 8-30 characters long, with at least 1 letter and 1 number."));

                if (!string.Equals(Password, ConfirmPassword))
                    errors.Add(new ValidationResult("Password does not match Confirm Password."));
            }

            return errors;
        }
    }
}