/*
This file is used for finlization and display of the eremittaqnce and eapyment

*/



$(document).ready(function () {

    // $("input:password").iphonePassword();
    $('#SignEpaymentDialog').show();

    //Added By Abhishek kamble 26-May-2014
    $("#btnPrintEpaymentDetails").click(function () {
        //alert('1');
        PrintEpaymentDetails("#SignEpaymentDialog");
    });
    $("#btnPrintERemDetails").click(function () {
        PrintEremDetails("#SignERemDialog");
    });

    if (varAlreadyReg == 1) {
        //loadPaymentGrid("view");
        //alert("SNA : "+$("#SNAHold").is(':checked'));
        //alert("EPAY : " + $("#Epay").is(':checked'));
        //alert("Holding : " + $("#SecHold").is(':checked'));

        if ($("#SNAHold").is(':checked')) {
            loadSNAToHoldingPaymentGrid("View");
        } else {
            loadPaymentGrid("view");
        }
    }
    else {
        // alert('not correct');
    }

    $("#fromDate").datepicker({
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        maxDate: new Date(),
        onClose: function () {
            $(this).focus().blur();
        }
    });

    $("#toDate").datepicker({
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        maxDate: new Date(),
        onClose: function () {
            $(this).focus().blur();
        }

    });


    //function to show search option
    $("#Search").click(function () {

        $("#tblSearch").toggle('slow', function () { });

        $("#tblOptions").toggle('slow', function () { });

    });

    //function to clear the search options
    $("#btnClearSearch,#iconCloseEPayment").click(function () {

        $("#fromDate").val("");
        $("#toDate").val("");
        $("#TXN_ID").val(0);
        $('#PaymentList').jqGrid('GridUnload');
        loadPaymentGrid("view");
        $("#tblSearch").toggle('slow', function () { });
        $("#tblOptions").toggle('slow', function () { });
    });

    //function for button search
    $("#btnSearch").click(function () {
        //PFMS Validations
        if ($('#TXN_ID').val() == "") {
            alert('Please select Transaction Type');
            return false;
        }
        if ($("#listForm").valid()) {
            $('#PaymentList').jqGrid('GridUnload');

            var mode = "Search";

            loadPaymentGrid(mode);
        }
    });

    //function for view details button click
    $("#btnViewSubmit").click(function () {
        if ($('#months').val() == 0) {
            alert("please select month");
            return false;
        }

        if ($('#year').val() == 0) {
            alert("please select year");
            return false;
        }

        $('#PaymentList').jqGrid('GridUnload');
        //loadPaymentGrid("view");
        if ($("#SNAHold").is(':checked')) {
            loadSNAToHoldingPaymentGrid("View");
        } else {
            loadPaymentGrid("view");
        }
    });

    //event for the rpayment and eremittance radio buttons
    //$("#Epay,#ERem").click(function () { //commented on 13-03-2023
    $("#Epay,#ERem,#SecHold,#SNAHold").click(function () {            //Added on 13-03-2023
        if ($('#months').val() == 0) {
            alert("please select month");
            return false;
        }

        if ($('#year').val() == 0) {
            alert("please select year");
            return false;
        }

        $('#PaymentList').jqGrid('GridUnload');
        //loadPaymentGrid("view");

        if ($("#SNAHold").is(':checked')) {
            loadSNAToHoldingPaymentGrid("View");
        } else {
            loadPaymentGrid("view");
        }

    });




    //function to submit & finalize the epayment
    $("#EpaySubmit").click(function () {

        if ($('#EpayVoucherPassword').val() == "") {
            alert("Please enter password");
            $('#EpayVoucherPassword').focus();
            return false;
        }
        if (confirm("Are you sure you want to finalize the Epayment ? ")) {

            $("#EpayVoucherPassword").hide();

            var hashedPassword = "";

            hashedPassword = hex_md5($('#EpayVoucherPassword').val());


            $.ajax({
                type: "POST",
                url: "/payment/FinalizeEpayment/" + $("#urlParam").val(),
                data: { 'EpayPassword': hashedPassword },
                error: function (xhr, status, error) {
                    unblockPage();
                    //$('#errorSpan1').text(xhr.responseText);
                    $('#divError1').show('slow');
                    $('#errorSpan1').show();
                    $("#EpayVoucherPassword").show();
                    return false;
                },
                success: function (data) {
                    unblockPage();
                    $('#divError1').hide('slow');
                    $('#errorSpan1').html("");
                    $('#errorSpan1').hide();

                    if (data.Success == 1) {

                        $('#PaymentList').jqGrid('GridUnload');
                        loadPaymentGrid("view");
                        $('#EpayVoucherPassword').val("");
                        $("#dialog").dialog("close");
                        alert("Epayment Finalized Successfully. ");
                        return false;
                    }
                    else if (data.Success == -1) {
                        alert("Epayment cant be Finalized as password is incorrect.");
                        $('#EpayVoucherPassword').val("");
                        $("#EpayVoucherPassword").show();
                        return false;
                    }
                    else if (data.Success == -3) {
                        alert("Epayment cant be Finalized as Date of Epayment finalization (current date) is not equal to Epayment date .");
                        $('#EpayVoucherPassword').val("");
                        $("#EpayVoucherPassword").show();
                        return false;
                    }
                    else {

                        //Added By Abhishek kamble 23-May-2014
                        if (data.ErrorMessage === undefined) {
                            alert("Error while finalizing the Epayment.");
                        }
                        else {
                            alert("Error while finalizing the Epayment " + data.ErrorMessage);
                        }
                        $('#EpayVoucherPassword').val("");
                        $("#EpayVoucherPassword").show();
                        return false;
                    }
                }
            });
        }

    });

    //$('#btnEpayClose').click(function () {
    //    alert('aaa');
    //    window.location = '/Payment/GetEPayList/';
    //    //$.ajax({
    //    //    type: "get",
    //    //    url: '/Payment/GetEPayList/'

    //    //})
    //});

    //function to submit and finalize the eremitance
    $("#EremSubmit").click(function () {

        if ($('#EremVoucherPassword').val() == "") {
            alert("Please enter password");
            $('#EremVoucherPassword').focus();
            return false;
        }
        if (confirm("Are you sure you want to finalize the Eremittance ? ")) {

            $("#EremVoucherPassword").hide();

            var hashedPassword = "";

            hashedPassword = hex_md5($('#EremVoucherPassword').val());


            $.ajax({
                type: "POST",
                url: "/payment/FinalizeEremittance/" + $("#urlParamRem").val(),
                data: { 'EpayPassword': hashedPassword },
                error: function (xhr, status, error) {
                    unblockPage();
                    //$('#errorSpan2').text(xhr.responseText);
                    $('#divError2').show('slow');
                    $('#errorSpan2').show();
                    $("#EremVoucherPassword").show();
                    return false;
                },
                success: function (data) {
                    unblockPage();
                    $('#divError2').hide('slow');
                    $('#errorSpan2').html("");
                    $('#errorSpan2').hide();

                    if (data.Success == 1) {

                        $('#PaymentList').jqGrid('GridUnload');
                        loadPaymentGrid("view");
                        $('#EremVoucherPassword').val("");
                        $("#PaymentEremDialogForMaster").dialog("close");
                        alert("Eremittance Finalized Successfully.");
                        return false;
                    }
                    else if (data.Success == -1) {
                        alert("Eremittance cant be Finalized as password is incorrect.");
                        $('#EremVoucherPassword').val("");
                        $("#EremVoucherPassword").show();
                        return false;
                    }
                    else if (data.Success == -3) {
                        alert("Eremittance cant be Finalized as Date of Eremittance finalization (current date) is not equal to Eremittance date .");
                        $('#EremVoucherPassword').val("");
                        $("#EremVoucherPassword").show();
                        return false;
                    }
                    else {
                        //Modified By Abhishek kamble 23-May-2014                      

                        if (data.ErrorMessage === undefined) {
                            alert("Error while finalizing the Eremittance.");
                        }
                        else {
                            alert("Error while finalizing the Eremittance " + data.ErrorMessage);
                        }

                        $('#EremVoucherPassword').val("");
                        $("#EremVoucherPassword").show();
                        return false;
                    }
                }
            });
        }

    });


});//document ready

