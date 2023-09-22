$(function () {
    
    $("#btCN2Details").click(function () {

        var population = $("#Population_CN2Details").val();

        if ($("#hdnLevelId").val() == 6) //mord
        {
           
            CN2StateReportListing(population);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
          
            CN2DistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), population);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
          
            CN2BlockReportListing($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), population);
        }
    });

    $("#btCN2Details").trigger("click");
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});


/*       STATE REPORT LISTING       */
function CN2StateReportListing(population) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CN2StateReportTable").jqGrid('GridUnload');
    $("#CN2DistrictReportTable").jqGrid('GridUnload');
    $("#CN2BlockReportTable").jqGrid('GridUnload');
    $("#CN2FinalReportTable").jqGrid('GridUnload');
    $("#CN2StateReportTable").jqGrid({
        url: '/CoreNetworkReports/CN2StateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', '1000+', '500-999', '250-499', '<250( Not eligible under PMGSY )', '1000+', '500-999', '250-499', '<250( Not eligible under PMGSY )', '1000+', '500-999', '250-499', '<250( Not eligible under PMGSY )', ],
        colModel: [
            { name: 'StateName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'TotalPopulationover1000', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TotalPopulationover999', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TotalPopulationover499', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TotalPopulationover250', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ConnectedPopulationover1000', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ConnectedPopulationover999', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ConnectedPopulationover499', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ConnectedPopulationover250', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'UnconnectedPopulationover1000', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'UnconnectedPopulationover999', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'UnconnectedPopulationover499', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'UnconnectedPopulationover250', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
              ],
        postData: {Population: population },
        pager: $("#CN2StateReportPager"),
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'StateName',
        sortorder: 'asc',
        rownumbers: true,
        height: '520',
        viewrecords: true,
        caption: 'State Core Network Details',
        //cmTemplate: { resizable: false },
        //shrinkToFit: false,
        autowidth: true,
        loadComplete: function () {
            //Total of Columns
            var TotalPopulationover1000T = $(this).jqGrid('getCol', 'TotalPopulationover1000', false, 'sum');
            var TotalPopulationover999T = $(this).jqGrid('getCol', 'TotalPopulationover999', false, 'sum');
            var TotalPopulationover499T = $(this).jqGrid('getCol', 'TotalPopulationover499', false, 'sum');
            var TotalPopulationover250T = $(this).jqGrid('getCol', 'TotalPopulationover250', false, 'sum');
            var UnconnectedPopulationover1000T = $(this).jqGrid('getCol', 'UnconnectedPopulationover1000', false, 'sum');
            var UnconnectedPopulationover999T = $(this).jqGrid('getCol', 'UnconnectedPopulationover999', false, 'sum');
            var UnconnectedPopulationover499T = $(this).jqGrid('getCol', 'UnconnectedPopulationover499', false, 'sum');
            var UnconnectedPopulationover250T = $(this).jqGrid('getCol', 'UnconnectedPopulationover250', false, 'sum');
            var ConnectedPopulationover1000T = $(this).jqGrid('getCol', 'ConnectedPopulationover1000', false, 'sum');
            var ConnectedPopulationover999T = $(this).jqGrid('getCol', 'ConnectedPopulationover999', false, 'sum');
            var ConnectedPopulationover499T = $(this).jqGrid('getCol', 'ConnectedPopulationover499', false, 'sum');
            var ConnectedPopulationover250T = $(this).jqGrid('getCol', 'ConnectedPopulationover250', false, 'sum');
            //

            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TotalPopulationover1000: TotalPopulationover1000T }, true);
            $(this).jqGrid('footerData', 'set', { TotalPopulationover999: TotalPopulationover999T }, true);
            $(this).jqGrid('footerData', 'set', { TotalPopulationover499: TotalPopulationover499T }, true);
            $(this).jqGrid('footerData', 'set', { TotalPopulationover250: TotalPopulationover250T }, true);
            $(this).jqGrid('footerData', 'set', { UnconnectedPopulationover1000: UnconnectedPopulationover1000T }, true);
            $(this).jqGrid('footerData', 'set', { UnconnectedPopulationover999: UnconnectedPopulationover999T }, true);
            $(this).jqGrid('footerData', 'set', { UnconnectedPopulationover499: UnconnectedPopulationover499T }, true);
            $(this).jqGrid('footerData', 'set', { UnconnectedPopulationover250: UnconnectedPopulationover250T }, true);
            $(this).jqGrid('footerData', 'set', { ConnectedPopulationover1000: ConnectedPopulationover1000T }, true);
            $(this).jqGrid('footerData', 'set', { ConnectedPopulationover999: ConnectedPopulationover999T }, true);
            $(this).jqGrid('footerData', 'set', { ConnectedPopulationover499: ConnectedPopulationover499T }, true);
            $(this).jqGrid('footerData', 'set', { ConnectedPopulationover250: ConnectedPopulationover250T }, true);
            $('#CN2StateReportTable_rn').html('Sr.<br/>No.');

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

    $("#CN2StateReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'TotalPopulationover1000', numberOfColumns: 4, titleText: '<em>Total  </em>' },
          { startColumnName: 'ConnectedPopulationover1000', numberOfColumns: 4, titleText: '<em>Connected</em>' },
          { startColumnName: 'UnconnectedPopulationover1000', numberOfColumns: 4, titleText: '<em>Unconnected</em>' }
        ]
    });
}
/**/

