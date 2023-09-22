$(function () {
    if ($("#hdnLevelId").val() == 6) //mord
    {
        HY2StateReportListing();
    }
    else if ($("#hdnLevelId").val() == 4) //state
    {
        HY2DistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val());
    }

    //$('#tblRptContents').bind('resize', function () {
    //    resizeJqGrid();
    //}).trigger('resize');
});


/*       STATE REPORT LISTING       */
function HY2StateReportListing() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#HY2StateReportTable").jqGrid('GridUnload');
    $("#HY2DistrictReportTable").jqGrid('GridUnload');
    $("#HY2StateReportTable").jqGrid({
        url: '/ProposalReports/HY2StateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Sanctioned Year', 'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost', 'Levied', 'Recovered', 'Remarks'],
        colModel: [
            { name: "MAST_STATE_NAME", width: 200, align: 'left', height: 'auto', sortable: false },
            { name: "IMS_YEAR", width: 100, align: 'left', height: 'auto', sortable: false },
            { name: "TOTAL_PROPOSALS", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_PROPOSALS_0", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN_0", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT_0", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_PROPOSALS_6", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN_6", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT_6", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_PROPOSALS_9", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN_9", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT_9", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_PROPOSALS_12", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN_12", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT_12", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_PROPOSAL_BAL", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN_BAL", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT_BAL", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "LEVIED", width: 100, align: 'left', height: 'auto', sortable: false },
            { name: "RECOVERED", width: 100, align: 'left', height: 'auto', sortable: false },
            { name: "REMARKS", width: 100, align: 'left', height: 'auto', sortable: false }
        ],
        pager: $("#HY2StateReportPager"),
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
        caption: 'State MPR Half Yearly II Report  Details',
        loadComplete: function () {
            //Total of Columns
            var TOTAL_PROPOSALS_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS', false, 'sum');
            var TOTAL_LEN_T = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            TOTAL_LEN_T = parseFloat(TOTAL_LEN_T).toFixed(3);
            var TOTAL_AMT_T = $(this).jqGrid('getCol', 'TOTAL_AMT', false, 'sum');
            TOTAL_AMT_T = parseFloat(TOTAL_AMT_T).toFixed(2);
            var TOTAL_PROPOSALS_0_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS_0', false, 'sum');
            var TOTAL_LEN_0_T = $(this).jqGrid('getCol', 'TOTAL_LEN_0', false, 'sum');
            TOTAL_LEN_0_T = parseFloat(TOTAL_LEN_0_T).toFixed(3);
            var TOTAL_AMT_0_T = $(this).jqGrid('getCol', 'TOTAL_AMT_0', false, 'sum');
            TOTAL_AMT_0_T = parseFloat(TOTAL_AMT_0_T).toFixed(2);
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
            var TOTAL_PROPOSALS_12_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS_12', false, 'sum');
            var TOTAL_LEN_12_T = $(this).jqGrid('getCol', 'TOTAL_LEN_12', false, 'sum');
            TOTAL_LEN_12_T = parseFloat(TOTAL_LEN_12_T).toFixed(3);
            var TOTAL_AMT_12_T = $(this).jqGrid('getCol', 'TOTAL_AMT_12', false, 'sum');
            TOTAL_AMT_12_T = parseFloat(TOTAL_AMT_12_T).toFixed(2);
            var TOTAL_PROPOSAL_BAL_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSAL_BAL', false, 'sum');
            var TOTAL_LEN_BAL_T = $(this).jqGrid('getCol', 'TOTAL_LEN_BAL', false, 'sum');
            TOTAL_LEN_BAL_T = parseFloat(TOTAL_LEN_BAL_T).toFixed(3);
            var TOTAL_AMT_BAL_T = $(this).jqGrid('getCol', 'TOTAL_AMT_BAL', false, 'sum');
            TOTAL_AMT_BAL_T = parseFloat(TOTAL_AMT_BAL_T).toFixed(2);

            //

            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS: TOTAL_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT: TOTAL_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS_0: TOTAL_PROPOSALS_0_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN_0: TOTAL_LEN_0_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT_0: TOTAL_AMT_0_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS_6: TOTAL_PROPOSALS_6_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN_6: TOTAL_LEN_6_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT_6: TOTAL_AMT_6_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS_9: TOTAL_PROPOSALS_9_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN_9: TOTAL_LEN_9_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT_9: TOTAL_AMT_9_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS_12: TOTAL_PROPOSALS_12_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN_12: TOTAL_LEN_12_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT_12: TOTAL_AMT_12_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSAL_BAL: TOTAL_PROPOSAL_BAL_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN_BAL: TOTAL_LEN_BAL_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT_BAL: TOTAL_AMT_BAL_T }, true);
            $("#HY2StateReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Cost Rs in Lacs & Length in Kms.</font>");
            $('#HY2StateReportTable_rn').html('Sr.<br/>No.');


            $.unblockUI();
        },
        onSelectRow: function (id) {
            //alert(id);
            var params = id.split('$');
            HY2DistrictReportListing(params[0], params[1]);

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

    $("#HY2StateReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'TOTAL_PROPOSALS', numberOfColumns: 3, titleText: '<em>Sanctioned Works</em>' },
          { startColumnName: 'TOTAL_PROPOSALS_0', numberOfColumns: 3, titleText: '<em> Works Completed within Stipulated Time</em>' },
          { startColumnName: 'TOTAL_PROPOSALS_6', numberOfColumns: 3, titleText: '<em> Works Completed with Delay upto 6 Months</em>' },
          { startColumnName: 'TOTAL_PROPOSALS_9', numberOfColumns: 3, titleText: '<em>  Works Completed with Delay of 6-9 Months</em>' },
          { startColumnName: 'TOTAL_PROPOSALS_12', numberOfColumns: 3, titleText: '<em>  Works Completed with Delay of >9 Months</em>' },
          { startColumnName: 'LEVIED', numberOfColumns: 2, titleText: '<em>Liquidated Damages</em>' },
          { startColumnName: 'TOTAL_PROPOSAL_BAL', numberOfColumns: 3, titleText: '<em>Balance Works to be Awarded</em>' }
        ]
    });
}
/**/

