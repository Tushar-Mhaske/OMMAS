/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMFillObservations.js
        * Description   :   Handles events, grids in  FillObservations process
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/
$.validator.unobtrusive.adapters.add('isvalidstartchainage', ['pavlength'], function (options) {
    options.rules['isvalidstartchainage'] = options.params;
    options.messages['isvalidstartchainage'] = options.message;
});

$.validator.addMethod("isvalidstartchainage", function (value, element, params) {

    var rdLength = parseFloat($("#IMS_PAV_LENGTH").val());
    var endChainage = parseFloat($("#TO_ROAD_LENGTH").val());
    var diff = endChainage - parseFloat(value);

    if ((parseFloat(value) >= 0) && (parseFloat(value) < rdLength) && (parseFloat(value) < endChainage) && (parseFloat(diff, 3) <= parseFloat(3.000))) {
        return true;
    }

    return false;
});


$.validator.unobtrusive.adapters.add('isvalidendchainage', ['pavlength'], function (options) {
    options.rules['isvalidendchainage'] = options.params;
    options.messages['isvalidendchainage'] = options.message;
});

$.validator.addMethod("isvalidendchainage", function (value, element, params) {

    var rdLength = parseFloat($("#IMS_PAV_LENGTH").val());
    var startChainage = parseFloat($("#FROM_ROAD_LENGTH").val());
    var diff = parseFloat(value) - startChainage;
    //alert("startChainage : " + startChainage + " Diff : " + parseFloat(diff, 3));
    if ((parseFloat(value) > 0) && (parseFloat(value) <= rdLength) && (parseFloat(value) > startChainage) && (parseFloat(diff, 3) <= parseFloat(3.000))) {
        return true;
    }
    return false;
});


