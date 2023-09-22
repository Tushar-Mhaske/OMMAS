$(document).ready(function () {
    $.validator.unobtrusive.parse($('#tourFiltersForm'));

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $('#btnGo').click(function () {
        //alert(($('#tourFiltersForm').valid()));
        $('#tbTourInvoiceList').jqGrid('GridUnload');
        if ($('#tourFiltersForm').valid()) {
            ListTourPayment();
        }
    });
});

function ListTourPayment() {
    blockPage();
    $('#tbTourPaymentList').jqGrid('GridUnload');
    jQuery("#tbTourPaymentList").jqGrid({
        url: '/QualityMonitoring/ListTourPaymentDetails',
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "Service Tax No", "Tour Expenditure [In Rs.]", "Tour Claim", "Submission Date", "Definalize", "View"],
        colModel: [
                    { name: 'Monitor', index: 'Monitor', width: 250, sortable: false, align: "center" },
                    { name: 'ServiceTax', index: 'ServiceTax', width: 200, sortable: false, align: "center" },
                    { name: 'TourExpenditure', index: 'TourExpenditure', width: 200, sortable: false, align: "center", summaryType: 'sum' },
                    { name: 'TourReport', index: 'TourReport', width: 180, sortable: false, align: "center", formatter: AnchorFormatter },
                    { name: 'SubmissionDate', index: 'SubmissionDate', width: 180, sortable: false, align: "center" },
                    { name: 'Definalize', index: 'Definalize', width: 80, sortable: false, align: "center" },
                    { name: 'View', index: 'View', width: 40, sortable: false, align: "center" }
        ],
        postData: { "IMS_YEAR": $('#ddlYear option:selected').val(), "Month": $('#ddlMonth option:selected').val(), "Monitor": $('#ddlMonitor option:selected').val() },
        pager: jQuery('#dvTourPaymentPager'),
        rowList: [10, 15, 20],
        rowNum: 10,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Tour Details",
        height: 'auto',
        //width: 'auto',
        sortname: 'Monitor',
        rownumbers: true,
        footerrow: true,
        loadComplete: function (data) {
            var grid = $("#tbTourPaymentList");
            var TourExpenditureTotal = grid.jqGrid('getCol', 'TourExpenditure', false, 'sum');
            TourExpenditureTotal = parseFloat(Math.round(TourExpenditureTotal)).toFixed(2);
            grid.jqGrid('footerData', 'set', { 'TourExpenditure': TourExpenditureTotal });
            //grid.jqGrid('footerData', 'set', { 'PerTotalValue': data.TotalModel.DIS_TOTAL_PER_TOT_VALUE });

            //ScrutinyAmount = grid.jqGrid('getCol', 'ValueofProposals', false, 'sum');
            //ScrutinyAmount = parseFloat(ScrutinyAmount).toFixed(2);
            ////grid.jqGrid('footerData', 'set', { 'ValueofProposals': ScrutinyAmount });
            //grid.jqGrid('footerData', 'set', { 'ValueofProposals': data.TotalModel.DIS_TOTAL_SANCTION_AMOUNT });

            //HonaroriumAmount = grid.jqGrid('getCol', 'HonorariumAmount', false, 'sum');
            //HonaroriumAmount = parseFloat(HonaroriumAmount).toFixed(2);
            ////grid.jqGrid('footerData', 'set', { 'HonorariumAmount': HonaroriumAmount });
            //grid.jqGrid('footerData', 'set', { 'HonorariumAmount': data.TotalModel.DIS_TOTAL_HON_AMOUNT });
            grid.jqGrid('footerData', 'set', { 'Monitor': 'Total' });

        },
        loadError: function (xhr, status, error) {
            unblockPage();
        }
    }); //end of grid   
    unblockPage();
}

function downloadFileFromAction(paramurl) {
    //window.location = paramurl;

    $.get(paramurl).done(function (response) {
        if (response.Success == 'false') {
            alert('File Not Found.');
            return false;

        }
        else if (response.Success === undefined) {
            window.location = paramurl;
        }
    });
}

