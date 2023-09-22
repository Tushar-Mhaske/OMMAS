$(document).ready(function () {
    

    $("#btCN3Details").click(function () {

        var roadCategory = $("#RoadCategory_CN3Details").val();
        var route = $("#Route_CN3Details").val();

        if ($("#hdnLevelId").val() == 6) //mord
        {
            
            CN3StateReportListing(route,roadCategory);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
         
            CN3DistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(),route, roadCategory);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
         
            CN3BlockReportListing($("#MAST_DISTRICT_CODE").val(), $("#MAST_STATE_CODE").val(), $("#DISTRICT_NAME").val(), route, roadCategory);
        }
    });
    $("#btCN3Details").trigger("click");
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});


/*       STATE REPORT LISTING       */
function CN3StateReportListing(route, roadCategory) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CN3StateReportTable").jqGrid('GridUnload');
    $("#CN3DistrictReportTable").jqGrid('GridUnload');
    $("#CN3BlockReportTable").jqGrid('GridUnload');
    $("#CN3FinalReportTable").jqGrid('GridUnload');    
    $("#CN3StateReportTable").jqGrid({
        url: '/CoreNetworkReports/CN3StateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'BT', 'WBM', 'Gravel', 'Track','Other', 'Total', 'BT', 'WBM', 'Gravel', 'Track','Other', 'Total'],
        colModel: [
            { name: 'StateName', width: 150, align: 'left', height: 'auto', sortable: true },
            { name: 'BT_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBM_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GRAVEL_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TRACK_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'OTHER_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'CN_BT_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'CN_WBM_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'CN_GRAVEL_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'CN_TRACK_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'CN_OTHER_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TOTAL_CN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
  
        ],
        postData: {"Route": route, "RoadCategory": roadCategory },
        pager: $("#CN3StateReportPager"),
        footerrow: true,
        sortname: 'StateName',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: 520,
        viewrecords: true,
        caption: 'State Core Network Details',
        loadComplete: function () {
            //Total of Columns
            var BT_LEN_T = $(this).jqGrid('getCol', 'BT_LEN', false, 'sum');
            BT_LEN_T = parseFloat(BT_LEN_T).toFixed(3);
            var WBM_LEN_T = $(this).jqGrid('getCol', 'WBM_LEN', false, 'sum');
            WBM_LEN_T = parseFloat(WBM_LEN_T).toFixed(3);
            var GRAVEL_LEN_T = $(this).jqGrid('getCol', 'GRAVEL_LEN', false, 'sum');
            GRAVEL_LEN_T = parseFloat(GRAVEL_LEN_T).toFixed(3);
            var TRACK_LEN_T = $(this).jqGrid('getCol', 'TRACK_LEN', false, 'sum');
            TRACK_LEN_T = parseFloat(TRACK_LEN_T).toFixed(3);
            var OTHER_LEN_T = $(this).jqGrid('getCol', 'OTHER_LEN', false, 'sum');
            OTHER_LEN_T = parseFloat(OTHER_LEN_T).toFixed(3);
            var TOTAL_LEN_T = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            TOTAL_LEN_T = parseFloat(TOTAL_LEN_T).toFixed(3);
            var CN_BT_LEN_T = $(this).jqGrid('getCol', 'CN_BT_LEN', false, 'sum');
            CN_BT_LEN_T = parseFloat(CN_BT_LEN_T).toFixed(3);
            var CN_WBM_LEN_T = $(this).jqGrid('getCol', 'CN_WBM_LEN', false, 'sum');
            CN_WBM_LEN_T = parseFloat(CN_WBM_LEN_T).toFixed(3);
            var CN_GRAVEL_LEN_T = $(this).jqGrid('getCol', 'CN_GRAVEL_LEN', false, 'sum');
            CN_GRAVEL_LEN_T = parseFloat(CN_GRAVEL_LEN_T).toFixed(3);
            var CN_TRACK_LEN_T = $(this).jqGrid('getCol', 'CN_TRACK_LEN', false, 'sum');
            CN_TRACK_LEN_T = parseFloat(CN_TRACK_LEN_T).toFixed(3);
            var CN_OTHER_LEN_T = $(this).jqGrid('getCol', 'CN_OTHER_LEN', false, 'sum');
            CN_OTHER_LEN_T = parseFloat(CN_OTHER_LEN_T).toFixed(3);
            var TOTAL_CN_T = $(this).jqGrid('getCol', 'TOTAL_CN', false, 'sum');
            TOTAL_CN_T = parseFloat(TOTAL_CN_T).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { BT_LEN: BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { WBM_LEN: WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { GRAVEL_LEN: GRAVEL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TRACK_LEN: TRACK_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { OTHER_LEN: OTHER_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_BT_LEN: CN_BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_WBM_LEN: CN_WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_GRAVEL_LEN: CN_GRAVEL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_TRACK_LEN: CN_TRACK_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_OTHER_LEN: CN_OTHER_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CN: TOTAL_CN_T }, true);
            $('#CN3StateReportTable_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        //gridComplete: function () {

        //    var ids = jQuery("#CN3StateReportTable").jqGrid('getDataIDs');
        //    var previousId = 0;

        //    for (var i = 0; i < ids.length; i++) {
        //        var rowId = ids[i];
        //        var rowData = jQuery('#CN3StateReportTable').jqGrid('getRowData', rowId);
        //        var newTotalLenST = parseFloat(rowData.BT_LEN) + parseFloat(rowData.WBM_LEN) + parseFloat(rowData.GRAVEL_LEN) + parseFloat(rowData.TRACK_LEN)+parseFloat(rowData.OTHER_LEN);
        //        var newTotalLenCN = parseFloat(rowData.CN_BT_LEN) + parseFloat(rowData.CN_WBM_LEN) + parseFloat(rowData.CN_GRAVEL_LEN) + parseFloat(rowData.CN_TRACK_LEN)+parseFloat(rowData.CN_OTHER_LEN);

        //        $("#CN3StateReportTable").jqGrid('setCell', rowId, 'TOTAL_LEN', parseFloat(newTotalLenST).toFixed(3).toString().toLocaleString("en-IN"));
        //        $("#CN3StateReportTable").jqGrid('setCell', rowId, 'TOTAL_CN', parseFloat(newTotalLenCN).toFixed(3).toString().toLocaleString("en-IN"));

        //    }
        //},
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }
    }); /*End of grid*/
    $("#CN3StateReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'BT_LEN', numberOfColumns: 6, titleText: '<em> Total road length as per DRRP (Kms) </em>' },
          { startColumnName: 'CN_BT_LEN', numberOfColumns: 6, titleText: '<em> Total road length as per Identified Core Network (Kms) </em>' }

        ]
    });

}
/**/

/*       DISTRICT REPORT LISTING       */
function CN3DistrictReportListing(stateCode, stateName, route, roadCategory) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CN3StateReportTable").jqGrid('setSelection', stateCode);
    $("#CN3StateReportTable").jqGrid('setGridState', 'hidden');
    $("#CN3DistrictReportTable").jqGrid('GridUnload');
    $("#CN3BlockReportTable").jqGrid('GridUnload');
    $("#CN3FinalReportTable").jqGrid('GridUnload');

    $("#CN3DistrictReportTable").jqGrid({
        url: '/CoreNetworkReports/CN3DistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'BT', 'WBM', 'Gravel', 'Track', 'Other', 'Total', 'BT', 'WBM', 'Gravel', 'Track', 'Other', 'Total'],
        colModel: [
            { name: 'DistrictName', width: 150, align: 'left', height: 'auto', sortable: true },
            { name: 'BT_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBM_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GRAVEL_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TRACK_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'OTHER_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'CN_BT_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'CN_WBM_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'CN_GRAVEL_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'CN_TRACK_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'CN_OTHER_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TOTAL_CN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },

        ],
        postData: { "StateCode": stateCode, "Route": route, "RoadCategory": roadCategory },
        pager: $("#CN3DistrictReportPager"),
        footerrow: true,
        sortname: 'DistrictName',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: 460,
        viewrecords: true,
        caption: 'District Core Network Details for ' + stateName,       
        loadComplete: function () {
            //Total of Columns
            var BT_LEN_T = $(this).jqGrid('getCol', 'BT_LEN', false, 'sum');
            BT_LEN_T = parseFloat(BT_LEN_T).toFixed(3);
            var WBM_LEN_T = $(this).jqGrid('getCol', 'WBM_LEN', false, 'sum');
            WBM_LEN_T = parseFloat(WBM_LEN_T).toFixed(3);
            var GRAVEL_LEN_T = $(this).jqGrid('getCol', 'GRAVEL_LEN', false, 'sum');
            GRAVEL_LEN_T = parseFloat(GRAVEL_LEN_T).toFixed(3);
            var TRACK_LEN_T = $(this).jqGrid('getCol', 'TRACK_LEN', false, 'sum');
            TRACK_LEN_T = parseFloat(TRACK_LEN_T).toFixed(3);
            var OTHER_LEN_T = $(this).jqGrid('getCol', 'OTHER_LEN', false, 'sum');
            OTHER_LEN_T = parseFloat(OTHER_LEN_T).toFixed(3);
            var TOTAL_LEN_T = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            TOTAL_LEN_T = parseFloat(TOTAL_LEN_T).toFixed(3);
            var CN_BT_LEN_T = $(this).jqGrid('getCol', 'CN_BT_LEN', false, 'sum');
            CN_BT_LEN_T = parseFloat(CN_BT_LEN_T).toFixed(3);
            var CN_WBM_LEN_T = $(this).jqGrid('getCol', 'CN_WBM_LEN', false, 'sum');
            CN_WBM_LEN_T = parseFloat(CN_WBM_LEN_T).toFixed(3);
            var CN_GRAVEL_LEN_T = $(this).jqGrid('getCol', 'CN_GRAVEL_LEN', false, 'sum');
            CN_GRAVEL_LEN_T = parseFloat(CN_GRAVEL_LEN_T).toFixed(3);
            var CN_TRACK_LEN_T = $(this).jqGrid('getCol', 'CN_TRACK_LEN', false, 'sum');
            CN_TRACK_LEN_T = parseFloat(CN_TRACK_LEN_T).toFixed(3);
            var CN_OTHER_LEN_T = $(this).jqGrid('getCol', 'CN_OTHER_LEN', false, 'sum');
            CN_OTHER_LEN_T = parseFloat(CN_OTHER_LEN_T).toFixed(3);
            var TOTAL_CN_T = $(this).jqGrid('getCol', 'TOTAL_CN', false, 'sum');
            TOTAL_CN_T = parseFloat(TOTAL_CN_T).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { BT_LEN: BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { WBM_LEN: WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { GRAVEL_LEN: GRAVEL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TRACK_LEN: TRACK_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { OTHER_LEN: OTHER_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_BT_LEN: CN_BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_WBM_LEN: CN_WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_GRAVEL_LEN: CN_GRAVEL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_TRACK_LEN: CN_TRACK_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_OTHER_LEN: CN_OTHER_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CN: TOTAL_CN_T }, true);
            $('#CN3DistrictReportTable_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        //gridComplete: function () {

        //    var ids = jQuery("#CN3DistrictReportTable").jqGrid('getDataIDs');
        //    var previousId = 0;

        //    for (var i = 0; i < ids.length; i++) {
        //        var rowId = ids[i];
        //        var rowData = jQuery('#CN3DistrictReportTable').jqGrid('getRowData', rowId);
        //        var newTotalLenST = parseFloat(rowData.BT_LEN) + parseFloat(rowData.WBM_LEN) + parseFloat(rowData.GRAVEL_LEN) + parseFloat(rowData.TRACK_LEN) + parseFloat(rowData.OTHER_LEN);
        //        var newTotalLenCN = parseFloat(rowData.CN_BT_LEN) + parseFloat(rowData.CN_WBM_LEN) + parseFloat(rowData.CN_GRAVEL_LEN) + parseFloat(rowData.CN_TRACK_LEN)+parseFloat(rowData.CN_OTHER_LEN);

        //        $("#CN3DistrictReportTable").jqGrid('setCell', rowId, 'TOTAL_LEN', parseFloat(newTotalLenST).toFixed(3).toString().toLocaleString("en-IN"));
        //        $("#CN3DistrictReportTable").jqGrid('setCell', rowId, 'TOTAL_CN', parseFloat(newTotalLenCN).toFixed(3).toString().toLocaleString("en-IN"));

        //    }
        //},
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }
    }); /*End of grid*/
    $("#CN3DistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'BT_LEN', numberOfColumns: 6, titleText: '<em> Total road length as per DRRP (Kms) </em>' },
          { startColumnName: 'CN_BT_LEN', numberOfColumns: 6, titleText: '<em> Total road length as per Identified Core Network (Kms) </em>' }

        ]
    });

}
/**/

function CN3BlockReportListing(districtCode, stateCode, districtName, route, roadCategory) {
    var distname;
    if (districtName == '')
        distname = $("#DISTRICT_NAME").val();
    else
        distname = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CN3DistrictReportTable").jqGrid('setSelection', stateCode);
    $("#CN3DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#CN3StateReportTable").jqGrid('setGridState', 'hidden');
    $("#CN3BlockReportTable").jqGrid('GridUnload');
    $("#CN3FinalReportTable").jqGrid('GridUnload');

    $("#CN3BlockReportTable").jqGrid({
        url: '/CoreNetworkReports/CN3BlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'BT', 'WBM', 'Gravel', 'Track', 'Other', 'Total', 'BT', 'WBM', 'Gravel', 'Track', 'Other', 'Total'],
        colModel: [
            { name: 'BlockName', width: 150, align: 'left', height: 'auto', sortable: true },
            { name: 'BT_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBM_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GRAVEL_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TRACK_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'OTHER_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'CN_BT_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'CN_WBM_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'CN_GRAVEL_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'CN_TRACK_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'CN_OTHER_LEN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TOTAL_CN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },

        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "Route": route, "RoadCategory": roadCategory },
        pager: $("#CN3BlockReportPager"),
        footerrow: true,
        sortname: 'BlockName',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: 420,
        viewrecords: true,
        caption: 'Block Core Network Details for ' + distname,        
        loadComplete: function () {
            //Total of Columns
            var BT_LEN_T = $(this).jqGrid('getCol', 'BT_LEN', false, 'sum');
            BT_LEN_T = parseFloat(BT_LEN_T).toFixed(3);
            var WBM_LEN_T = $(this).jqGrid('getCol', 'WBM_LEN', false, 'sum');
            WBM_LEN_T = parseFloat(WBM_LEN_T).toFixed(3);
            var GRAVEL_LEN_T = $(this).jqGrid('getCol', 'GRAVEL_LEN', false, 'sum');
            GRAVEL_LEN_T = parseFloat(GRAVEL_LEN_T).toFixed(3);
            var TRACK_LEN_T = $(this).jqGrid('getCol', 'TRACK_LEN', false, 'sum');
            TRACK_LEN_T = parseFloat(TRACK_LEN_T).toFixed(3);
            var OTHER_LEN_T = $(this).jqGrid('getCol', 'OTHER_LEN', false, 'sum');
            OTHER_LEN_T = parseFloat(OTHER_LEN_T).toFixed(3);
            var TOTAL_LEN_T = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            TOTAL_LEN_T = parseFloat(TOTAL_LEN_T).toFixed(3);
            var CN_BT_LEN_T = $(this).jqGrid('getCol', 'CN_BT_LEN', false, 'sum');
            CN_BT_LEN_T = parseFloat(CN_BT_LEN_T).toFixed(3);
            var CN_WBM_LEN_T = $(this).jqGrid('getCol', 'CN_WBM_LEN', false, 'sum');
            CN_WBM_LEN_T = parseFloat(CN_WBM_LEN_T).toFixed(3);
            var CN_GRAVEL_LEN_T = $(this).jqGrid('getCol', 'CN_GRAVEL_LEN', false, 'sum');
            CN_GRAVEL_LEN_T = parseFloat(CN_GRAVEL_LEN_T).toFixed(3);
            var CN_TRACK_LEN_T = $(this).jqGrid('getCol', 'CN_TRACK_LEN', false, 'sum');
            CN_TRACK_LEN_T = parseFloat(CN_TRACK_LEN_T).toFixed(3);
            var CN_OTHER_LEN_T = $(this).jqGrid('getCol', 'CN_OTHER_LEN', false, 'sum');
            CN_OTHER_LEN_T = parseFloat(CN_OTHER_LEN_T).toFixed(3);
            var TOTAL_CN_T = $(this).jqGrid('getCol', 'TOTAL_CN', false, 'sum');
            TOTAL_CN_T = parseFloat(TOTAL_CN_T).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { BT_LEN: BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { WBM_LEN: WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { GRAVEL_LEN: GRAVEL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TRACK_LEN: TRACK_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { OTHER_LEN: OTHER_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_BT_LEN: CN_BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_WBM_LEN: CN_WBM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_GRAVEL_LEN: CN_GRAVEL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_TRACK_LEN: CN_TRACK_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_OTHER_LEN: CN_OTHER_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CN: TOTAL_CN_T }, true);
            $('#CN3BlockReportTable_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        //gridComplete: function () {

        //    var ids = jQuery("#CN3BlockReportTable").jqGrid('getDataIDs');
        //    var previousId = 0;

        //    for (var i = 0; i < ids.length; i++) {
        //        var rowId = ids[i];
        //        var rowData = jQuery('#CN3BlockReportTable').jqGrid('getRowData', rowId);              
        //        var newTotalLenST = parseFloat(rowData.BT_LEN) + parseFloat(rowData.WBM_LEN) + parseFloat(rowData.GRAVEL_LEN) + parseFloat(rowData.TRACK_LEN) + parseFloat(rowData.OTHER_LEN);
        //        var newTotalLenCN = parseFloat(rowData.CN_BT_LEN) + parseFloat(rowData.CN_WBM_LEN) + parseFloat(rowData.CN_GRAVEL_LEN) + parseFloat(rowData.CN_TRACK_LEN) + parseFloat(rowData.CN_OTHER_LEN);

        //        $("#CN3BlockReportTable").jqGrid('setCell', rowId, 'TOTAL_LEN', parseFloat(newTotalLenST).toFixed(3).toString().toLocaleString("en-IN"));
        //        $("#CN3BlockReportTable").jqGrid('setCell', rowId, 'TOTAL_CN', parseFloat(newTotalLenCN).toFixed(3).toString().toLocaleString("en-IN"));

        //    }
        //},
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }
    }); /*End of grid*/
    $("#CN3BlockReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
           { startColumnName: 'BT_LEN', numberOfColumns: 6, titleText: '<em> Total road length as per DRRP (Kms) </em>' },
          { startColumnName: 'CN_BT_LEN', numberOfColumns: 6, titleText: '<em> Total road length as per Identified Core Network (Kms) </em>' }

        ]
    });


}

/*       FINAL REPORT LISTING       */
function CN3FinalReportListing(blockCode, districtCode, stateCode, blockName, route, roadCategory) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#CN3BlockReportTable").jqGrid('setGridState', 'hidden');
    $("#CN3BlockReportTable").jqGrid('setSelection', stateCode);
    $("#CN3FinalReportTable").jqGrid('GridUnload');

    $("#CN3FinalReportTable").jqGrid({
        url: '/CoreNetworkReports/CN3FinalReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Road Number', 'Road Name', 'Road From', 'Road To', 'Road Length (Kms)', 'Total Habitation', 'Total Population'],
        colModel: [
            { name: 'RoadNumber', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'RoadFrom', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadTo', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'RoadtoHabitation', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TotalHabitationPopulation', width: 120, align: 'right', sortable: false, height: 'auto', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "Route": route, "RoadCategory": roadCategory },
        pager: $("#CN3FinalReportPager"),
        footerrow: true,
        sortname: 'RoadName',
        rowNum: 2147483647,
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: 360,
        viewrecords: true,
        caption: 'Core Network Details ' + blockName,
        loadComplete: function () {
            //Total of Columns
            var RoadLengthT = $(this).jqGrid('getCol', 'RoadLength', false, 'sum');
            RoadLengthT = parseFloat(RoadLengthT).toFixed(3);

            var RoadtoHabitationT = $(this).jqGrid('getCol', 'RoadtoHabitation', false, 'sum');
            var TotalHabitationPopulationT = $(this).jqGrid('getCol', 'TotalHabitationPopulation', false, 'sum');
            //

            $(this).jqGrid('footerData', 'set', { RoadNumber: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { RoadLength: RoadLengthT }, true);
            $(this).jqGrid('footerData', 'set', { RoadtoHabitation: RoadtoHabitationT }, true);
            $(this).jqGrid('footerData', 'set', { TotalHabitationPopulation: TotalHabitationPopulationT }, true);
            $('#CN3FinalReportTable_rn').html('Sr.<br/>No.');

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