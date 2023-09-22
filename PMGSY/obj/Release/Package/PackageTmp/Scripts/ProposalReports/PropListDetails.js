$(function () {
    $('#btnPropListDetails').click(function () {
        var year = $('#ddYear_PropListDetails').val();
        var batch = $('#ddBatch_PropListDetails').val();
        var collaboration = $('#ddAgency_PropListDetails').val();
        var status = $('#ddStatus_PropListDetails').val();
        if ($("#hdnLevelId").val() == 6) //mord
        {
            PropListStateReportListing(year,batch,collaboration,status);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
            PropListDistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), year, batch, collaboration, status);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {

            PropListBlockReportListing($("#MAST_DISTRICT_CODE").val(), $("#MAST_STATE_CODE").val(), $("#DISTRICT_NAME").val(), year, batch, collaboration, status);
        }
    });

    $('#btnPropListDetails').trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});

/*       STATE REPORT LISTING       */
function PropListStateReportListing(year, batch, collaboration, status) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#PropListStateReportTable").jqGrid('GridUnload');
    $("#PropListDistrictReportTable").jqGrid('GridUnload');
    $("#PropListBlockReportTable").jqGrid('GridUnload');
    $("#PropListFinalReportTable").jqGrid('GridUnload');

    $("#PropListStateReportTable").jqGrid({
        url: '/ProposalReports/PropListStateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Total Proposal'],
        colModel: [
            { name: "MAST_STATE_NAME", width: 200, align: 'center',  height: 'auto', sortable: true },
            { name: "TOTAL_PROPOSALS", width: 150, align: 'center',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
             ],
        postData: { "Year": year, "Batch": batch, "Collaboration": collaboration, "Status": status },
        pager: $("#PropListStateReportPager"),
        footerrow: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'State Proposal List Details',
        loadComplete: function () {


            //Total of Columns
            var TOTAL_PROPOSALS_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS', false, 'sum');
            //
            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS: TOTAL_PROPOSALS_T }, true);
          
            $('#PropListStateReportTable_rn').html('Sr.<br/>No.');

         
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

    //$("#PropListStateReportTable").jqGrid('setGroupHeaders', {
    //    useColSpanStyle: false,
    //    groupHeaders: [
    //      { startColumnName: 'TN_PROPOSALS', numberOfColumns: 7, titleText: '<em> Total New Connectivity</em>' }         
    //    ]
    //});
}
/**/

/*       DISTRICT REPORT LISTING       */
function PropListDistrictReportListing(stateCode, stateName, year, batch, collaboration, status) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#PropListStateReportTable").jqGrid('setGridState', 'hidden');
    $("#PropListStateReportTable").jqGrid('setSelection', stateCode);
    $("#PropListDistrictReportTable").jqGrid('GridUnload');
    $("#PropListBlockReportTable").jqGrid('GridUnload');
    $("#PropListFinalReportTable").jqGrid('GridUnload');

    $("#PropListDistrictReportTable").jqGrid({
        url: '/ProposalReports/PropListDistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Total Proposal'],
        colModel: [
            { name: "MAST_DISTRICT_NAME", width: 200, align: 'center',  height: 'auto', sortable: true },
            { name: "TOTAL_PROPOSALS", width: 150, align: 'center',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "Year": year, "Batch": batch, "Collaboration": collaboration, "Status": status },
        pager: $("#PropListDistrictReportPager"),
        footerrow: true,
        sortname: 'MAST_DISTRICT_NAME',      
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '520',
        viewrecords: true,
        caption: 'District Proposal List for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_PROPOSALS_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS', false, 'sum');

            //
            $(this).jqGrid('footerData', 'set', { MAST_DISTRICT_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS: TOTAL_PROPOSALS_T }, true);
           
            $('#PropListDistrictReportTable_rn').html('Sr.<br/>No.');

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

    //$("#PropListDistrictReportTable").jqGrid('setGroupHeaders', {
    //    useColSpanStyle: false,
    //    groupHeaders: [
    //      { startColumnName: 'TN_PROPOSALS', numberOfColumns: 7, titleText: '<em> Total New Connectivity</em>' }   
    //    ]
    //});

}
/**/

