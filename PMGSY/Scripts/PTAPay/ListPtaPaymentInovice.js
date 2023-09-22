$(document).ready(function () {

    //$.validator.unobtrusive.parse('#searchPTAPayment');

    //disabled enter key
    $("input").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });  

    //add accordion
    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

   
    $("#idFilterDiv").click(function () {
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divFilterForm").toggle("slow");
    });

    $("#btnGo").click(function () {
        blockPage();
        if (validate()) {
            ListPtaPayment();
            $("#accordion").hide('slow');
            $("#divAddPTAPayment").hide('slow');
        }
        unblockPage();
    });

});

// to load PTAPayment details

function ListPtaPayment() {
   
    $("#tbPtaInvoiceList").jqGrid('GridUnload');
    blockPage();
    jQuery("#tbPtaInvoiceList").jqGrid({
        url: '/PtaPay/ListPtaPaymentInoviceDetails',
        datatype: "json",
        mtype: "POST",
        colNames: ["PTA Institute Name", "Honorarium Amount [In Rs.]", "Invoice Number", "Penalty Amount [In Rs.]", "TDS Amount [In Rs.]", "SC AMOUNT [In Rs.]", "Amount Paid [In Rs.]", "Generation Date", "Payment","BalanceAmount"],
        colModel: [

                    { name: 'PTAInstituteName', index: 'PTAInstituteName', width: 230, sortable: false, align: "center" },
                    { name: 'HonorariumAmount', index: 'HonorariumAmount', width: 140, sortable: false, align: "center" /*, summaryType: 'sum' */ },
                    { name: 'InvoiceNumber', index: 'InvoiceNumber', width: 140, sortable: false, align: "center" },
                    { name: 'PenaltyAmount', index: 'PenaltyAmount', width: 140, sortable: false, align: "center" },
                    { name: 'TDSAmount', index: 'TDSAmount', width: 90, sortable: false, align: "center" },
                    { name: 'SCAMOUNT', index: 'SCAMOUNT', width: 90, sortable: false, align: "center" },
                    { name: 'AmountPaid', index: 'AmountPaid', width: 90, sortable: false, align: "center" },
                    { name: 'GenerationDate', index: 'GenerationDate', width: 100, sortable: false, align: "center" },
                    { name: 'PaymentView', index: 'PaymentView', width: 50, sortable: false, align: "center" },
                    { name: 'BalanceAmount', index: 'BalanceAmount', width: 40, sortable: false, align: "center", hidden: true }
        ],
        postData: { "MAST_STATE_ID": $("#ddlState").val(), "IMS_YEAR": $("#ddlImsYear").val(), "IMS_BATCH": $("#ddlImsBatch").val(), "IMS_STREAM": $("#ddlImsStreams").val(), "IMS_PROPOSAL_TYPE": $("#ddlImsProposalTypes").val(), "PMGSY_SCHEME": $("#ddlImsProposalSchemes option:selected").val() },
        pager: jQuery('#dvPtaInvoicePager'),
        rowList: [10, 15, 20],
        rowNum: 10,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;PTA Payment",
        height: 'auto',
        //width: 'auto',
        sortname: 'PTAInstituteName',
        rownumbers: true,
        footerrow: true,
        loadComplete: function (data) {
           

        },
        loadError: function (xhr, status, error) {
            unblockPage();
        }
    }); //end of grid   
    unblockPage();
}

//populates result according to changed value
function FillInCascadeDropdown(map, dropdown, action) {
    var message = '';

    $(dropdown).empty();

    $.post(action, map, function (data) {
        $.each(data, function () {

            if (this.Selected == true) {
                $(dropdown).append("<option selected value=" + this.Value + ">" + this.Text + "</option>");
            }
            else {
                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            }
        });

    }, "json");
}

//returns the view of Physical progress of Proposal
function AddPtaPaymentDetail(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add Payment Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="ClosePaymentDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddPTAPayment").load('/PtaPay/PtaPaymentDetail?id=' + urlparameter, function () {
            //$.validator.unobtrusive.parse($('#frmAddPhysicalRoad'));
            unblockPage();
        });       
        $('#divAddPTAPayment').show('slow');
        $("#divAddPTAPayment").css('height', 'auto');
    });
    $("#tbPtaInvoiceList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');

}

//close the accordion of physical and financial details
function ClosePaymentDetails() {

    $("#accordion").hide('slow');
    $("#divAddPTAPayment").hide('slow');
    $("#tbPtaInvoiceList").jqGrid('setGridState', 'visible');
    $('#tbPtaInvoiceList').trigger('reloadGrid');
    ShowFilter();
}

//show the filter view 
function ShowFilter() {

    $("#divSearchPTAPayment").show('slow');
    $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    $('#idFilterDiv').trigger('click');
}

function validate() {
    if ($("#ddlState").val() == "0") {
        alert("Please Select State.");
        return false;
    }
    if ($("#ddlImsYear").val() == "0") {
        alert("Please Select Year.");
        return false;
    }
    if ($("#ddlImsBatch").val() == "0") {
        alert("Please Select Batch.");
        return false;
    }
    if ($("#ddlImsStreams").val() == "0") {
        alert("Please Select Funding Agency.");
        return false;
    }
    return true;
}