//$('#btnEpayClose').click(function () {
//    alert('aaa');
//    window.location = '/Payment/GetEPayList/';
//    //$.ajax({
//    //    type: "get",
//    //    url: '/Payment/GetEPayList/'

//    //})
//});

function UnlockEremittancePayment(urlParam) {

    if (confirm("Are you sure you want to unlock the Eremittance ? ")) {

        blockPage();

        $.ajax({
            type: "POST",
            url: "/payment/UnlockEpayment/" + urlParam,
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

                if (data.Success == 1) {

                    $("#PaymentList").jqGrid().setGridParam({ url: '/Payment/GetEPaymentListJson/' }).trigger("reloadGrid");

                    alert("Eremittance Definalized. ");
                    return false;
                }
                else if (data.Success == -1) {
                    alert("Eremittance cant be unlocked,as voucher is not finalized");
                    return false;
                }
                else if (data.Success == -2) {
                    alert("Eremittance cant be unlocked as bank authorization has already been issued.");
                    return false;
                }
                else if (data.Success == -3) {
                    alert("Epaymnet cant not be unlocked , as available for resending e-Payment.");
                    return false;
                }
                else if (data.Success == -222) {
                    alert("Epaymnet cant not be unlocked , as month is closed.");
                    return false;
                }
                else {
                    alert("Error while unlocking the Eremittance.");
                    return false;
                }
            }
        });


    }
}


