$(document).ready(function () {
      
    $("#btCNR2Details").click(function () {

        var route = $("#Route_CNR2Details").val();
        var roadCategory = $("#RoadCategory_CNR2Details").val();

        if ($("#hdnLevelId").val() == 6) //mord
        {
          
            CNR2StateReportListing(route, roadCategory);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
           
            CNR2DistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), route, roadCategory);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
          
            CNR2BlockReportListing($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), route, roadCategory);
        }
    });
  
    $("#btCNR2Details").trigger('click');

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});


/*       STATE REPORT LISTING       */
function CNR2StateReportListing(route, roadCategory) {
    $("#CNR2StateReportTable").jqGrid('GridUnload');
    $("#CNR2DistrictReportTable").jqGrid('GridUnload');
    $("#CNR2BlockReportTable").jqGrid('GridUnload');
    $("#CNR2FinalReportTable").jqGrid('GridUnload');
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNR2StateReportTable").jqGrid({
        url: '/CoreNetworkReports/CNR2StateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Number', 'Length', 'Number', 'Length ', 'Number', 'Length ', 'Number', 'Length ', 'Number', 'Length', 'Number', 'Length', 'Balance Length'],
        colModel: [
            { name: 'StateName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'TRCNNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRCNLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRPNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRPLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRCNNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRCNLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRPNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRPLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TCNNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TCNLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TPNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TPLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TBALLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } }


        ],
        postData: { "Route": route, "RoadCategory": roadCategory },
        pager: $("#CNR2StateReportPager"),
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
            var TRCNNumberT = $(this).jqGrid('getCol', 'TRCNNumber', false, 'sum');
            var TRCNLengthT = $(this).jqGrid('getCol', 'TRCNLength', false, 'sum');
            TRCNLengthT = parseFloat(TRCNLengthT).toFixed(3);
            var TRPNumberT = $(this).jqGrid('getCol', 'TRPNumber', false, 'sum');
            var TRPLengthT = $(this).jqGrid('getCol', 'TRPLength', false, 'sum');
            TRPLengthT = parseFloat(TRPLengthT).toFixed(3);
            var LRCNNumberT = $(this).jqGrid('getCol', 'LRCNNumber', false, 'sum');
            var LRCNLengthT = $(this).jqGrid('getCol', 'LRCNLength', false, 'sum');
            LRCNLengthT = parseFloat(LRCNLengthT).toFixed(3);
            var LRPNumberT = $(this).jqGrid('getCol', 'LRPNumber', false, 'sum');
            var LRPLengthT = $(this).jqGrid('getCol', 'LRPLength', false, 'sum');
            LRPLengthT = parseFloat(LRPLengthT).toFixed(3);
            var TCNNumberT = $(this).jqGrid('getCol', 'TCNNumber', false, 'sum');
            var TCNLengthT = $(this).jqGrid('getCol', 'TCNLength', false, 'sum');
            TCNLengthT = parseFloat(TCNLengthT).toFixed(3);
            var TPNumberT = $(this).jqGrid('getCol', 'TPNumber', false, 'sum');
            var TPLengthT = $(this).jqGrid('getCol', 'TPLength', false, 'sum');
            TPLengthT = parseFloat(TPLengthT).toFixed(3);
            var TBALLengthT = $(this).jqGrid('getCol', 'TBALLength', false, 'sum');
            TBALLengthT = parseFloat(TBALLengthT).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TRCNNumber: TRCNNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TRCNLength: TRCNLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TRPNumber: TRPNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TRPLength: TRPLengthT }, true);
            $(this).jqGrid('footerData', 'set', { LRCNNumber: LRCNNumberT }, true);
            $(this).jqGrid('footerData', 'set', { LRCNLength: LRCNLengthT }, true);
            $(this).jqGrid('footerData', 'set', { LRPNumber: LRPNumberT }, true);
            $(this).jqGrid('footerData', 'set', { LRPLength: LRPLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TCNNumber: TCNNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TCNLength: TCNLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TPNumber: TPNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TPLength: TPLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TBALLength: TBALLengthT }, true);
            $("#CNR2StateReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#CNR2StateReportTable_rn').html('Sr.<br/>No.');

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

    $("#CNR2StateReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          //{ startColumnName: 'TRCNNumber', numberOfColumns: 2, titleText: '<em>Core Network </em>' },
          //{ startColumnName: 'TRPNumber', numberOfColumns: 2, titleText: '<em>Proposals </em>' },
          //{ startColumnName: 'LRCNNumber', numberOfColumns: 2, titleText: '<em>Core Network </em>' },
          //{ startColumnName: 'LRPNumber', numberOfColumns: 2, titleText: '<em>Proposals </em>' },
          //{ startColumnName: 'TCNNumber', numberOfColumns: 2, titleText: '<em>Core Network </em>' },
          //{ startColumnName: 'TPNumber', numberOfColumns: 2, titleText: '<em>Proposals </em>' }
        {
            startColumnName: 'TRCNNumber', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                    '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Through Route </td>  </tr>' +
                    '<tr>' +
                        '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Core Network</td>' +

                        '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Proposals</td>' +
                    '</tr>' +
                    '</table>'
        },

        {
            startColumnName: 'LRCNNumber', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                    '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Link Route </td>  </tr>' +
                    '<tr>' +
                        '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Core Network</td>' +

                        '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Proposals</td>' +
                    '</tr>' +
                    '</table>'
        },
        {
            startColumnName: 'TCNNumber', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                    '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Total </td>  </tr>' +
                    '<tr>' +
                        '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Core Network</td>' +

                        '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Proposals</td>' +
                    '</tr>' +
                    '</table>'
        },
        ]
    });
}
/**/

