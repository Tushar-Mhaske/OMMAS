var firstFinYear;
var secondFinYear;
var thirdFinYear
$(function () {
    var route = $("#Route_PCIAbstractDetails").val();
    var dateVar = new Date();
    var month = dateVar.getMonth();
    var year = dateVar.getFullYear();

    if (month > 3) {
        year = year + 1;
    }
     firstFinYear = parseInt(year - 3) + "-" + parseInt(year - 2);
     secondFinYear = parseInt(year - 2) + "-" + parseInt(year - 1);
     thirdFinYear = parseInt(year - 1) + "-" + parseInt(year);

    $("#btPCIAbstractDetails").click(function () {

      
        if ($("#hdnLevelId").val() == 6) //mord
        {
        
            PCIAbstractStateReportListing(route);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
           
            PCIAbstractDistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), route);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
           
            PCIAbstractBlockReportListing($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), route);
        }
    });   
    $("#btPCIAbstractDetails").trigger("click");
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});


/*       STATE REPORT LISTING       */
function PCIAbstractStateReportListing(route) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#PCIAbstractStateReportTable").jqGrid('GridUnload');
    $("#PCIAbstractDistrictReportTable").jqGrid('GridUnload');
    $("#PCIAbstractBlockReportTable").jqGrid('GridUnload');
    $("#PCIAbstractFinalReportTable").jqGrid('GridUnload');
    $("#PCIAbstractStateReportTable").jqGrid({
        url: '/CoreNetworkReports/PCIAbstractStateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Number of Rural Route', 'Total Length (in Kms)', 'Number of Roads', 'Total Length (in Kms)', '1', '2', '3', '4', '5', firstFinYear, secondFinYear, thirdFinYear],
        colModel: [
            { name: 'MAST_STATE_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN1', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN2', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN3', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN4', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN5', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LY3', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LY2', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LY1', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } }
        ],
        postData: { "Route": route },
        pager: $("#PCIAbstractStateReportPager"),
        footerrow: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '520',
        viewrecords: true,
        caption: 'State PCI Abstract Details',
        loadComplete: function () {
            //Total of Columns
            var TOTAL_CN_T = $(this).jqGrid('getCol', 'TOTAL_CN', false, 'sum');
            var TOTAL_LEN_T = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            var TOTAL_PCI_T = $(this).jqGrid('getCol', 'TOTAL_PCI', false, 'sum');
            var TOTAL_PCI_LEN_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN', false, 'sum');
            TOTAL_PCI_LEN_T = parseFloat(TOTAL_PCI_LEN_T).toFixed(3);
            var TOTAL_PCI_LEN1_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN1', false, 'sum');
            TOTAL_PCI_LEN1_T = parseFloat(TOTAL_PCI_LEN1_T).toFixed(3);
            var TOTAL_PCI_LEN2_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN2', false, 'sum');
            TOTAL_PCI_LEN2_T = parseFloat(TOTAL_PCI_LEN2_T).toFixed(3);
            var TOTAL_PCI_LEN3_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN3', false, 'sum');
            TOTAL_PCI_LEN3_T = parseFloat(TOTAL_PCI_LEN3_T).toFixed(3);
            var TOTAL_PCI_LEN4_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN4', false, 'sum');
            TOTAL_PCI_LEN4_T = parseFloat(TOTAL_PCI_LEN4_T).toFixed(3);
            var TOTAL_PCI_LEN5_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN5', false, 'sum');
            TOTAL_PCI_LEN5_T = parseFloat(TOTAL_PCI_LEN5_T).toFixed(3);
            var TOTAL_PCI_LY1_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LY1', false, 'sum');
            TOTAL_PCI_LY1_T = parseFloat(TOTAL_PCI_LY1_T).toFixed(3);
            var TOTAL_PCI_LY2_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LY2', false, 'sum');
            TOTAL_PCI_LY2_T = parseFloat(TOTAL_PCI_LY2_T).toFixed(3);
            var TOTAL_PCI_LY3_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LY3', false, 'sum');
            TOTAL_PCI_LY3_T = parseFloat(TOTAL_PCI_LY3_T).toFixed(3);
            //

            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_CN: TOTAL_CN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI: TOTAL_PCI_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN: TOTAL_PCI_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN1: TOTAL_PCI_LEN1_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN2: TOTAL_PCI_LEN2_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN3: TOTAL_PCI_LEN3_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN4: TOTAL_PCI_LEN4_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN5: TOTAL_PCI_LEN5_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LY1: TOTAL_PCI_LY1_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LY2: TOTAL_PCI_LY2_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LY3: TOTAL_PCI_LY3_T }, true);
            $('#PCIAbstractStateReportTable_rn').html('Sr.<br/>No.');
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

    $("#PCIAbstractStateReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
         // { startColumnName: 'TOTAL_CN', numberOfColumns: 12, titleText: '<em>Total</em>' },
           {
               startColumnName: 'MAST_STATE_NAME', numberOfColumns: 3,
               titleText: '<table style="width:100%;border-spacing:0px"' +
                       '<tr><td id="h0"  style="border-bottom-width: 1px;" colspan="3">Target for PCI data to Enter </td>  </tr>' +
                       '<tr>' +
                           '<td id="h1" colspan="3" style="width: 22%; border-right-width: 1px; border-right-color: inherit;"></td>' +
                       '</tr>' +
                       '</table>'
           },
             {
                 startColumnName: 'TOTAL_PCI', numberOfColumns: 10,
                 titleText: '<table style="width:100%;border-spacing:0px"' +
                         '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="10">PCI Data Entered</td>  </tr>' +
                         '<tr>' +
                             '<td id="h1" colspan="2" style="width: 14%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">PCI data Entered For</td>' +
                               '<td id="h1" colspan="5" style="width: 36%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Length(Kms) with PCI Value</td>' +
                               '<td id="h2" colspan="3" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Year Wise Length (Kms)</td>' +
                         '</tr>' +
                         '</table>'
           }

        ]
    });

}
/**/

