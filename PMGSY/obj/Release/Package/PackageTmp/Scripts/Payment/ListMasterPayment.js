

jQuery.validator.addMethod("FromdateValidation", function (value, element) {

    // alert(value);
    // alert($("#toDate").val());

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

var levelId = 0; //variable to get the level 

$(document).ready(function () {

    levelId = $("#levelID").val();

    $.validator.unobtrusive.parse($("#listForm"));

    //Added By Abhishek kamble 26-May-2014
    $("#btnPrintEpaymentDetails").click(function () {
        PrintEpaymentDetails("#dvPrintEpaymentDetails");
    });
    $("#btnPrintERemDetails").click(function () {

        PrintEremDetails("#dvPrintEremmitanceDetails");
    });

    //Added By Abhishek kamble 3-jan-2014 start change
    GetClosedMonthAndYear();
    //Added By Abhishek kamble 3-jan-2014 end change
    loadPaymentGrid("view");

    $("#fromDate").datepicker({
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        maxDate: new Date()
    });

    $("#toDate").datepicker({
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        maxDate: new Date()
    });


    //Show the add new master details page
    $("#AddNew").click(function () {
        //  $("#mainDiv").load('/Payment/AddEditMasterPayment/' + $('#months').val() + '$' + $('#year').val());
        $("#mainDiv").load('/Payment/MasterDetailsPayment/' + $('#months').val() + '$' + $('#year').val() + '$' + "A");
    });


    //function to show search option
    $("#Search").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $("#tblSearch").toggle('slow', function () { });
        $("#tblOptions").toggle('slow', function () { });
        $.unblockUI();
    });

    //function to clear the search options
    $("#btnClearSearch").click(function () {

        $("#fromDate").val("");
        $("#toDate").val("");
        $("#TXN_ID").val(0);
        $("#Chq_Epay").val("");
        $('#PaymentList').jqGrid('GridUnload');
        loadPaymentGrid("view");
        $("#tblSearch").toggle('slow', function () { });
        $("#tblOptions").toggle('slow', function () { });

    });


    $("#iconClosePayment").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#fromDate").val("");
        $("#toDate").val("");
        $("#TXN_ID").val(0);
        $("#Chq_Epay").val("");
        $('#PaymentList').jqGrid('GridUnload');
        loadPaymentGrid("view");
        $("#tblSearch").toggle('slow', function () { });
        $("#tblOptions").toggle('slow', function () { });
        $.unblockUI();

    });


    //function for button search
    $("#btnSearch").click(function () {

        $('#fromDate').rules('add', {

            FromdateValidation: true,
            messages: {
                FromdateValidation: 'From Date must be less than or equal to To Date.'
            }
        });

        if ($("#listForm").valid()) {
            $('#PaymentList').jqGrid('GridUnload');

            var mode = "Search";

            loadPaymentGrid(mode);
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

        $('#PaymentList').jqGrid('GridUnload');
        loadPaymentGrid("view");

    });

    //$(window).bind('resize', function () {
    //    $("#PaymentList").jqGrid('destroyGroupHeader');
    //    if ($('#dvPaymentList').attr("id") !== undefined)
    //        $("#PaymentList").setGridWidth($('#dvPaymentList')[0].offsetWidth, true);
    //    grid_reconstruct_GroupHeaders();
    //});

    //new change done by Vikram for adding the changed month and year in session
    $("#months").change(function () {
        UpdateAccountSession($("#months").val(), $("#year").val());
    });

    $("#year").change(function () {
        UpdateAccountSession($("#months").val(), $("#year").val());
    });


});//document ready


//function grid_reconstruct_GroupHeaders() {
//    $("#PaymentList").jqGrid('setGroupHeaders', {
//        useColSpanStyle: true,
//        groupHeaders: [
//        { startColumnName: 'Edit', numberOfColumns: 2, titleText: 'Action' }

//        ]
//    });
//}

