$(document).ready(function () {

    $("#btCNR3Details").click(function () {

        var route = $("#Route_CNR3Details").val();
        var roadCategory = $("#RoadCategory_CNR3Details").val();

        if ($("#hdnLevelId").val() == 6) //mord
        {
         
            CNR3StateReportListing(route, roadCategory);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
           
            CNR3DistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), route, roadCategory);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
         
            CNR3BlockReportListing($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), route, roadCategory);
        }
    });

    $("#btCNR3Details").trigger('click');

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

});


/*       STATE REPORT LISTING       */
function CNR3StateReportListing(route, roadCategory) {
    $("#CNR3StateReportTable").jqGrid('GridUnload');
    $("#CNR3DistrictReportTable").jqGrid('GridUnload');
    $("#CNR3BlockReportTable").jqGrid('GridUnload');
    $("#CNR3FinalReportTable").jqGrid('GridUnload');

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNR3StateReportTable").jqGrid({
        url: '/CoreNetworkReports/CNR3StateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Habitations', 'Length', 'Habitations', 'Length ', 'Habitations', 'Length ', 'Habitations', 'Length ', 'Habitations', 'Length', 'Habitations', 'Length','Habitations', ' Length'],
        colModel: [
            { name: 'StateName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'TRCNHabNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRCNHabLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRPHabNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRPHabLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRHabCNNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRHabCNLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRPHabNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRPHabLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TCNHabNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TCNHabLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TPHabNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TPHabLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TBALHabi', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TBALLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            
        ],
        postData: { "Route": route, "RoadCategory": roadCategory },
        pager: $("#CNR3StateReportPager"),
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
            var TRCNHabNumberT = $(this).jqGrid('getCol', 'TRCNHabNumber', false, 'sum');
            var TRCNHabLengthT = $(this).jqGrid('getCol', 'TRCNHabLength', false, 'sum');
            TRCNHabLengthT = parseFloat(TRCNHabLengthT).toFixed(3);
            var TRPHabNumberT = $(this).jqGrid('getCol', 'TRPHabNumber', false, 'sum');           
            var TRPHabLengthT = $(this).jqGrid('getCol', 'TRPHabLength', false, 'sum');
            TRPHabLengthT = parseFloat(TRPHabLengthT).toFixed(3);
            var LRHabCNNumberT = $(this).jqGrid('getCol', 'LRHabCNNumber', false, 'sum');
            var LRHabCNLengthT = $(this).jqGrid('getCol', 'LRHabCNLength', false, 'sum');
            LRHabCNLengthT = parseFloat(LRHabCNLengthT).toFixed(3);
            var LRPHabNumberT = $(this).jqGrid('getCol', 'LRPHabNumber', false, 'sum');
            var LRPHabLengthT = $(this).jqGrid('getCol', 'LRPHabLength', false, 'sum');
            LRPHabLengthT = parseFloat(LRPHabLengthT).toFixed(3);
            var TCNHabNumberT = $(this).jqGrid('getCol', 'TCNHabNumber', false, 'sum');
            var TCNHabLengthT = $(this).jqGrid('getCol', 'TCNHabLength', false, 'sum');
            TCNHabLengthT = parseFloat(TCNHabLengthT).toFixed(3);
            var TPHabNumberT = $(this).jqGrid('getCol', 'TPHabNumber', false, 'sum');
            var TPHabLengthT = $(this).jqGrid('getCol', 'TPHabLength', false, 'sum');
            TPHabLengthT = parseFloat(TPHabLengthT).toFixed(3);
            var TBALHabiT = $(this).jqGrid('getCol', 'TBALHabi', false, 'sum');
            var TBALLengthT = $(this).jqGrid('getCol', 'TBALLength', false, 'sum');
            TBALLengthT = parseFloat(TBALLengthT).toFixed(3);


            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TRCNHabNumber: TRCNHabNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TRCNHabLength: TRCNHabLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TRPHabNumber: TRPHabNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TRPHabLength: TRPHabLengthT }, true);
            $(this).jqGrid('footerData', 'set', { LRHabCNNumber: LRHabCNNumberT }, true);
            $(this).jqGrid('footerData', 'set', { LRHabCNLength: LRHabCNLengthT }, true);
            $(this).jqGrid('footerData', 'set', { LRPHabNumber: LRPHabNumberT }, true);
            $(this).jqGrid('footerData', 'set', { LRPHabLength: LRPHabLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TCNHabNumber: TCNHabNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TCNHabLength: TCNHabLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TPHabNumber: TPHabNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TPHabLength: TPHabLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TBALHabi: TBALHabiT }, true);
            $(this).jqGrid('footerData', 'set', { TBALLength: TBALLengthT }, true);
            $("#CNR3StateReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#CNR3StateReportTable_rn').html('Sr.<br/>No.');

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

    $("#CNR3StateReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
        // { startColumnName: 'TRCNHabNumber', numberOfColumns: 2, titleText: '<em>Core Network </em>' },
        // { startColumnName: 'TRPHabNumber', numberOfColumns: 2, titleText: '<em>Proposals </em>' },
        // { startColumnName: 'LRHabCNNumber', numberOfColumns: 2, titleText: '<em>Core Network </em>' },
        // { startColumnName: 'LRPHabNumber', numberOfColumns: 2, titleText: '<em>Proposals </em>' },
        // { startColumnName: 'TCNHabNumber', numberOfColumns: 2, titleText: '<em>Core Network </em>' },
        // { startColumnName: 'TPHabNumber', numberOfColumns: 2, titleText: '<em>Proposals </em>' },
        //{ startColumnName: 'TBALHabi', numberOfColumns: 2, titleText: '<em>Balance </em>' }
          {
              startColumnName: 'TRCNHabNumber', numberOfColumns: 4,
              titleText: '<table style="width:100%;border-spacing:0px"' +
                      '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Through Route </td>  </tr>' +
                      '<tr>' +
                          '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Core Network</td>' +

                          '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Proposals</td>' +
                      '</tr>' +
                      '</table>'
          },

           {
               startColumnName: 'LRHabCNNumber', numberOfColumns: 4,
               titleText: '<table style="width:100%;border-spacing:0px"' +
                       '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Link Route </td>  </tr>' +
                       '<tr>' +
                           '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Core Network</td>' +

                           '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Proposals</td>' +
                       '</tr>' +
                       '</table>'
           },
           {
               startColumnName: 'TCNHabNumber', numberOfColumns: 6,
               titleText: '<table style="width:100%;border-spacing:0px"' +
                       '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="6">Total </td>  </tr>' +
                       '<tr>' +
                           '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Core Network</td>' +
                            '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Proposals</td>' +
                           '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Balance</td>' +
                       '</tr>' +
                       '</table>'
           },
      ]
    });
}
/**/