//function unlock the Epayment
function UnlockPayment(urlParam) {

    if (confirm("Are you sure you want to unlock the Epayment ? ")) {

        blockPage();

        $.ajax({
            type: "POST",
            url: "/payment/UnlockEpayment/" + urlParam,
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

                if (data.Success == 1) {

                    $("#PaymentList").jqGrid().setGridParam({ url: '/Payment/GetEPaymentListJson/' }).trigger("reloadGrid");

                    alert("Epayment Definalized.");
                    return false;
                }
                else if (data.Success == -1) {
                    alert("Epayment cant be unlocked,as voucher is not finalized");
                    return false;
                }
                else if (data.Success == -2) {
                    alert("Epayment cant be unlocked as bank authorization has already been issued.");
                    return false;
                }
                else if (data.Success == -222) {
                    alert("Epaymnet cant not be unlocked , as month is closed.");
                    return false;
                }
                else {
                    alert("Error while unlocking the voucher.");
                    return false;
                }
            }
        });


    }
}

var masterGridWidth = 0
function loadPaymentGrid(mode) {
    // alert('in');
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#PaymentList").jqGrid({

        url: '/Payment/GetEPaymentListJson/',
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: 15,
        postData: {
            'mode': mode, 'months': $('#months').val(), 'year': $('#year').val(), 'fromDate': $('#fromDate').val(), 'toDate': $('#toDate').val(), 'transType': $("#TXN_ID").val(), 'moduleType': module_type,
            'payType': function () {
                //return $("#Epay").is(':checked') ? "E" : "R";//commented on 13-03-2023 to add radio button for Holding to Security Deposit Account Transfer
                return $("#Epay").is(':checked') ? "E" : $("#ERem").is(':checked') ? "R" : "D";//Added on 13-03-2023 to add radio button for Holding to Security Deposit Account Transfer
            }
        },
        rownumbers: true,
        //width: 1150,
        autowidth: true,
        pginput: false,
        //shrinkToFit: false,
        rowList: [15, 20, 30],
        colNames: ['Voucher Number', 'Voucher Date', 'Cash/Cheque', 'Transaction Type', 'Epayment/Eremittance Number', 'Epayment/Eremittance Date', 'Contractor/Payee Name', 'Agreement Number', 'Cheque Amount </br>(In Rs.)', 'Cash Amount </br> (In Rs.)', 'Gross Amount (In Rs.)', 'Definalize', 'Sign Digitally -DBT', 'Sign Digitally -REAT', 'Epayment/</br> Eremittance </br> Order', 'Bill Type', 'PFMS Status', 'PFMS Remarks'],
        colModel: [
            {
                name: 'Voucher_Number',
                index: 'Voucher_Number',
                width: 50,
                align: "center",
                frozen: true

            },
            {
                name: 'voucher_date',
                index: 'auth_sig_name',
                width: 80,
                align: "center",
                frozen: true,

            },
            {
                name: 'Cash_Cheque',
                index: 'Cash_Cheque',
                width: 0,
                align: "center",
                frozen: true,
                hidden: true

            },

            {
                name: 'Transaction_type',
                index: 'Transaction_type',
                width: 0,
                align: "left",
                frozen: true,
                hidden: true

            },
            {
                name: 'cheque_number',
                index: 'cheque_number',
                width: 120,
                align: "center"

            },
            {
                name: 'cheque_Date',
                index: 'cheque_Date',
                width: 100,
                align: "Center"

            }, {
                name: 'Payee_Name',
                index: 'Payee_Name',
                width: 150,
                align: "Center"

            },
            {
                name: 'Agreement_Number',
                index: 'Agreement_Number',
                width: 130,
                align: "left",
                hidden: true


            }, {
                name: 'cheque_amount',
                index: 'cheque_amount',
                width: 100,
                align: "right"

            },
            {
                name: 'Cash_Amount',
                index: 'Cash_Amount',
                width: 100,
                align: "right"

            },
            {
                name: 'Gross_Amount',
                index: 'Gross_Amount',
                width: 100,
                align: "right"

            },


            {
                name: 'Definalize',
                index: 'Definalize',
                width: 50,
                align: "Center"

            },

            {
                name: 'Dig_Sign1',
                index: 'Dig_Sign1',
                width: 50,
                align: "left",
                hidden: true    //Added on 13-oct-2021


            },
            {
                name: 'Dig_Sign',
                index: 'Dig_Sign',
                width: 50,
                align: "left"


            },
            {
                name: 'ViewEPay',
                index: 'ViewEPay',
                width: 50,
                align: "Center"

            },
            {
                name: 'BillType',
                index: 'BillType',
                width: 50,
                align: "Center",
                hidden: true

            },
            {
                name: 'PFMSStatus',
                index: 'PFMSStatus',
                width: 60,
                align: "center",
                sortable: false,
                hidden: ($("#Epay").is(':checked') && $('#fundtype').val() == 'P') ? false : true,
            },
            {
                name: 'PFMSRemarks',
                index: 'PFMSRemarks',
                width: 150,
                align: "center",
                sortable: false,
                hidden: ($("#Epay").is(':checked') && $('#fundtype').val() == 'P') ? false : true,
            }

        ],
        pager: "#pager",
        viewrecords: true,
        loadError: function (xhr, st, err) {
            $.unblockUI();
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        loadComplete: function (xhr, st, err) {
            $.unblockUI();

            $("#PaymentList").parents('div.ui-jqgrid-bdiv').css("max-height", "385px");
            //Added By Abhishek kamble 11-nov-2013
            $('#PaymentList_rn').html('Sr.<br/>No');
        },
        sortname: 'voucher_date',
        sortorder: "desc",
        caption: "Payment Details"
    });



}