function DeletePayment(urlParam) {

    var Todelete = confirm('Are you sure you want to delete the master payment ?');

    if (Todelete) {

        blockPage();

        $.ajax({
            type: "POST",
            url: "/payment/DeletetMasterPaymentDetails/" + urlParam,
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
                    alert("Master Payment Deleted Successfuly.");

                    $("#lblBack").trigger('click');
                    $("#PaymentList").jqGrid().setGridParam({ url: '/Payment/GetMasterPaymentListJson/' }).trigger("reloadGrid");
                    return false;
                }
                else if (data.result == -1) {
                    alert("Finalized entry can not be deleted .");
                    return false;
                }
                else if (data.result == -3) {
                    alert("Payment Details can not be deleted as it is reconciled by bank");
                    return false;
                }
                else {

                    alert("Error while deleting Master Payment ");
                    return false;
                }
            }
        }); //end of ajax
    }

}

function EditPayment(urlParam) {
   
    //new change done by Vikram on 24 Jan 2014 for validating the transaction 
    ValidateCashChequeReciept(urlParam);
    if (isValid == true) {
        blockPage();
        $("#mainDiv").load('/Payment/MasterDetailsPaymentEdit/' + urlParam, function () {

            //added by Koustubh Nakate on 28/10/2013 to avoid async ajax call
            $.ajax({

                type: 'GET',
                url: '/Payment/PartialTransaction/' + Bill_ID,
                async: false,
                cache: false,
                success: function (data) {
                    unblockPage();
                    $('#PaymentDeductionData').html(data);

                    $("#TransactionForm").show('slow');

                    //show transaction form
                    $("#trnsShowtable").show('slow');



                    if (parseFloat(TotalAmtToEnterDedAmount) == 0) {
                        $("#AMOUNT_C").val(0).prop("readonly", 'readonly');
                        $("#cashAmtTr").hide();
                    }
                    else {
                        $("#AMOUNT_C").val(0).removeAttr("readonly");
                        $("#cashAmtTr").show();
                    }

                    setTimeout(function () {

                        $("#tblOptions").hide('slow');
                    }, 2000);

                    if ($("#HEAD_ID_P").is(':disabled')) {

                        $("#HEAD_ID_P").trigger('change');
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //$('#PaymentDeductionData').html('');
                    alert('Error while loading transaction details view.');
                    unblockPage();
                }


            });
            unblockPage();


            //load payment and deduction form
            /* $('#PaymentDeductionData').load('/Payment/PartialTransaction/' + Bill_ID, function () {
     
                 
                 $("#TransactionForm").show('slow');
     
                 //show transaction form
                 $("#trnsShowtable").show('slow');
     
                 
                
              
                 //new change done by Vikram -- as agreement dropdown was not shown at the time of save operation
                 if ($("#HEAD_ID_P").is(':disabled'))
                 {
                     
                     $("#HEAD_ID_P").trigger('change');
                 }
                 
                 //end of change
     
                 if (parseFloat(TotalAmtToEnterDedAmount) == 0) {
                     $("#AMOUNT_C").val(0).prop("readonly", 'readonly');
                     $("#cashAmtTr").hide();
                 }
                 else {
                     $("#AMOUNT_C").val(0).removeAttr("readonly");
                     $("#cashAmtTr").show();
                 }
     
                
                 setTimeout(function () {
     
                     $("#tblOptions").hide('slow');
                 }, 2000);
     
             });*/





        });
    }
    else {
        return false;
    }


}


//function to finalize the master payment details
function FinalizePayment(urlParam) {


    //added by vikram 28-8-2013 start
    getPaymentClosingBalanceFinalize($("#months").val(), $("#year").val());
    ValidateVoucherList(urlParam);

    if (isValid == false) {
        alert('Amount  must be less than or equal to Bank Authorization balance amount');


        return false;
    }
    //added by vikram 28-8-2013 end


    if (confirm("Are you sure you want to finalize the payment ? ")) {

        finalPay = false;

        $.ajax({
            type: "POST",
            url: "/payment/FinalizeVoucher/" + urlParam + '/' + finalPay,
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
                    $("#PaymentList").jqGrid().setGridParam({ url: '/Payment/GetMasterPaymentListJson/' }).trigger("reloadGrid");
                    alert("Voucher Finalized Successfully. ");
                    return false;
                }
                else if (data.Success == -1) {
                    alert("Voucher cant be Finalized as all transaction details are not entered.");
                    return false;
                }
                else if (data.Success == -2) {
                    alert("Voucher cant be Finalized as all transaction are not correct.");
                    return false;
                }
                else {
                    alert("Error while finalizing the voucher.");
                    return false;
                }
            }
        });
    }
}

