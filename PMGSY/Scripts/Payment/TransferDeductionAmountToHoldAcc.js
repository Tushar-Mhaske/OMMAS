/*
This file is used for finlization and display of the eremittaqnce and eapyment

*/


var isAutoLoad = "Y";
$(document).ready(function () {
    $("#transfer").dialog({
        autoOpen: false,
        // height:550,
        width: 1050,
        modal: true,
        title: 'Transfer Deduction Amount to Holding Account',
        show: {
            effect: "blind",
            duration: 1000
        },
        hide: {
            effect: "explode",
            duration: 1000
        }

    });

    $("#vDate").datepicker({
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

    $('#btnClose').click(function () {

        $("#transfer").dialog('close');
    });

   



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
      
   
            $('#PaymentList').jqGrid('GridUnload');

            var mode = "Search";

            loadPaymentGrid(mode);
        
    });

    
    //function for view details button click
   
    if (isAutoLoad == "Y") {
        // alert(isAutoLoad);
        loadPaymentGrid("view");
        //  $('#PaymentListGeneratedVouchers').jqGrid('GridUnload');


    }


   






});//document ready
$("#btnViewSubmit").click(function () {
    //if ($('#months').val() == 0) {
    //    alert("please select month");
    //    return false;
    //}

    //if ($('#year').val() == 0) {
    //    alert("please select year");
    //    return false;
    //}
    $('#btnGenerateVoucher').show('slow');
    isAutoLoad = "N";
    if ($("#listForm").valid()) {
        //  $('#PaymentList').jqGrid('GridUnload');
        // alert("1");
        $('#PaymentList').jqGrid('GridUnload');

        loadPaymentGrid("view");
    }

    // alert(isAutoLoad);
});




function ChangeDivToPending() {
    $('#divGeneratedVouchers').hide();
    $('#divPendingVouchers').show('slow');
    $('#PaymentList').jqGrid('GridUnload');
    $('#btnGenerateVoucher').show('slow');
    loadPaymentGrid("view");
}
function ChangeDivToGenerated() {
    $('#divPendingVouchers').hide();
    $('#divGeneratedVouchers').show('slow');

    $('#btnGenerateVoucher').hide();
    
  //  $('#generatedVoucher').show('slow');
    
    //$('#PaymentListGeneratedVouchers').jqGrid('GridUnload');
    //  loadPaymentGridGenratedVouchers("view");
  //  $('#btnViewSubmitGenerated').trigger('click');
    //$('#btnViewSubmitGenerated').trigger('click');
   // $('#PaymentListGeneratedVouchers').trigger('reloadGrid');
    $('#PaymentList').jqGrid('GridUnload');

    loadPaymentGridGenratedVouchers("view");
}



