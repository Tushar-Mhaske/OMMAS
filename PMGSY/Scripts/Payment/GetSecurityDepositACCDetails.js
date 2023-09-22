
/// <reference path="../jquery-1.9.1.js" />
/// <reference path="../jquery-1.9.1.min.js" />
/// <reference path="../jquery-1.7.2.intellisense.js" />

$("#divGridGetSecurityDepositACCDetails").click(function () {

    if ($("#divGetSecurityDepositACCDetailsData").is(":visible")) {

        $("#closeGridId").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

        $(this).next("#divGetSecurityDepositACCDetailsData").slideToggle(300);
    }

    else {
        $("#closeGridId").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

        $(this).next("#divGetSecurityDepositACCDetailsData").slideToggle(300);

        GeneratevoucherNo(); //call To function 
        GetEpayNumber();  //call To function

        //window.location.pathname = "PaymentController/";
    }
});

/*const date = new Date();
let month = date.getMonth() + 1;
let year = date.getFullYear();*/

//to generate Voucher Number
function GeneratevoucherNo() {

    $.ajax({
        type: "POST",
        url: "/payment/GenerateVoucherNo/V$" + $("#BILL_MONTH").val() + '$' + $("#BILL_YEAR").val(),
        //url: "/payment/GenerateVoucherNo/V$" + month + '$' + year,
        async: false,
        // data: $("#authSigForm").serialize(),
        error: function (xhr, status, error) {
            //unblockPage();
            $('#divError').show('slow');
            $('#errorSpan').text(xhr.responseText);

        },
        success: function (data) {
            unblockPage();
            $('#divError').hide('slow');
            $('#errorSpan').html("");
            if (data != "") {
                $("#BILL_NO").val("");
                $("#BILL_NO").val(data.strVoucherNumber);
                $("#BILL_NO").attr('readonly', true);
            }
        }
    });
}

//get epay number
function GetEpayNumber() {
    $.ajax({
        type: "POST",
        //url: "/payment/GetEpayNumber/" + month + '$' + year,
        url: "/payment/GetEpayNumber/" + $("#BILL_MONTH").val() + '$' + $("#BILL_YEAR").val(),
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
            $('#CHQ_NO').hide();
            $('#EPAY_NO').val(data).attr("readonly", "readonly");
            $('#EPAY_NO').show();
        }
    });

}


//function to fill the dropdownbox  dynamically
function FillInCascadeDropdown(map, dropdown, action) {
    // alert('a' + dropdown);
    $(dropdown).empty();
    $.post(action, map, function (data) {

        $.each(data, function () {
            // alert('TEST' + this.Selected);
            // alert("fillCascaded =" + this.Value);
            if (this.Selected == true) { $(dropdown).append("<option value='" + this.Value + "' selected =" + this.Selected + ">" + this.Text + "</option>"); }
            else { $(dropdown).append("<option value='" + this.Value + "'>" + this.Text + "</option>"); }
        });
    }, "json");
}

//load bank details
function getContratorBankDetails(mastConID) {

    $("#spnBankName").html('');
    $("#spnIFSCCode").html('');
    $("#spnBankAccNumber").html('');

    if (mastConID != 0 || mastConID != "") {
        // alert("ABC");       
        FillInCascadeDropdown(null, "#CONC_Account_ID", "/Payment/GetContratorBankNameAccNoAndIFSCcode/" + mastConID + '$' + "P" + '$' + "3199" + '$' + false + '$' + false);
        //    $("#trContractorBankDetails").show();
        //alert("pqr");
    }
    else {

    }
}


$("#MAST_CON_ID_C").change(function () {
    $("#spnBankAccNumber").html("-");
    $("#spnIFSCCode").html("-");
    $("#spnBankName").html("-");

    //alert("changed")
    if (($("#MAST_CON_ID_C").val() != 0 && $("#MAST_CON_ID_C").val() != "")) {
        getContratorBankDetails($("#MAST_CON_ID_C").val());
    }
});