function ViewPayment(urlParam) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


    $("#mainDiv").load('/Payment/MasterDetailsPaymentEdit/' + urlParam, function () {
        $("#TransactionForm").show('slow');
        $.unblockUI();

    });
}

var masterGridWidth = 0
function loadPaymentGrid(mode) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    //alert($('#fundtype').val());
    jQuery("#PaymentList").jqGrid({

        url: '/Payment/GetMasterPaymentListJson/',
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: 10,
        postData: { 'mode': mode, 'months': $('#months').val(), 'year': $('#year').val(), 'fromDate': $('#fromDate').val(), 'toDate': $('#toDate').val(), 'transType': $("#TXN_ID").val(), 'Chq_Epay': $("#Chq_Epay").val() },
        rownumbers: true,
        //width:'100%',
        autowidth: true,
        //pginput: false,
        //forceFit:true,
        //shrinkToFit: true,
        rowList: [10, 20, 30],
        onPaging: function (pgButton) {
            masterGridWidth = jQuery("#PaymentList").parent().height();
        },
        loadComplete: function (xhr, st, err) {
            $.unblockUI();

            $("#PaymentList").jqGrid('setLabel', "rn", "Sr.</br> No");
            //// if (masterGridWidth != 0)
            {
                $("#PaymentList").parents('div.ui-jqgrid-bdiv').css("max-height", "425px");
            }


        },
        colNames: ['Voucher Number', 'Voucher Date', 'Cash/Cheque', 'Transaction Type', 'Cheque / Epayment / Advice Number', 'Cheque / Epayment / Advice Date', 'Contractor/Payee Name', 'Agreement Number', 'Cheque Amount (In Rs.)', 'Cash Amount (In Rs.)', 'Gross Amount (In Rs.)', 'Edit', 'Delete', 'Finalize', 'Epayment</br>Eremittance</br>Order', 'Cancel /</br>Renew Cheque /</br>Advice ', 'Cheque </br> Acknowledged', 'Entry</br>Status', 'PFMS Status', 'PFMS Remarks'],
        colModel: [

                {
                    name: 'Voucher_Number',
                    index: 'Voucher_Number',
                    width: 50,
                    align: "center"
                    // frozen: true
                },
                {
                    name: 'voucher_date',
                    index: 'auth_sig_name',
                    width: 80,
                    align: "center"
                    // frozen: false,
                },
                {
                    name: 'Cash_Cheque',
                    index: 'Cash_Cheque',
                    width: 0,
                    align: "center",
                    // frozen: false,
                    hidden: true
                },

                {
                    name: 'Transaction_type',
                    index: 'Transaction_type',
                    width: 120,
                    align: "left",
                    //frozen: true,
                    // hidden: true
                },
                {
                    name: 'cheque_number',
                    index: 'cheque_number',
                    width: 100,
                    align: "left"
                },
                {
                    name: 'cheque_Date',
                    index: 'cheque_Date',
                    width: 65,
                    align: "Center"
                }, {
                    name: 'Payee_Name',
                    index: 'Payee_Name',
                    width: 130,
                    align: "left"
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
                    width: 80,
                    align: "right"

                },
                {
                    name: 'Gross_Amount',
                    index: 'Gross_Amount',
                    width: 80,
                    align: "right"
                },
                {
                    name: 'Edit',
                    index: 'Edit',
                    width: 50,
                    sortable: false,
                    align: "Center"
                },
                {
                    name: 'Delete',
                    index: 'Delete',
                    width: 50,
                    sortable: false,
                    align: "Center"

                },
                {
                    name: 'Finalize',
                    index: 'Finalize',
                    width: 44,
                    sortable: false,
                    align: "Center"



                },

                {
                    name: 'ViewEPay',
                    index: 'ViewEPay',
                    width: 55,
                    sortable: false,
                    align: "Center"


                },

                {
                    name: 'Renew',
                    index: 'Renew',
                    width: 50,
                    align: "center",
                    sortable: false,
                    //    hidden: (levelId == 4 ? true : false)//Commented By Abhishek kamble To provide cheque renewal provision at SRRDA level 23-July-2014                  
                },

                {
                    name: 'Ack',
                    index: 'Ack',
                    width: 40,
                    align: "center",
                    sortable: false,
                    hidden: (levelId == 4 ? true : false)
                },
                {
                    name: 'IsCorrectEntry',
                    index: 'IsCorrectEntry',
                    width: 60,
                    align: "center",
                    sortable: false
                    }
                    ,
                {
                    name: 'PFMSStatus',
                    index: 'PFMSStatus',
                    width: 60,
                    align: "center",
                    sortable: false,
                    hidden: ($('#fundtype').val() == 'P') ? false : true, 
                },
                {
                    name: 'PFMSRemarks',
                    index: 'PFMSRemarks',
                    width: 150,
                    align: "center",
                    sortable: false,
                    hidden: ($('#fundtype').val() == 'P') ? false : true,
                }

        ],
        pager: "#pager",
        viewrecords: true,
        loadError: function (xhr, st, err) {
            unblockPage();
            $.unblockUI();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },

        sortname: 'Voucher_Number,voucher_date',
        sortorder: "asc",
        caption: "Payment Details"
    });


    //jQuery("#").jqGrid('setFrozenColumns');
    //jQuery("#PaymentList").jqGrid('setGroupHeaders', {
    //    useColSpanStyle: true,
    //    groupHeaders: [
    //      { startColumnName: 'Edit', numberOfColumns: 2, titleText: 'Action' }

    //    ]
    //});
}