function AnchorFormatter(cellvalue, options, rowObject) {
    var url = "/QualityMonitoring/DownloadFileTour/" + cellvalue;
    return cellvalue == "-" ? "-" : "<a href='#' onclick=downloadFileFromAction('" + url + "'); return false;> <img style='height:16px;width:16px' height='16' width='16' border=0 src='../../Content/images/PDF.ico' /> </a>";
}

function CloseDetails() {
    $('#accordion').hide('slow');
    $('#divProposalForm').hide('slow');

    $("#tbStaPaymentList").jqGrid('setGridState', 'visible');
    $("#tbTourInvoiceList").jqGrid('setGridState', 'visible');
}

function QMDeFinalizeTourDetails(tourId) {
    if (confirm("Are you sure to finalize the tour details?")) {
        $.ajax({
            url: '/QualityMonitoring/DeFinalizeTourDetails/',
            type: 'POST',
            data: { tourId: tourId, value: Math.random() },
            success: function (response) {
                if (response.Success) {
                    alert("Tour Details definalized successfully");
                    $("#tbTourPaymentList").trigger("reloadGrid");
                    //closeMonitorsScheduleDetails();
                }
                else {
                    //$("#divTourDetailsError").show("slow");
                    //$("#divTourDetailsError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
    else {
        return false;
    }
}

function ListTourInvoiceDetails(ADMIN_SCHEDULE_CODE) {
    $('#tbTourInvoiceList').jqGrid('GridUnload');
    blockPage();
    jQuery("#tbTourInvoiceList").jqGrid({
        url: '/QualityMonitoring/ListTourGeneratedInvoice',
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "Tour Expenditure", "Invoice Number", "Travel Claim Allowance", "Reporting Allowance", "Mileage Allowance", "Holding Charge Allowance", "Dearness Allowance", "Honorarium Allowance", "Other Allowance", "Deduction", "Other Deduction", "Net Payable", "Generation Date", "View", "Delete", "IMS_INVOICE_ID"],
        colModel: [

                    { name: 'Monitor', index: 'Monitor', width: 100, sortable: false, align: "center" },
                    { name: 'TourExpenditure', index: 'TourExpenditure', width: 90, sortable: false, align: "center" /*, summaryType: 'sum' */ },
                    { name: 'InvoiceNumber', index: 'InvoiceNumber', width: 120, sortable: false, align: "center" },
                    { name: 'TRAVEL_CLAIM_ALLOWANCE', index: 'TRAVEL_CLAIM_ALLOWANCE', width: 90, sortable: false, align: "center" },
                    { name: 'REPORTING_ALLOWANCE', index: 'REPORTING_ALLOWANCE', width: 70, sortable: false, align: "center" },
                    { name: 'MILLAGE_ALLOWANCE', index: 'MILLAGE_ALLOWANCE', width: 90, sortable: false, align: "center" },
                    { name: 'HOLDING_CHARGE_ALLOWANCE', index: 'HOLDING_CHARGE_ALLOWANCE', width: 90, sortable: false, align: "center" },
                    { name: 'DEARNESS_ALLOWANCE', index: 'DEARNESS_ALLOWANCE', width: 90, sortable: false, align: "center" },

                    { name: 'HONORARIUM_ALLOWANCE', index: 'HONORARIUM_ALLOWANCE', width: 90, sortable: false, align: "center" },
                    { name: 'OTHER_ALLOWANCE', index: 'OTHER_ALLOWANCE', width: 90, sortable: false, align: "center" },
                    { name: 'TDS_DEDUCTION', index: 'TDS_DEDUCTION', width: 90, sortable: false, align: "center" },
                    { name: 'OTHER_DEDUCTION', index: 'OTHER_DEDUCTION', width: 90, sortable: false, align: "center" },
                    { name: 'NET_PAYABLE', index: 'NET_PAYABLE', width: 90, sortable: false, align: "center" },

                    { name: 'GenerationDate', index: 'GenerationDate', width: 70, sortable: false, align: "center" },
                    { name: 'View', index: 'View', width: 40, sortable: false, align: "center" },
                    { name: 'Delete', width: '50px', sortable: false, resize: false, align: "center" },
                    { name: 'IMS_INVOICE_ID', index: 'BalanceAmount', width: 40, sortable: false, align: "center", hidden: true }
        ],
        postData: { "scheduleCode": ADMIN_SCHEDULE_CODE },
        pager: jQuery('#dvTourInvoicePager'),
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
            var rowcount = jQuery("#tbTourInvoiceList").jqGrid('getGridParam', 'records');
            $("#tbTourInvoiceList #dvTourInvoicePager").css({ height: '31px' });
            //var button =
            $("#dvTourInvoicePager_left").html("<input type='button' style='margin-left:27px' id='idGenerateInvoice' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = AddInvoiceDetails(\"" + ADMIN_SCHEDULE_CODE + "\"); value='Generate Invoice'/>");
            //$("#dvTourInvoicePager_left").html(button);

            if (parseInt(rowcount) > 0) {
                $('#idGenerateInvoice').hide();
            }
            //unblockPage();

            var grid = $("#tbTourInvoiceList");
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

            //$("#tbTourInvoiceList #dvTourInvoicePager").css({ height: '31px' });
            //$("#dvTourInvoicePager").html("<input type='button' style='margin-left:27px' id='idGenerateInvoice' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = AddInvoiceDetails(\"" + MAST_STATE_CODE + "\",\"" + IMS_YEAR + "\",\"" + IMS_BATCH + "\",\"" + IMS_STREAM + "\",\"" + IMS_PROPOSAL_TYPE + "\",\"" + STA_SANCTIONED_BY.replace(/\s+/g, '$') + "\",\"" + STA_INSTITUTE_NAME.replace(/\s+/g, '$') + "\",\"" + HON_AMOUNT + "\",\"" + PMGSY_SCHEME + "\"); value='Generate Invoice'/>")
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

function AddInvoiceDetails(ADMIN_SCHEDULE_CODE) {

    ////var Parameters = arrParameters;//.split("$");
    //var STA_SANCTIONED_BY = STA_SANCTIONED_BY.replace("$", ' ');
    //var STA_INSTITUTE_NAME = INSTITUTE_NAME.replace(/\$/g, ' ');

    // var STA_INSTITUTE_NAME = INSTITUTE_NAME.trim();

    $('#accordion').show('slow', function () {
        blockPage();

        $.validator.unobtrusive.parse($('#tourFiltersForm'));

        $.ajax({
            url: "/QualityMonitoring/AddTourInvoiceDetails/",
            type: "GET",
            cache: false,
            data: { ADMIN_SCHEDULE_CODE: ADMIN_SCHEDULE_CODE },
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                unblockPage();
                $("#divAddTourInvoice").html(response);
            }
        });

        $('#divAddTourInvoice').show('slow');
        $("#divAddTourInvoice").css('height', 'auto');
    });

    unblockPage();

    jQuery('#tbStaPaymentList').jqGrid('setGridState', 'hidden');
    jQuery('#tbStaInvoiceList').jqGrid('setGridState', 'hidden');
}

function DeleteDetails(invoiceID) {
    $.ajax({

        type: 'POST',
        url: '/QualityMonitoring/DeleteTourGeneratedInvoice?invoiceID=' + invoiceID,
        async: false,
        cache: false,
        success: function (data) {
            if (data.Success == true) {
                $("#tbTourInvoiceList").trigger('reloadGrid');
                alert('Invoice details deleted successfully.');
            }
            else if (data.Success == false) {
                alert(data.ErrorMessage);
            }

        },
        error: function () { }
    });
}

function ShowDetails(param) {

    //*************New of SSRS Report PDF as Created*************//
    window.open('/QualityMonitoring/TourPaymentSSRSReport?id=' + param, '_blank');
}