/*       DISTRICT REPORT LISTING       */
function CNR2DistrictReportListing(stateCode, stateName, route, roadCategory) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNR2StateReportTable").jqGrid('setSelection', stateCode);
    $("#CNR2StateReportTable").jqGrid('setGridState', 'hidden');
    $("#CNR2DistrictReportTable").jqGrid('GridUnload');
    $("#CNR2BlockReportTable").jqGrid('GridUnload');
    $("#CNR2FinalReportTable").jqGrid('GridUnload');

    $("#CNR2DistrictReportTable").jqGrid({
        url: '/CoreNetworkReports/CNR2DistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Number', 'Length', 'Number', 'Length ', 'Number', 'Length ', 'Number', 'Length ', 'Number', 'Length', 'Number', 'Length', 'Balance Length'],
        colModel: [
            { name: 'DistrictName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'TRCNNumber', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRCNLength', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRPNumber', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRPLength', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRCNNumber', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRCNLength', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRPNumber', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRPLength', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TCNNumber', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TCNLength', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TPNumber', width: 120, align: 'right', height: 'auto', formatter: 'integer', sortable: false, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TPLength', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TBALLength', width: 120, align: 'right', height: 'auto', formatter: 'number', sortable: false, formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } }


        ],
        pager: $("#CNR2DistrictReportPager"),
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
            var TRCNNumberT = $(this).jqGrid('getCol', 'TRCNNumber', false, 'sum');
            var TRCNLengthT = $(this).jqGrid('getCol', 'TRCNLength', false, 'sum');
            TRCNLengthT = parseFloat(TRCNLengthT).toFixed(3);
            var TRPNumberT = $(this).jqGrid('getCol', 'TRPNumber', false, 'sum');
            var TRPLengthT = $(this).jqGrid('getCol', 'TRPLength', false, 'sum');
            TRPLengthT = parseFloat(TRPLengthT).toFixed(3);
            var LRCNNumberT = $(this).jqGrid('getCol', 'LRCNNumber', false, 'sum');
            var LRCNLengthT = $(this).jqGrid('getCol', 'LRCNLength', false, 'sum');
            LRCNLengthT = parseFloat(LRCNLengthT).toFixed(3);
            var LRPNumberT = $(this).jqGrid('getCol', 'LRPNumber', false, 'sum');
            var LRPLengthT = $(this).jqGrid('getCol', 'LRPLength', false, 'sum');
            LRPLengthT = parseFloat(LRPLengthT).toFixed(3);
            var TCNNumberT = $(this).jqGrid('getCol', 'TCNNumber', false, 'sum');
            var TCNLengthT = $(this).jqGrid('getCol', 'TCNLength', false, 'sum');
            TCNLengthT = parseFloat(TCNLengthT).toFixed(3);
            var TPNumberT = $(this).jqGrid('getCol', 'TPNumber', false, 'sum');
            var TPLengthT = $(this).jqGrid('getCol', 'TPLength', false, 'sum');
            TPLengthT = parseFloat(TPLengthT).toFixed(3);
            var TBALLengthT = $(this).jqGrid('getCol', 'TBALLength', false, 'sum');
            TBALLengthT = parseFloat(TBALLengthT).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TRCNNumber: TRCNNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TRCNLength: TRCNLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TRPNumber: TRPNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TRPLength: TRPLengthT }, true);
            $(this).jqGrid('footerData', 'set', { LRCNNumber: LRCNNumberT }, true);
            $(this).jqGrid('footerData', 'set', { LRCNLength: LRCNLengthT }, true);
            $(this).jqGrid('footerData', 'set', { LRPNumber: LRPNumberT }, true);
            $(this).jqGrid('footerData', 'set', { LRPLength: LRPLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TCNNumber: TCNNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TCNLength: TCNLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TPNumber: TPNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TPLength: TPLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TBALLength: TBALLengthT }, true);
            $("#CNR2DistrictReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#CNR2DistrictReportTable_rn').html('Sr.<br/>No.');

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

    $("#CNR2DistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          //{ startColumnName: 'TRCNNumber', numberOfColumns: 2, titleText: '<em>Core Network </em>' },
          //{ startColumnName: 'TRPNumber', numberOfColumns: 2, titleText: '<em>Proposals </em>' },
          //{ startColumnName: 'LRCNNumber', numberOfColumns: 2, titleText: '<em>Core Network </em>' },
          //{ startColumnName: 'LRPNumber', numberOfColumns: 2, titleText: '<em>Proposals </em>' },
          //{ startColumnName: 'TCNNumber', numberOfColumns: 2, titleText: '<em>Core Network </em>' },
          //{ startColumnName: 'TPNumber', numberOfColumns: 2, titleText: '<em>Proposals </em>' }
        {
            startColumnName: 'TRCNNumber', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                    '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Through Route </td>  </tr>' +
                    '<tr>' +
                        '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Core Network</td>' +

                        '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Proposals</td>' +
                    '</tr>' +
                    '</table>'
        },

        {
            startColumnName: 'LRCNNumber', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                    '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Link Route </td>  </tr>' +
                    '<tr>' +
                        '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Core Network</td>' +

                        '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Proposals</td>' +
                    '</tr>' +
                    '</table>'
        },
        {
            startColumnName: 'TCNNumber', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                    '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Total </td>  </tr>' +
                    '<tr>' +
                        '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Core Network</td>' +

                        '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Proposals</td>' +
                    '</tr>' +
                    '</table>'
        },
        ]

    });
}
/**/

