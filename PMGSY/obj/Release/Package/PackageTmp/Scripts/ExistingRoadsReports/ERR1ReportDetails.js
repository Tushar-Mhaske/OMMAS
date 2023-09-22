$(document).ready(function () {
    if ($("#hdnLevelId").val() == 6) //mord
    {
        
        ERR1StateReportListing();
    }
    else if ($("#hdnLevelId").val() == 4) //state
    {
      
        ERR1DistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val());
    }
    else if ($("#hdnLevelId").val() == 5) //District
    {
      
        ERR1BlockReportListing($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val(), $("#BLOCK_NAME").val());
    }
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});


/*       STATE REPORT LISTING       */
function ERR1StateReportListing() {
  
    $("#ERR1StateReportTable").jqGrid('GridUnload');
    $("#ERR1DistrictReportTable").jqGrid('GridUnload');
    $("#ERR1BlockReportTable").jqGrid('GridUnload');
    $("#ERR1FinalReportTable").jqGrid('GridUnload');
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR1StateReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR1StateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length'],
        colModel: [
            { name: 'StateName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_NH', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_NH_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_SH', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_SH_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_MDR', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: {thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_MDR_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_RR', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_RR_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_OTHER', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: {thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_OTHER_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Total', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: {thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Total_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } }

        ],
        pager: $("#ERR1StateReportPager"),
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'StateName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'State List of DRRP Roads',
        loadComplete: function () {
            //Total of Columns
            var TOTAL_NHT = $(this).jqGrid('getCol', 'TOTAL_NH', false, 'sum');
            var TOTAL_NH_LENT = $(this).jqGrid('getCol', 'TOTAL_NH_LEN', false, 'sum');
            TOTAL_NH_LENT = parseFloat(TOTAL_NH_LENT).toFixed(3);
            var TOTAL_SHT = $(this).jqGrid('getCol', 'TOTAL_SH', false, 'sum');
            var TOTAL_SH_LENT = $(this).jqGrid('getCol', 'TOTAL_SH_LEN', false, 'sum');
            TOTAL_SH_LENT = parseFloat(TOTAL_SH_LENT).toFixed(3);
            var TOTAL_MDRT = $(this).jqGrid('getCol', 'TOTAL_MDR', false, 'sum');
            var TOTAL_MDR_LENT = $(this).jqGrid('getCol', 'TOTAL_MDR_LEN', false, 'sum');
            TOTAL_MDR_LENT = parseFloat(TOTAL_MDR_LENT).toFixed(3);
            var TOTAL_RRT = $(this).jqGrid('getCol', 'TOTAL_RR', false, 'sum');
            var TOTAL_RR_LENT = $(this).jqGrid('getCol', 'TOTAL_RR_LEN', false, 'sum');
            TOTAL_RR_LENT = parseFloat(TOTAL_RR_LENT).toFixed(3);
            var TOTAL_OTHERT = $(this).jqGrid('getCol', 'TOTAL_OTHER', false, 'sum');
            var TOTAL_OTHER_LENT = $(this).jqGrid('getCol', 'TOTAL_OTHER_LEN', false, 'sum');          
            TOTAL_OTHER_LENT = parseFloat(TOTAL_OTHER_LENT).toFixed(3);
            var TotalT = $(this).jqGrid('getCol', 'Total', false, 'sum');
            var Total_LENT = $(this).jqGrid('getCol', 'Total_LEN', false, 'sum');
            Total_LENT = parseFloat(Total_LENT).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_NH: TOTAL_NHT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_NH_LEN: TOTAL_NH_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_SH: TOTAL_SHT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_SH_LEN: TOTAL_SH_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_MDR: TOTAL_MDRT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_MDR_LEN: TOTAL_MDR_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_RR: TOTAL_RRT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_RR_LEN: TOTAL_RR_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_OTHER: TOTAL_OTHERT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_OTHER_LEN: TOTAL_OTHER_LENT }, true);
            $(this).jqGrid('footerData', 'set', { Total: TotalT }, true);
            $(this).jqGrid('footerData', 'set', { Total_LEN: Total_LENT }, true);
            $("#ERR1StateReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#ERR1StateReportTable_rn').html('Sr.<br/>No.');

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

    $("#ERR1StateReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'TOTAL_NH', numberOfColumns: 2, titleText: '<em>NH</em>' },
          { startColumnName: 'TOTAL_SH', numberOfColumns: 2, titleText: '<em>SH </em>' },
          { startColumnName: 'TOTAL_MDR', numberOfColumns: 2, titleText: '<em>MDR </em>' },
          { startColumnName: 'TOTAL_RR', numberOfColumns: 2, titleText: '<em>RR </em>' },
          { startColumnName: 'TOTAL_OTHER', numberOfColumns: 2, titleText: '<em>Total Other </em>' },
          { startColumnName: 'Total', numberOfColumns: 2, titleText: '<em>Total </em>' }

        ]
    });
}
/**/

/*       DISTRICT REPORT LISTING       */
function ERR1DistrictReportListing(stateCode, stateName) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR1StateReportTable").jqGrid('setSelection', stateCode);
    $("#ERR1StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR1BlockReportTable").jqGrid('GridUnload');
    $("#ERR1FinalReportTable").jqGrid('GridUnload');
    $("#ERR1DistrictReportTable").jqGrid('GridUnload');

    $("#ERR1DistrictReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR1DistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length'],
        colModel: [
            { name: 'DistrictName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_NH', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_NH_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_SH', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_SH_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_MDR', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_MDR_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_RR', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_RR_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_OTHER', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_OTHER_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Total', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Total_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } }

        ],

        pager: $("#ERR1DistrictReportPager"),
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
        caption: 'District  List of DRRP Roads for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_NHT = $(this).jqGrid('getCol', 'TOTAL_NH', false, 'sum');
            var TOTAL_NH_LENT = $(this).jqGrid('getCol', 'TOTAL_NH_LEN', false, 'sum');
            TOTAL_NH_LENT = parseFloat(TOTAL_NH_LENT).toFixed(3);
            var TOTAL_SHT = $(this).jqGrid('getCol', 'TOTAL_SH', false, 'sum');
            var TOTAL_SH_LENT = $(this).jqGrid('getCol', 'TOTAL_SH_LEN', false, 'sum');
            TOTAL_SH_LENT = parseFloat(TOTAL_SH_LENT).toFixed(3);
            var TOTAL_MDRT = $(this).jqGrid('getCol', 'TOTAL_MDR', false, 'sum');
            var TOTAL_MDR_LENT = $(this).jqGrid('getCol', 'TOTAL_MDR_LEN', false, 'sum');
            TOTAL_MDR_LENT = parseFloat(TOTAL_MDR_LENT).toFixed(3);
            var TOTAL_RRT = $(this).jqGrid('getCol', 'TOTAL_RR', false, 'sum');
            var TOTAL_RR_LENT = $(this).jqGrid('getCol', 'TOTAL_RR_LEN', false, 'sum');
            TOTAL_RR_LENT = parseFloat(TOTAL_RR_LENT).toFixed(3);
            var TOTAL_OTHERT = $(this).jqGrid('getCol', 'TOTAL_OTHER', false, 'sum');
            var TOTAL_OTHER_LENT = $(this).jqGrid('getCol', 'TOTAL_OTHER_LEN', false, 'sum');
            TOTAL_OTHER_LENT = parseFloat(TOTAL_OTHER_LENT).toFixed(3);
            var TotalT = $(this).jqGrid('getCol', 'Total', false, 'sum');
            var Total_LENT = $(this).jqGrid('getCol', 'Total_LEN', false, 'sum');
            Total_LENT = parseFloat(Total_LENT).toFixed(3);
            //

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_NH: TOTAL_NHT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_NH_LEN: TOTAL_NH_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_SH: TOTAL_SHT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_SH_LEN: TOTAL_SH_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_MDR: TOTAL_MDRT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_MDR_LEN: TOTAL_MDR_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_RR: TOTAL_RRT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_RR_LEN: TOTAL_RR_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_OTHER: TOTAL_OTHERT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_OTHER_LEN: TOTAL_OTHER_LENT }, true);
            $(this).jqGrid('footerData', 'set', { Total: TotalT }, true);
            $(this).jqGrid('footerData', 'set', { Total_LEN: Total_LENT }, true);
            $("#ERR1DistrictReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#ERR1DistrictReportTable_rn').html('Sr.<br/>No.');

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

    $("#ERR1DistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'TOTAL_NH', numberOfColumns: 2, titleText: '<em>NH</em>' },
          { startColumnName: 'TOTAL_SH', numberOfColumns: 2, titleText: '<em>SH </em>' },
          { startColumnName: 'TOTAL_MDR', numberOfColumns: 2, titleText: '<em>MDR </em>' },
          { startColumnName: 'TOTAL_RR', numberOfColumns: 2, titleText: '<em>RR </em>' },
          { startColumnName: 'TOTAL_OTHER', numberOfColumns: 2, titleText: '<em>Total Other </em>' },
          { startColumnName: 'Total', numberOfColumns: 2, titleText: '<em>Total </em>' }

        ]
    });
}
/**/