function SignEPaymentXml(urlParam1) {

    $.ajax({
        url: '/PFMS1/SignPaymentXml',
        type: 'POST',
        cache: false,
        data: { encrBillId: urlParam1 },
        success: function (data) {
            //$("#containerDsc").html(response);
            unblockPage();

            $('#divSignError4').hide('slow');
            $('#errorSignSpan4').html("");
            $('#errorSignSpan4').hide();

            if (data != "") {
                //  alert(data);
                $("#SignEpaymentDiv1").html(data);

                $("#SignEpaymentDialog1").dialog("open");
                return false;
            }
        },
        complete: function () {

        },
        error: function (xhr, status, error) {
            unblockPage();

            $('#errorSignSpan4').text(xhr.responseText);
            $('#divSignError4').show('slow');
            $('#errorSignSpan4').show();
            return false;
        },
    });
}


function SignEPaymentREATXml(urlParam1) {
    //alert('a');
    $.ajax({

        // url: '/REAT/REAT/GetXml',
        url: '/REAT/REAT/SignEPaymentREATXml',
        type: 'POST',
        cache: false,
        data: { encrBillId: urlParam1 },
        success: function (data) {
            //$("#containerDsc").html(response);
            unblockPage();

            $('#REATdivSignError4').hide('slow');
            $('#REATerrorSignSpan4').html("");
            $('#REATerrorSignSpan4').hide();

            if (data != "") {
                //  alert(data);
                $("#REATSignEpaymentDiv1").html(data);

                $("#REATSignEpaymentDialog1").dialog("open");
                return false;
            }
        },
        complete: function () {

        },
        error: function (xhr, status, error) {
            unblockPage();

            $('#REATerrorSignSpan4').text(xhr.responseText);
            $('#REATdivSignError4').show('slow');
            $('#REATerrorSignSpan4').show();
            return false;
        },
    });
}



