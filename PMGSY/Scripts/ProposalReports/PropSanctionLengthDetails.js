$(function () {
    $('#btnPropSanctionLengthDetails').click(function () {
        var year = $('#ddYear_PropSanctionLengthDetails').val();
        var batch = $('#ddBatch_PropSanctionLengthDetails').val();
        var collaboration = $('#ddAgency_PropSanctionLengthDetails').val();
        var status = $('#ddStatus_PropSanctionLengthDetails').val();
        if ($("#hdnLevelId").val() == 6) //mord
        {
            PropSanctionLengthStateReportListing(year, batch, collaboration, status);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
            PropSanctionLengthDistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), year, batch, collaboration, status);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {

            PropSanctionLengthBlockReportListing($("#MAST_DISTRICT_CODE").val(), $("#MAST_STATE_CODE").val(), $("#DISTRICT_NAME").val(), year, batch, collaboration, status);
        }
    });

    $('#btnPropSanctionLengthDetails').trigger('click');
  
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});

/*       STATE REPORT LISTING       */
function PropSanctionLengthStateReportListing(year, batch, collaboration, status) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#PropSanctionLengthStateReportTable").jqGrid('GridUnload');
    $("#PropSanctionLengthDistrictReportTable").jqGrid('GridUnload');
    $("#PropSanctionLengthBlockReportTable").jqGrid('GridUnload');
    $("#PropSanctionLengthFinalReportTable").jqGrid('GridUnload');

    $("#PropSanctionLengthStateReportTable").jqGrid({
        url: '/ProposalReports/PropSanctionLengthStateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Total Proposals', 'Total Road Length (Kms.)', 'BT Length (Kms.)', 'CC Length (Kms.)', 'CC (%)'],
        colModel: [
            { name: "MAST_STATE_NAME", width: 200, align: 'left',  height: 'auto', sortable: true },
            { name: "TOTAL_PROPOSALS", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "Road_Length", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "BT_LEN", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CC_LEN", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CC_Per", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "Year": year, "Batch": batch, "Collaboration": collaboration, "Status": status },
        pager: $("#PropSanctionLengthStateReportPager"),
        footerrow: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        shrinktofit:true,
        height: '580',
        viewrecords: true,
        caption: 'State Proposal Sanction Length Details',
        loadComplete: function () {


            //Total of Columns
            var TOTAL_PROPOSALS_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS', false, 'sum');
            var Road_Length_T = $(this).jqGrid('getCol', 'Road_Length', false, 'sum');
            Road_Length_T = parseFloat(Road_Length_T).toFixed(3);
            var BT_LEN_T = $(this).jqGrid('getCol', 'BT_LEN', false, 'sum');
            BT_LEN_T = parseFloat(BT_LEN_T).toFixed(3);
            var CC_LEN_T = $(this).jqGrid('getCol', 'CC_LEN', false, 'sum');
            CC_LEN_T = parseFloat(CC_LEN_T).toFixed(3);
            var percentage = 0;
            if (Road_Length_T > 0) {
                percentage = (CC_LEN_T / Road_Length_T) * 100;
            }
            //
            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS: TOTAL_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { Road_Length: Road_Length_T }, true);
            $(this).jqGrid('footerData', 'set', { BT_LEN: BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CC_LEN: CC_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CC_Per: percentage }, true);

            //$("#PropSanctionLengthStateReportTable").jqGrid('setGridWidth', 1600);
            $('#PropSanctionLengthStateReportTable_rn').html('Sr.<br/>No.');
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
    });/* End of Grid*/

  
}
/**/

