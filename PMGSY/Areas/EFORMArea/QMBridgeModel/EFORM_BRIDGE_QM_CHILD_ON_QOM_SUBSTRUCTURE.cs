using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_ON_QOM_SUBSTRUCTURE
    {

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)-Quality of material table: Please enter Work Item Location of row ")]
        [StringLength(20, ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)-Quality of material table: The length must be 20 character or less for work Item Location  of row ")]     
        public string WORK_ITEM_LOC_15_1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)-Quality of material table: Please enter Concrete Grade  of row ")]
        [StringLength(20, ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)-Quality of material table: The length must be 20 character or less for Concrete Grade  of row")]   
        public string CONCRETE_GRADE_15_1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)-Quality of material table: Please enter Quality of material is adequate as per quality control register-Cement  of row ")]
        [StringLength(20, ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)-Quality of material table: The length must be 20 character or less for Quality of material is adequate as per quality control register-Cement  of row ")]     
        public string QM_CEMENT_QCR_15_1 { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)-Quality of material table: Please enter Quality of material is adequate as per quality control register-Coarse Aggregate  of row ")]
        [StringLength(20, ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)-Quality of material table: The length must be 20 character or less for Quality of material is adequate as per quality control register-Coarse Aggregate  of row ")]

        // [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,2})$|^\d{0,7}$", ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A). Please Enter Valid number{cant be greater than 9999.99} in Coarse Aggregate ")]      
        public string QM_COARSE_AGGR_QCR_15_1 { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)-Quality of material table: Please enter Quality of material is adequate as per quality control register-Fine Aggregate of row ")]
        [StringLength(20, ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)-Quality of material table: The length must be 20 character or less for Quality of material is adequate as per quality control register-Fine Aggregate  of row ")]

        // [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,2})$|^\d{0,7}$", ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A). Please Enter Valid number{cant be greater than 9999.99} in Fine Aggregate ")]     
        public string QM_FINE_AGGR_QCR_15_1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)-Quality of material table: Please enter Quality of material is adequate as per quality control register-Steel  of row ")]
        [StringLength(20, ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)-Quality of material table: The length must be 20 character or less for Quality of material is adequate as per quality control register-Steel of row ")]        
        public string QM_STEEL_QCR_15_1 { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
       // [Required(ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)-Quality of material table: Please enter Testing of Concrete Cube by NQM of row ")]
        [StringLength(20, ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)-Quality of material table: The length must be 20 character or less for Testing of Concrete Cube by NQM of row ")]      
        public string CONCRETE_TEST_NQM_15_1 { get; set; }



        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)-Quality of material table: Please select Grade(S/U)  of row ")]
        [StringLength(3, ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)-Quality of material table: The length must be 3 character or less for Grade(S/U) of row ")]
        public string GRADE_15_1 { get; set; }





        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string SubstructureType { get; set; }

    }
}