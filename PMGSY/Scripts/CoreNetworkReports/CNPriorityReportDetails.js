$(document).ready(function () {

   



    $("#btCNPriorityDetails").click(function () {

        var priority = $("#ddPriority_CNPriorityDetails").val();
        var route = $("#Route_CNPriorityDetails").val();
        if ($("#hdnLevelId").val() == 6) //mord
        {
           
            CNPriorityStateReportListing(priority, route);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
          
            CNPriorityDistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), priority, route);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
           
            CNPriorityBlockReportListing($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), priority, route);
        }
    });
    $("#btCNPriorityDetails").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});


/*       STATE REPORT LISTING       */
function CNPriorityStateReportListing(priority, route) {

    $("#CNPriorityStateReportTable").jqGrid('GridUnload');
    $("#CNPriorityDistrictReportTable").jqGrid('GridUnload');
    $("#CNPriorityBlockReportTable").jqGrid('GridUnload');
    $("#CNPriorityFinalReportTable").jqGrid('GridUnload');
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNPriorityStateReportTable").jqGrid({
        url: '/CoreNetworkReports/CNPriorityStateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Number of CN ', 'Total Length (KM)'],
        colModel: [
            { name: 'StateName', width: 300, align: 'left', height: 'auto' },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                 ],
        postData: { "Priority": priority, "Route": route },
        pager: $("#CNPriorityStateReportPager"),
        footerrow: true,
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        loadonce: false,
        rowNum: '2147483647',
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'State Core Network Priority Details',
        loadComplete: function () {
            //Total of Columns
            var TOTAL_CNT = $(this).jqGrid('getCol', 'TOTAL_CN', false, 'sum');
            var TOTAL_LENT = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            TOTAL_LENT = parseFloat(TOTAL_LENT).toFixed(3);
            //

            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_CN: TOTAL_CNT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LENT }, true);
            $('#CNPriorityStateReportTable_rn').html('Sr.<br/>No.');
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
    });//end of grid

  
}
/**/

/*      DISTRICT REPORT LISTING       */

function CNPriorityDistrictReportListing(stateCode, stateName, priority, route) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNPriorityStateReportTable").jqGrid('setSelection', stateCode);
    $("#CNPriorityStateReportTable").jqGrid('setGridState', 'hidden');

    $("#CNPriorityDistrictReportTable").jqGrid('GridUnload');
    $("#CNPriorityBlockReportTable").jqGrid('GridUnload');
    $("#CNPriorityFinalReportTable").jqGrid('GridUnload');

    $("#CNPriorityDistrictReportTable").jqGrid({
        url: '/CoreNetworkReports/CNPriorityDistrictReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Number of CN ', 'Total Length (KM)'],
        colModel: [
            { name: 'MAST_DISTRICT_NAME', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
        ],

        postData: { "StateCode": stateCode, "Priority": priority, "Route": route },
        pager: $("#CNPriorityDistrictReportPager"),
        loadonce: false,
        footerrow: true,
        pgbuttons: true,
        sortname: 'MAST_DISTRICT_NAME',
        sortorder: 'asc',
        rowNum: '2147483647',
        rownumbers: true,
        autowidth: true,
        height: '550',
        viewrecords: true,
        caption: 'District Core Network Priority Details for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_CNT = $(this).jqGrid('getCol', 'TOTAL_CN', false, 'sum');
            var TOTAL_LENT = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            TOTAL_LENT = parseFloat(TOTAL_LENT).toFixed(3);
            //

            $(this).jqGrid('footerData', 'set', { MAST_DISTRICT_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_CN: TOTAL_CNT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LENT }, true);
            $('#CNPriorityDistrictReportTable_rn').html('Sr.<br/>No.');
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

/*      BLOCK REPORT LISTING       */

function CNPriorityBlockReportListing(stateCode, districtCode, districtName, priority, route) {

    var distname;
    if (districtName == '')
        distname = $("#DISTRICT_NAME").val();
    else
        distname = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNPriorityDistrictReportTable").jqGrid('setSelection', districtCode); //District
    $("#CNPriorityDistrictReportTable").jqGrid('setGridState', 'hidden'); //District
    $("#CNPriorityStateReportTable").jqGrid('setGridState', 'hidden'); //State

    $("#CNPriorityBlockReportTable").jqGrid('GridUnload');  //Block
    $("#CNPriorityFinalReportTable").jqGrid('GridUnload');

    $("#CNPriorityBlockReportTable").jqGrid({
        url: '/CoreNetworkReports/CNPriorityBlockReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Number of CN ', 'Total Length (KM)'],
        colModel: [
            { name: 'BlockName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
        ],

        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "Priority": priority, "Route": route },
        pager: $("#CNPriorityBlockReportPager"),
        loadonce: false,
        footerrow: true,
        pgbuttons: true,
        sortname: 'BlockName',
        sortorder: 'asc',
        rowNum: '2147483647',
        rownumbers: true,
        autowidth: true,
        height: '500',
        viewrecords: true,
        caption: 'Block Core Network Priority Details for ' + distname,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_CNT = $(this).jqGrid('getCol', 'TOTAL_CN', false, 'sum');           
            var TOTAL_LENT = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            TOTAL_LENT = parseFloat(TOTAL_LENT).toFixed(3);
            //

            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_CN: TOTAL_CNT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LENT }, true);
            $('#CNPriorityBlockReportTable_rn').html('Sr.<br/>No.');
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
    }); /*End of Grid */
  
}