$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmFillObservations'));

    $("#QM_INSPECTION_DATE").addClass("pmgsy-textbox");

    $("#QM_INSPECTION_DATE").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        showOn: 'button',
        buttonImage: '../../Content/images/calendar_2.png',
        buttonImageOnly: true,
        onClose: function () {
            $(this).focus().blur();
        }
    }).attr('readonly', 'readonly');


    $("#QM_INSPECTION_DATE").datepicker("option", "minDate", $("#SCHEDULE_MONTH_YEAR_START_DATE").val());
    //  $("#QM_INSPECTION_DATE").datepicker("option", "maxDate", $("#CURRENT_DATE").val());
    $("#QM_INSPECTION_DATE").datepicker("option", "maxDate", $("#SCHEDULE_MONTH_YEAR_END_DATE").val());// Added by deendayal on 15/9/2017 to restrict the monitor to select any other month's day


    // Call to function of grading calculations for Maintenance Road
    // On Ready it is called because, grading of 3 rd Item for PCI Index is already calculated from Server Side
    // If need to compare NQM/SQM/CQC Rolewise, in case of change of items(in future), Compare 
    // $("#hdnRole").val() == 6 for NQM, $("#hdnRole").val() == 9 for CQC & $("#hdnRole").val() == 7 for SQM
    //if ($("#IMS_ISCOMPLETED").val() == "M") {

    //    fnGradeValidateForMaintenanceRoad();
    //}

    $("#btnCancel").click(function () {
        closeMonitorsObsDetails();
    });

    $("#btnSave").click(function () {
        //debugger;
        //alert("Pav " + $("#IMS_PAV_LENGTH").val())
        //alert("End  " + $("#TO_ROAD_LENGTH").val())
        // alert("Start " + $("#FROM_ROAD_LENGTH").val())
        //alert("Proposal Type " + $("#IMS_PROPOSAL_TYPE").val())

        if ($("#IMS_PROPOSAL_TYPE").val() === "P") {
            // Basic Validation 1

            var startChainage = parseFloat($("#FROM_ROAD_LENGTH").val()).toFixed(3);
            var endChainage = parseFloat($("#TO_ROAD_LENGTH").val()).toFixed(3);
            if (parseFloat(parseFloat(startChainage).toFixed(3)) > parseFloat(parseFloat(endChainage).toFixed(3))) {

                alert("Start Chainage (Km.) should be less than End Chainage (Km.) ");
                return false;
            }
            //if ($("#FROM_ROAD_LENGTH").val() > $("#TO_ROAD_LENGTH").val())
            //{
            //    alert("Start Chainage (Km.) should be less than End Chainage (Km.) ");
            //    return false;
            //}

            // Basic Validation 2
            var Diff1 = ($("#TO_ROAD_LENGTH").val() - $("#FROM_ROAD_LENGTH").val());

            if (Diff1 > $("#IMS_PAV_LENGTH").val()) {
                alert("Start Chainage (Km.) and End Chainage (Km.) difference can not be greater than Road Length.");
                return false;

            }


            if ($("#IMS_PAV_LENGTH").val() > 3) {// If road length is greater than 3
                var Diff = ($("#TO_ROAD_LENGTH").val() - $("#FROM_ROAD_LENGTH").val());
                if (Diff == 3) {
                }
                else {
                    // alert("Start Chainage (Km.) and End Chainage (Km.) difference should be 3 Km, Because Road length is greater than 3 Km.");
                    // return false;
                }

            }
            else {// If road length is less than 3

                var Diff = ($("#TO_ROAD_LENGTH").val() - $("#FROM_ROAD_LENGTH").val());



                //   alert(Diff);

                if (Diff == $("#IMS_PAV_LENGTH").val()) {
                }
                else { // Commented on 08/ 12/2020 as per suggestion from Pankaj Sir.
                    // alert("Start Chainage (Km.) and End Chainage (Km.) difference should be " + $("#IMS_PAV_LENGTH").val() + " Km, Because Road length is less than 3 Km.");
                    // return false;
                }


            }

        }


        //ap0257618 (sqm) -- ap0257618 
        //nqmabrol(nqm)--
        //fill obsrvation =--> plus sign

        //only for road --  ---> 

        //1 ) if in maintenace-->>enable start and end chainal-- done 

        //2) for any other road --> main , prog , compl --> if road length greater than 3 km thgen diff btn st and end chaing should be exactly 3

        //                                  -- road length is less than 3 then diff should be same road length

        if (Checkgrade()) {
            //alert("here")
            //if ($("#frmFillObservations").valid()) {
            //console.log($("#frmFillObservations"))
            //console.log($("#frmFillObservations").val())
            //alert($("#frmFillObservations").ItemID)
            //alert($("#frmFillObservations").serialize())


            if (confirm("Are you sure to submit details?")) {
                if ($("#IMS_ISCOMPLETED").val() == "M") {
                    $("#FROM_ROAD_LENGTH").val(0);
                    $("#TO_ROAD_LENGTH").val($("#IMS_PAV_LENGTH").val());
                    $("#TO_ROAD_LENGTH").prop("disabled", false);
                }
                $.ajax({
                    url: '/QualityMonitoring/QMFillObservations',
                    type: "POST",
                    cache: false,
                    data: $("#frmFillObservations").serialize(),
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
                            $("#tbMonitorsObsList").trigger('reloadGrid');
                            closeMonitorsObsDetails();
                        }
                        else {
                            $("#divFillObservationsError").show("slow");
                            $("#divFillObservationsError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                        }
                        unblockPage();
                    }
                }

                );//ajax call ends here

            }//confirm ends here
            else {
                return false;
            }
            // }

        } // CheckGrade() ends here
    });

    $(".textArea").blur(function () {
        var pattern = "^[a-zA-Z0-9 ,.()-]+$";

        if (!($(this).val().match(pattern))) {

            $('#spn' + $(this).attr('id')).html("Invalid Remarks, Can only contains AlphaNumeric values and ,.()-");
            $('#spn' + $(this).attr('id')).show("slow");

            //$("#divGradeError span:eq(1)").html("<strong>Alert: </strong>Invalid Remarks, Can only contains AlphaNumeric values and ,.()-");
            //$("#divGradeError").show("slow");
            return false;
        }
        else {
            $('#spn' + $(this).attr('id')).html("");
            $('#spn' + $(this).attr('id')).hide("slow");
        }
    });

});//doc.ready ends here



function closeDivError() {
    $('#divGradeError').hide('slow');
    $('#divFillObservationsError').hide('slow');
}


///-------------------------- NQM Grading Calculations for Completed & In Progress Roads  -----------------------------

//function ItemGrade(NoOfItem, currentItem, NoOfSubItem) {
//    /*
//    Condition for Generating Item Grade.
//    *the smallest value of sub item will grade of item.
//    */
//    var SubItemID;
//    var the_value;
//    var Small_Value = 0;

//    for (var i = 1; i < NoOfSubItem; i++) {
//        SubItemID = '[name="' + 'subitem' + currentItem + i + '"]:radio:checked';

//        the_value = jQuery(SubItemID).val();

//        if (parseInt(Small_Value) < parseInt(the_value) && parseInt(the_value) != 4) {
//            Small_Value = the_value;
//        }

//        // If selected item value is unsatisfactory, show Remarks 
//        // else set value to empty and Hide
//        if (the_value == 3) {
//            $("#remarks" + currentItem + i).show('slow');
//        }
//        else {
//            $("#remarks" + currentItem + i).val('');
//            $("#remarks" + currentItem + i).hide('slow');
//            $('#spnremarks' + currentItem + i).html("");
//            $('#spnremarks' + currentItem + i).hide("slow");
//        }
//    }

