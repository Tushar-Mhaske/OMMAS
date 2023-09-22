#region File Header
/*
        * Project Name  :   OMMAS II
        * Name          :   ProposalUpdateViewModel.cs
        * Description   :   This View Model is Used for updating the proposals detao;s like batch and year
        * Author        :   Vikram Nandanwar        
        * Creation Date :   16/July/2014
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class ProposalUpdateViewModel
    {
        public ProposalUpdateViewModel()
        {
            lstBatchs = new List<SelectListItem>();
            lstYears = new List<SelectListItem>();
        }

        public string EncryptedProposalCode { get; set; }

        [Required]
        [Range(1,1000,ErrorMessage="Please select Batch.")]
        public int Batch { get; set; }

        [Required]
        [Range(2000, 2100, ErrorMessage = "Please select Year.")]
        public int Year { get; set; }

        public List<SelectListItem> lstBatchs { get; set; }

        public List<SelectListItem> lstYears { get; set; }
    }
}