/*       FINAL BLOCK REPORT LISTING       */

function CNPriorityFinalReportListing(blockCode, districtCode, stateCode, blockName, priority, route) {

    $("#CNPriorityBlockReportTable").jqGrid('setSelection', blockCode);
    $("#CNPriorityBlockReportTable").jqGrid('setGridState', 'hidden'); //block
    $("#CNPriorityStateReportTable").jqGrid('setGridState', 'hidden'); //State
    $("#CNPriorityDistrictReportTable").jqGrid('setGridState', 'hidden'); //District

    $("#CNPriorityFinalReportTable").jqGrid('GridUnload');

    $("#CNPriorityFinalReportTable").jqGrid({
        url: '/CoreNetworkReports/CNPriorityFinalReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        //colNames: ['Block Name', 'Total Length in KM', 'Total Count'],
        colNames: ['Road Code', 'Road Name', 'Route Type', 'Road length (KM)', 'Habitation Name', 'Habitations Connected (Y/N)', 'Total Population'],
        colModel: [
            { name: 'PlannedRoadNumber', width: 300, align: 'right', height: 'auto', sortable: false },
            { name: 'PlannedRoadName', width: 300, align: 'right', height: 'auto', sortable: true },
            { name: 'RouteType', width: 300, align: 'right', height: 'auto', sortable: false },
            { name: 'RoadLength', width: 300, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'HabName', width: 300, align: 'right', height: 'auto', sortable: false },
            { name: 'HabConnected', width: 300, align: 'right', height: 'auto', sortable: false },
            { name: 'TotHabitisation', width: 300, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        grouping: false,
        postData: { "BlockCode": blockCode, "StateCode": stateCode, "DistrictCode": districtCode, "Priority": priority, "Route": route },
        pager: $("#CNPriorityFinalBlockReportPager"),
        loadonce: false,
        footerrow: true,
        pgbuttons: true,
        sortname: 'PlannedRoadNumber',
        sortorder: 'asc',
        rowNum: '2147483647',
        rownumbers: true,
        autowidth: true,
        height: '450',
        viewrecords: true,
        caption: 'Core Network Priority Details for ' + blockName,
        loadComplete: function () {
            //Total of Columns
             var RoadLengthT = $(this).jqGrid('getCol', 'RoadLength', false, 'sum');
             RoadLengthT = parseFloat(RoadLengthT).toFixed(3);
             var TotHabitisationT = $(this).jqGrid('getCol', 'TotHabitisation', false, 'sum');


            $(this).jqGrid('footerData', 'set', { PlannedRoadNumber: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { RoadLength: RoadLengthT });
            $(this).jqGrid('footerData', 'set', { TotHabitisation: TotHabitisationT });
            $('#CNPriorityFinalReportTable_rn').html('Sr.<br/>No.');
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