//#region file header

/*  Name : AuthorizationMaster.js
 
 *  Path : ~Script\Authorization\AuthorizationMaster.js
 
 *  Description : AuthorizationMaster.js is javascript file used for adding ,editing ,finalizing ,deleting the Authorization Master request.
                 
 *  Functions : 

 *  Author : Amol Jadhav (PE, e-gov)
 *  Company : C-DAC,E-GOV
 *  Dates of creation : 06/06/2013  

*/

//#endregion  file header

jQuery.validator.addMethod("FromdateValidation", function (value, element) {
    if (value == "")
    { return true; }

    if ($("#toDate").val() == "")
    { return true; }

    if (Date.parse(value) > Date.parse($("#toDate").val())) {

        return false;
    }
    else {
        return true;
    }

}, "");

var isValid;
$(document).ready(function () {
    GetClosedMonthAndYear();
    $.validator.unobtrusive.parse($("#listForm"));



    $("#ChkAll").live("change", function () {

        if ($(this).is(":checked"))
        {
            $(".AUTH_STATUS").attr("checked", "checked");
        }
        else {
            $(".AUTH_STATUS").removeAttr("checked");
        }
      
    });

    //event ot keep track of All checkbox button
    $(".AUTH_STATUS").live("change", function () {

        var count = 0;
       
        $(".AUTH_STATUS").each(function () {

            if ($(this).is(":checked")) {
                count = count + 1;
            }
        });

        if (count == 5) {
            $("#ChkAll").attr("checked", "checked");
        }
        else {
            $("#ChkAll").removeAttr("checked");
        }

    });


    loadAuthorizationGrid("view","");


    $("#fromDate").datepicker({
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy"

    });

    $("#toDate").datepicker({
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy"

    });


    //Show the add new master details page
    $("#AddNew").click(function () {
              
        ValidateDPIUBankAuthorization();
        if (isValid == false) {
            return false;
        }

        blockPage();
        

        $("#mainDiv").load('/Authorization/GetAuthMasterAndTransDetails/' + $('#months').val() + '$' + $('#year').val() + '$' + "A");
        unblockPage();
        

    });

    
    //function to show search option
    $("#Search").click(function () {
        $("#fromDate").val("");
        $("#toDate").val("");
        $("#TXN_ID").val(0);
        $("#AUTH_STATE").val("");
        $('#AuthList').jqGrid('GridUnload');
        loadAuthorizationGrid("view", "");
        $("#tblSearch").toggle('slow', function () { });
        $("#tblOptions").toggle('slow', function () { });

    });

    //function to clear the search options
    $("#btnClearSearch").click(function () {

        $("#fromDate").val("");
        $("#toDate").val("");
        $("#TXN_ID").val(0);
        $("#AUTH_STATE").val("");
        $('#AuthList').jqGrid('GridUnload');
        loadAuthorizationGrid("view","");
        $("#tblSearch").toggle('slow', function () { });
        $("#tblOptions").toggle('slow', function () { });

    });


    $("#iconCloseAuthRequest").click(function () {

        $("#fromDate").val("");
        $("#toDate").val("");
        $("#TXN_ID").val(0);
        $("#AUTH_STATE").val("");
        $('#AuthList').jqGrid('GridUnload');
        loadAuthorizationGrid("view", "");
        $("#tblSearch").toggle('slow', function () { });
        $("#tblOptions").toggle('slow', function () { });

    });


    //function for button search
    $("#btnSearch").click(function () {

        $('#fromDate').rules('add', {

            FromdateValidation: true,
            messages: {
                FromdateValidation: 'From Date must be less than or equal to To Date.'
            }
        });

        if ($("#listForm").valid())
        {
            $('#AuthList').jqGrid('GridUnload');

            var mode = "Search";

            var StringAuthStatus = "";
            
            $(".AUTH_STATUS").each(function () {

                if ($(this).is(":checked"))
                {
                    StringAuthStatus = StringAuthStatus +"$"+ $(this).val();
                }
            });

            loadAuthorizationGrid(mode, StringAuthStatus);
        }


    });

    $("#btnViewSubmit").click(function () {

        if ($('#months').val() == 0) {
            alert("please select month");
            return false;
        }

        if ($('#year').val() == 0) {
            alert("please select year");
            return false;
        }

        $('#AuthList').jqGrid('GridUnload');
        loadAuthorizationGrid("view","");

    });

    //new change done by Vikram for adding the changed month and year in session
    $("#months").change(function () {
        UpdateAccountSession($("#months").val(), $("#year").val());
    });

    $("#year").change(function () {
        UpdateAccountSession($("#months").val(), $("#year").val());
    });

});//document ready


