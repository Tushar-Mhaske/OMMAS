$(function () {
    $('#btnPropEMCDetails').click(function () {
        var year = $('#ddYear_PropEMCDetails').val();
        var batch = $('#ddBatch_PropEMCDetails').val();
        var collaboration = $('#ddAgency_PropEMCDetails').val();
        var status = "%";
        if ($("#hdnLevelId").val() == 6) //mord
        {
            PropEMCStateReportListing(year, batch, collaboration, status);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
            PropEMCDistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), year, batch, collaboration, status);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {

            PropEMCBlockReportListing($("#MAST_DISTRICT_CODE").val(), $("#MAST_STATE_CODE").val(), $("#DISTRICT_NAME").val(), year, batch, collaboration, status);
        }
    });

    $('#btnPropEMCDetails').trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});

/*       STATE REPORT LISTING       */
function PropEMCStateReportListing(year, batch, collaboration, status) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#PropEMCStateReportTable").jqGrid('GridUnload');
    $("#PropEMCDistrictReportTable").jqGrid('GridUnload');
    $("#PropEMCBlockReportTable").jqGrid('GridUnload');
    $("#PropEMCFinalReportTable").jqGrid('GridUnload');

    $("#PropEMCStateReportTable").jqGrid({
        url: '/ProposalReports/PropEMCStateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Total Proposal', 'Road Cost ', 'Bridge Cost ', 'Year1 ', 'Year2 ', 'Year3 ', 'Year4 ', 'Year5 ', 'Total Maintenance Cost', 'Total Cost'],
        colModel: [
            { name: "MAST_STATE_NAME", width: 220, align: 'left',  height: 'auto', sortable: true },
            { name: "TOTAL_PROPOSALS", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "ROAD_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "BRIDGE_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT1_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT2_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT3_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT4_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT5_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },

        ],
        postData: { "Year": year, "Batch": batch, "Collaboration": collaboration, "Status": status },
        pager: $("#PropEMCStateReportPager"),
        footerrow: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '520',
        viewrecords: true,
        caption: 'State Estimated Maintenance Cost Details',
        loadComplete: function () {


            //Total of Columns
            var TOTAL_PROPOSALS_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS', false, 'sum');
            var ROAD_AMT_T = $(this).jqGrid('getCol', 'ROAD_AMT', false, 'sum');
            ROAD_AMT_T = parseFloat(ROAD_AMT_T).toFixed(2);
            var BRIDGE_AMT_T = $(this).jqGrid('getCol', 'BRIDGE_AMT', false, 'sum');
            BRIDGE_AMT_T = parseFloat(BRIDGE_AMT_T).toFixed(2);
            var MAINT1_AMT_T = $(this).jqGrid('getCol', 'MAINT1_AMT', false, 'sum');
            MAINT1_AMT_T = parseFloat(MAINT1_AMT_T).toFixed(2);
            var MAINT2_AMT_T = $(this).jqGrid('getCol', 'MAINT2_AMT', false, 'sum');
            MAINT2_AMT_T = parseFloat(MAINT2_AMT_T).toFixed(2);
            var MAINT3_AMT_T = $(this).jqGrid('getCol', 'MAINT3_AMT', false, 'sum');
            MAINT3_AMT_T = parseFloat(MAINT3_AMT_T).toFixed(2);
            var MAINT4_AMT_T = $(this).jqGrid('getCol', 'MAINT4_AMT', false, 'sum');
            MAINT4_AMT_T = parseFloat(MAINT4_AMT_T).toFixed(2);
            var MAINT5_AMT_T = $(this).jqGrid('getCol', 'MAINT5_AMT', false, 'sum');
            MAINT5_AMT_T = parseFloat(MAINT5_AMT_T).toFixed(2);
            var MAINT_AMT_T = $(this).jqGrid('getCol', 'MAINT_AMT', false, 'sum');
            MAINT_AMT_T = parseFloat(MAINT_AMT_T).toFixed(2);
            var TOTAL_AMT_T = $(this).jqGrid('getCol', 'TOTAL_AMT', false, 'sum');
            TOTAL_AMT_T = parseFloat(TOTAL_AMT_T).toFixed(2);
            //
            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS: TOTAL_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_AMT: ROAD_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { BRIDGE_AMT: BRIDGE_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT1_AMT: MAINT1_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT2_AMT: MAINT2_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT3_AMT: MAINT3_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT4_AMT: MAINT4_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT5_AMT: MAINT5_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT_AMT: MAINT_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT: TOTAL_AMT_T }, true);
            $('#PropEMCStateReportTable_rn').html('Sr.<br/>No.');
            $("#PropEMCStateReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Cost Rs in Lacs</font>");

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
    });/* End of Grid*/
    $("#PropEMCStateReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'MAINT1_AMT', numberOfColumns: 5, titleText: '<em> Estimated Maintenance Cost</em>' },

        ]
    });
}
/**/