/*       DISTRICT REPORT LISTING       */
function PropSanctionLengthDistrictReportListing(stateCode, stateName, year, batch, collaboration, status) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#PropSanctionLengthStateReportTable").jqGrid('setGridState', 'hidden');
    $("#PropSanctionLengthStateReportTable").jqGrid('setSelection', stateCode);
    $("#PropSanctionLengthDistrictReportTable").jqGrid('GridUnload');
    $("#PropSanctionLengthBlockReportTable").jqGrid('GridUnload');
    $("#PropSanctionLengthFinalReportTable").jqGrid('GridUnload');

    $("#PropSanctionLengthDistrictReportTable").jqGrid({
        url: '/ProposalReports/PropSanctionLengthDistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Total Proposals', 'Total Road Length (Kms.)', 'BT Length (Kms.)', 'CC Length (Kms.)', 'CC (%)'],
        colModel: [
            { name: "MAST_DISTRICT_NAME", width: 200, align: 'left',  height: 'auto', sortable: true },
            { name: "TOTAL_PROPOSALS", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "Road_Length", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "BT_LEN", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CC_LEN", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CC_Per", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "Year": year, "Batch": batch, "Collaboration": collaboration, "Status": status },
        pager: $("#PropSanctionLengthDistrictReportPager"),
        footerrow: true,
        sortname: 'MAST_DISTRICT_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '530',
        viewrecords: true,
        caption: 'District Proposal Sanction Length for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_PROPOSALS_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS', false, 'sum');
            var Road_Length_T = $(this).jqGrid('getCol', 'Road_Length', false, 'sum');
            Road_Length_T = parseFloat(Road_Length_T).toFixed(3);
            var BT_LEN_T = $(this).jqGrid('getCol', 'BT_LEN', false, 'sum');
            BT_LEN_T = parseFloat(BT_LEN_T).toFixed(3);
            var CC_LEN_T = $(this).jqGrid('getCol', 'CC_LEN', false, 'sum');
            CC_LEN_T = parseFloat(CC_LEN_T).toFixed(3);
            var percentage = 0;
            if (Road_Length_T > 0) {
                percentage = (CC_LEN_T / Road_Length_T) * 100;
            }
            //
            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS: TOTAL_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { Road_Length: Road_Length_T }, true);
            $(this).jqGrid('footerData', 'set', { BT_LEN: BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CC_LEN: CC_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CC_Per: percentage }, true);
            $('#PropSanctionLengthDistrictReportTable_rn').html('Sr.<br/>No.');
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
    });/* End of Grid*/

   

}
/**/

/*       BLOCK REPORT LISTING       */
function PropSanctionLengthBlockReportListing(districtCode, stateCode, districtName, year, batch, collaboration, status) {
    var distName;
    if (districtName == '')
        distName = $("#DISTRICT_NAME").val();
    else
        distName = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#PropSanctionLengthDistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#PropSanctionLengthStateReportTable").jqGrid('setGridState', 'hidden');
    $("#PropSanctionLengthDistrictReportTable").jqGrid('setSelection', stateCode);
    $("#PropSanctionLengthBlockReportTable").jqGrid('GridUnload');
    $("#PropSanctionLengthFinalReportTable").jqGrid('GridUnload');

    $("#PropSanctionLengthBlockReportTable").jqGrid({
        url: '/ProposalReports/PropSanctionLengthBlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Total Proposals', 'Total Road Length (Kms.)', 'BT Length (Kms.)', 'CC Length (Kms.)', 'CC (%)'],
        colModel: [
            { name: "MAST_BLOCK_NAME", width: 200, align: 'left',  height: 'auto', sortable: true },
            { name: "TOTAL_PROPOSALS", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "Road_Length", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "BT_LEN", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CC_LEN", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CC_Per", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "Year": year, "Batch": batch, "Collaboration": collaboration, "Status": status },
        pager: $("#PropSanctionLengthBlockReportPager"),
        footerrow: true,
        sortname: 'MAST_BLOCK_NAME',
        sortorder: 'asc',      
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '470',
        viewrecords: true,
        caption: 'Block Proposal Sanction Length for ' + distName,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_PROPOSALS_T = $(this).jqGrid('getCol', 'TOTAL_PROPOSALS', false, 'sum');
            var Road_Length_T = $(this).jqGrid('getCol', 'Road_Length', false, 'sum');
            Road_Length_T = parseFloat(Road_Length_T).toFixed(3);
            var BT_LEN_T = $(this).jqGrid('getCol', 'BT_LEN', false, 'sum');
            BT_LEN_T = parseFloat(BT_LEN_T).toFixed(3);
            var CC_LEN_T = $(this).jqGrid('getCol', 'CC_LEN', false, 'sum');
            CC_LEN_T = parseFloat(CC_LEN_T).toFixed(3);
            var percentage = 0;
            if (Road_Length_T > 0) {
                percentage = (CC_LEN_T / Road_Length_T) * 100;
            }

            //
            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_PROPOSALS: TOTAL_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { Road_Length: Road_Length_T }, true);
            $(this).jqGrid('footerData', 'set', { BT_LEN: BT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CC_LEN: CC_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CC_Per: percentage }, true);
            $('#PropSanctionLengthBlockReportTable_rn').html('Sr.<br/>No.');
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
    }); /* End of Grid*/

  
}