var masterGridWidth = 0
function loadPaymentGrid(mode) {
    // alert('in');
   // $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    blockPage();
    jQuery("#PaymentList").jqGrid({

        url: '/Payment/GetTransferDeductionAmtList/',
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: 15,
        multiselect: true,
         
        postData: {
            'DeductionType': $('#DeductionTypeList').val(),  'mode': mode, 'months': $('#months').val(), 'year': $('#year').val(), 'fromDate': $('#fromDate').val(), 'toDate': $('#toDate').val(), 'transType': $("#TXN_ID").val(),
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
        colNames: ['Voucher Number', 'Voucher Date','Month','Year' ,'Cheque Number','Deduction Type', 'Head Name', 'Contractor Name',"Bank Name","Account Number", "Deduction Amount"/*,"Generated Voucher"*/],
        colModel: [
            {
                name: 'Voucher_Number',index: 'Voucher_Number',width: 50, align: "center",frozen: true

            },
            {
                name: 'voucher_date', index: 'voucher_date',width: 80, align: "center",frozen: true,

            },
            {
                name: 'voucher_month', index: 'voucher_month', width: 80, align: "center", frozen: true,

            },
            {
                name: 'voucher_year', index: 'voucher_year', width: 80, align: "center", frozen: true,

            },
            {
                name: 'cheque_number',index: 'cheque_number',width: 120,align: "center"

            },
            {
                name: 'Deduction_Type', index: 'Deduction_Type', width: 100, align: "Center"

            },
            {
                name: 'HEAD_NAME', index: 'HEAD_NAME', width: 100, align: "Center"

            },
            {
                name: 'Contractor_Name',index: 'Contractor_Name',width: 150, align: "Center"

            },
            {
                name: 'BANK_NAME', index: 'cheque_amount', width: 100, align: "Center"

            },
            {
                name: 'ACC_NUMBER',index: 'cheque_amount',width: 100,align: "Center"

            },
            {
                name: 'DEUDUCTION_AMOUNT',index: 'Cash_Amount',width: 100,align: "Center"

            },
            //{
            //    name: 'GENERATED_VOUCHER', index: 'GENERATED_VOUCHER', width: 100, align: "Center"

            //},


        ],
        pager: "#pager",
        viewrecords: true,
        pgbuttons: false,
        pgtext: null,
        loadError: function (xhr, st, err) {

           // $.unblockUI();
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        loadComplete: function (xhr, st, err) {
            var userdata = jQuery("#PaymentList").getGridParam('userData');
            var rocordCount = jQuery("#PaymentList").jqGrid('getGridParam', 'records');
           // alert($('#' + userdata.ids[0] + ' input[type=checkbox]').attr('checked', true));
           // alert(userdata);
            
         //   alert(userdata.ids.length);
            var count = 0;
            
            for (var i = 0; i < userdata.ids.length; i++) {

                if ($('#' + userdata.ids[i] + ' input[type=checkbox]').attr('checked', true)) {
                    jQuery("#jqg_PaymentList_" + userdata.ids[i]).attr("disabled", true);
                   
                }
            }
            
            
            for (var i = 0; i < userdata.ids.length; i++) {
                if (($("#jqg_PaymentList_" + userdata.ids[i]).attr("disabled"))) {
                    count = count + 1;
                }
            }
            
            //$("#cb_PaymentList").attr('disabled', true);
            if (count == rocordCount) {
                $("#cb_PaymentList").attr('checked', true);
                $("#cb_PaymentList").attr('disabled', true);
            }

           unblockPage();
           $.unblockUI();
            //unblockPage();
            $("#PaymentList").parents('div.ui-jqgrid-bdiv').css("max-height", "385px");
            //Added By Abhishek kamble 11-nov-2013
            $('#PaymentList_rn').html('Sr.<br/>No');
        },
       
        sortname: 'voucher_date',
        sortorder: "desc",
        caption: "Payment Details"
    });

   

}

$('#vDate').on('change', function () {
    var date = $('#vDate').val();

    var myArray = date.split("/");
    //var day = myArray[0];
    var month = myArray[1];
    var year = myArray[2];

    $.ajax({
        type: "POST",
        url: "/payment/GenerateVoucherNo/V$" + month + '$' + year,
        async: false,
        error: function (xhr, status, error) {
            $('#divError').show('slow');
            $('#errorSpan').text(xhr.responseText);

        },
        success: function (data) {
            //unblockPage();
            $('#divError').hide('slow');
            $('#errorSpan').html("");
            if (data != "") {
                $("#BILL_NO").val("");
                $("#BILL_NO").val(data.strVoucherNumber);
                $("#BILL_NO").attr('readonly', true);
            }
        }
    });

    $.ajax({
        type: "POST",
        url: "/payment/GetEpayNumber/" + month + "$" + year,
        async: false,
        error: function (xhr, status, error) {
            //unblockPage();
            $('#divError').show('slow');
            $('#errorSpan').text(xhr.responseText);
            return false;
        },
        success: function (data) {
            //unblockPage();
            $('#divError').hide('slow');
            $('#errorSpan').html("");
            $('#CHQ_NO').hide();
            $('#EPAY_NO').val(data).attr("readonly", "readonly");
            $('#EPAY_NO').show();
        }
    });

});


function GetDataInArray() {
    var submitArray = [];
    var selRowIds = jQuery('#PaymentList').jqGrid('getGridParam', 'selarrrow');
    if (selRowIds.length > 0) {
        for (var i = 0; i < selRowIds.length; i++) {
            //alert(selRowIds[i]);
            if (($("#jqg_PaymentList_" + selRowIds[i]).attr("disabled"))) {

            }
            else {
                submitArray.push(selRowIds[i]);
            }
           
        }
        AssignVouchers(submitArray);
    }
    else {
        alert("No records to assign");
        unblockPage();
        $.unblockUI();
    }
         

}

function AssignVouchers(submitArray) {
       
       
       
        $.ajax({
            type: "POST",
            url: "/Payment/IsAssignVouchersCorrect/",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ 'submitarray': submitArray }),
            cache: false,
            success: function (response) {
                if (response.success) {
                    TransferDeductionAmtDilofueBox(submitArray);
                }
                else {
                    Alert(response.Msg);
                }
                
                
            },
            error: function (xhr, status, error) {
                //unblockPage();
               
                return false;
            }

        });
   

}

