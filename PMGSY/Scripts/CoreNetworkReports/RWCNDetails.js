$(function () {
   

    $("#btRWCNDetails").click(function () {

        var route = $("#Route_RWCNDetails").val();
        var roadCategory = 0;

        if ($("#hdnLevelId").val() == 6) //mord
        {
          
            RWCNStateReportListing(route, roadCategory);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
            
            RWCNDistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), route, roadCategory);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
           
            RWCNBlockReportListing($("#MAST_DISTRICT_CODE").val(), $("#MAST_STATE_CODE").val(), $("#DISTRICT_NAME").val(), route, roadCategory);
        }
    });
 
    $("#btRWCNDetails").trigger("click");
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});


/*       STATE REPORT LISTING       */
function RWCNStateReportListing(route, roadCategory) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#RWCNStateReportTable").jqGrid('GridUnload');
    $("#RWCNDistrictReportTable").jqGrid('GridUnload');
    $("#RWCNBlockReportTable").jqGrid('GridUnload');
    $("#RWCNFinalReportTable").jqGrid('GridUnload');
    $("#RWCNStateReportTable").jqGrid({
        url: '/CoreNetworkReports/RWCNStateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'National Highway (NH)', 'NH Length (Kms)', 'State Highway (SH)', 'SH Length (Kms)', 'Major District Roads (MDR)', 'MDR Length (Kms)', 'Other Roads', 'Other Road Length (Kms)'],
        colModel: [
            { name: 'MAST_STATE_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_NH', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_NH_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_SH', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_SH_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_MDR', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_MDR_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_OTHER', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_OTHER_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        postData: { "Route": route, "RoadCategory": roadCategory },
        pager: $("#RWCNStateReportPager"),
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
            var TOTAL_NH_T = $(this).jqGrid('getCol', 'TOTAL_NH', false, 'sum');
            var TOTAL_NH_LEN_T = $(this).jqGrid('getCol', 'TOTAL_NH_LEN', false, 'sum');
            TOTAL_NH_LEN_T = parseFloat(TOTAL_NH_LEN_T).toFixed(3);
            var TOTAL_SH_T = $(this).jqGrid('getCol', 'TOTAL_SH', false, 'sum');
            var TOTAL_SH_LEN_T = $(this).jqGrid('getCol', 'TOTAL_SH_LEN', false, 'sum');
            TOTAL_SH_LEN_T = parseFloat(TOTAL_SH_LEN_T).toFixed(3);
            var TOTAL_MDR_T = $(this).jqGrid('getCol', 'TOTAL_MDR', false, 'sum');
            var TOTAL_MDR_LEN_T = $(this).jqGrid('getCol', 'TOTAL_MDR_LEN', false, 'sum');
            TOTAL_MDR_LEN_T = parseFloat(TOTAL_MDR_LEN_T).toFixed(3);
            var TOTAL_OTHER_T = $(this).jqGrid('getCol', 'TOTAL_OTHER', false, 'sum');
            var TOTAL_OTHER_LEN_T = $(this).jqGrid('getCol', 'TOTAL_OTHER_LEN', false, 'sum');
            TOTAL_OTHER_LEN_T = parseFloat(TOTAL_OTHER_LEN_T).toFixed(3);
            //

            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_NH: TOTAL_NH_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_NH_LEN: TOTAL_NH_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_SH: TOTAL_SH_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_SH_LEN: TOTAL_SH_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_MDR: TOTAL_MDR_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_MDR_LEN: TOTAL_MDR_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_OTHER: TOTAL_OTHER_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_OTHER_LEN: TOTAL_OTHER_LEN_T }, true);
            $('#RWCNStateReportTable_rn').html('Sr.<br/>No.');
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
function RWCNDistrictReportListing(stateCode, stateName, route, roadCategory) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#RWCNStateReportTable").jqGrid('setGridState', 'hidden');
    $("#RWCNStateReportTable").jqGrid('setSelection', stateCode);
    $("#RWCNDistrictReportTable").jqGrid('GridUnload');
    $("#RWCNBlockReportTable").jqGrid('GridUnload');
    $("#RWCNFinalReportTable").jqGrid('GridUnload');

    $("#RWCNDistrictReportTable").jqGrid({
        url: '/CoreNetworkReports/RWCNDistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'National Highway (NH)', 'NH Length (Kms)', 'State Highway (SH)', 'SH Length (Kms)', 'Major District Roads (MDR)', 'MDR Length (Kms)', 'Other Roads', 'Other Road Length (Kms)'],
        colModel: [
            { name: 'MAST_DISTRICT_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_NH', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_NH_LEN', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_SH', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_SH_LEN', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_MDR', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_MDR_LEN', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_OTHER', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_OTHER_LEN', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        postData: { "StateCode": stateCode, "Route": route, "RoadCategory": roadCategory },
        pager: $("#RWCNDistrictReportPager"),
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
            var TOTAL_NH_T = $(this).jqGrid('getCol', 'TOTAL_NH', false, 'sum');
            var TOTAL_NH_LEN_T = $(this).jqGrid('getCol', 'TOTAL_NH_LEN', false, 'sum');
            TOTAL_NH_LEN_T = parseFloat(TOTAL_NH_LEN_T).toFixed(3);
            var TOTAL_SH_T = $(this).jqGrid('getCol', 'TOTAL_SH', false, 'sum');
            var TOTAL_SH_LEN_T = $(this).jqGrid('getCol', 'TOTAL_SH_LEN', false, 'sum');
            TOTAL_SH_LEN_T = parseFloat(TOTAL_SH_LEN_T).toFixed(3);
            var TOTAL_MDR_T = $(this).jqGrid('getCol', 'TOTAL_MDR', false, 'sum');
            var TOTAL_MDR_LEN_T = $(this).jqGrid('getCol', 'TOTAL_MDR_LEN', false, 'sum');
            TOTAL_MDR_LEN_T = parseFloat(TOTAL_MDR_LEN_T).toFixed(3);
            var TOTAL_OTHER_T = $(this).jqGrid('getCol', 'TOTAL_OTHER', false, 'sum');
            var TOTAL_OTHER_LEN_T = $(this).jqGrid('getCol', 'TOTAL_OTHER_LEN', false, 'sum');
            TOTAL_OTHER_LEN_T = parseFloat(TOTAL_OTHER_LEN_T).toFixed(3);
            //

            $(this).jqGrid('footerData', 'set', { MAST_DISTRICT_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_NH: TOTAL_NH_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_NH_LEN: TOTAL_NH_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_SH: TOTAL_SH_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_SH_LEN: TOTAL_SH_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_MDR: TOTAL_MDR_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_MDR_LEN: TOTAL_MDR_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_OTHER: TOTAL_OTHER_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_OTHER_LEN: TOTAL_OTHER_LEN_T }, true);
            $('#RWCNDistrictReportTable_rn').html('Sr.<br/>No.');
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
function RWCNBlockReportListing(districtCode, stateCode, districtName, route, roadCategory) {
    var distname;
    if (districtName == '')
        distname = $("#DISTRICT_NAME").val();
    else
        distname = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#RWCNDistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#RWCNStateReportTable").jqGrid('setGridState', 'hidden');
    $("#RWCNDistrictReportTable").jqGrid('setSelection', stateCode);
    $("#RWCNBlockReportTable").jqGrid('GridUnload');  
    $("#RWCNFinalReportTable").jqGrid('GridUnload');
    $("#RWCNBlockReportTable").jqGrid({
        url: '/CoreNetworkReports/RWCNBlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'National Highway (NH)', 'NH Length (Kms)', 'State Highway (SH)', 'SH Length (Kms)', 'Major District Roads (MDR)', 'MDR Length (Kms)', 'Other Roads', 'Other Road Length (Kms)'],
        colModel: [
            { name: 'MAST_BLOCK_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_NH', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_NH_LEN', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_SH', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_SH_LEN', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_MDR', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_MDR_LEN', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_OTHER', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_OTHER_LEN', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "Route": route, "RoadCategory": roadCategory },
        pager: $("#RWCNBlockReportPager"),
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
            var TOTAL_NH_T = $(this).jqGrid('getCol', 'TOTAL_NH', false, 'sum');
            var TOTAL_NH_LEN_T = $(this).jqGrid('getCol', 'TOTAL_NH_LEN', false, 'sum');
            TOTAL_NH_LEN_T = parseFloat(TOTAL_NH_LEN_T).toFixed(3);
            var TOTAL_SH_T = $(this).jqGrid('getCol', 'TOTAL_SH', false, 'sum');
            var TOTAL_SH_LEN_T = $(this).jqGrid('getCol', 'TOTAL_SH_LEN', false, 'sum');
            TOTAL_SH_LEN_T = parseFloat(TOTAL_SH_LEN_T).toFixed(3);
            var TOTAL_MDR_T = $(this).jqGrid('getCol', 'TOTAL_MDR', false, 'sum');
            var TOTAL_MDR_LEN_T = $(this).jqGrid('getCol', 'TOTAL_MDR_LEN', false, 'sum');
            TOTAL_MDR_LEN_T = parseFloat(TOTAL_MDR_LEN_T).toFixed(3);
            var TOTAL_OTHER_T = $(this).jqGrid('getCol', 'TOTAL_OTHER', false, 'sum');
            var TOTAL_OTHER_LEN_T = $(this).jqGrid('getCol', 'TOTAL_OTHER_LEN', false, 'sum');
            TOTAL_OTHER_LEN_T = parseFloat(TOTAL_OTHER_LEN_T).toFixed(3);
            //

            $(this).jqGrid('footerData', 'set', { MAST_BLOCK_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_NH: TOTAL_NH_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_NH_LEN: TOTAL_NH_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_SH: TOTAL_SH_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_SH_LEN: TOTAL_SH_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_MDR: TOTAL_MDR_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_MDR_LEN: TOTAL_MDR_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_OTHER: TOTAL_OTHER_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_OTHER_LEN: TOTAL_OTHER_LEN_T }, true);
            $('#RWCNBlockReportTable_rn').html('Sr.<br/>No.');
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
function RWCNFinalReportListing(blockCode, districtCode, stateCode, blockName, route, roadCategory) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#RWCNBlockReportTable").jqGrid('setGridState', 'hidden');
    $("#RWCNDistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#RWCNStateReportTable").jqGrid('setGridState', 'hidden');
    $("#RWCNBlockReportTable").jqGrid('setSelection', stateCode);
    $("#RWCNFinalReportTable").jqGrid('GridUnload');


    $("#RWCNFinalReportTable").jqGrid({
        url: '/CoreNetworkReports/RWCNFinalReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Road Number', 'Road Name', 'Road Route', 'Road From Chainage (Kms)', 'Road To Chainage (Kms)', 'Partial Length', 'Length (Kms)', 'Road From', 'Road To', 'Habitation Name', 'Habitation Population', 'Total Population'],
        colModel: [
          { name: 'PLAN_CN_ROAD_NUMBER', width: 120, align: 'left', height: 'auto', sortable: true },
            { name: 'PLAN_RD_NAME', width: 200, align: 'left', height: 'auto', sortable: false },
            { name: 'PLAN_RD_ROUTE', width: 150, align: 'left', height: 'auto', sortable: false },
            { name: 'PLAN_RD_FROM_CHAINAGE', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', sformatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'PLAN_RD_TO_CHAINAGE', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', sformatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'PLAN_RD_LENG', width: 150, align: 'left', height: 'auto', sortable: false },
            { name: 'PLAN_RD_LENGTH', width: 150, align: 'right', height: 'auto', sortable: false, formatter: 'number', sformatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'RD_FROM', width: 150, align: 'left', height: 'auto', sortable: false },
            { name: 'RD_TO', width: 150, align: 'left', height: 'auto', sortable: false },
            { name: 'Hab_Name', width: 150, align: 'left', height: 'auto', sortable: false },
            { name: 'Hab_Population', width: 150, align: 'right', height: 'auto', sortable: false },
            { name: 'Total_Population', width: 150, align: 'right', height: 'auto', sortable: false },
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "Route": route, "RoadCategory": roadCategory },
        pager: $("#RWCNFinalReportPager"),
        footerrow: true,
        sortname: 'PLAN_RD_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '370',
        viewrecords: true,
        caption: 'Core Network Details for ' + blockName,
        loadComplete: function () {
            //Total of Columns
            var PLAN_RD_FROM_CHAINAGE_T = $(this).jqGrid('getCol', 'PLAN_RD_FROM_CHAINAGE', false, 'sum');
            PLAN_RD_FROM_CHAINAGE_T = parseFloat(PLAN_RD_FROM_CHAINAGE_T).toFixed(3);
            var PLAN_RD_TO_CHAINAGE_T = $(this).jqGrid('getCol', 'PLAN_RD_TO_CHAINAGE', false, 'sum');
            PLAN_RD_TO_CHAINAGE_T = parseFloat(PLAN_RD_TO_CHAINAGE_T).toFixed(3);
            var PLAN_RD_LENGTH_T = $(this).jqGrid('getCol', 'PLAN_RD_LENGTH', false, 'sum');
            PLAN_RD_LENGTH_T = parseFloat(PLAN_RD_LENGTH_T).toFixed(3);
            var Hab_Population_T = $(this).jqGrid('getCol', 'Hab_Population', false, 'sum');
            var Total_Population_T = $(this).jqGrid('getCol', 'Total_Population', false, 'sum');
            //

            $(this).jqGrid('footerData', 'set', { PLAN_CN_ROAD_NUMBER: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { PLAN_RD_FROM_CHAINAGE: PLAN_RD_FROM_CHAINAGE_T }, true);
            $(this).jqGrid('footerData', 'set', { PLAN_RD_TO_CHAINAGE: PLAN_RD_TO_CHAINAGE_T }, true);
            $(this).jqGrid('footerData', 'set', { PLAN_RD_LENGTH: PLAN_RD_LENGTH_T }, true);
            $(this).jqGrid('footerData', 'set', { Hab_Population: Hab_Population_T }, true);
            $(this).jqGrid('footerData', 'set', { Total_Population: Total_Population_T }, true);
            $('#RWCNFinalReportTable_rn').html('Sr.<br/>No.');

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