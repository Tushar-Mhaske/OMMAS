$(document).ready(function () {
    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $('#tbStaPaymentList').jqGrid('GridUnload');
    ListStaPay($("#ddlState").val(), $("#ddlImsYear").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#TOT_HON_MIN").val(), $("#ddlImsProposalSchemes").val());
});

function ListStaPay(MAST_STATE_CODE, IMS_YEAR, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE) {
    blockPage();
    jQuery("#tbStaPaymentList").jqGrid({
        url: '/StaPay/ListStaPaymentDetails',
        datatype: "json",
        mtype: "POST",
        colNames: ["STA Institute Name", "Service Tax No", "Value of Proposals Scrutinized [In Lakhs]", "% of Total Value", "Honorarium Amount [In Rs.]", "View"],
        colModel: [

                    { name: 'STAInstituteName', index: 'STAInstituteName', width: 250, sortable: false, align: "center" },
                    { name: 'ServiceTax', index: 'ServiceTax', width: 200, sortable: false, align: "center" },
                    { name: 'ValueofProposals', index: 'ValueofProposals', width: 200, sortable: false, align: "center", summaryType: 'sum' },
                    { name: 'PerTotalValue', index: 'PerTotalValue', width: 200, sortable: false, align: "center" },
                    { name: 'HonorariumAmount', index: 'HonorariumAmount', width: 200, sortable: false, align: "center" },
                    { name: 'View', index: 'View', width: 40, sortable: false, align: "center" }
        ],
        postData: { "MAST_STATE_ID": MAST_STATE_CODE, "IMS_YEAR": IMS_YEAR, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "TOT_HON_MIN": $("#TOT_HON_MIN").val(), "TOTAL_HON_AMOUNT_IN_RUPEES": $("#TOTAL_HON_AMOUNT_IN_RUPEES").val(), "TOT_HON_OF_SCRUTINY": $("#TOT_HON_OF_SCRUTINY").val(), "PMGSY_SCHEME": $("#ddlImsProposalSchemes option:selected").val() },
        pager: jQuery('#dvStaPaymentPager'),
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
            var grid = $("#tbStaPaymentList");
            PercentageTotal = grid.jqGrid('getCol', 'PerTotalValue', false, 'sum');
            PercentageTotal = parseFloat(Math.round(PercentageTotal)).toFixed(2);
            //grid.jqGrid('footerData', 'set', { 'PerTotalValue': PercentageTotal });
            grid.jqGrid('footerData', 'set', { 'PerTotalValue': data.TotalModel.DIS_TOTAL_PER_TOT_VALUE });

            ScrutinyAmount = grid.jqGrid('getCol', 'ValueofProposals', false, 'sum');
            ScrutinyAmount = parseFloat(ScrutinyAmount).toFixed(2);
            //grid.jqGrid('footerData', 'set', { 'ValueofProposals': ScrutinyAmount });
            grid.jqGrid('footerData', 'set', { 'ValueofProposals': data.TotalModel.DIS_TOTAL_SANCTION_AMOUNT });

            HonaroriumAmount = grid.jqGrid('getCol', 'HonorariumAmount', false, 'sum');
            HonaroriumAmount = parseFloat(HonaroriumAmount).toFixed(2);
            //grid.jqGrid('footerData', 'set', { 'HonorariumAmount': HonaroriumAmount });
            grid.jqGrid('footerData', 'set', { 'HonorariumAmount': data.TotalModel.DIS_TOTAL_HON_AMOUNT });
            grid.jqGrid('footerData', 'set', { 'STAInstituteName': 'Total' });

        },
        loadError: function (xhr, status, error) {
            unblockPage();
        }
    }); //end of grid   
    unblockPage();
}


function CloseDetails() {
    $('#accordion').hide('slow');
    $('#divProposalForm').hide('slow');

    $("#tbStaPaymentList").jqGrid('setGridState', 'visible');
    $("#tbStaInvoiceList").jqGrid('setGridState', 'visible');
}

function AddInvoiceDetails(MAST_STATE_CODE, IMS_YEAR, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, STA_SANCTIONED_BY, INSTITUTE_NAME, HON_AMOUNT, PMGSY_SCHEME) {

    //var Parameters = arrParameters;//.split("$");
    var STA_SANCTIONED_BY = STA_SANCTIONED_BY.replace("$", ' ');
    var STA_INSTITUTE_NAME = INSTITUTE_NAME.replace(/\$/g, ' ');

    // var STA_INSTITUTE_NAME = INSTITUTE_NAME.trim();

    $('#accordion').show('slow', function () {
        blockPage();

        $.validator.unobtrusive.parse($('#frmStaInvoice'));

        $.ajax({
            url: "/StaPay/AddStaInvoiceDetails/",
            type: "GET",
            cache: false,
            data: { MAST_STATE_ID: MAST_STATE_CODE, IMS_YEAR: IMS_YEAR, IMS_BATCH: IMS_BATCH, IMS_STREAM: IMS_STREAM, IMS_PROPOSAL_TYPE: IMS_PROPOSAL_TYPE, STA_SANCTIONED_BY: STA_SANCTIONED_BY, STA_INSTITUTE_NAME: STA_INSTITUTE_NAME, HON_AMOUNT: HON_AMOUNT, PMGSY_SCHEME: PMGSY_SCHEME, value: Math.random() },
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
                $("#divAddStaInvoice").html(response);
            }
        });

        $('#divAddStaInvoice').show('slow');
        $("#divAddStaInvoice").css('height', 'auto');
    });

    unblockPage();

    jQuery('#tbStaPaymentList').jqGrid('setGridState', 'hidden');
    jQuery('#tbStaInvoiceList').jqGrid('setGridState', 'hidden');
}

