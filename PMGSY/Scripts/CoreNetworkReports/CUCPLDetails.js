$(function () {
    $("#btCUCPLDetails").click(function () {

        var route = $("#Route_CUCPLDetails").val();

        if ($("#hdnLevelId").val() == 6) //mord
        {
           
            CUCPLStateReportListing(route);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
           
            CUCPLDistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), route);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
           
            CUCPLBlockReportListing($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), route);
        }
    });
    $("#btCUCPLDetails").trigger("click");
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});

/*       STATE REPORT LISTING       */
function CUCPLStateReportListing(route) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CUCPLStateReportTable").jqGrid('GridUnload');
    $("#CUCPLDistrictReportTable").jqGrid('GridUnload');
    $("#CUCPLBlockReportTable").jqGrid('GridUnload');
    $("#CUCPLFinalReportTable").jqGrid('GridUnload');
    $("#CUCPLStateReportTable").jqGrid({
        url: '/CoreNetworkReports/CUCPLStateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Core Network Number', 'Core Network Length (Kms)', '1', '2', '3', '4','5'],
        colModel: [
            { name: 'MAST_STATE_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'PCI1', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'PCI2', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'PCI3', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'PCI4', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'PCI5', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        postData: {"Route": route },
        pager: $("#CUCPLStateReportPager"),
        footerrow: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '550',
        viewrecords: true,
        caption: 'State Core Network Upgradation Priority List Details',
        loadComplete: function () {
            //Total of Columns
            var TOTAL_CN_T = $(this).jqGrid('getCol', 'TOTAL_CN', false, 'sum');
            var TOTAL_LEN_T = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            TOTAL_LEN_T = parseFloat(TOTAL_LEN_T).toFixed(3);
            var PCI1_T = $(this).jqGrid('getCol', 'PCI1', false, 'sum');
            var PCI2_T = $(this).jqGrid('getCol', 'PCI2', false, 'sum');
            var PCI3_T = $(this).jqGrid('getCol', 'PCI3', false, 'sum');
            var PCI4_T = $(this).jqGrid('getCol', 'PCI4', false, 'sum');
            var PCI5_T = $(this).jqGrid('getCol', 'PCI5', false, 'sum');
            //

            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_CN: TOTAL_CN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { PCI1: PCI1_T }, true);
            $(this).jqGrid('footerData', 'set', { PCI2: PCI2_T }, true);
            $(this).jqGrid('footerData', 'set', { PCI3: PCI3_T }, true);
            $(this).jqGrid('footerData', 'set', { PCI4: PCI4_T }, true);
            $(this).jqGrid('footerData', 'set', { PCI5: PCI5_T }, true);
            $('#CUCPLStateReportTable_rn').html('Sr.<br/>No.');

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
    }); /*End of Grid*/

    $("#CUCPLStateReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'PCI1', numberOfColumns: 5, titleText: '<em>PCI</em>' }        
        ]
    });
}
/**/

