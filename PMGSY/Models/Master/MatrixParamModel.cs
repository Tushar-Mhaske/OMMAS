using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Master
{
    public class MatrixParamModel
    {
        public int matrixId { get; set; }

        [StringLength(5,ErrorMessage="Matrix number should be less than 5 characters")]
        [Required(ErrorMessage="Please enter Parameter Code")]
        public string matrixNo { get; set; }

          [Required(ErrorMessage = "Please enter Parameter")]
        [StringLength(100,ErrorMessage="Parameter should be less than 100 characters")]
        public string parameterValue { get; set; }
  
        public int ParentId { get; set; }
        [Required(ErrorMessage = "Please enter Weight")]
        [Range(1,int.MaxValue,ErrorMessage="Please enter valid weight")]
        public int Weight { get; set; }

    }
}