
$.validator.unobtrusive.parse($("#frmRejectResendDetails"));

//function for view details button click
$("input[type=text]").bind("keypress", function (e) {
    if (e.keyCode == 13) {
        return false;
    }
});

$(document).ready(function () {
    //alert(($('#isPaymentRejected').val() == 'True' && $('#fndType').val() == 'P' && $('#CancelResend').val() == 'C'));
    //Below line is commented on 18-01-2022
    //if (!($('#isPaymentRejected').val() == 'True' &&  $('#fndType').val() == 'P' && $('#CancelResend').val() == 'C')) {
    //Below line is Modified on 18-01-2022 to disable file selection for fund type='A' 
    if (!($('#isPaymentRejected').val() == 'True' && ($('#fndType').val() == 'P' || $('#fndType').val() == 'A') && $('#CancelResend').val() == 'C')) {
        $('#spnNEC').show('slow');
        $('.NEC').show('slow');
    }
    else {
        $('#spnNEC').hide('slow');
        $('.NEC').hide('slow');
    }

    //Radio Button click
    //$("#rdDelete").click(function () {
    //    $("#dvResendDate").hide();
    //});

    //$("#rdResend").click(function () {
    //    $("#dvResendDate").show();
    //});

    $(function () {

        //alert($("#CancelResend").val());
        $("#ResendDate").attr('readonly', 'readonly');

        if ($("#CancelResend").val() == "C") {//Cancel
            $("#ResendDate").datepicker({
                showOn: "button",
                buttonImage: "/Content/Images/calendar_2.png",
                showButtonText: 'Choose a date',
                changeMonth: true,
                buttonImageOnly: true,
                buttonText: 'Date',
                changeYear: true,
                dateFormat: "dd/mm/yy",
                maxDate: $("#currentDate").val(),
                minDate: $("#BillDate").val(),
            });
            $("#ResendDate").datepicker("setDate", $("#currentDate").val());

        }
        else {//Resend
            $("#ResendDate").val($("#currentDate").val());
        }

    });

    //$("#ResendDate").datepicker({
    //    showOn: "button",
    //    buttonImage: "/Content/Images/calendar_2.png",
    //    showButtonText: 'Choose a date',
    //    changeMonth: true,
    //    buttonImageOnly: true,
    //    buttonText: 'Date',
    //    changeYear: true,
    //    dateFormat: "dd/mm/yy",
    //    maxDate: new Date(),
    //});
    //$("#ResendDate").datepicker("setDate", $("#currentDate").val());

    //$("#txtSearchBillDate").attr('readonly', 'readonly');

    $("#btnSubmit").click(function () {

        if ($("#frmRejectResendDetails").valid()) {
            var formData = new FormData();
            var file = document.getElementById("picFile").files[0];

            if ((document.getElementById("picFile").files.length) == 0) {
                $("#spnRejectResendError").html("Please select File");
                $("#divRejectResendError").show('slow');
                return false;
            }

            formData.append("fileUpload", file);
            formData.append("Reason", $("#Reason").val());
            formData.append("Remark", $("#Remark").val());
            formData.append("Encrypted_BIllID_EpayID", $("#Encrypted_BIllID_EpayID").val());
            formData.append("ResendDate", $("#ResendDate").val());
            formData.append("IsEpayErremi", $("#IsEpayErremi").val());

            formData.append("CancelResend", $("#CancelResend").val());

            formData.append("BillDate", $("#BillDate").val());

            formData.append("HeadId", $("#ddlHeadID option:selected").val());

            //if ($("#rdDelete").is(":checked")) {
            //    formData.append("DeleteResend", "D");
            //}
            //else {
            //    formData.append("DeleteResend", "R");
            //}


            blockPage();

            //ajax call      
            $.ajax({
                url: '/Payment/SaveRejectResendDetails',
                type: 'POST',
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (data) {

                    if (data.status) {
                        $("#divRejectResendError").hide('slow');
                        $("#spnRejectResendError").html("");
                        alert(data.message);
                        $("#dialog").dialog("close");
                        jQuery("#PaymentList").trigger("reloadGrid");

                    } else {
                        $("#spnRejectResendError").html(data.message);
                        $("#divRejectResendError").show("slow");
                    }
                    unblockPage();
                },
                error: function (err) {
                    alert("An Error occured while processing your request.");
                    unblockPage();

                    return false;
                }
            });
        }
    });

    $("#btnCancel").click(function () {

        if ($("#frmRejectResendDetails").valid()) {

            //alert($('#isPaymentRejected').val());
            //alert("working");
            var formData = new FormData();

            //Below condition is commented on 18-01-2022
            //if (!($('#isPaymentRejected').val() == 'True' && $('#fndType').val() == 'P' && $('#CancelResend').val() == 'C')) {
            //Below condition is modified on 18-01-2022 to disable file selection for fund type='A'
            if (!($('#isPaymentRejected').val() == 'True' && ($('#fndType').val() == 'P' || $('#fndType').val() == 'A') && $('#CancelResend').val() == 'C')) {

                var file = document.getElementById("picFile").files[0];

                if ((document.getElementById("picFile").files.length) == 0) {
                    $("#spnRejectResendError").html("Please select File");
                    $("#divRejectResendError").show('slow');
                    return false;
                }

                formData.append("fileUpload", file);
            }
            formData.append("isPaymentRejected", $("#isPaymentRejected").val());
            formData.append("Reason", $("#Reason").val());
            formData.append("Remark", $("#Remark").val());
            formData.append("Encrypted_BIllID_EpayID", $("#Encrypted_BIllID_EpayID").val());
            formData.append("ResendDate", $("#ResendDate").val());
            formData.append("IsEpayErremi", $("#IsEpayErremi").val());

            formData.append("CancelResend", $("#CancelResend").val());

            formData.append("BillDate", $("#BillDate").val());

            formData.append("HeadId", $("#ddlHeadID option:selected").val());

            //if ($("#rdDelete").is(":checked")) {
            //    formData.append("DeleteResend", "D");
            //}
            //else {
            //    formData.append("DeleteResend", "R");
            //}


            blockPage();

            //ajax call      
            $.ajax({
                url: '/Payment/SaveRejectResendDetails',
                type: 'POST',
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (data) {

                    if (data.status) {
                        $("#divRejectResendError").hide('slow');
                        $("#spnRejectResendError").html("");
                        alert(data.message);
                        $("#dialog").dialog("close");
                        jQuery("#PaymentList").trigger("reloadGrid");

                    } else {
                        $("#spnRejectResendError").html(data.message);
                        $("#divRejectResendError").show("slow");
                    }
                    unblockPage();
                },
                error: function (err) {
                    alert("An Error occured while processing your request.");
                    unblockPage();

                    return false;
                }
            });
        }
    });

    //load payment Details 
    //if ($("#CancelResend").val()=="C")

    // alert($("#EncBillID").val());
    loadPaymentDetailsGrid($("#EncBillID").val());

});