function TransferDeductionAmtDilofueBox(submitArray) {
    blockPage();
    if (confirm("Do you want to generate voucher for the selected records? ")) {

        $.ajax({
            type: "POST",
            dataType: "html",
            url: "/payment/TransferDeductionAmtDilogueBox/",
            async: false,
            cache: false,
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ 'submitarray': submitArray }),
           
            error: function (xhr, status, error) {
                //unblockPage();
                alert("Error");
                return false;
            },
            success: function (data) {
                $.unblockUI();
                unblockPage();
                $("#transfer").dialog("open");
                $("#tranferDeductionForm").html(data);
                var date = $('#vDate').val();
                var myArray = date.split("/");
                var month = myArray[1];
                var year = myArray[2];

                $.ajax({
                    type: "POST",
                    url: "/payment/GenerateVoucherNo/V$" + month + '$' + year,
                    async: false,
                    cache: false,
                    error: function (xhr, status, error) {
                        $('#divError').show('slow');
                        $('#errorSpan').text(xhr.responseText);

                    },
                    success: function (data) {
                        $.unblockUI();
                        $('#divError').hide('slow');
                        $('#errorSpan').html("");
                        if (data != "") {
                            $("#BILL_NO").val("");
                            $("#BILL_NO").val(data.strVoucherNumber);
                            $("#BILL_NO").attr('readonly', true);

                            //var date = new Date().toLocaleDateString('en-GB');
                            //$("#vDate").val(date);
                        }

                    }

                });

                $.ajax({
                    type: "POST",
                    url: "/payment/GetEpayNumber/" + month + "$" + year,
                    async: false,
                    cache: false,
                    error: function (xhr, status, error) {
                        //unblockPage();
                        $('#divError').show('slow');
                        $('#errorSpan').text(xhr.responseText);
                        return false;
                    },
                    success: function (data) {
                        //unblockPage();
                        $('#divError').hide('slow');
                        $('#errorSpan').html("");
                        $('#CHQ_NO').hide();
                        $('#EPAY_NO').val(data).attr("readonly", "readonly");
                        $('#EPAY_NO').show();
                    }

                });

            }
        });
        unblockPage();
        $.unblockUI();
    }
    else {
       
        unblockPage();
        $.unblockUI();
        return;
    }
    unblockPage();
    $.unblockUI();
}




//Generated Vouchers
$("#btnViewSubmitGenerated").click(function () {
    $('#btnGenerateVoucher').hide();
   // alert("VIKKY");
    // $('#generatedVoucher').show('slow');
    //  alert("vikky");
    if ($('#months').val() == 0) {
        alert("please select month");
        return false;
    }

    if ($('#year').val() == 0) {
        alert("please select year");
        return false;
    }

    if ($("#listForm").valid()) {
        //  $('#PaymentList').jqGrid('GridUnload');
        //alert("1");
        $('#PaymentList').jqGrid('GridUnload');

        loadPaymentGridGenratedVouchers("view");
    }

    // alert(isAutoLoad);
});