/*       DISTRICT REPORT LISTING       */
function CUCPLDistrictReportListing(stateCode, stateName, route) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


    $("#CUCPLStateReportTable").jqGrid('setSelection', stateCode);
    $("#CUCPLStateReportTable").jqGrid('setGridState', 'hidden');
    $("#CUCPLDistrictReportTable").jqGrid('GridUnload');
    $("#CUCPLBlockReportTable").jqGrid('GridUnload');
    $("#CUCPLFinalReportTable").jqGrid('GridUnload');

    $("#CUCPLDistrictReportTable").jqGrid({
        url: '/CoreNetworkReports/CUCPLDistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Core Network Number', 'Core Network Length (Kms)', '1', '2', '3', '4', '5'],
        colModel: [
            { name: 'MAST_DISTRICT_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'PCI1', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'PCI2', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'PCI3', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'PCI4', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'PCI5', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }

        ],
        postData: { "StateCode": stateCode, "Route": route },
        pager: $("#CUCPLDistrictReportPager"),
        footerrow: true,
        sortname: 'MAST_DISTRICT_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '510',
        viewrecords: true,
        caption: 'District Core Network Upgradation Priority List Details for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_CN_T = $(this).jqGrid('getCol', 'TOTAL_CN', false, 'sum');
            var TOTAL_LEN_T = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            TOTAL_LEN_T = parseFloat(TOTAL_LEN_T).toFixed(3);
            var PCI1_T = $(this).jqGrid('getCol', 'PCI1', false, 'sum');
            var PCI2_T = $(this).jqGrid('getCol', 'PCI2', false, 'sum');
            var PCI3_T = $(this).jqGrid('getCol', 'PCI3', false, 'sum');
            var PCI4_T = $(this).jqGrid('getCol', 'PCI4', false, 'sum');
            var PCI5_T = $(this).jqGrid('getCol', 'PCI5', false, 'sum');

            //

            $(this).jqGrid('footerData', 'set', { MAST_DISTRICT_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_CN: TOTAL_CN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { PCI1: PCI1_T }, true);
            $(this).jqGrid('footerData', 'set', { PCI2: PCI2_T }, true);
            $(this).jqGrid('footerData', 'set', { PCI3: PCI3_T }, true);
            $(this).jqGrid('footerData', 'set', { PCI4: PCI4_T }, true);
            $(this).jqGrid('footerData', 'set', { PCI5: PCI5_T }, true);
            $('#CUCPLDistrictReportTable_rn').html('Sr.<br/>No.');
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
    }); /*End of Grid*/

    $("#CUCPLDistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'PCI1', numberOfColumns: 5, titleText: '<em>PCI</em>' }
        ]
    });

}
/**/

/*       BLOCK REPORT LISTING       */
function CUCPLBlockReportListing(districtCode, stateCode, districtName, route) {

    var distname;
    if (districtName == '')
        distname = $("#DISTRICT_NAME").val();
    else
        distname = districtName;

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


    $("#CUCPLDistrictReportTable").jqGrid('setSelection', stateCode);
    $("#CUCPLDistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#CUCPLStateReportTable").jqGrid('setGridState', 'hidden');
    $("#CUCPLBlockReportTable").jqGrid('GridUnload');
    $("#CUCPLFinalReportTable").jqGrid('GridUnload');

    $("#CUCPLBlockReportTable").jqGrid({
        url: '/CoreNetworkReports/CUCPLBlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Core Network Number', 'Core Network Length (Kms)', '1', '2', '3', '4', '5'],
        colModel: [
            { name: 'MAST_BLOCK_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'PCI1', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'PCI2', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'PCI3', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'PCI4', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'PCI5', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }

        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "Route": route },
        pager: $("#CUCPLBlockReportPager"),
        footerrow: true,
        sortname: 'MAST_BLOCK_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '460',
        viewrecords: true,
        caption: 'Block Core Network Upgradation Priority List Details for ' + distname,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_CN_T = $(this).jqGrid('getCol', 'TOTAL_CN', false, 'sum');
            var TOTAL_LEN_T = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            TOTAL_LEN_T = parseFloat(TOTAL_LEN_T).toFixed(3);
         
            var PCI1_T = $(this).jqGrid('getCol', 'PCI1', false, 'sum');
            var PCI2_T = $(this).jqGrid('getCol', 'PCI2', false, 'sum');
            var PCI3_T = $(this).jqGrid('getCol', 'PCI3', false, 'sum');
            var PCI4_T = $(this).jqGrid('getCol', 'PCI4', false, 'sum');
            var PCI5_T = $(this).jqGrid('getCol', 'PCI5', false, 'sum');

            //

            $(this).jqGrid('footerData', 'set', { MAST_BLOCK_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_CN: TOTAL_CN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { PCI1: PCI1_T }, true);
            $(this).jqGrid('footerData', 'set', { PCI2: PCI2_T }, true);
            $(this).jqGrid('footerData', 'set', { PCI3: PCI3_T }, true);
            $(this).jqGrid('footerData', 'set', { PCI4: PCI4_T }, true);
            $(this).jqGrid('footerData', 'set', { PCI5: PCI5_T }, true);
            $('#CUCPLBlockReportTable_rn').html('Sr.<br/>No.');

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
    });/*End of Grid*/

    $("#CUCPLBlockReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'PCI1', numberOfColumns: 5, titleText: '<em>PCI</em>' }
        ]
    });
}

