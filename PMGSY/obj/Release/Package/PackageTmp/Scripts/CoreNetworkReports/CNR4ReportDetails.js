$(document).ready(function () {
    

    $("#btCNR4Details").click(function () {

        var route = $("#Route_CNR4Details").val();
        var roadCategory = $("#RoadCategory_CNR4Details").val();

        if ($("#hdnLevelId").val() == 6) //mord
        {
          
            CNR4StateReportListing(route, roadCategory);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
           
            CNR4DistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), route, roadCategory);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
         
            CNR4BlockReportListing($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), route, roadCategory);
        }
    });

    $("#btCNR4Details").trigger('click');

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

});


/*       STATE REPORT LISTING       */
function CNR4StateReportListing(route, roadCategory) {
    $("#CNR4StateReportTable").jqGrid('GridUnload');
    $("#CNR4DistrictReportTable").jqGrid('GridUnload');
    $("#CNR4BlockReportTable").jqGrid('GridUnload');
    $("#CNR4FinalReportTable").jqGrid('GridUnload');
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNR4StateReportTable").jqGrid({
        url: '/CoreNetworkReports/CNR4StateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Number of CN(Full length considered)', 'Number of CN(Partial length considered)', 'Number of CN(Not yet consider in proposal)', 'Number of CN(Full length considered) ', 'Number of CN(Partial length considered)', 'Number of CN(Not yet consider in proposal) '],
        colModel: [
            { name: 'StateName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'Total_TRF', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Total_TRP', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'PTR', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Total_LRF', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Total_LRP', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'PLR', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } }
       
        ],
        postData: { "Route": route, "RoadCategory": roadCategory },
        pager: $("#CNR4StateReportPager"),
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'StateName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '510',
        viewrecords: true,
        caption: 'State Core Network Details',
        loadComplete: function () {
            //Total of Columns
            var Total_TRFT = $(this).jqGrid('getCol', 'Total_TRF', false, 'sum');
            Total_TRFT = parseFloat(Total_TRFT).toFixed(3);
            var Total_TRPT = $(this).jqGrid('getCol', 'Total_TRP', false, 'sum');
            Total_TRPT = parseFloat(Total_TRPT).toFixed(3);
            var PTRT = $(this).jqGrid('getCol', 'PTR', false, 'sum');
            PTRT = parseFloat(PTRT).toFixed(3);
            var Total_LRFT = $(this).jqGrid('getCol', 'Total_LRF', false, 'sum');
            Total_LRFT = parseFloat(Total_LRFT).toFixed(3);
            var Total_LRPT = $(this).jqGrid('getCol', 'Total_LRP', false, 'sum');
            Total_LRPT = parseFloat(Total_LRPT).toFixed(3);
            var PLRT = $(this).jqGrid('getCol', 'PLR', false, 'sum');
            PLRT = parseFloat(PLRT).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { Total_TRF: Total_TRFT }, true);
            $(this).jqGrid('footerData', 'set', { Total_TRP: Total_TRPT }, true);
            $(this).jqGrid('footerData', 'set', { PTR: PTRT }, true);
            $(this).jqGrid('footerData', 'set', { Total_LRF: Total_LRFT }, true);
            $(this).jqGrid('footerData', 'set', { Total_LRP: Total_LRPT }, true);
            $(this).jqGrid('footerData', 'set', { PLR: PLRT }, true);
            $("#CNR4StateReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#CNR4StateReportTable_rn').html('Sr.<br/>No.');

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

    $("#CNR4StateReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'Total_TRF', numberOfColumns: 3, titleText: '<em>Through Route</em>' },
          { startColumnName: 'Total_LRF', numberOfColumns: 3, titleText: '<em>Link Route </em>' }       

        ]
    });
}
/**/

