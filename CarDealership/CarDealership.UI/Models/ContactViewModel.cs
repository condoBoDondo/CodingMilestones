using CarDealership.Models.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CarDealership.UI.Models
{
    public class ContactViewModel : IValidatableObject
    {
        public Contact Contact { get; set; } // model to get/set within view

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // validation that will work regardless of whether javascript is enabled
            List<ValidationResult> errors = new List<ValidationResult>(); // to be displayed with Html.ValidationSummary() in view

            // must enter name, either email OR phone, and message
            if (string.IsNullOrEmpty(Contact.Name))
                errors.Add(new ValidationResult("Name must be filled out."));

            if (string.IsNullOrEmpty(Contact.Email) && string.IsNullOrEmpty(Contact.Phone))
                errors.Add(new ValidationResult("Email and/or Phone must be filled out."));

            if (string.IsNullOrEmpty(Contact.Message))
                errors.Add(new ValidationResult("Message must be filled out."));

            return errors;
        }
    }
}