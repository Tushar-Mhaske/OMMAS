$(function () {
   

    $("#btCN6Details").click(function () {

        var route = $("#Route_CN6Details").val();

        if ($("#hdnLevelId").val() == 6) //mord
        {
           
            CN6StateReportListing(route);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
         
            CN6DistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), route);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
           
            CN6BlockReportListing($("#MAST_DISTRICT_CODE").val(), $("#MAST_STATE_CODE").val(), $("#DISTRICT_NAME").val(), route);
        }
    });
   

    $("#btCN6Details").trigger("click");
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});


/*       STATE REPORT LISTING       */
function CN6StateReportListing(route) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CN6StateReportTable").jqGrid('GridUnload');
    $("#CN6DistrictReportTable").jqGrid('GridUnload');
    $("#CN6BlockReportTable").jqGrid('GridUnload');
    $("#CN6FinalReportTable").jqGrid('GridUnload');
    $("#CN6StateReportTable").jqGrid({
        url: '/CoreNetworkReports/CN6StateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Core Network Number', 'Core Network Length (Kms) ', 'Number of Habitations', 'Total Population'],
        colModel: [
            { name: 'MAST_STATE_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TOTAL_HABS', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_POP', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        postData: {"Route": route },
        pager: $("#CN6StateReportPager"),
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
            $('#CN6StateReportTable_rn').html('Sr.<br/>No.');
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
function CN6DistrictReportListing(stateCode, stateName, route) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CN6StateReportTable").jqGrid('setSelection', stateCode);
    $("#CN6StateReportTable").jqGrid('setGridState', 'hidden');
    $("#CN6DistrictReportTable").jqGrid('GridUnload');
    $("#CN6BlockReportTable").jqGrid('GridUnload');
    $("#CN6FinalReportTable").jqGrid('GridUnload');

    $("#CN6DistrictReportTable").jqGrid({
        url: '/CoreNetworkReports/CN6DistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Core Network Number', 'Core Network Length (Kms) ', 'Number of Habitations', 'Total Population'],
        colModel: [
            { name: 'MAST_DISTRICT_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TOTAL_HABS', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_POP', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        postData: { "StateCode": stateCode, "Route": route },
        pager: $("#CN6DistrictReportPager"),
        footerrow: true,
        sortname: 'MAST_DISTRICT_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '460',
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
            $('#CN6DistrictReportTable_rn').html('Sr.<br/>No.');
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
function CN6BlockReportListing(districtCode, stateCode, districtName, route) {

    var distname;
    if (districtName == '') {

        distname = $("#DISTRICT_NAME").val();
    }
    else {
        distname = districtName;
    }


    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CN6DistrictReportTable").jqGrid('setSelection', stateCode);
    $("#CN6DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#CN6StateReportTable").jqGrid('setGridState', 'hidden');
    $("#CN6BlockReportTable").jqGrid('GridUnload');
    $("#CN6FinalReportTable").jqGrid('GridUnload');
    $("#CN6BlockReportTable").jqGrid({
        url: '/CoreNetworkReports/CN6BlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Core Network Number', 'Core Network Length (Kms) ', 'Number of Habitations', 'Total Population'],
        colModel: [
            { name: 'MAST_BLOCK_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TOTAL_HABS', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_POP', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "Route": route },
        pager: $("#CN6BlockReportPager"),
        footerrow: true,
        sortname: 'MAST_BLOCK_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '400',
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
            $('#CN6BlockReportTable_rn').html('Sr.<br/>No.');
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

/*       FINAL REPORT LISTING       */
function CN6FinalReportListing(blockCode, districtCode, stateCode, blockName, route) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


    $("#CN6BlockReportTable").jqGrid('setSelection', stateCode);
    $("#CN6BlockReportTable").jqGrid('setGridState', 'hidden');
    $("#CN6DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#CN6StateReportTable").jqGrid('setGridState', 'hidden');
    $("#CN6FinalReportTable").jqGrid('GridUnload');

    $("#CN6FinalReportTable").jqGrid({
        url: '/CoreNetworkReports/CN6FinalReportListing',
        datatype: 'json',
        mtype: 'POST',
        //colNames: ['Road Number', 'Road Name', 'Road From', 'Road To', 'Road Length in KM', 'Habitation Name', 'Habitation Total Population'],
        colNames: ['Block', 'Name of Target Habitation', 'Population', 'Road From', 'Road To', 'Length (in Kms)', 'Through Route / Link Route No.', 'Habitations being connected incidentally', 'Total population'],

        colModel: [
            { name: 'BlockName', width: 120, align: 'left', height: 'auto', sortable: true },
            { name: 'TargetHabs', width: 200, align: 'left', height: 'auto', sortable: false },
            { name: 'HabPop', width: 120, align: 'left', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'RoadFrom', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadTo', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'ThroughRoute', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'HabInc', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'HabIncTotal', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "Route": route },
        pager: $("#CN6FinalReportPager"),
        footerrow: true,
        sortname: 'RoadName',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '360',
        viewrecords: true,
        caption: 'Core Network Details for ' + blockName,
        loadComplete: function () {
            //Total of Columns
            var RoadLengthT = $(this).jqGrid('getCol', 'RoadLength', false, 'sum');
            RoadLengthT = parseFloat(RoadLengthT).toFixed(3);
            var HabPopT = $(this).jqGrid('getCol', 'HabPop', false, 'sum');
            var HabIncTotalT = $(this).jqGrid('getCol', 'HabIncTotal', false, 'sum');
            //

            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { RoadLength: RoadLengthT }, true);
            $(this).jqGrid('footerData', 'set', { HabPop: HabPopT }, true);
            $(this).jqGrid('footerData', 'set', { HabIncTotal: HabIncTotalT }, true);
            $('#CN6FinalReportTable_rn').html('Sr.<br/>No.');

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