$(document).ready(function () {
    //Load test result List

    ShowTourPaymentList();
    //validation 


});


function ShowTourPaymentList() {

    var IMS_INVOICE_CODE = $('#hdIMS_INVOICE_CODE').val();

    jQuery("#tbTourPaymentList").jqGrid({
        url: '/QualityMonitoring/GetTourPaymentList/',
        datatype: "json",
        mtype: "POST",
        colNames: ['Payment Type', 'Cheque Number', 'Payment Date', 'Entry Date', 'Edit', 'Delete', 'Finalize', 'DeFinalize'],
        colModel: [
            { name: 'IMS_NEFT_CHEQUE_PAYMENT', index: 'IMS_NEFT_CHEQUE_PAYMENT', width: '180px', sortable: false, align: 'center' },
            { name: 'IMS_NEFT_CHEQUE_NUMBER', index: 'IMS_NEFT_CHEQUE_NUMBER', width: '180px', sortable: false, align: 'center' },
            { name: 'IMS_PAYMENT_DATE', index: 'IMS_PAYMENT_DATE', width: '220px', sortable: false, align: "center" },
            { name: 'IMS_ENTRY_DATE', index: 'IMS_ENTRY_DATE', width: '220px', sortable: false, align: "center" },
            //{ name: 'Edit', width: '50px', sortable: false, resize: false, align: "center", formatter: formatColumnEdit },
            //{ name: 'Delete', width: '50px', sortable: false, resize: false, align: "center", formatter: formatColumDelete }
            { name: 'Edit', width: '50px', sortable: false, resize: false, align: "center" },
            { name: 'Delete', width: '50px', sortable: false, resize: false, align: "center" },
            { name: 'Finalize', width: '50px', sortable: false, resize: false, align: "center" },
            { name: 'DeFinalize', width: '50px', sortable: false, resize: false, align: "center" }
        ],
        postData: { IMS_INVOICE_CODE: IMS_INVOICE_CODE, value: Math.random() },
        pager: $("#dvTourPaymentListPager"),
        sortorder: "asc",
        sortname: "IMS_SAMPLE_ID",
        rowNum: 5,
        pginput: true,
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: 'Payment Details',
        height: 'auto',
        width: '100%',
        rownumbers: true,
        footerrow: false,
        loadComplete: function () {
            //Total of Columns
            // $("#tbTourPaymentList").jqGrid('setGridWidth', $('#divPropNotMappedDetail').width(), true);
            var recordCount = jQuery('#tbTourPaymentList').jqGrid('getGridParam', 'reccount');

            if (recordCount == 0) {
                var button = '<input type="button" id="btnAddTourPayment" name="btnAddTourPayment" value="Add Payment" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" title="Add Payment Detail" tabindex="200" style="font-size:1em; margin-left:25px" onclick="ShowPaymentDetail()" />'
                $('#dvTourPaymentListPager_left').html(button);
            }
            else {
                $('#dvTourPaymentListPager_left').html("");
            }
        },
        loaderror: function (xhr, status, error) {

            if (xhr.responseText == 'session expired') {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else { }
        },
    });

}

function formatColumnEdit(cellvalue, options, rowObject) {
    if (cellvalue == "") {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-locked ui-align-center'></span></center>";
    }
    else {
        return "<center><span style='border-color:white;cursor:pointer' class='ui-icon ui-icon-pencil ui-align-center' title='Click here to Edit Test Result Details' onClick='EditSTAPaymentDetails(\"" + cellvalue.toString() + "\" );'></span></center> ";
    }
}

function formatColumDelete(cellvalue, options, rowObject) {
    if (cellvalue == "") {
        return "<center><span style=' border-color:white;cursor:pointer;' class='ui-icon ui-icon-locked ui-align-center'></span></center>";
    }
    else {
        return "<center><span style=' border-color:white;cursor:pointer;' title='Click here to Delete Test Result Details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteSTAPaymentDetails(\"" + cellvalue.toString() + "\");'></span></center>";
    }
}

function EditTourPayment(urlparam) {
    //$("#dvSTAPaymentForm").load('/Proposal/EditSTAPaymentDetails/', urlparam);   

    $.ajax({
        url: '/QualityMonitoring/EditTourPaymentDetails/' + urlparam,
        Type: 'POST',
        catche: false,
        beforeSend: function () {
            blockPage();
        },
        error: function (xhr, status, error) {
            unblockPage();
            alert("An error occured while processing your request.");

            return false;
        },
        success: function (response) {

            $('#divTourPayementAdd').html('');
            $("#divTourPayementAdd").html(response);
            $("#divTourPayementAdd").show('slow');
            if ($("#radioPayment_TypeNEFT").is(':checked')) {
                $('#lblPaymentTypeNumber').text("NEFT Number");
            }
            else {
                $('#lblPaymentTypeNumber').text("Cheque Number");
            }
            unblockPage();
        }
    });
}

function DeleteTourPayment(urlParam) {
    //alert("Delete");

    if (confirm("Are you sure you want to delete payment details ? ")) {
        $.ajax({

            url: '/QualityMonitoring/DeleteTourPaymentDetails/' + urlParam,
            type: 'POST',
            catche: false,
            error: function (xhr, status, error) {
                alert("Request can not be processed at this time, please try after some time...");
                return false;
            },
            beforeSend: function () {
                blockPage();
            },
            success: function (response) {

                if (response.success) {
                    alert(response.message);

                    //loadSTAPaymentDetailsForm();
                    $('#tbTourPaymentList').trigger('reloadGrid');
                    $("#divTourPayementAdd").hide("slow");
                    unblockPage();
                }
                else {
                    $("#divError").show("slow");
                    $("#divError span:eq(1)").html("<strong>Alert: </strong>" + response.message);
                    unblockPage();

                }
                unblockPage();
            }
        });//end of delete ajax call
    }
}

function FinalizeTourPayment(urlParam) {
    //alert("Delete");

    if (confirm("Are you sure you want to finalize payment details ? ")) {
        $.ajax({

            url: '/QualityMonitoring/FinalizeTourPaymentDetails/' + urlParam,
            type: 'POST',
            catche: false,
            error: function (xhr, status, error) {
                alert("Request can not be processed at this time, please try after some time...");
                return false;
            },
            beforeSend: function () {
                blockPage();
            },
            success: function (response) {

                if (response.success) {
                    alert(response.message);

                    //loadSTAPaymentDetailsForm();
                    $('#tbTourPaymentList').trigger('reloadGrid');
                    $("#divTourPayementAdd").hide("slow");
                    unblockPage();
                }
                else {
                    $("#divError").show("slow");
                    $("#divError span:eq(1)").html("<strong>Alert: </strong>" + response.message);
                    unblockPage();

                }
                unblockPage();
            }
        });//end of delete ajax call
    }
}


function ShowPaymentDetail() {

    var param = $('#hdEncryptedIMS_Invoice_Code').val();
    blockPage();
    $("#divTourPayementAdd").load('/QualityMonitoring/TourPaymentAdd?id=' + $('#hdEncryptedIMS_Invoice_Code').val(), function () {
        unblockPage();
    });
    $("#divTourPayementAdd").show('slow');
}

function ClearDetail() {
    $('input[type=text]').each(function () {
        $(this).val('');
    });

}
function DeFinalizeTourPayment(urlParam) {
    $.ajax({

        type: 'POST',
        url: '/QualityMonitoring/DeFinalizeTourPaymentDetails/' + urlParam,
        async: false,
        cache: false,
        success: function (data) {
            if (data.Success == true) {
                alert('Payment details definalized successfully.');
                $('#tbTourPaymentList').trigger('reloadGrid');
                $("#divTourPayementAdd").hide("slow");
            }
            else if (data.Success == false) {
                alert(data.ErrorMessage);
            }

        },
        error: function () { }
    });
}