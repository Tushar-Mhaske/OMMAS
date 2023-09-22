using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;


namespace PMGSY.Models.Execution
{
    public class ProposalDetailsForLocationTracking
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ReportServerUrl { get; set; }
        public string FolderNameAndReportName { get; set; }
        public object ReportParameter { get; set; }
        public string QueryString { get; set; }

        public byte[] FileBytes { get; set; }

        [Display(Name = "Total Maintenance Cost (Lacs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Total Maintenance Cost(Range should be 0-99999)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total Maintenance Cost, can only contains Numeric values and 4 digits after decimal place")]
        public decimal? IMSSanctionedTotalMaintenanceCost { get; set; }

        public string ProposalType { get; set; }

        [Display(Name = "Total Cost (Lacs)")]
        [Range(1, 99999, ErrorMessage = " Total Cost range should be between 1 to 99999)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total Cost , can only contains Numeric values and 4 digits after decimal place")]
        public decimal? IMSSanctionedTotalCost { get; set; }


        public int ProposalCode { get; set; }

        [Display(Name = "Block")]
        public string Block { get; set; }

        [Display(Name = "Work Name")]
        public string ProposalName { get; set; }

        [Display(Name = "Package")]
        public string Package { get; set; }

        [Display(Name = "Financial Year")]
        public string FinancialYear { get; set; }

        [Display(Name = "Batch")]
        public string Batch { get; set; }

        [Display(Name = "Work Length")]
        public decimal? Length { get; set; }

        [Display(Name = "Total Cost (Rs. in Lacs)")]
        public decimal? TotalCost { get; set; }

        [Display(Name = "Maintenance Cost (Rs. in Lacs)")]
        public decimal? MaintenanceCost { get; set; }

        [Display(Name = "Images Uploaded")]
        public int ImagesUploaded { get; set; }

        [Display(Name = "Construction Completion Date")]
        public string ConstructionCompletionDate { get; set; }

        [Display(Name = "Agreement Start Date")]
        public string AgreementStartDate { get; set; }

        [Display(Name = "Sanction Date")]
        public string SanctionDate { get; set; }

        public string Operation { get; set; }
        
    }
}