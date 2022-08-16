using CarDealership.Models.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CarDealership.UI.Models
{
    // model needed for Admin/VehicleAdd view
    public class VehicleAddViewModel : IValidatableObject
    {
        // get all models that we need to select as dropdown menus
        public IEnumerable<SelectListItem> BodyStyles { get; set; }
        public IEnumerable<SelectListItem> Exteriors { get; set; }
        public IEnumerable<SelectListItem> Interiors { get; set; }
        public IEnumerable<SelectListItem> Makes { get; set; }
        public IEnumerable<SelectListItem> Models { get; set; }
        public IEnumerable<SelectListItem> Transmissions { get; set; }
        public Vehicle Vehicle { get; set; }
        public HttpPostedFileBase ImageUpload { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>(); // to be displayed with Html.ValidationSummary()

            // can't have make with no models
            if (Vehicle.ModelId == 0)
                errors.Add(new ValidationResult("You've selected a Make with no available Models."));

            // year must be between 2000 and current year + 1
            if (Vehicle.Year < 2000 || Vehicle.Year > DateTime.Now.AddYears(1).Year)
                errors.Add(new ValidationResult("Year must be between 2000 and " + DateTime.Now.AddYears(1).Year + "."));

            // mileage can't be negative
            if (Vehicle.Mileage < 0)
                errors.Add(new ValidationResult("Mileage cannot be a negative value."));

            // VIN must be 17 characters long
            // extra credit: use regex to validate
            if (string.IsNullOrEmpty(Vehicle.VIN) || Vehicle.VIN.Length != 17)
                errors.Add(new ValidationResult("VIN must be exactly 17 characters long."));

            // MSRP can't be negative
            if (Vehicle.MSRP < 0)
                errors.Add(new ValidationResult("MSRP cannot be a negative value."));

            // sale price can't be negative, can't be above MSRP
            if (Vehicle.SalePrice < 0)
                errors.Add(new ValidationResult("Sale Price cannot be a negative value."));
            else if (Vehicle.SalePrice > Vehicle.MSRP)
                errors.Add(new ValidationResult("Sale Price cannot be higher than MSRP value."));

            // description can't be empty
            if (string.IsNullOrEmpty(Vehicle.Description))
                errors.Add(new ValidationResult("Description is required."));

            // check for valid image
            if (ImageUpload != null && ImageUpload.ContentLength > 0)
            {
                // get valid image file extension
                var validExts = new string[] { ".jpg", ".jpeg", ".png" };
                var ext = Path.GetExtension(ImageUpload.FileName);

                if (!validExts.Contains(ext))
                    errors.Add(new ValidationResult("Image must be a .jpg, .jpeg, or .png file."));
            }
            else
                errors.Add(new ValidationResult("Image file is required."));

            return errors;
        }
    }
}