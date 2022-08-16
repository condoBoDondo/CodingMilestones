using CarDealership.Models.Queries;
using CarDealership.Models.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CarDealership.UI.Models
{
    public class ModelAddViewModel : IValidatableObject
    {
        public IEnumerable<SelectListItem> Makes { get; set; }
        public IEnumerable<ModelReport> ModelReports { get; set; } // list of models to display under add form
        public Model ModelModel { get; set; } // needed to rename from just "Model" as that was causing problems with model binding

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>(); // to be displayed with Html.ValidationSummary() in view

            // enter model name
            if (string.IsNullOrEmpty(ModelModel.ModelName))
                errors.Add(new ValidationResult("Model Name is required."));

            return errors;
        }
    }
}