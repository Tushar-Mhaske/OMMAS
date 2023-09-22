$(document).ready(function () {

    //$.validator.unobtrusive.parse('#searchSTAPayment');

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
            ListStaPayment();
            $("#accordion").hide('slow');
            $("#divAddSTAPayment").hide('slow');
        }
        unblockPage();
    });

});

// to load STAPayment details

function ListStaPayment() {
   
    $("#tbStaInvoiceList").jqGrid('GridUnload');
    blockPage();
    jQuery("#tbStaInvoiceList").jqGrid({
        url: '/StaPay/ListStaPaymentInoviceDetails',
        datatype: "json",
        mtype: "POST",
        colNames: ["STA Institute Name", "Honorarium Amount [In Rs.]", "Invoice Number", "Penalty Amount [In Rs.]", "TDS Amount [In Rs.]", "SC AMOUNT [In Rs.]", "Amount Paid [In Rs.]", "Generation Date", "Payment","BalanceAmount"],
        colModel: [

                    { name: 'STAInstituteName', index: 'STAInstituteName', width: 180, sortable: false, align: "center" },
                    { name: 'HonorariumAmount', index: 'HonorariumAmount', width: 140, sortable: false, align: "center" /*, summaryType: 'sum' */ },
                    { name: 'InvoiceNumber', index: 'InvoiceNumber', width: 140, sortable: false, align: "center" },
                    { name: 'PenaltyAmount', index: 'PenaltyAmount', width: 140, sortable: false, align: "center" },
                    { name: 'TDSAmount', index: 'TDSAmount', width: 90, sortable: false, align: "center" },
                    { name: 'SCAMOUNT', index: 'SCAMOUNT', width: 90, sortable: false, align: "center" },
                    { name: 'AmountPaid', index: 'AmountPaid', width: 90, sortable: false, align: "center" },
                    { name: 'GenerationDate', index: 'GenerationDate', width: 100, sortable: false, align: "center" },
                    { name: 'PaymentView', index: 'PaymentView', width: 60, sortable: false, align: "center" },
                    { name: 'BalanceAmount', index: 'BalanceAmount', width: 40, sortable: false, align: "center", hidden: true }
        ],
        postData: { "MAST_STATE_ID": $("#ddlState").val(), "IMS_YEAR": $("#ddlImsYear").val(), "IMS_BATCH": $("#ddlImsBatch").val(), "IMS_STREAM": $("#ddlImsStreams").val(), "IMS_PROPOSAL_TYPE": $("#ddlImsProposalTypes").val(), "PMGSY_SCHEME": $("#ddlImsProposalSchemes option:selected").val() },
        pager: jQuery('#dvStaInvoicePager'),
        rowList: [10, 15, 20],
        rowNum: 10,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;STA Payment",
        height: 'auto',
        //width: 'auto',
        sortname: 'STAInstituteName',
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
function AddStaPaymentDetail(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add Payment Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="ClosePaymentDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddSTAPayment").load('/StaPay/StaPaymentDetail?id=' + urlparameter, function () {
            //$.validator.unobtrusive.parse($('#frmAddPhysicalRoad'));
            unblockPage();
        });       
        $('#divAddSTAPayment').show('slow');
        $("#divAddSTAPayment").css('height', 'auto');
    });
    $("#tbStaInvoiceList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');

}

//close the accordion of physical and financial details
function ClosePaymentDetails() {
    $("#tbStaInvoiceList").trigger('reloadGrid');
    $("#accordion").hide('slow');
    $("#divAddSTAPayment").hide('slow');
    $("#tbStaInvoiceList").jqGrid('setGridState', 'visible');
    ShowFilter();
    
}

//show the filter view 
function ShowFilter() {

    $("#divSearchSTAPayment").show('slow');
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





