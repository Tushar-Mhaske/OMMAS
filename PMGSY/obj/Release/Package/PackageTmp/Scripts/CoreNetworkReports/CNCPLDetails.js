$(function () {
   
   
    $("#btCNCPLDetails").click(function () {

        var route = $("#Route_CNCPLDetails").val();

        if ($("#hdnLevelId").val() == 6) //mord
        {
          
            CNCPLStateReportListing(route);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
          
            CNCPLDistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), route);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
           
            CNCPLBlockReportListing( $("#MAST_DISTRICT_CODE").val(),$("#MAST_STATE_CODE").val(), $("#DISTRICT_NAME").val(), route);
        }
    });

  
    $("#btCNCPLDetails").trigger("click");
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});


/*       STATE REPORT LISTING       */
function CNCPLStateReportListing(route) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNCPLStateReportTable").jqGrid('GridUnload');
    $("#CNCPLDistrictReportTable").jqGrid('GridUnload');
    $("#CNCPLBlockReportTable").jqGrid('GridUnload');
    $("#CNCPLFinalReportTable").jqGrid('GridUnload');

    $("#CNCPLStateReportTable").jqGrid({
        url: '/CoreNetworkReports/CNCPLStateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Number of Core Network', 'Total Length (Kms)', 'Total Habitation', 'Total Population'],
        colModel: [
            { name: 'MAST_STATE_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TOTAL_HABS', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_POP', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "Route": route },
        pager: $("#CNCPLStateReportPager"),
        footerrow: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '550',
        viewrecords: true,
        caption: 'State Core Network New Connectivity Priority List Details',
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
            $('#CNCPLStateReportTable_rn').html('Sr.<br/>No.');
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
function CNCPLDistrictReportListing(stateCode, stateName, route) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNCPLStateReportTable").jqGrid('setSelection', stateCode);
    $("#CNCPLStateReportTable").jqGrid('setGridState', 'hidden');
    $("#CNCPLDistrictReportTable").jqGrid('GridUnload');
    $("#CNCPLBlockReportTable").jqGrid('GridUnload');
    $("#CNCPLFinalReportTable").jqGrid('GridUnload');
    $("#CNCPLDistrictReportTable").jqGrid({
        url: '/CoreNetworkReports/CNCPLDistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Number of Core Network', 'Total Length (Kms)', 'Total Habitation', 'Total Population'],
        colModel: [
            { name: 'MAST_DISTRICT_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TOTAL_HABS', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_POP', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "Route": route },
        pager: $("#CNCPLDistrictReportPager"),
        footerrow: true,
        sortname: 'MAST_DISTRICT_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '480',
        viewrecords: true,
        caption: 'District Core Network New Connectivity Priority List Details for ' + stateName,
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
            $('#CNCPLDistrictReportTable_rn').html('Sr.<br/>No.');
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
function CNCPLBlockReportListing(districtCode, stateCode, districtName, route) {
    var distname;
    if (districtName == '') {

        distname = $("#DISTRICT_NAME").val();
    }
    else {
        distname = districtName;
    }
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNCPLDistrictReportTable").jqGrid('setSelection', stateCode);
    $("#CNCPLDistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#CNCPLStateReportTable").jqGrid('setGridState', 'hidden');
    $("#CNCPLBlockReportTable").jqGrid('GridUnload');
    $("#CNCPLFinalReportTable").jqGrid('GridUnload');

    $("#CNCPLBlockReportTable").jqGrid({
        url: '/CoreNetworkReports/CNCPLBlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Number of Core Network', 'Total Length (Kms)', 'Total Habitation', 'Total Population'],
        colModel: [
            { name: 'MAST_BLOCK_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TOTAL_HABS', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_POP', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "Route": route },
        pager: $("#CNCPLBlockReportPager"),
        footerrow: true,
        sortname: 'MAST_BLOCK_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '420',
        viewrecords: true,
        caption: 'Block Core Network New Connectivity Priority List Details for ' + distname,
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
            $('#CNCPLBlockReportTable_rn').html('Sr.<br/>No.');
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
function CNCPLFinalReportListing(blockCode, districtCode, stateCode, blockName, route) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNCPLBlockReportTable").jqGrid('setSelection', stateCode);
    $("#CNCPLBlockReportTable").jqGrid('setGridState', 'hidden');
    $("#CNCPLDistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#CNCPLStateReportTable").jqGrid('setGridState', 'hidden');
    $("#CNCPLFinalReportTable").jqGrid('GridUnload');

    $("#CNCPLFinalReportTable").jqGrid({
        url: '/CoreNetworkReports/CNCPLFinalReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Road Number', 'Road Name', 'Road Route', 'Partial/Full Length', 'Road Length (Kms)', 'Road From', 'Road To', 'Target Habitation', 'Population of Target Habitation', 'Total Population Served'],
        colModel: [
            { name: 'RoadNumber', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadName', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'RoadRoute', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'PLAN_RD_LENG', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'RoadFrom', width: 200, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadTo', width: 200, align: 'left', height: 'auto', sortable: false },
            { name: 'HabitationName', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'TotalPopulation', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TotalPopulationServed', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "Route": route },
        pager: $("#CNCPLFinalReportPager"),
        footerrow: true,
        sortname: 'RoadName',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '330',
        viewrecords: true,
        caption: 'Core Network New Connectivity Priority List Details for ' + blockName,
        loadComplete: function () {
            //Total of Columns
            var RoadLengthT = $(this).jqGrid('getCol', 'RoadLength', false, 'sum');
            RoadLengthT = parseFloat(RoadLengthT).toFixed(3);
            var TotalPopulationT = $(this).jqGrid('getCol', 'TotalPopulation', false, 'sum');
            var TotalPopulationServedT = $(this).jqGrid('getCol', 'TotalPopulationServed', false, 'sum');
            //

            $(this).jqGrid('footerData', 'set', { RoadNumber: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { RoadLength: RoadLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TotalPopulation: TotalPopulationT }, true);
            $(this).jqGrid('footerData', 'set', { TotalPopulationServed: TotalPopulationServedT }, true);
            $('#CNCPLFinalReportTable_rn').html('Sr.<br/>No.');

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