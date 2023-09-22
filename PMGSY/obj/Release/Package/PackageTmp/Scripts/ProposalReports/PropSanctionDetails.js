$(function () {

    $('#btnPropSanctionDetails').click(function () {
        var year = $('#ddYear_PropSanctionDetails').val();
        var batch = $('#ddBatch_PropSanctionDetails').val();
        var collaboration = $('#ddAgency_PropSanctionDetails').val();
        // var status = $('#ddStatus_PropSanctionDetails').val();
        var status = 'Y';
        if ($("#hdnLevelId").val() == 6) //mord
        {
            PropSanctionStateReportListing(year, batch, collaboration, status);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
            PropSanctionDistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), year, batch, collaboration, status);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {

            PropSanctionBlockReportListing($("#MAST_DISTRICT_CODE").val(), $("#MAST_STATE_CODE").val(), $("#DISTRICT_NAME").val(), year, batch, collaboration, status);
        }
    });

    $('#btnPropSanctionDetails').trigger('click');

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});

/*       STATE REPORT LISTING       */
function PropSanctionStateReportListing(year, batch, collaboration, status) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#PropSanctionStateReportTable").jqGrid('GridUnload');
    $("#PropSanctionDistrictReportTable").jqGrid('GridUnload');
    $("#PropSanctionBlockReportTable").jqGrid('GridUnload');
    $("#PropSanctionFinalReportTable").jqGrid('GridUnload');

    $("#PropSanctionStateReportTable").jqGrid({
        url: '/ProposalReports/PropSanctionStateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Total Proposal'],
        colModel: [
            { name: "MAST_STATE_NAME", width: 200, align: 'center',  height: 'auto', sortable: true },
            { name: "TOTAL_PROPOSALS", width: 150, align: 'center',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "Year": year, "Batch": batch, "Collaboration": collaboration, "Status": status },
        pager: $("#PropSanctionStateReportPager"),
        footerrow: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'State Sanction Proposal List Details',
        loadComplete: function () {


            //Total of Columns
            var TOTAL_PROPOSALS_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS', false, 'sum');
            //
            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS: TOTAL_PROPOSALS_T }, true);

            $('#PropSanctionStateReportTable_rn').html('Sr.<br/>No.');


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

    //$("#PropSanctionStateReportTable").jqGrid('setGroupHeaders', {
    //    useColSpanStyle: false,
    //    groupHeaders: [
    //      { startColumnName: 'TN_PROPOSALS', numberOfColumns: 7, titleText: '<em> Total New Connectivity</em>' }         
    //    ]
    //});
}
/**/

/*       DISTRICT REPORT LISTING       */
function PropSanctionDistrictReportListing(stateCode, stateName, year, batch, collaboration, status) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#PropSanctionStateReportTable").jqGrid('setGridState', 'hidden');
    $("#PropSanctionStateReportTable").jqGrid('setSelection', stateCode);
    $("#PropSanctionDistrictReportTable").jqGrid('GridUnload');
    $("#PropSanctionBlockReportTable").jqGrid('GridUnload');
    $("#PropSanctionFinalReportTable").jqGrid('GridUnload');
    $("#PropSanctionDistrictReportTable").jqGrid({
        url: '/ProposalReports/PropSanctionDistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Total Proposal'],
        colModel: [
            { name: "MAST_DISTRICT_NAME", width: 200, align: 'center',  height: 'auto', sortable: true },
            { name: "TOTAL_PROPOSALS", width: 150, align: 'center',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "Year": year, "Batch": batch, "Collaboration": collaboration, "Status": status },
        pager: $("#PropSanctionDistrictReportPager"),
        footerrow: true,
        sortname: 'MAST_DISTRICT_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '520',
        viewrecords: true,
        caption: 'District Sanction Proposal List for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_PROPOSALS_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS', false, 'sum');

            //
            $(this).jqGrid('footerData', 'set', { MAST_DISTRICT_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS: TOTAL_PROPOSALS_T }, true);

            $('#PropSanctionDistrictReportTable_rn').html('Sr.<br/>No.');

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

    //$("#PropSanctionDistrictReportTable").jqGrid('setGroupHeaders', {
    //    useColSpanStyle: false,
    //    groupHeaders: [
    //      { startColumnName: 'TN_PROPOSALS', numberOfColumns: 7, titleText: '<em> Total New Connectivity</em>' }   
    //    ]
    //});

}
/**/