/*       DISTRICT REPORT LISTING       */
function CN2DistrictReportListing(stateCode, stateName, population) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CN2StateReportTable").jqGrid('setSelection', stateCode);
    $("#CN2StateReportTable").jqGrid('setGridState', 'hidden');
    $("#CN2DistrictReportTable").jqGrid('GridUnload');
    $("#CN2BlockReportTable").jqGrid('GridUnload');
    $("#CN2FinalReportTable").jqGrid('GridUnload');

    $("#CN2DistrictReportTable").jqGrid({
        url: '/CoreNetworkReports/CN2DistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', '1000+', '500-999', '250-499', '<250( Not eligible under PMGSY )', '1000+', '500-999', '250-499', '<250( Not eligible under PMGSY )', '1000+', '500-999', '250-499', '<250( Not eligible under PMGSY )'],
        colModel: [
            { name: 'DistrictName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'TotalPopulationover1000', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TotalPopulationover999', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TotalPopulationover499', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TotalPopulationover250', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ConnectedPopulationover1000', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ConnectedPopulationover999', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ConnectedPopulationover499', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ConnectedPopulationover250', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'UnconnectedPopulationover1000', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'UnconnectedPopulationover999', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'UnconnectedPopulationover499', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'UnconnectedPopulationover250', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
           ],
        pager: $("#CN2DistrictReportPager"),
        postData: { 'StateCode': stateCode, Population: population },
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
            var TotalPopulationover1000T = $(this).jqGrid('getCol', 'TotalPopulationover1000', false, 'sum');
            var TotalPopulationover999T = $(this).jqGrid('getCol', 'TotalPopulationover999', false, 'sum');
            var TotalPopulationover499T = $(this).jqGrid('getCol', 'TotalPopulationover499', false, 'sum');
            var TotalPopulationover250T = $(this).jqGrid('getCol', 'TotalPopulationover250', false, 'sum');
            var UnconnectedPopulationover1000T = $(this).jqGrid('getCol', 'UnconnectedPopulationover1000', false, 'sum');
            var UnconnectedPopulationover999T = $(this).jqGrid('getCol', 'UnconnectedPopulationover999', false, 'sum');
            var UnconnectedPopulationover499T = $(this).jqGrid('getCol', 'UnconnectedPopulationover499', false, 'sum');
            var UnconnectedPopulationover250T = $(this).jqGrid('getCol', 'UnconnectedPopulationover250', false, 'sum');
            var ConnectedPopulationover1000T = $(this).jqGrid('getCol', 'ConnectedPopulationover1000', false, 'sum');
            var ConnectedPopulationover999T = $(this).jqGrid('getCol', 'ConnectedPopulationover999', false, 'sum');
            var ConnectedPopulationover499T = $(this).jqGrid('getCol', 'ConnectedPopulationover499', false, 'sum');
            var ConnectedPopulationover250T = $(this).jqGrid('getCol', 'ConnectedPopulationover250', false, 'sum');
            //

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TotalPopulationover1000: TotalPopulationover1000T }, true);
            $(this).jqGrid('footerData', 'set', { TotalPopulationover999: TotalPopulationover999T }, true);
            $(this).jqGrid('footerData', 'set', { TotalPopulationover499: TotalPopulationover499T }, true);
            $(this).jqGrid('footerData', 'set', { TotalPopulationover250: TotalPopulationover250T }, true);
            $(this).jqGrid('footerData', 'set', { UnconnectedPopulationover1000: UnconnectedPopulationover1000T }, true);
            $(this).jqGrid('footerData', 'set', { UnconnectedPopulationover999: UnconnectedPopulationover999T }, true);
            $(this).jqGrid('footerData', 'set', { UnconnectedPopulationover499: UnconnectedPopulationover499T }, true);
            $(this).jqGrid('footerData', 'set', { UnconnectedPopulationover250: UnconnectedPopulationover250T }, true);
            $(this).jqGrid('footerData', 'set', { ConnectedPopulationover1000: ConnectedPopulationover1000T }, true);
            $(this).jqGrid('footerData', 'set', { ConnectedPopulationover999: ConnectedPopulationover999T }, true);
            $(this).jqGrid('footerData', 'set', { ConnectedPopulationover499: ConnectedPopulationover499T }, true);
            $(this).jqGrid('footerData', 'set', { ConnectedPopulationover250: ConnectedPopulationover250T }, true);
            $('#CN2DistrictReportTable_rn').html('Sr.<br/>No.');
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

    $("#CN2DistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'TotalPopulationover1000', numberOfColumns: 4, titleText: '<em>Total  </em>' },
          { startColumnName: 'ConnectedPopulationover1000', numberOfColumns: 4, titleText: '<em>Connected</em>' },
          { startColumnName: 'UnconnectedPopulationover1000', numberOfColumns: 4, titleText: '<em>Unconnected</em>' }
         
        ]
    });
}
/**/

