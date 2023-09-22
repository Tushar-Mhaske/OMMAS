$(document).ready(function () {

    //$.validator.unobtrusive.parse($("#frmRejectResend"));
    //$('#registerFormId').validate({
    if ($('#fundType').val() == "A") {
        $('#rdDPIU').click(function () {
            //alert('#rdDPIU');
            $('#PaymentList').jqGrid('GridUnload');
            $('#ddlSRRDA').val(0);
            $('#ddlSRRDA').hide();
            $('#ddlDPIU').show('slow');
            //$('#ddlDPIU').rules('add', {
            //    Required: true,
            //    messages: {
            //        Required: 'DPIU is Required',
            //    },
            //    regex: '^[1-9]\d*$'

            //});
            //$.validator.addMethod("CheckDropDownList", function (value, element, param) {  
            //    if (value == '0')  
            //        return false;  
            //    else  
            //        return true;  
            //},"Please select a Department.");    

            //$("#frmRejectResend").validate({  
            //    rules: {  

            //    'ddlDPIU':{  
            //             CheckDropDownList:true  
            //         },  
            //    },  
            //    messages: {  
            //        //This section we need to place our custom validation message for each control.  
            //    },  
            //});  

        });


        $('#rdSRRDA').click(function () {
            //alert('#rdSRRDA');
            $('#PaymentList').jqGrid('GridUnload');
            $('#ddlDPIU').val(0);
            $('#ddlDPIU').hide();
            $('#ddlSRRDA').show('slow');
            //$('#ddlSRRDA').rules('add', {
            //    // maxlength: 6,
            //    RequiredDrp: true,
            //    messages: {
            //        RequiredDrp: 'SRRDA is Required',
            //    },
            //    regex: '^[1-9]\d*$'
            //});
        });
    }
    //});
});
$("#dialog").dialog({
    autoOpen: false,
    // height:550,
    //width: 600,
    //width: 'auto',
    width: 1100,
    modal: true,
    show: {
        effect: "blind",
        duration: 1000
    },
    hide: {
        effect: "explode",
        duration: 1000
    }
})

//function for view details button click
$("#btnViewRejectResend").click(function () {

    //Below condition added on 06-01-2022 
    if ($('#fundType').val() == "A") {
        if ($('#rdDPIU').is(':checked')) {
            if ($('#ddlDPIU').val() == "0") {
                alert("Please select valid DPIU");
                return false;
            }
        }

        if ($('#rdSRRDA').is(':checked')) {
            if ($('#ddlSRRDA').val() == "0") {
                alert("Please select valid SRRDA");
                return false;
            }
        }
    } else if ($('#ddlDPIU').val() == "0") {
        alert($('#fundType').val());
        alert("Please select valid DPIU");
        return false;
    }

    if ($('#months').val() == 0) {
        alert("please select month");
        return false;
    }

    if ($('#year').val() == 0) {
        alert("please select year");
        return false;
    }
    $('#PaymentList').jqGrid('GridUnload');

    if ($("#frmRejectResend").valid()) {
        loadPaymentGrid();
    }
});

//event for the rpayment and eremittance radio buttons
$("#Epay,#ERem").click(function () {
    if ($('#months').val() == 0) {
        alert("please select month");
        return false;
    }

    if ($('#year').val() == 0) {
        alert("please select year");
        return false;
    }

    if ($("#frmRejectResend").valid()) {
        $('#PaymentList').jqGrid('GridUnload');
        loadPaymentGrid();
    }

});



