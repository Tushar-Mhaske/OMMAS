$(document).ready(function () {

    $('#ddCBR_ERR5').change(function () {
        var cbrValue = $('#ddCBR_ERR5').val();
        loadLevelWiseGrid(cbrValue);
    });
    $('#ddCBR_ERR5').trigger('change');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});

function loadLevelWiseGrid(cbrValue) {
    if ($("#hdnLevelId").val() == 6) //mord
    {
        ERR5StateReportListing(cbrValue);
    }
    else if ($("#hdnLevelId").val() == 4) //state
    {
        ERR5DistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), cbrValue);
    }
    else if ($("#hdnLevelId").val() == 5) //District
    {
        ERR5BlockReportListing($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val(), $("#BLOCK_NAME").val(), cbrValue);
    }
}
/*       STATE REPORT LISTING       */
function ERR5StateReportListing(cbrValue) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR5StateReportTable").jqGrid('GridUnload');
    $("#ERR5DistrictReportTable").jqGrid('GridUnload');
    $("#ERR5BlockReportTable").jqGrid('GridUnload');
    $("#ERR5FinalReportTable").jqGrid('GridUnload');

    $("#ERR5StateReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR5StateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'CBR Less than 3', 'CBR 3 to 4.99', 'CBR 5 to 9.99', 'CBR with 10 and more', 'Undefined', 'Total'],
        colModel: [
            { name: 'StateName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'CBR3', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CBR4', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CBR9', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CBR10', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CBRUndefined', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CBRTotal', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },

        ],
        postData: {"CBRValue": cbrValue },
        pager: $("#ERR5StateReportPager"),
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'StateName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '550',
        viewrecords: true,
        caption: 'State  DRRP CBR Wise Length Details',
        loadComplete: function () {
            //Total of Columns
            var CBR3T = $(this).jqGrid('getCol', 'CBR3', false, 'sum');
            var CBR4T = $(this).jqGrid('getCol', 'CBR4', false, 'sum');
            var CBR9T = $(this).jqGrid('getCol', 'CBR9', false, 'sum');
            var CBR10T = $(this).jqGrid('getCol', 'CBR10', false, 'sum');
            var CBRUndefinedT = $(this).jqGrid('getCol', 'CBRUndefined', false, 'sum');
            var CBRTotalT = $(this).jqGrid('getCol', 'CBRTotal', false, 'sum');

            //

            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { CBR3: CBR3T }, true);
            $(this).jqGrid('footerData', 'set', { CBR4: CBR4T }, true);
            $(this).jqGrid('footerData', 'set', { CBR9: CBR9T }, true);
            $(this).jqGrid('footerData', 'set', { CBR10: CBR10T }, true);
            $(this).jqGrid('footerData', 'set', { CBRUndefined: CBRUndefinedT }, true);
            $(this).jqGrid('footerData', 'set', { CBRTotal: CBRTotalT }, true);
            $('#ERR5StateReportTable_rn').html('Sr.<br/>No.');

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

    //$("#ERR5StateReportTable").jqGrid('setGroupHeaders', {
    //    useColSpanStyle: false,
    //    groupHeaders: [
    //      { startColumnName: 'CBR3', numberOfColumns: 6, titleText: '<em>CBR (Length in Kms.) </em>' }
    //    ]
    //});
}
/**/