function SignEPayment(urlParam1) {
    //alert("SignEpayment :"+$("#SecHold").is(':checked'));

    $.ajax({
        type: "POST",

        url: $("#SecHold").is(':checked') ? "/payment/SignEpaymentPDFHolding/" + urlParam1 + '#' + Math.random() : "/payment/SignEpaymentPDF/" + urlParam1 + '#' + Math.random(),
        //async: false,
        error: function (xhr, status, error) {
            unblockPage();

            $('#errorSignSpan4').text(xhr.responseText);
            $('#divSignError4').show('slow');
            $('#errorSignSpan4').show();
            return false;
        },
        success: function (data) {
            unblockPage();

            $('#divSignError4').hide('slow');
            $('#errorSignSpan4').html("");
            $('#errorSignSpan4').hide();

            if (data != "") {
                //alert(JSON.stringify(data));
                if (data.status != null && data.status == "error") {
                    alert(data.message);
                    return false;
                }
                $("#SignEpaymentDiv1").html(data);

                $("#SignEpaymentDialog1").dialog("open");

                return false;
            }
        }
    });
}


function SignERem(urlParam1) {
    $.ajax({
        type: "POST",
        url: "/payment/SignERemtPDF/" + urlParam1 + '#' + Math.random(),
        //async: false,
        error: function (xhr, status, error) {
            unblockPage();

            $('#errorSignSpan5').text(xhr.responseText);
            $('#divSignError5').show('slow');
            $('#errorSignSpan5').show();
            return false;
        },
        success: function (data) {
            unblockPage();

            $('#divSignError5').hide('slow');
            $('#errorSignSpan5').html("");
            $('#errorSignSpan5').hide();

            if (data != "") {
                //  alert(data);
                $("#SignERemDiv1").html(data);

                $("#SignERemDialog1").dialog("open");
                return false;
            }
        }
    });
}

//function to show epayment order 
function ViewEpayOrder(urlParam1) {


    $.ajax({
        type: "POST",
        url: "/payment/GetEpaymentDetailsForSigning/" + urlParam1 + '#' + Math.random(),
        //async: false,
        error: function (xhr, status, error) {
            unblockPage();

            $('#errorSignSpan2').text(xhr.responseText);
            $('#divSignError2').show('slow');
            $('#errorSignSpan2').show();
            return false;
        },
        success: function (data) {
            unblockPage();

            $('#divSignError2').hide('slow');
            $('#errorSignSpan2').html("");
            $('#errorSignSpan2').hide();

            if (data != "") {
                //  alert(data);
                $("#SignEpaymentDiv").html(data);

                $("#SignEpaymentDialog").dialog("open");

                return false;
            }
        }
    });

}

//function to show Eremittance order 
function ViewEremOrder(urlParam, randomno) {
    $.ajax({
        type: "POST",
        url: "/payment/GetERemDetailsForSigning/" + urlParam + '#' + Math.random(),
        //async: false,
        error: function (xhr, status, error) {
            unblockPage();

            $('#errorSignSpan3').text(xhr.responseText);
            $('#divSignError3').show('slow');
            $('#errorSignSpan3').show();
            return false;
        },
        success: function (data) {
            unblockPage();

            $('#divSignError3').hide('slow');
            $('#errorSignSpan3').html("");
            $('#errorSignSpan3').hide();

            if (data != "") {
                //  alert(data);
                $("#SignERemDiv").html(data);

                $("#SignERemDialog").dialog("open");

                return false;
            }
        }
    });

}


