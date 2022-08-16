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
    public class PurchaseViewModel : IValidatableObject
    {
        // need details of selected vehicle, dropdown lists, and pending sale
        public VehicleDetails VehicleDetails { get; set; }
        public IEnumerable<SelectListItem> States { get; set; }
        public IEnumerable<SelectListItem> PurchaseTypes { get; set; }
        public Sale Sale { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            // must enter name, address1, city, zip, email OR phone
            if (string.IsNullOrEmpty(Sale.CustomerName))
                errors.Add(new ValidationResult("Customer Name is required."));

            if (string.IsNullOrEmpty(Sale.Address1))
                errors.Add(new ValidationResult("Address 1 is required."));

            if (string.IsNullOrEmpty(Sale.City))
                errors.Add(new ValidationResult("City is required."));

            // zip must also be 5 characters long
            if (string.IsNullOrEmpty(Sale.Zip) || Sale.Zip.Length != 5)
                errors.Add(new ValidationResult("Zip is required and must be 5 characters long."));

            if (string.IsNullOrEmpty(Sale.Email) && string.IsNullOrEmpty(Sale.Phone))
                errors.Add(new ValidationResult("Email and/or Phone is required."));

            // validate purchase price; can't be below 95% of sale price
            if (Sale.PurchasePrice < Decimal.Multiply(0.95m, VehicleDetails.SalePrice))
                errors.Add(new ValidationResult("Purchase Price cannot be below 95% of sale price."));

            return errors;
        }
    }
}