/*       BLOCK REPORT LISTING       */
function PropListBlockReportListing(districtCode, stateCode, districtName, year, batch, collaboration, status) {
    var distName;
    if (districtName == '')
        distName = $("#DISTRICT_NAME").val();
    else
        distName = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#PropListDistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#PropListStateReportTable").jqGrid('setGridState', 'hidden');
    $("#PropListDistrictReportTable").jqGrid('setSelection', stateCode);
    $("#PropListBlockReportTable").jqGrid('GridUnload');
    $("#PropListFinalReportTable").jqGrid('GridUnload');

    $("#PropListBlockReportTable").jqGrid({
        url: '/ProposalReports/PropListBlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Total Proposal'],
        colModel: [
            { name: "MAST_BLOCK_NAME", width: 200, align: 'center',  height: 'auto', sortable: true },
            { name: "TOTAL_PROPOSALS", width: 150, align: 'center',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "Year": year, "Batch": batch, "Collaboration": collaboration, "Status": status },
        pager: $("#PropListBlockReportPager"),
        footerrow: true,
        sortname: 'MAST_BLOCK_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '470',
        viewrecords: true,
        caption: 'Block Proposal List for ' + distName,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_PROPOSALS_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS', false, 'sum');

            //
            $(this).jqGrid('footerData', 'set', { MAST_BLOCK_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS: TOTAL_PROPOSALS_T }, true);
           
            $('#PropListBlockReportTable_rn').html('Sr.<br/>No.');

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

    //$("#PropListBlockReportTable").jqGrid('setGroupHeaders', {
    //    useColSpanStyle: false,
    //    groupHeaders: [
    //      { startColumnName: 'TN_PROPOSALS', numberOfColumns: 7, titleText: '<em> Total New Connectivity</em>' },
        
    //    ]
    //});
}

/*       FINAL REPORT LISTING       */
function PropListFinalReportListing(blockCode, districtCode, stateCode, blockName, year, batch, collaboration, status) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#PropListBlockReportTable").jqGrid('setGridState', 'hidden');
    $("#PropListDistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#PropListStateReportTable").jqGrid('setGridState', 'hidden');
    $("#PropListBlockReportTable").jqGrid('setSelection', stateCode);
    $("#PropListFinalReportTable").jqGrid('GridUnload');
    $("#PropListFinalReportTable").jqGrid({
        url: '/ProposalReports/PropListFinalReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Sanctioned Year', 'Batch', 'Package', 'Road / Bridge Name', 'Road Length ', 'BT Length ', 'CC Length', 'Collaboration',
                   'Road Amount', 'Bridge Amount', 'Maintenance Amount', 'Habitation Name', 'Habtation Status', 'Total Habitation Population','Status'],
        colModel: [
            { name: "MAST_BLOCK_NAME", width: 120, align: 'left',  height: 'auto', sortable: true },
            { name: "IMS_YEAR", width: 120, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_BATCH", width: 70, align: 'center',  height: 'auto', sortable: false },
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
            { name: "STATUS", width: 180, align: 'left',  height: 'auto', sortable: false }
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "Year": year, "Batch": batch, "Collaboration": collaboration, "Status": status },
        pager: $("#PropListFinalReportPager"),
        footerrow: true,
        sortname: 'MAST_BLOCK_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '400',
        //width: 1120,
        //shrinkToFit:false,
        viewrecords: true,
        caption: 'Proposal List for  ' + blockName,
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
            BRIDGE_AMT_T = parseFloat(BRIDGE_AMT_T).toFixed(2);
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

            $("#PropListFinalReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Amount Rs in Lacs & Length in Kms.</font>");
            $('#PropListFinalReportTable_rn').html('Sr.<br/>No.');

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