function ListInvoiceDetails(MAST_STATE_CODE, IMS_YEAR, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, STA_SANCTIONED_BY, STA_INSTITUTE_NAME, HON_AMOUNT, PMGSY_SCHEME) {
    $('#tbStaInvoiceList').jqGrid('GridUnload');
    blockPage();
    jQuery("#tbStaInvoiceList").jqGrid({
        url: '/StaPay/ListGeneratedInvoice',
        datatype: "json",
        mtype: "POST",
        colNames: ["STA Institute Name", "Honorarium Amount [In Rs.]", "Invoice Number", "Penalty Amount [In Rs.]", "TDS Amount [In Rs.]", "SC AMOUNT [In Rs.]", "Service Tax Amount (In Rs.)", "Amount Paid [In Rs.]", "Generation Date", "View", "Delete","BalanceAmount"],
        colModel: [

                    { name: 'STAInstituteName', index: 'STAInstituteName', width: 180, sortable: false, align: "center" },
                    { name: 'HonorariumAmount', index: 'HonorariumAmount', width: 120, sortable: false, align: "center" /*, summaryType: 'sum' */ },
                    { name: 'InvoiceNumber', index: 'InvoiceNumber', width: 120, sortable: false, align: "center" },
                    { name: 'PenaltyAmount', index: 'PenaltyAmount', width: 120, sortable: false, align: "center" },
                    { name: 'TDSAmount', index: 'TDSAmount', width: 90, sortable: false, align: "center" },
                    { name: 'SCAMOUNT', index: 'SCAMOUNT', width: 90, sortable: false, align: "center" },
                    { name: 'ServiceAMOUNT', index: 'ServiceAMOUNT', width: 90, sortable: false, align: "center" },
                    { name: 'AmountPaid', index: 'AmountPaid', width: 90, sortable: false, align: "center" },
                    { name: 'GenerationDate', index: 'GenerationDate', width: 100, sortable: false, align: "center" },
                    { name: 'View', index: 'View', width: 40, sortable: false, align: "center" },
                    { name: 'Delete', width: '50px', sortable: false, resize: false, align: "center" },
                    { name: 'BalanceAmount', index: 'BalanceAmount', width: 40, sortable: false, align: "center", hidden: true }
        ],
        postData: { "MAST_STATE_ID": MAST_STATE_CODE, "IMS_YEAR": IMS_YEAR, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "STA_SANCTIONED_BY": STA_SANCTIONED_BY, "STA_INSTITUTE_NAME": STA_INSTITUTE_NAME, "HON_AMOUNT": HON_AMOUNT, "PMGSY_SCHEME": PMGSY_SCHEME },
        pager: jQuery('#dvStaInvoicePager'),
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

            var grid = $("#tbStaInvoiceList");
            HonorariumAmount = grid.jqGrid('getCol', 'HonorariumAmount', false, 'sum');
            HonorariumAmount = parseFloat(HonorariumAmount).toFixed(2);
            //grid.jqGrid('footerData', 'set', { 'HonorariumAmount': HonorariumAmount });
            grid.jqGrid('footerData', 'set', { 'HonorariumAmount': data.TotalModel.DIS_TOTAL_HONORARIUM_AMOUNT });

            PenaltyAmount = grid.jqGrid('getCol', 'PenaltyAmount', false, 'sum');
            PenaltyAmount = parseFloat(PenaltyAmount).toFixed(2);
            // grid.jqGrid('footerData', 'set', { 'PenaltyAmount': PenaltyAmount });
            grid.jqGrid('footerData', 'set', { 'PenaltyAmount': data.TotalModel.DIS_TOTAL_PENALTY_AMOUNT });

            TDSAmount = grid.jqGrid('getCol', 'TDSAmount', false, 'sum');
            TDSAmount = parseFloat(TDSAmount).toFixed(2);
            grid.jqGrid('footerData', 'set', { 'TDSAmount': data.TotalModel.DIS_TOTAL_TDS_AMOUNT });

            SCAMOUNT = grid.jqGrid('getCol', 'SCAMOUNT', false, 'sum');
            SCAMOUNT = parseFloat(SCAMOUNT).toFixed(2);
            grid.jqGrid('footerData', 'set', { 'SCAMOUNT': data.TotalModel.DIS_TOTAL_SC_AMOUNT });

            ServiceAMOUNT = grid.jqGrid('getCol', 'ServiceAMOUNT', false, 'sum');
            ServiceAMOUNT = parseFloat(ServiceAMOUNT).toFixed(2);
            grid.jqGrid('footerData', 'set', { 'ServiceAMOUNT': data.TotalModel.DIS_TOTAL_SERVICE_TAX_AMOUNT });

            AmountPaid = grid.jqGrid('getCol', 'AmountPaid', false, 'sum');
            AmountPaid = parseFloat(AmountPaid).toFixed(2);
            // grid.jqGrid('footerData', 'set', { 'AmountPaid': AmountPaid });
            grid.jqGrid('footerData', 'set', { 'AmountPaid': data.TotalModel.DIS_TOTAL_AMOUNT });
            grid.jqGrid('footerData', 'set', { 'STAInstituteName': 'Total' });

            $("#tbStaInvoiceList #dvStaInvoicePager").css({ height: '31px' });
            $("#dvStaInvoicePager_left").html("<input type='button' style='margin-left:27px' id='idGenerateInvoice' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = AddInvoiceDetails(\"" + MAST_STATE_CODE + "\",\"" + IMS_YEAR + "\",\"" + IMS_BATCH + "\",\"" + IMS_STREAM + "\",\"" + IMS_PROPOSAL_TYPE + "\",\"" + STA_SANCTIONED_BY.replace(/\s+/g, '$') + "\",\"" + STA_INSTITUTE_NAME.replace(/\s+/g, '$') + "\",\"" + HON_AMOUNT + "\",\"" + PMGSY_SCHEME + "\"); value='Generate Invoice'/>")
            if (HonorariumAmount > 0) {
                $('#idGenerateInvoice').hide();
            }
            unblockPage();
        },
        loadError: function (xhr, status, error) {
            unblockPage();
        }
    }); //end of grid   
    unblockPage();
}