//    if (Small_Value == 0)
//        Small_Value = 4;

//    var ItemID = 'input:radio[name=' + 'item' + currentItem + ']';
//    var $radios = $(ItemID);
//    var filterValue = '[value=' + Small_Value + ']';
//    $radios.filter(filterValue).attr('checked', true);

//    var hiddenItemID = "#hiddenitem" + currentItem;
//    $(hiddenItemID).val(parseInt(Small_Value));
//    OverAllGrade(NoOfItem);
//}


//function OverAllGrade(NoOfItem) {

//    /*
//    Condition for Generating OverAllItem Grade.
//    1. If all item grade NA, Then Overall Grade NA.
//    2. If All Item Grade is S, Then Overall Grade S.
//    3. If any one Item 4,5,6,7 is U then Overall Grade U.
//    4. Other wise RI.

//    */
//    var OverallItemID = 'input:radio[name=item' + NoOfItem + ']';
//    var HiddenOverallItemID = '#hiddenitem' + NoOfItem;
//    var flag = false;
//    var FinalGrade = 4;
//    if (IsNotApplicable(NoOfItem)) {
//        FinalGrade = 4
//        flag = true;
//    }
//    if (!flag) {
//        if (IsSatisfactory(NoOfItem)) {
//            FinalGrade = 1;
//            flag = true;
//        }
//    }
//    if (!flag) {
//        if (IsUnSatisfactory()) {
//            FinalGrade = 3;
//            flag = true;
//        }
//    }

//    if (!flag) {
//        FinalGrade = 2;
//    }


//    var $radios = $(OverallItemID);
//    var filterValue = '[value=' + FinalGrade + ']';
//    $radios.filter(filterValue).attr('checked', true);
//    $(HiddenOverallItemID).val(FinalGrade);
//}

//function IsNotApplicable(NoOfItem) {
//    var itemId;

//    for (var i = 1; i < NoOfItem; i++) {
//        itemId = "item" + i;
//        if ($('#' + itemId).length) {
//            ItemID = '[name="' + itemId + '"]:radio:checked';
//            the_value = jQuery(ItemID).val();

//            if (the_value != 4)
//                return false;
//        }
//    }
//    return true;
//}
//function IsSatisfactory(NoOfItem) {
//    var itemId;
//    for (var i = 1; i < NoOfItem; i++) {
//        itemId = "item" + i;
//        if ($('#' + itemId).length) {
//            ItemID = '[name="' + 'item' + i + '"]:radio:checked';

//            the_value = jQuery(ItemID).val();
//            if (!(the_value == 1 || the_value == 4))
//                return false;
//        }
//    }
//    return true;
//}
//function IsUnSatisfactory() {
//    var itemId;
//    var arrItem = ["4", "5", "6", "7"]; // If any one among these item value u then overall U.
//    for (var i = 0; i < arrItem.length; i++) {
//        itemId = "item" + arrItem[i];
//        if ($('#' + itemId).length) {
//            ItemID = '[name="' + 'item' + arrItem[i] + '"]:radio:checked';
//            the_value = jQuery(ItemID).val();
//            if (the_value == 3)
//                return true;
//        }
//    }
//    return false;
//}


//function Checkgrade() {

//    var ItemID;
//    var the_value;
//    var Small_Value = 1;

//    // For any Type of road, First check Is Remarks Entered valid
//    // Iterate through all textareas using class
//    $(".textArea").each(function (key, value) {
//        var pattern = "^[a-zA-Z0-9 ,.()-]+$";
//        if (($(this).val() != "") && !($(this).val().match(pattern))) {
//            $("#trGradeError").show("slow");
//            $("#divGradeError").show("slow");
//            $("#divGradeError span:eq(1)").html("<strong>Alert: </strong>Invalid Remarks, Can only contains AlphaNumeric values and ,.()-");

//            return false;
//        }
//    });


//    if ($("#IMS_PROPOSAL_TYPE").val() === "P") {
//        if (($("#hdnRole").val() == 6 && ($("#IMS_ISCOMPLETED").val() == "C" || $("#IMS_ISCOMPLETED").val() == "P")) || ($("#hdnRole").val() == 9 && ($("#IMS_ISCOMPLETED").val() == "C" || $("#IMS_ISCOMPLETED").val() == "P")))                   // NQM or CQC for Completed & In Progress Road
//        {
//            ItemID = '[name="item13"]:radio:checked';
//            the_value = jQuery(ItemID).val();

//            if (parseInt(the_value) != 4) {
//                $("#trGradeError").hide("slow");
//                $("#divGradeError").hide("slow");
//                $("#divGradeError span:eq(1)").html("");
//                return true;
//            }