/*       DISTRICT REPORT LISTING       */
function PropEMCDistrictReportListing(stateCode, stateName, year, batch, collaboration, status) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#PropEMCStateReportTable").jqGrid('setGridState', 'hidden');
    $("#PropEMCStateReportTable").jqGrid('setSelection', stateCode);
    $("#PropEMCDistrictReportTable").jqGrid('GridUnload');
    $("#PropEMCBlockReportTable").jqGrid('GridUnload');
    $("#PropEMCFinalReportTable").jqGrid('GridUnload');

    $("#PropEMCDistrictReportTable").jqGrid({
        url: '/ProposalReports/PropEMCDistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Total Proposal', 'Road Cost ', 'Bridge Cost ', 'Year1 ', 'Year2 ', 'Year3 ', 'Year4 ', 'Year5 ', 'Total Maintenance Cost', 'Total Cost'],
        colModel: [
            { name: "MAST_DISTRICT_NAME", width: 200, align: 'left',  height: 'auto', sortable: true },
            { name: "TOTAL_PROPOSALS", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "ROAD_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "BRIDGE_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT1_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT2_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT3_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT4_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT5_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },

        ],
        postData: { "StateCode": stateCode, "Year": year, "Batch": batch, "Collaboration": collaboration, "Status": status },
        pager: $("#PropEMCDistrictReportPager"),
        footerrow: true,
        sortname: 'MAST_DISTRICT_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '480',
        viewrecords: true,
        caption: 'District Estimated Maintenance Cost for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_PROPOSALS_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS', false, 'sum');
            var ROAD_AMT_T = $(this).jqGrid('getCol', 'ROAD_AMT', false, 'sum');
            ROAD_AMT_T = parseFloat(ROAD_AMT_T).toFixed(2);
            var BRIDGE_AMT_T = $(this).jqGrid('getCol', 'BRIDGE_AMT', false, 'sum');
            BRIDGE_AMT_T = parseFloat(BRIDGE_AMT_T).toFixed(2);
            var MAINT1_AMT_T = $(this).jqGrid('getCol', 'MAINT1_AMT', false, 'sum');
            MAINT1_AMT_T = parseFloat(MAINT1_AMT_T).toFixed(2);
            var MAINT2_AMT_T = $(this).jqGrid('getCol', 'MAINT2_AMT', false, 'sum');
            MAINT2_AMT_T = parseFloat(MAINT2_AMT_T).toFixed(2);
            var MAINT3_AMT_T = $(this).jqGrid('getCol', 'MAINT3_AMT', false, 'sum');
            MAINT3_AMT_T = parseFloat(MAINT3_AMT_T).toFixed(2);
            var MAINT4_AMT_T = $(this).jqGrid('getCol', 'MAINT4_AMT', false, 'sum');
            MAINT4_AMT_T = parseFloat(MAINT4_AMT_T).toFixed(2);
            var MAINT5_AMT_T = $(this).jqGrid('getCol', 'MAINT5_AMT', false, 'sum');
            MAINT5_AMT_T = parseFloat(MAINT5_AMT_T).toFixed(2);
            var MAINT_AMT_T = $(this).jqGrid('getCol', 'MAINT_AMT', false, 'sum');
            MAINT_AMT_T = parseFloat(MAINT_AMT_T).toFixed(2);
            var TOTAL_AMT_T = $(this).jqGrid('getCol', 'TOTAL_AMT', false, 'sum');
            TOTAL_AMT_T = parseFloat(TOTAL_AMT_T).toFixed(2);
            //
            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS: TOTAL_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_AMT: ROAD_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { BRIDGE_AMT: BRIDGE_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT1_AMT: MAINT1_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT2_AMT: MAINT2_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT3_AMT: MAINT3_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT4_AMT: MAINT4_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT5_AMT: MAINT5_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT_AMT: MAINT_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT: TOTAL_AMT_T }, true);
            $("#PropEMCDistrictReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Cost Rs in Lacs</font>");

            $('#PropEMCDistrictReportTable_rn').html('Sr.<br/>No.');

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
    }); /* End of Grid*/

    $("#PropEMCDistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'MAINT1_AMT', numberOfColumns: 5, titleText: '<em> Estimated Maintenance Cost</em>' },

        ]
    });
}
/**/