/*       DISTRICT REPORT LISTING       */
function CNR3DistrictReportListing(stateCode, stateName, route, roadCategory) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNR3StateReportTable").jqGrid('setSelection', stateCode);
    $("#CNR3StateReportTable").jqGrid('setGridState', 'hidden');
    $("#CNR3DistrictReportTable").jqGrid('GridUnload');
    $("#CNR3BlockReportTable").jqGrid('GridUnload');
    $("#CNR3FinalReportTable").jqGrid('GridUnload');

    $("#CNR3DistrictReportTable").jqGrid({
        url: '/CoreNetworkReports/CNR3DistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Habitations', 'Length', 'Habitations', 'Length ', 'Habitations', 'Length ', 'Habitations', 'Length ', 'Habitations', 'Length', 'Habitations', 'Length', 'Habitations', ' Length'],
        colModel: [
            { name: 'DistrictName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'TRCNHabNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRCNHabLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRPHabNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRPHabLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRHabCNNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRHabCNLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRPHabNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRPHabLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TCNHabNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TCNHabLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TPHabNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TPHabLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TBALHabi', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TBALLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },


        ],
        pager: $("#CNR3DistrictReportPager"),
        postData: { 'StateCode': stateCode, "Route": route, "RoadCategory": roadCategory },
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'DistrictName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '480',
        viewrecords: true,
        caption: 'District Core Network Details for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var TRCNHabNumberT = $(this).jqGrid('getCol', 'TRCNHabNumber', false, 'sum');
            var TRCNHabLengthT = $(this).jqGrid('getCol', 'TRCNHabLength', false, 'sum');
            TRCNHabLengthT = parseFloat(TRCNHabLengthT).toFixed(3);
            var TRPHabNumberT = $(this).jqGrid('getCol', 'TRPHabNumber', false, 'sum');
            var TRPHabLengthT = $(this).jqGrid('getCol', 'TRPHabLength', false, 'sum');
            TRPHabLengthT = parseFloat(TRPHabLengthT).toFixed(3);
            var LRHabCNNumberT = $(this).jqGrid('getCol', 'LRHabCNNumber', false, 'sum');
            var LRHabCNLengthT = $(this).jqGrid('getCol', 'LRHabCNLength', false, 'sum');
            LRHabCNLengthT = parseFloat(LRHabCNLengthT).toFixed(3);
            var LRPHabNumberT = $(this).jqGrid('getCol', 'LRPHabNumber', false, 'sum');
            var LRPHabLengthT = $(this).jqGrid('getCol', 'LRPHabLength', false, 'sum');
            LRPHabLengthT = parseFloat(LRPHabLengthT).toFixed(3);
            var TCNHabNumberT = $(this).jqGrid('getCol', 'TCNHabNumber', false, 'sum');
            var TCNHabLengthT = $(this).jqGrid('getCol', 'TCNHabLength', false, 'sum');
            TCNHabLengthT = parseFloat(TCNHabLengthT).toFixed(3);
            var TPHabNumberT = $(this).jqGrid('getCol', 'TPHabNumber', false, 'sum');
            var TPHabLengthT = $(this).jqGrid('getCol', 'TPHabLength', false, 'sum');
            TPHabLengthT = parseFloat(TPHabLengthT).toFixed(3);
            var TBALHabiT = $(this).jqGrid('getCol', 'TBALHabi', false, 'sum');
            var TBALLengthT = $(this).jqGrid('getCol', 'TBALLength', false, 'sum');
            TBALLengthT = parseFloat(TBALLengthT).toFixed(3);

            //
            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TRCNHabNumber: TRCNHabNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TRCNHabLength: TRCNHabLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TRPHabNumber: TRPHabNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TRPHabLength: TRPHabLengthT }, true);
            $(this).jqGrid('footerData', 'set', { LRHabCNNumber: LRHabCNNumberT }, true);
            $(this).jqGrid('footerData', 'set', { LRHabCNLength: LRHabCNLengthT }, true);
            $(this).jqGrid('footerData', 'set', { LRPHabNumber: LRPHabNumberT }, true);
            $(this).jqGrid('footerData', 'set', { LRPHabLength: LRPHabLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TCNHabNumber: TCNHabNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TCNHabLength: TCNHabLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TPHabNumber: TPHabNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TPHabLength: TPHabLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TBALHabi: TBALHabiT }, true);
            $(this).jqGrid('footerData', 'set', { TBALLength: TBALLengthT }, true);
            $("#CNR3DistrictReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#CNR3DistrictReportTable_rn').html('Sr.<br/>No.');

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

    $("#CNR3DistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
         // { startColumnName: 'TRCNHabNumber', numberOfColumns: 2, titleText: '<em>Core Network </em>' },
         // { startColumnName: 'TRPHabNumber', numberOfColumns: 2, titleText: '<em>Proposals </em>' },
         // { startColumnName: 'LRHabCNNumber', numberOfColumns: 2, titleText: '<em>Core Network </em>' },
         // { startColumnName: 'LRPHabNumber', numberOfColumns: 2, titleText: '<em>Proposals </em>' },
         // { startColumnName: 'TCNHabNumber', numberOfColumns: 2, titleText: '<em>Core Network </em>' },
         // { startColumnName: 'TPHabNumber', numberOfColumns: 2, titleText: '<em>Proposals </em>' },
         //{ startColumnName: 'TBALHabi', numberOfColumns: 2, titleText: '<em>Balance </em>' }
           {
               startColumnName: 'TRCNHabNumber', numberOfColumns: 4,
               titleText: '<table style="width:100%;border-spacing:0px"' +
                       '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Through Route </td>  </tr>' +
                       '<tr>' +
                           '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Core Network</td>' +

                           '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Proposals</td>' +
                       '</tr>' +
                       '</table>'
           },

            {
                startColumnName: 'LRHabCNNumber', numberOfColumns: 4,
                titleText: '<table style="width:100%;border-spacing:0px"' +
                        '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Link Route </td>  </tr>' +
                        '<tr>' +
                            '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Core Network</td>' +

                            '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Proposals</td>' +
                        '</tr>' +
                        '</table>'
            },
            {
                startColumnName: 'TCNHabNumber', numberOfColumns: 6,
                titleText: '<table style="width:100%;border-spacing:0px"' +
                        '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="6">Total </td>  </tr>' +
                        '<tr>' +
                            '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Core Network</td>' +
                             '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Proposals</td>' +
                            '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Balance</td>' +
                        '</tr>' +
                        '</table>'
            },
        ]
    });
}
/**/