/*      BLOCK REPORT LISTING       */

function ERR1BlockReportListing(stateCode, districtCode, districtName) {

    var distname;
    if (districtName == '')
        distname = $("#DISTRICT_NAME").val();
    else
        distname = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR1DistrictReportTable").jqGrid('setSelection', districtCode);
    $("#ERR1DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR1StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR1BlockReportTable").jqGrid('GridUnload');
    $("#ERR1FinalReportTable").jqGrid('GridUnload');
    $("#ERR1BlockReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR1BlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length'],
        colModel: [
            { name: 'BlockName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_NH', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_NH_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_SH', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_SH_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_MDR', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_MDR_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_RR', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_RR_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_OTHER', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_OTHER_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Total', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Total_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } }

        ],
        pager: $("#ERR1BlockReportPager"),
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
        caption: 'Block  List of DRRP Roads for ' + distname,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_NHT = $(this).jqGrid('getCol', 'TOTAL_NH', false, 'sum');
            var TOTAL_NH_LENT = $(this).jqGrid('getCol', 'TOTAL_NH_LEN', false, 'sum');
            TOTAL_NH_LENT = parseFloat(TOTAL_NH_LENT).toFixed(3);
            var TOTAL_SHT = $(this).jqGrid('getCol', 'TOTAL_SH', false, 'sum');
            var TOTAL_SH_LENT = $(this).jqGrid('getCol', 'TOTAL_SH_LEN', false, 'sum');
            TOTAL_SH_LENT = parseFloat(TOTAL_SH_LENT).toFixed(3);
            var TOTAL_MDRT = $(this).jqGrid('getCol', 'TOTAL_MDR', false, 'sum');
            var TOTAL_MDR_LENT = $(this).jqGrid('getCol', 'TOTAL_MDR_LEN', false, 'sum');
            TOTAL_MDR_LENT = parseFloat(TOTAL_MDR_LENT).toFixed(3);
            var TOTAL_RRT = $(this).jqGrid('getCol', 'TOTAL_RR', false, 'sum');
            var TOTAL_RR_LENT = $(this).jqGrid('getCol', 'TOTAL_RR_LEN', false, 'sum');
            TOTAL_RR_LENT = parseFloat(TOTAL_RR_LENT).toFixed(3);
            var TOTAL_OTHERT = $(this).jqGrid('getCol', 'TOTAL_OTHER', false, 'sum');
            var TOTAL_OTHER_LENT = $(this).jqGrid('getCol', 'TOTAL_OTHER_LEN', false, 'sum');
            TOTAL_OTHER_LENT = parseFloat(TOTAL_OTHER_LENT).toFixed(3);
            var TotalT = $(this).jqGrid('getCol', 'Total', false, 'sum');
            var Total_LENT = $(this).jqGrid('getCol', 'Total_LEN', false, 'sum');
            Total_LENT = parseFloat(Total_LENT).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_NH: TOTAL_NHT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_NH_LEN: TOTAL_NH_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_SH: TOTAL_SHT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_SH_LEN: TOTAL_SH_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_MDR: TOTAL_MDRT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_MDR_LEN: TOTAL_MDR_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_RR: TOTAL_RRT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_RR_LEN: TOTAL_RR_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_OTHER: TOTAL_OTHERT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_OTHER_LEN: TOTAL_OTHER_LENT }, true);
            $(this).jqGrid('footerData', 'set', { Total: TotalT }, true);
            $(this).jqGrid('footerData', 'set', { Total_LEN: Total_LENT }, true);
            $("#ERR1BlockReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#ERR1BlockReportTable_rn').html('Sr.<br/>No.');

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

    $("#ERR1BlockReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'TOTAL_NH', numberOfColumns: 2, titleText: '<em>NH</em>' },
          { startColumnName: 'TOTAL_SH', numberOfColumns: 2, titleText: '<em>SH </em>' },
          { startColumnName: 'TOTAL_MDR', numberOfColumns: 2, titleText: '<em>MDR </em>' },
          { startColumnName: 'TOTAL_RR', numberOfColumns: 2, titleText: '<em>RR </em>' },
          { startColumnName: 'TOTAL_OTHER', numberOfColumns: 2, titleText: '<em>Total Other </em>' },
          { startColumnName: 'Total', numberOfColumns: 2, titleText: '<em>Total </em>' }

        ]
    });
}

