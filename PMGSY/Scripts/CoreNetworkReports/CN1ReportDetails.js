$(document).ready(function () {
   

    $("#btCN1Details").click(function () {

        var route = $("#Route_CN1Details").val();

        if ($("#hdnLevelId").val() == 6) //mord
        {
            
            CN1StateReportListing(route);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
       
            CN1DistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), route);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
           
            CN1BlockReportListing($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), route);
        }
    });

    $("#btCN1Details").trigger("click");
    //$('#tblRptContents').bind('resize', function () {
    //    resizeJqGrid();
    //}).trigger('resize');
});


/*       STATE REPORT LISTING       */
function CN1StateReportListing(route) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CN1ReportTable").jqGrid('GridUnload');
    $("#CN1DistrictReportTable").jqGrid('GridUnload');
    $("#CN1BlockReportTable").jqGrid('GridUnload');
    $("#CN1FinalReportTable").jqGrid('GridUnload');
    $("#CN1ReportTable").jqGrid({
        url: '/CoreNetworkReports/CN1ReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Total Core Network Roads', 'Total Length', 'BT ', 'WBM ', 'Gravel ', 'Track ', 'Other '],
        colModel: [

            { name: 'StateName', width: 180, align: 'left', height: 'auto', sortable: true },
            { name: 'TotalCount', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TotalLength', width: 115, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'BTLength', width: 115, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBMLength', width: 115, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GravelLength', width: 115, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TrackLength', width: 115, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'OtherLength', width: 130, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },

        ],
        postData: {"Route": route },
        pager: $("#CN1ReportPager"),
        footerrow: true,
        pgbuttons: true,
        sortname: 'StateName',
        sortorder: 'asc',
        loadonce: false,
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: false,
        shrinkToFit: false,
        width: 1100,
        height: '520',
        viewrecords: true,
        caption: 'State Core Network Details',
        loadComplete: function () {
            //Total of Columns
            var TotalCountT = $(this).jqGrid('getCol', 'TotalCount', false, 'sum');
            var TotalLengthT = $(this).jqGrid('getCol', 'TotalLength', false, 'sum');
            TotalLengthT = parseFloat(TotalLengthT).toFixed(3);
            var BTLengthT = $(this).jqGrid('getCol', 'BTLength', false, 'sum');
            BTLengthT = parseFloat(BTLengthT).toFixed(3);
            var WBMLengthT = $(this).jqGrid('getCol', 'WBMLength', false, 'sum');
            WBMLengthT = parseFloat(WBMLengthT).toFixed(3);
            var GravelLengthT = $(this).jqGrid('getCol', 'GravelLength', false, 'sum');
            GravelLengthT = parseFloat(GravelLengthT).toFixed(3);
            var TrackLengthT = $(this).jqGrid('getCol', 'TrackLength', false, 'sum');
            TrackLengthT = parseFloat(TrackLengthT).toFixed(3);
            var OtherLengthT = $(this).jqGrid('getCol', 'OtherLength', false, 'sum');
            OtherLengthT = parseFloat(OtherLengthT).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TotalCount: TotalCountT }, true);
            $(this).jqGrid('footerData', 'set', { TotalLength: TotalLengthT }, true);
            $(this).jqGrid('footerData', 'set', { BTLength: BTLengthT }, true);
            $(this).jqGrid('footerData', 'set', { WBMLength: WBMLengthT }, true);
            $(this).jqGrid('footerData', 'set', { GravelLength: GravelLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TrackLength: TrackLengthT }, true);
            $(this).jqGrid('footerData', 'set', { OtherLength: OtherLengthT }, true);
            $('#CN1ReportTable_rn').html('Sr.<br/>No.');
            $("#CN1ReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");

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

    $("#CN1ReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'BTLength', numberOfColumns: 5, titleText: '<em>Existing Surface Type</em>' }

        ]

    });
}
/**/

/*      DISTRICT REPORT LISTING       */