/*       DISTRICT REPORT LISTING       */
function CNR4DistrictReportListing(stateCode, stateName, route, roadCategory) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNR4StateReportTable").jqGrid('setSelection', stateCode);
    $("#CNR4StateReportTable").jqGrid('setGridState', 'hidden');
    $("#CNR4DistrictReportTable").jqGrid('GridUnload');
    $("#CNR4BlockReportTable").jqGrid('GridUnload');
    $("#CNR4FinalReportTable").jqGrid('GridUnload');

    $("#CNR4DistrictReportTable").jqGrid({
        url: '/CoreNetworkReports/CNR4DistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Number of CN(Full length considered)', 'Number of CN(Partial length considered)', 'Number of CN(Not yet consider in proposal)', 'Number of CN(Full length considered) ', 'Number of CN(Partial length considered)', 'Number of CN(Not yet consider in proposal) '],
        colModel: [
            { name: 'DistrictName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'Total_TRF', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Total_TRP', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'PTR', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Total_LRF', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Total_LRP', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'PLR', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } }

        ],

        pager: $("#CNR4DistrictReportPager"),
        postData: { 'StateCode': stateCode, "Route": route, "RoadCategory": roadCategory },
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'DistrictName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '460',
        viewrecords: true,
        caption: 'District Core Network Details for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var Total_TRFT = $(this).jqGrid('getCol', 'Total_TRF', false, 'sum');
            Total_TRFT = parseFloat(Total_TRFT).toFixed(3);
            var Total_TRPT = $(this).jqGrid('getCol', 'Total_TRP', false, 'sum');
            Total_TRPT = parseFloat(Total_TRPT).toFixed(3);
            var PTRT = $(this).jqGrid('getCol', 'PTR', false, 'sum');
            PTRT = parseFloat(PTRT).toFixed(3);
            var Total_LRFT = $(this).jqGrid('getCol', 'Total_LRF', false, 'sum');
            Total_LRFT = parseFloat(Total_LRFT).toFixed(3);
            var Total_LRPT = $(this).jqGrid('getCol', 'Total_LRP', false, 'sum');
            Total_LRPT = parseFloat(Total_LRPT).toFixed(3);
            var PLRT = $(this).jqGrid('getCol', 'PLR', false, 'sum');
            PLRT = parseFloat(PLRT).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { Total_TRF: Total_TRFT }, true);
            $(this).jqGrid('footerData', 'set', { Total_TRP: Total_TRPT }, true);
            $(this).jqGrid('footerData', 'set', { PTR: PTRT }, true);
            $(this).jqGrid('footerData', 'set', { Total_LRF: Total_LRFT }, true);
            $(this).jqGrid('footerData', 'set', { Total_LRP: Total_LRPT }, true);
            $(this).jqGrid('footerData', 'set', { PLR: PLRT }, true);
            $("#CNR4DistrictReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#CNR4DistrictReportTable_rn').html('Sr.<br/>No.');

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

    $("#CNR4DistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'Total_TRF', numberOfColumns: 3, titleText: '<em>Through Route</em>' },
          { startColumnName: 'Total_LRF', numberOfColumns: 3, titleText: '<em>Link Route </em>' }

        ]
    });
}
/**/

/*      BLOCK REPORT LISTING       */

function CNR4BlockReportListing(stateCode, districtCode, districtName, route, roadCategory) {

    var distname;
    if (districtName == '')
        distname = $("#DISTRICT_NAME").val();
    else
        distname = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNR4DistrictReportTable").jqGrid('setSelection', districtCode);
    $("#CNR4DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#CNR4StateReportTable").jqGrid('setGridState', 'hidden');
    $("#CNR4BlockReportTable").jqGrid('GridUnload');
    $("#CNR4FinalReportTable").jqGrid('GridUnload');

    $("#CNR4BlockReportTable").jqGrid({
        url: '/CoreNetworkReports/CNR4BlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Number of CN(Full length considered)', 'Number of CN(Partial length considered)', 'Number of CN(Not yet consider in proposal)', 'Number of CN(Full length considered) ', 'Number of CN(Partial length considered)', 'Number of CN(Not yet consider in proposal) '],
        colModel: [
            { name: 'BlockName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'Total_TRF', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Total_TRP', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'PTR', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Total_LRF', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Total_LRP', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'PLR', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } }

        ],
        pager: $("#CNR4BlockReportPager"),
        postData: { 'StateCode': stateCode, 'DistrictCode': districtCode, "Route": route, "RoadCategory": roadCategory },
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'BlockName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '420',
        viewrecords: true,
        caption: 'Block Core Network Details for ' + distname,
        loadComplete: function () {
            //Total of Columns
            var Total_TRFT = $(this).jqGrid('getCol', 'Total_TRF', false, 'sum');
            Total_TRFT = parseFloat(Total_TRFT).toFixed(3);
            var Total_TRPT = $(this).jqGrid('getCol', 'Total_TRP', false, 'sum');
            Total_TRPT = parseFloat(Total_TRPT).toFixed(3);
            var PTRT = $(this).jqGrid('getCol', 'PTR', false, 'sum');
            PTRT = parseFloat(PTRT).toFixed(3);
            var Total_LRFT = $(this).jqGrid('getCol', 'Total_LRF', false, 'sum');
            Total_LRFT = parseFloat(Total_LRFT).toFixed(3);
            var Total_LRPT = $(this).jqGrid('getCol', 'Total_LRP', false, 'sum');
            Total_LRPT = parseFloat(Total_LRPT).toFixed(3);
            var PLRT = $(this).jqGrid('getCol', 'PLR', false, 'sum');
            PLRT = parseFloat(PLRT).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { Total_TRF: Total_TRFT }, true);
            $(this).jqGrid('footerData', 'set', { Total_TRP: Total_TRPT }, true);
            $(this).jqGrid('footerData', 'set', { PTR: PTRT }, true);
            $(this).jqGrid('footerData', 'set', { Total_LRF: Total_LRFT }, true);
            $(this).jqGrid('footerData', 'set', { Total_LRP: Total_LRPT }, true);
            $(this).jqGrid('footerData', 'set', { PLR: PLRT }, true);
            $("#CNR4BlockReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#CNR4BlockReportTable_rn').html('Sr.<br/>No.');

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

    $("#CNR4BlockReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'Total_TRF', numberOfColumns: 3, titleText: '<em>Through Route</em>' },
          { startColumnName: 'Total_LRF', numberOfColumns: 3, titleText: '<em>Link Route </em>' }

        ]
    });
}

