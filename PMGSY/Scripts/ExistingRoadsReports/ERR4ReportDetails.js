$(document).ready(function () {
    if ($("#hdnLevelId").val() == 6) //mord
    {
    
        ERR4StateReportListing();
    }
    else if ($("#hdnLevelId").val() == 4) //state
    {
         ERR4DistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val());
    }
    else if ($("#hdnLevelId").val() == 5) //District
    {
        ERR4BlockReportListing($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val(), $("#BLOCK_NAME").val());
    }
    //$('#tblRptContents').bind('resize', function () {
    //    resizeJqGrid();
    //}).trigger('resize');
});


/*       STATE REPORT LISTING       */
function ERR4StateReportListing() {
    $("#ERR4FinalReportTable").jqGrid('GridUnload'); $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR4StateReportTable").jqGrid('GridUnload');
    $("#ERR4DistrictReportTable").jqGrid('GridUnload');
    $("#ERR4BlockReportTable").jqGrid('GridUnload');
    $("#ERR4StateReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR4StateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Good', 'Fair', 'Bad', 'Total', 'Good', 'Fair', 'Bad', 'Total', 'Good', 'Fair', 'Bad', 'Total', 'Good', 'Fair', 'Bad', 'Total', 'Good', 'Fair', 'Bad', 'Total', 'Good', 'Fair', 'Bad', 'Total'],
        colModel: [
            { name: 'StateName', width: 200, align: 'left',  height: 'auto', sortable: true },
            { name: 'BTGood', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'BTFair', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'BTBad', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'BTTotal', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBMGood', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBMFair', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBMBad', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBMTotal', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GravelGood', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GravelFair', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GravelBad', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GravelTotal', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TrackGood', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TrackFair', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TrackBad', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TrackTotal', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'OtherGood', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'OtherFair', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'OtherBad', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'OtherTotal', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TotalGood', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TotalFair', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TotalBad', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GrandTotal', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } }

        ],
        pager: $("#ERR4StateReportPager"),
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'StateName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: false,
        height: '580',
        width: '1120',
        shrinkToFit: false,
        viewrecords: true,
        caption: 'State  DRRP Surface Wise Length Details',
        loadComplete: function () {
            //Total of Columns
            var BTGoodT = $(this).jqGrid('getCol', 'BTGood', false, 'sum');
            BTGoodT = parseFloat(BTGoodT).toFixed(3);
            var BTFairT = $(this).jqGrid('getCol', 'BTFair', false, 'sum');
            BTFairT = parseFloat(BTFairT).toFixed(3);
            var BTBadT = $(this).jqGrid('getCol', 'BTBad', false, 'sum');
            BTBadT = parseFloat(BTBadT).toFixed(3);
            var BTTotalT = $(this).jqGrid('getCol', 'BTTotal', false, 'sum');
            BTTotalT = parseFloat(BTTotalT).toFixed(3);

            var WBMGoodT = $(this).jqGrid('getCol', 'WBMGood', false, 'sum');
            WBMGoodT = parseFloat(WBMGoodT).toFixed(3);
            var WBMFairT = $(this).jqGrid('getCol', 'WBMFair', false, 'sum');
            WBMFairT = parseFloat(WBMFairT).toFixed(3);
            var WBMBadT = $(this).jqGrid('getCol', 'WBMBad', false, 'sum');
            WBMBadT = parseFloat(WBMBadT).toFixed(3);
            var WBMTotalT = $(this).jqGrid('getCol', 'WBMTotal', false, 'sum');
            WBMTotalT = parseFloat(WBMTotalT).toFixed(3);

            var GravelGoodT = $(this).jqGrid('getCol', 'GravelGood', false, 'sum');
            GravelGoodT = parseFloat(GravelGoodT).toFixed(3);
            var GravelFairT = $(this).jqGrid('getCol', 'GravelFair', false, 'sum');
            GravelFairT = parseFloat(GravelFairT).toFixed(3);
            var GravelBadT = $(this).jqGrid('getCol', 'GravelBad', false, 'sum');
            GravelBadT = parseFloat(GravelBadT).toFixed(3);
            var GravelTotalT = $(this).jqGrid('getCol', 'GravelTotal', false, 'sum');
            GravelTotalT = parseFloat(GravelTotalT).toFixed(3);

            var TrackGoodT = $(this).jqGrid('getCol', 'TrackGood', false, 'sum');
            TrackGoodT = parseFloat(TrackGoodT).toFixed(3);
            var TrackFairT = $(this).jqGrid('getCol', 'TrackFair', false, 'sum');
            TrackFairT = parseFloat(TrackFairT).toFixed(3);
            var TrackBadT = $(this).jqGrid('getCol', 'TrackBad', false, 'sum');
            TrackBadT = parseFloat(TrackBadT).toFixed(3);
            var TrackTotalT = $(this).jqGrid('getCol', 'TrackTotal', false, 'sum');
            TrackTotalT = parseFloat(TrackTotalT).toFixed(3);

            var OtherGoodT = $(this).jqGrid('getCol', 'OtherGood', false, 'sum');
            OtherGoodT = parseFloat(OtherGoodT).toFixed(3);
            var OtherFairT = $(this).jqGrid('getCol', 'OtherFair', false, 'sum');
            OtherFairT = parseFloat(OtherFairT).toFixed(3);
            var OtherBadT = $(this).jqGrid('getCol', 'OtherBad', false, 'sum');
            OtherBadT = parseFloat(OtherBadT).toFixed(3);
            var OtherTotalT = $(this).jqGrid('getCol', 'OtherTotal', false, 'sum');
            OtherTotalT = parseFloat(OtherTotalT).toFixed(3);

            var TotalGoodT = $(this).jqGrid('getCol', 'TotalGood', false, 'sum');
            TotalGoodT = parseFloat(TotalGoodT).toFixed(3);
            var TotalFairT = $(this).jqGrid('getCol', 'TotalFair', false, 'sum');
            TotalFairT = parseFloat(TotalFairT).toFixed(3);
            var TotalBadT = $(this).jqGrid('getCol', 'TotalBad', false, 'sum');
            TotalBadT = parseFloat(TotalBadT).toFixed(3);
            var GrandTotalT = $(this).jqGrid('getCol', 'GrandTotal', false, 'sum');
            GrandTotalT = parseFloat(GrandTotalT).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { BTGood: BTGoodT }, true);
            $(this).jqGrid('footerData', 'set', { BTFair: BTFairT }, true);
            $(this).jqGrid('footerData', 'set', { BTBad: BTBadT }, true);
            $(this).jqGrid('footerData', 'set', { BTTotal: BTTotalT }, true);

            $(this).jqGrid('footerData', 'set', { WBMGood: WBMGoodT }, true);
            $(this).jqGrid('footerData', 'set', { WBMFair: WBMFairT }, true);
            $(this).jqGrid('footerData', 'set', { WBMBad: WBMBadT }, true);
            $(this).jqGrid('footerData', 'set', { WBMTotal: WBMTotalT }, true);

            $(this).jqGrid('footerData', 'set', { GravelGood: GravelGoodT }, true);
            $(this).jqGrid('footerData', 'set', { GravelFair: GravelFairT }, true);
            $(this).jqGrid('footerData', 'set', { GravelBad: GravelBadT }, true);
            $(this).jqGrid('footerData', 'set', { GravelTotal: GravelTotalT }, true);

            $(this).jqGrid('footerData', 'set', { TrackGood: TrackGoodT }, true);
            $(this).jqGrid('footerData', 'set', { TrackFair: TrackFairT }, true);
            $(this).jqGrid('footerData', 'set', { TrackBad: TrackBadT }, true);
            $(this).jqGrid('footerData', 'set', { TrackTotal: TrackTotalT }, true);

            $(this).jqGrid('footerData', 'set', { OtherGood: OtherGoodT }, true);
            $(this).jqGrid('footerData', 'set', { OtherFair: OtherFairT }, true);
            $(this).jqGrid('footerData', 'set', { OtherBad: OtherBadT }, true);
            $(this).jqGrid('footerData', 'set', { OtherTotal: OtherTotalT }, true);

            $(this).jqGrid('footerData', 'set', { TotalGood: TotalGoodT }, true);
            $(this).jqGrid('footerData', 'set', { TotalFair: TotalFairT }, true);
            $(this).jqGrid('footerData', 'set', { TotalBad: TotalBadT }, true);
            $(this).jqGrid('footerData', 'set', { GrandTotal: GrandTotalT }, true);
            $("#ERR4StateReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#ERR4StateReportTable_rn').html('Sr.<br/>No.');

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

    $("#ERR4StateReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          //{ startColumnName: 'BTGood', numberOfColumns: 4, titleText: '<em>BT</em>' },
          //{ startColumnName: 'WBMGood', numberOfColumns: 4, titleText: '<em>WBM </em>' },
          //{ startColumnName: 'GravelGood', numberOfColumns: 4, titleText: '<em>Gravel </em>' },
          //{ startColumnName: 'TrackGood', numberOfColumns: 4, titleText: '<em>Track</em>' },
          // { startColumnName: 'OtherGood', numberOfColumns: 4, titleText: '<em>Total </em>' }
        {
            startColumnName: 'BTGood', numberOfColumns: 24,
            titleText: '<table style="width:100%;border-spacing:0px"' +
            '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="24">Surface Type (Length in Kms) </td>  </tr>' +
            '<tr>' +
                '<td id="h1" colspan="4" style="width: 16.6%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">BT</td>' +
                '<td id="h1" colspan="4" style="width: 16.6%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">WBM</td>' +
                '<td id="h1" colspan="4" style="width: 16.6%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Gravel</td>' +
                 '<td id="h1" colspan="4" style="width: 16.6%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Track</td>' +
                 '<td id="h1" colspan="4" style="width: 16.6%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Other Total</td>' +
                  '<td id="h2" colspan="4" style="width: 16.6%;  border-right-color: inherit;  padding: 4px 0px;">Total</td>' +

            '</tr>' +
            '</table>'
         },
        ]
    });
}
/**/