function loadPaymentGrid(mode) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    //alert($("#Epay").is(':checked'));
    //alert($('#fundType').val());

    jQuery("#PaymentList").jqGrid({
        url: '/Payment/GetEpaymentRejectResendList/',
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: 15,
        postData: {
            'month': $('#ddlMonth').val(), 'year': $('#ddlYear').val(), 'dpiu': $('#ddlDPIU').val(),
            'payType': function () {
                return $("#Epay").is(':checked') ? "E" : "R";
            },
            'srrda': $('#fundType').val() == "A" ? $('#ddlSRRDA').val() : "0"
        },
        rownumbers: true,
        //width: 1150,
        autowidth: true,
        pginput: false,
        //shrinkToFit: false,
        rowList: [15, 20, 30],
        colNames: ['Voucher Number', 'Voucher Date', 'Epayment/Eremittance Number', 'Epayment/Eremittance Date', 'Contractor/Payee Name', 'Cheque Amount </br>(In Rs.)', 'Cash Amount </br> (In Rs.)', 'Gross Amount (In Rs.)', 'Resend Epayment/Eremittance', 'Cancel Epayment/Eremittance'],
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
                width: 50,
                align: "center",
                frozen: true,
            },
        {
            name: 'cheque_number',
            index: 'cheque_number',
            width: 170,
            align: "center"

        },
            {
                name: 'cheque_Date',
                index: 'cheque_Date',
                width: 85,
                align: "Center"

            }, {
                name: 'Payee_Name',
                index: 'Payee_Name',
                width: 150,
                align: "left"
            },
            {
                name: 'cheque_amount',
                index: 'cheque_amount',
                width: 80,
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
                   name: 'RejectResend',
                   index: 'RejectResend',
                   width: 90,
                   align: "Center",
                   //PFMS Validations
                   //Below Codition Commented on 17-01-2022
                   //hidden: ($("#Epay").is(':checked') && $("#fundType").val() == 'P') ? true : false
                   //Below Condition modified on 17-01-2022 to hide resend column for fund type 'A' and 'P'
                   hidden: ($("#fundType").val() == 'P' || $("#fundType").val() == 'A') ? true : false
               },
               {
                   name: 'DeleteEpayEremi',
                   index: 'DeleteEpayEremi',
                   width: 90,
                   align: "Center",
                   hidden: false
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


function RejectResendEpayment(param) {
    // alert(param);

    var EpayEremi;

    if ($("#Epay").is(":checked")) {
        EpayEremi = "E";
    } else {
        EpayEremi = "R";
    }
    var dpiu = $('#ddlDPIU').val();
    var srrda = $('#fundType').val() == "A" ? $('#ddlSRRDA').val() : "0";

    $.ajax({
        url: "/Payment/RejectResendForm",
        type: 'GET',
        //data: { EncId: param, EpayEremi: EpayEremi, CancelResend: "R" },
        data: { EncId: param, EpayEremi: EpayEremi, CancelResend: "R", dpiu: dpiu, srrda: srrda },
        success: function (data) {
            if (data.message != null && data.message != undefined) {
                alert(data.message);
            }
            else {
                $("#dvLoadForm").html(data);

                if (EpayEremi == "E") {
                    $("#dialog").dialog("option", "title", "Resend Epayment Details");
                }
                else {
                    $("#dialog").dialog("option", "title", "Resend Eremittance Details");
                }

                $("#dialog").dialog("open");
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("An Error occured while processing your request.");
        }
    });

}


function CancelEpayEremi(param) {

    var EpayEremi;

    if ($("#Epay").is(":checked")) {
        EpayEremi = "E";
    } else {
        EpayEremi = "R";
    }
    //alert($('#fundType').val() == "A" ? $('#ddlSRRDA').val() : "0");
    //alert($('#ddlDPIU').val());
    var dpiu = $('#ddlDPIU').val();
    var srrda = $('#fundType').val() == "A" ? $('#ddlSRRDA').val() : "0";

    $.ajax({
        url: "/Payment/RejectResendForm",
        type: 'GET',
        //data: { EncId: param, EpayEremi: EpayEremi, CancelResend: "C" },
        data: { EncId: param, EpayEremi: EpayEremi, CancelResend: "C", dpiu: dpiu, srrda: srrda },
        success: function (data) {
            $("#dvLoadForm").html(data);

            if (EpayEremi == "E") {
                $("#dialog").dialog("option", "title", "Cancel Epayment");
            }
            else {
                $("#dialog").dialog("option", "title", "Cancel Eremittance");
            }


            $("#dialog").dialog("open");
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("An Error occured while processing your request.");
        }
    });

} 
