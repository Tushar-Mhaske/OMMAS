$(document).ready(function () { 

   
   
    $("#btCNR1Details").click(function () {

        var route = $("#Route_CNR1Details").val();
        var roadCategory = $("#RoadCategory_CNR1Details").val();
        if ($("#hdnLevelId").val() == 6) //mord
        {
        
            CNR1StateReportListing(route, roadCategory);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
          
            CNR1DistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), route, roadCategory);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
         
            CNR1BlockReportListing($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), route, roadCategory);
        }
    });
    $("#btCNR1Details").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});


/*       STATE REPORT LISTING       */
function CNR1StateReportListing(route, roadCategory) {
    
    $("#CNR1StateReportTable").jqGrid('GridUnload');
    $("#CNR1DistrictReportTable").jqGrid('GridUnload');
    $("#CNR1BlockReportTable").jqGrid('GridUnload');
    $("#CNR1FinalReportTable").jqGrid('GridUnload');
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNR1StateReportTable").jqGrid({
        url: '/CoreNetworkReports/CNR1StateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Number', 'Length ', 'Number', 'Length  ', 'Number', 'Length  ', 'Number', 'Length '],
        colModel: [
            { name: 'StateName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'NHNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'NHLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'SHNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'SHLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MDRNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MDRLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'RRNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'RRLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "Route": route, "RoadCategory": roadCategory },
        pager: $("#CNR1StateReportPager"),
        footerrow: true,
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        loadonce: false,
        rowNum: '2147483647',
        rownumbers: true,
        autowidth: true,
        height: '520',
        viewrecords: true,
        caption: 'State Core Network Details',
        loadComplete: function () {
            //Total of Columns
            var NHNumberT = $(this).jqGrid('getCol', 'NHNumber', false, 'sum');
            var NHLengthT = $(this).jqGrid('getCol', 'NHLength', false, 'sum');
            NHLengthT = parseFloat(NHLengthT).toFixed(3);
            var SHNumberT = $(this).jqGrid('getCol', 'SHNumber', false, 'sum');
            var SHLengthT = $(this).jqGrid('getCol', 'SHLength', false, 'sum');
            SHLengthT = parseFloat(SHLengthT).toFixed(3);
            var MDRNumberT = $(this).jqGrid('getCol', 'MDRNumber', false, 'sum');
            var MDRLengthT = $(this).jqGrid('getCol', 'MDRLength', false, 'sum');
            MDRLengthT = parseFloat(MDRLengthT).toFixed(3);
            var RRNumberT = $(this).jqGrid('getCol', 'RRNumber', false, 'sum');
            var RRLengthT = $(this).jqGrid('getCol', 'RRLength', false, 'sum');
            RRLengthT = parseFloat(RRLengthT).toFixed(3);
            //

            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { NHNumber: NHNumberT }, true);
            $(this).jqGrid('footerData', 'set', { NHLength: NHLengthT }, true);
            $(this).jqGrid('footerData', 'set', { SHNumber: SHNumberT }, true);
            $(this).jqGrid('footerData', 'set', { SHLength: SHLengthT }, true);
            $(this).jqGrid('footerData', 'set', { MDRNumber: MDRNumberT }, true);
            $(this).jqGrid('footerData', 'set', { MDRLength: MDRLengthT }, true);
            $(this).jqGrid('footerData', 'set', { RRNumber: RRNumberT }, true);
            $(this).jqGrid('footerData', 'set', { RRLength: RRLengthT }, true);
            $("#CNR1StateReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#CNR1StateReportTable_rn').html('Sr.<br/>No.');
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

    $("#CNR1StateReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'NHNumber', numberOfColumns: 2, titleText: '<em> NH </em>' },
          { startColumnName: 'SHNumber', numberOfColumns: 2, titleText: '<em> SH </em>' },
          { startColumnName: 'MDRNumber', numberOfColumns: 2, titleText: '<em> MDR </em>' },
          { startColumnName: 'RRNumber', numberOfColumns: 2, titleText: '<em>RR and Others </em>' }
  
        ]
    });
}
/**/

/*      DISTRICT REPORT LISTING       */