/*       BLOCK REPORT LISTING       */
function PropEMCBlockReportListing(districtCode, stateCode, districtName, year, batch, collaboration, status) {
    var distName;
    if (districtName == '')
        distName = $("#DISTRICT_NAME").val();
    else
        distName = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#PropEMCDistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#PropEMCStateReportTable").jqGrid('setGridState', 'hidden');
    $("#PropEMCDistrictReportTable").jqGrid('setSelection', stateCode);
    $("#PropEMCBlockReportTable").jqGrid('GridUnload');
    $("#PropEMCFinalReportTable").jqGrid('GridUnload');

    $("#PropEMCBlockReportTable").jqGrid({
        url: '/ProposalReports/PropEMCBlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Total Proposal', 'Road Cost ', 'Bridge Cost ', 'Year1 ', 'Year2 ', 'Year3 ', 'Year4 ', 'Year5 ', 'Total Maintenance Cost', 'Total Cost'],
        colModel: [
            { name: "MAST_BLOCK_NAME", width: 200, align: 'left',  height: 'auto', sortable: true },
            { name: "TOTAL_PROPOSALS", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "ROAD_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "BRIDGE_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT1_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT2_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT3_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT4_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT5_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },

        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "Year": year, "Batch": batch, "Collaboration": collaboration, "Status": status },
        pager: $("#PropEMCBlockReportPager"),
        footerrow: true,
        sortname: 'MAST_BLOCK_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '430',
        viewrecords: true,
        caption: 'Block Estimated Maintenance Cost  for ' + distName,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_PROPOSALS_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS', false, 'sum');
            var ROAD_AMT_T = $(this).jqGrid('getCol', 'ROAD_AMT', false, 'sum');
            ROAD_AMT_T = parseFloat(ROAD_AMT_T).toFixed(2);
            var BRIDGE_AMT_T = $(this).jqGrid('getCol', 'BRIDGE_AMT', false, 'sum');
            BRIDGE_AMT_T = parseFloat(BRIDGE_AMT_T).toFixed(2);
            var MAINT1_AMT_T = $(this).jqGrid('getCol', 'MAINT1_AMT', false, 'sum');
            MAINT1_AMT_T = parseFloat(MAINT1_AMT_T).toFixed(2);
            var MAINT2_AMT_T = $(this).jqGrid('getCol', 'MAINT2_AMT', false, 'sum');
            MAINT2_AMT_T = parseFloat(MAINT2_AMT_T).toFixed(2);
            var MAINT3_AMT_T = $(this).jqGrid('getCol', 'MAINT3_AMT', false, 'sum');
            MAINT3_AMT_T = parseFloat(MAINT3_AMT_T).toFixed(2);
            var MAINT4_AMT_T = $(this).jqGrid('getCol', 'MAINT4_AMT', false, 'sum');
            MAINT4_AMT_T = parseFloat(MAINT4_AMT_T).toFixed(2);
            var MAINT5_AMT_T = $(this).jqGrid('getCol', 'MAINT5_AMT', false, 'sum');
            MAINT5_AMT_T = parseFloat(MAINT5_AMT_T).toFixed(2);
            var MAINT_AMT_T = $(this).jqGrid('getCol', 'MAINT_AMT', false, 'sum');
            MAINT_AMT_T = parseFloat(MAINT_AMT_T).toFixed(2);
            var TOTAL_AMT_T = $(this).jqGrid('getCol', 'TOTAL_AMT', false, 'sum');
            TOTAL_AMT_T = parseFloat(TOTAL_AMT_T).toFixed(2);
            //
            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS: TOTAL_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_AMT: ROAD_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { BRIDGE_AMT: BRIDGE_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT1_AMT: MAINT1_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT2_AMT: MAINT2_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT3_AMT: MAINT3_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT4_AMT: MAINT4_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT5_AMT: MAINT5_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT_AMT: MAINT_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT: TOTAL_AMT_T }, true);
            $('#PropEMCBlockReportTable_rn').html('Sr.<br/>No.');
            $("#PropEMCBlockReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Cost Rs in Lacs</font>");

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

    $("#PropEMCBlockReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'MAINT1_AMT', numberOfColumns: 5, titleText: '<em> Estimated Maintenance Cost</em>' },

        ]
    });
}

