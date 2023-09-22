$(function () {

    //$('#tblRptContents').bind('resize', function () {
    //    resizeJqGrid();
    //}).trigger('resize');
       var roadCategory = 0;
        var route ="%";
        if ($("#hdnLevelId").val() == 6) //mord
        {

            CN5StateReportListing(route, roadCategory);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {

            CN5DistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), route, roadCategory);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {

            CN5BlockReportListing($("#MAST_DISTRICT_CODE").val(), $("#MAST_STATE_CODE").val(), $("#DISTRICT_NAME").val(), route, roadCategory);
        }
   
});

/*       STATE REPORT LISTING       */
function CN5StateReportListing(route, roadCategory) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CN5StateReportTable").jqGrid('GridUnload');
    $("#CN5DistrictReportTable").jqGrid('GridUnload');
    $("#CN5BlockReportTable").jqGrid('GridUnload');
    $("#CN5FinalReportTable").jqGrid('GridUnload');

    $("#CN5StateReportTable").jqGrid({
        url: '/CoreNetworkReports/CN5StateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Total Number', 'Total Length', 'BT Length', 'WBM Length', 'Gravel Length', 'Track Length', 'Total', 'BT Length', 'WBM Length',
                 'Total', 'BT Length', 'WBM Length', 'Total', 'BT Length', 'WBM Length', 'Gravel Length', 'Track Length', 'Total', 'BT Length',
                 'WBM Length', 'Gravel Length', 'Track Length', 'Total', 'BT Length', 'WBM Length', 'Gravel Length', 'Track Length', 'Total'],
        colModel: [
            { name: 'MAST_STATE_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'BT_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBM_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GRAVEL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TRACK_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TotalRoadLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'NH_BT_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'NH_WBM_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'NH_TotalRoadLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'SH_BT_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'SH_WBM_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'SH_TotalRoadLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'MDR_BT_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'MDR_WBM_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'MDR_GRAVEL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'MDR_TRACK_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'MDR_TotalRoadLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'LR_BT_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'LR_WBM_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'LR_GRAVEL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'LR_TRACK_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'LR_TotalRoadLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TR_BT_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TR_WBM_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TR_GRAVEL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TR_TRACK_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TR_TotalRoadLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } }

        ],
        postData: { "Route": route, "RoadCategory": roadCategory },

        pager: $("#CN5StateReportPager"),
        footerrow: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: false,
        height: '550',
        width: '1100',
        shrinkToFit: false,
        viewrecords: true,
        caption: 'State Core Network Details',
        loadComplete: function () {
            //Total of Columns
            var TOTAL_CN_T = $(this).jqGrid('getCol', 'TOTAL_CN', false, 'sum');
            var TOTAL_LEN_T = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            TOTAL_LEN_T = parseFloat(TOTAL_LEN_T).toFixed(3);
            var BT_LEN_T = $(this).jqGrid('getCol', 'BT_LEN', false, 'sum');
            BT_LEN_T = parseFloat(BT_LEN_T).toFixed(3);
            var WBM_LEN_T = $(this).jqGrid('getCol', 'WBM_LEN', false, 'sum');
            WBM_LEN_T = parseFloat(WBM_LEN_T).toFixed(3);
            var GRAVEL_LEN_T = $(this).jqGrid('getCol', 'GRAVEL_LEN', false, 'sum');
            GRAVEL_LEN_T = parseFloat(GRAVEL_LEN_T).toFixed(3);
            var TRACK_LEN_T = $(this).jqGrid('getCol', 'TRACK_LEN', false, 'sum');
            TRACK_LEN_T = parseFloat(TRACK_LEN_T).toFixed(3);
            var TotalRoadLength_T = $(this).jqGrid('getCol', 'TotalRoadLength', false, 'sum');
            TotalRoadLength_T = parseFloat(TotalRoadLength_T).toFixed(3);

            var NH_BT_LEN_T = $(this).jqGrid('getCol', 'NH_BT_LEN', false, 'sum');
            NH_BT_LEN_T = parseFloat(NH_BT_LEN_T).toFixed(3);
            var NH_WBM_LEN_T = $(this).jqGrid('getCol', 'NH_WBM_LEN', false, 'sum');
            NH_WBM_LEN_T = parseFloat(NH_WBM_LEN_T).toFixed(3);
            var NH_TotalRoadLength_T = $(this).jqGrid('getCol', 'NH_TotalRoadLength', false, 'sum');
            NH_TotalRoadLength_T = parseFloat(NH_TotalRoadLength_T).toFixed(3);

            var SH_BT_LEN_T = $(this).jqGrid('getCol', 'SH_BT_LEN', false, 'sum');
            SH_BT_LEN_T = parseFloat(SH_BT_LEN_T).toFixed(3);
            var SH_WBM_LEN_T = $(this).jqGrid('getCol', 'SH_WBM_LEN', false, 'sum');
            SH_WBM_LEN_T = parseFloat(SH_WBM_LEN_T).toFixed(3);
            var SH_TotalRoadLength_T = $(this).jqGrid('getCol', 'SH_TotalRoadLength', false, 'sum');
            SH_TotalRoadLength_T = parseFloat(SH_TotalRoadLength_T).toFixed(3);

            var MDR_BT_LEN_T = $(this).jqGrid('getCol', 'MDR_BT_LEN', false, 'sum');
            MDR_BT_LEN_T = parseFloat(MDR_BT_LEN_T).toFixed(3);
            var MDR_WBM_LEN_T = $(this).jqGrid('getCol', 'MDR_WBM_LEN', false, 'sum');
            MDR_WBM_LEN_T = parseFloat(MDR_WBM_LEN_T).toFixed(3);
            var MDR_GRAVEL_LEN_T = $(this).jqGrid('getCol', 'MDR_GRAVEL_LEN', false, 'sum');
            MDR_GRAVEL_LEN_T = parseFloat(MDR_GRAVEL_LEN_T).toFixed(3);
            var MDR_TRACK_LEN_T = $(this).jqGrid('getCol', 'MDR_TRACK_LEN', false, 'sum');
            MDR_TRACK_LEN_T = parseFloat(MDR_TRACK_LEN_T).toFixed(3);
            var MDR_TotalRoadLength_T = $(this).jqGrid('getCol', 'MDR_TotalRoadLength', false, 'sum');
            MDR_TotalRoadLength_T = parseFloat(MDR_TotalRoadLength_T).toFixed(3);

            var LR_BT_LEN_T = $(this).jqGrid('getCol', 'LR_BT_LEN', false, 'sum');
            LR_BT_LEN_T = parseFloat(LR_BT_LEN_T).toFixed(3);
            var LR_WBM_LEN_T = $(this).jqGrid('getCol', 'LR_WBM_LEN', false, 'sum');
            LR_WBM_LEN_T = parseFloat(LR_WBM_LEN_T).toFixed(3);
            var LR_GRAVEL_LEN_T = $(this).jqGrid('getCol', 'LR_GRAVEL_LEN', false, 'sum');
            LR_GRAVEL_LEN_T = parseFloat(LR_GRAVEL_LEN_T).toFixed(3);
            var LR_TRACK_LEN_T = $(this).jqGrid('getCol', 'LR_TRACK_LEN', false, 'sum');
            LR_TRACK_LEN_T = parseFloat(LR_TRACK_LEN_T).toFixed(3);
            var LR_TotalRoadLength_T = $(this).jqGrid('getCol', 'LR_TotalRoadLength', false, 'sum');
            LR_TotalRoadLength_T = parseFloat(LR_TotalRoadLength_T).toFixed(3);

            var TR_BT_LEN_T = $(this).jqGrid('getCol', 'TR_BT_LEN', false, 'sum');
            TR_BT_LEN_T = parseFloat(TR_BT_LEN_T).toFixed(3);
            var TR_WBM_LEN_T = $(this).jqGrid('getCol', 'TR_WBM_LEN', false, 'sum');
            TR_WBM_LEN_T = parseFloat(TR_WBM_LEN_T).toFixed(3);
            var TR_GRAVEL_LEN_T = $(this).jqGrid('getCol', 'TR_GRAVEL_LEN', false, 'sum');
            TR_GRAVEL_LEN_T = parseFloat(TR_GRAVEL_LEN_T).toFixed(3);
            var TR_TRACK_LEN_T = $(this).jqGrid('getCol', 'TR_TRACK_LEN', false, 'sum');
            TR_TRACK_LEN_T = parseFloat(TR_TRACK_LEN_T).toFixed(3);
            var TR_TotalRoadLength_T = $(this).jqGrid('getCol', 'TR_TotalRoadLength', false, 'sum');
            TR_TotalRoadLength_T = parseFloat(TR_TotalRoadLength_T).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_CN: TOTAL_CN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { BT_LEN: BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { WBM_LEN: WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { GRAVEL_LEN: GRAVEL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TRACK_LEN: TRACK_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TotalRoadLength: TotalRoadLength_T }, true);

            $(this).jqGrid('footerData', 'set', { NH_BT_LEN: NH_BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { NH_WBM_LEN: NH_WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { NH_TotalRoadLength: NH_TotalRoadLength_T }, true);

            $(this).jqGrid('footerData', 'set', { SH_BT_LEN: SH_BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { SH_WBM_LEN: SH_WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { SH_TotalRoadLength: SH_TotalRoadLength_T }, true);

            $(this).jqGrid('footerData', 'set', { MDR_BT_LEN: MDR_BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { MDR_WBM_LEN: MDR_WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { MDR_GRAVEL_LEN: MDR_GRAVEL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { MDR_TRACK_LEN: MDR_TRACK_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { MDR_TotalRoadLength: MDR_TotalRoadLength_T }, true);

            $(this).jqGrid('footerData', 'set', { LR_BT_LEN: LR_BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { LR_WBM_LEN: LR_WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { LR_GRAVEL_LEN: LR_GRAVEL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { LR_TRACK_LEN: LR_TRACK_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { LR_TotalRoadLength: LR_TotalRoadLength_T }, true);

            $(this).jqGrid('footerData', 'set', { TR_BT_LEN: TR_BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TR_WBM_LEN: TR_WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TR_GRAVEL_LEN: TR_GRAVEL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TR_TRACK_LEN: TR_TRACK_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TR_TotalRoadLength: TR_TotalRoadLength_T }, true);
            $('#CN5StateReportTable_rn').html('Sr.<br/>No.');
            $.unblockUI();
        },
        gridComplete: function () {
            var ids = jQuery("#CN5StateReportTable").jqGrid('getDataIDs');
            var previousId = 0;
            for (var i = 0; i < ids.length; i++) {
                var rowId = ids[i];
                var rowData = jQuery('#CN5StateReportTable').jqGrid('getRowData', rowId);
                var newTotalRoadLength = parseFloat(rowData.BT_LEN) + parseFloat(rowData.WBM_LEN) + parseFloat(rowData.GRAVEL_LEN) + parseFloat(rowData.TRACK_LEN);
                var newNH_TotalRoadLength = parseFloat(rowData.NH_BT_LEN) + parseFloat(rowData.NH_WBM_LEN);
                var newSH_TotalRoadLength = parseFloat(rowData.SH_BT_LEN) + parseFloat(rowData.SH_WBM_LEN);
                var newMDR_TotalRoadLength = parseFloat(rowData.MDR_BT_LEN) + parseFloat(rowData.MDR_WBM_LEN) + parseFloat(rowData.MDR_GRAVEL_LEN) + parseFloat(rowData.MDR_TRACK_LEN);
                var newLR_TotalRoadLength = parseFloat(rowData.LR_BT_LEN) + parseFloat(rowData.LR_WBM_LEN) + parseFloat(rowData.LR_GRAVEL_LEN) + parseFloat(rowData.LR_TRACK_LEN);
                var newTR_TotalRoadLength = parseFloat(rowData.TR_BT_LEN) + parseFloat(rowData.TR_WBM_LEN) + parseFloat(rowData.TR_GRAVEL_LEN) + parseFloat(rowData.TR_TRACK_LEN);

                $("#CN5StateReportTable").jqGrid('setCell', rowId, 'TotalRoadLength', parseFloat(newTotalRoadLength).toFixed(3).toString().toLocaleString("en-IN"));
                $("#CN5StateReportTable").jqGrid('setCell', rowId, 'NH_TotalRoadLength', parseFloat(newNH_TotalRoadLength).toFixed(3).toString().toLocaleString("en-IN"));
                $("#CN5StateReportTable").jqGrid('setCell', rowId, 'SH_TotalRoadLength', parseFloat(newSH_TotalRoadLength).toFixed(3).toString().toLocaleString("en-IN"));
                $("#CN5StateReportTable").jqGrid('setCell', rowId, 'MDR_TotalRoadLength', parseFloat(newMDR_TotalRoadLength).toFixed(3).toString().toLocaleString("en-IN"));
                $("#CN5StateReportTable").jqGrid('setCell', rowId, 'LR_TotalRoadLength', parseFloat(newLR_TotalRoadLength).toFixed(3).toString().toLocaleString("en-IN"));
                $("#CN5StateReportTable").jqGrid('setCell', rowId, 'TR_TotalRoadLength', parseFloat(newTR_TotalRoadLength).toFixed(3).toString().toLocaleString("en-IN"));

            }
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

    $("#CN5StateReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'BT_LEN', numberOfColumns: 5, titleText: '<em>Total Road Length(Kms)</em>' },
          { startColumnName: 'NH_BT_LEN', numberOfColumns: 3, titleText: '<em>National Highway(Kms)</em>' },
          { startColumnName: 'SH_BT_LEN', numberOfColumns: 3, titleText: '<em>State Highway(Kms)</em>' },
          { startColumnName: 'MDR_BT_LEN', numberOfColumns: 5, titleText: '<em>MDR(Kms)</em>' },
          { startColumnName: 'LR_BT_LEN', numberOfColumns: 5, titleText: '<em>Link Routes Road Length(Kms)</em>' },
          { startColumnName: 'TR_BT_LEN', numberOfColumns: 5, titleText: '<em>Through Routes Road Length(Kms)</em>' },
        ]
    });

}
/**/

/*       DISTRICT REPORT LISTING       */
function CN5DistrictReportListing(stateCode, stateName, route, roadCategory) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#CN5StateReportTable").jqGrid('setSelection', stateCode);
    $("#CN5StateReportTable").jqGrid('setGridState', 'hidden');
    $("#CN5DistrictReportTable").jqGrid('GridUnload');
    $("#CN5BlockReportTable").jqGrid('GridUnload');
    $("#CN5FinalReportTable").jqGrid('GridUnload');

    $("#CN5DistrictReportTable").jqGrid({
        url: '/CoreNetworkReports/CN5DistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Total Number', 'Total Length', 'BT Length', 'WBM Length', 'Gravel Length', 'Track Length', 'Total', 'BT Length', 'WBM Length',
                 'Total', 'BT Length', 'WBM Length', 'Total', 'BT Length', 'WBM Length', 'Gravel Length', 'Track Length', 'Total', 'BT Length',
                 'WBM Length', 'Gravel Length', 'Track Length', 'Total', 'BT Length', 'WBM Length', 'Gravel Length', 'Track Length', 'Total'],
        colModel: [
            { name: 'MAST_DISTRICT_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'BT_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBM_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GRAVEL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TRACK_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TotalRoadLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'NH_BT_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'NH_WBM_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'NH_TotalRoadLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'SH_BT_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'SH_WBM_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'SH_TotalRoadLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'MDR_BT_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'MDR_WBM_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'MDR_GRAVEL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'MDR_TRACK_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'MDR_TotalRoadLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'LR_BT_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'LR_WBM_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'LR_GRAVEL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'LR_TRACK_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'LR_TotalRoadLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TR_BT_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TR_WBM_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TR_GRAVEL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TR_TRACK_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TR_TotalRoadLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } }

        ],
        postData: { "StateCode": stateCode, "Route": route, "RoadCategory": roadCategory },
        pager: $("#CN5DistrictReportPager"),
        footerrow: true,
        sortname: 'MAST_DISTRICT_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: false,
        height: '520',
        width: '1100',
        shrinkToFit: false,
        viewrecords: true,
        caption: 'District Core Network Details for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_CN_T = $(this).jqGrid('getCol', 'TOTAL_CN', false, 'sum');
            var TOTAL_LEN_T = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            TOTAL_LEN_T = parseFloat(TOTAL_LEN_T).toFixed(3);
            var BT_LEN_T = $(this).jqGrid('getCol', 'BT_LEN', false, 'sum');
            BT_LEN_T = parseFloat(BT_LEN_T).toFixed(3);
            var WBM_LEN_T = $(this).jqGrid('getCol', 'WBM_LEN', false, 'sum');
            WBM_LEN_T = parseFloat(WBM_LEN_T).toFixed(3);
            var GRAVEL_LEN_T = $(this).jqGrid('getCol', 'GRAVEL_LEN', false, 'sum');
            GRAVEL_LEN_T = parseFloat(GRAVEL_LEN_T).toFixed(3);
            var TRACK_LEN_T = $(this).jqGrid('getCol', 'TRACK_LEN', false, 'sum');
            TRACK_LEN_T = parseFloat(TRACK_LEN_T).toFixed(3);
            var TotalRoadLength_T = $(this).jqGrid('getCol', 'TotalRoadLength', false, 'sum');
            TotalRoadLength_T = parseFloat(TotalRoadLength_T).toFixed(3);

            var NH_BT_LEN_T = $(this).jqGrid('getCol', 'NH_BT_LEN', false, 'sum');
            NH_BT_LEN_T = parseFloat(NH_BT_LEN_T).toFixed(3);
            var NH_WBM_LEN_T = $(this).jqGrid('getCol', 'NH_WBM_LEN', false, 'sum');
            NH_WBM_LEN_T = parseFloat(NH_WBM_LEN_T).toFixed(3);
            var NH_TotalRoadLength_T = $(this).jqGrid('getCol', 'NH_TotalRoadLength', false, 'sum');
            NH_TotalRoadLength_T = parseFloat(NH_TotalRoadLength_T).toFixed(3);

            var SH_BT_LEN_T = $(this).jqGrid('getCol', 'SH_BT_LEN', false, 'sum');
            SH_BT_LEN_T = parseFloat(SH_BT_LEN_T).toFixed(3);
            var SH_WBM_LEN_T = $(this).jqGrid('getCol', 'SH_WBM_LEN', false, 'sum');
            SH_WBM_LEN_T = parseFloat(SH_WBM_LEN_T).toFixed(3);
            var SH_TotalRoadLength_T = $(this).jqGrid('getCol', 'SH_TotalRoadLength', false, 'sum');
            SH_TotalRoadLength_T = parseFloat(SH_TotalRoadLength_T).toFixed(3);

            var MDR_BT_LEN_T = $(this).jqGrid('getCol', 'MDR_BT_LEN', false, 'sum');
            MDR_BT_LEN_T = parseFloat(MDR_BT_LEN_T).toFixed(3);
            var MDR_WBM_LEN_T = $(this).jqGrid('getCol', 'MDR_WBM_LEN', false, 'sum');
            MDR_WBM_LEN_T = parseFloat(MDR_WBM_LEN_T).toFixed(3);
            var MDR_GRAVEL_LEN_T = $(this).jqGrid('getCol', 'MDR_GRAVEL_LEN', false, 'sum');
            MDR_GRAVEL_LEN_T = parseFloat(MDR_GRAVEL_LEN_T).toFixed(3);
            var MDR_TRACK_LEN_T = $(this).jqGrid('getCol', 'MDR_TRACK_LEN', false, 'sum');
            MDR_TRACK_LEN_T = parseFloat(MDR_TRACK_LEN_T).toFixed(3);
            var MDR_TotalRoadLength_T = $(this).jqGrid('getCol', 'MDR_TotalRoadLength', false, 'sum');
            MDR_TotalRoadLength_T = parseFloat(MDR_TotalRoadLength_T).toFixed(3);

            var LR_BT_LEN_T = $(this).jqGrid('getCol', 'LR_BT_LEN', false, 'sum');
            LR_BT_LEN_T = parseFloat(LR_BT_LEN_T).toFixed(3);
            var LR_WBM_LEN_T = $(this).jqGrid('getCol', 'LR_WBM_LEN', false, 'sum');
            LR_WBM_LEN_T = parseFloat(LR_WBM_LEN_T).toFixed(3);
            var LR_GRAVEL_LEN_T = $(this).jqGrid('getCol', 'LR_GRAVEL_LEN', false, 'sum');
            LR_GRAVEL_LEN_T = parseFloat(LR_GRAVEL_LEN_T).toFixed(3);
            var LR_TRACK_LEN_T = $(this).jqGrid('getCol', 'LR_TRACK_LEN', false, 'sum');
            LR_TRACK_LEN_T = parseFloat(LR_TRACK_LEN_T).toFixed(3);
            var LR_TotalRoadLength_T = $(this).jqGrid('getCol', 'LR_TotalRoadLength', false, 'sum');
            LR_TotalRoadLength_T = parseFloat(LR_TotalRoadLength_T).toFixed(3);

            var TR_BT_LEN_T = $(this).jqGrid('getCol', 'TR_BT_LEN', false, 'sum');
            TR_BT_LEN_T = parseFloat(TR_BT_LEN_T).toFixed(3);
            var TR_WBM_LEN_T = $(this).jqGrid('getCol', 'TR_WBM_LEN', false, 'sum');
            TR_WBM_LEN_T = parseFloat(TR_WBM_LEN_T).toFixed(3);
            var TR_GRAVEL_LEN_T = $(this).jqGrid('getCol', 'TR_GRAVEL_LEN', false, 'sum');
            TR_GRAVEL_LEN_T = parseFloat(TR_GRAVEL_LEN_T).toFixed(3);
            var TR_TRACK_LEN_T = $(this).jqGrid('getCol', 'TR_TRACK_LEN', false, 'sum');
            TR_TRACK_LEN_T = parseFloat(TR_TRACK_LEN_T).toFixed(3);
            var TR_TotalRoadLength_T = $(this).jqGrid('getCol', 'TR_TotalRoadLength', false, 'sum');
            TR_TotalRoadLength_T = parseFloat(TR_TotalRoadLength_T).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { MAST_DISTRICT_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_CN: TOTAL_CN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { BT_LEN: BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { WBM_LEN: WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { GRAVEL_LEN: GRAVEL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TRACK_LEN: TRACK_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TotalRoadLength: TotalRoadLength_T }, true);

            $(this).jqGrid('footerData', 'set', { NH_BT_LEN: NH_BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { NH_WBM_LEN: NH_WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { NH_TotalRoadLength: NH_TotalRoadLength_T }, true);

            $(this).jqGrid('footerData', 'set', { SH_BT_LEN: SH_BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { SH_WBM_LEN: SH_WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { SH_TotalRoadLength: SH_TotalRoadLength_T }, true);

            $(this).jqGrid('footerData', 'set', { MDR_BT_LEN: MDR_BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { MDR_WBM_LEN: MDR_WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { MDR_GRAVEL_LEN: MDR_GRAVEL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { MDR_TRACK_LEN: MDR_TRACK_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { MDR_TotalRoadLength: MDR_TotalRoadLength_T }, true);

            $(this).jqGrid('footerData', 'set', { LR_BT_LEN: LR_BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { LR_WBM_LEN: LR_WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { LR_GRAVEL_LEN: LR_GRAVEL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { LR_TRACK_LEN: LR_TRACK_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { LR_TotalRoadLength: LR_TotalRoadLength_T }, true);

            $(this).jqGrid('footerData', 'set', { TR_BT_LEN: TR_BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TR_WBM_LEN: TR_WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TR_GRAVEL_LEN: TR_GRAVEL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TR_TRACK_LEN: TR_TRACK_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TR_TotalRoadLength: TR_TotalRoadLength_T }, true);
            $('#CN5DistrictReportTable_rn').html('Sr.<br/>No.');
            $.unblockUI();
        },
        gridComplete: function () {
            var ids = jQuery("#CN5DistrictReportTable").jqGrid('getDataIDs');
            var previousId = 0;
            for (var i = 0; i < ids.length; i++) {
                var rowId = ids[i];
                var rowData = jQuery('#CN5DistrictReportTable').jqGrid('getRowData', rowId);
                var newTotalRoadLength = parseFloat(rowData.BT_LEN) + parseFloat(rowData.WBM_LEN) + parseFloat(rowData.GRAVEL_LEN) + parseFloat(rowData.TRACK_LEN);
                var newNH_TotalRoadLength = parseFloat(rowData.NH_BT_LEN) + parseFloat(rowData.NH_WBM_LEN);
                var newSH_TotalRoadLength = parseFloat(rowData.SH_BT_LEN) + parseFloat(rowData.SH_WBM_LEN);
                var newMDR_TotalRoadLength = parseFloat(rowData.MDR_BT_LEN) + parseFloat(rowData.MDR_WBM_LEN) + parseFloat(rowData.MDR_GRAVEL_LEN) + parseFloat(rowData.MDR_TRACK_LEN);
                var newLR_TotalRoadLength = parseFloat(rowData.LR_BT_LEN) + parseFloat(rowData.LR_WBM_LEN) + parseFloat(rowData.LR_GRAVEL_LEN) + parseFloat(rowData.LR_TRACK_LEN);
                var newTR_TotalRoadLength = parseFloat(rowData.TR_BT_LEN) + parseFloat(rowData.TR_WBM_LEN) + parseFloat(rowData.TR_GRAVEL_LEN) + parseFloat(rowData.TR_TRACK_LEN);

                $("#CN5DistrictReportTable").jqGrid('setCell', rowId, 'TotalRoadLength', parseFloat(newTotalRoadLength).toFixed(3).toString().toLocaleString("en-IN"));
                $("#CN5DistrictReportTable").jqGrid('setCell', rowId, 'NH_TotalRoadLength', parseFloat(newNH_TotalRoadLength).toFixed(3).toString().toLocaleString("en-IN"));
                $("#CN5DistrictReportTable").jqGrid('setCell', rowId, 'SH_TotalRoadLength', parseFloat(newSH_TotalRoadLength).toFixed(3).toString().toLocaleString("en-IN"));
                $("#CN5DistrictReportTable").jqGrid('setCell', rowId, 'MDR_TotalRoadLength', parseFloat(newMDR_TotalRoadLength).toFixed(3).toString().toLocaleString("en-IN"));
                $("#CN5DistrictReportTable").jqGrid('setCell', rowId, 'LR_TotalRoadLength', parseFloat(newLR_TotalRoadLength).toFixed(3).toString().toLocaleString("en-IN"));
                $("#CN5DistrictReportTable").jqGrid('setCell', rowId, 'TR_TotalRoadLength', parseFloat(newTR_TotalRoadLength).toFixed(3).toString().toLocaleString("en-IN"));

            }
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

    $("#CN5DistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
           { startColumnName: 'BT_LEN', numberOfColumns: 5, titleText: '<em>Total Road Length(Kms)</em>' },
           { startColumnName: 'NH_BT_LEN', numberOfColumns: 3, titleText: '<em>National Highway(Kms)</em>' },
           { startColumnName: 'SH_BT_LEN', numberOfColumns: 3, titleText: '<em>State Highway(Kms)</em>' },
           { startColumnName: 'MDR_BT_LEN', numberOfColumns: 5, titleText: '<em>MDR(Kms)</em>' },
           { startColumnName: 'LR_BT_LEN', numberOfColumns: 5, titleText: '<em>Link Routes Road Length(Kms)</em>' },
           { startColumnName: 'TR_BT_LEN', numberOfColumns: 5, titleText: '<em>Through Routes Road Length(Kms)</em>' },
        ]
    });

}
/**/

/*       BLOCK REPORT LISTING       */
function CN5BlockReportListing(districtCode, stateCode, districtName, route, roadCategory) {

    var distname;
    if (districtName == '') {

        distname = $("#DISTRICT_NAME").val();
    }
    else {
        distname = districtName;
    }

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#CN5DistrictReportTable").jqGrid('setSelection', districtCode);
    $("#CN5DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#CN5StateReportTable").jqGrid('setGridState', 'hidden');

    $("#CN5BlockReportTable").jqGrid('GridUnload');
    $("#CN5FinalReportTable").jqGrid('GridUnload');

    $("#CN5BlockReportTable").jqGrid({
        url: '/CoreNetworkReports/CN5BlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Total Number', 'Total Length', 'BT Length', 'WBM Length', 'Gravel Length', 'Track Length', 'Total', 'BT Length', 'WBM Length',
                   'Total', 'BT Length', 'WBM Length', 'Total', 'BT Length', 'WBM Length', 'Gravel Length', 'Track Length', 'Total', 'BT Length',
                   'WBM Length', 'Gravel Length', 'Track Length', 'Total', 'BT Length', 'WBM Length', 'Gravel Length', 'Track Length', 'Total'],
        colModel: [
            { name: 'MAST_BLOCK_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'BT_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBM_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GRAVEL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TRACK_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TotalRoadLength', width: 120, align: 'right', sortable: false, height: 'auto', formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'NH_BT_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'NH_WBM_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'NH_TotalRoadLength', width: 120, align: 'right', sortable: false, height: 'auto', formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'SH_BT_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'SH_WBM_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'SH_TotalRoadLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'MDR_BT_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'MDR_WBM_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'MDR_GRAVEL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'MDR_TRACK_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'MDR_TotalRoadLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'LR_BT_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'LR_WBM_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'LR_GRAVEL_LEN', width: 120, align: 'right', height: 'auto', sortable: false , formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'LR_TRACK_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'LR_TotalRoadLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TR_BT_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TR_WBM_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TR_GRAVEL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TR_TRACK_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TR_TotalRoadLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } }

        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "Route": route, "RoadCategory": roadCategory },
        pager: $("#CN5BlockReportPager"),
        footerrow: true,
        sortname: 'MAST_BLOCK_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: false,
        height: '470',
        width: '1100',
        shrinkToFit: false,
        viewrecords: true,
        caption: 'Block Core Network Details for ' + distname,

        loadComplete: function () {
            //Total of Columns
            var TOTAL_CN_T = $(this).jqGrid('getCol', 'TOTAL_CN', false, 'sum');
            var TOTAL_LEN_T = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            TOTAL_LEN_T = parseFloat(TOTAL_LEN_T).toFixed(3);
            var BT_LEN_T = $(this).jqGrid('getCol', 'BT_LEN', false, 'sum');
            BT_LEN_T = parseFloat(BT_LEN_T).toFixed(3);
            var WBM_LEN_T = $(this).jqGrid('getCol', 'WBM_LEN', false, 'sum');
            WBM_LEN_T = parseFloat(WBM_LEN_T).toFixed(3);
            var GRAVEL_LEN_T = $(this).jqGrid('getCol', 'GRAVEL_LEN', false, 'sum');
            GRAVEL_LEN_T = parseFloat(GRAVEL_LEN_T).toFixed(3);
            var TRACK_LEN_T = $(this).jqGrid('getCol', 'TRACK_LEN', false, 'sum');
            TRACK_LEN_T = parseFloat(TRACK_LEN_T).toFixed(3);
            var TotalRoadLength_T = $(this).jqGrid('getCol', 'TotalRoadLength', false, 'sum');
            TotalRoadLength_T = parseFloat(TotalRoadLength_T).toFixed(3);

            var NH_BT_LEN_T = $(this).jqGrid('getCol', 'NH_BT_LEN', false, 'sum');
            NH_BT_LEN_T = parseFloat(NH_BT_LEN_T).toFixed(3);
            var NH_WBM_LEN_T = $(this).jqGrid('getCol', 'NH_WBM_LEN', false, 'sum');
            NH_WBM_LEN_T = parseFloat(NH_WBM_LEN_T).toFixed(3);
            var NH_TotalRoadLength_T = $(this).jqGrid('getCol', 'NH_TotalRoadLength', false, 'sum');
            NH_TotalRoadLength_T = parseFloat(NH_TotalRoadLength_T).toFixed(3);

            var SH_BT_LEN_T = $(this).jqGrid('getCol', 'SH_BT_LEN', false, 'sum');
            SH_BT_LEN_T = parseFloat(SH_BT_LEN_T).toFixed(3);
            var SH_WBM_LEN_T = $(this).jqGrid('getCol', 'SH_WBM_LEN', false, 'sum');
            SH_WBM_LEN_T = parseFloat(SH_WBM_LEN_T).toFixed(3);
            var SH_TotalRoadLength_T = $(this).jqGrid('getCol', 'SH_TotalRoadLength', false, 'sum');
            SH_TotalRoadLength_T = parseFloat(SH_TotalRoadLength_T).toFixed(3);

            var MDR_BT_LEN_T = $(this).jqGrid('getCol', 'MDR_BT_LEN', false, 'sum');
            MDR_BT_LEN_T = parseFloat(MDR_BT_LEN_T).toFixed(3);
            var MDR_WBM_LEN_T = $(this).jqGrid('getCol', 'MDR_WBM_LEN', false, 'sum');
            MDR_WBM_LEN_T = parseFloat(MDR_WBM_LEN_T).toFixed(3);
            var MDR_GRAVEL_LEN_T = $(this).jqGrid('getCol', 'MDR_GRAVEL_LEN', false, 'sum');
            MDR_GRAVEL_LEN_T = parseFloat(MDR_GRAVEL_LEN_T).toFixed(3);
            var MDR_TRACK_LEN_T = $(this).jqGrid('getCol', 'MDR_TRACK_LEN', false, 'sum');
            MDR_TRACK_LEN_T = parseFloat(MDR_TRACK_LEN_T).toFixed(3);
            var MDR_TotalRoadLength_T = $(this).jqGrid('getCol', 'MDR_TotalRoadLength', false, 'sum');
            MDR_TotalRoadLength_T = parseFloat(MDR_TotalRoadLength_T).toFixed(3);

            var LR_BT_LEN_T = $(this).jqGrid('getCol', 'LR_BT_LEN', false, 'sum');
            LR_BT_LEN_T = parseFloat(LR_BT_LEN_T).toFixed(3);
            var LR_WBM_LEN_T = $(this).jqGrid('getCol', 'LR_WBM_LEN', false, 'sum');
            LR_WBM_LEN_T = parseFloat(LR_WBM_LEN_T).toFixed(3);
            var LR_GRAVEL_LEN_T = $(this).jqGrid('getCol', 'LR_GRAVEL_LEN', false, 'sum');
            LR_GRAVEL_LEN_T = parseFloat(LR_GRAVEL_LEN_T).toFixed(3);
            var LR_TRACK_LEN_T = $(this).jqGrid('getCol', 'LR_TRACK_LEN', false, 'sum');
            LR_TRACK_LEN_T = parseFloat(LR_TRACK_LEN_T).toFixed(3);
            var LR_TotalRoadLength_T = $(this).jqGrid('getCol', 'LR_TotalRoadLength', false, 'sum');
            LR_TotalRoadLength_T = parseFloat(LR_TotalRoadLength_T).toFixed(3);

            var TR_BT_LEN_T = $(this).jqGrid('getCol', 'TR_BT_LEN', false, 'sum');
            TR_BT_LEN_T = parseFloat(TR_BT_LEN_T).toFixed(3);
            var TR_WBM_LEN_T = $(this).jqGrid('getCol', 'TR_WBM_LEN', false, 'sum');
            TR_WBM_LEN_T = parseFloat(TR_WBM_LEN_T).toFixed(3);
            var TR_GRAVEL_LEN_T = $(this).jqGrid('getCol', 'TR_GRAVEL_LEN', false, 'sum');
            TR_GRAVEL_LEN_T = parseFloat(TR_GRAVEL_LEN_T).toFixed(3);
            var TR_TRACK_LEN_T = $(this).jqGrid('getCol', 'TR_TRACK_LEN', false, 'sum');
            TR_TRACK_LEN_T = parseFloat(TR_TRACK_LEN_T).toFixed(3);
            var TR_TotalRoadLength_T = $(this).jqGrid('getCol', 'TR_TotalRoadLength', false, 'sum');
            TR_TotalRoadLength_T = parseFloat(TR_TotalRoadLength_T).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { MAST_BLOCK_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_CN: TOTAL_CN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { BT_LEN: BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { WBM_LEN: WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { GRAVEL_LEN: GRAVEL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TRACK_LEN: TRACK_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TotalRoadLength: TotalRoadLength_T }, true);

            $(this).jqGrid('footerData', 'set', { NH_BT_LEN: NH_BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { NH_WBM_LEN: NH_WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { NH_TotalRoadLength: NH_TotalRoadLength_T }, true);

            $(this).jqGrid('footerData', 'set', { SH_BT_LEN: SH_BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { SH_WBM_LEN: SH_WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { SH_TotalRoadLength: SH_TotalRoadLength_T }, true);

            $(this).jqGrid('footerData', 'set', { MDR_BT_LEN: MDR_BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { MDR_WBM_LEN: MDR_WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { MDR_GRAVEL_LEN: MDR_GRAVEL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { MDR_TRACK_LEN: MDR_TRACK_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { MDR_TotalRoadLength: MDR_TotalRoadLength_T }, true);

            $(this).jqGrid('footerData', 'set', { LR_BT_LEN: LR_BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { LR_WBM_LEN: LR_WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { LR_GRAVEL_LEN: LR_GRAVEL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { LR_TRACK_LEN: LR_TRACK_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { LR_TotalRoadLength: LR_TotalRoadLength_T }, true);

            $(this).jqGrid('footerData', 'set', { TR_BT_LEN: TR_BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TR_WBM_LEN: TR_WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TR_GRAVEL_LEN: TR_GRAVEL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TR_TRACK_LEN: TR_TRACK_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TR_TotalRoadLength: TR_TotalRoadLength_T }, true);
            $('#CN5BlockReportTable_rn').html('Sr.<br/>No.');
            $.unblockUI();
        },
        gridComplete: function () {
            var ids = jQuery("#CN5BlockReportTable").jqGrid('getDataIDs');
            var previousId = 0;
            for (var i = 0; i < ids.length; i++) {
                var rowId = ids[i];
                var rowData = jQuery('#CN5BlockReportTable').jqGrid('getRowData', rowId);
                var newTotalRoadLength = parseFloat(rowData.BT_LEN) + parseFloat(rowData.WBM_LEN) + parseFloat(rowData.GRAVEL_LEN) + parseFloat(rowData.TRACK_LEN);
                var newNH_TotalRoadLength = parseFloat(rowData.NH_BT_LEN) + parseFloat(rowData.NH_WBM_LEN);
                var newSH_TotalRoadLength = parseFloat(rowData.SH_BT_LEN) + parseFloat(rowData.SH_WBM_LEN);
                var newMDR_TotalRoadLength = parseFloat(rowData.MDR_BT_LEN) + parseFloat(rowData.MDR_WBM_LEN) + parseFloat(rowData.MDR_GRAVEL_LEN) + parseFloat(rowData.MDR_TRACK_LEN);
                var newLR_TotalRoadLength = parseFloat(rowData.LR_BT_LEN) + parseFloat(rowData.LR_WBM_LEN) + parseFloat(rowData.LR_GRAVEL_LEN) + parseFloat(rowData.LR_TRACK_LEN);
                var newTR_TotalRoadLength = parseFloat(rowData.TR_BT_LEN) + parseFloat(rowData.TR_WBM_LEN) + parseFloat(rowData.TR_GRAVEL_LEN) + parseFloat(rowData.TR_TRACK_LEN);

                $("#CN5BlockReportTable").jqGrid('setCell', rowId, 'TotalRoadLength', parseFloat(newTotalRoadLength).toFixed(3).toString().toLocaleString("en-IN"));
                $("#CN5BlockReportTable").jqGrid('setCell', rowId, 'NH_TotalRoadLength', parseFloat(newNH_TotalRoadLength).toFixed(3).toString().toLocaleString("en-IN"));
                $("#CN5BlockReportTable").jqGrid('setCell', rowId, 'SH_TotalRoadLength', parseFloat(newSH_TotalRoadLength).toFixed(3).toString().toLocaleString("en-IN"));
                $("#CN5BlockReportTable").jqGrid('setCell', rowId, 'MDR_TotalRoadLength', parseFloat(newMDR_TotalRoadLength).toFixed(3).toString().toLocaleString("en-IN"));
                $("#CN5BlockReportTable").jqGrid('setCell', rowId, 'LR_TotalRoadLength', parseFloat(newLR_TotalRoadLength).toFixed(3).toString().toLocaleString("en-IN"));
                $("#CN5BlockReportTable").jqGrid('setCell', rowId, 'TR_TotalRoadLength', parseFloat(newTR_TotalRoadLength).toFixed(3).toString().toLocaleString("en-IN"));

            }
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

    $("#CN5BlockReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'BT_LEN', numberOfColumns: 5, titleText: '<em>Total Road Length(Kms)</em>' },
          { startColumnName: 'NH_BT_LEN', numberOfColumns: 3, titleText: '<em>National Highway(Kms)</em>' },
          { startColumnName: 'SH_BT_LEN', numberOfColumns: 3, titleText: '<em>State Highway(Kms)</em>' },
          { startColumnName: 'MDR_BT_LEN', numberOfColumns: 5, titleText: '<em>MDR(Kms)</em>' },
          { startColumnName: 'LR_BT_LEN', numberOfColumns: 5, titleText: '<em>Link Routes Road Length(Kms)</em>' },
          { startColumnName: 'TR_BT_LEN', numberOfColumns: 5, titleText: '<em>Through Routes Road Length(Kms)</em>' },
        ]
    });
}
/**/

/*       FINAL REPORT LISTING       */
function CN5FinalReportListing(blockCode, districtCode, stateCode, blockName, route, roadCategory) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#CN5BlockReportTable").jqGrid('setGridState', 'hidden');
    $("#CN5DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#CN5StateReportTable").jqGrid('setGridState', 'hidden');
    $("#CN5BlockReportTable").jqGrid('setSelection', stateCode);
    $("#CN5FinalReportTable").jqGrid('GridUnload');

    $("#CN5FinalReportTable").jqGrid({
        url: '/CoreNetworkReports/CN5FinalReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Road Code', 'Road Name', 'DRRP Road Code', 'DRRP Road Name', 'Road Length in KM', 'Road From', 'Road To', 'Route Type', 'Length Covered (P/F)', 'Habitation', 'Population'],
        //colNames: ['Road Number', 'Road Name', 'Road From', 'Road To', 'Road Length', 'Road to Habitation', 'Total Habitation Population'],
        colModel: [
            { name: 'PlannedRoadNumber', width: 80, align: 'left', height: 'auto', sortable: false },
            { name: 'PlannedRoadName', width: 150, align: 'left', height: 'auto', sortable: true },
            { name: 'ERRoadNumber', width: 80, align: 'left', height: 'auto', sortable: false },
            { name: 'ERRoadName', width: 150, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadLengthinKM', width: 80, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'RoadFrom', width: 100, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadTo', width: 100, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadRoute', width: 100, align: 'left', height: 'auto', sortable: false },
            { name: 'PlanRoadLength', width: 100, align: 'left', height: 'auto', sortable: false },
            { name: 'HabitationName', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'TotalPopulationofHabitation', width: 100, align: 'right', sortable: false, height: 'auto', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "Route": route, "RoadCategory": roadCategory },
        pager: $("#CN5FinalReportPager"),
        footerrow: true,
        sortname: 'PlannedRoadNumber',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: false,
        height: '420',
        width: 1100,
        shrinkToFit: false,
        viewrecords: true,
        caption: 'Core Network Details for ' + blockName,
        //grouping: true,
        //groupingView: {
        //    groupField: ['PlannedRoadNumber'],
        //    groupSummary: [true],
        //    groupColumnShow: [false],
        //    groupDataSorted: true
        //},
        loadComplete: function () {
            //Total of Columns
            var RoadLengthinKMT = $(this).jqGrid('getCol', 'RoadLengthinKM', false, 'sum');
            RoadLengthinKMT = parseFloat(RoadLengthinKMT).toFixed(3);
            var TotalPopulationofHabitationT = $(this).jqGrid('getCol', 'TotalPopulationofHabitation', false, 'sum');
            //

            $(this).jqGrid('footerData', 'set', { PlannedRoadNumber: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { RoadLengthinKM: RoadLengthinKMT });
            $(this).jqGrid('footerData', 'set', { TotalPopulationofHabitation: TotalPopulationofHabitationT });
            $('#CN5FinalReportTable_rn').html('Sr.<br/>No.');
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

}
/**/