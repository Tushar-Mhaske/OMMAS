$(document).ready(function () {
    $('#ddTerrainType_ERR7').change(function () {
        var terrainType = $('#ddTerrainType_ERR7').val();
        loadLevelWiseGrid(terrainType);
    });

    $('#ddTerrainType_ERR7').trigger('change');
    //$('#tblRptContents').bind('resize', function () {
    //    resizeJqGrid();
    //}).trigger('resize');
});

function loadLevelWiseGrid(terrainType) {
    if ($("#hdnLevelId").val() == 6) //mord
    {
             ERR7StateReportListing(terrainType);
    }
    else if ($("#hdnLevelId").val() == 4) //state
    {
        $("#ERR7BlockReportTable").jqGrid('GridUnload');
        $("#ERR7FinalReportTable").jqGrid('GridUnload');
        ERR7DistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), terrainType);
    }
    else if ($("#hdnLevelId").val() == 5) //District
    {
        $("#ERR7FinalReportTable").jqGrid('GridUnload');
        ERR7BlockReportListing($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val(), $("#BLOCK_NAME").val(), terrainType);
    }
}

/*       STATE REPORT LISTING       */
function ERR7StateReportListing(terrainType) {
  
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });  
    $("#ERR7DistrictReportTable").jqGrid('GridUnload');
    $("#ERR7BlockReportTable").jqGrid('GridUnload');
    $("#ERR7FinalReportTable").jqGrid('GridUnload');
    $("#ERR7StateReportTable").jqGrid('GridUnload');

    $("#ERR7StateReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR7StateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length'],
        colModel: [
            { name: 'StateName', width: 200, align: 'left',  height: 'auto', sortable: true },
            { name: 'Road_Plain', width: 100, align: 'right',  height: 'auto', sortable: false,  formatter: 'integer', formatoptions: {thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Plain_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CPlain', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CPlain_Len', width: 100, align: 'right',  height: 'auto', sortable: false,  formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Roll', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: {thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Roll_Len', width: 100, align: 'right',  height: 'auto', sortable: false,  formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CRoll', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CRoll_Len', width: 100, align: 'right',  height: 'auto', sortable: false,  formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Hilly', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: {  thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Hilly_Len', width: 100, align: 'right',  height: 'auto', sortable: false,  formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CHilly', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CHilly_Len', width: 100, align: 'right',  height: 'auto', sortable: false,  formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Steep', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: {thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Steep_Len', width: 100, align: 'right',  height: 'auto', sortable: false,  formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CSteep', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CSteep_Len', width: 100, align: 'right',  height: 'auto', sortable: false,  formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Total', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: {  thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Total_Len', width: 100, align: 'right',  height: 'auto', sortable: false,  formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CTotal', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: {  thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CTotal_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",   decimalPlaces: 3, defaulValue: "N.A" } }

        ],
        postData: { "TerrainType": terrainType },
        pager: $("#ERR7StateReportPager"),
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'StateName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: false,
        height: '520',
        width: '1120',
        shrinkToFit: false,
        viewrecords: true,
        caption: 'State  DRRP based on terrian type Details',
        loadComplete: function () {
            //Total of Columns
            var Road_PlainT = $(this).jqGrid('getCol', 'Road_Plain', false, 'sum');
            var Road_Plain_LenT = $(this).jqGrid('getCol', 'Road_Plain_Len', false, 'sum');
            Road_Plain_LenT = parseFloat(Road_Plain_LenT).toFixed(3);
            var Road_CPlainT = $(this).jqGrid('getCol', 'Road_CPlain', false, 'sum');
            var Road_CPlain_LenT = $(this).jqGrid('getCol', 'Road_CPlain_Len', false, 'sum');
            Road_CPlain_LenT = parseFloat(Road_CPlain_LenT).toFixed(3);
            var Road_RollT = $(this).jqGrid('getCol', 'Road_Roll', false, 'sum');
            var Road_Roll_LenT = $(this).jqGrid('getCol', 'Road_Roll_Len', false, 'sum');
            Road_Roll_LenT = parseFloat(Road_Roll_LenT).toFixed(3);
            var Road_CRollT = $(this).jqGrid('getCol', 'Road_CRoll', false, 'sum');
            var Road_CRoll_LenT = $(this).jqGrid('getCol', 'Road_CRoll_Len', false, 'sum');
            Road_CRoll_LenT = parseFloat(Road_CRoll_LenT).toFixed(3);
            var Road_HillyT = $(this).jqGrid('getCol', 'Road_Hilly', false, 'sum');
            var Road_Hilly_LenT = $(this).jqGrid('getCol', 'Road_Hilly_Len', false, 'sum');
            Road_Hilly_LenT = parseFloat(Road_Hilly_LenT).toFixed(3);
            var Road_CHillyT = $(this).jqGrid('getCol', 'Road_CHilly', false, 'sum');
            var Road_CHilly_LenT = $(this).jqGrid('getCol', 'Road_CHilly_Len', false, 'sum');
            Road_CHilly_LenT = parseFloat(Road_CHilly_LenT).toFixed(3);
            var Road_SteepT = $(this).jqGrid('getCol', 'Road_Steep', false, 'sum');
            var Road_Steep_LenT = $(this).jqGrid('getCol', 'Road_Steep_Len', false, 'sum');
            Road_Steep_LenT = parseFloat(Road_Steep_LenT).toFixed(3);
            var Road_CSteepT = $(this).jqGrid('getCol', 'Road_CSteep', false, 'sum');
            var Road_CSteep_LenT = $(this).jqGrid('getCol', 'Road_CSteep_Len', false, 'sum');
            Road_CSteep_LenT = parseFloat(Road_CSteep_LenT).toFixed(3);
            var Road_TotalT = $(this).jqGrid('getCol', 'Road_Total', false, 'sum');
            var Road_Total_LenT = $(this).jqGrid('getCol', 'Road_Total_Len', false, 'sum');
            Road_Total_LenT = parseFloat(Road_Total_LenT).toFixed(3);
            var Road_CTotalT = $(this).jqGrid('getCol', 'Road_CTotal', false, 'sum');
            var Road_CTotal_LenT = $(this).jqGrid('getCol', 'Road_CTotal_Len', false, 'sum');
            Road_CTotal_LenT = parseFloat(Road_CTotal_LenT).toFixed(3);
            //

            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { Road_Plain: Road_PlainT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Plain_Len: Road_Plain_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CPlain: Road_CPlainT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CPlain_Len: Road_CPlain_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Roll: Road_RollT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Roll_Len: Road_Roll_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CRoll: Road_CRollT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CRoll_Len: Road_CRoll_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Hilly: Road_HillyT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Hilly_Len: Road_Hilly_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CHilly: Road_CHillyT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CHilly_Len: Road_CHilly_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Steep: Road_SteepT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Steep_Len: Road_Steep_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CSteep: Road_CSteepT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CSteep_Len: Road_CSteep_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Total: Road_TotalT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Total_Len: Road_Total_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CTotal: Road_CTotalT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CTotal_Len: Road_CTotal_LenT }, true);
            $("#ERR7StateReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#ERR7StateReportTable_rn').html('Sr.<br/>No.');

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

    $("#ERR7StateReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          //{ startColumnName: 'Road_Plain', numberOfColumns: 2, titleText: '<em>Plain DRRP</em>' },
          //{ startColumnName: 'Road_CPlain', numberOfColumns: 2, titleText: '<em>Plain Included in CN </em>' },
          //{ startColumnName: 'Road_Roll', numberOfColumns: 2, titleText: '<em>Rolling DRRP</em>' },
          //{ startColumnName: 'Road_CRoll', numberOfColumns: 2, titleText: '<em>Rolling Included in CN </em>' },
          //{ startColumnName: 'Road_Hilly', numberOfColumns: 2, titleText: '<em>Hilly DRRP</em>' },
          //{ startColumnName: 'Road_CHilly', numberOfColumns: 2, titleText: '<em>Hilly Included in CN </em>' },
          //{ startColumnName: 'Road_Steep', numberOfColumns: 2, titleText: '<em>Steep DRRP</em>' },
          //{ startColumnName: 'Road_CSteep', numberOfColumns: 2, titleText: '<em>Steep Included in CN </em>' },
          //{ startColumnName: 'Road_Total', numberOfColumns: 2, titleText: '<em>Total DRRP</em>' },
          //{ startColumnName: 'Road_CTotal', numberOfColumns: 2, titleText: '<em>Total Included in CN </em>' }
        {
            startColumnName: 'Road_Plain', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                      '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Plain</td>  </tr>' +
                      '<tr>' +
                          '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                          '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                      '</tr>' +
                      '</table>'
        },
        {
            startColumnName: 'Road_Roll', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                    '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Rolling</td>  </tr>' +
                    '<tr>' +
                        '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                        '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                    '</tr>' +
                    '</table>'
        },
        {
            startColumnName: 'Road_Hilly', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                    '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Hilly</td>  </tr>' +
                    '<tr>' +
                        '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                        '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                    '</tr>' +
                    '</table>'
        },
        {
            startColumnName: 'Road_Steep', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                    '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Steep</td>  </tr>' +
                    '<tr>' +
                        '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                        '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                    '</tr>' +
                    '</table>'
        },
        {
            startColumnName: 'Road_Total', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                    '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Total</td>  </tr>' +
                    '<tr>' +
                        '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                        '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                    '</tr>' +
                    '</table>'
        },
        ]
    });
}
/**/

/*       DISTRICT REPORT LISTING       */
function ERR7DistrictReportListing(stateCode, stateName, terrainType) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR7StateReportTable").jqGrid('setSelection', stateCode);
    $("#ERR7StateReportTable").jqGrid('setGridState', 'hidden');  
    $("#ERR7BlockReportTable").jqGrid('GridUnload');
    $("#ERR7FinalReportTable").jqGrid('GridUnload');
    $("#ERR7DistrictReportTable").jqGrid('GridUnload');

    $("#ERR7DistrictReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR7DistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length'],
        colModel: [
            { name: 'DistrictName', width: 200, align: 'left',  height: 'auto', sortable: true },
            { name: 'Road_Plain', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Plain_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CPlain', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CPlain_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Roll', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Roll_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CRoll', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CRoll_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Hilly', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Hilly_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CHilly', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CHilly_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Steep', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Steep_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CSteep', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CSteep_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Total', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Total_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CTotal', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CTotal_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } }
        ],
        pager: $("#ERR7DistrictReportPager"),
        postData: { 'StateCode': stateCode, "TerrainType": terrainType },
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'DistrictName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: false,
        height: '460',
        width: '1120',
        shrinkToFit: false,
        viewrecords: true,
        caption: 'District  DRRP based on terrian type Details for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var Road_PlainT = $(this).jqGrid('getCol', 'Road_Plain', false, 'sum');
            var Road_Plain_LenT = $(this).jqGrid('getCol', 'Road_Plain_Len', false, 'sum');
            Road_Plain_LenT = parseFloat(Road_Plain_LenT).toFixed(3);
            var Road_CPlainT = $(this).jqGrid('getCol', 'Road_CPlain', false, 'sum');
            var Road_CPlain_LenT = $(this).jqGrid('getCol', 'Road_CPlain_Len', false, 'sum');
            Road_CPlain_LenT = parseFloat(Road_CPlain_LenT).toFixed(3);
            var Road_RollT = $(this).jqGrid('getCol', 'Road_Roll', false, 'sum');
            var Road_Roll_LenT = $(this).jqGrid('getCol', 'Road_Roll_Len', false, 'sum');
            Road_Roll_LenT = parseFloat(Road_Roll_LenT).toFixed(3);
            var Road_CRollT = $(this).jqGrid('getCol', 'Road_CRoll', false, 'sum');
            var Road_CRoll_LenT = $(this).jqGrid('getCol', 'Road_CRoll_Len', false, 'sum');
            Road_CRoll_LenT = parseFloat(Road_CRoll_LenT).toFixed(3);
            var Road_HillyT = $(this).jqGrid('getCol', 'Road_Hilly', false, 'sum');
            var Road_Hilly_LenT = $(this).jqGrid('getCol', 'Road_Hilly_Len', false, 'sum');
            Road_Hilly_LenT = parseFloat(Road_Hilly_LenT).toFixed(3);
            var Road_CHillyT = $(this).jqGrid('getCol', 'Road_CHilly', false, 'sum');
            var Road_CHilly_LenT = $(this).jqGrid('getCol', 'Road_CHilly_Len', false, 'sum');
            Road_CHilly_LenT = parseFloat(Road_CHilly_LenT).toFixed(3);
            var Road_SteepT = $(this).jqGrid('getCol', 'Road_Steep', false, 'sum');
            var Road_Steep_LenT = $(this).jqGrid('getCol', 'Road_Steep_Len', false, 'sum');
            Road_Steep_LenT = parseFloat(Road_Steep_LenT).toFixed(3);
            var Road_CSteepT = $(this).jqGrid('getCol', 'Road_CSteep', false, 'sum');
            var Road_CSteep_LenT = $(this).jqGrid('getCol', 'Road_CSteep_Len', false, 'sum');
            Road_CSteep_LenT = parseFloat(Road_CSteep_LenT).toFixed(3);
            var Road_TotalT = $(this).jqGrid('getCol', 'Road_Total', false, 'sum');
            var Road_Total_LenT = $(this).jqGrid('getCol', 'Road_Total_Len', false, 'sum');
            Road_Total_LenT = parseFloat(Road_Total_LenT).toFixed(3);
            var Road_CTotalT = $(this).jqGrid('getCol', 'Road_CTotal', false, 'sum');
            var Road_CTotal_LenT = $(this).jqGrid('getCol', 'Road_CTotal_Len', false, 'sum');
            Road_CTotal_LenT = parseFloat(Road_CTotal_LenT).toFixed(3);
            //

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { Road_Plain: Road_PlainT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Plain_Len: Road_Plain_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CPlain: Road_CPlainT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CPlain_Len: Road_CPlain_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Roll: Road_RollT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Roll_Len: Road_Roll_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CRoll: Road_CRollT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CRoll_Len: Road_CRoll_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Hilly: Road_HillyT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Hilly_Len: Road_Hilly_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CHilly: Road_CHillyT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CHilly_Len: Road_CHilly_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Steep: Road_SteepT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Steep_Len: Road_Steep_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CSteep: Road_CSteepT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CSteep_Len: Road_CSteep_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Total: Road_TotalT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Total_Len: Road_Total_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CTotal: Road_CTotalT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CTotal_Len: Road_CTotal_LenT }, true);
            $("#ERR7DistrictReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#ERR7DistrictReportTable_rn').html('Sr.<br/>No.');

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

    $("#ERR7DistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
         //{ startColumnName: 'Road_Plain', numberOfColumns: 2, titleText: '<em>Plain DRRP</em>' },
         //{ startColumnName: 'Road_CPlain', numberOfColumns: 2, titleText: '<em>Plain Included in CN </em>' },
         //{ startColumnName: 'Road_Roll', numberOfColumns: 2, titleText: '<em>Rolling DRRP</em>' },
         //{ startColumnName: 'Road_CRoll', numberOfColumns: 2, titleText: '<em>Rolling Included in CN </em>' },
         //{ startColumnName: 'Road_Hilly', numberOfColumns: 2, titleText: '<em>Hilly DRRP</em>' },
         //{ startColumnName: 'Road_CHilly', numberOfColumns: 2, titleText: '<em>Hilly Included in CN </em>' },
         //{ startColumnName: 'Road_Steep', numberOfColumns: 2, titleText: '<em>Steep DRRP</em>' },
         //{ startColumnName: 'Road_CSteep', numberOfColumns: 2, titleText: '<em>Steep Included in CN </em>' },
         //{ startColumnName: 'Road_Total', numberOfColumns: 2, titleText: '<em>Total DRRP</em>' },
         //{ startColumnName: 'Road_CTotal', numberOfColumns: 2, titleText: '<em>Total Included in CN </em>' }
       {
           startColumnName: 'Road_Plain', numberOfColumns: 4,
           titleText: '<table style="width:100%;border-spacing:0px"' +
                     '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Plain</td>  </tr>' +
                     '<tr>' +
                         '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                         '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                     '</tr>' +
                     '</table>'
       },
       {
           startColumnName: 'Road_Roll', numberOfColumns: 4,
           titleText: '<table style="width:100%;border-spacing:0px"' +
                   '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Rolling</td>  </tr>' +
                   '<tr>' +
                       '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                       '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                   '</tr>' +
                   '</table>'
       },
       {
           startColumnName: 'Road_Hilly', numberOfColumns: 4,
           titleText: '<table style="width:100%;border-spacing:0px"' +
                   '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Hilly</td>  </tr>' +
                   '<tr>' +
                       '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                       '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                   '</tr>' +
                   '</table>'
       },
       {
           startColumnName: 'Road_Steep', numberOfColumns: 4,
           titleText: '<table style="width:100%;border-spacing:0px"' +
                   '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Steep</td>  </tr>' +
                   '<tr>' +
                       '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                       '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                   '</tr>' +
                   '</table>'
       },
       {
           startColumnName: 'Road_Total', numberOfColumns: 4,
           titleText: '<table style="width:100%;border-spacing:0px"' +
                   '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Total</td>  </tr>' +
                   '<tr>' +
                       '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                       '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                   '</tr>' +
                   '</table>'
       },
        ]

    });
}
/**/

