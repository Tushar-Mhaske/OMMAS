var collapse = true;
$(document).ready(function () {

    $("#tabs").tabs();
    $("#tabhabTrafiicCBRMain").tabs();
    $("#tabBridgehabTrafiicCBRMain").tabs();

    if ($("#MAST_STATE_CODE").val() > 0) {
        $("#ddState_PropAnalysisDetails").attr("disabled", "disabled");
    }
    $('#btnGoPropAnalysis').click(function () {
        $('#tabhabTrafiicCBRMain').hide();
        $("#tabBridgehabTrafiicCBRMain").hide();
        var stateCode = $("#ddState_PropAnalysisDetails option:selected").val();
        var scrutiny = $("#ddScrutiny_PropAnalysisDetails option:selected").val();
        var sanctioned = $("#ddSanctioned_PropAnalysisDetails option:selected").val();
        var type = ''; //P- road,L-Bridge
        var report = 'A'; //A,H,S,M
        $("#tbPropDataGapRoadDetailReport").jqGrid('GridUnload');
        $("#tbPropAnalysisDetailReport").jqGrid('GridUnload');
        $("#tbPropAnalysisHabDetailReport").jqGrid('GridUnload');
        $("#tbPropAnalysistrafficDetailReport").jqGrid('GridUnload');
        $("#tbPropAnalysisCBRDetailReport").jqGrid('GridUnload');

        $("#tbPropDataGapBridgeDetailReport").jqGrid('GridUnload');
        $("#tbBridgePropAnalysisDetailReport").jqGrid('GridUnload');
        $("#tbBridgePropAnalysisHabDetailReport").jqGrid('GridUnload');
        $("#tbBridgePropAnalysistrafficDetailReport").jqGrid('GridUnload');
        $("#tbBridgePropAnalysisCBRDetailReport").jqGrid('GridUnload');
        LoadPropDataGapRoadDetailsGrid(stateCode, type, scrutiny, sanctioned, report);
        LoadPropDataGapBridgeDetailsGrid(stateCode, type, scrutiny, sanctioned, report);
    });

    $('#btnGoPropAnalysis').trigger('click');

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

    $('#spCollapseIconTrafiicCBRMain').click(function () {
        if ($("#spCollapseIconTrafiicCBRMain").hasClass('ui-icon-circle-triangle-n')) {
            $("#tbPropAnalysisHabDetailReport").jqGrid('setGridState', 'hidden');
            $("#tbPropAnalysistrafficDetailReport").jqGrid('setGridState', 'hidden');
            $("#tbPropAnalysisCBRDetailReport").jqGrid('setGridState', 'hidden');
            $("#spCollapseIconTrafiicCBRMain").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
        }
        else {
            $("#tbPropAnalysisHabDetailReport").jqGrid('setGridState', 'visible');
            $("#tbPropAnalysistrafficDetailReport").jqGrid('setGridState', 'visible');
            $("#tbPropAnalysisCBRDetailReport").jqGrid('setGridState', 'visible');
            $("#spCollapseIconTrafiicCBRMain").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

        }
    });
    $('#spCollapseIconBridgeTrafiicCBRMain').click(function () {
        
        if ($("#spCollapseIconBridgeTrafiicCBRMain").hasClass('ui-icon-circle-triangle-n')) {
            $("#tbBridgePropAnalysisHabDetailReport").jqGrid('setGridState', 'hidden');
            $("#tbBridgePropAnalysistrafficDetailReport").jqGrid('setGridState', 'hidden');
            $("#tbBridgePropAnalysisCBRDetailReport").jqGrid('setGridState', 'hidden');
            $("#spCollapseIconBridgeTrafiicCBRMain").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            }
            else {
            $("#tbBridgePropAnalysisHabDetailReport").jqGrid('setGridState', 'visible');
            $("#tbBridgePropAnalysistrafficDetailReport").jqGrid('setGridState', 'visible');
            $("#tbBridgePropAnalysisCBRDetailReport").jqGrid('setGridState', 'visible');
            $("#spCollapseIconBridgeTrafiicCBRMain").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            }
        
    });
    $('#rbtnExpandAll').click(function () {
        collapse = false;
        //$("#tbPropDataGapRoadDetailReport").jqGrid('GridUnload');
        $('#btnGoPropAnalysis').trigger('click');
        //$("#tbPropDataGapRoadDetailReport").trigger('reloadGrid');

       // $("#tbPropAnalysistrafficDetailReport").jqGrid('groupCollapse', collapse);

    });

    $('#rbtnCollapsAll').click(function () {
        collapse = true;
       // $("#tbPropDataGapRoadDetailReport").jqGrid('GridUnload');
        $('#btnGoPropAnalysis').trigger('click');
        //$("#tbPropDataGapRoadDetailReport").trigger('reloadGrid');

        // $("#tbPropAnalysistrafficDetailReport").jqGrid('groupCollapse', collapse);

    });
});