function ShowDetails(param) {

    //********* OLD HTML REPORT START***********/
    //jQuery('#tbStaPaymentList').jqGrid('setSelection', param);

    ////Clear the contents of Div
    //$('#accordion div').html('');

    ////Set Text of Clicked href
    //$("#accordion h3").html(
    //                    '<a href="#"> STA Payment Details<a href="#" style="float: right;">' +
    //                    '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseDetails();" /></a>'
    //                    );


    //$('#accordion').show('fold', function () {
    //    blockPage();
    //  //  $("#divAddStaInvoice").load("/StaPay/StaPaymentReport/" + param, function () { unblockPage(); }); //HTML View Call

    //    //$('#divAddStaInvoice').show('slow'); //HTML View Call
    //    //$("#divAddStaInvoice").css('height', 'auto');//HTML View Call
    //});

    //jQuery('#tbStaPaymentList').jqGrid('setGridState', 'hidden');//HTML View Call
    //jQuery('#tbStaInvoiceList').jqGrid('setGridState', 'hidden');//HTML View Call

    //********* OLD HTML REPORT END***********/

    //*************New of SSRS Report PDF as Created*************//
    window.open('/StaPay/StaPaymentSSRSReport?id=' + param, '_blank');



}
function PreviewSTAPaymentList() {
    var minHonValue;
    if (($("#TOT_HON_OF_SCRUTINY").val() * 100000 < $("#TOT_HON_MIN").val())) {
        minHonValue = ($("#TOT_HON_MIN").val() / 100000);
    }
    else {
        minHonValue = $("#TOT_HON_OF_SCRUTINY").val();
    }


    window.open('/StaPay/PreviewSTAPayment?id=' + $("#ddlState").val() + "$" + $("#ddlImsYear").val() + "$" + $("#ddlImsBatch").val() + "$" + $("#ddlImsStreams").val() + "$" + $("#ddlImsProposalTypes").val() + "$" + minHonValue + "$" + $("#ddlImsProposalSchemes").val() + "$" + $("#lblScrHon").text() + "$" + $("#lblScrAmount").text() + "$" + $("#TOT_HON_MIN").val(), '_blank');
}
function DeleteDetails(invoiceID)
{
    $.ajax({

        type: 'POST',
        url: '/StaPay/DeleteGeneratedInvoice?invoiceID=' + invoiceID,
        async: false,
        cache: false,
        success: function (data) {
            if (data.Success == true)
            {
                alert('Invoice details deleted successfully.');
            }
            else if (data.Success == false)
            {
                alert(data.ErrorMessage);
            }
            $("#tbStaInvoiceList").trigger('reloadGrid');
        },
        error: function () { }
    });
}