/*      BLOCK REPORT LISTING       */

function CN2BlockReportListing(stateCode, districtCode, districtName, population) {
    var distname;
    if (districtName == '')
        distname = $("#DISTRICT_NAME").val();
    else
        distname = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CN2DistrictReportTable").jqGrid('setSelection', districtCode);
    $("#CN2DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#CN2StateReportTable").jqGrid('setGridState', 'hidden');
    $("#CN2BlockReportTable").jqGrid('GridUnload');
    $("#CN2FinalReportTable").jqGrid('GridUnload');

    $("#CN2BlockReportTable").jqGrid({
        url: '/CoreNetworkReports/CN2BlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', '1000+', '500-999', '250-499', '<250( Not eligible under PMGSY )', '1000+', '500-999', '250-499', '<250( Not eligible under PMGSY )', '1000+', '500-999', '250-499', '<250( Not eligible under PMGSY )'],
        colModel: [
            { name: 'BlockName', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'TotalPopulationover1000', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TotalPopulationover999', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TotalPopulationover499', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TotalPopulationover250', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ConnectedPopulationover1000', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ConnectedPopulationover999', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ConnectedPopulationover499', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'ConnectedPopulationover250', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'UnconnectedPopulationover1000', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'UnconnectedPopulationover999', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'UnconnectedPopulationover499', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'UnconnectedPopulationover250', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
               ],
        pager: $("#CN2BlockReportPager"),
        postData: { 'StateCode': stateCode, 'DistrictCode': districtCode, Population: population },
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'DistrictName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '420',
        viewrecords: true,
        caption: 'Block Core Network Details for ' + distname,
        loadComplete: function () {
            //Total of Columns
            var TotalPopulationover1000T = $(this).jqGrid('getCol', 'TotalPopulationover1000', false, 'sum');
            var TotalPopulationover999T = $(this).jqGrid('getCol', 'TotalPopulationover999', false, 'sum');
            var TotalPopulationover499T = $(this).jqGrid('getCol', 'TotalPopulationover499', false, 'sum');
            var TotalPopulationover250T = $(this).jqGrid('getCol', 'TotalPopulationover250', false, 'sum');
            var UnconnectedPopulationover1000T = $(this).jqGrid('getCol', 'UnconnectedPopulationover1000', false, 'sum');
            var UnconnectedPopulationover999T = $(this).jqGrid('getCol', 'UnconnectedPopulationover999', false, 'sum');
            var UnconnectedPopulationover499T = $(this).jqGrid('getCol', 'UnconnectedPopulationover499', false, 'sum');
            var UnconnectedPopulationover250T = $(this).jqGrid('getCol', 'UnconnectedPopulationover250', false, 'sum');
            var ConnectedPopulationover1000T = $(this).jqGrid('getCol', 'ConnectedPopulationover1000', false, 'sum');
            var ConnectedPopulationover999T = $(this).jqGrid('getCol', 'ConnectedPopulationover999', false, 'sum');
            var ConnectedPopulationover499T = $(this).jqGrid('getCol', 'ConnectedPopulationover499', false, 'sum');
            var ConnectedPopulationover250T = $(this).jqGrid('getCol', 'ConnectedPopulationover250', false, 'sum');
            //

            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TotalPopulationover1000: TotalPopulationover1000T }, true);
            $(this).jqGrid('footerData', 'set', { TotalPopulationover999: TotalPopulationover999T }, true);
            $(this).jqGrid('footerData', 'set', { TotalPopulationover499: TotalPopulationover499T }, true);
            $(this).jqGrid('footerData', 'set', { TotalPopulationover250: TotalPopulationover250T }, true);
            $(this).jqGrid('footerData', 'set', { UnconnectedPopulationover1000: UnconnectedPopulationover1000T }, true);
            $(this).jqGrid('footerData', 'set', { UnconnectedPopulationover999: UnconnectedPopulationover999T }, true);
            $(this).jqGrid('footerData', 'set', { UnconnectedPopulationover499: UnconnectedPopulationover499T }, true);
            $(this).jqGrid('footerData', 'set', { UnconnectedPopulationover250: UnconnectedPopulationover250T }, true);
            $(this).jqGrid('footerData', 'set', { ConnectedPopulationover1000: ConnectedPopulationover1000T }, true);
            $(this).jqGrid('footerData', 'set', { ConnectedPopulationover999: ConnectedPopulationover999T }, true);
            $(this).jqGrid('footerData', 'set', { ConnectedPopulationover499: ConnectedPopulationover499T }, true);
            $(this).jqGrid('footerData', 'set', { ConnectedPopulationover250: ConnectedPopulationover250T }, true);
            $('#CN2BlockReportTable_rn').html('Sr.<br/>No.');
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

    $("#CN2BlockReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'TotalPopulationover1000', numberOfColumns: 4, titleText: '<em>Total  </em>' },
          { startColumnName: 'ConnectedPopulationover1000', numberOfColumns: 4, titleText: '<em>Connected</em>' },
          { startColumnName: 'UnconnectedPopulationover1000', numberOfColumns: 4, titleText: '<em>Unconnected</em>' }
        ]
    });
}

