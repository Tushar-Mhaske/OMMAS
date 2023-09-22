$(function () {
   

    $("#btHWCNDetails").click(function () {

        var route = $("#Route_HWCNDetails").val();

        if ($("#hdnLevelId").val() == 6) //mord
        {
         
            HWCNStateReportListing(route);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
           
            HWCNDistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), route);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
           
            HWCNBlockReportListing($("#MAST_DISTRICT_CODE").val(), $("#MAST_STATE_CODE").val(), $("#DISTRICT_NAME").val(), route);
        }
    }); 
    $("#btHWCNDetails").trigger("click");
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});

/*       STATE REPORT LISTING       */
function HWCNStateReportListing(route) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#HWCNStateReportTable").jqGrid('GridUnload');
    $("#HWCNDistrictReportTable").jqGrid('GridUnload');
    $("#HWCNBlockReportTable").jqGrid('GridUnload');
    $("#HWCNFinalReportTable").jqGrid('GridUnload');
    $("#HWCNStateReportTable").jqGrid({
        url: '/CoreNetworkReports/HWCNStateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Total Core Network Number', 'Total Length (Kms)', 'Total Habitation', 'Total Population'],
        colModel: [
            { name: 'MAST_STATE_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TOTAL_HABS', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_POP', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "Route": route },
        pager: $("#HWCNStateReportPager"),
        footerrow: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '510',
        viewrecords: true,
        caption: 'State Core Network Details',
        loadComplete: function () {
            //Total of Columns
            var TOTAL_CN_T = $(this).jqGrid('getCol', 'TOTAL_CN', false, 'sum');
            var TOTAL_LEN_T = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            TOTAL_LEN_T = parseFloat(TOTAL_LEN_T).toFixed(3);
            var TOTAL_HABS_T = $(this).jqGrid('getCol', 'TOTAL_HABS', false, 'sum');
            var TOTAL_POP_T = $(this).jqGrid('getCol', 'TOTAL_POP', false, 'sum');
            //

            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_CN: TOTAL_CN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_HABS: TOTAL_HABS_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_POP: TOTAL_POP_T }, true);
            $('#HWCNStateReportTable_rn').html('Sr.<br/>No.');
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

/*       DISTRICT REPORT LISTING       */
function HWCNDistrictReportListing(stateCode, stateName, route) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#HWCNStateReportTable").jqGrid('setGridState', 'hidden');
    $("#HWCNStateReportTable").jqGrid('setSelection', stateCode);
    $("#HWCNDistrictReportTable").jqGrid('GridUnload');
    $("#HWCNBlockReportTable").jqGrid('GridUnload');
    $("#HWCNFinalReportTable").jqGrid('GridUnload');

    $("#HWCNDistrictReportTable").jqGrid({
        url: '/CoreNetworkReports/HWCNDistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Total Core Network Number', 'Total Length (Kms)', 'Total Habitation', 'Total Population'],
        colModel: [
            { name: 'MAST_DISTRICT_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TOTAL_HABS', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_POP', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "Route": route },
        pager: $("#HWCNDistrictReportPager"),
        footerrow: true,
        sortname: 'MAST_DISTRICT_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '470',
        viewrecords: true,
        caption: 'District Core Network Details for ' + stateName,
        loadComplete: function () {
           
            //Total of Columns
            var TOTAL_CN_T = $(this).jqGrid('getCol', 'TOTAL_CN', false, 'sum');
            var TOTAL_LEN_T = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            TOTAL_LEN_T = parseFloat(TOTAL_LEN_T).toFixed(3);
            var TOTAL_HABS_T = $(this).jqGrid('getCol', 'TOTAL_HABS', false, 'sum');
            var TOTAL_POP_T = $(this).jqGrid('getCol', 'TOTAL_POP', false, 'sum');
            //

            $(this).jqGrid('footerData', 'set', { MAST_DISTRICT_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_CN: TOTAL_CN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_HABS: TOTAL_HABS_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_POP: TOTAL_POP_T }, true);
            $('#HWCNDistrictReportTable_rn').html('Sr.<br/>No.');
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

/*       BLOCK REPORT LISTING       */
function HWCNBlockReportListing(districtCode, stateCode, districtName, route) {
    var distname;
    if (districtName == '')
        distname = $("#DISTRICT_NAME").val();
    else
        distname = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#HWCNDistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#HWCNStateReportTable").jqGrid('setGridState', 'hidden');
    $("#HWCNDistrictReportTable").jqGrid('setSelection', stateCode);
    $("#HWCNBlockReportTable").jqGrid('GridUnload');
    $("#HWCNFinalReportTable").jqGrid('GridUnload');

    $("#HWCNBlockReportTable").jqGrid({
        url: '/CoreNetworkReports/HWCNBlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Total Core Network Number', 'Total Length (Kms)', 'Total Habitation', 'Total Population'],
        colModel: [
            { name: 'MAST_BLOCK_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TOTAL_HABS', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_POP', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "Route": route },
        pager: $("#HWCNBlockReportPager"),
        footerrow: true,
        sortname: 'MAST_BLOCK_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '420',
        viewrecords: true,
        caption: 'Block Core Network Details for ' + distname,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_CN_T = $(this).jqGrid('getCol', 'TOTAL_CN', false, 'sum');
            var TOTAL_LEN_T = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            TOTAL_LEN_T = parseFloat(TOTAL_LEN_T).toFixed(3);
            var TOTAL_HABS_T = $(this).jqGrid('getCol', 'TOTAL_HABS', false, 'sum');
            var TOTAL_POP_T = $(this).jqGrid('getCol', 'TOTAL_POP', false, 'sum');
            //

            $(this).jqGrid('footerData', 'set', { MAST_BLOCK_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_CN: TOTAL_CN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_HABS: TOTAL_HABS_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_POP: TOTAL_POP_T }, true);
            $('#HWCNBlockReportTable_rn').html('Sr.<br/>No.');
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

/*       FINAL REPORT LISTING       */
function HWCNFinalReportListing(blockCode, districtCode, stateCode, blockName, route) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#HWCNBlockReportTable").jqGrid('setGridState', 'hidden');
    $("#HWCNDistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#HWCNStateReportTable").jqGrid('setGridState', 'hidden');
    $("#HWCNBlockReportTable").jqGrid('setSelection', stateCode);
    $("#HWCNFinalReportTable").jqGrid('GridUnload');

    $("#HWCNFinalReportTable").jqGrid({
        url: '/CoreNetworkReports/HWCNFinalReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Habitation Name', 'Road Number', 'Road Name', 'Road Route', 'Road From Chainage', 'Road To Chainage', 'Partial/Full Length', 'Road Length (Kms)', 'Road From', 'Road To', 'Total Population'],
        colModel: [
            { name: 'HabitationName', width: 150, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadNumber', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadName', width: 250, align: 'left', height: 'auto', sortable: true },
            { name: 'RoadRoute', width: 150, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadFromChainage', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadToChainage', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'PLAN_RD_LENG', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadLength', width: 180, align: 'right', height: 'auto', sortable: false, formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'RoadFrom', width: 180, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadTo', width: 180, align: 'left', height: 'auto', sortable: false },
            { name: 'TotalPopulation', width: 180, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode,"Route":route },
        pager: $("#HWCNFinalReportPager"),
        footerrow: true,
        sortname: 'RoadName',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '370',
        viewrecords: true,
        caption: 'Core Network Details for ' + blockName,
        loadComplete: function () {
            //Total of Columns
            var RoadLengthT = $(this).jqGrid('getCol', 'RoadLength', false, 'sum');
            RoadLengthT = parseFloat(RoadLengthT).toFixed(3);
            var TotalPopulationT = $(this).jqGrid('getCol', 'TotalPopulation', false, 'sum');
            //

            $(this).jqGrid('footerData', 'set', { RoadNumber: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { RoadLength: RoadLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TotalPopulation: TotalPopulationT }, true);
            $('#HWCNFinalReportTable_rn').html('Sr.<br/>No.');

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