/*      BLOCK REPORT LISTING       */

function CNR3BlockReportListing(stateCode, districtCode, districtName, route, roadCategory) {

    var distname;
    if (districtName == '')
        distname = $("#DISTRICT_NAME").val();
    else
        distname = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNR3DistrictReportTable").jqGrid('setSelection', districtCode);
    $("#CNR3DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#CNR3StateReportTable").jqGrid('setGridState', 'hidden');
    $("#CNR3BlockReportTable").jqGrid('GridUnload');
    $("#CNR3FinalReportTable").jqGrid('GridUnload');

    $("#CNR3BlockReportTable").jqGrid({
        url: '/CoreNetworkReports/CNR3BlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Habitations', 'Length', 'Habitations', 'Length ', 'Habitations', 'Length ', 'Habitations', 'Length ', 'Habitations', 'Length', 'Habitations', 'Length', 'Habitations', ' Length'],
        colModel: [
            { name: 'BlockName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'TRCNHabNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRCNHabLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRPHabNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRPHabLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRHabCNNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRHabCNLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRPHabNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'LRPHabLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TCNHabNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TCNHabLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TPHabNumber', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TPHabLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TBALHabi', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TBALLength', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },


        ],
        pager: $("#CNR3BlockReportPager"),
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
            var TRCNHabNumberT = $(this).jqGrid('getCol', 'TRCNHabNumber', false, 'sum');
            var TRCNHabLengthT = $(this).jqGrid('getCol', 'TRCNHabLength', false, 'sum');
            TRCNHabLengthT = parseFloat(TRCNHabLengthT).toFixed(3);
            var TRPHabNumberT = $(this).jqGrid('getCol', 'TRPHabNumber', false, 'sum');
            var TRPHabLengthT = $(this).jqGrid('getCol', 'TRPHabLength', false, 'sum');
            TRPHabLengthT = parseFloat(TRPHabLengthT).toFixed(3);
            var LRHabCNNumberT = $(this).jqGrid('getCol', 'LRHabCNNumber', false, 'sum');
            var LRHabCNLengthT = $(this).jqGrid('getCol', 'LRHabCNLength', false, 'sum');
            LRHabCNLengthT = parseFloat(LRHabCNLengthT).toFixed(3);
            var LRPHabNumberT = $(this).jqGrid('getCol', 'LRPHabNumber', false, 'sum');
            var LRPHabLengthT = $(this).jqGrid('getCol', 'LRPHabLength', false, 'sum');
            LRPHabLengthT = parseFloat(LRPHabLengthT).toFixed(3);
            var TCNHabNumberT = $(this).jqGrid('getCol', 'TCNHabNumber', false, 'sum');
            var TCNHabLengthT = $(this).jqGrid('getCol', 'TCNHabLength', false, 'sum');
            TCNHabLengthT = parseFloat(TCNHabLengthT).toFixed(3);
            var TPHabNumberT = $(this).jqGrid('getCol', 'TPHabNumber', false, 'sum');
            var TPHabLengthT = $(this).jqGrid('getCol', 'TPHabLength', false, 'sum');
            TPHabLengthT = parseFloat(TPHabLengthT).toFixed(3);
            var TBALHabiT = $(this).jqGrid('getCol', 'TBALHabi', false, 'sum');
            var TBALLengthT = $(this).jqGrid('getCol', 'TBALLength', false, 'sum');
            TBALLengthT = parseFloat(TBALLengthT).toFixed(3);

            //
            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TRCNHabNumber: TRCNHabNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TRCNHabLength: TRCNHabLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TRPHabNumber: TRPHabNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TRPHabLength: TRPHabLengthT }, true);
            $(this).jqGrid('footerData', 'set', { LRHabCNNumber: LRHabCNNumberT }, true);
            $(this).jqGrid('footerData', 'set', { LRHabCNLength: LRHabCNLengthT }, true);
            $(this).jqGrid('footerData', 'set', { LRPHabNumber: LRPHabNumberT }, true);
            $(this).jqGrid('footerData', 'set', { LRPHabLength: LRPHabLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TCNHabNumber: TCNHabNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TCNHabLength: TCNHabLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TPHabNumber: TPHabNumberT }, true);
            $(this).jqGrid('footerData', 'set', { TPHabLength: TPHabLengthT }, true);
            $(this).jqGrid('footerData', 'set', { TBALHabi: TBALHabiT }, true);
            $(this).jqGrid('footerData', 'set', { TBALLength: TBALLengthT }, true);
            $("#CNR3BlockReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#CNR3BlockReportTable_rn').html('Sr.<br/>No.');

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

    $("#CNR3BlockReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
        // { startColumnName: 'TRCNHabNumber', numberOfColumns: 2, titleText: '<em>Core Network </em>' },
        // { startColumnName: 'TRPHabNumber', numberOfColumns: 2, titleText: '<em>Proposals </em>' },
        // { startColumnName: 'LRHabCNNumber', numberOfColumns: 2, titleText: '<em>Core Network </em>' },
        // { startColumnName: 'LRPHabNumber', numberOfColumns: 2, titleText: '<em>Proposals </em>' },
        // { startColumnName: 'TCNHabNumber', numberOfColumns: 2, titleText: '<em>Core Network </em>' },
        // { startColumnName: 'TPHabNumber', numberOfColumns: 2, titleText: '<em>Proposals </em>' },
        //{ startColumnName: 'TBALHabi', numberOfColumns: 2, titleText: '<em>Balance </em>' }
          {
              startColumnName: 'TRCNHabNumber', numberOfColumns: 4,
              titleText: '<table style="width:100%;border-spacing:0px"' +
                      '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Through Route </td>  </tr>' +
                      '<tr>' +
                          '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Core Network</td>' +

                          '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Proposals</td>' +
                      '</tr>' +
                      '</table>'
          },

           {
               startColumnName: 'LRHabCNNumber', numberOfColumns: 4,
               titleText: '<table style="width:100%;border-spacing:0px"' +
                       '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Link Route </td>  </tr>' +
                       '<tr>' +
                           '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Core Network</td>' +

                           '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Proposals</td>' +
                       '</tr>' +
                       '</table>'
           },
           {
               startColumnName: 'TCNHabNumber', numberOfColumns: 6,
               titleText: '<table style="width:100%;border-spacing:0px"' +
                       '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="6">Total </td>  </tr>' +
                       '<tr>' +
                           '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Core Network</td>' +
                            '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Proposals</td>' +
                           '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Balance</td>' +
                       '</tr>' +
                       '</table>'
           },
        ]
    });
}