/*      BLOCK REPORT LISTING       */

function ERR7BlockReportListing(stateCode, districtCode, districtName, terrainType) {

    var distname;
    if (districtName == '')
        distname = $("#DISTRICT_NAME").val();
    else
        distname = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR7DistrictReportTable").jqGrid('setSelection', districtCode);
    $("#ERR7DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR7StateReportTable").jqGrid('setGridState', 'hidden');  
    $("#ERR7FinalReportTable").jqGrid('GridUnload');
    $("#ERR7BlockReportTable").jqGrid('GridUnload');

    $("#ERR7BlockReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR7BlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length'],
        colModel: [
            { name: 'BlockName', width: 200, align: 'left',  height: 'auto', sortable: true },
            { name: 'Road_Plain', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Plain_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CPlain', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CPlain_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Roll', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Roll_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CRoll', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CRoll_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Hilly', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Hilly_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CHilly', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CHilly_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Steep', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Steep_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CSteep', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CSteep_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Total', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Total_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CTotal', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CTotal_Len', width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",  decimalPlaces: 3, defaulValue: "N.A" } }

        ],
        pager: $("#ERR7BlockReportPager"),
        postData: { 'StateCode': stateCode, 'DistrictCode': districtCode, "TerrainType": terrainType },
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'BlockName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: false,
        height: '420',
        width: '1120',
        shrinkToFit: false,
        viewrecords: true,
        caption: 'Block  DRRP based on terrian type Details for ' + distname,
        loadComplete: function () {
            //Total of Columns
            var Road_PlainT = $(this).jqGrid('getCol', 'Road_Plain', false, 'sum');
            var Road_Plain_LenT = $(this).jqGrid('getCol', 'Road_Plain_Len', false, 'sum');
            Road_Plain_LenT = parseFloat(Road_Plain_LenT).toFixed(3);
            var Road_CPlainT = $(this).jqGrid('getCol', 'Road_CPlain', false, 'sum');
            var Road_CPlain_LenT = $(this).jqGrid('getCol', 'Road_CPlain_Len', false, 'sum');
            Road_CPlain_LenT = parseFloat(Road_CPlain_LenT).toFixed(3);
            var Road_RollT = $(this).jqGrid('getCol', 'Road_Roll', false, 'sum');
            var Road_Roll_LenT = $(this).jqGrid('getCol', 'Road_Roll_Len', false, 'sum');
            Road_Roll_LenT = parseFloat(Road_Roll_LenT).toFixed(3);
            var Road_CRollT = $(this).jqGrid('getCol', 'Road_CRoll', false, 'sum');
            var Road_CRoll_LenT = $(this).jqGrid('getCol', 'Road_CRoll_Len', false, 'sum');
            Road_CRoll_LenT = parseFloat(Road_CRoll_LenT).toFixed(3);
            var Road_HillyT = $(this).jqGrid('getCol', 'Road_Hilly', false, 'sum');
            var Road_Hilly_LenT = $(this).jqGrid('getCol', 'Road_Hilly_Len', false, 'sum');
            Road_Hilly_LenT = parseFloat(Road_Hilly_LenT).toFixed(3);
            var Road_CHillyT = $(this).jqGrid('getCol', 'Road_CHilly', false, 'sum');
            var Road_CHilly_LenT = $(this).jqGrid('getCol', 'Road_CHilly_Len', false, 'sum');
            Road_CHilly_LenT = parseFloat(Road_CHilly_LenT).toFixed(3);
            var Road_SteepT = $(this).jqGrid('getCol', 'Road_Steep', false, 'sum');
            var Road_Steep_LenT = $(this).jqGrid('getCol', 'Road_Steep_Len', false, 'sum');
            Road_Steep_LenT = parseFloat(Road_Steep_LenT).toFixed(3);
            var Road_CSteepT = $(this).jqGrid('getCol', 'Road_CSteep', false, 'sum');
            var Road_CSteep_LenT = $(this).jqGrid('getCol', 'Road_CSteep_Len', false, 'sum');
            Road_CSteep_LenT = parseFloat(Road_CSteep_LenT).toFixed(3);
            var Road_TotalT = $(this).jqGrid('getCol', 'Road_Total', false, 'sum');
            var Road_Total_LenT = $(this).jqGrid('getCol', 'Road_Total_Len', false, 'sum');
            Road_Total_LenT = parseFloat(Road_Total_LenT).toFixed(3);
            var Road_CTotalT = $(this).jqGrid('getCol', 'Road_CTotal', false, 'sum');
            var Road_CTotal_LenT = $(this).jqGrid('getCol', 'Road_CTotal_Len', false, 'sum');
            Road_CTotal_LenT = parseFloat(Road_CTotal_LenT).toFixed(3);
            //

            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { Road_Plain: Road_PlainT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Plain_Len: Road_Plain_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CPlain: Road_CPlainT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CPlain_Len: Road_CPlain_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Roll: Road_RollT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Roll_Len: Road_Roll_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CRoll: Road_CRollT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CRoll_Len: Road_CRoll_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Hilly: Road_HillyT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Hilly_Len: Road_Hilly_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CHilly: Road_CHillyT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CHilly_Len: Road_CHilly_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Steep: Road_SteepT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Steep_Len: Road_Steep_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CSteep: Road_CSteepT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CSteep_Len: Road_CSteep_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Total: Road_TotalT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Total_Len: Road_Total_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CTotal: Road_CTotalT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CTotal_Len: Road_CTotal_LenT }, true);
            $("#ERR7BlockReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#ERR7BlockReportTable_rn').html('Sr.<br/>No.');

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

    $("#ERR7BlockReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
         //{ startColumnName: 'Road_Plain', numberOfColumns: 2, titleText: '<em>Plain DRRP</em>' },
         //{ startColumnName: 'Road_CPlain', numberOfColumns: 2, titleText: '<em>Plain Included in CN </em>' },
         //{ startColumnName: 'Road_Roll', numberOfColumns: 2, titleText: '<em>Rolling DRRP</em>' },
         //{ startColumnName: 'Road_CRoll', numberOfColumns: 2, titleText: '<em>Rolling Included in CN </em>' },
         //{ startColumnName: 'Road_Hilly', numberOfColumns: 2, titleText: '<em>Hilly DRRP</em>' },
         //{ startColumnName: 'Road_CHilly', numberOfColumns: 2, titleText: '<em>Hilly Included in CN </em>' },
         //{ startColumnName: 'Road_Steep', numberOfColumns: 2, titleText: '<em>Steep DRRP</em>' },
         //{ startColumnName: 'Road_CSteep', numberOfColumns: 2, titleText: '<em>Steep Included in CN </em>' },
         //{ startColumnName: 'Road_Total', numberOfColumns: 2, titleText: '<em>Total DRRP</em>' },
         //{ startColumnName: 'Road_CTotal', numberOfColumns: 2, titleText: '<em>Total Included in CN </em>' }
       {
           startColumnName: 'Road_Plain', numberOfColumns: 4,
           titleText: '<table style="width:100%;border-spacing:0px"' +
                     '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Plain</td>  </tr>' +
                     '<tr>' +
                         '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                         '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                     '</tr>' +
                     '</table>'
       },
       {
           startColumnName: 'Road_Roll', numberOfColumns: 4,
           titleText: '<table style="width:100%;border-spacing:0px"' +
                   '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Rolling</td>  </tr>' +
                   '<tr>' +
                       '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                       '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                   '</tr>' +
                   '</table>'
       },
       {
           startColumnName: 'Road_Hilly', numberOfColumns: 4,
           titleText: '<table style="width:100%;border-spacing:0px"' +
                   '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Hilly</td>  </tr>' +
                   '<tr>' +
                       '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                       '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                   '</tr>' +
                   '</table>'
       },
       {
           startColumnName: 'Road_Steep', numberOfColumns: 4,
           titleText: '<table style="width:100%;border-spacing:0px"' +
                   '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Steep</td>  </tr>' +
                   '<tr>' +
                       '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                       '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                   '</tr>' +
                   '</table>'
       },
       {
           startColumnName: 'Road_Total', numberOfColumns: 4,
           titleText: '<table style="width:100%;border-spacing:0px"' +
                   '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Total</td>  </tr>' +
                   '<tr>' +
                       '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                       '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                   '</tr>' +
                   '</table>'
       },
      ]

    });
}

/*       FINAL BLOCK REPORT LISTING       */

function ERR7FinalReportListing(blockCode, districtCode, stateCode, blockName, terrainType) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR7BlockReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR7DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR7StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR7BlockReportTable").jqGrid('setSelection', blockCode);
    $("#ERR7FinalReportTable").jqGrid('GridUnload');

    $("#ERR7FinalReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR7FinalReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Road Code', 'Road Name', 'Road Category', 'Road Type', 'Length (Kms.)', 'Year of Construction', 'Included in Core Network (Y/N)', 'Habitations Status (Y/N)', 'Habitation Name', 'Population', 'Soil type', 'Terrain Type'],
        colModel: [
           { name: 'PlannedRoadNumber', width: 150, align: 'left',  height: 'auto', sortable: true },
           { name: 'PlannedRoadName', width: 250, align: 'left',  height: 'auto', sortable: false },
           { name: 'MAST_ROAD_CAT_CODE', width: 120, align: 'left',  height: 'auto', sortable: false },
           { name: 'MAST_ER_ROAD_TYPE', width: 150, align: 'left',  height: 'auto', sortable: false },
           { name: 'ROAD_LENGTH', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
           { name: 'MAST_CONS_YEAR', width: 120, align: 'center',  height: 'auto', sortable: false },
           { name: 'MAST_CORE_NETWORK', width: 120, align: 'center',  height: 'auto', sortable: false },
           { name: 'MAST_HAB_STATUS', width: 120, align: 'center',  height: 'auto', sortable: false },
           { name: 'MAST_HAB_NAME', width: 120, align: 'left',  height: 'auto', sortable: false },
           { name: 'MAST_HAB_TOT_POP', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
           { name: 'MAST_SOIL_TYPE_NAME', width: 200, align: 'left',  height: 'auto', sortable: false },
           { name: 'MAST_TERRAIN_TYPE_NAME', width: 200, align: 'left',  height: 'auto', sortable: false }

        ],

        postData: { "BlockCode": blockCode, "StateCode": stateCode, "DistrictCode": districtCode, "TerrainType": terrainType },
        rowNum: '2147483647',
        pager: $("#ERR7FinalReportPager"),
        footerrow: true,
        pgbuttons: true,
        sortname: 'PlannedRoadNumber',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '420',
        viewrecords: true,
        caption: 'DRRP based on terrian type Details for ' + blockName,
        loadComplete: function () {
            //Total of Columns
           
            var ROAD_LENGTHT = $(this).jqGrid('getCol', 'ROAD_LENGTH', false, 'sum');
            ROAD_LENGTHT = parseFloat(ROAD_LENGTHT).toFixed(3);
            var NoHablitisationT = $(this).jqGrid('getCol', 'MAST_HAB_TOT_POP', false, 'sum');
            

            $(this).jqGrid('footerData', 'set', { PlannedRoadNumber: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { ROAD_LENGTH: ROAD_LENGTHT });
            $(this).jqGrid('footerData', 'set', { MAST_HAB_TOT_POP: NoHablitisationT });
            $('#ERR7FinalReportTable_rn').html('Sr.<br/>No.');

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