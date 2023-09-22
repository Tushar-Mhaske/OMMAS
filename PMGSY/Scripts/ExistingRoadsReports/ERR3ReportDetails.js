$(document).ready(function () {
    if ($("#hdnLevelId").val() == 6) //mord
    {
           ERR3StateReportListing();
    }
    else if ($("#hdnLevelId").val() == 4) //state
    {
         ERR3DistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val());
    }
    else if ($("#hdnLevelId").val() == 5) //District
    {
        ERR3BlockReportListing($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val(), $("#BLOCK_NAME").val());
    }
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});


/*       STATE REPORT LISTING       */
function ERR3StateReportListing() {
 
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR3DistrictReportTable").jqGrid('GridUnload');
    $("#ERR3BlockReportTable").jqGrid('GridUnload');
    $("#ERR3FinalReportTable").jqGrid('GridUnload');
     $("#ERR3StateReportTable").jqGrid('GridUnload');
     $("#ERR3StateReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR3StateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', '1000+', '499-999', '250-499', '<250', 'Total', '1000+', '499-999', '250-499', '<250', 'Total'],
        colModel: [
            { name: 'StateName', width: 300, align: 'left',  height: 'auto', sortable: true },
            { name: 'DRRPPOP1000', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: {thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'DRRPPOP999', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: {thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'DRRPPOP499', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: {thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'DRRPPOP250', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: {thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'DRRPTotal', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: {thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CNPOP1000', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: {thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CNPOP999', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: {thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CNPOP499', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: {thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CNPOP250', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: {thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CNTotal', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }

        ],
        pager: $("#ERR3StateReportPager"),
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'StateName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'State  DRRP Habitations Details',
        loadComplete: function () {
            //Total of Columns
            var DRRPPOP1000T = $(this).jqGrid('getCol', 'DRRPPOP1000', false, 'sum');
            var DRRPPOP999T = $(this).jqGrid('getCol', 'DRRPPOP999', false, 'sum');
            var DRRPPOP499T = $(this).jqGrid('getCol', 'DRRPPOP499', false, 'sum');
            var DRRPPOP250T = $(this).jqGrid('getCol', 'DRRPPOP250', false, 'sum');
            var DRRPTotalT = $(this).jqGrid('getCol', 'DRRPTotal', false, 'sum');
            var CNPOP1000T = $(this).jqGrid('getCol', 'CNPOP1000', false, 'sum');
            var CNPOP999T = $(this).jqGrid('getCol', 'CNPOP999', false, 'sum');
            var CNPOP499T = $(this).jqGrid('getCol', 'CNPOP499', false, 'sum');
            var CNPOP250T = $(this).jqGrid('getCol', 'CNPOP250', false, 'sum');
            var CNTotalT = $(this).jqGrid('getCol', 'CNTotal', false, 'sum');

            //

            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { DRRPPOP1000: DRRPPOP1000T }, true);
            $(this).jqGrid('footerData', 'set', { DRRPPOP999: DRRPPOP999T }, true);
            $(this).jqGrid('footerData', 'set', { DRRPPOP499: DRRPPOP499T }, true);
            $(this).jqGrid('footerData', 'set', { DRRPPOP250: DRRPPOP250T }, true);
            $(this).jqGrid('footerData', 'set', { DRRPTotal: DRRPTotalT }, true);
            $(this).jqGrid('footerData', 'set', { CNPOP1000: CNPOP1000T }, true);
            $(this).jqGrid('footerData', 'set', { CNPOP999: CNPOP999T }, true);
            $(this).jqGrid('footerData', 'set', { CNPOP499: CNPOP499T }, true);
            $(this).jqGrid('footerData', 'set', { CNPOP250: CNPOP250T }, true);
            $(this).jqGrid('footerData', 'set', { CNTotal: CNTotalT }, true);   
            $('#ERR3StateReportTable_rn').html('Sr.<br/>No.');

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

    $("#ERR3StateReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'DRRPPOP1000', numberOfColumns: 5, titleText: '<em>Number of Habitations as per DRRP</em>' },
          { startColumnName: 'CNPOP1000', numberOfColumns: 5, titleText: '<em>Number of Habitations as per CN </em>' },

        ]
    });
}
/**/

/*       DISTRICT REPORT LISTING       */
function ERR3DistrictReportListing(stateCode, stateName) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR3StateReportTable").jqGrid('setSelection', stateCode);
    $("#ERR3StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR3BlockReportTable").jqGrid('GridUnload');
    $("#ERR3FinalReportTable").jqGrid('GridUnload');
    $("#ERR3DistrictReportTable").jqGrid('GridUnload');

    $("#ERR3DistrictReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR3DistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', '1000+', '499-999', '250-499', '<250', 'Total', '1000+', '499-999', '250-499', '<250', 'Total'],
        colModel: [
            { name: 'DistrictName', width: 300, align: 'left',  height: 'auto', sortable: true },
            { name: 'DRRPPOP1000', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'DRRPPOP999', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'DRRPPOP499', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'DRRPPOP250', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'DRRPTotal', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CNPOP1000', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CNPOP999', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CNPOP499', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CNPOP250', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CNTotal', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }

        ],

        pager: $("#ERR3DistrictReportPager"),
        postData: { 'StateCode': stateCode },
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'DistrictName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '550',
        viewrecords: true,
        caption: 'District DRRP Habitations Details for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var DRRPPOP1000T = $(this).jqGrid('getCol', 'DRRPPOP1000', false, 'sum');
            var DRRPPOP999T = $(this).jqGrid('getCol', 'DRRPPOP999', false, 'sum');
            var DRRPPOP499T = $(this).jqGrid('getCol', 'DRRPPOP499', false, 'sum');
            var DRRPPOP250T = $(this).jqGrid('getCol', 'DRRPPOP250', false, 'sum');
            var DRRPTotalT = $(this).jqGrid('getCol', 'DRRPTotal', false, 'sum');
            var CNPOP1000T = $(this).jqGrid('getCol', 'CNPOP1000', false, 'sum');
            var CNPOP999T = $(this).jqGrid('getCol', 'CNPOP999', false, 'sum');
            var CNPOP499T = $(this).jqGrid('getCol', 'CNPOP499', false, 'sum');
            var CNPOP250T = $(this).jqGrid('getCol', 'CNPOP250', false, 'sum');
            var CNTotalT = $(this).jqGrid('getCol', 'CNTotal', false, 'sum');

            //

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { DRRPPOP1000: DRRPPOP1000T }, true);
            $(this).jqGrid('footerData', 'set', { DRRPPOP999: DRRPPOP999T }, true);
            $(this).jqGrid('footerData', 'set', { DRRPPOP499: DRRPPOP499T }, true);
            $(this).jqGrid('footerData', 'set', { DRRPPOP250: DRRPPOP250T }, true);
            $(this).jqGrid('footerData', 'set', { DRRPTotal: DRRPTotalT }, true);
            $(this).jqGrid('footerData', 'set', { CNPOP1000: CNPOP1000T }, true);
            $(this).jqGrid('footerData', 'set', { CNPOP999: CNPOP999T }, true);
            $(this).jqGrid('footerData', 'set', { CNPOP499: CNPOP499T }, true);
            $(this).jqGrid('footerData', 'set', { CNPOP250: CNPOP250T }, true);
            $(this).jqGrid('footerData', 'set', { CNTotal: CNTotalT }, true);
            $('#ERR3DistrictReportTable_rn').html('Sr.<br/>No.');
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

    $("#ERR3DistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
              { startColumnName: 'DRRPPOP1000', numberOfColumns: 5, titleText: '<em>Number of Habitations as per DRRP</em>' },
              { startColumnName: 'CNPOP1000', numberOfColumns: 5, titleText: '<em>Number of Habitations as per CN </em>' },

        ]
    });
}
/**/

/*      BLOCK REPORT LISTING       */

function ERR3BlockReportListing(stateCode, districtCode, districtName) {

    var distname;
    if (districtName == '')
        distname = $("#DISTRICT_NAME").val();
    else
        distname = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR3DistrictReportTable").jqGrid('setSelection', districtCode);
    $("#ERR3DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR3StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR3FinalReportTable").jqGrid('GridUnload');
    $("#ERR3BlockReportTable").jqGrid('GridUnload');

    $("#ERR3BlockReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR3BlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', '1000+', '499-999', '250-499', '<250', 'Total', '1000+', '499-999', '250-499', '<250', 'Total'],
        colModel: [
            { name: 'BlockName', width: 300, align: 'left',  height: 'auto', sortable: true },
            { name: 'DRRPPOP1000', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'DRRPPOP999', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'DRRPPOP499', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'DRRPPOP250', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'DRRPTotal', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CNPOP1000', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CNPOP999', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CNPOP499', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CNPOP250', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'CNTotal', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }

        ],
        pager: $("#ERR3BlockReportPager"),
        postData: { 'StateCode': stateCode, 'DistrictCode': districtCode },
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'BlockName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '500',
        viewrecords: true,
        caption: 'Block  DRRP Habitations Details for ' + distname,
        loadComplete: function () {
            //Total of Columns
            var DRRPPOP1000T = $(this).jqGrid('getCol', 'DRRPPOP1000', false, 'sum');
            var DRRPPOP999T = $(this).jqGrid('getCol', 'DRRPPOP999', false, 'sum');
            var DRRPPOP499T = $(this).jqGrid('getCol', 'DRRPPOP499', false, 'sum');
            var DRRPPOP250T = $(this).jqGrid('getCol', 'DRRPPOP250', false, 'sum');
            var DRRPTotalT = $(this).jqGrid('getCol', 'DRRPTotal', false, 'sum');
            var CNPOP1000T = $(this).jqGrid('getCol', 'CNPOP1000', false, 'sum');
            var CNPOP999T = $(this).jqGrid('getCol', 'CNPOP999', false, 'sum');
            var CNPOP499T = $(this).jqGrid('getCol', 'CNPOP499', false, 'sum');
            var CNPOP250T = $(this).jqGrid('getCol', 'CNPOP250', false, 'sum');
            var CNTotalT = $(this).jqGrid('getCol', 'CNTotal', false, 'sum');

            //

            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { DRRPPOP1000: DRRPPOP1000T }, true);
            $(this).jqGrid('footerData', 'set', { DRRPPOP999: DRRPPOP999T }, true);
            $(this).jqGrid('footerData', 'set', { DRRPPOP499: DRRPPOP499T }, true);
            $(this).jqGrid('footerData', 'set', { DRRPPOP250: DRRPPOP250T }, true);
            $(this).jqGrid('footerData', 'set', { DRRPTotal: DRRPTotalT }, true);
            $(this).jqGrid('footerData', 'set', { CNPOP1000: CNPOP1000T }, true);
            $(this).jqGrid('footerData', 'set', { CNPOP999: CNPOP999T }, true);
            $(this).jqGrid('footerData', 'set', { CNPOP499: CNPOP499T }, true);
            $(this).jqGrid('footerData', 'set', { CNPOP250: CNPOP250T }, true);
            $(this).jqGrid('footerData', 'set', { CNTotal: CNTotalT }, true);
            $('#ERR3BlockReportTable_rn').html('Sr.<br/>No.');

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

    $("#ERR3BlockReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
           { startColumnName: 'DRRPPOP1000', numberOfColumns: 5, titleText: '<em>Number of Habitations as per DRRP</em>' },
           { startColumnName: 'CNPOP1000', numberOfColumns: 5, titleText: '<em>Number of Habitations as per CN </em>' },

        ]
    });
}

/*       FINAL BLOCK REPORT LISTING       */

function ERR3FinalReportListing(blockCode, districtCode, stateCode, blockName) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR3BlockReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR3DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR3StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR3BlockReportTable").jqGrid('setSelection', blockCode);
    $("#ERR3FinalReportTable").jqGrid('GridUnload');

    $("#ERR3FinalReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR3FinalReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Road Code', 'Road Name', 'Road Category', 'Road Type', 'Road Length (Kms.)', 'Habitation Status (Y/N)','Habitation Name','Population','Included in Core Network (Y/N)'],
        colModel: [
            { name: 'PlannedRoadNumber', width: 150, align: 'left',  height: 'auto', sortable: true },
            { name: 'PlannedRoadName', width: 250, align: 'left',  height: 'auto', sortable: false },
            { name: 'MAST_ROAD_CAT_CODE', width: 120, align: 'left',  height: 'auto', sortable: false },
            { name: 'MAST_ER_ROAD_TYPE', width: 150, align: 'left',  height: 'auto', sortable: false },
            { name: 'ROAD_LENGTH', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'MAST_HAB_STATUS', width: 120, align: 'center',  height: 'auto', sortable: false },
            { name: 'MAST_HAB_NAME', width: 120, align: 'left',  height: 'auto', sortable: false },
            { name: 'MAST_HAB_TOT_POP', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MAST_CORE_NETWORK', width: 120, align: 'center',  height: 'auto', sortable: false }

        ],

        postData: { "BlockCode": blockCode, "StateCode": stateCode, "DistrictCode": districtCode },
        rowNum: '2147483647',
        pager: $("#ERR3FinalReportPager"),
        footerrow: true,
        pgbuttons: true,
        sortname: 'PlannedRoadNumber',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '480',
        viewrecords: true,
        caption: 'DRRP Habitations Details for ' + blockName,
        loadComplete: function () {
            //Total of Columns
            var ROAD_LENGTHT = $(this).jqGrid('getCol', 'ROAD_LENGTH', false, 'sum');
            ROAD_LENGTHT = parseFloat(ROAD_LENGTHT).toFixed(3);
            var NoHablitisationT = $(this).jqGrid('getCol', 'MAST_HAB_TOT_POP', false, 'sum');


            $(this).jqGrid('footerData', 'set', { PlannedRoadNumber: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { ROAD_LENGTH: ROAD_LENGTHT });
            $(this).jqGrid('footerData', 'set', { MAST_HAB_TOT_POP: NoHablitisationT });
            $('#ERR3FinalReportTable_rn').html('Sr.<br/>No.');
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