/*       FINAL REPORT LISTING       */
function PropEMCFinalReportListing(blockCode, districtCode, stateCode, blockName, year, batch, collaboration, status) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#PropEMCBlockReportTable").jqGrid('setGridState', 'hidden');
    $("#PropEMCDistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#PropEMCStateReportTable").jqGrid('setGridState', 'hidden');
    $("#PropEMCBlockReportTable").jqGrid('setSelection', stateCode);
    $("#PropEMCFinalReportTable").jqGrid('GridUnload');


    $("#PropEMCFinalReportTable").jqGrid({
        url: '/ProposalReports/PropEMCFinalReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Sanctioned Year', 'Batch', 'Package', 'Road Name', 'Upgrade Connect', 'Road Cost ', 'Bridge Cost',
                    'Year1 ', 'Year2 ', 'Year3 ', 'Year4 ', 'Year5', 'Total Maintenance Cost ', 'Total Cost'],
        colModel: [
            { name: "MAST_BLOCK_NAME", width: 180, align: 'left',  height: 'auto', sortable: true },
            { name: "IMS_YEAR", width: 140, align: 'right',  height: 'auto', sortable: false },
            { name: "IMS_BATCH", width: 80, align: 'right',  height: 'auto', sortable: false },
            { name: "IMS_PACKAGE_ID", width: 150, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_ROAD_NAME", width: 250, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_UPGRADE_CONNECT", width: 120, align: 'left',  height: 'auto', sortable: false },
            { name: "ROAD_AMT", width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "BRIDGE_AMT", width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "IMS_SANCTIONED_MAN_AMT1", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "IMS_SANCTIONED_MAN_AMT2", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "IMS_SANCTIONED_MAN_AMT3", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "IMS_SANCTIONED_MAN_AMT4", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "IMS_SANCTIONED_MAN_AMT5", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "Year": year, "Batch": batch, "Collaboration": collaboration, "Status": status },
        pager: $("#PropEMCFinalReportPager"),
        footerrow: true,
        sortname: 'MAST_BLOCK_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '380',
        viewrecords: true,
        caption: 'Estimated Maintenance Cost for' + blockName,
        loadComplete: function () {
            //Total of Columns
            var ROAD_AMT_T = $(this).jqGrid('getCol', 'ROAD_AMT', false, 'sum');
            ROAD_AMT_T = parseFloat(ROAD_AMT_T).toFixed(2);
            var BRIDGE_AMT_T = $(this).jqGrid('getCol', 'BRIDGE_AMT', false, 'sum');
            BRIDGE_AMT_T = parseFloat(BRIDGE_AMT_T).toFixed(2);
            var IMS_SANCTIONED_MAN_AMT1_T = $(this).jqGrid('getCol', 'IMS_SANCTIONED_MAN_AMT1', false, 'sum');
            IMS_SANCTIONED_MAN_AMT1_T = parseFloat(IMS_SANCTIONED_MAN_AMT1_T).toFixed(2);
            var IMS_SANCTIONED_MAN_AMT2_T = $(this).jqGrid('getCol', 'IMS_SANCTIONED_MAN_AMT2', false, 'sum');
            IMS_SANCTIONED_MAN_AMT2_T = parseFloat(IMS_SANCTIONED_MAN_AMT2_T).toFixed(2);
            var IMS_SANCTIONED_MAN_AMT3_T = $(this).jqGrid('getCol', 'IMS_SANCTIONED_MAN_AMT3', false, 'sum');
            IMS_SANCTIONED_MAN_AMT3_T = parseFloat(IMS_SANCTIONED_MAN_AMT3_T).toFixed(2);
            var IMS_SANCTIONED_MAN_AMT4_T = $(this).jqGrid('getCol', 'IMS_SANCTIONED_MAN_AMT4', false, 'sum');
            IMS_SANCTIONED_MAN_AMT4_T = parseFloat(IMS_SANCTIONED_MAN_AMT4_T).toFixed(2);
            var IMS_SANCTIONED_MAN_AMT5_T = $(this).jqGrid('getCol', 'IMS_SANCTIONED_MAN_AMT5', false, 'sum');
            IMS_SANCTIONED_MAN_AMT5_T = parseFloat(IMS_SANCTIONED_MAN_AMT5_T).toFixed(2);
            var MAINT_AMT_T = $(this).jqGrid('getCol', 'MAINT_AMT', false, 'sum');
            MAINT_AMT_T = parseFloat(MAINT_AMT_T).toFixed(2);
            var TOTAL_AMT_T = $(this).jqGrid('getCol', 'TOTAL_AMT', false, 'sum');
            TOTAL_AMT_T = parseFloat(TOTAL_AMT_T).toFixed(2);
            ////

            $(this).jqGrid('footerData', 'set', { MAST_BLOCK_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { ROAD_AMT: ROAD_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { BRIDGE_AMT: BRIDGE_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { IMS_SANCTIONED_MAN_AMT1: IMS_SANCTIONED_MAN_AMT1_T }, true);
            $(this).jqGrid('footerData', 'set', { IMS_SANCTIONED_MAN_AMT2: IMS_SANCTIONED_MAN_AMT2_T }, true);
            $(this).jqGrid('footerData', 'set', { IMS_SANCTIONED_MAN_AMT3: IMS_SANCTIONED_MAN_AMT3_T }, true);
            $(this).jqGrid('footerData', 'set', { IMS_SANCTIONED_MAN_AMT4: IMS_SANCTIONED_MAN_AMT4_T }, true);
            $(this).jqGrid('footerData', 'set', { IMS_SANCTIONED_MAN_AMT5: IMS_SANCTIONED_MAN_AMT5_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT_AMT: MAINT_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT: TOTAL_AMT_T }, true);
            $('#PropEMCFinalReportTable_rn').html('Sr.<br/>No.');
            $("#PropEMCFinalReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Cost Rs in Lacs</font>");

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
    });/* End of Grid*/
    $("#PropEMCFinalReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'IMS_SANCTIONED_MAN_AMT1', numberOfColumns: 5, titleText: '<em> Estimated Maintenance Cost</em>' },

        ]
    });
}
/**/