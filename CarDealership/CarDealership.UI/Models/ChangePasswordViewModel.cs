using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace CarDealership.UI.Models
{
    public class ChangePasswordViewModel : IValidatableObject
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>(); // to be displayed with Html.ValidationSummary() in view

            // must enter current password
            if (string.IsNullOrEmpty(CurrentPassword))
                errors.Add(new ValidationResult("Please enter your current Password."));

            // must enter password of adequate format, must match confirm
            if (string.IsNullOrEmpty(NewPassword))
                errors.Add(new ValidationResult("Please enter a new Password."));
            else
            {
                // alphanumeric + specials, 8-20 chars, at least 1 letter/num
                Regex pattern = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,30}$");

                if (NewPassword.Length < 8 || !pattern.IsMatch(NewPassword))
                    errors.Add(new ValidationResult("New Password must be 8-30 characters long, with at least 1 letter and 1 number."));

                if (!string.Equals(NewPassword, ConfirmPassword))
                    errors.Add(new ValidationResult("New Password does not match Confirm New Password."));
            }

            return errors;
        }
    }
}