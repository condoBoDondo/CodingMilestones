using CarDealership.Models.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CarDealership.UI.Models
{
    public class SpecialAddViewModel : IValidatableObject
    {
        public IEnumerable<Special> Specials { get; set; } // list of all specials to display below add form
        public Special Special { get; set; } // new special that you're filling out

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>(); // to be displayed with Html.ValidationSummary() in view

            // must enter title and description
            if (string.IsNullOrEmpty(Special.Title))
                errors.Add(new ValidationResult("Title is required."));

            if (string.IsNullOrEmpty(Special.Description))
                errors.Add(new ValidationResult("Description is required."));

            return errors;
        }
    }
}