/*       FINAL BLOCK REPORT LISTING       */

function CNR3FinalReportListing(blockCode, districtCode, stateCode, blockName, route, roadCategory) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CNR3BlockReportTable").jqGrid('setGridState', 'hidden');
    $("#CNR3DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#CNR3StateReportTable").jqGrid('setGridState', 'hidden');
    $("#CNR3BlockReportTable").jqGrid('setSelection', blockCode);
    $("#CNR3FinalReportTable").jqGrid('GridUnload');

    $("#CNR3FinalReportTable").jqGrid({
        url: '/CoreNetworkReports/CNR3FinalReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Road Code', 'Road Name', 'DRRP Road Code', 'DRRP Road Name', 'Road Length', 'Road From', 'Road To', 'Route Type', 'Length Covered (P/F)', 'Balance Length', 'No of Habitations', 'Habitations benifited','Balance Habitations'],
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
            { name: 'NoHablitisation', width: 300, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'HabBenefited', width: 300, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'BalHabitisation', width: 300, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }
   
        ],

        postData: { "BlockCode": blockCode, "StateCode": stateCode, "DistrictCode": districtCode, "Route": route, "RoadCategory": roadCategory },
        rowNum: '2147483647',
        pager: $("#CNR3FinalReportPager"),
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
            var HabBenefitedT = $(this).jqGrid('getCol', 'HabBenefited', false, 'sum');
            var BalHabitisationT = $(this).jqGrid('getCol', 'BalHabitisation', false, 'sum');

            $(this).jqGrid('footerData', 'set', { PlannedRoadNumber: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { RoadLength: RoadLengthT });
            $(this).jqGrid('footerData', 'set', { LengthCovered: LengthCoveredT });
            $(this).jqGrid('footerData', 'set', { BalLength: BalLengthT });
            $(this).jqGrid('footerData', 'set', { NoHablitisation: NoHablitisationT });
            $(this).jqGrid('footerData', 'set', { HabBenefited: HabBenefitedT });
            $(this).jqGrid('footerData', 'set', { BalHabitisation: BalHabitisationT });
            $("#CNR3FinalReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#CNR3FinalReportTable_rn').html('Sr.<br/>No.');
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