//Added By Abhishek kamble 26-May-2014
function PrintEpaymentDetails(elem) {
    //var size = $(elem).css('font-size');
    PrintEpayPopup($(elem).html());
}

function PrintEpayPopup(data) {



    if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {   // Chrome Browser Detected?

        var windowSummary = window.open('', '', 'height=700,width=700');
        windowSummary.document.write('<html><head><title></title>');
        windowSummary.document.write('<style type="text/css">td, th{border:1px solid black;}#first {border-collapse:collapse;}th.ui-th-column div {white-space: normal !important;height: auto !important;padding: 2px;}</style>');

        windowSummary.document.write('</head><body>');
        //  windowSummary.document.write('<h3 style="text-align:center">Epayment Details</h3>');

        windowSummary.document.write(data);
        windowSummary.document.write('</body></html>');

        windowSummary.PPClose = false;                                     // Clear Close Flag
        windowSummary.onbeforeunload = function () {                         // Before Window Close Event
            if (windowSummary.PPClose === false) {                           // Close not OK?
                return 'Leaving this page will block the parent window!\nPlease select "Stay on this Page option" and use the \nCancel button instead to close the Print Preview Window.\n';
            }
        }
        windowSummary.print();
        //mywindow.close();
    } else {

        var windowSummary = window.open('', 'Epayment', 'height=700,width=700');
        windowSummary.document.write('<html><head><title></title>');
        windowSummary.document.write('<style type="text/css">td, th{border:1px solid black;}#first {border-collapse:collapse;}th.ui-th-column div {white-space: normal !important;height: auto !important;padding: 2px;}</style>');

        windowSummary.document.write('</head><body>');
        windowSummary.document.write('<h3 style="text-align:center">Epayment Details</h3>');

        windowSummary.document.write(data);
        windowSummary.document.write('</body></html>');
        windowSummary.print();
    }



    return true;
}


//Added By Abhishek kamble 26-May-2014
function PrintEremDetails(elem) {
    //var size = $(elem).css('font-size');
    PrintERemPopup($(elem).html());
}

function PrintERemPopup(data) {
    //alert('b');
    // $("#divScroll").style.overflow = 'hidden';
    if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {   // Chrome Browser Detected?

        var windowSummary = window.open('', 'ERemittace', 'height=800,width=700');
        windowSummary.document.write('<html><head><title></title>');
        windowSummary.document.write('<style type="text/css">td, th{border:1px solid black;}#first {border-collapse:collapse;}th.ui-th-column div {white-space: normal !important;height: auto !important;padding: 2px;}</style>');

        windowSummary.document.write('</head><body>');
        windowSummary.document.write('<h3 style="text-align:center">E-Remittance Details</h3>');
        windowSummary.document.write(data);
        windowSummary.document.write('</body></html>');

        windowSummary.PPClose = false;                                     // Clear Close Flag
        windowSummary.onbeforeunload = function () {                         // Before Window Close Event
            if (windowSummary.PPClose === false) {                           // Close not OK?
                return 'Leaving this page will block the parent window!\nPlease select "Stay on this Page option" and use the \nCancel button instead to close the Print Preview Window.\n';
            }
        }

        windowSummary.print();
        //mywindow.close();
    } else {
        var windowSummary = window.open('', 'ERemittace', 'height=800,width=700');
        windowSummary.document.write('<html><head><title></title>');
        windowSummary.document.write('<style type="text/css">td, th{border:1px solid black;}#first {border-collapse:collapse;}th.ui-th-column div {white-space: normal !important;height: auto !important;padding: 2px;}</style>');

        windowSummary.document.write('</head><body>');
        windowSummary.document.write('<h3 style="text-align:center">E-Remittance Details</h3>');
        windowSummary.document.write(data);
        windowSummary.document.write('</body></html>');

        windowSummary.print();
    }
    return true;
}

//Below Region added to display list of SNA to holding account transfer