//set IFSCCODE & AC Number
$('#CONC_Account_ID').change(function () {
    //alert($("#MAST_CON_ID_C").val())
    // if ($("#MAST_CON_ID_C").html() != 0) {

    if ($("#CONC_Account_ID").val() > 0) {
        var arr = $("#CONC_Account_ID option:selected").text().split(':');

        $("#spnBankName").html(arr[0]);
        $("#spnIFSCCode").html(arr[1]);
        $("#spnBankAccNumber").html(arr[2]);
    }
    else {
        $("#spnBankAccNumber").html("-");
        $("#spnIFSCCode").html("-");
        $("#spnBankName").html("-");
    }

    //PFMS Validations
    // $('#conAccountId').val($('#CONC_Account_ID').val());
    /*    } else {
            $("#spnBankName").html('');
            $("#spnIFSCCode").html('');
            $("#spnBankAccNumber").html('');
        }*/
    /*    if ($("#CONC_Account_ID").html() == "---Select Account---") {
            alert("hii ---Select Account---")
            $("#spnBankAccNumber").html("-");
            $("#spnIFSCCode").html("-");
            $("#spnBankName").html("-");
        }
        else {
            alert("in else")
        }*/
});

//reset Form
function resetMasterFormNew() {
    $("#TOTAL_AMOUNT").val('');
    $('#MAST_CON_ID_C').val('').trigger('chosen:updated');
    $('#CONC_Account_ID').val($('#CONC_Account_ID option:first').val());
    $("#spnBankName").html('-');
    $("#spnIFSCCode").html('-');
    $("#spnBankAccNumber").html('-');
}

//reset form conformation ?
$("#btnReset").click(function () {
    if (confirm("Do You Want To Reset Form?")) {
        resetMasterFormNew();
    }
});

//function add master details
$("#btnSubmit").click(function (e) {
    $.validator.unobtrusive.parse("#masterPaymentFormAcc");

    //alert($("#MAST_CON_ID_C").val())
    if ($("#MAST_CON_ID_C").val() == 0) {
        alert("Select Company Name (Contractor)")
        $("#MAST_CON_ID_C").focus();
        return false;
    }

    //alert($("#masterPaymentFormAcc").serialize())
    e.preventDefault();
    if ($("#masterPaymentFormAcc").valid()) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

        $.ajax({
            type: "POST",
            url: "/payment/AddSecurityDepositAccOpeningBalance/",
            async: true,
            data: $("#masterPaymentFormAcc").serialize(),
            error: function (xhr, status, error) {
                alert("Some Error Occured. Please Try After Some Time")
                $.unblockUI();
                return false;
            },
            success: function (data) {
                //alert(data);

                console.log(data);

                //alert("In sucess")
                /*                if (data == undefined) {
                                    alert(data.message);
                                }*/
                if (data.Success == true) {
                    //unblockPage();
                    alert("Security Deposit Account Opening Balance Entry Done Successfully")

                    $("#PaymentList").trigger("reloadGrid");
                    loadPaymentGrid();
                    resetMasterFormNew()
                    $("#closeGridId").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

                    $("#divGetSecurityDepositACCDetailsData").hide(300);

                    //18-05-2023        --- chnges suggested after testing                 
                    $("#divGetSecurityDepositACCDetailsData").hide();
                    $("#divGridGetSecurityDepositACCDetails").hide();


                    //$("#listId").show("slow");

                    $.unblockUI();
                }
                /*                else if (data.Success == false && data.output == "-1")
                                {
                                    alert("Contractor bank details are not present.");
                                    return false;
                                }
                */
                else if (data.Success == false && data.Result == -1) {
                    alert(data.message); //Only One Entry For SRRDA Is Possible

                    //$("#errorSpan").html("<h3>Security Deposit Account Opening Balance Entry Already Exist</h3>")
                    // $("#divError").show();
                    $.unblockUI();
                }
                else if (data.Success == false && data.Bill_ID == "-5") {
                    alert("Contractor bank details are not present.");
                    return false;
                }
                else {
                    alert("Some Error Occured While Saving Data. Please Try After Some Time")
                    $.unblockUI();
                }
            }
        });
    }
});


