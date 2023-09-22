using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.QualityMonitoring
{
    public class AddTechnicalExpert
    {
        public int ID { get; set; }

        [Display(Name = "First Name :")]
        [Required(ErrorMessage = "First Name is required.")]
        [RegularExpression(@"^([a-zA-Z _.]+)$", ErrorMessage = "First Name is not in valid format.")]
        [StringLength(50, ErrorMessage = "First Name must be less than 50 characters.")]
        public string TECHNICAL_EXPERT_FNAME { get; set; }

        [Display(Name = "Middle Name :")]
        [RegularExpression(@"^([a-zA-Z _.]+)$", ErrorMessage = "Middle Name is not in valid format.")]
        [StringLength(50, ErrorMessage = "Middle Name must be less than 50 characters.")]
        public string TECHNICAL_EXPERT_MNAME { get; set; }

        [Display(Name = "Last Name :")]
        [Required(ErrorMessage = "Last Name is required.")]
        [RegularExpression(@"^([a-zA-Z _.]+)$", ErrorMessage = "Last Name is not in valid format.")]
        [StringLength(50, ErrorMessage = "Last Name must be less than 50 characters.")]
        public string TECHNICAL_EXPERT_LNAME { get; set; }

        [Display(Name = "PAN Number :")]
        [Required(ErrorMessage = "Please enter PAN Number")]
        [StringLength(10, ErrorMessage = "PAN Number is not in valid format.")]
        [RegularExpression(pattern: @"([A-Z]){5}([0-9]){4}([A-Z]){1}$", ErrorMessage = "PAN Number is not in valid format.")]
        public string PAN_NUMBER { get; set; }

        [Display(Name = "Mobile Number :")]
        [Required(ErrorMessage = "Mobile is required.")]
        [RegularExpression(@"([0-9]+)$", ErrorMessage = "Invalid Mobile number")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be 10 digits only.")]
        public string TECHNICAL_EXPERT_MOBILE { get; set; }

        [Display(Name = "Email :")]
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Email is not in valid format.")]
        [StringLength(100, ErrorMessage = "Email address must be less than 100 characters.")]
        public string TECHNICAL_EXPERT_EMAIL { get; set; }

        public string ADD_EDIT_FLAG { get; set; }

    }
}