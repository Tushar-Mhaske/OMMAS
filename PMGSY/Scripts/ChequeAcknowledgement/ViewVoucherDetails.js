$(document).ready(function ()
{   
    //Added by Ashish Markande on 21/08/2013
    $("#btnView").click(function () {
        if ($("#frmViewDetails").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $("#tbListVouchers").jqGrid('GridUnload');

            LoadVoucherDetailsList();


            //$.ajax({
            //    url: "/ChequeAcknowledgement/CheckMonthClose",
            //    type: "POST",
            //    data: { BILL_MONTH: $("#BILL_MONTH option:selected").val(), BILL_YEAR: $('#BILL_YEAR option:selected').val(), DPIU: $("#DPIU option:selected").val() },
            //    success: function (data) {
            //        $.unblockUI();

            //        if (data.success == true) {
            //            LoadVoucherDetailsList();
            //        }
            //        else if (data.success == false) {
            //            if (data.message != "") {
            //                alert(data.message);
            //            }
            //        }
            //    },
            //    error: function (xhr, ajaxOptions, thrownError) {
            //        $.unblockUI();
            //        alert(xhr.responseText);
            //        $.unblockUI();
            //    }
            //});
        }
        $.unblockUI();
    });

    //Added by Ashish Markande on 21/08/2013
    $("#btnAddVoucher").click(function () {
        var AddMode = "Y";
        AddNewVoucherDetails(AddMode);
     
    });

    //$('#BILL_MONTH').change(function () {
    //    GetClosedMonthYear();
    //});

});
function LoadVoucherDetailsList() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#tbListVouchers").jqGrid({
        url: '/ChequeAcknowledgement/ListVoucherDetails',
        datatype: "json",
        mtype: "POST",
        postData: { BILL_MONTH: $("#BILL_MONTH option:selected").val(), BILL_YEAR: $('#BILL_YEAR option:selected').val(), DPIU: $("#DPIU option:selected").val(), ACC_TYPE: $("#ACCOUNT_TYPE option:selected").val() },
        colNames: ['Cheque/Epay Number', 'Cheque/Epay Date', 'Month-Year', 'Account Type', 'Payee Name', 'Amount(In Rs.)', 'Finalize', 'Action'],
        colModel: [
            { name: 'BILL_NO', index: 'BILL_NO', height: 'auto', width: 80, align: "center", search: false },
            { name: 'Bill_Date', index: 'Bill_Date', height: 'auto', width: 80, align: "left", search: false },
            { name: 'M_Year', index: 'M_Year', height: 'auto', width: 100, align: "left", search: false },
            { name: 'Acc_Type', index: 'Acc_Type', height: 'auto', width: 80, align: "left", search: false },
            { name: 'ADMIN_ND_NAME', index: 'ADMIN_ND_NAME', height: 'auto', width: 250, align: "left", search: true },
            { name: 'Amount', index: 'Amount', height: 'auto', width: 100, align: "right", search: false },
            { name: 'Finalize', width: 50, sortable: false, resize: false, align: "center", search: false },
            { name: 'Action', width: 70, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#dvpagerListVouchers').width(20),
        rowNum: 20,
        rowList: [20, 40, 60],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "BILL_NO",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Acknowledged Voucher List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        loadComplete: function (data) {
            $.unblockUI();

            $("#tbListVouchers").jqGrid('setLabel', "rn", "Sr.</br> No");
        },
        loadError: function (xhr, ststus, error) {
            $.unblockUI();

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });

}
function ViewVoucherDetails(billNumber) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#divVoucherView").load('/ChequeAcknowledgement/GetChequeAckMasterList?' + $.param({ Month: $("#BILL_MONTH option:selected").val(), Year: $("#BILL_YEAR option:selected").val(), DPIU: $("#DPIU option:selected").val(), BillNo: billNumber, ACC_TYPE: $("#ACCOUNT_TYPE option:selected").val() }), function () {
        $.unblockUI();

        $("#btnViewDetails").trigger('click');
        $("#BILL_NO").attr('disabled', true);
        $("#DPIU").attr('disabled', true);
        $("#BILL_MONTH").attr('disabled', true);
        $("#BILL_YEAR").attr('disabled', true);
        $("#btnViewDetails").hide('slow');
        // Added by Srishti on 08-03-2023
        $("#ACCOUNT_TYPE").attr('disabled', true);
    });

}