/*      BLOCK REPORT LISTING       */

function CNR2BlockReportListing(stateCode, districtCode, districtName, route, roadCategory) {

    var distname;
    if (districtName == '')
        distname = $("#DISTRICT_NAME").val();
    else
        distname = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNR2DistrictReportTable").jqGrid('setSelection', districtCode);
    $("#CNR2DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#CNR2StateReportTable").jqGrid('setGridState', 'hidden');
    $("#CNR2BlockReportTable").jqGrid('GridUnload');
    $("#CNR2FinalReportTable").jqGrid('GridUnload');

    $("#CNR2BlockReportTable").jqGrid({
        url: '/CoreNetworkReports/CNR2BlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Number', 'Length', 'Number', 'Length ', 'Number', 'Length ', 'Number', 'Length ', 'Number', 'Length', 'Number', 'Length', 'Balance Length'],
        colModel: [
            { name: 'BlockName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'TRCNNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRCNLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRPNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRPLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRCNNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRCNLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRPNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRPLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TCNNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TCNLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TPNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TPLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TBALLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } }


        ],
        pager: $("#CNR2BlockReportPager"),
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
            var TRCNNumberT = $(this).jqGrid('getCol', 'TRCNNumber', false, 'sum');
            var TRCNLengthT = $(this).jqGrid('getCol', 'TRCNLength', false, 'sum');
            TRCNLengthT = parseFloat(TRCNLengthT).toFixed(3);
            var TRPNumberT = $(this).jqGrid('getCol', 'TRPNumber', false, 'sum');
            var TRPLengthT = $(this).jqGrid('getCol', 'TRPLength', false, 'sum');
            TRPLengthT = parseFloat(TRPLengthT).toFixed(3);
            var LRCNNumberT = $(this).jqGrid('getCol', 'LRCNNumber', false, 'sum');
            var LRCNLengthT = $(this).jqGrid('getCol', 'LRCNLength', false, 'sum');
            LRCNLengthT = parseFloat(LRCNLengthT).toFixed(3);
            var LRPNumberT = $(this).jqGrid('getCol', 'LRPNumber', false, 'sum');
            var LRPLengthT = $(this).jqGrid('getCol', 'LRPLength', false, 'sum');
            LRPLengthT = parseFloat(LRPLengthT).toFixed(3);
            var TCNNumberT = $(this).jqGrid('getCol', 'TCNNumber', false, 'sum');
            var TCNLengthT = $(this).jqGrid('getCol', 'TCNLength', false, 'sum');
            TCNLengthT = parseFloat(TCNLengthT).toFixed(3);
            var TPNumberT = $(this).jqGrid('getCol', 'TPNumber', false, 'sum');
            var TPLengthT = $(this).jqGrid('getCol', 'TPLength', false, 'sum');
            TPLengthT = parseFloat(TPLengthT).toFixed(3);
            var TBALLengthT = $(this).jqGrid('getCol', 'TBALLength', false, 'sum');
            TBALLengthT = parseFloat(TBALLengthT).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TRCNNumber: TRCNNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TRCNLength: TRCNLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TRPNumber: TRPNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TRPLength: TRPLengthT }, true);
            $(this).jqGrid('footerData', 'set', { LRCNNumber: LRCNNumberT }, true);
            $(this).jqGrid('footerData', 'set', { LRCNLength: LRCNLengthT }, true);
            $(this).jqGrid('footerData', 'set', { LRPNumber: LRPNumberT }, true);
            $(this).jqGrid('footerData', 'set', { LRPLength: LRPLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TCNNumber: TCNNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TCNLength: TCNLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TPNumber: TPNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TPLength: TPLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TBALLength: TBALLengthT }, true);
            $("#CNR2BlockReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#CNR2BlockReportTable_rn').html('Sr.<br/>No.');

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

    $("#CNR2BlockReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          //{ startColumnName: 'TRCNNumber', numberOfColumns: 2, titleText: '<em>Core Network </em>' },
          //{ startColumnName: 'TRPNumber', numberOfColumns: 2, titleText: '<em>Proposals </em>' },
          //{ startColumnName: 'LRCNNumber', numberOfColumns: 2, titleText: '<em>Core Network </em>' },
          //{ startColumnName: 'LRPNumber', numberOfColumns: 2, titleText: '<em>Proposals </em>' },
          //{ startColumnName: 'TCNNumber', numberOfColumns: 2, titleText: '<em>Core Network </em>' },
          //{ startColumnName: 'TPNumber', numberOfColumns: 2, titleText: '<em>Proposals </em>' }
        {
            startColumnName: 'TRCNNumber', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                    '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Through Route </td>  </tr>' +
                    '<tr>' +
                        '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Core Network</td>' +

                        '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Proposals</td>' +
                    '</tr>' +
                    '</table>'
        },

        {
            startColumnName: 'LRCNNumber', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                    '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Link Route </td>  </tr>' +
                    '<tr>' +
                        '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Core Network</td>' +

                        '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Proposals</td>' +
                    '</tr>' +
                    '</table>'
        },
        {
            startColumnName: 'TCNNumber', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                    '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Total </td>  </tr>' +
                    '<tr>' +
                        '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Core Network</td>' +

                        '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Proposals</td>' +
                    '</tr>' +
                    '</table>'
        },
        ]

    });
}

