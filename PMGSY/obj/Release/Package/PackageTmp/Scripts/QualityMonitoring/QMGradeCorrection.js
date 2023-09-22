/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMGradeCorrection.js
        * Description   :   Handles events, grids in GradeCorrection process
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/
$(document).ready(function () {

   
    $.validator.unobtrusive.parse($('#frmGradeCorrection'));

     $("#btnCancel").click(function () {
         CloseInspectionDetails();
     });


     $("#btnSave").click(function () {
         
         if (Checkgrade()) {
             if (confirm("Are you sure to submit details?")) {
                 $.validator.unobtrusive.parse($('#frmGradeCorrection'));

                 //alert($("#ADMIN_SCHEDULE_CODE").val());
                 $.ajax({
                     url: '/QualityMonitoring/QMGradingCorrection/',
                     type: "POST",
                     cache: false,
                     data: $("#frmGradeCorrection").serialize(),
                     beforeSend: function () {
                         blockPage();
                     },
                     error: function (xhr, status, error) {
                         unblockPage();
                         alert("Request can not be processed at this time,please try after some time!!!");
                         return false;
                     },
                     success: function (response) {

                         if (response.Success) {
                             alert("Observations saved successfully.");
                             $("#tb3TierInspectionList").trigger('reloadGrid');
                             CloseInspectionDetails();
                         }
                         else {
                             $("#divFillObservationsError").show("slow");
                             $("#divFillObservationsError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                         }
                         unblockPage();
                     }
                 });//ajax call ends here

             }//confirm ends here
             else {
                 return false;
             }
         }//Check Grade ends here

     });


});//doc.ready ends here


function closeDivError() {
    $('#divGradeError').hide('slow');
    $('#divFillObservationsError').hide('slow');
}


///-------------------------- NQM Grading  -----------------------------

function ItemGrade(NoOfItem, currentItem, NoOfSubItem) {
    /*
    Condition for Generating Item Grade.
    *the smallest value of sub item will grade of item.
    */
    var SubItemID;
    var the_value;
    var Small_Value = 0;
   
    for (var i = 1; i < NoOfSubItem; i++) {
        SubItemID = '[name="' + 'subitem' + currentItem + i + '"]:radio:checked';

        the_value = jQuery(SubItemID).val();

        if (parseInt(Small_Value) < parseInt(the_value) && parseInt(the_value) != 4) {
            Small_Value = the_value;
        }
    }
    if (Small_Value == 0)
    { Small_Value = 4; }
        
    var ItemID = 'input:radio[name=' + 'item' + currentItem + ']';
    
    var $radios = $(ItemID);
    var filterValue = '[value=' + Small_Value + ']';


    //$radios.filter(4).removeAttr("checked");
    $radios.filter(filterValue).attr('checked', true);
    
    var hiddenItemID = "#hiddenitem" + currentItem;
    $(hiddenItemID).val(parseInt(Small_Value));

    //alert(hiddenItemID + "  :  " + $(hiddenItemID).val() + "  " + $("#hiddenitem" + currentItem).val());
    OverAllGrade(NoOfItem);
}


function OverAllGrade(NoOfItem) {

    /*
    Condition for Generating OverAllItem Grade.
    1. If all item grade NA, Then Overall Grade NA.
    2. If All Item Grade is S, Then Overall Grade S.
    3. If any one Item 4,5,6,7 is U then Overall Grade U.
    4. Other wise RI.
        
    */
    var OverallItemID = 'input:radio[name=item' + NoOfItem + ']';
    var HiddenOverallItemID = '#hiddenitem' + NoOfItem;
    var flag = false;
    var FinalGrade = 4;
    //alert(NoOfItem);
    //alert(OverallItemID);
    if (IsNotApplicable(NoOfItem)) {
        FinalGrade = 4
        flag = true;
    }
    if (!flag) {
        if (IsSatisfactory(NoOfItem)) {
            FinalGrade = 1;
            flag = true;
        }
    }
    if (!flag) {
        if (IsUnSatisfactory()) {
            FinalGrade = 3;
            flag = true;
        }
    }

    if (!flag) {
        FinalGrade = 2;
    }


    var $radios = $(OverallItemID);
    var filterValue = '[value=' + FinalGrade + ']';

    //$radios.filter(4).removeAttr("checked");
    $radios.filter(filterValue).attr('checked', true);

    $(HiddenOverallItemID).val(FinalGrade);



    //alert(FinalGrade);
}

function IsNotApplicable(NoOfItem) {
    var itemId;

    for (var i = 1; i < NoOfItem; i++) {
        itemId = "item" + i;
        if ($('#' + itemId).length) {
            ItemID = '[name="' + itemId + '"]:radio:checked';
            the_value = jQuery(ItemID).val();
            if (the_value != 4)
                return false;
        }

    }
    return true;
}
function IsSatisfactory(NoOfItem) {
    var itemId;
    for (var i = 1; i < NoOfItem; i++) {
        itemId = "item" + i;
        if ($('#' + itemId).length) {
            ItemID = '[name="' + 'item' + i + '"]:radio:checked';

            the_value = jQuery(ItemID).val();
            if (!(the_value == 1 || the_value == 4))
                return false;
        }
    }
    return true;
}
function IsUnSatisfactory() {

    var itemId;
    var arrItem = ["4", "5", "6", "7"]; // If any one among these item value u then overall U.
    for (var i = 0; i < arrItem.length; i++) {
        itemId = "item" + arrItem[i];
        if ($('#' + itemId).length) {
            ItemID = '[name="' + 'item' + arrItem[i] + '"]:radio:checked';
            the_value = jQuery(ItemID).val();
            if (the_value == 3)
                return true;
        }
    }
    return false;
}

function Checkgrade() {

    var ItemID;
    var the_value;
    var Small_Value = 1;
    if ($("#IMS_PROPOSAL_TYPE").val() === "P") {
        //if ($("#hdnRole").val() == 5 && ($("#IMS_ISCOMPLETED").val() == "C" || $("#IMS_ISCOMPLETED").val() == "P"))                   //CQCAdmin
        if ($("#hdnRole").val() == 9 && ($("#IMS_ISCOMPLETED").val() == "C" || $("#IMS_ISCOMPLETED").val() == "P"))                   //CQC
        {
            ItemID = '[name="item21"]:radio:checked';
            the_value = jQuery(ItemID).val();
            //alert(the_value);
            if (parseInt(the_value) != 4) {
                $("#trGradeError").hide("slow");
                $("#divGradeError").hide("slow");
                $("#divGradeError span:eq(1)").html("");
                return true;
            }

            $("#trGradeError").show("slow");
            $("#divGradeError").show("slow");
            $("#divGradeError span:eq(1)").html("<strong>Alert: </strong> Please select atleast one item!");
            return false;

        } else if ($("#hdnRole").val() == 8 && ($("#IMS_ISCOMPLETED").val() == "C" || $("#IMS_ISCOMPLETED").val() == "P" || $("#IMS_ISCOMPLETED").val() == "M"))        //SQC
        {
            if ($("input[name=optItem]:checked").val() == 4) {
                $("#trGradeError").show("slow");
                $("#divGradeError").show("slow");
                $("#divGradeError span:eq(1)").html("<strong>Alert: </strong> Please select atleast one item!");
                return false;
            }
            else {
                return true;
            }
        }
        //else if ($("#hdnRole").val() == 5 && $("#IMS_ISCOMPLETED").val() == "M")        //Maintenance Road, In future Rolewise Items may change(for NQM/SQM), so for that case, compare RoleCodes as compared above
        else if ($("#hdnRole").val() == 9 && $("#IMS_ISCOMPLETED").val() == "M")        //Maintenance Road, In future Rolewise Items may change(for NQM/SQM), so for that case, compare RoleCodes as compared above
        {
            //alert($("input[name=optItem]:checked").val());
            if ($("input[name=optItemMaintenance]:checked").val() == 4) {
                $("#trGradeError").show("slow");
                $("#divGradeError").show("slow");
                $("#divGradeError span:eq(1)").html("<strong>Alert: </strong> Please select atleast one item!");
                return false;
            }
            else {
                return true;
            }
        }
    }
    else if ($("#IMS_PROPOSAL_TYPE").val() === "L")     //Bridge
    {
        //alert($("input[name=optItem8]:checked").val());
        if ($("input[name=optItem8]:checked").val() == 4) {
            $("#trGradeError").show("slow");
            $("#divGradeError").show("slow");
            $("#divGradeError span:eq(1)").html("<strong>Alert: </strong> Please select atleast one item!");
            return false;
        }
        else {
            return true;
        }
    }
    


}

///--------------------------NQM Grading Ends Here -----------------------------

////--------------------------SQM Grading Calculations --------------------------

function fnGradeValidate() {

    /*-----
            If All NA then NA
            Else IF Any one Of 5,6,7,8,9 is U then U
            Else IF All of 5,6,7,8,9 are S and any other is SRI or U then Overall is SRI
            Else S
       ----*/


    if ($("#IMS_ISCOMPLETED").val() == "P") {

        if ($("input[name=optItem1]:checked").val() == 4 && $("input[name=optItem2]:checked").val() == 4 && $("input[name=optItem3]:checked").val() == 4 && $("input[name=optItem4]:checked").val() == 4 && $("input[name=optItem5]:checked").val() == 4 && $("input[name=optItem6]:checked").val() == 4 && $("input[name=optItem7]:checked").val() == 4 && $("input[name=optItem8]:checked").val() == 4 && $("input[name=optItem9]:checked").val() == 4 && $("input[name=optItem10]:checked").val() == 4 && $("input[name=optItem11]:checked").val() == 4 && $("input[name=optItem12]:checked").val() == 4 && $("input[name=optItem13]:checked").val() == 4 && $("input[name=optItem14]:checked").val() == 4) {
            optItem = "NA";

        }
        else if ($("input[name=optItem5]:checked").val() == 3 || $("input[name=optItem6]:checked").val() == 3 || $("input[name=optItem7]:checked").val() == 3 || $("input[name=optItem8]:checked").val() == 3 || $("input[name=optItem9]:checked").val() == 3) {
            optItem = "U";

        }
        else if ($("input[name=optItem1]:checked").val() == 3 || $("input[name=optItem2]:checked").val() == 3 || $("input[name=optItem3]:checked").val() == 3 || $("input[name=optItem4]:checked").val() == 3 || $("input[name=optItem10]:checked").val() == 3 || $("input[name=optItem11]:checked").val() == 3 || $("input[name=optItem12]:checked").val() == 3) {
            optItem = "SRI";

        }
        else if ($("input[name=optItem1]:checked").val() == 2 || $("input[name=optItem2]:checked").val() == 2 || $("input[name=optItem3]:checked").val() == 2 || $("input[name=optItem4]:checked").val() == 2 || $("input[name=optItem10]:checked").val() == 2 || $("input[name=optItem11]:checked").val() == 2 || $("input[name=optItem12]:checked").val() == 2 || $("input[name=optItem13]:checked").val() == 2 || $("input[name=optItem14]:checked").val() == 2) {
            optItem = "SRI";

        }
        else {
            optItem = "S";
        }
    }
    else if ($("#IMS_ISCOMPLETED").val() == "C") {


        if ($("input[name=optItem4]:checked").val() == 4 && $("input[name=optItem5]:checked").val() == 4 && $("input[name=optItem6]:checked").val() == 4 && $("input[name=optItem7]:checked").val() == 4 && $("input[name=optItem8]:checked").val() == 4 && $("input[name=optItem9]:checked").val() == 4 && $("input[name=optItem10]:checked").val() == 4 && $("input[name=optItem11]:checked").val() == 4 && $("input[name=optItem12]:checked").val() == 4 && $("input[name=optItem13]:checked").val() == 4 && $("input[name=optItem14]:checked").val() == 4) {
            optItem = "NA";
        }
        else if ($("input[name=optItem5]:checked").val() == 3 || $("input[name=optItem6]:checked").val() == 3 || $("input[name=optItem7]:checked").val() == 3 || $("input[name=optItem8]:checked").val() == 3 || $("input[name=optItem9]:checked").val() == 3) {
            optItem = "U";

        }
        else if ($("input[name=optItem4]:checked").val() == 3 || $("input[name=optItem10]:checked").val() == 3 || $("input[name=optItem11]:checked").val() == 3 || $("input[name=optItem12]:checked").val() == 3) {
            optItem = "SRI";

        }
        else if ($("input[name=optItem4]:checked").val() == 2 || $("input[name=optItem10]:checked").val() == 2 || $("input[name=optItem11]:checked").val() == 2 || $("input[name=optItem12]:checked").val() == 2 || $("input[name=optItem13]:checked").val() == 3 || $("input[name=optItem14]:checked").val() == 3) {
            optItem = "SRI";

        }
        else {
            optItem = "S";
        }

    }

    //alert($("input[name=optItem]:checked").val());
    //$("#overAllGrade").val($("input[name=optItem]:checked").val());
    //$("input[name=optItem4]:checked").val()
    if (optItem == "S") {
        $("input[name=optItem][value=" + 1 + "]").attr('checked', 'checked');
        $("#overAllGrade").val(1);
    }
    else if (optItem == "SRI") {
        $("input[name=optItem][value=" + 2 + "]").attr('checked', 'checked');
        $("#overAllGrade").val(2);
    }
    else if (optItem == "U") {
        $("input[name=optItem][value=" + 3 + "]").attr('checked', 'checked');
        $("#overAllGrade").val(3);
    }
    else if (optItem == "NA") {
        $("input[name=optItem][value=" + 4 + "]").attr('checked', 'checked');
        $("#overAllGrade").val(4);
    }

}


/////----------------------------------------------------------------------------


//----------------------------NQM/SQM/CQC Grading Calculations for Maintenance Roads Starts Here -----------------------------------
function fnGradeValidateForMaintenanceRoad() {

    /*-----
            If All NA then NA
            Else IF Any one Of 2,3 is U then U
            Else IF All of 2,3 are S and any other is U then Overall is SRI
            Else S
       ----*/
    
    
    if ($("#IMS_ISCOMPLETED").val() == "M") {

        
        

        if ($("input[name=optItemMaintenance1]:checked").val() == 4 && $("input[name=optItemMaintenance2]:checked").val() == 4 && $("input[name=optItemMaintenance3]:checked").val() == 4 && $("input[name=optItemMaintenance4]:checked").val() == 4 && $("input[name=optItemMaintenance5]:checked").val() == 4 && $("input[name=optItemMaintenance6]:checked").val() == 4) {
            //alert($("input[name=optItemMaintenance2]:checked").val());
            optItemMaintenance = "NA";

        }
        else if ($("input[name=optItemMaintenance2]:checked").val() == 3 || $("input[name=optItemMaintenance3]:checked").val() == 3) {
            optItemMaintenance = "U";

        }
        else if ($("input[name=optItemMaintenance1]:checked").val() == 3 || $("input[name=optItemMaintenance4]:checked").val() == 3 || $("input[name=optItemMaintenance5]:checked").val() == 3 || $("input[name=optItemMaintenance6]:checked").val() == 3) {
            optItemMaintenance = "SRI";

        }
        else {
            optItemMaintenance = "S";
        }
    }

    //$("#overAllGradeMaintenance").val(optItemMaintenance);
    //alert(optItemMaintenance);
    if (optItemMaintenance == "S") {
        $("input[name=optItemMaintenance][value=" + 1 + "]").attr('checked', 'checked');
        $("#overAllGradeMaintenance").val(1);
    }
    else if (optItemMaintenance == "SRI") {
        $("input[name=optItemMaintenance][value=" + 2 + "]").attr('checked', 'checked');
        $("#overAllGradeMaintenance").val(2);
    }
    else if (optItemMaintenance == "U") {
        $("input[name=optItemMaintenance][value=" + 3 + "]").attr('checked', 'checked');
        $("#overAllGradeMaintenance").val(3);
    }
    else if (optItemMaintenance == "NA") {
        $("input[name=optItemMaintenance][value=" + 4 + "]").attr('checked', 'checked');
        $("#overAllGradeMaintenance").val(4);
    }

}


//----------------------------NQM/SQM/CQC Grading Calculations for Maintenance Roads Ends Here -----------------------------------



//----------------------------NQM/SQM/CQC Grading Calculations for Long Span Bridges Starts Here -----------------------------------
function fnGradeValidateForLSB() {

    /*-----
            If All NA then NA
            Else IF Any one Of 2,3,4 is U then U
            Else IF All of 2,3,4 are S and any other is U then Overall is SRI
            Else If All are S then Overall is S
            Else SRI
       ----*/
    //alert($("input[name=optItem2]:checked").val());
    //--- Progress Road
    if ($("#IMS_ISCOMPLETED").val() == "C") {

        if ($("input[name=optItem2]:checked").val() == 4 && $("input[name=optItem3]:checked").val() == 4 && $("input[name=optItem4]:checked").val() == 4 && $("input[name=optItem5]:checked").val() == 4 && $("input[name=optItem6]:checked").val() == 4 && $("input[name=optItem7]:checked").val() == 4) {
            optItem = "NA";

        }
        else if ($("input[name=optItem2]:checked").val() == 3 || $("input[name=optItem3]:checked").val() == 3 || $("input[name=optItem4]:checked").val() == 3) {
            optItem = "U";

        }
        else if (($("input[name=optItem2]:checked").val() == 1 && $("input[name=optItem3]:checked").val() == 1 && $("input[name=optItem4]:checked").val() == 1) && ($("input[name=optItem5]:checked").val() == 3 || $("input[name=optItem6]:checked").val() == 3 || $("input[name=optItem7]:checked").val() == 3)) {
            optItem = "SRI";

        }
        else if (($("input[name=optItem2]:checked").val() == 1 && $("input[name=optItem3]:checked").val() == 1 && $("input[name=optItem4]:checked").val() == 1) && ($("input[name=optItem5]:checked").val() == 2 || $("input[name=optItem6]:checked").val() == 2 || $("input[name=optItem7]:checked").val() == 2)) {
            optItem = "SRI";

        }
        else if (($("input[name=optItem2]:checked").val() == 2 || $("input[name=optItem3]:checked").val() == 2 || $("input[name=optItem4]:checked").val() == 2) || $("input[name=optItem5]:checked").val() == 2 || $("input[name=optItem6]:checked").val() == 2 || $("input[name=optItem7]:checked").val() == 2) {
            optItem = "SRI";

        }
        else if ($("input[name=optItem2]:checked").val() == 1 && $("input[name=optItem3]:checked").val() == 1 && $("input[name=optItem4]:checked").val() == 1 && $("input[name=optItem5]:checked").val() == 1 && $("input[name=optItem6]:checked").val() == 1 && $("input[name=optItem7]:checked").val() == 1) {
            optItem = "S";

        }
        else
        {
            optItem = "S";
        }
    }
    else {

        if ($("input[name=optItem1]:checked").val() == 4 && $("input[name=optItem2]:checked").val() == 4 && $("input[name=optItem3]:checked").val() == 4 && $("input[name=optItem4]:checked").val() == 4 && $("input[name=optItem5]:checked").val() == 4 && $("input[name=optItem6]:checked").val() == 4 && $("input[name=optItem7]:checked").val() == 4) {
            optItem = "NA";

        }
        else if ($("input[name=optItem2]:checked").val() == 3 || $("input[name=optItem3]:checked").val() == 3 || $("input[name=optItem4]:checked").val() == 3) {
            optItem = "U";

        }
        else if (($("input[name=optItem2]:checked").val() == 1 && $("input[name=optItem3]:checked").val() == 1 && $("input[name=optItem4]:checked").val() == 1) && ($("input[name=optItem1]:checked").val() == 3 || $("input[name=optItem5]:checked").val() == 3 || $("input[name=optItem6]:checked").val() == 3 || $("input[name=optItem7]:checked").val() == 3)) {
            optItem = "SRI";

        }
        else if (($("input[name=optItem2]:checked").val() == 1 && $("input[name=optItem3]:checked").val() == 1 && $("input[name=optItem4]:checked").val() == 1) && ($("input[name=optItem1]:checked").val() == 2 || $("input[name=optItem5]:checked").val() == 2 || $("input[name=optItem6]:checked").val() == 2 || $("input[name=optItem7]:checked").val() == 2)) {
            optItem = "SRI";

        }
        else if (($("input[name=optItem1]:checked").val() == 2 || $("input[name=optItem2]:checked").val() == 2 || $("input[name=optItem3]:checked").val() == 2 || $("input[name=optItem4]:checked").val() == 2) || $("input[name=optItem5]:checked").val() == 2 || $("input[name=optItem6]:checked").val() == 2 || $("input[name=optItem7]:checked").val() == 2) {
            optItem = "SRI";

        }
        else if ($("input[name=optItem1]:checked").val() == 1 && $("input[name=optItem2]:checked").val() == 1 && $("input[name=optItem3]:checked").val() == 1 && $("input[name=optItem4]:checked").val() == 1 && $("input[name=optItem5]:checked").val() == 1 && $("input[name=optItem6]:checked").val() == 1 && $("input[name=optItem7]:checked").val() == 1) {
            optItem = "S";

        }
        //else if (($("input[name=optItem1]:checked").val() == 1  || $("input[name=optItem2]:checked").val() == 1 || $("input[name=optItem3]:checked").val() == 1 || $("input[name=optItem4]:checked").val() == 1 || $("input[name=optItem5]:checked").val() == 1 || $("input[name=optItem6]:checked").val() == 1 || $("input[name=optItem7]:checked").val() == 1)
        //      && ($("input[name=optItem1]:checked").val() == 4  || $("input[name=optItem2]:checked").val() == 4 || $("input[name=optItem3]:checked").val() == 4 || $("input[name=optItem4]:checked").val() == 4 || $("input[name=optItem5]:checked").val() == 4 || $("input[name=optItem6]:checked").val() == 4 || $("input[name=optItem7]:checked").val() == 4)) 
        //{
        //    optItem = "S";
        //}
        else
        {
            optItem = "S";
        }
    }

    //alert(optItem);
    console.log(optItem);
    if (optItem == "S") {
        $("input[name=optItem8][value=" + 1 + "]").attr('checked', 'checked');
        $("#hiddenitem8").val(1);
    }
    else if (optItem == "SRI") {
        $("input[name=optItem8][value=" + 2 + "]").attr('checked', 'checked');
        $("#hiddenitem8").val(2);
        
    }
    else if (optItem == "U") {
        $("input[name=optItem8][value=" + 3 + "]").attr('checked', 'checked');
        $("#hiddenitem8").val(3);
    }
    else if (optItem == "NA") {
        $("input[name=optItem8][value=" + 4 + "]").attr('checked', 'checked');
        $("#hiddenitem8").val(4);
    }
    
}

function ItemGradeForLSB(NoOfItem, currentItem, NoOfSubItem) {
    /*
    Condition for Generating Item Grade.
    *the smallest value of sub item will grade of item.
    */
    var SubItemID;
    var the_value;
    var Small_Value = 0;

    for (var i = 1; i < NoOfSubItem; i++) {
        SubItemID = '[name="' + 'subitem' + currentItem + i + '"]:radio:checked';

        the_value = jQuery(SubItemID).val();

        if (parseInt(Small_Value) < parseInt(the_value) && parseInt(the_value) != 4) {
            Small_Value = the_value;
        }
    }
   
    if (Small_Value == 0)
        Small_Value = 4;

    var ItemID = 'input:radio[name=' + 'optItem' + currentItem + ']';
    var $radios = $(ItemID);
    var filterValue = '[value=' + Small_Value + ']';
    $radios.filter(filterValue).attr('checked', true);

    var hiddenItemID = "#hiddenitem" + currentItem;
    $(hiddenItemID).val(parseInt(Small_Value));

    OverAllGradeForLSB(NoOfItem);
}


function OverAllGradeForLSB(NoOfItem) {

    /*-----
            If All NA then NA
            Else IF Any one Of 2,3,4 is U then U
            Else IF All of 2,3,4 are S and any other is U then Overall is SRI
            Else If All are S then Overall is S
            Else S
   ----*/

    var OverallItemID = 'input:radio[name=optItem' + NoOfItem + ']';
    var HiddenOverallItemID = '#hiddenitem' + NoOfItem;

    var flag = false;
    var FinalGrade = 4;
    if (IsNotApplicableForLSB(NoOfItem)) {
        FinalGrade = 4
        flag = true;
    }
    if (!flag) {
        if (IsSatisfactoryForLSB(NoOfItem)) {
            FinalGrade = 1;
            flag = true;
        }
    }
    if (!flag) {
        if (IsUnSatisfactoryForLSB()) {
            FinalGrade = 3;
            flag = true;
        }
    }

    if (!flag) {
        FinalGrade = 2;
        console.log("In 2");
    }

    var $radios = $(OverallItemID);
    var filterValue = '[value=' + FinalGrade + ']';
    $radios.filter(filterValue).attr('checked', true);
    $(HiddenOverallItemID).val(FinalGrade);

    $("#LSBOverallCorrectedGrade").val(FinalGrade);
   // alert("Final" + $("#LSBOverallCorrectedGrade").val())

}


function IsNotApplicableForLSB(NoOfItem) {
    var itemId;

    for (var i = 1; i < NoOfItem; i++) {
        itemId = "optItem" + i;
        if ($('#' + itemId).length) {
            ItemID = '[name="' + itemId + '"]:radio:checked';
            the_value = jQuery(ItemID).val();
            if (the_value != 4)
                return false;
        }
    }
    return true;
}
function IsSatisfactoryForLSB(NoOfItem) {
    var itemId;
    for (var i = 1; i < NoOfItem; i++) {
        itemId = "optItem" + i;
        if ($('#' + itemId).length) {
            ItemID = '[name="' + 'optItem' + i + '"]:radio:checked';

            the_value = jQuery(ItemID).val();
            if (!(the_value == 1 || the_value == 4))
                return false;
        }
    }
    return true;
}
function IsUnSatisfactoryForLSB() {
    var itemId;
    var arrItem = ["2", "3", "4"]; // If any one among these item value u then overall U.
    for (var i = 0; i < arrItem.length; i++) {
        itemId = "optItem" + arrItem[i];
        if ($('#' + itemId).length) {
            ItemID = '[name="' + 'optItem' + arrItem[i] + '"]:radio:checked';
            the_value = jQuery(ItemID).val();
            if (the_value == 3)
                return true;
        }
    }
    return false;
}
//----------------------------NQM/SQM/CQC Grading Calculations for Long Span Bridges Roads Ends Here -----------------------------------