/*       DISTRICT REPORT LISTING       */
function ERR4DistrictReportListing(stateCode, stateName) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR4StateReportTable").jqGrid('setSelection', stateCode);
    $("#ERR4StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR4DistrictReportTable").jqGrid('GridUnload');
    $("#ERR4BlockReportTable").jqGrid('GridUnload');
    $("#ERR4FinalReportTable").jqGrid('GridUnload');

    $("#ERR4DistrictReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR4DistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Good', 'Fair', 'Bad', 'Total', 'Good', 'Fair', 'Bad', 'Total', 'Good', 'Fair', 'Bad', 'Total', 'Good', 'Fair', 'Bad', 'Total', 'Good', 'Fair', 'Bad', 'Total', 'Good', 'Fair', 'Bad', 'Total'],
        colModel: [
            { name: 'DistrictName', width: 200, align: 'left',  height: 'auto', sortable: true },
            { name: 'BTGood', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'BTFair', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'BTBad', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'BTTotal', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBMGood', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBMFair', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBMBad', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBMTotal', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GravelGood', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GravelFair', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GravelBad', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GravelTotal', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TrackGood', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TrackFair', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TrackBad', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TrackTotal', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'OtherGood', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'OtherFair', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'OtherBad', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'OtherTotal', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TotalGood', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TotalFair', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TotalBad', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GrandTotal', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } }

        ],

        pager: $("#ERR4DistrictReportPager"),
        postData: { 'StateCode': stateCode },
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'DistrictName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: false,
        height: '550',
        width: '1120',
        shrinkToFit: false,
        viewrecords: true,
        caption: 'District  DRRP Surface Wise Length Details for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var BTGoodT = $(this).jqGrid('getCol', 'BTGood', false, 'sum');
            BTGoodT = parseFloat(BTGoodT).toFixed(3);
            var BTFairT = $(this).jqGrid('getCol', 'BTFair', false, 'sum');
            BTFairT = parseFloat(BTFairT).toFixed(3);
            var BTBadT = $(this).jqGrid('getCol', 'BTBad', false, 'sum');
            BTBadT = parseFloat(BTBadT).toFixed(3);
            var BTTotalT = $(this).jqGrid('getCol', 'BTTotal', false, 'sum');
            BTTotalT = parseFloat(BTTotalT).toFixed(3);

            var WBMGoodT = $(this).jqGrid('getCol', 'WBMGood', false, 'sum');
            WBMGoodT = parseFloat(WBMGoodT).toFixed(3);
            var WBMFairT = $(this).jqGrid('getCol', 'WBMFair', false, 'sum');
            WBMFairT = parseFloat(WBMFairT).toFixed(3);
            var WBMBadT = $(this).jqGrid('getCol', 'WBMBad', false, 'sum');
            WBMBadT = parseFloat(WBMBadT).toFixed(3);
            var WBMTotalT = $(this).jqGrid('getCol', 'WBMTotal', false, 'sum');
            WBMTotalT = parseFloat(WBMTotalT).toFixed(3);

            var GravelGoodT = $(this).jqGrid('getCol', 'GravelGood', false, 'sum');
            GravelGoodT = parseFloat(GravelGoodT).toFixed(3);
            var GravelFairT = $(this).jqGrid('getCol', 'GravelFair', false, 'sum');
            GravelFairT = parseFloat(GravelFairT).toFixed(3);
            var GravelBadT = $(this).jqGrid('getCol', 'GravelBad', false, 'sum');
            GravelBadT = parseFloat(GravelBadT).toFixed(3);
            var GravelTotalT = $(this).jqGrid('getCol', 'GravelTotal', false, 'sum');
            GravelTotalT = parseFloat(GravelTotalT).toFixed(3);

            var TrackGoodT = $(this).jqGrid('getCol', 'TrackGood', false, 'sum');
            TrackGoodT = parseFloat(TrackGoodT).toFixed(3);
            var TrackFairT = $(this).jqGrid('getCol', 'TrackFair', false, 'sum');
            TrackFairT = parseFloat(TrackFairT).toFixed(3);
            var TrackBadT = $(this).jqGrid('getCol', 'TrackBad', false, 'sum');
            TrackBadT = parseFloat(TrackBadT).toFixed(3);
            var TrackTotalT = $(this).jqGrid('getCol', 'TrackTotal', false, 'sum');
            TrackTotalT = parseFloat(TrackTotalT).toFixed(3);

            var OtherGoodT = $(this).jqGrid('getCol', 'OtherGood', false, 'sum');
            OtherGoodT = parseFloat(OtherGoodT).toFixed(3);
            var OtherFairT = $(this).jqGrid('getCol', 'OtherFair', false, 'sum');
            OtherFairT = parseFloat(OtherFairT).toFixed(3);
            var OtherBadT = $(this).jqGrid('getCol', 'OtherBad', false, 'sum');
            OtherBadT = parseFloat(OtherBadT).toFixed(3);
            var OtherTotalT = $(this).jqGrid('getCol', 'OtherTotal', false, 'sum');
            OtherTotalT = parseFloat(OtherTotalT).toFixed(3);

            var TotalGoodT = $(this).jqGrid('getCol', 'TotalGood', false, 'sum');
            TotalGoodT = parseFloat(TotalGoodT).toFixed(3);
            var TotalFairT = $(this).jqGrid('getCol', 'TotalFair', false, 'sum');
            TotalFairT = parseFloat(TotalFairT).toFixed(3);
            var TotalBadT = $(this).jqGrid('getCol', 'TotalBad', false, 'sum');
            TotalBadT = parseFloat(TotalBadT).toFixed(3);
            var GrandTotalT = $(this).jqGrid('getCol', 'GrandTotal', false, 'sum');
            GrandTotalT = parseFloat(GrandTotalT).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { BTGood: BTGoodT }, true);
            $(this).jqGrid('footerData', 'set', { BTFair: BTFairT }, true);
            $(this).jqGrid('footerData', 'set', { BTBad: BTBadT }, true);
            $(this).jqGrid('footerData', 'set', { BTTotal: BTTotalT }, true);

            $(this).jqGrid('footerData', 'set', { WBMGood: WBMGoodT }, true);
            $(this).jqGrid('footerData', 'set', { WBMFair: WBMFairT }, true);
            $(this).jqGrid('footerData', 'set', { WBMBad: WBMBadT }, true);
            $(this).jqGrid('footerData', 'set', { WBMTotal: WBMTotalT }, true);

            $(this).jqGrid('footerData', 'set', { GravelGood: GravelGoodT }, true);
            $(this).jqGrid('footerData', 'set', { GravelFair: GravelFairT }, true);
            $(this).jqGrid('footerData', 'set', { GravelBad: GravelBadT }, true);
            $(this).jqGrid('footerData', 'set', { GravelTotal: GravelTotalT }, true);

            $(this).jqGrid('footerData', 'set', { TrackGood: TrackGoodT }, true);
            $(this).jqGrid('footerData', 'set', { TrackFair: TrackFairT }, true);
            $(this).jqGrid('footerData', 'set', { TrackBad: TrackBadT }, true);
            $(this).jqGrid('footerData', 'set', { TrackTotal: TrackTotalT }, true);

            $(this).jqGrid('footerData', 'set', { OtherGood: OtherGoodT }, true);
            $(this).jqGrid('footerData', 'set', { OtherFair: OtherFairT }, true);
            $(this).jqGrid('footerData', 'set', { OtherBad: OtherBadT }, true);
            $(this).jqGrid('footerData', 'set', { OtherTotal: OtherTotalT }, true);

            $(this).jqGrid('footerData', 'set', { TotalGood: TotalGoodT }, true);
            $(this).jqGrid('footerData', 'set', { TotalFair: TotalFairT }, true);
            $(this).jqGrid('footerData', 'set', { TotalBad: TotalBadT }, true);
            $(this).jqGrid('footerData', 'set', { GrandTotal: GrandTotalT }, true);
            $("#ERR4DistrictReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#ERR4DistrictReportTable_rn').html('Sr.<br/>No.');

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

    $("#ERR4DistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
           //{ startColumnName: 'BTGood', numberOfColumns: 4, titleText: '<em>BT</em>' },
           //{ startColumnName: 'WBMGood', numberOfColumns: 4, titleText: '<em>WBM </em>' },
           //{ startColumnName: 'GravelGood', numberOfColumns: 4, titleText: '<em>Gravel </em>' },
           //{ startColumnName: 'TrackGood', numberOfColumns: 4, titleText: '<em>Track</em>' },
           // { startColumnName: 'OtherGood', numberOfColumns: 4, titleText: '<em>Total </em>' }


          {
              startColumnName: 'BTGood', numberOfColumns: 24,
              titleText: '<table style="width:100%;border-spacing:0px"' +
              '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="24">Surface Type (Length in Kms) </td>  </tr>' +
              '<tr>' +
                  '<td id="h1" colspan="4" style="width: 16.6%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">BT</td>' +
                  '<td id="h1" colspan="4" style="width: 16.6%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">WBM</td>' +
                  '<td id="h1" colspan="4" style="width: 16.6%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Gravel</td>' +
                   '<td id="h1" colspan="4" style="width: 16.6%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Track</td>' +
                   '<td id="h1" colspan="4" style="width: 16.6%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Other Total</td>' +
                    '<td id="h2" colspan="4" style="width: 16.6%;  border-right-color: inherit;  padding: 4px 0px;">Total</td>' +

              '</tr>' +
              '</table>'
          },
        ]
    });
}
/**/