/*       BLOCK REPORT LISTING       */
function PropSanctionBlockReportListing(districtCode, stateCode, districtName, year, batch, collaboration, status) {
    var distName;
    if (districtName == '')
        distName = $("#DISTRICT_NAME").val();
    else
        distName = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#PropSanctionDistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#PropSanctionStateReportTable").jqGrid('setGridState', 'hidden');
    $("#PropSanctionDistrictReportTable").jqGrid('setSelection', stateCode);
    $("#PropSanctionBlockReportTable").jqGrid('GridUnload');
    $("#PropSanctionFinalReportTable").jqGrid('GridUnload');

    $("#PropSanctionBlockReportTable").jqGrid({
        url: '/ProposalReports/PropSanctionBlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Total Proposal'],
        colModel: [
            { name: "MAST_BLOCK_NAME", width: 200, align: 'center',  height: 'auto', sortable: true },
            { name: "TOTAL_PROPOSALS", width: 150, align: 'center',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "Year": year, "Batch": batch, "Collaboration": collaboration, "Status": status },
        pager: $("#PropSanctionBlockReportPager"),
        footerrow: true,
        sortname: 'MAST_BLOCK_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '470',
        viewrecords: true,
        caption: 'Block Sanction Proposal List for ' + distName,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_PROPOSALS_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS', false, 'sum');

            //
            $(this).jqGrid('footerData', 'set', { MAST_BLOCK_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS: TOTAL_PROPOSALS_T }, true);

            $('#PropSanctionBlockReportTable_rn').html('Sr.<br/>No.');

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

    //$("#PropSanctionBlockReportTable").jqGrid('setGroupHeaders', {
    //    useColSpanStyle: false,
    //    groupHeaders: [
    //      { startColumnName: 'TN_PROPOSALS', numberOfColumns: 7, titleText: '<em> Total New Connectivity</em>' },

    //    ]
    //});
}

/*       FINAL REPORT LISTING       */
function PropSanctionFinalReportListing(blockCode, districtCode, stateCode, blockName, year, batch, collaboration, status) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#PropSanctionBlockReportTable").jqGrid('setGridState', 'hidden');
    $("#PropSanctionDistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#PropSanctionStateReportTable").jqGrid('setGridState', 'hidden');
    $("#PropSanctionBlockReportTable").jqGrid('setSelection', stateCode);
    $("#PropSanctionFinalReportTable").jqGrid('GridUnload');


    $("#PropSanctionFinalReportTable").jqGrid({
        url: '/ProposalReports/PropSanctionFinalReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Sanctioned Year', 'Batch', 'Package Number', 'Road / Bridge Name', 'Road Length ', 'BT Length ', 'CC Length', 'Collaboration',
                   'Road Amount', 'Bridge Amount', 'Maintenance Amount', 'Habitation Name', 'Habtation Status', 'Total Habitation Population'],
        colModel: [
            { name: "MAST_BLOCK_NAME", width: 120, align: 'left',  height: 'auto', sortable: true },
            { name: "IMS_YEAR", width: 120, align: 'right',  height: 'auto', sortable: false },
            { name: "IMS_BATCH", width: 70, align: 'right',  height: 'auto', sortable: false },
            { name: "IMS_PACKAGE_ID", width: 150, align: 'left',  height: 'auto', sortable: false },
             { name: "IMS_ROAD_NAME", width: 200, align: 'left',  height: 'auto', sortable: false },
            { name: "ROAD_LENGTH", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "IMS_BT_LENGTH", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "IMS_CC_LENGTH", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "IMS_COLLABORATION", width: 200, align: 'left',  height: 'auto', sortable: false },
            { name: "ROAD_AMT", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "BRIDGE_AMT", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT_AMT", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAST_HAB_NAME", width: 150, align: 'left',  height: 'auto', sortable: false },
            { name: "MAST_HAB_STATUS", width: 150, align: 'left',  height: 'auto', sortable: false },
            { name: "MAST_HAB_TOT_POP", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },

        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "Year": year, "Batch": batch, "Collaboration": collaboration, "Status": status },
        pager: $("#PropSanctionFinalReportPager"),
        footerrow: true,
        sortname: 'MAST_BLOCK_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '400',
        viewrecords: true,
        caption: 'Sanction Proposal List for  ' + blockName,
        loadComplete: function () {
            //Total of Columns
            var ROAD_LENGTH_T = $(this).jqGrid('getCol', 'ROAD_LENGTH', false, 'sum');
            ROAD_LENGTH_T = parseFloat(ROAD_LENGTH_T).toFixed(3);
            var IMS_BT_LENGTH_T = $(this).jqGrid('getCol', 'IMS_BT_LENGTH', false, 'sum');
            IMS_BT_LENGTH_T = parseFloat(IMS_BT_LENGTH_T).toFixed(3);
            var IMS_CC_LENGTH_T = $(this).jqGrid('getCol', 'IMS_CC_LENGTH', false, 'sum');
            IMS_CC_LENGTH_T = parseFloat(IMS_CC_LENGTH_T).toFixed(3);
            var ROAD_AMT_T = $(this).jqGrid('getCol', 'ROAD_AMT', false, 'sum');
            IMS_CC_LENGTH_T = parseFloat(IMS_CC_LENGTH_T).toFixed(3);
            var BRIDGE_AMT_T = $(this).jqGrid('getCol', 'BRIDGE_AMT', false, 'sum');
            BRIDGE_AMT_T = parseFloat(BRIDGE_AMT_T).toFixed(2);
            var MAINT_AMT_T = $(this).jqGrid('getCol', 'MAINT_AMT', false, 'sum');
            MAINT_AMT_T = parseFloat(MAINT_AMT_T).toFixed(2);
            var MAST_HAB_TOT_POP_T = $(this).jqGrid('getCol', 'MAST_HAB_TOT_POP', false, 'sum');
            ////

            $(this).jqGrid('footerData', 'set', { MAST_BLOCK_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { ROAD_LENGTH: ROAD_LENGTH_T }, true);
            $(this).jqGrid('footerData', 'set', { IMS_BT_LENGTH: IMS_BT_LENGTH_T }, true);
            $(this).jqGrid('footerData', 'set', { IMS_CC_LENGTH: IMS_CC_LENGTH_T }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_AMT: ROAD_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { BRIDGE_AMT: BRIDGE_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT_AMT: MAINT_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAST_HAB_TOT_POP: MAST_HAB_TOT_POP_T }, true);

            $("#PropSanctionFinalReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Amount Rs in Lacs & Length in Kms.</font>");
            $('#PropSanctionFinalReportTable_rn').html('Sr.<br/>No.');

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