function loadPaymentGridGenratedVouchers(mode) {
    // alert('in');
    // $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    // $('#PaymentListGeneratedVouchers').jqGrid('GridUnload');
     blockPage();
    jQuery("#PaymentList").jqGrid({

        url: '/Payment/GetTransferDeductionAmtGeneratedVoucherList/',
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: 15,
        multiselect: false,

        postData: {
            'DeductionType': $('#DeductionTypeList').val(), 'mode': mode, 'months': $('#months').val(), 'year': $('#year').val(), 'fromDate': $('#fromDate').val(), 'toDate': $('#toDate').val(), 'transType': $("#TXN_ID").val(),
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
        colNames: ['Voucher Number', 'Voucher Date', 'Epayment Number', 'Deduction Type', 'Cheque Amount (In Rs.)', 'Cash Amount (In Rs.)', 'Gross Amount (In Rs.)', 'Transaction Description', 'PFMS Status', 'PFMS Remarks', 'Reinitiate payment'],
        colModel: [
            {
                name: 'Voucher_Number', index: 'Voucher_Number', width: 50, align: "center", frozen: true

            },
            {
                name: 'voucher_date', index: 'voucher_date', width: 80, align: "center", frozen: true,

            },

            {
                name: 'Epayment_number', index: 'cheque_number', width: 120, align: "center"

            },
            {
                name: 'Deduction_Type', index: 'Deduction_Type', width: 100, align: "Center"

            },

            {
                name: 'Cheque_Amount', index: 'Cheque_Amount', width: 100, align: "Center"

            },
            {
                name: 'Cash_Amount', index: 'Cash_Amount', width: 100, align: "Center"

            },
            {
                name: 'Gross_Amount', index: 'Gross_Amount', width: 100, align: "Center"

            },
            {
                name: 'TransactionDescr', index: 'TransactionDescr', width: 200, align: "Center"

            },
            {
                name: 'PFMSStatus', index: 'PFMSStatus', width: 150, align: "Center"

            },
            {
                name: 'PFMSRemarks', index: 'PFMSRemarks', width: 150, align: "Center"

            },
            {
                name: 'Reinite', index: 'Reinite', width: 150, align: "Center",hidden:true

            },

        ],
        pager: "#pager",
        viewrecords: true,
        pgbuttons: false,
        pgtext: null,
        loadError: function (xhr, st, err) {

            // $.unblockUI();
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        loadComplete: function (xhr, st, err) {
            //   $("#gbox_PaymentListGeneratedVouchers").style.width = '500';
            // document.getElementById('gbox_PaymentListGeneratedVouchers').style.width = '500';
            //  var userdata = jQuery("#PaymentListGeneratedVouchers").getGridParam('userData');
            //  var rocordCount = jQuery("#PaymentListGeneratedVouchers").jqGrid('getGridParam', 'records');
            // alert($('#' + userdata.ids[0] + ' input[type=checkbox]').attr('checked', true));
            // alert(userdata);

            //   alert(userdata.ids.length);
            //var count = 0;

            //for (var i = 0; i < userdata.ids.length; i++) {

            //    if ($('#' + userdata.ids[i] + ' input[type=checkbox]').attr('checked', true)) {
            //        jQuery("#jqg_PaymentListGeneratedVouchers_" + userdata.ids[i]).attr("disabled", true);

            //    }
            //}


            //for (var i = 0; i < userdata.ids.length; i++) {
            //    if (($("#jqg_PaymentListGeneratedVouchers_" + userdata.ids[i]).attr("disabled"))) {
            //        count = count + 1;
            //    }
            //}

            ////$("#cb_PaymentList").attr('disabled', true);
            //if (count == rocordCount) {
            //    $("#cb_PaymentListGeneratedVouchers").attr('checked', true);
            //    $("#cb_PaymentListGeneratedVouchers").attr('disabled', true);
            //}

            unblockPage();
           $.unblockUI();
            //unblockPage();
            $("#PaymentList").parents('div.ui-jqgrid-bdiv').css("max-height", "385px");
            //Added By Abhishek kamble 11-nov-2013
            $('#PaymentList_rn').html('Sr.<br/>No');
        },
      
        sortname: 'voucher_date',
        sortorder: "desc",
        caption: "Payment Details"
    });



}

function RenewEpaymentHoldingAcc(urlParam) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });     
    $("#PaymentList").jqGrid('setGridState', 'hidden');
   // $("#tblSearch").hide('slow', function () { });
    $("#RenewEpaymentHoldingAcc").load("/payment/GetEpaymentHoldingAccReinit/" + urlParam, function () {
        var date = $('#BILL_DATE').val();
        var myArray = date.split("/");
        var month = myArray[1];
        var year = myArray[2];
        $.ajax({
            type: "POST",
            url: "/payment/GetEpayNumber/" + month + "$" + year,
            async: false,
            cache: false,
            error: function (xhr, status, error) {
                //unblockPage();
                $('#divError').show('slow');
                $('#errorSpan').text(xhr.responseText);
                return false;
            },
            success: function (data) {
                //unblockPage();
                $('#divError').hide('slow');
                $('#errorSpan').html("");
                $('#CHQ_NO').hide();
                $('#EPAY_NO').val(data).attr("readonly", "readonly");
                $('#EPAY_NO').show();
            }

        });
        $.unblockUI();
        return false;
    });
  
}