/*       DISTRICT REPORT LISTING       */
function PCIAbstractDistrictReportListing(stateCode, stateName, route) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#PCIAbstractStateReportTable").jqGrid('setGridState', 'hidden');
    $("#PCIAbstractStateReportTable").jqGrid('setSelection', stateCode);
    $("#PCIAbstractDistrictReportTable").jqGrid('GridUnload');
    $("#PCIAbstractBlockReportTable").jqGrid('GridUnload');
    $("#PCIAbstractFinalReportTable").jqGrid('GridUnload');

    $("#PCIAbstractDistrictReportTable").jqGrid({
        url: '/CoreNetworkReports/PCIAbstractDistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Number of Rural Route', 'Total Length (in Kms)', 'Number of Roads', 'Total Length (in Kms)', '1', '2', '3', '4', '5', firstFinYear, secondFinYear, thirdFinYear],
        colModel: [
            { name: 'MAST_DISTRICT_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN1', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN2', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN3', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN4', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN5', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LY3', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LY2', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LY1', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } }
        ],
        postData: { "StateCode": stateCode, "Route": route },
        pager: $("#PCIAbstractDistrictReportPager"),
        footerrow: true,
        sortname: 'MAST_DISTRICT_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '480',
        viewrecords: true,
        caption: 'District PCI Abstract Details for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_CN_T = $(this).jqGrid('getCol', 'TOTAL_CN', false, 'sum');
            var TOTAL_LEN_T = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            var TOTAL_PCI_T = $(this).jqGrid('getCol', 'TOTAL_PCI', false, 'sum');
            var TOTAL_PCI_LEN_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN', false, 'sum');
            TOTAL_PCI_LEN_T = parseFloat(TOTAL_PCI_LEN_T).toFixed(3);
            var TOTAL_PCI_LEN1_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN1', false, 'sum');
            TOTAL_PCI_LEN1_T = parseFloat(TOTAL_PCI_LEN1_T).toFixed(3);
            var TOTAL_PCI_LEN2_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN2', false, 'sum');
            TOTAL_PCI_LEN2_T = parseFloat(TOTAL_PCI_LEN2_T).toFixed(3);
            var TOTAL_PCI_LEN3_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN3', false, 'sum');
            TOTAL_PCI_LEN3_T = parseFloat(TOTAL_PCI_LEN3_T).toFixed(3);
            var TOTAL_PCI_LEN4_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN4', false, 'sum');
            TOTAL_PCI_LEN4_T = parseFloat(TOTAL_PCI_LEN4_T).toFixed(3);
            var TOTAL_PCI_LEN5_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN5', false, 'sum');
            TOTAL_PCI_LEN5_T = parseFloat(TOTAL_PCI_LEN5_T).toFixed(3);
            var TOTAL_PCI_LY1_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LY1', false, 'sum');
            TOTAL_PCI_LY1_T = parseFloat(TOTAL_PCI_LY1_T).toFixed(3);
            var TOTAL_PCI_LY2_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LY2', false, 'sum');
            TOTAL_PCI_LY2_T = parseFloat(TOTAL_PCI_LY2_T).toFixed(3);
            var TOTAL_PCI_LY3_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LY3', false, 'sum');
            TOTAL_PCI_LY3_T = parseFloat(TOTAL_PCI_LY3_T).toFixed(3);
            //

            $(this).jqGrid('footerData', 'set', { MAST_DISTRICT_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_CN: TOTAL_CN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI: TOTAL_PCI_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN: TOTAL_PCI_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN1: TOTAL_PCI_LEN1_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN2: TOTAL_PCI_LEN2_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN3: TOTAL_PCI_LEN3_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN4: TOTAL_PCI_LEN4_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN5: TOTAL_PCI_LEN5_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LY1: TOTAL_PCI_LY1_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LY2: TOTAL_PCI_LY2_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LY3: TOTAL_PCI_LY3_T }, true);
            $('#PCIAbstractDistrictReportTable_rn').html('Sr.<br/>No.');
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

    $("#PCIAbstractDistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          //{ startColumnName: 'TOTAL_PCI_LEN1', numberOfColumns: 5, titleText: '<em>Total</em>' },
            {
                startColumnName: 'MAST_DISTRICT_NAME', numberOfColumns: 3,
                titleText: '<table style="width:100%;border-spacing:0px"' +
                        '<tr><td id="h0"  style="border-bottom-width: 1px;" colspan="3">Target for PCI data to Enter </td>  </tr>' +
                        '<tr>' +
                            '<td id="h1" colspan="3" style="width: 22%; border-right-width: 1px; border-right-color: inherit;"></td>' +
                        '</tr>' +
                        '</table>'
            },
             {
                 startColumnName: 'TOTAL_PCI', numberOfColumns: 10,
                 titleText: '<table style="width:100%;border-spacing:0px"' +
                         '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="10">PCI Data Entered</td>  </tr>' +
                         '<tr>' +
                             '<td id="h1" colspan="2" style="width: 14%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">PCI Data Entered For</td>' +
                               '<td id="h1" colspan="5" style="width: 36%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Length (Kms) with PCI Value</td>' +
                               '<td id="h2" colspan="3" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Yearwise Length (Kms)</td>' +
                         '</tr>' +
                         '</table>'
             }

        ]
    });
}
/**/

