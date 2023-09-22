using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Menu
{
    
        public class RoleActionMappingDTO
        {
            public Int32 MenuId { get; set; }
            public Int16 RoleId { get; set; }
            public Boolean AddChildren { get; set; }
        }

        public class RoleActionListDTO
        {
            public Int32 MenuId { get; set; }
            public String Description { get; set; }
            public String Active { get; set; }
            public Int32 ParentId { get; set; }
            public Boolean IsLeaf { get; set; }
        }


        public class MenuLevelCombinationDTO
        {
            public Int32 MenuId { get; set; }
            public String MenuName { get; set; }
            public Int32 Level { get; set; }
            public Int32 ParentId { get; set; }
            public String LevelCombination { get; set; }
            public Int16 Admin { get; set; }
            public Int16 Ministry { get; set; }
            public Int16 NRRDA { get; set; }
            public Int16 National { get; set; }
            public Int16 State { get; set; }
            public Int16 District { get; set; }
            public Int16 MoRD{ get; set; }
           
        }

        public class MenuLevelCombinationListDTO
        {
            List<MenuLevelCombinationDTO> lstLevelComb { get; set; }
            public Int32 MenuId { get; set; }
            public String MenuName { get; set; }
            public Int32 Level { get; set; }
            public Int32 ParentId { get; set; }
            public string LevelStr { get; set; }
        }


        public class MenuDTO
        {
            public int IntMenuId { get; set; }
            public string StrMenuCombinationCode { get; set; }
            public int IntMenuParentId { get; set; }
            public int IntLevelId { get; set; }
            public int IntRoleId { get; set; }
            public int IntMenuSeqNo { get; set; }
            public bool BoolStatus { get; set; }
            public int IntMenuLevel { get; set; }
            public string StrMenuName { get; set; }
            public string StrController { get; set; }
            public string StrAction { get; set; }
            public int transactionid { get; set; }
            public int IntPayableId { get; set; }
            public string ModuleName { get; set; }
        }


        public class Menu_Master
        {
            [ScaffoldColumn(false)]
            public int MenuID { get; set; }

            [Required]
            [Display(Name = "Menu Name")]
            [RegularExpression(@"[A-Za-z0-9_ ]{2,100}", ErrorMessage = "Invalid Menu Name, Minimum Length 3 Characters,Can be AlphaNumeric,Can Contain ( _ ) ")]
            public string MenuName { get; set; }

            [Display(Name = "Parent Menu")]
            public int ParentID { get; set; }

            [Required]
            [Display(Name = "Level Group")]
            public string LevelGroupCode { get; set; }

            [Required]
            [Display(Name = "Sequence")]
            public short Sequence { get; set; }

            [Display(Name = "Horizontal Sequence")]
            public short HorizontalSequence { get; set; }

            [Required]
            [Display(Name = "Vertical Position")]
            public string VerticalLevel { get; set; }


            public bool IsActive { get; set; }

            [Required]
            [Display(Name = "Combination Of Menus")]
            public string MenucombinationCode { get; set; }

            public int MenuCombinationLevelList1 { get; set; }
            public int MenuCombinationLevelList2 { get; set; }
            public int MenuCombinationLevelList3 { get; set; }

            public List<SelectListItem> MenuList { get; set; }
            public List<SelectListItem> MenuCombinationList1 { get; set; }
            public List<SelectListItem> MenuCombinationList2 { get; set; }
            public List<SelectListItem> MenuCombinationList3 { get; set; }
        }


        public class Menu_Rights
        {
            public int UserID { get; set; }
            public int RoleID { get; set; }
            public int MenuID { get; set; }
            public string MenuName { get; set; }
            public bool IsAdd { get; set; }
            public bool IsEdit { get; set; }
            public bool IsDelete { get; set; }
            public string Save { get; set; }
        }


        public class UserRoleMapping
        {
            [Required]
            [Display(Name = "User Name")]
            public int UserID { get; set; }

            [Required]
            [Display(Name = "Role")]
            public string RoleID { get; set; }
            public string RoleStr { get; set; }
            //public List<SelectListItem> RoleList { get; set; }
        }

}