$(document).ready(function () {

    //$.validator.unobtrusive.parse('#searchSTAPayment');

    //disabled enter key
    //$("input").bind("keypress", function (e) {
    //    if (e.keyCode == 13) {
    //        return false;
    //    }
    //});  

    //add accordion
    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    //$("#idFilterDiv").click(function () {
    //    $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
    //    $("#divFilterForm").toggle("slow");
    //});

    $('#btnGo').click(function () {
        //alert(($('#tourPaymentFiltersForm').valid()));
        ClosePaymentDetails();
        if ($('#tourPaymentFiltersForm').valid()) {
            ListTourPaymentInvoice();
        }
    });
});

function ListTourPaymentInvoice() {
    $('#tblTourPaymentInvoiceList').jqGrid('GridUnload');
    blockPage();
    jQuery("#tblTourPaymentInvoiceList").jqGrid({
        url: '/QualityMonitoring/ListTourPaymentInovice',
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "Tour Expenditure", "Invoice Number", "Travel Claim Allowance", "Reporting Allowance", "Mileage Allowance", "Holding Charge Allowance", "Dearness Allowance", "Honorarium Allowance", "Other Allowance", "Deduction", "Other Deduction", "Net Payable", "Generation Date", "View", "IMS_INVOICE_ID"],
        colModel: [

                    { name: 'Monitor', index: 'Monitor', width: 180, sortable: false, align: "center" },
                    { name: 'TourExpenditure', index: 'TourExpenditure', width: 120, sortable: false, align: "center" /*, summaryType: 'sum' */ },
                    { name: 'InvoiceNumber', index: 'InvoiceNumber', width: 120, sortable: false, align: "center" },
                    { name: 'TRAVEL_CLAIM_ALLOWANCE', index: 'TRAVEL_CLAIM_ALLOWANCE', width: 120, sortable: false, align: "center" },
                    { name: 'REPORTING_ALLOWANCE', index: 'REPORTING_ALLOWANCE', width: 90, sortable: false, align: "center" },
                    { name: 'MILLAGE_ALLOWANCE', index: 'MILLAGE_ALLOWANCE', width: 90, sortable: false, align: "center" },
                    { name: 'HOLDING_CHARGE_ALLOWANCE', index: 'HOLDING_CHARGE_ALLOWANCE', width: 90, sortable: false, align: "center" },
                    { name: 'DEARNESS_ALLOWANCE', index: 'DEARNESS_ALLOWANCE', width: 90, sortable: false, align: "center" },

                    { name: 'HONORARIUM_ALLOWANCE', index: 'HONORARIUM_ALLOWANCE', width: 90, sortable: false, align: "center" },
                    { name: 'OTHER_ALLOWANCE', index: 'OTHER_ALLOWANCE', width: 90, sortable: false, align: "center" },
                    { name: 'TDS_DEDUCTION', index: 'TDS_DEDUCTION', width: 90, sortable: false, align: "center" },
                    { name: 'OTHER_DEDUCTION', index: 'OTHER_DEDUCTION', width: 90, sortable: false, align: "center" },
                    { name: 'NET_PAYABLE', index: 'NET_PAYABLE', width: 90, sortable: false, align: "center" },
                    { name: 'GenerationDate', index: 'GenerationDate', width: 100, sortable: false, align: "center" },
                    { name: 'View', index: 'View', width: 40, sortable: false, align: "center", },
                    //{ name: 'Delete', width: '50px', sortable: false, resize: false, align: "center" },
                    { name: 'IMS_INVOICE_ID', index: 'BalanceAmount', width: 40, sortable: false, align: "center", hidden: true }
        ],
        postData: { "IMS_YEAR": $('#ddlYear option:selected').val(), "Month": $('#ddlMonth option:selected').val(), "Monitor": $('#ddlMonitor option:selected').val() },
        pager: jQuery('#divTourPaymentInvoicePager'),
        rowList: [5, 10, 15],
        rowNum: 5,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Details of Amount Paid",
        height: 'auto',
        //width: 'auto',
        sortname: 'STAInstituteName',//Added By Abhishek kamble 30-Apr-2014
        rownumbers: true,
        footerrow: true,
        loadComplete: function (data) {
            //var rowcount = jQuery("#tblTourPaymentInvoiceList").jqGrid('getGridParam', 'records');
            $("#tblTourPaymentInvoiceList #divTourPaymentInvoicePager").css({ height: '31px' });
            //$("#divTourPaymentInvoicePager").html("<input type='button' style='margin-left:27px' id='idGenerateInvoice' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = AddInvoiceDetails(\"" + $('#ddlYear option:selected').val() + "\"); value='Generate Invoice'/>")

            //if (parseInt(rowcount) > 0) {
            //    $('#idGenerateInvoice').hide();
            //}

            //unblockPage();

            var grid = $("#tblTourPaymentInvoiceList");
            
            TotTourExpenditure = grid.jqGrid('getCol', 'TourExpenditure', false, 'sum');
            TotTourExpenditure = parseFloat(TotTourExpenditure).toFixed(2);
            grid.jqGrid('footerData', 'set', { 'TourExpenditure': TotTourExpenditure });
            //grid.jqGrid('footerData', 'set', { 'HonorariumAmount': data.TotalModel.DIS_TOTAL_HONORARIUM_AMOUNT });

            TotTRAVEL_CLAIM_ALLOWANCE = grid.jqGrid('getCol', 'TRAVEL_CLAIM_ALLOWANCE', false, 'sum');
            TotTRAVEL_CLAIM_ALLOWANCE = parseFloat(TotTRAVEL_CLAIM_ALLOWANCE).toFixed(2);
            grid.jqGrid('footerData', 'set', { 'TRAVEL_CLAIM_ALLOWANCE': TotTRAVEL_CLAIM_ALLOWANCE });
            //grid.jqGrid('footerData', 'set', { 'PenaltyAmount': data.TotalModel.DIS_TOTAL_PENALTY_AMOUNT });

            TotREPORTING_ALLOWANCE = grid.jqGrid('getCol', 'REPORTING_ALLOWANCE', false, 'sum');
            TotREPORTING_ALLOWANCE = parseFloat(TotREPORTING_ALLOWANCE).toFixed(2);
            grid.jqGrid('footerData', 'set', { 'REPORTING_ALLOWANCE': TotREPORTING_ALLOWANCE });

            TotMILLAGE_ALLOWANCE = grid.jqGrid('getCol', 'MILLAGE_ALLOWANCE', false, 'sum');
            TotMILLAGE_ALLOWANCE = parseFloat(TotMILLAGE_ALLOWANCE).toFixed(2);
            grid.jqGrid('footerData', 'set', { 'MILLAGE_ALLOWANCE': TotMILLAGE_ALLOWANCE });
            //grid.jqGrid('footerData', 'set', { 'MILLAGE_ALLOWANCE': data.TotalModel.DIS_TOTAL_SC_AMOUNT });

            TotHOLDING_CHARGE_ALLOWANCE = grid.jqGrid('getCol', 'HOLDING_CHARGE_ALLOWANCE', false, 'sum');
            TotHOLDING_CHARGE_ALLOWANCE = parseFloat(TotHOLDING_CHARGE_ALLOWANCE).toFixed(2);
            grid.jqGrid('footerData', 'set', { 'HOLDING_CHARGE_ALLOWANCE': TotHOLDING_CHARGE_ALLOWANCE });
            //grid.jqGrid('footerData', 'set', { 'ServiceAMOUNT': data.TotalModel.DIS_TOTAL_SERVICE_TAX_AMOUNT });

            TotDEARNESS_ALLOWANCE = grid.jqGrid('getCol', 'DEARNESS_ALLOWANCE', false, 'sum');
            TotDEARNESS_ALLOWANCE = parseFloat(TotDEARNESS_ALLOWANCE).toFixed(2);
            grid.jqGrid('footerData', 'set', { 'DEARNESS_ALLOWANCE': TotDEARNESS_ALLOWANCE });
            //grid.jqGrid('footerData', 'set', { 'AmountPaid': data.TotalModel.DIS_TOTAL_AMOUNT });


            TotHONORARIUM_ALLOWANCE = grid.jqGrid('getCol', 'HONORARIUM_ALLOWANCE', false, 'sum');
            TotHONORARIUM_ALLOWANCE = parseFloat(TotHONORARIUM_ALLOWANCE).toFixed(2);
            grid.jqGrid('footerData', 'set', { 'HONORARIUM_ALLOWANCE': TotHONORARIUM_ALLOWANCE });
            //grid.jqGrid('footerData', 'set', { 'PenaltyAmount': data.TotalModel.DIS_TOTAL_PENALTY_AMOUNT });

            TotOTHER_ALLOWANCE = grid.jqGrid('getCol', 'OTHER_ALLOWANCE', false, 'sum');
            TotOTHER_ALLOWANCE = parseFloat(TotOTHER_ALLOWANCE).toFixed(2);
            grid.jqGrid('footerData', 'set', { 'OTHER_ALLOWANCE': TotOTHER_ALLOWANCE });
            //grid.jqGrid('footerData', 'set', { 'PenaltyAmount': data.TotalModel.DIS_TOTAL_PENALTY_AMOUNT });

            TotDEDUCTION = grid.jqGrid('getCol', 'TDS_DEDUCTION', false, 'sum');
            TotDEDUCTION = parseFloat(TotDEDUCTION).toFixed(2);
            grid.jqGrid('footerData', 'set', { 'TDS_DEDUCTION': TotDEDUCTION });
            //grid.jqGrid('footerData', 'set', { 'PenaltyAmount': data.TotalModel.DIS_TOTAL_PENALTY_AMOUNT });

            TotOTHER_DEDUCTION = grid.jqGrid('getCol', 'OTHER_DEDUCTION', false, 'sum');
            TotOTHER_DEDUCTION = parseFloat(TotOTHER_DEDUCTION).toFixed(2);
            grid.jqGrid('footerData', 'set', { 'OTHER_DEDUCTION': TotOTHER_DEDUCTION });
            //grid.jqGrid('footerData', 'set', { 'PenaltyAmount': data.TotalModel.DIS_TOTAL_PENALTY_AMOUNT });

            TotNET_PAYABLE = grid.jqGrid('getCol', 'NET_PAYABLE', false, 'sum');
            TotNET_PAYABLE = parseFloat(TotNET_PAYABLE).toFixed(2);
            grid.jqGrid('footerData', 'set', { 'NET_PAYABLE': TotNET_PAYABLE });
            //grid.jqGrid('footerData', 'set', { 'PenaltyAmount': data.TotalModel.DIS_TOTAL_PENALTY_AMOUNT });

            grid.jqGrid('footerData', 'set', { 'Monitor': 'Total' });

            //grid.jqGrid('footerData', 'set', { 'STAInstituteName': 'Total' });

            //$("#tblTourPaymentInvoiceList #divTourPaymentInvoicePager").css({ height: '31px' });
            //$("#divTourPaymentInvoicePager").html("<input type='button' style='margin-left:27px' id='idGenerateInvoice' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = AddInvoiceDetails(\"" + MAST_STATE_CODE + "\",\"" + IMS_YEAR + "\",\"" + IMS_BATCH + "\",\"" + IMS_STREAM + "\",\"" + IMS_PROPOSAL_TYPE + "\",\"" + STA_SANCTIONED_BY.replace(/\s+/g, '$') + "\",\"" + STA_INSTITUTE_NAME.replace(/\s+/g, '$') + "\",\"" + HON_AMOUNT + "\",\"" + PMGSY_SCHEME + "\"); value='Generate Invoice'/>")
            //if (HonorariumAmount > 0) {
            //    $('#idGenerateInvoice').hide();
            //}
            //unblockPage();
        },
        loadError: function (xhr, status, error) {
            unblockPage();
        }
    }); //end of grid   
    unblockPage();
}

function AddTourPaymentDetail(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add Payment Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="ClosePaymentDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddTourPayment").load('/QualityMonitoring/TourPaymentDetail?id=' + urlparameter, function () {
            //$.validator.unobtrusive.parse($('#frmAddPhysicalRoad'));
            unblockPage();
        });
        $('#divAddTourPayment').show('slow');
        $("#divAddTourPayment").css('height', 'auto');
    });
    $("#tblTourPaymentInvoiceList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');

}

//close the accordion of physical and financial details
function ClosePaymentDetails() {

    $("#accordion").hide('slow');
    $("#divAddTourPayment").hide('slow');
    $("#tblTourPaymentInvoiceList").jqGrid('setGridState', 'visible');
    //ShowFilter();
}