/*       BLOCK REPORT LISTING       */
function PCIAbstractBlockReportListing(districtCode, stateCode, districtName, route) {

    var distname;
    if (districtName == '')
        distname = $("#DISTRICT_NAME").val();
    else
        distname = districtName;

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#PCIAbstractDistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#PCIAbstractStateReportTable").jqGrid('setGridState', 'hidden');
    $("#PCIAbstractDistrictReportTable").jqGrid('setSelection', stateCode);
    $("#PCIAbstractBlockReportTable").jqGrid('GridUnload');
    $("#PCIAbstractFinalReportTable").jqGrid('GridUnload');

    $("#PCIAbstractBlockReportTable").jqGrid({
        url: '/CoreNetworkReports/PCIAbstractBlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Number of Rural Route', 'Total Length (in Kms)', 'Number of Roads', 'Total Length (in Kms)', '1', '2', '3', '4', '5', firstFinYear, secondFinYear, thirdFinYear],
        colModel: [
            { name: 'MAST_BLOCK_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN1', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN2', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN3', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN4', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN5', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LY3', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LY2', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LY1', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } }
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "Route": route },
        pager: $("#PCIAbstractBlockReportPager"),
        footerrow: true,
        sortname: 'MAST_BLOCK_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '440',
        viewrecords: true,
        caption: 'Block PCI Abstract Details for ' + distname,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_CN_T = $(this).jqGrid('getCol', 'TOTAL_CN', false, 'sum');
            var TOTAL_LEN_T = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            var TOTAL_PCI_T = $(this).jqGrid('getCol', 'TOTAL_PCI', false, 'sum');
            var TOTAL_PCI_LEN_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN', false, 'sum');
            TOTAL_PCI_LEN_T = parseFloat(TOTAL_PCI_LEN_T).toFixed(3);
            var TOTAL_PCI_LEN1_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN1', false, 'sum');
            TOTAL_PCI_LEN1_T = parseFloat(TOTAL_PCI_LEN1_T).toFixed(3);
            var TOTAL_PCI_LEN2_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN2', false, 'sum');
            TOTAL_PCI_LEN2_T = parseFloat(TOTAL_PCI_LEN2_T).toFixed(3);
            var TOTAL_PCI_LEN3_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN3', false, 'sum');
            TOTAL_PCI_LEN3_T = parseFloat(TOTAL_PCI_LEN3_T).toFixed(3);
            var TOTAL_PCI_LEN4_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN4', false, 'sum');
            TOTAL_PCI_LEN4_T = parseFloat(TOTAL_PCI_LEN4_T).toFixed(3);
            var TOTAL_PCI_LEN5_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN5', false, 'sum');
            TOTAL_PCI_LEN5_T = parseFloat(TOTAL_PCI_LEN5_T).toFixed(3);
            var TOTAL_PCI_LY1_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LY1', false, 'sum');
            TOTAL_PCI_LY1_T = parseFloat(TOTAL_PCI_LY1_T).toFixed(3);
            var TOTAL_PCI_LY2_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LY2', false, 'sum');
            TOTAL_PCI_LY2_T = parseFloat(TOTAL_PCI_LY2_T).toFixed(3);
            var TOTAL_PCI_LY3_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LY3', false, 'sum');
            TOTAL_PCI_LY3_T = parseFloat(TOTAL_PCI_LY3_T).toFixed(3);
            //

            $(this).jqGrid('footerData', 'set', { MAST_BLOCK_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_CN: TOTAL_CN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI: TOTAL_PCI_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN: TOTAL_PCI_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN1: TOTAL_PCI_LEN1_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN2: TOTAL_PCI_LEN2_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN3: TOTAL_PCI_LEN3_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN4: TOTAL_PCI_LEN4_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN5: TOTAL_PCI_LEN5_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LY1: TOTAL_PCI_LY1_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LY2: TOTAL_PCI_LY2_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LY3: TOTAL_PCI_LY3_T }, true);
            $('#PCIAbstractBlockReportTable_rn').html('Sr.<br/>No.');
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

    $("#PCIAbstractBlockReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
         // { startColumnName: 'TOTAL_CN', numberOfColumns: 12, titleText: '<em>Total</em>' },
          {
              startColumnName: 'MAST_BLOCK_NAME', numberOfColumns: 3,
              titleText: '<table style="width:100%;border-spacing:0px"' +
                      '<tr><td id="h0"  style="border-bottom-width: 1px;" colspan="3">Target for PCI data to Enter </td>  </tr>' +
                      '<tr>' +
                          '<td id="h1" colspan="3" style="width: 22%; border-right-width: 1px; border-right-color: inherit;"></td>' +
                      '</tr>' +
                      '</table>'
          },
             {
                 startColumnName: 'TOTAL_PCI', numberOfColumns: 10,
                 titleText: '<table style="width:100%;border-spacing:0px"' +
                         '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="10">PCI Data Entered</td>  </tr>' +
                         '<tr>' +
                             '<td id="h1" colspan="2" style="width: 14%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">PCI data Entered For</td>' +
                               '<td id="h1" colspan="5" style="width: 36%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Length(Kms) with PCI Value</td>' +
                               '<td id="h2" colspan="3" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Year Wise Length (Kms)</td>' +
                         '</tr>' +
                         '</table>'
             }
        ]
    });
}