//Added by Ashish Markande on 21/08/2013
function AddNewVoucherDetails(AddMode) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#divVoucherView").html('');
    $("#divVoucherView").load('/ChequeAcknowledgement/GetChequeAckMasterList?' + $.param({ Month: $("#BILL_MONTH option:selected").val(), Year: $("#BILL_YEAR option:selected").val(), DPIU: $("#DPIU option:selected").val(), ACC_TYPE: $("#ACCOUNT_TYPE option:selected").val(), Mode: AddMode, AckUnackFlag: "A" }), function () {

        // $("#btnViewDetails").trigger('click');
        $("#BILL_NO").attr('disabled', false);
        $("#DPIU").attr('disabled', false);
        $("#BILL_MONTH").attr('disabled', false);
        $("#BILL_YEAR").attr('disabled', false);
        $.unblockUI();


    });

}
function AddVoucherDetails(billNumber) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#divVoucherView").load('/ChequeAcknowledgement/GetChequeAckMasterList?' + $.param({ Month: $("#BILL_MONTH option:selected").val(), Year: $("#BILL_YEAR option:selected").val(), DPIU: $("#DPIU option:selected").val(), BillNo: billNumber, AckUnackFlag: "A", ACC_TYPE: $("#ACCOUNT_TYPE option:selected").val() }), function () {
        $("#btnViewDetails").trigger('click');
        // Added by Srishti on 09-03-2023
        $("#BILL_NO").attr('disabled', true);
        $("#DPIU").attr('disabled', true);
        $("#BILL_MONTH").attr('disabled', true);
        $("#BILL_YEAR").attr('disabled', true);
        $("#ACCOUNT_TYPE").attr('disabled', true);
        $("#btnViewDetails").hide();
        $.unblockUI();

    });

}

function UnAcknowledgeVoucherDetails(billNumber) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#divVoucherView").load('/ChequeAcknowledgement/GetChequeAckMasterList?' + $.param({ Month: $("#BILL_MONTH option:selected").val(), Year: $("#BILL_YEAR option:selected").val(), DPIU: $("#DPIU option:selected").val(), BillNo: billNumber, AckUnackFlag: "U", ACC_TYPE: $("#ACCOUNT_TYPE option:selected").val() }), function () {
        $("#btnViewDetails").trigger('click');
        // Added by Srishti on 09-03-2023
        $("#BILL_NO").attr('disabled', true);
        $("#DPIU").attr('disabled', true);
        $("#BILL_MONTH").attr('disabled', true);
        $("#BILL_YEAR").attr('disabled', true);
        $("#ACCOUNT_TYPE").attr('disabled', true);
        $("#btnViewDetails").hide();
        $.unblockUI();
    });

}

function DeleteChqAckVoucherDetails(billNumber) {


    if (confirm("Are you sure you want to delete Cheque Acknowledgement Vocher Details?")) {


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        //Call Action
        $.ajax({
            type: 'POST',
            //url: '/Master/AddInfoDetails?',
            url: '/ChequeAcknowledgement/DeleteChequeAckVocherDetails?' + $.param({ Month: $("#BILL_MONTH option:selected").val(), Year: $("#BILL_YEAR option:selected").val(), DPIU: $("#DPIU option:selected").val(), BillNo: billNumber }),
            //url: '/ChequeAcknowledgement/DeleteChequeAckVocherDetails/',
            async: false,
            //data: { Month: $("#BILL_MONTH option:selected").val(), Year: $("#BILL_YEAR option:selected").val(), DPIU: $("#DPIU option:selected").val(), BillNo: billNumber },
            success: function (data) {
                if (data.status == true) {
                    alert(data.message);
                    $('#tbListVouchers').trigger('reloadGrid');//Reload Ggrid                
                    $.unblockUI();
                }
                else if (data.status == false) {
                    alert(data.message);
                    //$('#message').html(data.message);
                    //$('#dvErrorMessage').show('slow');
                    $.unblockUI();
                }
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                $.unblockUI();
                alert("Error occured while processing your request.");
                return false;
            }
        })//Ajax End

    }
}

function GetClosedMonthYear() {
    blockPage();

    $.ajax({
        type: "GET",
        url: "/MonthlyClosing/GetClosedMonthYear/",
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
            console.log('monthClosed: ' + data.monthClosed);
            if (data.monthClosed) {
                $('#btnAddVoucher').hide('slow');
                return false;
            }
            else if (data.monthClosed == false) {
                $('#btnAddVoucher').show('slow');
                return false;
            }
            else {

                alert("Error While getting Monthly Closing Details");
                return false;
            }

        }
    });
}