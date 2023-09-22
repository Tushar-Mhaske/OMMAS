$(function () {
    if ($("#hdnLevelId").val() == 6) //mord
    {
        HY1StateReportListing();
    }
    else if ($("#hdnLevelId").val() == 4) //state
    {
        HY1DistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val());
    }

    //$('#tblRptContents').bind('resize', function () {
    //    resizeJqGrid();
    //}).trigger('resize');
});


/*       STATE REPORT LISTING       */
function HY1StateReportListing() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#HY1StateReportTable").jqGrid('GridUnload');
    $("#HY1DistrictReportTable").jqGrid('GridUnload');

    $("#HY1StateReportTable").jqGrid({
        url: '/ProposalReports/HY1StateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Sanctioned Year', 'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost', 'Month of Clearance Min/Max'],
        colModel: [
            { name: "MAST_STATE_NAME", width: 200, align: 'left', height: 'auto', sortable: false },
            { name: "IMS_YEAR", width: 100, align: 'left', height: 'auto', sortable: false },
            { name: "TOTAL_PROPOSALS", width: 100, align: 'right', height: 'auto',sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_PROPOSALS_3", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN_3", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT_3", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_PROPOSALS_6", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN_6", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT_6", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_PROPOSALS_9", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN_9", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT_9", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_PROPOSAL_BAL", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN_BAL", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT_BAL", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MIN_MAX", width: 150, align: 'left', height: 'auto', sortable: false }
        ],
        pager: $("#HY1StateReportPager"),
        footerrow: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: false,
        height: '580',
        width: '1120',
        shrinkToFit: false,
        viewrecords: true,
        grouping: true,
        groupingView: {
            groupField: ['MAST_STATE_NAME'],
            groupSummary: [true],
            groupColumnShow: [false],
            groupDataSorted: true,
            showSummaryOnHide: true,
            //   groupText: ['<b>{0}- {1} </b>'],
            groupCollapse: true,
        },
        caption: 'State MPR Half Yearly I Report Details',
        loadComplete: function () {
            //Total of Columns
            var TOTAL_PROPOSALS_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS', false, 'sum');
            var TOTAL_LEN_T = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            TOTAL_LEN_T = parseFloat(TOTAL_LEN_T).toFixed(3);
            var TOTAL_AMT_T = $(this).jqGrid('getCol', 'TOTAL_AMT', false, 'sum');
            TOTAL_AMT_T = parseFloat(TOTAL_AMT_T).toFixed(2);
            var TOTAL_PROPOSALS_3_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS_3', false, 'sum');
            var TOTAL_LEN_3_T = $(this).jqGrid('getCol', 'TOTAL_LEN_3', false, 'sum');
            TOTAL_LEN_3_T = parseFloat(TOTAL_LEN_3_T).toFixed(3);
            var TOTAL_AMT_3_T = $(this).jqGrid('getCol', 'TOTAL_AMT_3', false, 'sum');
            TOTAL_AMT_3_T = parseFloat(TOTAL_AMT_3_T).toFixed(2);
            var TOTAL_PROPOSALS_6_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS_6', false, 'sum');
            var TOTAL_LEN_6_T = $(this).jqGrid('getCol', 'TOTAL_LEN_6', false, 'sum');
            TOTAL_LEN_6_T = parseFloat(TOTAL_LEN_6_T).toFixed(3);
            var TOTAL_AMT_6_T = $(this).jqGrid('getCol', 'TOTAL_AMT_6', false, 'sum');
            TOTAL_AMT_6_T = parseFloat(TOTAL_AMT_6_T).toFixed(2);
            var TOTAL_PROPOSALS_9_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS_9', false, 'sum');
            var TOTAL_LEN_9_T = $(this).jqGrid('getCol', 'TOTAL_LEN_9', false, 'sum');
            TOTAL_LEN_9_T = parseFloat(TOTAL_LEN_9_T).toFixed(3);
            var TOTAL_AMT_9_T = $(this).jqGrid('getCol', 'TOTAL_AMT_9', false, 'sum');
            TOTAL_AMT_9_T = parseFloat(TOTAL_AMT_9_T).toFixed(2);
            var TOTAL_PROPOSAL_BAL_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSAL_BAL', false, 'sum');
            var TOTAL_LEN_BAL_T = $(this).jqGrid('getCol', 'TOTAL_LEN_BAL', false, 'sum');
            TOTAL_LEN_BAL_T = parseFloat(TOTAL_LEN_BAL_T).toFixed(3);
            var TOTAL_AMT_BAL_T = $(this).jqGrid('getCol', 'TOTAL_AMT_BAL', false, 'sum');
            TOTAL_AMT_BAL_T = parseFloat(TOTAL_AMT_BAL_T).toFixed(2);
            //  var MIN_MAX_T = $(this).jqGrid('getCol', 'MIN_MAX', false, 'sum');
           
            //

            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS: TOTAL_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT: TOTAL_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS_3: TOTAL_PROPOSALS_3_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN_3: TOTAL_LEN_3_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT_3: TOTAL_AMT_3_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS_6: TOTAL_PROPOSALS_6_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN_6: TOTAL_LEN_6_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT_6: TOTAL_AMT_6_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS_9: TOTAL_PROPOSALS_9_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN_9: TOTAL_LEN_9_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT_9: TOTAL_AMT_9_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSAL_BAL: TOTAL_PROPOSAL_BAL_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN_BAL: TOTAL_LEN_BAL_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT_BAL: TOTAL_AMT_BAL_T }, true);
          //  $(this).jqGrid('footerData', 'set', { MIN_MAX: MIN_MAX_T }, true);
            $("#HY1StateReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Amount Rs in Lacs & Length in Kms.</font>");
            $('#HY1StateReportTable_rn').html('Sr.<br/>No.');


            $.unblockUI();
        },
        onSelectRow: function (id) {
            //alert(id);
            var params = id.split('$');
            HY1DistrictReportListing(params[0], params[1]);

        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }
    });

    $("#HY1StateReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'TOTAL_PROPOSALS', numberOfColumns: 3, titleText: '<em>Cleared Works</em>' },
          { startColumnName: 'TOTAL_PROPOSALS_3', numberOfColumns: 3, titleText: '<em> No. of Works Awarded within 3 Months of Clearance</em>' },
          { startColumnName: 'TOTAL_PROPOSALS_6', numberOfColumns: 3, titleText: '<em> No. of Works Awarded within 6 Months of Clearance</em>' },
          { startColumnName: 'TOTAL_PROPOSALS_9', numberOfColumns: 3, titleText: '<em> No. of Works Awarded within 9 Months of Clearance</em>' },
          { startColumnName: 'TOTAL_PROPOSAL_BAL', numberOfColumns: 3, titleText: '<em>Balance Works to be Awarded</em>' }
        ]
    });
}
/**/