function LoadPropDataGapRoadDetailsGrid(stateCode, type, scrutiny, sanctioned, report) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbPropDataGapRoadDetailReport").jqGrid('GridUnload');
    jQuery("#tbPropDataGapRoadDetailReport").jqGrid({
        url: '/ProposalReports/PropAnalysisDataGapReportListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["PHASE", "Batch", "Total Proposal", "Total Proposal Hidden", "Proposal With Single Habitation", "Single Habitation", "Multiple Habitation", "Habitation Not Mapped", "Maintenance Cost Not Specified ", "CN Road Not Mapped", "Non Standard Carriage Width", "Traffic Intensity Not Specified", "CBR Value Not Specified"],
        colModel: [
            { name: "PHASE", width: 150, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_BATCH", width: 120, align: 'center',  height: 'auto', sortable: false, summaryType: 'count', summaryTpl: '<b> Total </b>' },
            { name: "ROAD_COUNT", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum' ,formatter: 'integer',  formatoptions: {thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "ROAD_COUNTHidden", width: 50, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', hidden: true },
            { name: "SINGLE_HAB", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "HAB_MAPPED_SINGLE", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "HAB_MAPPED_MANY", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "HAB_NOT_MAPPED", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum' ,formatter: 'integer',  formatoptions: {thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT_COST", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CN_NOT_MAPPED", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CARRIAGE_WIDTH", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TRAFFIC_INTENSITY", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CBR_VALUE", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "Type": "P", "Scrutiny": scrutiny, "Sanctioned": sanctioned, "Report": report },
        pager: jQuery('#dvPropDataGapRoadDetailReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Road Proposal Data Gaps",
        height: 420,
        rownumbers: true,
        autowidth: true,
        footerrow: true,
        grouping: true,
        groupingView: {
            groupField: ['PHASE'],
            groupSummary: [true],
            groupColumnShow: [false],
            groupDataSorted: true,
            showSummaryOnHide: true,
         //   groupText: ['<b>{0}- {1} </b>'],
            groupCollapse: collapse,
        },
        loadComplete: function () {

           // alert($('tr.ui-widget-content jqfoot ui-row-ltr').children('td').val());

           // $("#tbPropDataGapRoadDetailReport").jqGrid('setGridWidth', $('#tabs').width(), true);

            //Total of Columns
            var ROAD_COUNTHiddenT = $(this).jqGrid('getCol', 'ROAD_COUNTHidden', false, 'sum');
             var SINGLE_HABT = $(this).jqGrid('getCol', 'SINGLE_HAB', false, 'sum');
            var HAB_MAPPED_SINGLET = $(this).jqGrid('getCol', 'HAB_MAPPED_SINGLE', false, 'sum');
            var HAB_MAPPED_MANYT = $(this).jqGrid('getCol', 'HAB_MAPPED_MANY', false, 'sum');
            var HAB_NOT_MAPPEDT = $(this).jqGrid('getCol', 'HAB_NOT_MAPPED', false, 'sum');
            var MAINT_COSTT = $(this).jqGrid('getCol', 'MAINT_COST', false, 'sum');
            MAINT_COSTT = parseFloat(MAINT_COSTT).toFixed(2);
            var CN_NOT_MAPPEDT = $(this).jqGrid('getCol', 'CN_NOT_MAPPED', false, 'sum');
            var CARRIAGE_WIDTHT = $(this).jqGrid('getCol', 'CARRIAGE_WIDTH', false, 'sum');
            var TRAFFIC_INTENSITYT = $(this).jqGrid('getCol', 'TRAFFIC_INTENSITY', false, 'sum');
            var CBR_VALUET = $(this).jqGrid('getCol', 'CBR_VALUE', false, 'sum');


            ////

            $(this).jqGrid('footerData', 'set', { IMS_BATCH: '<b>Total for State</b>' });
            $(this).jqGrid('footerData', 'set', { ROAD_COUNT: ROAD_COUNTHiddenT }, true);
            $(this).jqGrid('footerData', 'set', { SINGLE_HAB: SINGLE_HABT }, true);
            $(this).jqGrid('footerData', 'set', { HAB_MAPPED_SINGLE: HAB_MAPPED_SINGLET }, true);
            $(this).jqGrid('footerData', 'set', { HAB_MAPPED_MANY: HAB_MAPPED_MANYT }, true);
            $(this).jqGrid('footerData', 'set', { HAB_NOT_MAPPED: HAB_NOT_MAPPEDT }, true);
            $(this).jqGrid('footerData', 'set', { MAINT_COST: MAINT_COSTT }, true);
            $(this).jqGrid('footerData', 'set', { CN_NOT_MAPPED: CN_NOT_MAPPEDT }, true);
            $(this).jqGrid('footerData', 'set', { CARRIAGE_WIDTH: CARRIAGE_WIDTHT }, true);
            $(this).jqGrid('footerData', 'set', { TRAFFIC_INTENSITY: TRAFFIC_INTENSITYT }, true);
            $(this).jqGrid('footerData', 'set', { CBR_VALUE: CBR_VALUET }, true);
         //   $("#dvPropDataGapRoadDetailReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Cost Rs. in lacs </font>");
            $('#tbPropDataGapRoadDetailReport_rn').html('Sr.<br/>No.');

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
        },
        onSelectRow: function (id) {
            //alert(id);
            var params = id.split('$');
            LoadPropAnalysisDetailGrid(params[0], params[1], params[2], params[3], params[4], params[5], params[6], params[7]);
        }

    }); //end of grid


    $("#tbPropDataGapRoadDetailReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'SINGLE_HAB', numberOfColumns: 4, titleText: '<em>Proposals with Complete data, and </em>' },
          { startColumnName: 'MAINT_COST', numberOfColumns: 5, titleText: '<em>Proposal with Datagaps</em>' }
        ]
    });


}

function LoadPropDataGapBridgeDetailsGrid(stateCode, type, scrutiny, sanctioned, report) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbPropDataGapBridgeDetailReport").jqGrid('GridUnload');
    jQuery("#tbPropDataGapBridgeDetailReport").jqGrid({
        url: '/ProposalReports/PropAnalysisDataGapReportListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["PHASE", "Batch", "Total Proposal", "Total Proposal Hidden", "Proposal With Single Habitation", "Single Habitation", "Multiple Habitation", "Habitation Not Mapped", "Maintenance Cost Not Specified ", "CN Road Not Mapped", "Non Standard Carriage Width ", "Traffic Intensity Not Specified", "CBR Value Not Specified"],
        colModel: [
            { name: "PHASEB", width: 150, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_BATCHB", width: 120, align: 'center',  height: 'auto', sortable: false, summaryType: 'count', summaryTpl: '<b> Total </b>' },
            { name: "ROAD_COUNTB", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
             { name: "ROAD_COUNTHiddenB", width: 50, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', hidden: true },
            { name: "SINGLE_HABB", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "HAB_MAPPED_SINGLEB", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "HAB_MAPPED_MANYB", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "HAB_NOT_MAPPEDB", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT_COSTB", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CN_NOT_MAPPEDB", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CARRIAGE_WIDTHB", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TRAFFIC_INTENSITYB", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CBR_VALUEB", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "Type": "L", "Scrutiny": scrutiny, "Sanctioned": sanctioned, "Report": report },
        pager: jQuery('#dvPropDataGapBridgeDetailReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Bridge Proposal Data Gaps",
        height: 420,
        rownumbers: true,
        autowidth: true,
        footerrow: true,
        grouping: true,
        groupingView: {
            groupField: ['PHASEB'],
            groupSummary: [true],
            groupColumnShow: [false],
            groupDataSorted: true,
            showSummaryOnHide: true,
            //  groupText: ['<b>{0} - {1} Total Works</b>'],
            groupCollapse: true,
        },
        loadComplete: function () {
           // $("#tbPropDataGapBridgeDetailReport").jqGrid('setGridWidth', $('#tabs').width(), true);

            //Total of Columns
            var ROAD_COUNTHiddenT = $(this).jqGrid('getCol', 'ROAD_COUNTHiddenB', false, 'sum');
            var SINGLE_HABT = $(this).jqGrid('getCol', 'SINGLE_HABB', false, 'sum');
            var HAB_MAPPED_SINGLET = $(this).jqGrid('getCol', 'HAB_MAPPED_SINGLEB', false, 'sum');
            var HAB_MAPPED_MANYT = $(this).jqGrid('getCol', 'HAB_MAPPED_MANYB', false, 'sum');
            var HAB_NOT_MAPPEDT = $(this).jqGrid('getCol', 'HAB_NOT_MAPPEDB', false, 'sum');
            var MAINT_COSTT = $(this).jqGrid('getCol', 'MAINT_COSTB', false, 'sum');
            MAINT_COSTT = parseFloat(MAINT_COSTT).toFixed(2);
            var CN_NOT_MAPPEDT = $(this).jqGrid('getCol', 'CN_NOT_MAPPEDB', false, 'sum');
            var CARRIAGE_WIDTHT = $(this).jqGrid('getCol', 'CARRIAGE_WIDTHB', false, 'sum');
            var TRAFFIC_INTENSITYT = $(this).jqGrid('getCol', 'TRAFFIC_INTENSITYB', false, 'sum');
            var CBR_VALUET = $(this).jqGrid('getCol', 'CBR_VALUEB', false, 'sum');


            ////

            $(this).jqGrid('footerData', 'set', { IMS_BATCHB: '<b>Total for State</b>' });
            $(this).jqGrid('footerData', 'set', { ROAD_COUNTB: ROAD_COUNTHiddenT }, true);
            $(this).jqGrid('footerData', 'set', { SINGLE_HABB: SINGLE_HABT }, true);
            $(this).jqGrid('footerData', 'set', { HAB_MAPPED_SINGLEB: HAB_MAPPED_SINGLET }, true);
            $(this).jqGrid('footerData', 'set', { HAB_MAPPED_MANYB: HAB_MAPPED_MANYT }, true);
            $(this).jqGrid('footerData', 'set', { HAB_NOT_MAPPEDB: HAB_NOT_MAPPEDT }, true);
            $(this).jqGrid('footerData', 'set', { MAINT_COSTB: MAINT_COSTT }, true);
            $(this).jqGrid('footerData', 'set', { CN_NOT_MAPPEDB: CN_NOT_MAPPEDT }, true);
            $(this).jqGrid('footerData', 'set', { CARRIAGE_WIDTHB: CARRIAGE_WIDTHT }, true);
            $(this).jqGrid('footerData', 'set', { TRAFFIC_INTENSITYB: TRAFFIC_INTENSITYT }, true);
            $(this).jqGrid('footerData', 'set', { CBR_VALUEB: CBR_VALUET }, true);

           // $("#dvPropDataGapBridgeDetailReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Cost Rs. in lacs </font>");

            jQuery("#tbPropDataGapBridgeDetailReport").jqGrid('destroyGroupHeader');
            $("#tbPropDataGapBridgeDetailReport").jqGrid('setGroupHeaders', {
                useColSpanStyle: false,
                groupHeaders: [
                  { startColumnName: 'SINGLE_HABB', numberOfColumns: 4, titleText: '<em>Proposals with Complete data, and</em>' },
                  { startColumnName: 'MAINT_COSTB', numberOfColumns: 5, titleText: '<em>Proposals with Datagaps</em>' }
                ]
            });

            $('#tbPropDataGapBridgeDetailReport_rn').html('Sr.<br/>No.');

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
        },
        onSelectRow: function (id) {
            //alert(id);
            var params = id.split('$');
            LoadPropAnalysisDetailGrid(params[0], params[1], params[2], params[3], params[4], params[5], params[6], params[7]);
        }



    }); //end of grid


 


}

function LoadPropAnalysisDetailGrid(stateCode, year, batch, type, scrutiny, sanctioned, report, proposals) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    var tblgrid;
    var divpager;
      if (type == "P") {
          //Road
          tblgrid = "#tbPropAnalysisDetailReport";
          divpager = "#dvPropAnalysisDetailReportPager";
        $('#tabhabTrafiicCBRMain').hide();
        $("#tbPropDataGapRoadDetailReport").jqGrid('setGridState', 'hidden');
        $("#tbPropDataGapRoadDetailReport").jqGrid('setSelection', stateCode);
    }
    else {
          //Bridge
          tblgrid = "#tbBridgePropAnalysisDetailReport";
          divpager = "#dvBridgePropAnalysisDetailReportPager";
        $("#tbPropDataGapBridgeDetailReport").jqGrid('setGridState', 'hidden');
        $("#tbPropDataGapBridgeDetailReport").jqGrid('setSelection', stateCode);
    }
   
 
    $(tblgrid).jqGrid('GridUnload');
    jQuery(tblgrid).jqGrid({
        url: '/ProposalReports/PropAnalysisDetailsReportListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["District Name", "Block Name", "Package", "Stream", "Connectivity Type", "Scrutinized", "CN Road Number ", "Road Name", "Carriage Width (Mts.)", "Proposed Length (Kms.)",
                 "CN Length (Kms.)", "Sanctioned Length (Previous)", "Extra Length (Kms.)", "% Variation in Length", "MoRD Shares (Rs. in Lacs)", "State Shares (Rs. in Lacs)",
                "Total Cost (Rs. in Lacs)", "Maintenance Cost (Rs. in Lacs)"],
        colModel: [
            { name: "MAST_DISTRICT_NAME", width: 200, align: 'left',  height: 'auto', sortable: false },
            { name: "MAST_BLOCK_NAME", width: 200, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_PACKAGE_ID", width: 150, align: 'left',  height: 'auto', sortable: false },
            { name: "STREAM", width: 150, align: 'left',  height: 'auto', sortable: false },
            { name: "CONN_TYPE", width: 150, align: 'left',  height: 'auto', sortable: false },
            { name: "SCRUTINY", width: 120, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_CN_ROAD_NUMBER", width: 150, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_ROAD_NAME", width: 300, align: 'left',  height: 'auto', sortable: false, summaryType: 'count', summaryTpl: '<b>{0} Total </b>' },
            { name: "IMS_CARRIAGED_WIDTH", width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "IMS_PAV_LENGTH", width: 120, align: 'right',  height: 'auto', sortable: false ,formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CN_LENGTH", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "PREV_CN_LENGTH", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "EXTRA_LENGTH", width: 150, align: 'right',  height: 'auto', sortable: false,  formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "PER_EXTRA", width: 150, align: 'right',  height: 'auto', sortable: false },
            { name: "PROP_COST", width: 150, align: 'right',  height: 'auto', sortable: false,formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "STATE_SHARE", width: 150, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum' },
            { name: "TOTAL_COST", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT_COST", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "Year": year, "Batch": batch, "Type": type, "Scrutiny": scrutiny, "Sanctioned": sanctioned, "Report": report },
        pager: jQuery(divpager),
        rowNum: 2147483647,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Proposal Details: [ Year - " + year + " , Batch - " + batch + " , Total Proposals-" + proposals + " ]",
        height: 380,
        rownumbers: true,
        autowidth: true,
        footerrow: true,
        grouping: true,
        groupingView: {
            groupField: ['MAST_DISTRICT_NAME'],
            groupSummary: [true],
            groupColumnShow: [false],
            groupDataSorted: true,
            showSummaryOnHide: true,
            //  groupText: ['<b>{0} - {1} Total Works</b>'],
            groupCollapse: collapse,
        },
        loadComplete: function (data) {
            //Total of Columns
            var IMS_CARRIAGED_WIDTHT = $(this).jqGrid('getCol', 'IMS_CARRIAGED_WIDTH', false, 'sum');
            IMS_CARRIAGED_WIDTHT = parseFloat(IMS_CARRIAGED_WIDTHT).toFixed(3);
            var IMS_PAV_LENGTHT = $(this).jqGrid('getCol', 'IMS_PAV_LENGTH', false, 'sum');
            IMS_PAV_LENGTHT = parseFloat(IMS_PAV_LENGTHT).toFixed(3);
            var CN_LENGTHT = $(this).jqGrid('getCol', 'CN_LENGTH', false, 'sum');
            CN_LENGTHT = parseFloat(CN_LENGTHT).toFixed(3);
            var PREV_CN_LENGTHT = $(this).jqGrid('getCol', 'PREV_CN_LENGTH', false, 'sum');
            PREV_CN_LENGTHT = parseFloat(PREV_CN_LENGTHT).toFixed(3);
            var EXTRA_LENGTHT = $(this).jqGrid('getCol', 'EXTRA_LENGTH', false, 'sum');
            EXTRA_LENGTHT = parseFloat(EXTRA_LENGTHT).toFixed(3);
            var PROP_COSTT = $(this).jqGrid('getCol', 'PROP_COST', false, 'sum');
            PROP_COSTT = parseFloat(PROP_COSTT).toFixed(2);
            var STATE_SHARET = $(this).jqGrid('getCol', 'STATE_SHARE', false, 'sum');
            STATE_SHARET = parseFloat(STATE_SHARET).toFixed(2);
            var TOTAL_COSTT = $(this).jqGrid('getCol', 'TOTAL_COST', false, 'sum');
            TOTAL_COSTT = parseFloat(TOTAL_COSTT).toFixed(2);
            var MAINT_COSTT = $(this).jqGrid('getCol', 'MAINT_COST', false, 'sum');
            MAINT_COSTT = parseFloat(MAINT_COSTT).toFixed(2);


            ////

            $(this).jqGrid('footerData', 'set', { MAST_BLOCK_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { IMS_ROAD_NAME: 'Total ' + data["records"].toString() + ' Proposals' }, true);
            $(this).jqGrid('footerData', 'set', { IMS_CARRIAGED_WIDTH: IMS_CARRIAGED_WIDTHT }, true);
            $(this).jqGrid('footerData', 'set', { IMS_PAV_LENGTH: IMS_PAV_LENGTHT }, true);
            $(this).jqGrid('footerData', 'set', { CN_LENGTH: CN_LENGTHT }, true);
            $(this).jqGrid('footerData', 'set', { PREV_CN_LENGTH: PREV_CN_LENGTHT }, true);
            $(this).jqGrid('footerData', 'set', { EXTRA_LENGTH: EXTRA_LENGTHT }, true);
            $(this).jqGrid('footerData', 'set', { PROP_COST: PROP_COSTT }, true);
            $(this).jqGrid('footerData', 'set', { STATE_SHARE: STATE_SHARET }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_COST: TOTAL_COSTT }, true);
            $(this).jqGrid('footerData', 'set', { MAINT_COST: MAINT_COSTT }, true);
             // alert(data["records"].toString());
            $(tblgrid+'_rn').html('Sr.<br/>No.');
            $.unblockUI();
        },
        onSelectRow: function (id) {
            //alert(id);
            var params = id.split('$');
            LoadHabitationTrafficCBRGrid(params[0], params[1],type);
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



    }); //end of grid



}

function LoadHabitationTrafficCBRGrid(roadcode, roadName, type) {

    //Common function For Loading Habitation Traffic and CBR Grid
    var tblgrid;
    var divpager;
    if (type == "P") {     

        $("#tbPropAnalysisDetailReport").jqGrid('setGridState', 'hidden');
        $("#tbPropAnalysisDetailReport").jqGrid('setSelection', roadcode);

        $('#tabhabTrafiicCBRMain').show();
        LoadPropAnalysisHabitationGrid(roadcode, roadName, type);
        LoadPropAnalysisTrafficGrid(roadcode, roadName, type);
        LoadPropAnalysisCBRGrid(roadcode, roadName, type);
    }
    else {
        $("#tbBridgePropAnalysisDetailReport").jqGrid('setGridState', 'hidden');
        $("#tbBridgePropAnalysisDetailReport").jqGrid('setSelection', roadcode);
        $('#tabBridgehabTrafiicCBRMain').show();
        LoadPropAnalysisHabitationGrid(roadcode, roadName, type);
        LoadPropAnalysisTrafficGrid(roadcode, roadName, type);
        LoadPropAnalysisCBRGrid(roadcode, roadName, type);

    }
}

function LoadPropAnalysisHabitationGrid(roadcode,roadName,type) {
    var tblgrid;
    var divpager;
    var tabdiv;
    if (type == "P") {
        tblgrid = "#tbPropAnalysisHabDetailReport";
        divpager = "#dvPropAnalysisHabDetailReportPager";
        tabdiv = "#tabhabTrafiicCBRMain";
    }
    else {
        tblgrid = "#tbBridgePropAnalysisHabDetailReport";
        divpager = "#dvBridgePropAnalysisHabDetailReportPager";
        tabdiv = "#tabBridgehabTrafiicCBRMain";
    }
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $(tblgrid).jqGrid('GridUnload');
    jQuery(tblgrid).jqGrid({
        url: '/ProposalReports/PropAnalysisHabitationReportListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["Habitation", "Population", "SC/ST Population", "Connected"],
        colModel: [
            { name: "MAST_HAB_NAME", width: 150, align: 'center',  height: 'auto', sortable: true },
            { name: "MAST_HAB_TOT_POP", width: 150, align: 'center',  height: 'auto', sortable: false },
            { name: "IMS_IS_HAB_SCST", width: 150, align: 'center',  height: 'auto', sortable: false },
            { name: "MAST_HAB_CONNECTED", width: 150, align: 'center',  height: 'auto', sortable: false }
           ],
        postData: { "Roadcode": roadcode,"Type":"Hab" },
        pager: jQuery(divpager),
        rowNum: 2147483647,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Habitation Details: " + roadName + "",
        height: 260,
        autowidth: false,
        sortname: 'MAST_HAB_NAME',
        footerrow: true,       
        rownumbers: true,
        loadComplete: function () {
            //$(tblgrid).jqGrid('setGridWidth', $(tabdiv).width(tabdiv) / 2, true);
            $(tblgrid).jqGrid('setGridWidth', 500, true);
            //Total of Columns
            var MAST_HAB_TOT_POPT = $(this).jqGrid('getCol', 'MAST_HAB_TOT_POP', false, 'sum');
          
            ////

            $(this).jqGrid('footerData', 'set', { MAST_HAB_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { MAST_HAB_TOT_POP: MAST_HAB_TOT_POPT }, true);
            $(tblgrid + '_rn').html('Sr.<br/>No.');

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



    }); //end of grid



}

function LoadPropAnalysisTrafficGrid(roadcode, roadName,type) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    var tblgrid;
    var divpager;
    var tabdiv;
    if (type == "P") {
        tblgrid = "#tbPropAnalysistrafficDetailReport";
        divpager = "#dvPropAnalysistrafficDetailReportPager";
        tabdiv = "#tabhabTrafiicCBRMain";
    }
    else {
        tblgrid = "#tbBridgePropAnalysistrafficDetailReport";
        divpager = "#dvBridgePropAnalysistrafficDetailReportPager";
        tabdiv = "#tabBridgehabTrafiicCBRMain";
    }
    $(tblgrid).jqGrid('GridUnload');
    jQuery(tblgrid).jqGrid({
        url: '/ProposalReports/PropAnalysisTraffReportListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["Year", "Total Motorised Traffic/Day", "CCVPD/ESAL"],
        colModel: [
            { name: "IMS_TI_YEAR", width: 200, align: 'center',  height: 'auto', sortable: true },
            { name: "IMS_TOTAL_TI", width: 200, align: 'center',  height: 'auto', sortable: false },
            { name: "IMS_COMM_TI", width: 200, align: 'center',  height: 'auto', sortable: false }
              ],
        postData: { "Roadcode": roadcode, "Type": "Traff" },
        pager: jQuery(divpager),
        rowNum: 2147483647,
        rownumbers: true,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Traffic Details: "+roadName+"",
        height: 260,
        autowidth: false,
        sortname: 'IMS_TI_YEAR',
        footerrow: true,
        loadComplete: function () {

          //  $(tblgrid).jqGrid('setGridWidth', $(tabdiv).width() / 2, true);
            $(tblgrid).jqGrid('setGridWidth', 500, true);

            //Total of Columns
            //var IMS_PAV_LENGTHT = $(this).jqGrid('getCol', 'IMS_PAV_LENGTH', false, 'sum');
            //var PAV_COSTT = $(this).jqGrid('getCol', 'PAV_COST', false, 'sum');
            
            ////

            //$(this).jqGrid('footerData', 'set', { MAST_DISTRICT_NAME: '<b>Totals</b>' });
            //$(this).jqGrid('footerData', 'set', { IMS_PAV_LENGTH: IMS_PAV_LENGTHT }, true);
            //$(this).jqGrid('footerData', 'set', { PAV_COST: PAV_COSTT }, true);
            $(tblgrid + '_rn').html('Sr.<br/>No.');

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



    }); //end of grid



}

function LoadPropAnalysisCBRGrid(roadcode, roadName,type) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    var tblgrid;
    var divpager;
    var tabdiv;
    if (type == "P") {
        tblgrid = "#tbPropAnalysisCBRDetailReport";
        divpager = "#dvPropAnalysisCBRDetailReportPager";
        tabdiv = "#tabhabTrafiicCBRMain";
    }
    else {
        tblgrid = "#tbBridgePropAnalysisCBRDetailReport";
        divpager = "#dvBridgePropAnalysisCBRDetailReportPager";
        tabdiv = "#tabBridgehabTrafiicCBRMain";
    }
    $(tblgrid).jqGrid('GridUnload');
    jQuery(tblgrid).jqGrid({
        url: '/ProposalReports/PropAnalysisCBRReportListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["Segment Number", "Start Chainage", "End Chainage", "CBR Value"],
        colModel: [
            { name: "IMS_SEGMENT_NO", width: 150, align: 'center',  height: 'auto', sortable: true },
            { name: "IMS_STR_CHAIN", width: 150, align: 'center',  height: 'auto', sortable: false },
            { name: "IMS_END_CHAIN", width: 150, align: 'center',  height: 'auto', sortable: false },
            { name: "IMS_CBR_VALUE", width: 150, align: 'center',  height: 'auto', sortable: false },
           ],
        postData: { "Roadcode": roadcode, "Type": "CBR" },
        pager: jQuery('#dvPropAnalysisCBRDetailReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        caption: "&nbsp;&nbsp;CBR Values : "+roadName+"",
        height: 260,
        sortname: 'IMS_SEGMENT_NO',
        autowidth: false,
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {

           // $(tblgrid).jqGrid('setGridWidth', $(tabdiv).width() / 2, true);
            $(tblgrid).jqGrid('setGridWidth', 500, true);

            //Total of Columns
            //var IMS_PAV_LENGTHT = $(this).jqGrid('getCol', 'IMS_PAV_LENGTH', false, 'sum');
            //var PAV_COSTT = $(this).jqGrid('getCol', 'PAV_COST', false, 'sum');


            //////

            //$(this).jqGrid('footerData', 'set', { MAST_DISTRICT_NAME: '<b>Totals</b>' });
            //$(this).jqGrid('footerData', 'set', { IMS_PAV_LENGTH: IMS_PAV_LENGTHT }, true);
            //$(this).jqGrid('footerData', 'set', { PAV_COST: PAV_COSTT }, true);
            $(tblgrid + '_rn').html('Sr.<br/>No.');

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



    }); //end of grid



}