/*       DISTRICT REPORT LISTING       */
function HY2DistrictReportListing(stateCode, stateName) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#HY2StateReportTable").jqGrid('setGridState', 'hidden');
    $("#HY2StateReportTable").jqGrid('setSelection', stateCode);
    $("#HY2DistrictReportTable").jqGrid('GridUnload');

    $("#HY2DistrictReportTable").jqGrid({
        url: '/ProposalReports/HY2DistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Sanctioned Year', 'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost', 'Levied', 'Recovered', 'Remarks'],
        colModel: [
            { name: "MAST_DISTRICT_NAME", width: 200, align: 'left', height: 'auto', sortable: false },
             { name: "IMS_YEAR", width: 100, align: 'left', height: 'auto', sortable: false },
            { name: "TOTAL_PROPOSALS", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_PROPOSALS_0", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN_0", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT_0", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_PROPOSALS_6", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN_6", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT_6", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_PROPOSALS_9", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN_9", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT_9", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_PROPOSALS_12", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN_12", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT_12", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_PROPOSAL_BAL", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LEN_BAL", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT_BAL", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "LEVIED", width: 100, align: 'left', height: 'auto', sortable: false },
            { name: "RECOVERED", width: 100, align: 'left', height: 'auto', sortable: false },
            { name: "REMARKS", width: 100, align: 'left', height: 'auto', sortable: false }
        ],
        postData: { "StateCode": stateCode },
        pager: $("#HY2DistrictReportPager"),
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
        caption: 'District MPR Half Yearly II Report for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_PROPOSALS_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS', false, 'sum');
            var TOTAL_LEN_T = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            TOTAL_LEN_T = parseFloat(TOTAL_LEN_T).toFixed(3);
            var TOTAL_AMT_T = $(this).jqGrid('getCol', 'TOTAL_AMT', false, 'sum');
            TOTAL_AMT_T = parseFloat(TOTAL_AMT_T).toFixed(2);
            var TOTAL_PROPOSALS_0_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS_0', false, 'sum');
            var TOTAL_LEN_0_T = $(this).jqGrid('getCol', 'TOTAL_LEN_0', false, 'sum');
            TOTAL_LEN_0_T = parseFloat(TOTAL_LEN_0_T).toFixed(3);
            var TOTAL_AMT_0_T = $(this).jqGrid('getCol', 'TOTAL_AMT_0', false, 'sum');
            TOTAL_AMT_0_T = parseFloat(TOTAL_AMT_0_T).toFixed(2);
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
            var TOTAL_PROPOSALS_12_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS_12', false, 'sum');
            var TOTAL_LEN_12_T = $(this).jqGrid('getCol', 'TOTAL_LEN_12', false, 'sum');
            TOTAL_LEN_12_T = parseFloat(TOTAL_LEN_12_T).toFixed(3);
            var TOTAL_AMT_12_T = $(this).jqGrid('getCol', 'TOTAL_AMT_12', false, 'sum');
            TOTAL_AMT_12_T = parseFloat(TOTAL_AMT_12_T).toFixed(2);
            var TOTAL_PROPOSAL_BAL_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSAL_BAL', false, 'sum');
            var TOTAL_LEN_BAL_T = $(this).jqGrid('getCol', 'TOTAL_LEN_BAL', false, 'sum');
            TOTAL_LEN_BAL_T = parseFloat(TOTAL_LEN_BAL_T).toFixed(3);
            var TOTAL_AMT_BAL_T = $(this).jqGrid('getCol', 'TOTAL_AMT_BAL', false, 'sum');
            TOTAL_AMT_BAL_T = parseFloat(TOTAL_AMT_BAL_T).toFixed(2);
            //

            $(this).jqGrid('footerData', 'set', { MAST_DISTRICT_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS: TOTAL_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT: TOTAL_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS_0: TOTAL_PROPOSALS_0_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN_0: TOTAL_LEN_0_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT_0: TOTAL_AMT_0_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS_6: TOTAL_PROPOSALS_6_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN_6: TOTAL_LEN_6_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT_6: TOTAL_AMT_6_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS_9: TOTAL_PROPOSALS_9_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN_9: TOTAL_LEN_9_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT_9: TOTAL_AMT_9_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS_12: TOTAL_PROPOSALS_12_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN_12: TOTAL_LEN_12_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT_12: TOTAL_AMT_12_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSAL_BAL: TOTAL_PROPOSAL_BAL_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN_BAL: TOTAL_LEN_BAL_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT_BAL: TOTAL_AMT_BAL_T }, true);
            $("#HY2DistrictReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Cost Rs in Lacs & Length in Kms.</font>");
            $('#HY2DistrictReportTable_rn').html('Sr.<br/>No.');


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
    $("#HY2DistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'TOTAL_PROPOSALS', numberOfColumns: 3, titleText: '<em>Sanctioned Works</em>' },
          { startColumnName: 'TOTAL_PROPOSALS_0', numberOfColumns: 3, titleText: '<em> Works Completed within Stipulated Time</em>' },
          { startColumnName: 'TOTAL_PROPOSALS_6', numberOfColumns: 3, titleText: '<em> Works Completed with Delay upto 6 Months</em>' },
          { startColumnName: 'TOTAL_PROPOSALS_9', numberOfColumns: 3, titleText: '<em>  Works Completed with Delay of 6-9 Months</em>' },
          { startColumnName: 'TOTAL_PROPOSALS_12', numberOfColumns: 3, titleText: '<em>  Works Completed with Delay of >9 Months</em>' },
          { startColumnName: 'LEVIED', numberOfColumns: 2, titleText: '<em>Liquidated Damages</em>' },
          { startColumnName: 'TOTAL_PROPOSAL_BAL', numberOfColumns: 3, titleText: '<em>Balance Works to be Awarded</em>' }
        ]
    });
}
/**/