/*       DISTRICT REPORT LISTING       */
function HY1DistrictReportListing(stateCode, stateName) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#HY1StateReportTable").jqGrid('setGridState', 'hidden');
    $("#HY1StateReportTable").jqGrid('setSelection', stateCode);
    $("#HY1DistrictReportTable").jqGrid('GridUnload');

    $("#HY1DistrictReportTable").jqGrid({
        url: '/ProposalReports/HY1DistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Sanctioned Year', 'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost', 'Month of Clearance Min/Max'],
        colModel: [
            { name: "MAST_DISTRICT_NAME", width: 200, align: 'left', height: 'auto', sortable: false },
            { name: "IMS_YEAR", width: 100, align: 'left', height: 'auto', sortable: false },
            { name: "TOTAL_PROPOSALS", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_PROPOSALS_3", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN_3", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT_3", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_PROPOSALS_6", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN_6", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT_6", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_PROPOSALS_9", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN_9", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT_9", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_PROPOSAL_BAL", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN_BAL", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT_BAL", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MIN_MAX", width: 150, align: 'left', height: 'auto', sortable: false }
        ],
        postData: { "StateCode": stateCode },
        pager: $("#HY1DistrictReportPager"),
        footerrow: true,
        sortname: 'MAST_DISTRICT_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: false,
        height: '550',
        width: '1120',
        shrinkToFit: false,
        viewrecords: true,
        grouping: true,
        groupingView: {
            groupField: ['MAST_DISTRICT_NAME'],
            groupSummary: [true],
            groupColumnShow: [false],
            groupDataSorted: true,
            showSummaryOnHide: true,
            //   groupText: ['<b>{0}- {1} </b>'],
            groupCollapse: true,
        },
        caption: 'District MPR Half Yearly I Report for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_PROPOSALS_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS', false, 'sum');
            var TOTAL_LEN_T = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            TOTAL_LEN_T = parseFloat(TOTAL_LEN_T).toFixed(3);
            var TOTAL_AMT_T = $(this).jqGrid('getCol', 'TOTAL_AMT', false, 'sum');
            TOTAL_AMT_T = parseFloat(TOTAL_AMT_T).toFixed(2);
            var TOTAL_PROPOSALS_3_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS_3', false, 'sum');
            var TOTAL_LEN_3_T = $(this).jqGrid('getCol', 'TOTAL_LEN_3', false, 'sum');
            TOTAL_LEN_3_T = parseFloat(TOTAL_LEN_3_T).toFixed(3);
            var TOTAL_AMT_3_T = $(this).jqGrid('getCol', 'TOTAL_AMT_3', false, 'sum');
            TOTAL_AMT_3_T = parseFloat(TOTAL_AMT_3_T).toFixed(2);
            var TOTAL_PROPOSALS_6_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS_6', false, 'sum');
            var TOTAL_LEN_6_T = $(this).jqGrid('getCol', 'TOTAL_LEN_6', false, 'sum');
            TOTAL_LEN_6_T = parseFloat(TOTAL_LEN_6_T).toFixed(3);
            var TOTAL_AMT_6_T = $(this).jqGrid('getCol', 'TOTAL_AMT_6', false, 'sum');
            TOTAL_AMT_6_T = parseFloat(TOTAL_AMT_6_T).toFixed(2);
            var TOTAL_PROPOSALS_9_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS_9', false, 'sum');
            var TOTAL_LEN_9_T = $(this).jqGrid('getCol', 'TOTAL_LEN_9', false, 'sum');
            TOTAL_LEN_9_T = parseFloat(TOTAL_LEN_9_T).toFixed(3);
            var TOTAL_AMT_9_T = $(this).jqGrid('getCol', 'TOTAL_AMT_9', false, 'sum');
            TOTAL_AMT_9_T = parseFloat(TOTAL_AMT_9_T).toFixed(2);
            var TOTAL_PROPOSAL_BAL_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSAL_BAL', false, 'sum');
            var TOTAL_LEN_BAL_T = $(this).jqGrid('getCol', 'TOTAL_LEN_BAL', false, 'sum');
            TOTAL_LEN_BAL_T = parseFloat(TOTAL_LEN_BAL_T).toFixed(3);
            var TOTAL_AMT_BAL_T = $(this).jqGrid('getCol', 'TOTAL_AMT_BAL', false, 'sum');
            TOTAL_AMT_BAL_T = parseFloat(TOTAL_AMT_BAL_T).toFixed(2);
            //  var MIN_MAX_T = $(this).jqGrid('getCol', 'MIN_MAX', false, 'sum');

            //

            $(this).jqGrid('footerData', 'set', { MAST_DISTRICT_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS: TOTAL_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT: TOTAL_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS_3: TOTAL_PROPOSALS_3_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN_3: TOTAL_LEN_3_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT_3: TOTAL_AMT_3_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS_6: TOTAL_PROPOSALS_6_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN_6: TOTAL_LEN_6_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT_6: TOTAL_AMT_6_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS_9: TOTAL_PROPOSALS_9_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN_9: TOTAL_LEN_9_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT_9: TOTAL_AMT_9_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSAL_BAL: TOTAL_PROPOSAL_BAL_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN_BAL: TOTAL_LEN_BAL_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT_BAL: TOTAL_AMT_BAL_T }, true);
            //  $(this).jqGrid('footerData', 'set', { MIN_MAX: MIN_MAX_T }, true);
            $("#HY1DistrictReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Cost Rs in Lacs & Length in Kms.</font>");
            $('#HY1DistrictReportTable_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }
    });

    $("#HY1DistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'TOTAL_PROPOSALS', numberOfColumns: 3, titleText: '<em>Cleared Works</em>' },
          { startColumnName: 'TOTAL_PROPOSALS_3', numberOfColumns: 3, titleText: '<em> No. of Works Awarded within 3 Months of Clearance</em>' },
          { startColumnName: 'TOTAL_PROPOSALS_6', numberOfColumns: 3, titleText: '<em> No. of Works Awarded within 6 Months of Clearance</em>' },
          { startColumnName: 'TOTAL_PROPOSALS_9', numberOfColumns: 3, titleText: '<em> No. of Works Awarded within 9 Months of Clearance</em>' },
          { startColumnName: 'TOTAL_PROPOSAL_BAL', numberOfColumns: 3, titleText: '<em>Balance Works to be Awarded</em>' }
        ]
    });
}
/**/