/*       FINAL BLOCK REPORT LISTING       */

function CNR2FinalReportListing(blockCode, districtCode, stateCode, blockName, route, roadCategory) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNR2BlockReportTable").jqGrid('setGridState', 'hidden');
    $("#CNR2DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#CNR2StateReportTable").jqGrid('setGridState', 'hidden');
    $("#CNR2BlockReportTable").jqGrid('setSelection', blockCode);
    $("#CNR2FinalReportTable").jqGrid('GridUnload');

    $("#CNR2FinalReportTable").jqGrid({
        url: '/CoreNetworkReports/CNR2FinalReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
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

        postData: { "BlockCode": blockCode, "StateCode": stateCode, "DistrictCode": districtCode, "Route": route, "RoadCategory": roadCategory },
        rowNum: '2147483647',
        pager: $("#CNR2FinalReportPager"),
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
            var NoHablitisationT = $(this).jqGrid('getCol', 'NoHablitisation', false, 'sum');
            var TotHabitisationT = $(this).jqGrid('getCol', 'TotHabitisation', false, 'sum');


            $(this).jqGrid('footerData', 'set', { PlannedRoadNumber: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { RoadLength: RoadLengthT });
            $(this).jqGrid('footerData', 'set', { LengthCovered: LengthCoveredT });
            $(this).jqGrid('footerData', 'set', { NoHablitisation: NoHablitisationT });
            $(this).jqGrid('footerData', 'set', { TotHabitisation: TotHabitisationT });
            $("#CNR2FinalReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#CNR2FinalReportTable_rn').html('Sr.<br/>No.');

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