//            $("#trGradeError").show("slow");
//            $("#divGradeError").show("slow");
//            $("#divGradeError span:eq(1)").html("<strong>Alert: </strong> Please select atleast one item!");
//            return false;

//        } else if ($("#hdnRole").val() == 7 && ($("#IMS_ISCOMPLETED").val() == "C" || $("#IMS_ISCOMPLETED").val() == "P"))        //SQM for Completed & In Progress Road
//        {
//            if ($("input[name=optItem]:checked").val() == 4) {
//                $("#trGradeError").show("slow");
//                $("#divGradeError").show("slow");
//                $("#divGradeError span:eq(1)").html("<strong>Alert: </strong> Please select atleast one item!");
//                return false;
//            }
//            else {
//                return true;
//            }

//        } else if ($("#IMS_ISCOMPLETED").val() == "M")        //Maintenance Road, In future Rolewise Items may change(for NQM/SQM), so for that case, compare RoleCodes as compared above
//        {
//            //alert($("input[name=optItem]:checked").val());
//            if ($("input[name=optItem]:checked").val() == 4) {
//                $("#trGradeError").show("slow");
//                $("#divGradeError").show("slow");
//                $("#divGradeError span:eq(1)").html("<strong>Alert: </strong> Please select atleast one item!");
//                return false;
//            }
//            else {
//                return true;
//            }
//        }
//    }
//    else if ($("#IMS_PROPOSAL_TYPE").val() === "L")     //Bridge
//    {
//        //alert($("input[name=optItem8]:checked").val());
//        if ($("input[name=optItem8]:checked").val() == 4) {
//            $("#trGradeError").show("slow");
//            $("#divGradeError").show("slow");
//            $("#divGradeError span:eq(1)").html("<strong>Alert: </strong> Please select atleast one item!");
//            return false;
//        }
//        else {
//            return true;
//        }
//    }
//}

///-------------------------- NQM Grading Calculations for Completed & In Progress Roads Ends Here -----------------------------





////------------------------ NQM and SQM Grading Calculations for Completed & In Progress Starts Here --------------------------

/*  IF all NA then NA
       all S then S
       any 5 to 11 are U then U
       all 5 to 11 are S and other are either SRI or U then SRI  */



function ItemGrade(NoOfItem, currentItem, NoOfSubItem) {

    var SubItemID;
    var the_value;


    var the_value1;
    var the_value2;
    var Small_Value = 0;



    for (var i = 1; i < NoOfSubItem; i++) {
        SubItemID = '[name="' + 'subitem' + currentItem + i + '"]:radio:checked';

        the_value = jQuery(SubItemID).val();

        if (parseInt(Small_Value) < parseInt(the_value) && parseInt(the_value) != 4) {
            Small_Value = the_value;
        }

        // If selected item value is unsatisfactory, show Remarks 
        // else set value to empty and Hide
        if (the_value == 3) {
            $("#remarks" + currentItem + i).show('slow');
        }
        else {
            $("#remarks" + currentItem + i).val('');
            $("#remarks" + currentItem + i).hide('slow');
            $('#spnremarks' + currentItem + i).html("");
            $('#spnremarks' + currentItem + i).hide("slow");
        }
    }

    if (Small_Value == 0)
        Small_Value = 4;

    var ItemID = 'input:radio[name=' + 'item' + currentItem + ']';
    var $radios = $(ItemID);
    var filterValue = '[value=' + Small_Value + ']';
    $radios.filter(filterValue).attr('checked', true);

    var hiddenItemID = "#hiddenitem" + currentItem;
    $(hiddenItemID).val(parseInt(Small_Value));
    OverAllGrade(NoOfItem);
}


function OverAllGrade(NoOfItem) {


    var OverallItemID = 'input:radio[name=item' + NoOfItem + ']';
    var HiddenOverallItemID = '#hiddenitem' + NoOfItem;
    var flag = false;
    var FinalGrade = 4;



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

    //if (!flag) {
    //    if (IsStillRequiredImp()) {
    //        FinalGrade = 2;
    //        flag = true;
    //    }
    //}

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
    $radios.filter(filterValue).attr('checked', true);
    $(HiddenOverallItemID).val(FinalGrade);
}