function CN1DistrictReportListing(stateCode, stateName, route) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CN1ReportTable").jqGrid('setSelection', stateCode);
    $("#CN1ReportTable").jqGrid('setGridState', 'hidden');
    $("#CN1DistrictReportTable").jqGrid('GridUnload');
    $("#CN1BlockReportTable").jqGrid('GridUnload');
    $("#CN1FinalReportTable").jqGrid('GridUnload');

    $("#CN1DistrictReportTable").jqGrid({
        url: '/CoreNetworkReports/CN1DistrictReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Total Core Network Roads', 'Total Length', 'BT ', 'WBM ', 'Gravel ', 'Track ', 'Other '],
        colModel: [
            { name: 'DistrictName', width: 180, align: 'left', height: 'auto', sortable: true },
            { name: 'TotalCount', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TotalLength', width: 115, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'BTLength', width: 115, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBMLength', width: 115, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GravelLength', width: 115, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TrackLength', width: 115, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'OtherLength', width: 130, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },

        ],
        postData: { "StateCode": stateCode,"Route":route },
        pager: $("#CN1DistrictReportPager"),
      //  recordtext: '{2} records found',
        loadonce: false,
        footerrow: true,
        pgbuttons: true,
        sortname: 'MAST_DISTRICT_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: false,
        shrinkToFit: false,
        width: 1100,
        height: '460',
        viewrecords: true,
        caption: 'District Core Network Details for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var TotalCountT = $(this).jqGrid('getCol', 'TotalCount', false, 'sum');
            var TotalLengthT = $(this).jqGrid('getCol', 'TotalLength', false, 'sum');
            TotalLengthT = parseFloat(TotalLengthT).toFixed(3);
            var BTLengthT = $(this).jqGrid('getCol', 'BTLength', false, 'sum');
            BTLengthT = parseFloat(BTLengthT).toFixed(3);
            var WBMLengthT = $(this).jqGrid('getCol', 'WBMLength', false, 'sum');
            WBMLengthT = parseFloat(WBMLengthT).toFixed(3);
            var GravelLengthT = $(this).jqGrid('getCol', 'GravelLength', false, 'sum');
            GravelLengthT = parseFloat(GravelLengthT).toFixed(3);
            var TrackLengthT = $(this).jqGrid('getCol', 'TrackLength', false, 'sum');
            TrackLengthT = parseFloat(TrackLengthT).toFixed(3);
            var OtherLengthT = $(this).jqGrid('getCol', 'OtherLength', false, 'sum');
            OtherLengthT = parseFloat(OtherLengthT).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TotalLength: TotalLengthT }, true);
            $(this).jqGrid('footerData', 'set', { BTLength: BTLengthT }, true);
            $(this).jqGrid('footerData', 'set', { WBMLength: WBMLengthT }, true);
            $(this).jqGrid('footerData', 'set', { GravelLength: GravelLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TrackLength: TrackLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TotalCount: TotalCountT }, true);
            $(this).jqGrid('footerData', 'set', { OtherLength: OtherLengthT }, true);
            $('#CN1DistrictReportTable_rn').html('Sr.<br/>No.');
            $("#CN1DistrictReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");

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

    $("#CN1DistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'BTLength', numberOfColumns: 5, titleText: '<em>Existing Surface Type</em>' }

        ]

    });
}

/*      BLOCK REPORT LISTING       */

function CN1BlockReportListing(stateCode, districtCode, districtName, route) {

    var distname;
    if (districtName == '')
        distname = $("#DISTRICT_NAME").val();
    else
        distname = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CN1DistrictReportTable").jqGrid('setSelection', districtCode); //District
    $("#CN1DistrictReportTable").jqGrid('setGridState', 'hidden'); //District
    $("#CN1ReportTable").jqGrid('setGridState', 'hidden'); //State
    $("#CN1BlockReportTable").jqGrid('GridUnload');  //Block
    $("#CN1FinalReportTable").jqGrid('GridUnload');

    $("#CN1BlockReportTable").jqGrid({
        url: '/CoreNetworkReports/CN1BlockReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Total Core Network Roads', 'Total Length', 'BT ', 'WBM ', 'Gravel ', 'Track ', 'Other '],
        colModel: [
            { name: 'BlockName', width: 180, align: 'left', height: 'auto', sortable: true },
            { name: 'TotalCount', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TotalLength', width: 115, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'BTLength', width: 115, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBMLength', width: 115, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GravelLength', width: 115, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TrackLength', width: 115, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'OtherLength', width: 130, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },

        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "Route": route },
        pager: $("#CN1BlockReportPager"),
      //  recordtext: '{2} records found',
        loadonce: false,
        footerrow: true,
        pgbuttons: true,
        sortname: 'MAST_BLOCK_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: false,
        shrinkToFit: false,
        width: 1100,
        height: '410',
        viewrecords: true,
        caption: 'Block Core Network Details for ' + distname,
        loadComplete: function () {
            //Total of Columns
            var TotalCountT = $(this).jqGrid('getCol', 'TotalCount', false, 'sum');
            var TotalLengthT = $(this).jqGrid('getCol', 'TotalLength', false, 'sum');
            TotalLengthT = parseFloat(TotalLengthT).toFixed(3);
            var BTLengthT = $(this).jqGrid('getCol', 'BTLength', false, 'sum');
            BTLengthT = parseFloat(BTLengthT).toFixed(3);
            var WBMLengthT = $(this).jqGrid('getCol', 'WBMLength', false, 'sum');
            WBMLengthT = parseFloat(WBMLengthT).toFixed(3);
            var GravelLengthT = $(this).jqGrid('getCol', 'GravelLength', false, 'sum');
            GravelLengthT = parseFloat(GravelLengthT).toFixed(3);
            var TrackLengthT = $(this).jqGrid('getCol', 'TrackLength', false, 'sum');
            TrackLengthT = parseFloat(TrackLengthT).toFixed(3);
            var OtherLengthT = $(this).jqGrid('getCol', 'OtherLength', false, 'sum');
            OtherLengthT = parseFloat(OtherLengthT).toFixed(3);
            //

            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TotalLength: TotalLengthT }, true);
            $(this).jqGrid('footerData', 'set', { BTLength: BTLengthT }, true);
            $(this).jqGrid('footerData', 'set', { WBMLength: WBMLengthT }, true);
            $(this).jqGrid('footerData', 'set', { GravelLength: GravelLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TrackLength: TrackLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TotalCount: TotalCountT }, true);
            $(this).jqGrid('footerData', 'set', { OtherLength: OtherLengthT }, true);
            $('#CN1BlockReportTable_rn').html('Sr.<br/>No.');
            $("#CN1BlockReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");

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

    $("#CN1BlockReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'BTLength', numberOfColumns: 5, titleText: '<em>Existing Surface Type</em>' }

        ]

    });
}