var masterGridWidth = 0
function loadPaymentGrid() {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#PaymentList").jqGrid({

        url: '/Payment/GetSecurityDepositAccOpeningBalanceJson/',
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: 10,
        postData: { 'month': $("#BILL_MONTH").val(), 'year': $("#BILL_YEAR").val() },
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
            var windWidth = window.innerWidth;
            var grid = $("#PaymentList");
            grid.setGridWidth(windWidth - 100);
            $.unblockUI();

            $("#PaymentList").jqGrid('setLabel', "rn", "Sr.</br> No");
            //// if (masterGridWidth != 0)
            /*            {
                            $("#PaymentList").parents('div.ui-jqgrid-bdiv').css("max-height", "425px");
                        }*/



        },
        //colNames: ['Voucher Number', 'Voucher Date', 'Transaction Type', 'Cheque / Epayment / Advice Number', 'Cheque / Epayment / Advice Date', 'Contractor/Payee Name', 'Agreement Number', 'Total Amount (In Rs.)', 'Gross Amount (In Rs.)', 'Edit', 'Delete', 'Finalize', 'Epayment</br>Eremittance</br>Order', 'Cancel /</br>Renew Cheque /</br>Advice ', 'Cheque </br> Acknowledged', 'Entry</br>Status', 'PFMS Status', 'PFMS Remarks'],
        //colNames: ['Voucher Number', 'Voucher Date', 'Transaction Type', 'Cheque / Epayment / Advice Number', 'Voucher Date', 'Contractor/Payee Name', 'Total Amount (In Rs.)', 'Gross Amount (In Rs.)' ],
        colNames: ['Voucher Number', 'Voucher Date', 'Cheque / Epayment / Advice Number', 'Contractor/Payee Name', 'Total Amount (In Rs.)', 'Gross Amount (In Rs.)', 'Finalize'],
        colModel: [

            {
                name: 'Voucher_Number',
                index: 'Voucher_Number',
                width: 100,
                align: "center"
                // frozen: true
            },
            {
                name: 'voucher_date',
                index: 'auth_sig_name',
                width: 100,
                align: "center"
                // frozen: false,
            },

            {
                name: 'cheque_number',
                index: 'cheque_number',
                width: 100,
                align: "left"
            },

            {
                name: 'Payee_Name',
                index: 'Payee_Name',
                width: 130,
                align: "left"
            },

            {
                name: 'total_amount',
                index: 'total_amount',
                width: 100,
                align: "center"

            },

            {
                name: 'Gross_Amount',
                index: 'Gross_Amount',
                width: 100,
                align: "center"
            },
            {
                name: 'Finalize',
                index: 'finalize',
                width: 100,
                align: "center"
            },


        ],
        index: "Voucher Number",
        pager: "#pager",
        viewrecords: true,
        loadError: function (xhr, st, err) {
            unblockPage();
            $.unblockUI();
            /*            $('#errorSpan').text(xhr.responseText);
                        $('#divError').show('slow');*/
            return false;
        },


        //sortname: 'Voucher_Number,voucher_date',
        sortname: "Voucher_Date",
        sortorder: "desc",
        caption: "Security Deposit Account Opening Balance Entry Data List"
    });


    //jQuery("#").jqGrid('setFrozenColumns');
    //jQuery("#PaymentList").jqGrid('setGroupHeaders', {
    //    useColSpanStyle: true,
    //    groupHeaders: [
    //      { startColumnName: 'Edit', numberOfColumns: 2, titleText: 'Action' }

    //    ]
    //});
}

/*function ViewPayment(id) {
    alert("view")
}*/

//function to finalize the master payment details
function FinalizePayment(urlParam) {


    //added by vikram 28-8-2013 start
    // getPaymentClosingBalanceFinalize($("#months").val(), $("#year").val());
    //ValidateVoucherList(urlParam);

    /*    if (isValid == false) {
            alert('Amount  must be less than or equal to Bank Authorization balance amount');
    
    
            return false;
        }*/
    //added by vikram 28-8-2013 end


    if (confirm("Are you sure you want to finalize the payment ? ")) {

        finalPay = false;

        $.ajax({
            type: "POST",
            url: "/payment/FinalizeVoucher/" + urlParam + '/' + finalPay,
            //async: false,
            error: function (xhr, status, error) {

                alert(xhr.responseText)
                unblockPage();

                return false;
            },
            success: function (data) {
                unblockPage();

                if (data.Success == 1) {
                    $("#PaymentList").jqGrid().setGridParam({ url: '/Payment/GetSecurityDepositAccOpeningBalanceJson/' }).trigger("reloadGrid");
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

function closeDivError() {
    $('#divError').hide('slow');
}

/*function process(date) {
    //alert("process" + date)
    var parts = date.split("/");
    return new Date(parts[2], parts[1] - 1, parts[0]);
}
*/

//on page load function will call
$(document).ready(function () {
    //alert("ready");
    $('#MAST_CON_ID_C').chosen(); //make serchable dropdown
    //$('#CONC_Account_ID').chosen();//make serchable dropdown

    GeneratevoucherNo(); //call To function 
    GetEpayNumber();  //call To function

    loadPaymentGrid()// to show listing of voucher entry's
    //$.validator.setDefaults({ ignore: ":hidden:not(.chosen-select)" }) //for all select having class .chosen-select



    //alert("ready end");
});