/*       DISTRICT REPORT LISTING       */
function ERR5DistrictReportListing(stateCode, stateName, cbrValue) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR5StateReportTable").jqGrid('setSelection', stateCode);
    $("#ERR5StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR5DistrictReportTable").jqGrid('GridUnload');
    $("#ERR5BlockReportTable").jqGrid('GridUnload');
    $("#ERR5FinalReportTable").jqGrid('GridUnload');

    $("#ERR5DistrictReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR5DistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'CBR Less than 3', 'CBR 3 to 4.99', 'CBR 5 to 9.99', 'CBR with 10 and more', 'Undefined', 'Total'],
        colModel: [
            { name: 'DistrictName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'CBR3', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CBR4', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CBR9', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CBR10', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CBRUndefined', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CBRTotal', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },

        ],

        pager: $("#ERR5DistrictReportPager"),
        postData: { 'StateCode': stateCode, "CBRValue": cbrValue },
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'DistrictName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '520',
        viewrecords: true,
        caption: 'District  DRRP CBR Wise Length Details for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var CBR3T = $(this).jqGrid('getCol', 'CBR3', false, 'sum');
            var CBR4T = $(this).jqGrid('getCol', 'CBR4', false, 'sum');
            var CBR9T = $(this).jqGrid('getCol', 'CBR9', false, 'sum');
            var CBR10T = $(this).jqGrid('getCol', 'CBR10', false, 'sum');
            var CBRUndefinedT = $(this).jqGrid('getCol', 'CBRUndefined', false, 'sum');
            var CBRTotalT = $(this).jqGrid('getCol', 'CBRTotal', false, 'sum');

            //

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { CBR3: CBR3T }, true);
            $(this).jqGrid('footerData', 'set', { CBR4: CBR4T }, true);
            $(this).jqGrid('footerData', 'set', { CBR9: CBR9T }, true);
            $(this).jqGrid('footerData', 'set', { CBR10: CBR10T }, true);
            $(this).jqGrid('footerData', 'set', { CBRUndefined: CBRUndefinedT }, true);
            $(this).jqGrid('footerData', 'set', { CBRTotal: CBRTotalT }, true);
            $('#ERR5DistrictReportTable_rn').html('Sr.<br/>No.');

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

    //$("#ERR5DistrictReportTable").jqGrid('setGroupHeaders', {
    //    useColSpanStyle: false,
    //    groupHeaders: [
    //      { startColumnName: 'CBR3', numberOfColumns: 6, titleText: '<em>CBR (Length in Kms.) </em>' }
    //    ]
    //});
}
/**/

/*      BLOCK REPORT LISTING       */