/*       FINAL BLOCK REPORT LISTING       */

function CNR4FinalReportListing(blockCode, districtCode, stateCode, blockName, route, roadCategory) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNR4BlockReportTable").jqGrid('setGridState', 'hidden');
    $("#CNR4DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#CNR4StateReportTable").jqGrid('setGridState', 'hidden');
    $("#CNR4BlockReportTable").jqGrid('setSelection', blockCode);
    $("#CNR4FinalReportTable").jqGrid('GridUnload');

    $("#CNR4FinalReportTable").jqGrid({
        url: '/CoreNetworkReports/CNR4FinalReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Road Code', 'Road Name', 'DRRP Road Code', 'DRRP Road Name', 'Road Length', 'Road From', 'Road To', 'Route Type', 'Length Covered (P/F) (KM)', 'Balance Length (KM)', 'No of Habitations'],
        colModel: [
            { name: 'PlannedRoadNumber', width: 300, align: 'left', height: 'auto', sortable: false },
            { name: 'PlannedRoadName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'DRRPRoadCode', width: 300, align: 'right', height: 'auto', sortable: false },
            { name: 'DRRPRoadName', width: 300, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadLength', width: 300, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'RoadFrom', width: 300, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadTo', width: 300, align: 'left', height: 'auto', sortable: false },
            { name: 'RouteType', width: 300, align: 'left', height: 'auto', sortable: false },
            { name: 'LengthCovered', width: 300, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'BalLength', width: 300, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'NoHablitisation', width: 300, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }
         
        ],

        postData: { "BlockCode": blockCode, "StateCode": stateCode, "DistrictCode": districtCode, "Route": route, "RoadCategory": roadCategory },
        rowNum: '2147483647',
        pager: $("#CNR4FinalReportPager"),
        footerrow: true,
        pgbuttons: true,
        sortname: 'PlannedRoadNumber',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '370',
        viewrecords: true,
        caption: 'Core Network Details for ' + blockName,
        loadComplete: function () {
            //Total of Columns
       
            var RoadLengthT = $(this).jqGrid('getCol', 'RoadLength', false, 'sum');
            RoadLengthT = parseFloat(RoadLengthT).toFixed(3);
            var LengthCoveredT = $(this).jqGrid('getCol', 'LengthCovered', false, 'sum');
            LengthCoveredT = parseFloat(LengthCoveredT).toFixed(3);
            var BalLengthT = $(this).jqGrid('getCol', 'BalLength', false, 'sum');
            BalLengthT = parseFloat(BalLengthT).toFixed(3);
            var NoHablitisationT = $(this).jqGrid('getCol', 'NoHablitisation', false, 'sum');
      
            $(this).jqGrid('footerData', 'set', { PlannedRoadNumber: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { RoadLength: RoadLengthT });
            $(this).jqGrid('footerData', 'set', { LengthCovered: LengthCoveredT });
            $(this).jqGrid('footerData', 'set', { BalLength: BalLengthT });
            $(this).jqGrid('footerData', 'set', { NoHablitisation: NoHablitisationT });
            $("#CNR4FinalReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#CNR4FinalReportTable_rn').html('Sr.<br/>No.');

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