function IsNotApplicable(NoOfItem) {
    //alert("IsNotApplicable")
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
    //alert("IsSatisfactory")
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
    //alert("IsUnSatisfactory")
    var itemId;
    var arrItem;
    if ($("#IMS_ISCOMPLETED").val() == "C") //completed
    {
        arrItem = ["3", "4", "5", "6", "7", "8", "9"]; // If any one among these item value u then overall U.
    }
    else if ($("#IMS_ISCOMPLETED").val() == "P") //inprogress
    {
        arrItem = ["4", "5", "6", "7", "8", "9"]; // If any one among these item value u then overall U.
    }


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



//function IsStillRequiredImp() {         //add on 10-11-21
//    //alert("IsStillRequiredImp")


//    var arrItem1 = ["5", "6", "7", "8", "9", "10", "11"]; // If any among these items value are S .

//    var count=0;
//    for (var i = 0; i < arrItem1.length; i++) {
//        itemId = "item" + arrItem1[i];

//        ItemID = 'input[name="' + 'item' + arrItem1[i] + '"]:checked';

//            the_value = jQuery(ItemID).val();
//            if (the_value == 1)
//            {
//                
//                count++;
//            }
//    }


//    if (count>0) { 

//        if ((($("input[name=item5]:checked").val() == 1 || $("input[name=item5]:checked").val() == 4) && ($("input[name=item6]:checked").val() == 1 || $("input[name=item6]:checked").val() == 4) && ($("input[name=item7]:checked").val() == 1 || $("input[name=item7]:checked").val() == 4) && ($("input[name=item8]:checked").val() == 1 || $("input[name=item8]:checked").val() == 4) && ($("input[name=item9]:checked").val() == 1 || $("input[name=item9]:checked").val() == 4) && ($("input[name=item10]:checked").val() == 1 || $("input[name=item10]:checked").val() == 4) && ($("input[name=item11]:checked").val() == 1 || $("input[name=item11]:checked").val() == 4)) && ($("input[name=item1]:checked").val() == 3 || $("input[name=item2]:checked").val() == 3 || $("input[name=item3]:checked").val() == 3 || $("input[name=item4]:checked").val() == 3 || $("input[name=item12]:checked").val() == 3 || $("input[name=item13]:checked").val() == 3 || $("input[name=item14]:checked").val() == 3 || $("input[name=item15]:checked").val() == 3 || $("input[name=item16]:checked").val() == 3 || $("input[name=item17]:checked").val() == 3 || $("input[name=item18]:checked").val() == 3 || $("input[name=item19]:checked").val() == 3 || $("input[name=item20]:checked").val() == 3)) {

//            return true;
//        }
//        if ((($("input[name=item5]:checked").val() == 1 || $("input[name=item5]:checked").val() == 4) && ($("input[name=item6]:checked").val() == 1 || $("input[name=item6]:checked").val() == 4) && ($("input[name=item7]:checked").val() == 1 || $("input[name=item7]:checked").val() == 4) && ($("input[name=item8]:checked").val() == 1 || $("input[name=item8]:checked").val() == 4) && ($("input[name=item9]:checked").val() == 1 || $("input[name=item9]:checked").val() == 4) && ($("input[name=item10]:checked").val() == 1 || $("input[name=item10]:checked").val() == 4) && ($("input[name=item11]:checked").val() == 1 || $("input[name=item11]:checked").val() == 4)) && ($("input[name=item1]:checked").val() == 2 || $("input[name=item2]:checked").val() == 2 || $("input[name=item3]:checked").val() == 2 || $("input[name=item4]:checked").val() == 2 || $("input[name=item12]:checked").val() == 2 || $("input[name=item13]:checked").val() == 2 || $("input[name=item14]:checked").val() == 2 || $("input[name=item15]:checked").val() == 2 || $("input[name=item16]:checked").val() == 2 || $("input[name=item17]:checked").val() == 2 || $("input[name=item18]:checked").val() == 2 || $("input[name=item19]:checked").val() == 2 || $("input[name=item20]:checked").val() == 2)) {

//            return true;
//        }


//    }
//    return false;
//}


function Checkgrade() {

    var ItemID;
    var the_value;
    var Small_Value = 1;

    // For any Type of road, First check Is Remarks Entered valid
    // Iterate through all textareas using class
    $(".textArea").each(function (key, value) {
        var pattern = "^[a-zA-Z0-9 ,.()-]+$";
        if (($(this).val() != "") && !($(this).val().match(pattern))) {
            $("#trGradeError").show("slow");
            $("#divGradeError").show("slow");
            $("#divGradeError span:eq(1)").html("<strong>Alert: </strong>Invalid Remarks, Can only contains AlphaNumeric values and ,.()-");

            return false;
        }
    });


    if ($("#IMS_PROPOSAL_TYPE").val() === "P") {
        if (($("#hdnRole").val() == 6 && ($("#IMS_ISCOMPLETED").val() == "C" || $("#IMS_ISCOMPLETED").val() == "P")) || ($("#hdnRole").val() == 9 && ($("#IMS_ISCOMPLETED").val() == "C" || $("#IMS_ISCOMPLETED").val() == "P")) || ($("#hdnRole").val() == 7 && ($("#IMS_ISCOMPLETED").val() == "C" || $("#IMS_ISCOMPLETED").val() == "P")))                   // NQM or CQC for Completed & In Progress Road
        {
            if ($("#IMS_ISCOMPLETED").val() == "C") {
                ItemID = '[name="item20"]:radio:checked';
            }
            else if ($("#IMS_ISCOMPLETED").val() == "P") {
                ItemID = '[name="item18"]:radio:checked';
            }

            the_value = jQuery(ItemID).val();

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

        }
            //else if ($("#hdnRole").val() == 7 && ($("#IMS_ISCOMPLETED").val() == "C" || $("#IMS_ISCOMPLETED").val() == "P"))        //SQM for Completed & In Progress Road
            //{
            //    if ($("input[name=optItem]:checked").val() == 4) {
            //        $("#trGradeError").show("slow");
            //        $("#divGradeError").show("slow");
            //        $("#divGradeError span:eq(1)").html("<strong>Alert: </strong> Please select atleast one item!");
            //        return false;
            //    }
            //    else {
            //        return true;
            //    }

            //}
        else if ($("#IMS_ISCOMPLETED").val() == "M")        //Maintenance Road, In future Rolewise Items may change(for NQM/SQM), so for that case, compare RoleCodes as compared above
        {
            //alert($("input[name=optItem]:checked").val());
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
    }
    else if ($("#IMS_PROPOSAL_TYPE").val() === "L")     //Bridge
    {
        //alert($("input[name=optItem10]:checked").val());
        if ($("input[name=optItem10]:checked").val() == 4) {
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










////------------------------ NQM and SQM Grading Calculations for Completed & In Progress Ends Here --------------------------









////-------------------------- SQM Grading Calculations for Completed & In Progress Roads --------------------------

function fnGradeValidate(currentItem) {

    /*-----
            If All NA then NA
            Else IF Any one Of 5,6,7,8,9 is U then U
            Else IF All of 5,6,7,8,9 are S and any other is SRI or U then Overall is SRI
            Else S
       ----*/

    // If selected item value is unsatisfactory, show Remarks 
    // else set value to empty and Hide
    if ($("input[name=optItem" + currentItem + "]:checked").val() == 3) {
        $("#remarks" + currentItem).show('slow');
    }
    else {
        $("#remarks" + currentItem).val('');
        $("#remarks" + currentItem).hide('slow');
        $('#spnremarks' + currentItem).html("");
        $('#spnremarks' + currentItem).hide("slow");
    }

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

    //$("#overAllGrade").val($("input[name=optItem]:checked").val());
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


//----------------------------SQM Grading Calculations for Completed & In Progress Roads Ends Here -----------------------------------





//----------------------------NQM/SQM/CQC Grading Calculations for Maintenance Roads Starts Here -----------------------------------
function fnGradeValidateForMaintenanceRoad(currentItem) {

    /*-----
            If All NA then NA
            Else IF Any one Of 2,3 is U then U
            Else IF All of 2,3 are S and any other is U then Overall is SRI
            Else S
       ----*/


    if ($("#IMS_ISCOMPLETED").val() == "M") {

        // If selected item value is unsatisfactory, show Remarks 
        // else set value to empty and Hide
        if ($("input[name=optItem" + currentItem + "]:checked").val() == 3) {
            $("#remarks" + currentItem).show('slow');
        }
        else {
            $("#remarks" + currentItem).val('');
            $("#remarks" + currentItem).hide('slow');
            $('#spnremarks' + currentItem).html("");
            $('#spnremarks' + currentItem).hide("slow");
        }


        // Calculation of overall grading
        if ($("input[name=optItem1]:checked").val() == 4 && $("input[name=optItem2]:checked").val() == 4 && $("input[name=optItem3]:checked").val() == 4 && $("input[name=optItem4]:checked").val() == 4 && $("input[name=optItem5]:checked").val() == 4 && $("input[name=optItem6]:checked").val() == 4) {
            optItem = "NA";

        }
        else if ($("input[name=optItem2]:checked").val() == 3 || $("input[name=optItem3]:checked").val() == 3) {
            optItem = "U";

        }
        else if ($("input[name=optItem1]:checked").val() == 3 || $("input[name=optItem4]:checked").val() == 3 || $("input[name=optItem5]:checked").val() == 3 || $("input[name=optItem6]:checked").val() == 3) {
            optItem = "SRI";

        }
        else {
            optItem = "S";
        }
    }

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


//----------------------------NQM/SQM/CQC Grading Calculations for Maintenance Roads Ends Here -----------------------------------


//----------------------------NQM/SQM/CQC Grading Calculations for Long Span Bridges Starts Here -----------------------------------
//changes made by sachin in bridge inspection format
function fnGradeValidateForLSB(currentItem) {

    /*-----
            If All NA then NA
            Else IF Any one Of 2,3,4,6 is U then U
            Else IF All of 2,3,4,6 are S and any other is U or SRI then Overall is SRI 
            Else If All are S then Overall is S
            Else SRI
       ----*/

    // If selected item value is unsatisfactory, show Remarks 
    // else set value to empty and Hide
    if ($("input[name=optItem" + currentItem + "]:checked").val() == 3) {
        $("#remarks" + currentItem).show('slow');
    }
    else {
        $("#remarks" + currentItem).val('');
        $("#remarks" + currentItem).hide('slow');
        $('#spnremarks' + currentItem).html("");
        $('#spnremarks' + currentItem).hide("slow");
    }

    //--- Progress Road
    if ($("#IMS_ISCOMPLETED").val() == "C") {

        if ($("input[name=optItem2]:checked").val() == 4 && $("input[name=optItem3]:checked").val() == 4 && $("input[name=optItem4]:checked").val() == 4 && $("input[name=optItem5]:checked").val() == 4 && $("input[name=optItem6]:checked").val() == 4 && $("input[name=optItem7]:checked").val() == 4 && $("input[name=optItem8]:checked").val() == 4) {
            optItem = "NA";
        }
        else if ($("input[name=optItem2]:checked").val() == 3 || $("input[name=optItem3]:checked").val() == 3 || $("input[name=optItem4]:checked").val() == 3 || $("input[name=optItem6]:checked").val() == 3) {
            optItem = "U";
        }
        else if (($("input[name=optItem2]:checked").val() == 1 && $("input[name=optItem3]:checked").val() == 1 && $("input[name=optItem4]:checked").val() == 1 && $("input[name=optItem6]:checked").val() == 1) && ($("input[name=optItem5]:checked").val() == 3 || $("input[name=optItem8]:checked").val() == 3 || $("input[name=optItem7]:checked").val() == 3)) {
            optItem = "SRI";
        }
        else if (($("input[name=optItem2]:checked").val() == 1 && $("input[name=optItem3]:checked").val() == 1 && $("input[name=optItem4]:checked").val() == 1 && $("input[name=optItem6]:checked").val() == 1) && ($("input[name=optItem5]:checked").val() == 2 || $("input[name=optItem8]:checked").val() == 2 || $("input[name=optItem7]:checked").val() == 2)) {
            optItem = "SRI";

        }
        else if (($("input[name=optItem2]:checked").val() == 2 || $("input[name=optItem3]:checked").val() == 2 || $("input[name=optItem4]:checked").val() == 2) || $("input[name=optItem5]:checked").val() == 2 || $("input[name=optItem6]:checked").val() == 2 || $("input[name=optItem7]:checked").val() == 2 || $("input[name=optItem8]:checked").val() == 2) {
            optItem = "SRI";

        }
        else if ($("input[name=optItem2]:checked").val() == 1 && $("input[name=optItem3]:checked").val() == 1 && $("input[name=optItem4]:checked").val() == 1 && $("input[name=optItem5]:checked").val() == 1 && $("input[name=optItem6]:checked").val() == 1 && $("input[name=optItem7]:checked").val() == 1 && $("input[name=optItem8]:checked").val() == 1) {
            optItem = "S";
        }

        else if (($("input[name=optItem2]:checked").val() !== 3 && $("input[name=optItem3]:checked").val() !== 3 && $("input[name=optItem4]:checked").val() !== 3 && $("input[name=optItem6]:checked").val() !== 3) && ($("input[name=optItem5]:checked").val() == 2 || $("input[name=optItem8]:checked").val() == 2 || $("input[name=optItem7]:checked").val() == 2)) {
            optItem = "SRI";

        }

        else if (($("input[name=optItem2]:checked").val() !== 3 && $("input[name=optItem3]:checked").val() !== 3 && $("input[name=optItem4]:checked").val() !== 3 && $("input[name=optItem6]:checked").val() !== 3) && ($("input[name=optItem5]:checked").val() == 3 || $("input[name=optItem8]:checked").val() == 3 || $("input[name=optItem7]:checked").val() == 3)) {
            optItem = "SRI";

        }

        else {
            optItem = "S";
        }
    }
    else {

        if ($("input[name=optItem1]:checked").val() == 4 && $("input[name=optItem2]:checked").val() == 4 && $("input[name=optItem3]:checked").val() == 4 && $("input[name=optItem4]:checked").val() == 4 && $("input[name=optItem5]:checked").val() == 4 && $("input[name=optItem6]:checked").val() == 4 && $("input[name=optItem7]:checked").val() == 4 && $("input[name=optItem8]:checked").val() == 4) {
            optItem = "NA";
        }
        else if ($("input[name=optItem2]:checked").val() == 3 || $("input[name=optItem3]:checked").val() == 3 || $("input[name=optItem4]:checked").val() == 3 || $("input[name=optItem6]:checked").val() == 3) {
            optItem = "U";
        }
        else if (($("input[name=optItem2]:checked").val() == 1 && $("input[name=optItem3]:checked").val() == 1 && $("input[name=optItem4]:checked").val() == 1 && $("input[name=optItem6]:checked").val() == 1) && ($("input[name=optItem1]:checked").val() == 3 || $("input[name=optItem5]:checked").val() == 3 || $("input[name=optItem8]:checked").val() == 3 || $("input[name=optItem7]:checked").val() == 3)) {
            optItem = "SRI";
        }
        else if (($("input[name=optItem2]:checked").val() == 1 && $("input[name=optItem3]:checked").val() == 1 && $("input[name=optItem4]:checked").val() == 1 && $("input[name=optItem6]:checked").val() == 1) && ($("input[name=optItem1]:checked").val() == 2 || $("input[name=optItem5]:checked").val() == 2 || $("input[name=optItem8]:checked").val() == 2 || $("input[name=optItem7]:checked").val() == 2)) {
            optItem = "SRI";

        }
        else if (($("input[name=optItem1]:checked").val() == 2 || $("input[name=optItem2]:checked").val() == 2 || $("input[name=optItem3]:checked").val() == 2 || $("input[name=optItem4]:checked").val() == 2) || $("input[name=optItem5]:checked").val() == 2 || $("input[name=optItem6]:checked").val() == 2 || $("input[name=optItem7]:checked").val() == 2 || $("input[name=optItem8]:checked").val() == 2) {
            optItem = "SRI";

        }
        else if ($("input[name=optItem1]:checked").val() == 1 && $("input[name=optItem2]:checked").val() == 1 && $("input[name=optItem3]:checked").val() == 1 && $("input[name=optItem4]:checked").val() == 1 && $("input[name=optItem5]:checked").val() == 1 && $("input[name=optItem6]:checked").val() == 1 && $("input[name=optItem7]:checked").val() == 1 && $("input[name=optItem8]:checked").val() == 2) {
            optItem = "S";
        }

        else if (($("input[name=optItem2]:checked").val() !== 3 && $("input[name=optItem3]:checked").val() !== 3 && $("input[name=optItem4]:checked").val() !== 3 && $("input[name=optItem6]:checked").val() !== 3) && ($("input[name=optItem5]:checked").val() == 2 || $("input[name=optItem8]:checked").val() == 2 || $("input[name=optItem1]:checked").val() == 2 || $("input[name=optItem7]:checked").val() == 2)) {
            optItem = "SRI";

        }
        else if (($("input[name=optItem2]:checked").val() !== 3 && $("input[name=optItem3]:checked").val() !== 3 && $("input[name=optItem4]:checked").val() !== 3 && $("input[name=optItem6]:checked").val() !== 3) && ($("input[name=optItem5]:checked").val() == 3 || $("input[name=optItem8]:checked").val() == 3 || $("input[name=optItem1]:checked").val() == 3 || $("input[name=optItem7]:checked").val() == 3)) {
            optItem = "SRI";

        }


        else {
            optItem = "S";
        }
    }

    if (optItem == "S") {
        $("input[name=optItem10][value=" + 1 + "]").attr('checked', 'checked');
        $("#hiddenitem18").val(1);
    }
    else if (optItem == "SRI") {
        $("input[name=optItem10][value=" + 2 + "]").attr('checked', 'checked');
        $("#hiddenitem18").val(2);
    }
    else if (optItem == "U") {
        $("input[name=optItem10][value=" + 3 + "]").attr('checked', 'checked');
        $("#hiddenitem18").val(3);
    }
    else if (optItem == "NA") {
        $("input[name=optItem10][value=" + 4 + "]").attr('checked', 'checked');
        $("#hiddenitem18").val(4);
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

        // If selected item value is unsatisfactory, show Remarks 
        // else set value to empty and Hide

        if (the_value == 3) {

            $("#remarks" + currentItem + i).show('slow');
        }
        else {

            $("#remarks" + currentItem + i).val('');
            $("#remarks" + currentItem + i).hide('slow');
            $('#spnremarks' + currentItem + i).html("");
            $('#spnremarks' + currentItem + i).hide("slow");
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
            Else IF Any one Of 2,3,4 ,6 is U then U
            Else IF All of 2,3,4,6 are S and any other is U then Overall is SRI
            Else If All are S then Overall is S
            Else SRI
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
    }


    var $radios = $(OverallItemID);
    var filterValue = '[value=' + FinalGrade + ']';
    $radios.filter(filterValue).attr('checked', true);
    $(HiddenOverallItemID).val(FinalGrade);
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
    var arrItem = ["2", "3", "4", "6"]; // If any one among these item value u then overall U.
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