/*       FINAL BLOCK REPORT LISTING       */

function CN2FinalReportListing(blockCode, districtCode, stateCode, blockName, population) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#CN2BlockReportTable").jqGrid('setGridState', 'hidden');
    $("#CN2DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#CN2StateReportTable").jqGrid('setGridState', 'hidden');
    $("#CN2BlockReportTable").jqGrid('setSelection', blockCode);
    $("#CN2FinalReportTable").jqGrid('GridUnload');

    $("#CN2FinalReportTable").jqGrid({
        url: '/CoreNetworkReports/CN2FinalReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Road Code', 'Road Name', 'DRRP Road Code', 'DRRP Road Name', 'Road Length (Kms)', 'Road From', 'Road To', 'Route Type', 'Length Covered (P/F)', 'Habitation', 'Population', 'Connected (Y/N)'],
        colModel: [
            { name: 'PlannedRoadNumber', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'PlannedRoadName', width: 120, align: 'left', height: 'auto', sortable: true },
            { name: 'ERRoadNumber', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'ERRoadName', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadLengthinKM', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'RoadFrom', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadTo', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadRoute', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'PlanRoadLength', width: 120, align: 'center', height: 'auto', sortable: false },
            { name: 'HabitationName', width: 120, align: 'left', height: 'auto', sortable: false },
            { name: 'TotalPopulationofHabitation', width: 120, align: 'right', sortable: false, height: 'auto', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'HabitationConnected', width: 120, align: 'center', height: 'auto', sortable: false }
        ],

        postData: { "BlockCode": blockCode, "StateCode": stateCode, "DistrictCode": districtCode,Population:population },
        rowNum: '2147483647',
        pager: $("#CN2FinalReportPager"),
        footerrow: true,
        pgbuttons: true,
        sortname: 'PlannedRoadCode',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '360',
        viewrecords: true,
        caption: 'Core Network Details for ' + blockName,
        //grouping: true,
        //groupingView: {
        //    groupField: ['PlannedRoadNumber'],
        //    groupSummary: [true],
        //    groupColumnShow: [false],
        //    groupDataSorted: true
        //},
        loadComplete: function () {
            //Total of Columns
            var RoadLengthinKMT = $(this).jqGrid('getCol', 'RoadLengthinKM', false, 'sum');
            RoadLengthinKMT = parseFloat(RoadLengthinKMT).toFixed(3);
         
            var TotalPopulationofHabitationT = $(this).jqGrid('getCol', 'TotalPopulationofHabitation', false, 'sum');

            //Set
            $(this).jqGrid('footerData', 'set', { PlannedRoadNumber: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { RoadLengthinKM: RoadLengthinKMT });
             $(this).jqGrid('footerData', 'set', { TotalPopulationofHabitation: TotalPopulationofHabitationT });
            $('#CN2FinalReportTable_rn').html('Sr.<br/>No.');
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