function loadSNAToHoldingPaymentGrid(mode) {
    // alert('in');
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#PaymentList").jqGrid({

        url: '/Payment/GetSecondLevelSuccessEPaymentListJson/',
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: 15,
        postData: {
            'mode': mode, 'months': $('#months').val(), 'year': $('#year').val(), 'fromDate': $('#fromDate').val(), 'toDate': $('#toDate').val(), 'transType': $("#TXN_ID").val(), 'moduleType': module_type,
            'payType': function () {
                /* return $("#Epay").is(':checked') ? "E" : "R";*/
                return "E";
            }
        },
        rownumbers: true,
        //width: 1150,
        autowidth: true,
        pginput: false,
        //shrinkToFit: false,
        rowList: [15, 20, 30],
        colNames: ['Voucher Number', 'Voucher Date', 'Epayment Number', 'Epayment Date', /*'Contractor Name', */'Cheque Amount </br>(In Rs.)', 'Cash Amount </br> (In Rs.)', 'Gross Amount (In Rs.)', 'Payment Acknowledgement', 'Transaction Description',
            'Sign Digitally -REAT',],
        colModel: [
            {
                name: 'Voucher_Number',
                index: 'Voucher_Number',
                width: 50,
                align: "center",
                frozen: true

            },
            {
                name: 'voucher_date',
                index: 'auth_sig_name',
                width: 80,
                align: "center",
                frozen: true,

            },

            {
                name: 'cheque_number',
                index: 'cheque_number',
                width: 120,
                align: "center"

            },
            {
                name: 'cheque_Date',
                index: 'cheque_Date',
                width: 100,
                align: "Center"

            },
            //{
            //    name: 'Contractor_Name',
            //    index: 'Contractor_Name',
            //    width: 150,
            //    align: "Center"

            //},

            {
                name: 'cheque_amount',
                index: 'cheque_amount',
                width: 100,
                align: "Center"

            },
            {
                name: 'Cash_Amount',
                index: 'Cash_Amount',
                width: 100,
                align: "Center"

            },
            {
                name: 'Gross_Amount',
                index: 'Gross_Amount',
                width: 100,
                align: "Center"

            },
            {
                name: 'ack_Date',
                index: 'ack_Date',
                width: 100,
                align: "Center"

            },

            {
                name: 'Txn_Descrp',
                index: 'Txn_Descrp',
                width: 50,
                align: "left"


            },
            {
                name: 'Dig_Sign',
                index: 'Dig_Sign',
                width: 50,
                align: "left"


            }


        ],
        pager: "#pager",
        viewrecords: true,
        loadError: function (xhr, st, err) {
            $.unblockUI();
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        loadComplete: function (xhr, st, err) {
            $.unblockUI();

            $("#PaymentList").parents('div.ui-jqgrid-bdiv').css("max-height", "385px");
            //Added By Abhishek kamble 11-nov-2013
            $('#PaymentList_rn').html('Sr.<br/>No');
        },
        sortname: 'voucher_date',
        sortorder: "desc",
        caption: "Payment Details"
    });
}

function SignSecondlevelSuccessEPaymentREATXml(urlParam1) {
    //alert('a');
    $.ajax({

        // url: '/REAT/REAT/GetXml',
        url: '/REAT/REAT/SignSecondlevelSuccessEPaymentREATXml',
        type: 'POST',
        cache: false,
        data: { encrBillId: urlParam1 },
        success: function (data) {
            //$("#containerDsc").html(response);
            unblockPage();

            $('#REATdivSignError4').hide('slow');
            $('#REATerrorSignSpan4').html("");
            $('#REATerrorSignSpan4').hide();

            if (data != "") {
                //  alert(data);
                $("#REATSignEpaymentDiv1").html(data);

                $("#REATSignEpaymentDialog1").dialog("open");
                //$("#SNAHold").attr('checked', true)
                //alert("SNA set : " + $("#SNAHold").is(':checked'));
                return false;
            }
        },
        complete: function () {

        },
        error: function (xhr, status, error) {
            unblockPage();

            $('#REATerrorSignSpan4').text(xhr.responseText);
            $('#REATdivSignError4').show('slow');
            $('#REATerrorSignSpan4').show();
            return false;
        },
    });
}