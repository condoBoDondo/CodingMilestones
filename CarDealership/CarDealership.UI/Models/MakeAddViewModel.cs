using CarDealership.Models.Queries;
using CarDealership.Models.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CarDealership.UI.Models
{
    public class MakeAddViewModel : IValidatableObject
    {
        public IEnumerable<MakeReport> MakeReports { get; set; } // list of makes to display under add form
        public Make Make { get; set; } // new make that you're filling out

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>(); // to be displayed with Html.ValidationSummary() in view

            // enter make name
            if (string.IsNullOrEmpty(Make.MakeName))
                errors.Add(new ValidationResult("Make Name is required."));

            return errors;
        }
    }
}