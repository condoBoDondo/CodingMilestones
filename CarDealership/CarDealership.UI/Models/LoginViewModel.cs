using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CarDealership.UI.Models
{
    public class LoginViewModel : IValidatableObject
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>(); // to be displayed with Html.ValidationSummary() in view

            // must enter name and password
            if (string.IsNullOrEmpty(UserName))
                errors.Add(new ValidationResult("Please enter UserName."));

            if (string.IsNullOrEmpty(Password))
                errors.Add(new ValidationResult("Please enter Password."));

            return errors;
        }
    }
}