//function to load payment & deduction grid of the transaction
function loadPaymentDetailsGrid(_Bill_ID) {

    blockPage();

    //   $("#PaymentGridDivList").jqGrid('GridUnload');

    jQuery("#PaymentGridDivList").jqGrid({

        url: '/Payment/GetPaymentDetailList/' + _Bill_ID,
        datatype: 'json',
        mtype: 'POST',
        //height: 'auto',
        height: '170px',
        //rowNum: 0,
        rowNum: 1000,
        //width:$("#gview_PaymentMasterList").width(),
        autowidth: true,
        pginput: false,
        hiddengrid: false,
        pgbuttons: false,
        colNames: ['Payment/Deduction', 'Tr.No', 'Head Code', 'Transaction type', 'Contractor Company Name', 'Agreement', 'Road', 'Cash <br/>/Cheque', 'Amount (In Rs.)', 'Narration', 'Edit', 'Delete', 'status'],
        colModel: [
             {
                 name: 'Pay_Ded',
                 index: 'Pay_Ded',
                 width: 120,
                 align: "left",
                 sortable: false


             },

            {
                name: 'Tr_No',
                index: 'Tr_No',
                width: 55,
                align: "left",
                sortable: false,
                hidden: true

            },

            {
                name: 'head_Id',
                index: 'head_Id',
                width: 35,
                align: "left",
                sortable: false
            },

            {
                name: 'Account_Head',
                index: 'Account_Head',
                width: 150,
                align: "left",
                sortable: false
            },


             {
                 name: 'Contractor_Name',
                 index: 'ContractorName',
                 width: 110,
                 align: "left",
                 sortable: false,
                 hidden: (fundType == 'A' ? true : false)

             },

            {
                name: 'Agreemnt',
                index: 'Agreemnt',
                width: 110,
                align: "left",
                sortable: false,
                hidden: (fundType == 'A' ? true : false)

            },
            {
                name: 'Road',
                index: 'Road',
                width: 150,
                align: "left",
                sortable: false,
                hidden: (fundType == 'A' ? true : false)

            },

             {
                 name: 'Cash_Cheque',
                 index: 'Cash_Cheque',
                 width: 45,
                 align: "left",
                 sortable: false


             },

             {
                 name: 'Amount',
                 index: 'Amount',
                 width: 75,
                 align: "right",
                 sortable: false


             },
        {
            name: 'Narration',
            index: 'Narration',
            width: 290,
            align: "left",
            sortable: false

        },
            {
                name: 'Edit',
                index: 'Edit',
                width: 50,
                align: "Center",
                sortable: false,
                hidden: true,

            }, {
                name: 'Delete',
                index: 'Delete',
                width: 50,
                align: "Center",
                sortable: false,
                hidden: true,
            },
            {
                name: 'Status',
                index: 'Status',
                width: 70,
                align: "Center",
                sortable: false,
                hidden: false
            }


        ],
        pager: "#PaymentGridDivpager",
        viewrecords: true,
        loadComplete: function () {
            unblockPage();
            $('#PaymentGridDivList').jqGrid('setGridWidth', $("#gview_PaymentMasterList").width());
            //$("#PaymentGridDivList").parents('div.ui-jqgrid-bdiv').css("max-height", "100px");
        },
        loadError: function (xhr, st, err) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        sortname: 'Tr_No',
        grouping: true,
        pgbuttons: false,
        groupingView: {
            groupField: ['Pay_Ded'],
            groupColumnShow: [false],
            groupText: ['<b>{0}</b>']
            //,groupCollapse: true
        },
        sortorder: "asc",
        caption: "Transaction Details"
    });



}




//event for the rpayment and eremittance radio buttons
//$("#Epay,#ERem").click(function () {
//    if ($('#months').val() == 0) {
//        alert("please select month");
//        return false;
//    }

//    if ($('#year').val() == 0) {
//        alert("please select year");
//        return false;
//    }

//    if ($("#frmRejectResend").valid()) {
//        $('#PaymentList').jqGrid('GridUnload');
//        loadPaymentGrid();
//    }
//    //loadPaymentGrid();
//});

//function RejectResendEpayment(param)
//{
//    // alert(param);
//    $.ajax({
//        url: "/Payment/RejectResendForm",
//        type: 'GET',
//        success: function (data) {
//            $("#dvLoadForm").html(data);
//            $("#dialog").dialog("open");
//        },
//        error: function (xhr, ajaxOptions, thrownError) {
//            alert("An Error occured while processing your request.");
//        }
//    });

//}