/*       FINAL REPORT LISTING       */
function CUCPLFinalReportListing(blockCode, districtCode, stateCode, blockName, route) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CUCPLBlockReportTable").jqGrid('setSelection', stateCode);
    $("#CUCPLBlockReportTable").jqGrid('setGridState', 'hidden');
    $("#CUCPLDistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#CUCPLStateReportTable").jqGrid('setGridState', 'hidden');

    $("#CUCPLFinalReportTable").jqGrid('GridUnload');


    $("#CUCPLFinalReportTable").jqGrid({
        url: '/CoreNetworkReports/CUCPLFinalReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Road Number', 'Road Name', 'Road Length (Kms)', 'Year of Construction', 'Year of Last Periodic Renewal', 'Segment Number', 'Year', 'Start Chainage', 'End Chainage', 'PCI Value', 'Average PCI', 'Total Population of the Habitations served', 'Year', 'Value'],
        colModel: [
            { name: 'PLAN_CN_ROAD_NUMBER', width: 120, align: 'left', height: 'auto', sortable: true },
            { name: 'PLAN_RD_NAME', width: 200, align: 'left', height: 'auto', sortable: false },
            { name: 'PLAN_RD_LENGTH', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MAST_CONS_YEAR', width: 100, align: 'right', height: 'auto', sortable: false },
            { name: 'MAST_RENEW_YEAR', width: 100, align: 'right', height: 'auto', sortable: false },
            { name: 'MANE_SEGMENT_NO', width: 50, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MANE_PCI_YEAR', width: 50, align: 'right', height: 'auto', sortable: false },
            { name: 'MANE_STR_CHAIN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MANE_END_CHAIN', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MANE_PCIINDEX', width: 80, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'AVG_PCI', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'POP', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MAST_TI_YEAR', width: 50, align: 'right', height: 'auto', sortable: false },
            { name: 'MAST_COMM_TI', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode,"Route":route },
        pager: $("#CUCPLFinalReportPager"),
        footerrow: true,
        sortname: 'RoadName',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '410',
        viewrecords: true,
        caption: 'Core Network Upgradation Priority List Details for ' + blockName,
        loadComplete: function () {
            //Total of Columns
            var PLAN_RD_LENGTH_T = $(this).jqGrid('getCol', 'PLAN_RD_LENGTH', false, 'sum');
            PLAN_RD_LENGTH_T = parseFloat(PLAN_RD_LENGTH_T).toFixed(3);
            var POP_T = $(this).jqGrid('getCol', 'POP', false, 'sum');

           // var MANE_STR_CHAIN_T = $(this).jqGrid('getCol', 'MANE_STR_CHAIN', false, 'sum');
            //var MANE_END_CHAIN_T = $(this).jqGrid('getCol', 'MANE_END_CHAIN', false, 'sum');
            //var AVG_PCI_T = $(this).jqGrid('getCol', 'AVG_PCI', false, 'sum');
            //

            $(this).jqGrid('footerData', 'set', { PLAN_CN_ROAD_NUMBER: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { PLAN_RD_LENGTH: PLAN_RD_LENGTH_T }, true);
            $(this).jqGrid('footerData', 'set', { POP: POP_T }, true);

            //$(this).jqGrid('footerData', 'set', { MANE_STR_CHAIN: MANE_STR_CHAIN_T }, true);
            //$(this).jqGrid('footerData', 'set', { MANE_END_CHAIN: MANE_END_CHAIN_T }, true);
            //$(this).jqGrid('footerData', 'set', { AVG_PCI: AVG_PCI_T }, true);
            $('#CUCPLFinalReportTable_rn').html('Sr.<br/>No.');
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
    });/*End of Grid*/
    $("#CUCPLFinalReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'MANE_PCI_YEAR', numberOfColumns: 4, titleText: '<em>PCI</em>' },
          { startColumnName: 'MAST_TI_YEAR', numberOfColumns: 4, titleText: '<em>AADT</em>' }

        ]
    });

}
/**/