function ERR5BlockReportListing(stateCode, districtCode, districtName, cbrValue) {

    var distname;
    if (districtName == '')
        distname = $("#DISTRICT_NAME").val();
    else
        distname = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR5DistrictReportTable").jqGrid('setSelection', districtCode);
    $("#ERR5DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR5StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR5BlockReportTable").jqGrid('GridUnload');
    $("#ERR5FinalReportTable").jqGrid('GridUnload');

    $("#ERR5BlockReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR5BlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'CBR Less than 3', 'CBR 3 to 4.99', 'CBR 5 to 9.99', 'CBR with 10 and more', 'Undefined', 'Total'],
        colModel: [
            { name: 'BlockName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'CBR3', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CBR4', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CBR9', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CBR10', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CBRUndefined', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CBRTotal', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },

        ],
        pager: $("#ERR5BlockReportPager"),
        postData: { 'StateCode': stateCode, 'DistrictCode': districtCode, "CBRValue": cbrValue },
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'BlockName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '480',
        viewrecords: true,
        caption: 'Block  DRRP CBR Wise Length Details for ' + distname,
        loadComplete: function () {
            //Total of Columns
            var CBR3T = $(this).jqGrid('getCol', 'CBR3', false, 'sum');
            var CBR4T = $(this).jqGrid('getCol', 'CBR4', false, 'sum');
            var CBR9T = $(this).jqGrid('getCol', 'CBR9', false, 'sum');
            var CBR10T = $(this).jqGrid('getCol', 'CBR10', false, 'sum');
            var CBRUndefinedT = $(this).jqGrid('getCol', 'CBRUndefined', false, 'sum');
            var CBRTotalT = $(this).jqGrid('getCol', 'CBRTotal', false, 'sum');

            //

            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { CBR3: CBR3T }, true);
            $(this).jqGrid('footerData', 'set', { CBR4: CBR4T }, true);
            $(this).jqGrid('footerData', 'set', { CBR9: CBR9T }, true);
            $(this).jqGrid('footerData', 'set', { CBR10: CBR10T }, true);
            $(this).jqGrid('footerData', 'set', { CBRUndefined: CBRUndefinedT }, true);
            $(this).jqGrid('footerData', 'set', { CBRTotal: CBRTotalT }, true);
            $('#ERR5BlockReportTable_rn').html('Sr.<br/>No.');

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

    //$("#ERR5BlockReportTable").jqGrid('setGroupHeaders', {
    //    useColSpanStyle: false,
    //    groupHeaders: [
    //      { startColumnName: 'CBR3', numberOfColumns: 6, titleText: '<em>CBR (Length in Kms.) </em>' }
    //    ]
    //});
}

/*       FINAL BLOCK REPORT LISTING       */

function ERR5FinalReportListing(blockCode, districtCode, stateCode, blockName, cbrValue) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR5BlockReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR5DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR5StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR5BlockReportTable").jqGrid('setSelection', blockCode);
    $("#ERR5FinalReportTable").jqGrid('GridUnload');
    $("#ERR5FinalReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR5FinalReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Road Code', 'Road Name', 'Road Category', 'Road Type', 'Length (Kms.)', 'Year of Construction', 'Included in Core Network (Y/N)', 'Soil type', 'Terrain Type', 'Start Chainage', 'End Chainage', 'CBR Value'],
        colModel: [
            { name: 'PlannedRoadNumber', width: 150, align: 'left', height: 'auto', sortable: true },
            { name: 'PlannedRoadName', width: 250, align: 'left', height: 'auto', sortable: false },
            { name: 'MAST_ROAD_CAT_CODE', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'MAST_ER_ROAD_TYPE', width: 150, align: 'left', height: 'auto', sortable: false },
            { name: 'ROAD_LENGTH', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'MAST_CONS_YEAR', width: 120, align: 'center', height: 'auto', sortable: false },
            { name: 'MAST_CORE_NETWORK', width: 120, align: 'center', height: 'auto', sortable: false },
            { name: 'MAST_SOIL_TYPE_NAME', width: 200, align: 'left', height: 'auto', sortable: false },
            { name: 'MAST_TERRAIN_TYPE_NAME', width: 200, align: 'left', height: 'auto', sortable: false },
            { name: 'StartChainage', width: 120, align: 'right', height: 'auto', sortable: false },
            { name: 'EndChainage', width: 120, align: 'right', height: 'auto', sortable: false },
            { name: 'CBRValue', width: 120, align: 'right', height: 'auto', sortable: false }
        ],

        postData: { "BlockCode": blockCode, "StateCode": stateCode, "DistrictCode": districtCode, "CBRValue": cbrValue },
        rowNum: '2147483647',
        pager: $("#ERR5FinalReportPager"),
        footerrow: true,
        pgbuttons: true,
        sortname: 'PlannedRoadNumber',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '400',
        viewrecords: true,
        caption: 'DRRP CBR Wise Length Details for ' + blockName,
        loadComplete: function () {
            //Total of Columns
            var PlannedRoadlengthT = $(this).jqGrid('getCol', 'PlannedRoadlength', false, 'sum');
            PlannedRoadlengthT = parseFloat(PlannedRoadlengthT).toFixed(3);
            var RoadLengthT = $(this).jqGrid('getCol', 'ROAD_LENGTH', false, 'sum');
            RoadLengthT = parseFloat(RoadLengthT).toFixed(2);
            //var StartChainageT = $(this).jqGrid('getCol', 'StartChainage', false, 'sum');
            //var EndChainageT = $(this).jqGrid('getCol', 'EndChainage', false, 'sum');
            //var CBRValueT = $(this).jqGrid('getCol', 'CBRValue', false, 'sum');
            $(this).jqGrid('footerData', 'set', { PlannedRoadNumber: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { ROAD_LENGTH: RoadLengthT });         
            //$(this).jqGrid('footerData', 'set', { StartChainage: StartChainageT });
            //$(this).jqGrid('footerData', 'set', { EndChainage: EndChainageT });
            //$(this).jqGrid('footerData', 'set', { CBRValue: CBRValueT });
            $('#ERR5FinalReportTable_rn').html('Sr.<br/>No.');
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