/*       FINAL REPORT LISTING       */
function PCIAbstractFinalReportListing(blockCode, districtCode, stateCode, blockName, route) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#PCIAbstractDistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#PCIAbstractStateReportTable").jqGrid('setGridState', 'hidden');
    $("#PCIAbstractBlockReportTable").jqGrid('setGridState', 'hidden');
    $("#PCIAbstractBlockReportTable").jqGrid('setSelection', stateCode);
    $("#PCIAbstractFinalReportTable").jqGrid('GridUnload');


    $("#PCIAbstractFinalReportTable").jqGrid({
        url: '/CoreNetworkReports/PCIAbstractFinalReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Road Number', 'Road Name', 'Route', 'Length (Kms)', 'Year', 'Segment Number', 'Start Chainage (Kms)', 'End Chainage (Kms)', 'Surface Type', 'PCI Index'],
        colModel: [
            { name: 'PLAN_CN_ROAD_NUMBER', width: 120, align: 'left', height: 'auto', sortable: true },
            { name: 'PLAN_RD_NAME', width: 200, align: 'left', height: 'auto', sortable: false },
            { name: 'PLAN_RD_ROUTE', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'PLAN_RD_LENGTH', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MANE_PCI_YEAR', width: 120, align: 'center', height: 'auto', sortable: false },
            { name: 'MANE_SEGMENT_NO', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MANE_STR_CHAIN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MANE_END_CHAIN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MANE_SURFACE_TYPE', width: 120, align: 'center', height: 'auto', sortable: false },
            { name: 'MANE_PCIINDEX', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { decimalPlaces: 3, defaulValue: "N.A" } }
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "Route": route },
        pager: $("#PCIAbstractFinalReportPager"),
        footerrow: true,
        sortname: 'RoadName',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '400',
        viewrecords: true,
        caption: 'PCI Abstract Details ' + blockName,
        loadComplete: function () {
            //Total of Columns
            var PLAN_RD_LENGTH_T = $(this).jqGrid('getCol', 'PLAN_RD_LENGTH', false, 'sum');
            PLAN_RD_LENGTH_T = parseFloat(PLAN_RD_LENGTH_T).toFixed(3);
            //

            $(this).jqGrid('footerData', 'set', { PLAN_CN_ROAD_NUMBER: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { PLAN_RD_LENGTH: PLAN_RD_LENGTH_T }, true);
            $('#PCIAbstractFinalReportTable_rn').html('Sr.<br/>No.');
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