/*       FINAL BLOCK REPORT LISTING       */

/* Old Report */
function CN1FinalReportListing(blockCode, districtCode, stateCode, blockName, route) {

    $("#CN1BlockReportTable").jqGrid('setSelection', blockCode);
    $("#CN1BlockReportTable").jqGrid('setGridState', 'hidden'); //block
    $("#CN1ReportTable").jqGrid('setGridState', 'hidden'); //State
    $("#CN1DistrictReportTable").jqGrid('setGridState', 'hidden'); //District

    $("#CN1FinalReportTable").jqGrid('GridUnload');

    $("#CN1FinalReportTable").jqGrid({
        url: '/CoreNetworkReports/CN1FinalBlockReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Unique Road ID', 'Road Number', 'Road From', 'Road To', 'Total Length ', 'Length ', 'Condition', 'Length ', 'Condition', 'Length ', 'Condition', 'Length ', 'Condition', 'Total Population served', 'Name', 'Population', 'Connected (Yes/No)', 'KML File'], //'Total Population Served'

        colModel: [
            { name: 'UniqueRoadId', width: 180, align: 'left', height: 'auto', sortable: true },
            { name: 'PlannedRoadNumber', width: 100, align: 'left', height: 'auto', sortable: false },
            { name: 'PlannedRoadfrom', width: 100, align: 'left', height: 'auto', search: false, sortable: false },
            { name: 'PlannedRoadto', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'PlannedRoadlength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'BTLengthinKM', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'BTConinKM', width: 100, align: 'center', height: 'auto', sortable: false },
            { name: 'WBMLengthinKM', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBMConinKM', width: 100, align: 'center', height: 'auto', sortable: false },
            { name: 'GravelLengthinKM', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GravelConinKM', width: 100, align: 'center', height: 'auto', sortable: false },
            { name: 'TrackLengthinKM', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TrackConinKM', width: 100, align: 'center', height: 'auto', sortable: false },
            { name: 'TotalPopulationServed', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'HabitationName', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'HabTotPop', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'HabitationConnected', width: 70, align: 'center', height: 'auto', sortable: false },
             { name: 'DownloadKMLfile', width: 100, align: 'right', height: 'auto', sortable: false }
        ],
        //grouping: true,
        //groupingView: {
        //    //groupField: ['PlannedRoadNumber', 'PlannedRoadName', 'PlannedRoadfrom', 'PlannedRoadto', 'PlannedRoadlength', 'BTLengthinKM', 'WBMLengthinKM', 'GravelLengthinKM', 'TrackLengthinKM'],
        //    groupField: ['PlannedRoadNumber'],
        //    groupSummary: [true],
        //    groupColumnShow: [false],
        //    groupDataSorted: true
        //},
        postData: { "BlockCode": blockCode, "StateCode": stateCode, "DistrictCode": districtCode, "Route": route },
        pager: $("#CN1FinalReportPager"),
        footerrow: true,
        pgbuttons: true,         
        rowNum: 999999,       
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "PlannedRoadNumber",
        sortorder: "asc",
        caption: 'Core Network Details for ' + blockName,
        height: 'auto',
        height: '380',
        autowidth: false,
        shrinkToFit: false,
        width: 1100,
        rownumbers: true,         
        loadComplete: function () {
            //Total of Columns
            var PlannedRoadlengthT = $(this).jqGrid('getCol', 'PlannedRoadlength', false, 'sum');
            PlannedRoadlengthT = parseFloat(PlannedRoadlengthT).toFixed(2);
            var BTLengthT = $(this).jqGrid('getCol', 'BTLengthinKM', false, 'sum');
            BTLengthT = parseFloat(BTLengthT).toFixed(3);
            var WBMLengthT = $(this).jqGrid('getCol', 'WBMLengthinKM', false, 'sum');
            WBMLengthT = parseFloat(WBMLengthT).toFixed(3);
            var GravelLengthT = $(this).jqGrid('getCol', 'GravelLengthinKM', false, 'sum');
            GravelLengthT = parseFloat(GravelLengthT).toFixed(3);
            var TrackLengthT = $(this).jqGrid('getCol', 'TrackLengthinKM', false, 'sum');
            TrackLengthT = parseFloat(TrackLengthT).toFixed(3);
            var TotalPopulationofHabitationT = $(this).jqGrid('getCol', 'TotalPopulationofHabitation', false, 'sum');
            var TotalPopulationServedT = $(this).jqGrid('getCol', 'TotalPopulationServed', false, 'sum');
            var HabTotPopT = $(this).jqGrid('getCol', 'HabTotPop', false, 'sum');

           // var DownloadKMLfileT = $(this).jqGrid('getCol', 'DownloadKMLfile', false, 'sum');
         
            //

            $(this).jqGrid('footerData', 'set', { PlannedRoadNumber: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { PlannedRoadlength: PlannedRoadlengthT });
            $(this).jqGrid('footerData', 'set', { BTLengthinKM: BTLengthT });
            $(this).jqGrid('footerData', 'set', { WBMLengthinKM: WBMLengthT });
            $(this).jqGrid('footerData', 'set', { GravelLengthinKM: GravelLengthT });
            $(this).jqGrid('footerData', 'set', { TrackLengthinKM: TrackLengthT });
            $(this).jqGrid('footerData', 'set', { TotalPopulationofHabitation: TotalPopulationofHabitationT });
            $(this).jqGrid('footerData', 'set', { TotalPopulationServed: TotalPopulationServedT });
            $(this).jqGrid('footerData', 'set', { TotalPopulationServed: TotalPopulationServedT });
            $(this).jqGrid('footerData', 'set', { HabTotPop: HabTotPopT });

           // $(this).jqGrid('footerData', 'set', { DownloadKMLfile: DownloadKMLfileT });
            $('#CN1FinalReportTable_rn').html('Sr.<br/>No.');
            $("#CN1FinalReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");

            $.unblockUI();
        },
        //gridComplete: function () {

        //    var ids = jQuery("#CN1FinalReportTable").jqGrid('getDataIDs');
        //    var previousId = 0;

        //    for (var i = 0; i < ids.length; i++) {
        //        var rowId = ids[i];
        //        var rowData = jQuery('#CN1FinalReportTable').jqGrid('getRowData', rowId);              
        //        var newTotalPop = parseFloat(rowData.PlannedRoadlength) + parseFloat(rowData.BTLengthinKM) + parseFloat(rowData.WBMLengthinKM) + parseFloat(rowData.GravelLengthinKM) + parseFloat(rowData.TrackLengthinKM);
        //        $("#CN1FinalReportTable").jqGrid('setCell', rowId, 'TotalPopulationServed', parseFloat(newTotalPop).toFixed(2).toString().toLocaleString("en-IN"));

        //    }
        //},
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        },
        resizeStop: function () {
            var $self = $(this),
                shrinkToFit = $self.jqGrid("getGridParam", "shrinkToFit");

            $self.jqGrid("setGridWidth", this.grid.newWidth, shrinkToFit);
            setHeaderWidth.call(this);
        }
    });/*End of Grid*/

    $("#CN1FinalReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          //{ startColumnName: 'BTLengthinKM', numberOfColumns:2, titleText: '<em>BT</em>' },
          //{ startColumnName: 'WBMLengthinKM', numberOfColumns: 2, titleText: '<em>WBM</em>' },
          //{ startColumnName: 'GravelLengthinKM', numberOfColumns: 2, titleText: '<em>Gravel</em>' },
          //{ startColumnName: 'TrackLengthinKM', numberOfColumns: 2, titleText: '<em>Track</em>' },
            {
                  startColumnName: 'BTLengthinKM', numberOfColumns: 8,
                  titleText: '<table style="width:100%;border-spacing:0px"' +
                            '<tr><td id="h0" colspan="8">Existing Surface Type</td></tr>' +
                            '<tr>' +
                                '<td id="h1" colspan="2" style="width:24.5%">BT</td>' +
                                '<td id="h2" colspan="2" style="width:24.5%">WBM</td>' +
                                '<td id="h3" colspan="2" style="width:24.5%">Gravel</td>' +
                                '<td id="h4" colspan="2" style="width:24.5%">Track</td>' +
                            '</tr>' +
                            '</table>'
            },

             {
                 startColumnName: 'HabitationName', numberOfColumns: 3,
                 titleText: '<table style="width:100%;border-spacing:0px"' +
                           '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="3">Habitations Served</td></tr>' +
                            '<tr>' +
                                '<td id="h1" colspan="3" style="width:100%">Directly</td>' +                               
                            '</tr>' +
                           '</table>'
             },
           // { startColumnName: 'HabitationName', numberOfColumns: 3, titleText: '<em>Habitation Served </em>' },
        ]

    });
    $("th[title=CN1Report]").removeAttr("title");
    $("#h0").css({
        borderBottomWidth: "1px",
        borderBottomColor: "inherit", // the color from jQuery UI which you use
        borderBottomStyle: "solid",
        padding: "4px 0 6px 0"
    });
    $("#h1").css({
        borderRightWidth: "1px",
        borderRightColor: "inherit", // the color from jQuery UI which you use
        borderRightStyle: "solid",
        padding: "4px 0 4px 0"
    });
    $("#h2").css({
        borderRightWidth: "1px",
        borderRightColor: "inherit", // the color from jQuery UI which you use
        borderRightStyle: "solid",
        padding: "4px 0 4px 0"
    });
    $("#h3").css({
        borderRightWidth: "1px",
        borderRightColor: "inherit", // the color from jQuery UI which you use
        borderRightStyle: "solid",
        padding: "4px 0 4px 0"
    });
    $("#h4").css({
        padding: "4px 0 4px 0"
    });
    setHeaderWidth.call(grid[0]);
}

var grid = $("#CN1FinalReportTable"),
setHeaderWidth = function () {
    var $self = $(this),
        colModel = $self.jqGrid("getGridParam", "colModel"),
        cmByName = {},
        ths = this.grid.headers, // array with column headers
        cm,
        i,
        l = colModel.length;

    // save width of every column header in cmByName map
    // to make easy access there by name
    for (i = 0; i < l; i++) {
        cm = colModel[i];
        cmByName[cm.name] = $(ths[i].el).outerWidth();
    }
    // resize headers of additional columns based on the size of
    // the columns below the header
    //$("#h1").width(cmByName.No + cmByName.Date + cmByName.total - 1);
    $("#h1").width(cmByName.No + cmByName.Date - 1);
    //$("#h2").width(cmByName.in_Rs - 1);
    //$("#h3").width(cmByName.in_Rs - 1);
    //$("#h4").width(cmByName.in_Rs - 1);
};


/*End Old Report*/
////////////////////////////////////////////////////////////////////
///* Start New Report */

//function CN1FinalReportListing(blockCode, districtCode, stateCode, blockName,route) {

//    $("#CN1BlockReportTable").jqGrid('setSelection', blockCode);
//    $("#CN1BlockReportTable").jqGrid('setGridState', 'hidden'); //block
//    $("#CN1ReportTable").jqGrid('setGridState', 'hidden'); //State
//    $("#CN1DistrictReportTable").jqGrid('setGridState', 'hidden'); //District

//    $("#CN1FinalReportTable").jqGrid('GridUnload');

//    $("#CN1FinalReportTable").jqGrid({
//        url: '/CoreNetworkReports/CN1FinalBlockReportListing?' + Math.random(),
//        datatype: 'json',
//        mtype: 'POST',
//        colNames: ['Unique Road ID', 'Road Number', 'Road From', 'Road To', 'Total Length ', 'Length ', 'Condition', 'Length ', 'Condition', 'Length ', 'Condition', 'Length ', 'Condition', 'Total Population served', 'Name', 'Population', 'Connected (Yes/No)', 'KML File'], //'Total Population Served'

//        colModel: [
//            //{ name: 'UniqueRoadId', width: 180, align: 'left', height: 'auto', sortable: true,cellattr: arrtSetting0 },
//            //{ name: 'PlannedRoadNumber', width: 150, align: 'right', height: 'auto', cellattr: arrtSetting1 },
//            //{ name: 'PlannedRoadfrom', width: 120, align: 'right', height: 'auto', search: false, sortable: false, cellattr: arrtSetting2 },
//            //{ name: 'PlannedRoadto', width: 120, align: 'right', height: 'auto', cellattr: arrtSetting3 },
//            //{ name: 'PlannedRoadlength', width: 120, align: 'right', height: 'auto', formatter: 'number',cellattr: arrtSetting4 , formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2, defaulValue: "N.A" } },
//            //{ name: 'BTLengthinKM', width: 120, align: 'right', height: 'auto', formatter: 'number',cellattr: arrtSetting5 , formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2, defaulValue: "N.A" } },
//            //{ name: 'BTConinKM', width: 120, align: 'right', height: 'auto', cellattr: arrtSetting6 },
//            //{ name: 'WBMLengthinKM', width: 120, align: 'right', height: 'auto', cellattr: arrtSetting7, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2, defaulValue: "N.A" } },
//            //{ name: 'WBMConinKM', width: 120, align: 'right', height: 'auto', cellattr: arrtSetting8},
//            //{ name: 'GravelLengthinKM', width: 200, align: 'right', height: 'auto', cellattr: arrtSetting9, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2, defaulValue: "N.A" } },
//            //{ name: 'GravelConinKM', width: 200, align: 'right', height: 'auto', cellattr: arrtSetting10 },
//            //{ name: 'TrackLengthinKM', width: 200, align: 'right', height: 'auto', cellattr: arrtSetting11, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2, defaulValue: "N.A" } },
//            //{ name: 'TrackConinKM', width: 200, align: 'right', height: 'auto', cellattr: arrtSetting12},
//            //{ name: 'HabitationName', width: 200, align: 'right', height: 'auto' },
//            //{ name: 'HabTotPop', width: 200, align: 'right', height: 'auto', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
//            //{ name: 'HabitationConnected', width: 200, align: 'right', height: 'auto' },
//            //{ name: 'TotalPopulationServed', width: 200, align: 'right', height: 'auto', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
//            //{ name: 'DownloadKMLfile', width: 200, align: 'right', height: 'auto' }

//            { name: 'UniqueRoadId', width: 180, align: 'left', height: 'auto', sortable: true,cellattr: arrtSetting0 },
//            { name: 'PlannedRoadNumber', width: 100, align: 'left', height: 'auto', sortable: false, cellattr: arrtSetting1 },
//            { name: 'PlannedRoadfrom', width: 100, align: 'left', height: 'auto', search: false, sortable: false, cellattr: arrtSetting2 },
//            { name: 'PlannedRoadto', width: 120, align: 'left', height: 'auto', sortable: false, cellattr: arrtSetting3 },
//            { name: 'PlannedRoadlength', width: 120, align: 'right', height: 'auto', sortable: false, cellattr: arrtSetting4, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
//            { name: 'BTLengthinKM', width: 100, align: 'right', height: 'auto', sortable: false, cellattr: arrtSetting5, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
//            { name: 'BTConinKM', width: 100, align: 'center', height: 'auto', sortable: false, cellattr: arrtSetting6 },
//            { name: 'WBMLengthinKM', width: 100, align: 'right', height: 'auto', sortable: false, cellattr: arrtSetting7, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
//            { name: 'WBMConinKM', width: 100, align: 'center', height: 'auto', sortable: false, cellattr: arrtSetting8 },
//            { name: 'GravelLengthinKM', width: 100, align: 'right', height: 'auto', sortable: false, cellattr: arrtSetting9, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
//            { name: 'GravelConinKM', width: 100, align: 'center', height: 'auto', sortable: false, cellattr: arrtSetting10 },
//            { name: 'TrackLengthinKM', width: 100, align: 'right', height: 'auto', sortable: false, cellattr: arrtSetting11, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
//            { name: 'TrackConinKM', width: 100, align: 'center', height: 'auto', sortable: false, cellattr: arrtSetting12 },
//            { name: 'TotalPopulationServed', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
//            { name: 'HabitationName', width: 120, align: 'left', height: 'auto', sortable: false },
//            { name: 'HabTotPop', width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
//            { name: 'HabitationConnected', width: 70, align: 'center', height: 'auto', sortable: false },
//            { name: 'DownloadKMLfile', width: 100, align: 'right', height: 'auto', sortable: false }
    

        
//        ],
//        //grouping: true,
//        //groupingView: {
//        //    //groupField: ['PlannedRoadNumber', 'PlannedRoadName', 'PlannedRoadfrom', 'PlannedRoadto', 'PlannedRoadlength', 'BTLengthinKM', 'WBMLengthinKM', 'GravelLengthinKM', 'TrackLengthinKM'],
//        //    groupField: ['PlannedRoadNumber'],
//        //    groupSummary: [true],
//        //    groupColumnShow: [false],
//        //    groupDataSorted: true
//        //},
//        postData: { "BlockCode": blockCode, "StateCode": stateCode, "DistrictCode": districtCode, "Route": route },
//        pager: $("#CN1FinalReportPager"),
//        pgbuttons: true,
//        rowNum: 999999,       
//        viewrecords: true,
//        recordtext: '{2} records found',
//        sortname: "PlannedRoadNumber",
//        sortorder: "asc",
//        caption: 'Core Network Details for ' + blockName,
//        height: 'auto',
//        height: '380',
//        autowidth: false,
//        shrinkToFit: false,
//        width: 1100,
//        rownumbers: true, 
//        footerrow: true,           
//        hidegrid: true,
//        rownumbers: true,
//        gridview: true,
//        hoverrows: false,
//        autoencode: true,
//        ignoreCase: true,
//        pginput: false,
//        cmTemplate: { title: false },
//        beforeProcessing: function () {

//        },
//        gridComplete: function () {
//            //var grid = this;

//            //$('td[rowspan="1"]', grid).each(function () {
//            //    var spans = $('td[rowspanid="' + this.id + '"]', grid).length + 1;

//            //    if (spans > 1) {
//            //        $(this).attr('rowspan', spans).attr('vertical-align','central');
//            //    }
//            //});

//            //var ids = jQuery("#CN1FinalReportTable").jqGrid('getDataIDs');
//            //var previousId = 0;

//            //for (var i = 0; i < ids.length; i++) {
//            //    var rowId = ids[i];
            
//            //    var rowData = jQuery('#CN1FinalReportTable').jqGrid('getRowData', rowId);
//            //    // alert(rowData.PlannedRoadNumber);
//            //    var previousrowData = jQuery('#CN1FinalReportTable').jqGrid('getRowData', parseInt(rowId-1));
//            //   // alert(previousrowData);
//            //    if (rowData.PlannedRoadNumber == previousrowData.PlannedRoadNumber) {
//            //        var newTotalPop = parseFloat(previousrowData.BTLengthinKM) + parseFloat(rowData.WBMLengthinKM) + parseFloat(rowData.GravelLengthinKM) + parseFloat(rowData.TrackLengthinKM);
//            //        alert(newTotalPop);
//            //        $("#CN1FinalReportTable").jqGrid('setCell', rowId, 'TotalPopulationServed', parseFloat(newTotalPop).toFixed(2).toString().toLocaleString("en-IN"));
                    
                  
//            //    }
//            //}
//        },
//        loadComplete: function () {
//                        //Total of Columns
//                        var PlannedRoadlengthT = $(this).jqGrid('getCol', 'PlannedRoadlength', false, 'sum');
//                        PlannedRoadlengthT = parseFloat(PlannedRoadlengthT).toFixed(2);
//                        var BTLengthT = $(this).jqGrid('getCol', 'BTLengthinKM', false, 'sum');
//                        BTLengthT = parseFloat(BTLengthT).toFixed(3);
//                        var WBMLengthT = $(this).jqGrid('getCol', 'WBMLengthinKM', false, 'sum');
//                        WBMLengthT = parseFloat(WBMLengthT).toFixed(3);
//                        var GravelLengthT = $(this).jqGrid('getCol', 'GravelLengthinKM', false, 'sum');
//                        GravelLengthT = parseFloat(GravelLengthT).toFixed(3);
//                        var TrackLengthT = $(this).jqGrid('getCol', 'TrackLengthinKM', false, 'sum');
//                        TrackLengthT = parseFloat(TrackLengthT).toFixed(3);
//                        var TotalPopulationofHabitationT = $(this).jqGrid('getCol', 'TotalPopulationofHabitation', false, 'sum');
//                        var TotalPopulationServedT = $(this).jqGrid('getCol', 'TotalPopulationServed', false, 'sum');
//                        var HabTotPopT = $(this).jqGrid('getCol', 'HabTotPop', false, 'sum');

//                       // var DownloadKMLfileT = $(this).jqGrid('getCol', 'DownloadKMLfile', false, 'sum');
         
//                        //

//                        $(this).jqGrid('footerData', 'set', { UniqueRoadId: '<b>Total</b>' });
//                        $(this).jqGrid('footerData', 'set', { PlannedRoadlength: PlannedRoadlengthT });
//                        $(this).jqGrid('footerData', 'set', { BTLengthinKM: BTLengthT });
//                        $(this).jqGrid('footerData', 'set', { WBMLengthinKM: WBMLengthT });
//                        $(this).jqGrid('footerData', 'set', { GravelLengthinKM: GravelLengthT });
//                        $(this).jqGrid('footerData', 'set', { TrackLengthinKM: TrackLengthT });
//                        $(this).jqGrid('footerData', 'set', { TotalPopulationofHabitation: TotalPopulationofHabitationT });
//                        $(this).jqGrid('footerData', 'set', { TotalPopulationServed: TotalPopulationServedT });
//                        $(this).jqGrid('footerData', 'set', { TotalPopulationServed: TotalPopulationServedT });
//                        $(this).jqGrid('footerData', 'set', { HabTotPop: HabTotPopT });

//                       // $(this).jqGrid('footerData', 'set', { DownloadKMLfile: DownloadKMLfileT });
//                        $('#CN1FinalReportTable_rn').html('Sr.<br/>No.');
//                        $("#CN1FinalReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");

//                        $.unblockUI();
//                    },
//        loadError: function (xhr, status, error) {
//            if (xhr.responseText == "session expired") {
//                window.location.href = "/Login/SessionExpire";
//            }
//            else {
//                window.location.href = "/Login/SessionExpire";
//            }
//            $.unblockUI();
//        }
//    });/*End of Grid*/

//    $("#CN1FinalReportTable").jqGrid('setGroupHeaders', {
//        useColSpanStyle: false,
//        groupHeaders: [
//          //{ startColumnName: 'BTLengthinKM', numberOfColumns:2, titleText: '<em>BT</em>' },
//          //{ startColumnName: 'WBMLengthinKM', numberOfColumns: 2, titleText: '<em>WBM</em>' },
//          //{ startColumnName: 'GravelLengthinKM', numberOfColumns: 2, titleText: '<em>Gravel</em>' },
//          //{ startColumnName: 'TrackLengthinKM', numberOfColumns: 2, titleText: '<em>Track</em>' },
//            {
//                startColumnName: 'BTLengthinKM', numberOfColumns: 8,
//                titleText: '<table style="width:100%;border-spacing:0px"' +
//                        '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="8">Existing Surface Type </td>  </tr>' +
//                        '<tr>' +
//                            '<td id="h1" colspan="2" style="width: 8%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">BT</td>' +
//                            '<td id="h1" colspan="2" style="width: 8%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">WBM</td>' +
//                            '<td id="h1" colspan="2" style="width: 8%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Gravel</td>' +
//                            '<td id="h2" colspan="2" style="width: 8%;  border-right-color: inherit;  padding: 4px 0px;">Track</td>' +
//                        '</tr>' +
//                        '</table>'
//            },
//          { startColumnName: 'HabitationName', numberOfColumns: 3, titleText: '<em>Habitation Served </em>' },
//        ]

//    });
//}

//var prevCellVal0 = { cellId: undefined, value: undefined };

//arrtSetting0 = function (rowId, val, rawObject, cm, rdata) {
//    var result;

//    if (prevCellVal0.value == val) {
//        result = ' style="display: none" rowspanid="' + prevCellVal0.cellId + '"';
//    }
//    else {
//        var cellId = this.id + '_row_' + rowId + '_' + cm.name;

//        result = ' rowspan="1" id="' + cellId + '"';
//        prevCellVal0 = { cellId: cellId, value: val };
//    }

//    return result;
//}
//var prevCellVal1 = { cellId: undefined, value: undefined };

//arrtSetting1 = function (rowId, val, rawObject, cm, rdata) {
//    var result;
   
//    if (prevCellVal1.value == val) {
//        result = ' style="display: none" rowspanid="' + prevCellVal1.cellId + '"';
//    }
//    else {
//        var cellId = this.id + '_row_' + rowId + '_' + cm.name;

//        result = ' rowspan="1" id="' + cellId + '"';
//        prevCellVal1 = { cellId: cellId, value: val };
//    }

//    return result;
//}

//var prevCellVal2 = { cellId: undefined, value: undefined };

//arrtSetting2 = function (rowId, val, rawObject, cm, rdata) {
//    var result;

//    if (prevCellVal2.value == val) {
//        result = ' style="display: none" rowspanid="' + prevCellVal2.cellId + '"';
//    }
//    else {
//        var cellId = this.id + '_row_' + rowId + '_' + cm.name;

//        result = ' rowspan="1" id="' + cellId + '"';
//        prevCellVal2 = { cellId: cellId, value: val };
//    }

//    return result;
//}


//var prevCellVal3 = { cellId: undefined, value: undefined };

//arrtSetting3 = function (rowId, val, rawObject, cm, rdata) {
//    var result;

//    if (prevCellVal3.value == val) {
//        result = ' style="display: none" rowspanid="' + prevCellVal3.cellId + '"';
//    }
//    else {
//        var cellId = this.id + '_row_' + rowId + '_' + cm.name;

//        result = ' rowspan="1" id="' + cellId + '"';
//        prevCellVal3 = { cellId: cellId, value: val };
//    }

//    return result;
//}

//var prevCellVal4 = { cellId: undefined, value: undefined };

//arrtSetting4 = function (rowId, val, rawObject, cm, rdata) {
//    var result;

//    if (prevCellVal4.value == val) {
//        result = ' style="display: none" rowspanid="' + prevCellVal4.cellId + '"';
//    }
//    else {
//        var cellId = this.id + '_row_' + rowId + '_' + cm.name;

//        result = ' rowspan="1" id="' + cellId + '"';
//        prevCellVal4= { cellId: cellId, value: val };
//    }

//    return result;
//}

//var prevCellVal5 = { cellId: undefined, value: undefined };


//arrtSetting5 = function (rowId, val, rawObject, cm, rdata) {
//    var result;

//    if (prevCellVal5.value == val) {
//        result = ' style="display: none" rowspanid="' + prevCellVal5.cellId + '"';
//    }
//    else {
//        var cellId = this.id + '_row_' + rowId + '_' + cm.name;

//        result = ' rowspan="1" id="' + cellId + '"';
//        prevCellVal5 = { cellId: cellId, value: val };
//    }

//    return result;
//}
//var prevCellVal6 = { cellId: undefined, value: undefined };
//arrtSetting6 = function (rowId, val, rawObject, cm, rdata) {
//    var result;

//    if (prevCellVal6.value == val) {
//        result = ' style="display: none" rowspanid="' + prevCellVal6.cellId + '"';
//    }
//    else {
//        var cellId = this.id + '_row_' + rowId + '_' + cm.name;

//        result = ' rowspan="1" id="' + cellId + '"';
//        prevCellVal6 = { cellId: cellId, value: val };
//    }

//    return result;
//}
//var prevCellVal7 = { cellId: undefined, value: undefined };
//arrtSetting7 = function (rowId, val, rawObject, cm, rdata) {
//    var result;

//    if (prevCellVal7.value == val) {
//        result = ' style="display: none" rowspanid="' + prevCellVal7.cellId + '"';
//    }
//    else {
//        var cellId = this.id + '_row_' + rowId + '_' + cm.name;

//        result = ' rowspan="1" id="' + cellId + '"';
//        prevCellVal7 = { cellId: cellId, value: val };
//    }

//    return result;
//}
//var prevCellVal8 = { cellId: undefined, value: undefined };
//arrtSetting8 = function (rowId, val, rawObject, cm, rdata) {
//    var result;

//    if (prevCellVal8.value ==val) {
//        result = ' style="display: none" rowspanid="' + prevCellVal8.cellId + '"';
//    }
//    else {
//        var cellId = this.id + '_row_' + rowId + '_' + cm.name;

//        result = ' rowspan="1" id="' + cellId + '"';
//        prevCellVal8 = { cellId: cellId, value: val };
//    }

//    return result;
//}
//var prevCellVal9 = { cellId: undefined, value: undefined };
//arrtSetting9 = function (rowId, val, rawObject, cm, rdata) {
//    var result;

//    if (prevCellVal9.value == val) {
//        result = ' style="display: none" rowspanid="' + prevCellVal9.cellId + '"';
//    }
//    else {
//        var cellId = this.id + '_row_' + rowId + '_' + cm.name;

//        result = ' rowspan="1" id="' + cellId + '"';
//        prevCellVal9 = { cellId: cellId, value: val };
//    }

//    return result;
//}
//var prevCellVal10 = { cellId: undefined, value: undefined };
//arrtSetting10 = function (rowId, val, rawObject, cm, rdata) {
//    var result;

//    if (prevCellVal10.value == val) {
//        result = ' style="display: none" rowspanid="' + prevCellVal10.cellId + '"';
//    }
//    else {
//        var cellId = this.id + '_row_' + rowId + '_' + cm.name;

//        result = ' rowspan="1" id="' + cellId + '"';
//        prevCellVal10 = { cellId: cellId, value: val };
//    }

//    return result;
//}
//var prevCellVal11 = { cellId: undefined, value: undefined };
//arrtSetting11 = function (rowId, val, rawObject, cm, rdata) {
//    var result;

//    if (prevCellVal11.value == val) {
//        result = ' style="display: none" rowspanid="' + prevCellVal11.cellId + '"';
//    }
//    else {
//        var cellId = this.id + '_row_' + rowId + '_' + cm.name;

//        result = ' rowspan="1" id="' + cellId + '"';
//        prevCellVal11 = { cellId: cellId, value: val };
//    }

//    return result;
//}
//var prevCellVal12 = { cellId: undefined, value: undefined };
//arrtSetting12 = function (rowId, val, rawObject, cm, rdata) {
//    var result;

//    if (prevCellVal12.value == val) {
//        result = ' style="display: none" rowspanid="' + prevCellVal12.cellId + '"';
//    }
//    else {
//        var cellId = this.id + '_row_' + rowId + '_' + cm.name;

//        result = ' rowspan="1" id="' + cellId + '"';
//        prevCellVal12 = { cellId: cellId, value: val };
//    }

//    return result;
//}

///*End New Report*/