/*       FINAL REPORT LISTING       */
function PropSanctionLengthFinalReportListing(blockCode, districtCode, stateCode, blockName, year, batch, collaboration, status) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#PropSanctionLengthBlockReportTable").jqGrid('setGridState', 'hidden');
    $("#PropSanctionLengthDistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#PropSanctionLengthStateReportTable").jqGrid('setGridState', 'hidden');
    $("#PropSanctionLengthBlockReportTable").jqGrid('setSelection', stateCode);
    $("#PropSanctionLengthFinalReportTable").jqGrid('GridUnload');


    $("#PropSanctionLengthFinalReportTable").jqGrid({
        url: '/ProposalReports/PropSanctionLengthFinalReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Sanctioned Year', 'Batch', 'Package Number',  'Road Name','Upagrade Connect','Total Road Length', 'BT Length ', 'CC Length ', 'Road Amount',
                   'Bridge Amount', 'Total Maintenance Cost ', 'Total Amount'],
        colModel: [
            { name: "MAST_BLOCK_NAME", width: 200, align: 'left',  height: 'auto', sortable: true },
            { name: "IMS_YEAR", width: 120, align: 'right',  height: 'auto', sortable: false },
            { name: "IMS_BATCH", width: 100, align: 'right',  height: 'auto', sortable: false },
            { name: "IMS_PACKAGE_ID", width: 150, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_ROAD_NAME", width: 200, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_UPGRADE_CONNECT", width: 150, align: 'left',  height: 'auto', sortable: false },
            { name: "TotalRoadLength", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "IMS_BT_LENGTH", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "IMS_CC_LENGTH", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "ROAD_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "BRIDGE_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
           { name: "MAINT_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "Year": year, "Batch": batch, "Collaboration": collaboration, "Status": status },
        pager: $("#PropSanctionLengthFinalReportPager"),
        footerrow: true,
        sortname: 'MAST_BLOCK_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '370',
        viewrecords: true,
        caption: 'Proposal Sanction Details for ' + blockName,
        loadComplete: function () {
            //Total of Columns
            var TotalRoadLength_T = $(this).jqGrid('getCol', 'TotalRoadLength', false, 'sum');
            TotalRoadLength_T = parseFloat(TotalRoadLength_T).toFixed(3);
            var IMS_BT_LENGTH_T = $(this).jqGrid('getCol', 'IMS_BT_LENGTH', false, 'sum');
            IMS_BT_LENGTH_T = parseFloat(IMS_BT_LENGTH_T).toFixed(3);
            var IMS_CC_LENGTH_T = $(this).jqGrid('getCol', 'IMS_CC_LENGTH', false, 'sum');
            IMS_CC_LENGTH_T = parseFloat(IMS_CC_LENGTH_T).toFixed(3);
            var ROAD_AMT_T = $(this).jqGrid('getCol', 'ROAD_AMT', false, 'sum');
            ROAD_AMT_T = parseFloat(ROAD_AMT_T).toFixed(2);
            var BRIDGE_AMT_T = $(this).jqGrid('getCol', 'BRIDGE_AMT', false, 'sum');
            BRIDGE_AMT_T = parseFloat(BRIDGE_AMT_T).toFixed(2);
                   var MAINT_AMT_T = $(this).jqGrid('getCol', 'MAINT_AMT', false, 'sum');
            MAINT_AMT_T = parseFloat(MAINT_AMT_T).toFixed(2);
            var TOTAL_AMT_T = $(this).jqGrid('getCol', 'TOTAL_AMT', false, 'sum');
            TOTAL_AMT_T = parseFloat(TOTAL_AMT_T).toFixed(2);
            ////

            $(this).jqGrid('footerData', 'set', { MAST_BLOCK_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TotalRoadLength: TotalRoadLength_T }, true);
            $(this).jqGrid('footerData', 'set', { IMS_BT_LENGTH: IMS_BT_LENGTH_T }, true);
            $(this).jqGrid('footerData', 'set', { IMS_CC_LENGTH: IMS_CC_LENGTH_T }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_AMT: ROAD_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { BRIDGE_AMT: BRIDGE_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT_AMT: MAINT_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMT: TOTAL_AMT_T }, true);
            $("#PropSanctionLengthFinalReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Amount Rs in Lacs & Length in Kms.</font>");
            $('#PropSanctionLengthFinalReportTable_rn').html('Sr.<br/>No.');

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
    //$("#PropSanctionLengthFinalReportTable").jqGrid('setGroupHeaders', {
    //    useColSpanStyle: false,
    //    groupHeaders: [
    //      { startColumnName: 'IMS_SANCTIONED_MAN_AMT1', numberOfColumns: 5, titleText: '<em> Sanctioned Maintenance Cost</em>' },

    //    ]
    //});
}
/**/