//function to renew cheque
function RenewCheque(urlParam) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    //If else condition commneted by Abhishek kamble to allow cheque renewal at SRRDA level also 23-July-2014
    //if (levelId == 5)
    //{        
    $("#PaymentList").jqGrid('setGridState', 'hidden');
    $("#tblSearch").hide('slow', function () { });
    $("#RenewChqDiv").load("/payment/GetChequeRenew/" + urlParam, function () {
        $.unblockUI();
        return false;
    });
    //}
    //else {
    //    //if SRRDA dont provide facility to cheque cancel renewal (date 10/07/2012 ); Reason unspecified
    //    $.unblockUI();
    //    return false;
    //}
}


//function to show epayment order 
function ViewEpayOrder(urlParam) {

    //$.ajax({
    //    type: "POST",
    //    url: "/payment/GetEpaymentOrderDetails/" + urlParam,
    //    //async: false,
    //    error: function (xhr, status, error) {
    //        unblockPage();

    //        $('#errorSpan').text(xhr.responseText);
    //        $('#divError').show('slow');
    //        $('#errorSpan').show();
    //        return false;
    //    },
    //    success: function (data) {
    //        unblockPage();           
    //        $('#divError').hide('slow');
    //        $('#errorSpan').html("");
    //        $('#errorSpan').hide();

    //        if (data.Success) {

    //            $("#EmailRecepient").text(data.EmailRecepient).css("font-weight", "bold");
    //            $("#DPIUName").text(data.DPIUName).css("font-weight", "bold");
    //            $("#STATEName").text(data.STATEName).css("font-weight", "bold");
    //            $("#EmailDate").text(data.EmailDate).css("font-weight", "bold");
    //            $("#Bankaddress").text(data.Bankaddress).css("font-weight", "bold");
    //            $("#BankAcNumber").text(data.BankAcNumber).css("font-weight", "bold");
    //            $("#EpayNumber").text(data.EpayNumber).css("font-weight", "bold");
    //            $("#EpayDate").text(data.EpayDate).css("font-weight", "bold");
    //            $("#EpayState").text(data.EpayState).css("font-weight", "bold");
    //            $("#EpayDPIU").text(data.EpayDPIU).css("font-weight", "bold");
    //            $("#EpayVNumber").text(data.EpayVNumber).css("font-weight", "bold");
    //            $("#EpayVDate").text(data.EpayVDate).css("font-weight", "bold");
    //            $("#EpayVPackages").text(data.EpayVPackages).css("font-weight", "bold");
    //            $("#EpayConName").text(data.EpayConName).css("font-weight", "bold");
    //            $("#EpayConAcNum").text(data.EpayConAcNum).css("font-weight", "bold");
    //            $("#EpayConBankName").text(data.EpayConBankName).css("font-weight", "bold");
    //            $("#EpayConBankIFSCCode").text(data.EpayConBankIFSCCode).css("font-weight", "bold");
    //            $("#EpayAmount").text(data.EpayAmount).css("font-weight", "bold");
    //            $("#EpayNo").text(data.EpayNumber).css("font-weight", "bold");
    //            $("#EpayAmountInWord").text(data.EpayAmountInWord).css("font-weight", "bold");
    //            $("#AuthSignName").text(data.AuthSignName).css("font-weight", "bold");
    //            $("#AuthSignPhoneNumber").text(data.AuthSignPhoneNumber).css("font-weight", "bold");

    //            //Added by Abhishek kamble 29-May-2014                
    //            if (data.EpayContLegalHeirName == "" || data.EpayContLegalHeirName == null) {
    //                $("#trContLegalHeirDetails").hide();
    //            }
    //            else {
    //                $("#trContLegalHeirDetails").show();
    //            }
    //            $("#EpayConLegalHeirName").text(data.EpayContLegalHeirName).css("font-weight", "bold");


    //            //Flag to to disp watermark for epayment 1-Jul-2014
    //            epaymentFinalizedByAuthSig = data.FinalizedByAuthSig;

    //            //Added by Abhishek kamble 1-Jul-2014 to show/hide package row from dialog box
    //            //47,737 - contractor work payment
    //            //if ((data.EpayMasterTxnID != 47) && (data.EpayMasterTxnID != 737)) {
    //            //    $("#trPackageNumber").hide();
    //            //}
    //            //else {
    //            //    $("#trPackageNumber").show();
    //            //}
    //            $("#dialog").dialog("open");

    //            return false;
    //        }
    //     }
    //});

    $.ajax({
        type: "POST",
        url: "/payment/GetEpaymentDetailsForSigning/" + urlParam + '#' + Math.random(),
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
function ViewEremOrder(urlParam) {

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

//Added by vikram 28-8-2013 start

function getPaymentClosingBalanceFinalize(month, year) {
    $.ajax({
        type: "POST",
        url: "/payment/GetClosingBalanceForPayment/" + month + "$" + year,
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
            if (data != "") {
                balanceCashAmount = data.Cash;
                balanceChequeAmount = data.BankAuth;
                if (data.BankAuth == '0') {
                    isValid = false;
                }
                else {
                    isValid = true;
                }

                $("#lblCash").text(parseFloat(data.Cash).toFixed(2));
                $("#lblBank").text(parseFloat(data.BankAuth).toFixed(2));
            }
        }
    });
}
function ValidateVoucherList(billId) {
    $.ajax({
        type: "POST",
        url: "/payment/GetVoucherPayment/" + billId,
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
            if (data != '') {
                voucherPayment = data.Payment;
                paymentType = data.PaymentType;
                if (paymentType === "C") {
                    if (voucherPayment <= balanceCashAmount) {
                        isValid = true;
                    }
                    else {
                        isValid = false;
                    }
                }
                else if (paymentType === "Q") {
                    if (voucherPayment <= balanceChequeAmount) {
                        isValid = true;
                    }
                    else {
                        isValid = false;
                    }
                }

            }
        }
    });
}

//Added by vikram 28-8-2013 end

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


//Added By Abhishek kamble 3-jan-2014
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
                //$("#TrAccountStatus").hide();
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



//function for validation of transaction that mismatch with the screen design parameters
function ValidateCashChequeReciept(urlParam) {
    blockPage();
    $.ajax({
        type: "POST",
        url: "/Receipt/IsValidTransaction/" + urlParam,
        cache: false,
        async: false,
        error: function (xhr, status, error) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            isValid = false;
            return false;
        },
        success: function (data) {
            unblockPage();
            if (data.success == true) {
                isValid = true;
            }
            else if (data.success == false) {
                alert(data.message);
                isValid = false;
            }
        },
    });
}

var windowSummary;

var epaymentFinalizedByAuthSig;
var eremittanceFinalizedByAuthSig;

//Added By Abhishek kamble 26-May-2014
function PrintEpaymentDetails(elem) {
    //var size = $(elem).css('font-size');
    PrintEpayPopup($(elem).html());

}

function PrintEpayPopup(data) {

    //alert();

    if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {   // Chrome Browser Detected?

        windowSummary = window.open('', 'Epayment', 'height=700,width=700');

        windowSummary.document.write('<html><head><title></title>');
        windowSummary.document.write('<style type="text/css">#first td, th{border:1px solid black;}#first {border-collapse:collapse;}th.ui-th-column div {white-space: normal !important;height: auto !important;padding: 2px;}</style>');
        windowSummary.document.write('</head><body>');
        windowSummary.document.write('<h3 style="text-align:center">Epayment Details</h3>');
        windowSummary.document.write(data);

        if (epaymentFinalizedByAuthSig == "N") {
            windowSummary.document.write('<h4 style="text-align:left"> This Voucher is not finalize by Authorized Signatory.</h4>');
        }

        windowSummary.document.write('</head><body>');

        windowSummary.PPClose = false;                                     // Clear Close Flag
        windowSummary.onbeforeunload = function () {                         // Before Window Close Event
            if (windowSummary.PPClose === false) {                           // Close not OK?
                return 'Leaving this page will block the parent window!\nPlease select "Stay on this Page option" and use the \nCancel button instead to close the Print Preview Window.\n';
            }
        }
        windowSummary.print();
        windowSummary.PPClose = true;                                      // Set Close Flag to OK.
    }
    else {

        var windowSummary = window.open('', 'Epayment', 'height=700,width=700');
        windowSummary.document.write('<html><head><title></title>');
        windowSummary.document.write('<style type="text/css">#first td, th{border:1px solid black;}#first {border-collapse:collapse;}th.ui-th-column div {white-space: normal !important;height: auto !important;padding: 2px;}</style>');

        windowSummary.document.write('</head><body>');
        //background = "/Content/images/Draft.png"

        windowSummary.document.write('<h3 style="text-align:center">Epayment Details</h3>');
        windowSummary.document.write(data);

        if (epaymentFinalizedByAuthSig == "N") {
            windowSummary.document.write('<h4 style="text-align:left"> This Voucher is not finalize by Authorized Signatory.</h4>');
        }

        windowSummary.document.write('</body></html>');

        windowSummary.print();
        windowSummary.close();
        //return true;
    }




}

//Added By Abhishek kamble 26-May-2014
function PrintEremDetails(elem) {
    //var size = $(elem).css('font-size');
    PrintERemPopup($(elem).html());
}

function PrintERemPopup(data) {
    //alert(data);


    if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {   // Chrome Browser Detected?


        var windowSummary = window.open('', 'ERemittace', 'height=800,width=700');

        windowSummary.document.write('<html><head><title>E-Remittace details</title>');
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

    }
    else {
        var windowSummary = window.open('', 'ERemittace', 'height=800,width=700');

        windowSummary.document.write('<html><head><title>E-Remittace details</title>');
        windowSummary.document.write('<style type="text/css">td, th{border:1px solid black;}#first {border-collapse:collapse;}th.ui-th-column div {white-space: normal !important;height: auto !important;padding: 2px;}</style>');

        windowSummary.document.write('</head><body>');

        windowSummary.document.write('<h3 style="text-align:center">E-Remittance Details</h3>');

        windowSummary.document.write(data);


        windowSummary.document.write('</body></html>');
        windowSummary.print();

    }

    //windowSummary.close();

    return true;
}