function CNR1DistrictReportListing(stateCode, stateName, route, roadCategory) {
   
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNR1StateReportTable").jqGrid('setSelection', stateCode);
    $("#CNR1StateReportTable").jqGrid('setGridState', 'hidden');

    $("#CNR1DistrictReportTable").jqGrid('GridUnload');
    $("#CNR1BlockReportTable").jqGrid('GridUnload');
    $("#CNR1FinalReportTable").jqGrid('GridUnload');

    $("#CNR1DistrictReportTable").jqGrid({
        url: '/CoreNetworkReports/CNR1DistrictReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Number', 'Length ', 'Number', 'Length  ', 'Number', 'Length  ', 'Number', 'Length '],
        colModel: [
            { name: 'DistrictName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'NHNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'NHLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'SHNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'SHLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MDRNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MDRLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'RRNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'RRLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "Route": route, "RoadCategory": roadCategory },
        pager: $("#CNR1DistrictReportPager"),
        loadonce: false,
        footerrow: true,
        pgbuttons: true,
        sortname: 'MAST_DISTRICT_NAME',
        sortorder: 'asc',
       rowNum: '2147483647',
        rownumbers: true,
        autowidth: true,
        height: '480',
        viewrecords: true,
        caption: 'District Core Network Details for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var NHNumberT = $(this).jqGrid('getCol', 'NHNumber', false, 'sum');
            var NHLengthT = $(this).jqGrid('getCol', 'NHLength', false, 'sum');
            NHLengthT = parseFloat(NHLengthT).toFixed(3);
            var SHNumberT = $(this).jqGrid('getCol', 'SHNumber', false, 'sum');
            var SHLengthT = $(this).jqGrid('getCol', 'SHLength', false, 'sum');
            SHLengthT = parseFloat(SHLengthT).toFixed(3);
            var MDRNumberT = $(this).jqGrid('getCol', 'MDRNumber', false, 'sum');
            var MDRLengthT = $(this).jqGrid('getCol', 'MDRLength', false, 'sum');
            MDRLengthT = parseFloat(MDRLengthT).toFixed(3);
            var RRNumberT = $(this).jqGrid('getCol', 'RRNumber', false, 'sum');
            var RRLengthT = $(this).jqGrid('getCol', 'RRLength', false, 'sum');
            RRLengthT = parseFloat(RRLengthT).toFixed(3);        //

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { NHNumber: NHNumberT }, true);
            $(this).jqGrid('footerData', 'set', { NHLength: NHLengthT }, true);
            $(this).jqGrid('footerData', 'set', { SHNumber: SHNumberT }, true);
            $(this).jqGrid('footerData', 'set', { SHLength: SHLengthT }, true);
            $(this).jqGrid('footerData', 'set', { MDRNumber: MDRNumberT }, true);
            $(this).jqGrid('footerData', 'set', { MDRLength: MDRLengthT }, true);
            $(this).jqGrid('footerData', 'set', { RRNumber: RRNumberT }, true);
            $(this).jqGrid('footerData', 'set', { RRLength: RRLengthT }, true);
            $("#CNR1DistrictReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#CNR1DistrictReportTable_rn').html('Sr.<br/>No.');
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
    $("#CNR1DistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'NHNumber', numberOfColumns: 2, titleText: '<em> NH </em>' },
          { startColumnName: 'SHNumber', numberOfColumns: 2, titleText: '<em> SH </em>' },
          { startColumnName: 'MDRNumber', numberOfColumns: 2, titleText: '<em> MDR </em>' },
          { startColumnName: 'RRNumber', numberOfColumns: 2, titleText: '<em>RR and Others </em>' }
        ]
    });
}

/*      BLOCK REPORT LISTING       */

function CNR1BlockReportListing(stateCode, districtCode, districtName, route, roadCategory) {

    var distname;
    if (districtName == '')
        distname = $("#DISTRICT_NAME").val();
    else
        distname = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNR1DistrictReportTable").jqGrid('setSelection', districtCode); //District
    $("#CNR1DistrictReportTable").jqGrid('setGridState', 'hidden'); //District
    $("#CNR1StateReportTable").jqGrid('setGridState', 'hidden'); //State


    $("#CNR1BlockReportTable").jqGrid('GridUnload');  //Block
    $("#CNR1FinalReportTable").jqGrid('GridUnload');

    $("#CNR1BlockReportTable").jqGrid({
        url: '/CoreNetworkReports/CNR1BlockReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Number', 'Length ', 'Number', 'Length  ', 'Number', 'Length  ', 'Number', 'Length '],
        colModel: [
            { name: 'BlockName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'NHNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'NHLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'SHNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'SHLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MDRNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MDRLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'RRNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'RRLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "Route": route, "RoadCategory": roadCategory },
        pager: $("#CNR1BlockReportPager"),
        loadonce: false,
        footerrow: true,
        pgbuttons: true,
        sortname: 'BlockName',
        sortorder: 'asc',
       rowNum: '2147483647',
        rownumbers: true,
        autowidth: true,
        height: '420',
        viewrecords: true,
        caption: 'Block Core Network Details for ' + distname,
        loadComplete: function () {
            //Total of Columns
            var NHNumberT = $(this).jqGrid('getCol', 'NHNumber', false, 'sum');
            var NHLengthT = $(this).jqGrid('getCol', 'NHLength', false, 'sum');
            NHLengthT = parseFloat(NHLengthT).toFixed(3);
            var SHNumberT = $(this).jqGrid('getCol', 'SHNumber', false, 'sum');
            var SHLengthT = $(this).jqGrid('getCol', 'SHLength', false, 'sum');
            SHLengthT = parseFloat(SHLengthT).toFixed(3);
            var MDRNumberT = $(this).jqGrid('getCol', 'MDRNumber', false, 'sum');
            var MDRLengthT = $(this).jqGrid('getCol', 'MDRLength', false, 'sum');
            MDRLengthT = parseFloat(MDRLengthT).toFixed(3);
            var RRNumberT = $(this).jqGrid('getCol', 'RRNumber', false, 'sum');
            var RRLengthT = $(this).jqGrid('getCol', 'RRLength', false, 'sum');
            RRLengthT = parseFloat(RRLengthT).toFixed(3);
            //

            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { NHNumber: NHNumberT }, true);
            $(this).jqGrid('footerData', 'set', { NHLength: NHLengthT }, true);
            $(this).jqGrid('footerData', 'set', { SHNumber: SHNumberT }, true);
            $(this).jqGrid('footerData', 'set', { SHLength: SHLengthT }, true);
            $(this).jqGrid('footerData', 'set', { MDRNumber: MDRNumberT }, true);
            $(this).jqGrid('footerData', 'set', { MDRLength: MDRLengthT }, true);
            $(this).jqGrid('footerData', 'set', { RRNumber: RRNumberT }, true);
            $(this).jqGrid('footerData', 'set', { RRLength: RRLengthT }, true);
            $("#CNR1BlockReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#CNR1BlockReportTable_rn').html('Sr.<br/>No.');

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

    $("#CNR1BlockReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'NHNumber', numberOfColumns: 2, titleText: '<em> NH </em>' },
          { startColumnName: 'SHNumber', numberOfColumns: 2, titleText: '<em> SH </em>' },
          { startColumnName: 'MDRNumber', numberOfColumns: 2, titleText: '<em> MDR </em>' },
          { startColumnName: 'RRNumber', numberOfColumns: 2, titleText: '<em>RR and Others </em>' }
        ]
    });
}

/*       FINAL BLOCK REPORT LISTING       */

function CNR1FinalReportListing(blockCode, districtCode, stateCode, blockName, route, roadCategory) {
    
    $("#CNR1BlockReportTable").jqGrid('setSelection', blockCode);
    $("#CNR1BlockReportTable").jqGrid('setGridState', 'hidden'); //block
    $("#CNR1StateReportTable").jqGrid('setGridState', 'hidden'); //State
    $("#CNR1DistrictReportTable").jqGrid('setGridState', 'hidden'); //District

    $("#CNR1FinalReportTable").jqGrid('GridUnload');

    $("#CNR1FinalReportTable").jqGrid({
        url: '/CoreNetworkReports/CNR1FinalBlockReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        //colNames: ['Block Name', 'Total Length in KM', 'Total Count'],
        colNames: ['Road Code', 'Road Name', 'DRRP Road Code', 'DRRP Road Name', 'Road Length', 'Road From', 'Road To', 'Route Type', 'Length Covered (P/F)', 'No of Habitations', 'Total Population'],
        colModel: [
            { name: 'PlannedRoadNumber', width: 300, align: 'left', height: 'auto', sortable: false },
            { name: 'PlannedRoadName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'DRRPRoadCode', width: 300, align: 'right', height: 'auto', sortable: false },
            { name: 'DRRPRoadName', width: 300, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadLength', width: 300, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'RoadFrom', width: 300, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadTo', width: 300, align: 'left', height: 'auto', sortable: false },
            { name: 'RouteType', width: 300, align: 'left', height: 'auto', sortable: false },
            { name: 'LengthCovered', width: 300, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'NoHablitisation', width: 300, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TotHabitisation', width: 300, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        grouping: false,      
        postData: { "BlockCode": blockCode, "StateCode": stateCode, "DistrictCode": districtCode, "Route": route, "RoadCategory": roadCategory },
        pager: $("#CNR1FinalReportPager"),
        loadonce: false,
        footerrow: true,
        pgbuttons: true,
        sortname: 'PlannedRoadNumber',
        sortorder: 'asc',
       rowNum: '2147483647',
        rownumbers: true,
        autowidth: true,
        height: '380',
        viewrecords: true,
        caption: 'Core Network Details for ' + blockName,
        loadComplete: function () {
            //Total of Columns
            var PlannedRoadlengthT = $(this).jqGrid('getCol', 'PlannedRoadlength', false, 'sum');
            var RoadLengthT = $(this).jqGrid('getCol', 'RoadLength', false, 'sum');
            RoadLengthT = parseFloat(RoadLengthT).toFixed(3);
            var LengthCoveredT = $(this).jqGrid('getCol', 'LengthCovered', false, 'sum');
            var NoHablitisationT = $(this).jqGrid('getCol', 'NoHablitisation', false, 'sum');
            var TotHabitisationT = $(this).jqGrid('getCol', 'TotHabitisation', false, 'sum');
          

            $(this).jqGrid('footerData', 'set', { PlannedRoadNumber: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { RoadLength: RoadLengthT });
            $(this).jqGrid('footerData', 'set', { LengthCovered: LengthCoveredT });
            $(this).jqGrid('footerData', 'set', { NoHablitisation: NoHablitisationT });
            $(this).jqGrid('footerData', 'set', { TotHabitisation: TotHabitisationT });
            $("#CNR1FinalBlockReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#CNR1FinalReportTable_rn').html('Sr.<br/>No.');

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