//function to delete the authorization request
function DeleteAuthorization(urlParam) {

    var Todelete = confirm('Are you sure you want to delete the authorization request ?');

    if (Todelete) {

        blockPage();

        $.ajax({
            type: "POST",
            url: "/Authorization/DeleteAuthorizationRequest/" + urlParam,
            // async: false,
            data: $("form").serialize(),
            error: function (xhr, status, error) {
                unblockPage();
                $('#errorSpan').text(xhr.responseText);
                $('#divError').show('slow');

                return false;

            },
            success: function (data) {
                unblockPage();
                $('#divError').hide('slow');
                $('#errorSpan').html("");
                $('#errorSpan').hide();

                if (data.result == 1) {

                    alert("Authorization request deleted successfuly.");

                    $("#AuthList").jqGrid().setGridParam({ url: '/Authorization/AuthorizationRequestList/' }).trigger("reloadGrid");
                    return false;
                }
                else if (data.result == -1) {
                    alert("Finalized entry can not be deleted .");
                    return false;
                }
                else {

                    alert("Error while deleting  Authorization ");
                    return false;
                }
            }
        }); //end of ajax
    }

}

//function to edit the authorization request
function EditAuthorization(urlParam) {
    blockPage();

    $("#mainDiv").load('/Authorization/GetEditAuthorizationRequest/' + urlParam, function () {

        //$("#TransactionForm").hide();

        //load payment and deduction form
        $('#AuthDetailsEntryDiv').load('/Authorization/GetAuthDetailsEntryForm/' + Bill_id, function () {

            unblockPage();
            //populate trqansaction grid
            loadPaymentGrid(Bill_id);

            //show transaction form
            $("#trnsShowtable").show('slow');

            $("#TransactionForm").show('slow');

            if (parseFloat(TotalAmtToEnterDedAmount) == 0) {
                $("#AMOUNT_C").val(0).prop("readonly", 'readonly');
                $("#cashAmtTr").hide();
            }
            else {
                $("#AMOUNT_C").val(0).removeAttr("readonly");
                $("#cashAmtTr").show();
            }
            

            changeNarrationDedAuth = true;
            changeNarrationPayAuth = true;

        });
    });
    unblockPage();
}


//function to finalize the auhtorization request
function FinalizeAuthorization(urlParam) {


    if (confirm("Are you sure you want to finalize the authorization request? ")) {
        blockPage();
        $.ajax({
            type: "POST",
            url: "/Authorization/FinalizeAuthorization/" + urlParam ,
            //async: false,
            error: function (xhr, status, error) {
                unblockPage();
                $('#errorSpan').text(xhr.responseText);
                $('#divError').show('slow');
                return false;
            },
            success: function (data) {
                unblockPage();
                $('#divError').hide('slow');
                $('#errorSpan').html("");
                $('#errorSpan').hide();

                if (data.result == "1")
                {
                    $("#AuthList").jqGrid().setGridParam({ url: '/Authorization/AuthorizationRequestList/' }).trigger("reloadGrid");
                    alert("Authorization Finalized Successfully.");
                    return false;
                }
                else if (data.result == "-1") {
                    alert("Authorization cant be Finalized as all transaction details are not entered.");
                    return false;
                }
                else {
                    alert("Error while finalizing the Authorization.");
                    return false;
                }
            }
        });
    }
}

//function to view finalized authorization request
function ViewAuthorization(urlParam) {
   
    blockPage();
    

    $("#mainDiv").load('/Authorization/GetEditAuthorizationRequest/' + urlParam, function () {

        $("#trShowHideLinkTable").hide();

        //populate trqansaction grid
        loadPaymentGrid(urlParam);

        $('#AuthDetailsEntryDiv').load('/Authorization/GetAuthDetailsEntryForm/' + urlParam, function () {

            //show authorization daata entry form
            $("#AuthMasterEntryDiv").show();


            $("#TransactionForm").show('slow');

            if (parseFloat($("#TotalAmtToEnterCachAmount").text()) == 0) {
                $("#AMOUNT_C").val(0).prop("readonly", 'readonly');
            }
            else {
                $("#AMOUNT_C").val(0).removeAttr("readonly");
            }
           
            unblockPage();
            

        });
        unblockPage();

    });
}

var masterGridWidth = 0

