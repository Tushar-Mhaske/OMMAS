using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.Master
{
    public class FeedbackCategoryViewModel
    {
        [UIHint("hidden")]
        public string EncryptedFeedId { get; set; }      

        public int MAST_FEED_ID { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Please enter name.")]
        [RegularExpression(@"^([a-zA-Z, ]+)$", ErrorMessage = "Only Alpabets are allowed.")]
        [StringLength(255, ErrorMessage = "Name must be less than 255 characters")]
        public string MAST_FEED_NAME { get; set; }
    
        public virtual ICollection<ADMIN_FEEDBACK> ADMIN_FEEDBACK { get; set; }
    }
}