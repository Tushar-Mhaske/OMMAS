using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Master
{
    public class MasterTestViewModel
    {
        [UIHint("hidden")]
        public string EncryptedTestCode { get; set; }

        public int MAST_TEST_CODE { get; set; }
        
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Please enter name.")]
        [RegularExpression(@"^([a-zA-Z0-9 ()%]+)$", ErrorMessage = "Only Alpanumeric Characters are allowed.")]
        [StringLength(25, ErrorMessage = "Name must be less than 25 characters")]
        public string MAST_TEST_NAME { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Please enter description.")]
        [StringLength(255, ErrorMessage = "Description must be less than 255 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._',\r\n&()-]+)$", ErrorMessage = "Description is not in valid format.")]          
        public string MAST_TEST_DESC { get; set; }

        [Display(Name = "Active")]
        public string MAST_TEST_STATUS { get; set; }

        public bool status { get; set; }
    }
}