//function to populate the grid od authorization request
function loadAuthorizationGrid(mode,paramAuthStatus) {
    blockPage();

    jQuery("#AuthList").jqGrid({

        url: '/Authorization/AuthorizationRequestList/',
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: 15,
        postData: { 'mode': mode, 'months': $('#months').val(), 'year': $('#year').val(), 'fromDate': $('#fromDate').val(), 'toDate': $('#toDate').val(), 'transType': $("#TXN_ID").val(), 'AUTH_STATUS': paramAuthStatus },
        rownumbers: true,
        //width: 1150,
        autowidth: true,
        pginput: false,
        //shrinkToFit: false,
        rowList: [15, 20, 30],
        colNames: ['Authorization Number ', 'Authorization Date', 'Transaction Type',  'Bank Authorization</br>Request Amount (In Rs.)', 'Cash Amount (In Rs.)', 'Gross Amount (In Rs.)', 'Status', 'Edit', 'Delete','Finalize'],
        colModel: [

            {
                name: 'Auth_Number',
                index: 'Auth_Number',
                width:90,
                align: "center",
                frozen: true

            },
            {
                name: 'Auth_date',
                index: 'Auth_date',
                width: 80,
                align: "center",
                frozen: true,

            },
             {
                 name: 'Trans_Type',
                 index: 'Trans_Type',
                 width: 120,
                 align: "left",
                 frozen: true,

             },

              {
                  name: 'ChequeAmount',
                  index: 'ChequeAmount',
                width: 80,
                align: "right"

            },
            {
                name: 'CashAmount',
                index: 'CashAmount',
                width: 80,
                align: "right"

            },
            {
                name: 'GrossAmount',
                index: 'GrossAmount',
                width: 80,
                align: "right"

            },

            {
                name: 'Auth_Status',
                index: 'Auth_Status',
                width: 80,
                align: "left"

            },

              {
                   name: 'Edit',
                   index: 'Edit',
                   width: 40,
                   align: "Center"

               },
               {
                   name: 'Delete',
                   index: 'Delete',
                   width: 50,
                   align: "Center"

               },
                {
                name: 'Finalize',
                index: 'Finalize',
                width: 44,
                align: "Center"

                }



        ],
        pager: "#Authpager",
        viewrecords: true,
        loadError: function (xhr, st, err) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        loadComplete: function (xhr, st, err) {
            unblockPage();


            $("#AuthList").jqGrid('setLabel', "rn", "Sr.</br> No");

        },
        sortname: 'Auth_Number',
        sortorder: "asc",
        caption: "Authorization Request Details"
    });


    //jQuery("#").jqGrid('setFrozenColumns');
    jQuery("#AuthList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
          { startColumnName: 'Edit', numberOfColumns: 2, titleText: 'Action' }

        ]
    });
}
//new method added by Vikram
//function to validate whether Bank Authorization is Enabled or not.
function ValidateDPIUBankAuthorization()
{
    $.ajax({
        type: "POST",
        url: "/Authorization/ValidateDPIUBankAuthorization/",
        async: false,
        error: function (xhr, status, error) {
            unblockPage();
            $('#divError').show('slow');
            $('#errorSpan').text(xhr.responseText);
            return false;
        },
        success: function (data) {
            unblockPage();
            $('#divError').hide('slow');
            $('#errorSpan').html("");
            if (data.success == false) {
                isValid = false;
                alert('User is not authorized to add authorization request .');
            }
            else if (data.success == true) {
                isValid = true;
            }
        }
    });
}

//added by Vikram on 01-Jan-2014
function UpdateAccountSession(month, year) {
    $.ajax({
        url: "/Receipt/UpdateAccountSession",
        type: "GET",
        async: false,
        cache: false,
        data:
            {
                "Month": month,
                "Year": year
            },
        success: function (data) {
            return false;
        },
        error: function () { }
    });
    return false;
}


//Added By Abhishek kamble 14-jan-2014
//function to get the account  Close month and year
function GetClosedMonthAndYear() {
    blockPage();

    $.ajax({
        type: "POST",
        url: "/MonthlyClosing/GetClosedMonthandYear/",
        // async: false,

        error: function (xhr, status, error) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');

            return false;

        },
        success: function (data) {
            unblockPage();
            $('#divError').hide('slow');
            $('#errorSpan').html("");
            $('#errorSpan').hide();

            if (data.monthClosed) {
                $("#lblMonth").text(data.month);
                $("#lblYear").text(data.year);

                $("#TrMonthlyClosing").show('Slow');
                $("#AccountNotClosedTr").hide('Slow');
                return false;
            }
            else if (data.monthClosed == false) {
                $("#AccountNotClosedTr").show('Slow');
                $("#TrMonthlyClosing").hide('Slow');
                return false;
            }
            else {

                alert("Error While getting Monthly Closing Details");
                return false;
            }

        }
    });


}