/*      BLOCK REPORT LISTING       */

function ERR4BlockReportListing(stateCode, districtCode, districtName) {

    var distname;
    if (districtName == '')
        distname = $("#DISTRICT_NAME").val();
    else
        distname = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR4DistrictReportTable").jqGrid('setSelection', districtCode);
    $("#ERR4DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR4StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR4BlockReportTable").jqGrid('GridUnload');
    $("#ERR4FinalReportTable").jqGrid('GridUnload');

    $("#ERR4BlockReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR4BlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Good', 'Fair', 'Bad', 'Total', 'Good', 'Fair', 'Bad', 'Total', 'Good', 'Fair', 'Bad', 'Total', 'Good', 'Fair', 'Bad', 'Total', 'Good', 'Fair', 'Bad', 'Total', 'Good', 'Fair', 'Bad', 'Total'],
        colModel: [
            { name: 'BlockName', width: 200, align: 'left',  height: 'auto', sortable: true },
            { name: 'BTGood', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'BTFair', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'BTBad', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'BTTotal', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBMGood', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBMFair', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBMBad', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'WBMTotal', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GravelGood', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GravelFair', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GravelBad', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GravelTotal', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TrackGood', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TrackFair', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TrackBad', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TrackTotal', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'OtherGood', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'OtherFair', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'OtherBad', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'OtherTotal', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TotalGood', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TotalFair', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'TotalBad', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'GrandTotal', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } }

        ],
        pager: $("#ERR4BlockReportPager"),
        postData: { 'StateCode': stateCode, 'DistrictCode': districtCode },
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'BlockName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: false,
        height: '500',
        width: '1120',
        shrinkToFit: false,
        viewrecords: true,
        caption: 'Block  DRRP Surface Wise Length Details for ' + distname,
        loadComplete: function () {
            //Total of Columns
            var BTGoodT = $(this).jqGrid('getCol', 'BTGood', false, 'sum');
            BTGoodT = parseFloat(BTGoodT).toFixed(3);
            var BTFairT = $(this).jqGrid('getCol', 'BTFair', false, 'sum');
            BTFairT = parseFloat(BTFairT).toFixed(3);
            var BTBadT = $(this).jqGrid('getCol', 'BTBad', false, 'sum');
            BTBadT = parseFloat(BTBadT).toFixed(3);
            var BTTotalT = $(this).jqGrid('getCol', 'BTTotal', false, 'sum');
            BTTotalT = parseFloat(BTTotalT).toFixed(3);

            var WBMGoodT = $(this).jqGrid('getCol', 'WBMGood', false, 'sum');
            WBMGoodT = parseFloat(WBMGoodT).toFixed(3);
            var WBMFairT = $(this).jqGrid('getCol', 'WBMFair', false, 'sum');
            WBMFairT = parseFloat(WBMFairT).toFixed(3);
            var WBMBadT = $(this).jqGrid('getCol', 'WBMBad', false, 'sum');
            WBMBadT = parseFloat(WBMBadT).toFixed(3);
            var WBMTotalT = $(this).jqGrid('getCol', 'WBMTotal', false, 'sum');
            WBMTotalT = parseFloat(WBMTotalT).toFixed(3);

            var GravelGoodT = $(this).jqGrid('getCol', 'GravelGood', false, 'sum');
            GravelGoodT = parseFloat(GravelGoodT).toFixed(3);
            var GravelFairT = $(this).jqGrid('getCol', 'GravelFair', false, 'sum');
            GravelFairT = parseFloat(GravelFairT).toFixed(3);
            var GravelBadT = $(this).jqGrid('getCol', 'GravelBad', false, 'sum');
            GravelBadT = parseFloat(GravelBadT).toFixed(3);
            var GravelTotalT = $(this).jqGrid('getCol', 'GravelTotal', false, 'sum');
            GravelTotalT = parseFloat(GravelTotalT).toFixed(3);

            var TrackGoodT = $(this).jqGrid('getCol', 'TrackGood', false, 'sum');
            TrackGoodT = parseFloat(TrackGoodT).toFixed(3);
            var TrackFairT = $(this).jqGrid('getCol', 'TrackFair', false, 'sum');
            TrackFairT = parseFloat(TrackFairT).toFixed(3);
            var TrackBadT = $(this).jqGrid('getCol', 'TrackBad', false, 'sum');
            TrackBadT = parseFloat(TrackBadT).toFixed(3);
            var TrackTotalT = $(this).jqGrid('getCol', 'TrackTotal', false, 'sum');
            TrackTotalT = parseFloat(TrackTotalT).toFixed(3);

            var OtherGoodT = $(this).jqGrid('getCol', 'OtherGood', false, 'sum');
            OtherGoodT = parseFloat(OtherGoodT).toFixed(3);
            var OtherFairT = $(this).jqGrid('getCol', 'OtherFair', false, 'sum');
            OtherFairT = parseFloat(OtherFairT).toFixed(3);
            var OtherBadT = $(this).jqGrid('getCol', 'OtherBad', false, 'sum');
            OtherBadT = parseFloat(OtherBadT).toFixed(3);
            var OtherTotalT = $(this).jqGrid('getCol', 'OtherTotal', false, 'sum');
            OtherTotalT = parseFloat(OtherTotalT).toFixed(3);

            var TotalGoodT = $(this).jqGrid('getCol', 'TotalGood', false, 'sum');
            TotalGoodT = parseFloat(TotalGoodT).toFixed(3);
            var TotalFairT = $(this).jqGrid('getCol', 'TotalFair', false, 'sum');
            TotalFairT = parseFloat(TotalFairT).toFixed(3);
            var TotalBadT = $(this).jqGrid('getCol', 'TotalBad', false, 'sum');
            TotalBadT = parseFloat(TotalBadT).toFixed(3);
            var GrandTotalT = $(this).jqGrid('getCol', 'GrandTotal', false, 'sum');
            GrandTotalT = parseFloat(GrandTotalT).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { BTGood: BTGoodT }, true);
            $(this).jqGrid('footerData', 'set', { BTFair: BTFairT }, true);
            $(this).jqGrid('footerData', 'set', { BTBad: BTBadT }, true);
            $(this).jqGrid('footerData', 'set', { BTTotal: BTTotalT }, true);

            $(this).jqGrid('footerData', 'set', { WBMGood: WBMGoodT }, true);
            $(this).jqGrid('footerData', 'set', { WBMFair: WBMFairT }, true);
            $(this).jqGrid('footerData', 'set', { WBMBad: WBMBadT }, true);
            $(this).jqGrid('footerData', 'set', { WBMTotal: WBMTotalT }, true);

            $(this).jqGrid('footerData', 'set', { GravelGood: GravelGoodT }, true);
            $(this).jqGrid('footerData', 'set', { GravelFair: GravelFairT }, true);
            $(this).jqGrid('footerData', 'set', { GravelBad: GravelBadT }, true);
            $(this).jqGrid('footerData', 'set', { GravelTotal: GravelTotalT }, true);

            $(this).jqGrid('footerData', 'set', { TrackGood: TrackGoodT }, true);
            $(this).jqGrid('footerData', 'set', { TrackFair: TrackFairT }, true);
            $(this).jqGrid('footerData', 'set', { TrackBad: TrackBadT }, true);
            $(this).jqGrid('footerData', 'set', { TrackTotal: TrackTotalT }, true);

            $(this).jqGrid('footerData', 'set', { OtherGood: OtherGoodT }, true);
            $(this).jqGrid('footerData', 'set', { OtherFair: OtherFairT }, true);
            $(this).jqGrid('footerData', 'set', { OtherBad: OtherBadT }, true);
            $(this).jqGrid('footerData', 'set', { OtherTotal: OtherTotalT }, true);

            $(this).jqGrid('footerData', 'set', { TotalGood: TotalGoodT }, true);
            $(this).jqGrid('footerData', 'set', { TotalFair: TotalFairT }, true);
            $(this).jqGrid('footerData', 'set', { TotalBad: TotalBadT }, true);
            $(this).jqGrid('footerData', 'set', { GrandTotal: GrandTotalT }, true);
            $("#ERR4BlockReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#ERR4BlockReportTable_rn').html('Sr.<br/>No.');

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

    $("#ERR4BlockReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          //{ startColumnName: 'BTGood', numberOfColumns: 4, titleText: '<em>BT</em>' },
          //{ startColumnName: 'WBMGood', numberOfColumns: 4, titleText: '<em>WBM </em>' },
          //{ startColumnName: 'GravelGood', numberOfColumns: 4, titleText: '<em>Gravel </em>' },
          //{ startColumnName: 'TrackGood', numberOfColumns: 4, titleText: '<em>Track</em>' },
          // { startColumnName: 'OtherGood', numberOfColumns: 4, titleText: '<em>Total </em>' }


           {
               startColumnName: 'BTGood', numberOfColumns: 24,
               titleText: '<table style="width:100%;border-spacing:0px"' +
               '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="24">Surface Type (Length in Kms) </td>  </tr>' +
               '<tr>' +
                   '<td id="h1" colspan="4" style="width: 16.6%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">BT</td>' +
                   '<td id="h1" colspan="4" style="width: 16.6%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">WBM</td>' +
                   '<td id="h1" colspan="4" style="width: 16.6%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Gravel</td>' +
                    '<td id="h1" colspan="4" style="width: 16.6%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Track</td>' +
                    '<td id="h1" colspan="4" style="width: 16.6%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Other Total</td>' +
                     '<td id="h2" colspan="4" style="width: 16.6%;  border-right-color: inherit;  padding: 4px 0px;">Total</td>' +

               '</tr>' +
               '</table>'
           },
        ]
    });
}

/*       FINAL BLOCK REPORT LISTING       */

function ERR4FinalReportListing(blockCode, districtCode, stateCode, blockName) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR4BlockReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR4DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR4StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR4BlockReportTable").jqGrid('setSelection', blockCode);
    $("#ERR4FinalReportTable").jqGrid('GridUnload');

    $("#ERR4FinalReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR4FinalReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Road Code', 'Road Name', 'Road Category', 'Road Type', 'Road Length', 'Year of Construction', 'Included in Core Network (Y/N)', 'Soil type', 'Terrain Type','Length (Surface)'],
        colModel: [
            { name: 'PlannedRoadNumber', width: 120, align: 'left',  height: 'auto', sortable: true },
            { name: 'PlannedRoadName', width: 250, align: 'left',  height: 'auto', sortable: false },
            { name: 'MAST_ROAD_CAT_CODE', width: 100, align: 'left',  height: 'auto', sortable: false },
            { name: 'MAST_ER_ROAD_TYPE', width: 150, align: 'left',  height: 'auto', sortable: false },
            { name: 'ROAD_LENGTH', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'MAST_CONS_YEAR', width: 120, align: 'center',  height: 'auto', sortable: false },
            { name: 'MAST_CORE_NETWORK', width: 120, align: 'center',  height: 'auto', sortable: false },
            { name: 'MAST_SOIL_TYPE_NAME', width: 200, align: 'left',  height: 'auto', sortable: false },
            { name: 'MAST_TERRAIN_TYPE_NAME', width: 200, align: 'left',  height: 'auto', sortable: false },
            { name: 'Length_Surface', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } },

        ],

        postData: { "BlockCode": blockCode, "StateCode": stateCode, "DistrictCode": districtCode },
        rowNum: '2147483647',
        pager: $("#ERR4FinalReportPager"),
        footerrow: true,
        pgbuttons: true,
        sortname: 'PlannedRoadNumber',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '450',
        viewrecords: true,
        caption: 'DRRP Surface Wise Length Details for ' + blockName,
        loadComplete: function () {
            //Total of Columns
            var PlannedRoadlengthT = $(this).jqGrid('getCol', 'PlannedRoadlength', false, 'sum');
            PlannedRoadlengthT = parseFloat(PlannedRoadlengthT).toFixed(3);
            var RoadLengthT = $(this).jqGrid('getCol', 'ROAD_LENGTH', false, 'sum');
            PlannedRoadlengthT = parseFloat(PlannedRoadlengthT).toFixed(3);
            var NoHablitisationT = $(this).jqGrid('getCol', 'MAST_HAB_TOT_POP', false, 'sum');
            var Length_SurfaceT = $(this).jqGrid('getCol', 'Length_Surface', false, 'sum');
            Length_SurfaceT = parseFloat(Length_SurfaceT).toFixed(3);

            $(this).jqGrid('footerData', 'set', { PlannedRoadNumber: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { ROAD_LENGTH: RoadLengthT });
            $(this).jqGrid('footerData', 'set', { MAST_HAB_TOT_POP: NoHablitisationT });
            $(this).jqGrid('footerData', 'set', { Length_Surface: Length_SurfaceT });
            $("#ERR4FinalReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#ERR4FinalReportTable_rn').html('Sr.<br/>No.');

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