/*       FINAL BLOCK REPORT LISTING       */

function ERR1FinalReportListing(blockCode, districtCode, stateCode, blockName) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR1BlockReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR1DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR1StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR1BlockReportTable").jqGrid('setSelection', blockCode);
    $("#ERR1FinalReportTable").jqGrid('GridUnload');

    $("#ERR1FinalReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR1FinalReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Road Code', 'Road Name', 'Road Category', 'Road Type', 'Length (Kms.)', 'Year of Construction', 'Included in Core Network (Y/N)', 'Habitations Status (Y/N)', 'Habitations Name', 'Population', 'Soil type', 'Terrain Type'],
        colModel: [
            { name: 'PlannedRoadNumber', width: 150, align: 'left', height: 'auto', sortable: true },
            { name: 'PlannedRoadName', width: 250, align: 'Left', height: 'auto', sortable: false },
            { name: 'MAST_ROAD_CAT_CODE', width: 80, align: 'left', height: 'auto', sortable: false },
            { name: 'MAST_ER_ROAD_TYPE', width: 150, align: 'Left', height: 'auto', sortable: false },
            { name: 'ROAD_LENGTH', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MAST_CONS_YEAR', width: 120, align: 'center', height: 'auto', sortable: false },
            { name: 'MAST_CORE_NETWORK', width: 120, align: 'center', height: 'auto', sortable: false },
            { name: 'MAST_HAB_STATUS', width: 120, align: 'center', height: 'auto', sortable: false },
            { name: 'MAST_HAB_NAME', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'MAST_HAB_TOT_POP', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MAST_SOIL_TYPE_NAME', width: 250, align: 'left', height: 'auto', sortable: false },
            { name: 'MAST_TERRAIN_TYPE_NAME', width: 250, align: 'left', height: 'auto', sortable: false }

        ],

        postData: { "BlockCode": blockCode, "StateCode": stateCode, "DistrictCode": districtCode },
        rowNum: '2147483647',
        pager: $("#ERR1FinalReportPager"),
        footerrow: true,
        pgbuttons: true,
        sortname: 'PlannedRoadNumber',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '450',
        viewrecords: true,
        caption: 'List of DRRP Roads for ' + blockName,
        loadComplete: function () {
            //Total of Columns
            var ROAD_LENGTHT = $(this).jqGrid('getCol', 'ROAD_LENGTH', false, 'sum');
            ROAD_LENGTHT = parseFloat(ROAD_LENGTHT).toFixed(3);
            var NoHablitisationT = $(this).jqGrid('getCol', 'MAST_HAB_TOT_POP', false, 'sum');


            $(this).jqGrid('footerData', 'set', { PlannedRoadNumber: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { ROAD_LENGTH: ROAD_LENGTHT });
            $(this).jqGrid('footerData', 'set', { MAST_HAB_TOT_POP: NoHablitisationT });
            $('#ERR1FinalReportTable_rn').html('Sr.<br/>No.');


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
    }); //End of Jq Grid


}