$(document).ready(function () {
   
   
    $('#ddRoadType_ERR6').change(function () {
        var roadType = $('#ddRoadType_ERR6').val();
        loadLevelWiseGrid(roadType);
    });

    $('#ddRoadType_ERR6').trigger('change');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

});

function loadLevelWiseGrid(roadType) {
    if ($("#hdnLevelId").val() == 6) //mord
    {
            ERR6StateReportListing(roadType);
    }
    else if ($("#hdnLevelId").val() == 4) //state
    {
       ERR6DistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), roadType);
    }
    else if ($("#hdnLevelId").val() == 5) //District
    {
        ERR6BlockReportListing($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val(), $("#BLOCK_NAME").val(), roadType);
    }

}
/*       STATE REPORT LISTING       */
function ERR6StateReportListing(roadType) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR6StateReportTable").jqGrid('GridUnload');
    $("#ERR6DistrictReportTable").jqGrid('GridUnload');
    $("#ERR6BlockReportTable").jqGrid('GridUnload');
    $("#ERR6FinalReportTable").jqGrid('GridUnload');
    $("#ERR6StateReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR6StateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length'],
        colModel: [
            { name: 'StateName', width: 300, align: 'left',  height: 'auto', sortable: true },
            { name: 'ROAD_ALL', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_ALL_LEN', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_CALL', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_CALL_LEN', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_FAIR', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: {thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_FAIR_LEN', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_CFAIR', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_CFAIR_LEN', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_TOTAL', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_TOTAL_LEN', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_CTOTAL', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: {thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_CTOTAL_LEN', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } }

        ],
        postData: {  "RoadType": roadType },
        pager: $("#ERR6StateReportPager"),
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'StateName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '520',
        viewrecords: true,
        caption: 'State DRRP based on road type Details',
        loadComplete: function () {
            //Total of Columns
            var ROAD_ALLT = $(this).jqGrid('getCol', 'ROAD_ALL', false, 'sum');
            var ROAD_ALL_LENT = $(this).jqGrid('getCol', 'ROAD_ALL_LEN', false, 'sum');
            ROAD_ALL_LENT = parseFloat(ROAD_ALL_LENT).toFixed(3);
            var ROAD_CALLT = $(this).jqGrid('getCol', 'ROAD_CALL', false, 'sum');
            var ROAD_CALL_LENT = $(this).jqGrid('getCol', 'ROAD_CALL_LEN', false, 'sum');
            ROAD_CALL_LENT = parseFloat(ROAD_ALL_LENT).toFixed(3);
            var ROAD_FAIRT = $(this).jqGrid('getCol', 'ROAD_FAIR', false, 'sum');
            var ROAD_FAIR_LENT = $(this).jqGrid('getCol', 'ROAD_FAIR_LEN', false, 'sum');
            ROAD_FAIR_LENT = parseFloat(ROAD_FAIR_LENT).toFixed(3);
            var ROAD_CFAIRT = $(this).jqGrid('getCol', 'ROAD_CFAIR', false, 'sum');
            var ROAD_CFAIR_LENT = $(this).jqGrid('getCol', 'ROAD_CFAIR_LEN', false, 'sum');
            ROAD_CFAIR_LENT = parseFloat(ROAD_CFAIR_LENT).toFixed(3);
            var ROAD_TOTALT = $(this).jqGrid('getCol', 'ROAD_TOTAL', false, 'sum');
            var ROAD_TOTAL_LENT = $(this).jqGrid('getCol', 'ROAD_TOTAL_LEN', false, 'sum');
            ROAD_TOTAL_LENT = parseFloat(ROAD_TOTAL_LENT).toFixed(3);
            var ROAD_CTOTALT = $(this).jqGrid('getCol', 'ROAD_CTOTAL', false, 'sum');
            var ROAD_CTOTAL_LENT = $(this).jqGrid('getCol', 'ROAD_CTOTAL_LEN', false, 'sum');
            ROAD_CTOTAL_LENT = parseFloat(ROAD_CTOTAL_LENT).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { ROAD_ALL: ROAD_ALLT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_ALL_LEN: ROAD_ALL_LENT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_CALL: ROAD_CALLT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_CALL_LEN: ROAD_CALL_LENT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_FAIR: ROAD_FAIRT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_FAIR_LEN: ROAD_FAIR_LENT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_CFAIR: ROAD_CFAIRT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_CFAIR_LEN: ROAD_CFAIR_LENT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_TOTAL: ROAD_TOTALT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_TOTAL_LEN: ROAD_TOTAL_LENT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_CTOTAL: ROAD_CTOTALT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_CTOTAL_LEN: ROAD_CTOTAL_LENT }, true);
            $("#ERR6StateReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#ERR6StateReportTable_rn').html('Sr.<br/>No.');

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

    $("#ERR6StateReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          //{ startColumnName: 'ROAD_ALL', numberOfColumns: 2, titleText: '<em>All Weather DRRP</em>' },
          //{ startColumnName: 'ROAD_CALL', numberOfColumns: 2, titleText: '<em>All Weather Included in CN </em>' },
          //{ startColumnName: 'ROAD_FAIR', numberOfColumns: 2, titleText: '<em>Fair Weather DRRP </em>' },
          //{ startColumnName: 'ROAD_CFAIR', numberOfColumns: 2, titleText: '<em>Fair Weather Included in CN </em>' },
          //{ startColumnName: 'ROAD_TOTAL', numberOfColumns: 2, titleText: '<em>Total DRRP </em>' },
          //{ startColumnName: 'ROAD_CTOTAL', numberOfColumns: 2, titleText: '<em>Total Included in CN </em>' }
             {
                 startColumnName: 'ROAD_ALL', numberOfColumns: 4,
                 titleText: '<table style="width:100%;border-spacing:0px"' +
                         '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">All Weather</td>  </tr>' +
                         '<tr>' +
                             '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                             '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                         '</tr>' +
                         '</table>'
             },

            {
                startColumnName: 'ROAD_FAIR', numberOfColumns: 4,
                titleText: '<table style="width:100%;border-spacing:0px"' +
                        '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Fair Weather</td>  </tr>' +
                        '<tr>' +
                            '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                            '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                        '</tr>' +
                        '</table>'
            },

              {
                  startColumnName: 'ROAD_TOTAL', numberOfColumns: 4,
                  titleText: '<table style="width:100%;border-spacing:0px"' +
                          '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Total </td>  </tr>' +
                          '<tr>' +
                              '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                              '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                          '</tr>' +
                          '</table>'
              },
        ]
    });
}
/**/

/*       DISTRICT REPORT LISTING       */
function ERR6DistrictReportListing(stateCode, stateName, roadType) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR6StateReportTable").jqGrid('setSelection', stateCode);
    $("#ERR6StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR6DistrictReportTable").jqGrid('GridUnload');
    $("#ERR6BlockReportTable").jqGrid('GridUnload');
    $("#ERR6FinalReportTable").jqGrid('GridUnload');


    $("#ERR6DistrictReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR6DistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length'],
        colModel: [
            { name: 'DistrictName', width: 300, align: 'left',  height: 'auto', sortable: true },
            { name: 'ROAD_ALL', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_ALL_LEN', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_CALL', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_CALL_LEN', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_FAIR', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_FAIR_LEN', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_CFAIR', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_CFAIR_LEN', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_TOTAL', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_TOTAL_LEN', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_CTOTAL', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_CTOTAL_LEN', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } }

        ],
        pager: $("#ERR6DistrictReportPager"),
        postData: { 'StateCode': stateCode, "RoadType": roadType },
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'DistrictName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '480',
        viewrecords: true,
        caption: 'District DRRP based on road type Details for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var ROAD_ALLT = $(this).jqGrid('getCol', 'ROAD_ALL', false, 'sum');
            var ROAD_ALL_LENT = $(this).jqGrid('getCol', 'ROAD_ALL_LEN', false, 'sum');
            ROAD_ALL_LENT = parseFloat(ROAD_ALL_LENT).toFixed(3);
            var ROAD_CALLT = $(this).jqGrid('getCol', 'ROAD_CALL', false, 'sum');
            var ROAD_CALL_LENT = $(this).jqGrid('getCol', 'ROAD_CALL_LEN', false, 'sum');
            ROAD_CALL_LENT = parseFloat(ROAD_ALL_LENT).toFixed(3);
            var ROAD_FAIRT = $(this).jqGrid('getCol', 'ROAD_FAIR', false, 'sum');
            var ROAD_FAIR_LENT = $(this).jqGrid('getCol', 'ROAD_FAIR_LEN', false, 'sum');
            ROAD_FAIR_LENT = parseFloat(ROAD_FAIR_LENT).toFixed(3);
            var ROAD_CFAIRT = $(this).jqGrid('getCol', 'ROAD_CFAIR', false, 'sum');
            var ROAD_CFAIR_LENT = $(this).jqGrid('getCol', 'ROAD_CFAIR_LEN', false, 'sum');
            ROAD_CFAIR_LENT = parseFloat(ROAD_CFAIR_LENT).toFixed(3);
            var ROAD_TOTALT = $(this).jqGrid('getCol', 'ROAD_TOTAL', false, 'sum');
            var ROAD_TOTAL_LENT = $(this).jqGrid('getCol', 'ROAD_TOTAL_LEN', false, 'sum');
            ROAD_TOTAL_LENT = parseFloat(ROAD_TOTAL_LENT).toFixed(3);
            var ROAD_CTOTALT = $(this).jqGrid('getCol', 'ROAD_CTOTAL', false, 'sum');
            var ROAD_CTOTAL_LENT = $(this).jqGrid('getCol', 'ROAD_CTOTAL_LEN', false, 'sum');
            ROAD_CTOTAL_LENT = parseFloat(ROAD_CTOTAL_LENT).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { ROAD_ALL: ROAD_ALLT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_ALL_LEN: ROAD_ALL_LENT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_CALL: ROAD_CALLT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_CALL_LEN: ROAD_CALL_LENT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_FAIR: ROAD_FAIRT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_FAIR_LEN: ROAD_FAIR_LENT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_CFAIR: ROAD_CFAIRT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_CFAIR_LEN: ROAD_CFAIR_LENT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_TOTAL: ROAD_TOTALT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_TOTAL_LEN: ROAD_TOTAL_LENT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_CTOTAL: ROAD_CTOTALT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_CTOTAL_LEN: ROAD_CTOTAL_LENT }, true);
            $("#ERR6DistrictReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#ERR6DistrictReportTable_rn').html('Sr.<br/>No.');

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

    $("#ERR6DistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
         //{ startColumnName: 'ROAD_ALL', numberOfColumns: 2, titleText: '<em>All Weather DRRP</em>' },
         //{ startColumnName: 'ROAD_CALL', numberOfColumns: 2, titleText: '<em>All Weather Included in CN </em>' },
         //{ startColumnName: 'ROAD_FAIR', numberOfColumns: 2, titleText: '<em>Fair Weather DRRP </em>' },
         //{ startColumnName: 'ROAD_CFAIR', numberOfColumns: 2, titleText: '<em>Fair Weather Included in CN </em>' },
         //{ startColumnName: 'ROAD_TOTAL', numberOfColumns: 2, titleText: '<em>Total DRRP </em>' },
         //{ startColumnName: 'ROAD_CTOTAL', numberOfColumns: 2, titleText: '<em>Total Included in CN </em>' }
            {
                startColumnName: 'ROAD_ALL', numberOfColumns: 4,
                titleText: '<table style="width:100%;border-spacing:0px"' +
                        '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">All Weather</td>  </tr>' +
                        '<tr>' +
                            '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                            '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                        '</tr>' +
                        '</table>'
            },

           {
               startColumnName: 'ROAD_FAIR', numberOfColumns: 4,
               titleText: '<table style="width:100%;border-spacing:0px"' +
                       '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Fair Weather</td>  </tr>' +
                       '<tr>' +
                           '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                           '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                       '</tr>' +
                       '</table>'
           },

             {
                 startColumnName: 'ROAD_TOTAL', numberOfColumns: 4,
                 titleText: '<table style="width:100%;border-spacing:0px"' +
                         '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Total </td>  </tr>' +
                         '<tr>' +
                             '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                             '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                         '</tr>' +
                         '</table>'
             },
        ]
    });
}
/**/

/*      BLOCK REPORT LISTING       */

function ERR6BlockReportListing(stateCode, districtCode, districtName, roadType) {

    var distname;
    if (districtName == '')
        distname = $("#DISTRICT_NAME").val();
    else
        distname = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR6DistrictReportTable").jqGrid('setSelection', districtCode);
    $("#ERR6DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR6StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR6BlockReportTable").jqGrid('GridUnload');
    $("#ERR6FinalReportTable").jqGrid('GridUnload');

    $("#ERR6BlockReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR6BlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length'],
        colModel: [
            { name: 'BlockName', width: 300, align: 'left',  height: 'auto', sortable: true },
              { name: 'ROAD_ALL', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_ALL_LEN', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_CALL', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_CALL_LEN', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_FAIR', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_FAIR_LEN', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_CFAIR', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_CFAIR_LEN', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_TOTAL', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_TOTAL_LEN', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_CTOTAL', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ROAD_CTOTAL_LEN', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } }
        ],
        pager: $("#ERR6BlockReportPager"),
        postData: { 'StateCode': stateCode, 'DistrictCode': districtCode, "RoadType": roadType },
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'BlockName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '420',
        viewrecords: true,
        caption: 'Block DRRP based on road type Details for ' + distname,
        loadComplete: function () {
            //Total of Columns
            var ROAD_ALLT = $(this).jqGrid('getCol', 'ROAD_ALL', false, 'sum');
            var ROAD_ALL_LENT = $(this).jqGrid('getCol', 'ROAD_ALL_LEN', false, 'sum');
            ROAD_ALL_LENT = parseFloat(ROAD_ALL_LENT).toFixed(3);
            var ROAD_CALLT = $(this).jqGrid('getCol', 'ROAD_CALL', false, 'sum');
            var ROAD_CALL_LENT = $(this).jqGrid('getCol', 'ROAD_CALL_LEN', false, 'sum');
            ROAD_CALL_LENT = parseFloat(ROAD_ALL_LENT).toFixed(3);
            var ROAD_FAIRT = $(this).jqGrid('getCol', 'ROAD_FAIR', false, 'sum');
            var ROAD_FAIR_LENT = $(this).jqGrid('getCol', 'ROAD_FAIR_LEN', false, 'sum');
            ROAD_FAIR_LENT = parseFloat(ROAD_FAIR_LENT).toFixed(3);
            var ROAD_CFAIRT = $(this).jqGrid('getCol', 'ROAD_CFAIR', false, 'sum');
            var ROAD_CFAIR_LENT = $(this).jqGrid('getCol', 'ROAD_CFAIR_LEN', false, 'sum');
            ROAD_CFAIR_LENT = parseFloat(ROAD_CFAIR_LENT).toFixed(3);
            var ROAD_TOTALT = $(this).jqGrid('getCol', 'ROAD_TOTAL', false, 'sum');
            var ROAD_TOTAL_LENT = $(this).jqGrid('getCol', 'ROAD_TOTAL_LEN', false, 'sum');
            ROAD_TOTAL_LENT = parseFloat(ROAD_TOTAL_LENT).toFixed(3);
            var ROAD_CTOTALT = $(this).jqGrid('getCol', 'ROAD_CTOTAL', false, 'sum');
            var ROAD_CTOTAL_LENT = $(this).jqGrid('getCol', 'ROAD_CTOTAL_LEN', false, 'sum');
            ROAD_CTOTAL_LENT = parseFloat(ROAD_CTOTAL_LENT).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { ROAD_ALL: ROAD_ALLT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_ALL_LEN: ROAD_ALL_LENT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_CALL: ROAD_CALLT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_CALL_LEN: ROAD_CALL_LENT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_FAIR: ROAD_FAIRT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_FAIR_LEN: ROAD_FAIR_LENT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_CFAIR: ROAD_CFAIRT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_CFAIR_LEN: ROAD_CFAIR_LENT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_TOTAL: ROAD_TOTALT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_TOTAL_LEN: ROAD_TOTAL_LENT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_CTOTAL: ROAD_CTOTALT }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_CTOTAL_LEN: ROAD_CTOTAL_LENT }, true);
            $("#ERR6BlockReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#ERR6BlockReportTable_rn').html('Sr.<br/>No.');

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

    $("#ERR6BlockReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          //{ startColumnName: 'ROAD_ALL', numberOfColumns: 2, titleText: '<em>All Weather DRRP</em>' },
          //{ startColumnName: 'ROAD_CALL', numberOfColumns: 2, titleText: '<em>All Weather Included in CN </em>' },
          //{ startColumnName: 'ROAD_FAIR', numberOfColumns: 2, titleText: '<em>Fair Weather DRRP </em>' },
          //{ startColumnName: 'ROAD_CFAIR', numberOfColumns: 2, titleText: '<em>Fair Weather Included in CN </em>' },
          //{ startColumnName: 'ROAD_TOTAL', numberOfColumns: 2, titleText: '<em>Total DRRP </em>' },
          //{ startColumnName: 'ROAD_CTOTAL', numberOfColumns: 2, titleText: '<em>Total Included in CN </em>' }
             {
                 startColumnName: 'ROAD_ALL', numberOfColumns: 4,
                 titleText: '<table style="width:100%;border-spacing:0px"' +
                         '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">All Weather</td>  </tr>' +
                         '<tr>' +
                             '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                             '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                         '</tr>' +
                         '</table>'
             },

            {
                startColumnName: 'ROAD_FAIR', numberOfColumns: 4,
                titleText: '<table style="width:100%;border-spacing:0px"' +
                        '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Fair Weather</td>  </tr>' +
                        '<tr>' +
                            '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                            '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                        '</tr>' +
                        '</table>'
            },

              {
                  startColumnName: 'ROAD_TOTAL', numberOfColumns: 4,
                  titleText: '<table style="width:100%;border-spacing:0px"' +
                          '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Total </td>  </tr>' +
                          '<tr>' +
                              '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                              '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                          '</tr>' +
                          '</table>'
              },
        ]
    });
}

/*       FINAL BLOCK REPORT LISTING       */

function ERR6FinalReportListing(blockCode, districtCode, stateCode, blockName, roadType) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR6BlockReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR6DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR6StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR6BlockReportTable").jqGrid('setSelection', blockCode);
    $("#ERR6FinalReportTable").jqGrid('GridUnload');

    $("#ERR6FinalReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR6FinalReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Road Code', 'Road Name', 'Road Category', 'Road Type', 'Length (Kms.)', 'Year of Construction', 'Included in Core Network (Y/N)', 'Habitations Status (Y/N)', 'Habitation Name', 'Population', 'Soil type', 'Terrain Type'],
        colModel: [
            { name: 'PlannedRoadNumber', width: 150, align: 'left',  height: 'auto', sortable: true },
            { name: 'PlannedRoadName', width: 250, align: 'left',  height: 'auto', sortable: false },
            { name: 'MAST_ROAD_CAT_CODE', width: 120, align: 'left',  height: 'auto', sortable: false },
            { name: 'MAST_ER_ROAD_TYPE', width: 150, align: 'left',  height: 'auto', sortable: false },
            { name: 'ROAD_LENGTH', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MAST_CONS_YEAR', width: 120, align: 'center',  height: 'auto', sortable: false },
            { name: 'MAST_CORE_NETWORK', width: 120, align: 'center',  height: 'auto', sortable: false },
            { name: 'MAST_HAB_STATUS', width: 120, align: 'center',  height: 'auto', sortable: false },
            { name: 'MAST_HAB_NAME', width: 120, align: 'left',  height: 'auto', sortable: false },
            { name: 'MAST_HAB_TOT_POP', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MAST_SOIL_TYPE_NAME', width: 150, align: 'left',  height: 'auto', sortable: false },
            { name: 'MAST_TERRAIN_TYPE_NAME', width: 150, align: 'left',  height: 'auto', sortable: false }
         
        ],

        postData: { "BlockCode": blockCode, "StateCode": stateCode, "DistrictCode": districtCode, "RoadType": roadType },
        rowNum: '2147483647',
        pager: $("#ERR6FinalReportPager"),
        footerrow: true,
        pgbuttons: true,
        sortname: 'PlannedRoadNumber',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '420',
        viewrecords: true,
        caption: 'DRRP based on road type Details for ' + blockName,
        loadComplete: function () {
            //Total of Columns
            var ROAD_LENGTHT = $(this).jqGrid('getCol', 'ROAD_LENGTH', false, 'sum');
            ROAD_LENGTHT = parseFloat(ROAD_LENGTHT).toFixed(3);
            var NoHablitisationT = $(this).jqGrid('getCol', 'MAST_HAB_TOT_POP', false, 'sum');
      

            $(this).jqGrid('footerData', 'set', { PlannedRoadNumber: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { ROAD_LENGTH: ROAD_LENGTHT });
            $(this).jqGrid('footerData', 'set', { MAST_HAB_TOT_POP: NoHablitisationT });
            $('#ERR6FinalReportTable_rn').html('Sr.<br/>No.');

         

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