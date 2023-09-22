/*
    * Project Id    :
    * Project Name  :   OMMAS II
    * Name          :   Form4StateLevel.js
    * Description   :   Form4 Reports Details in jqGrid, accordingly populate further details as per District, Block etc.
    * Author        :   Shyam Yadav
    * Creation Date :   10/Sep/2013  
    * Depend on Files:  ReportsLayout.js, ReportsMenu.js
*/

$(document).ready(function () {


    //on chage, reload State Details Grid
    $("#btnViewForm4Details").click(function () {
        

        var proposalType = $("#ddlMAST_PROPOSAL_TYPESearchForm4").val();
        var year = $("#ddlMAST_YEARSearchForm4").val();
        var batch = $("#ddlIMS_BATCHSearchForm4").val();
        var collaboration = $("#ddlIMS_COLLABORATIONSearchForm4").val();

        if ($("#hdnLevelId").val() == 6) //mord
        {
            //  $("#CNPriorityStateReportTable").jqGrid('GridUnload');
            $("#tbForm4DistrictLevelReport").jqGrid('GridUnload');
            $("#tbForm4BlockLevelReport").jqGrid('GridUnload');
            $("#tbForm4FinalLevelReport").jqGrid('GridUnload');
            loadForm4StateLevelReportGrid(proposalType, year, batch, collaboration);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
            // $("#tbForm4DistrictLevelReport").jqGrid('GridUnload');
            $("#tbForm4BlockLevelReport").jqGrid('GridUnload');
            $("#tbForm4FinalLevelReport").jqGrid('GridUnload');
            loadForm4DistrictLevelReportGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), proposalType, year, batch, collaboration);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
            //  $("#tbForm4BlockLevelReport").jqGrid('GridUnload');
            $("#tbForm4FinalLevelReport").jqGrid('GridUnload');
            loadForm4BlockLevelReportGrid($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), proposalType, year, batch, collaboration);
        }
    });


    $("#btnViewForm4Details").trigger('click');

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
    });

function loadForm4StateLevelReportGrid(proposalType, year, batch, collaboration) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbForm4StateLevelReport").jqGrid('GridUnload');
    $("#tbForm4DistrictLevelReport").jqGrid('GridUnload');
    $("#tbForm4BlockLevelReport").jqGrid('GridUnload');
    $("#tbForm4FinalLevelReport").jqGrid('GridUnload');

    jQuery("#tbForm4StateLevelReport").jqGrid({
        url: '/FormReports/Form4StateLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["State","Total Proposals", "Road", "Bridge", "Road", "Bridge", "Road (State)", "Bridge (State)", "Maintenence", "1000 +", "500 to 999", "250 to 499", "< 250"],
        colModel: [
                        { name: 'StateName', index: 'StateName', width: 250, sortable: true, align: "left" },
                        { name: 'TotProposals', index: 'TotProposals', width: 120, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'RdLength', index: 'RdLength', width: 120, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BrLength', index: 'BrLength', width: 120, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'RdAmt', index: 'RdAmt', width: 120, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BrAmt', index: 'BrAmt', width: 120, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'StateRdAmt', index: 'StateRdAmt', width: 120, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'StateBrAmt', index: 'StateBrAmt', width: 120, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'MaintenenceAmt', index: 'MaintenenceAmt', width: 120, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver1000', index: 'PopOver1000', width: 120, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver500', index: 'PopOver500', width: 120, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver250', index: 'PopOver250', width: 120, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopBelow250', index: 'PopBelow250', width: 120, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "proposalType": proposalType, "batch": batch, "year": year, "collaboration": collaboration },
        pager: jQuery('#dvForm4StateLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;All States",
        //height: ($("#tblRptContents").height() - 175),
        autowidth: true,
        sortname: 'StateName',
        height:520,
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {
            //$("#gview_tbForm1StateLevelReport > .ui-jqgrid-titlebar").hide();

            if ($("#hdnLevelId").val() != 5) //for district level, no need to show totals
            {
                var totalProposals = $(this).jqGrid('getCol', 'TotProposals', false, 'sum');               
                var totalRdLength = $(this).jqGrid('getCol', 'RdLength', false, 'sum');
                totalRdLength = parseFloat(totalRdLength).toFixed(3);
                var totalBrLength = $(this).jqGrid('getCol', 'BrLength', false, 'sum');
                totalBrLength = parseFloat(totalBrLength).toFixed(3);
                var totalRdAmt = $(this).jqGrid('getCol', 'RdAmt', false, 'sum');
                totalRdAmt = parseFloat(totalRdAmt).toFixed(2);
                var totalBrAmt = $(this).jqGrid('getCol', 'BrAmt', false, 'sum');
                totalBrAmt = parseFloat(totalBrAmt).toFixed(2);
                var totalStateRdAmt = $(this).jqGrid('getCol', 'StateRdAmt', false, 'sum');
                totalStateRdAmt = parseFloat(totalStateRdAmt).toFixed(2);
                var totalStateBrAmt = $(this).jqGrid('getCol', 'StateBrAmt', false, 'sum');
                totalStateBrAmt = parseFloat(totalStateBrAmt).toFixed(2);
                var totalMaintenenceAmt = $(this).jqGrid('getCol', 'MaintenenceAmt', false, 'sum');
                totalMaintenenceAmt = parseFloat(totalMaintenenceAmt).toFixed(2);

                var totalPopOver1000 = $(this).jqGrid('getCol', 'PopOver1000', false, 'sum');
                var totalPopOver500 = $(this).jqGrid('getCol', 'PopOver500', false, 'sum');
                var totalPopOver250 = $(this).jqGrid('getCol', 'PopOver250', false, 'sum');
                var totalPopBelow250 = $(this).jqGrid('getCol', 'PopBelow250', false, 'sum');

                $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });

                $(this).jqGrid('footerData', 'set', { TotProposals: totalProposals });
                $(this).jqGrid('footerData', 'set', { RdLength: totalRdLength });
                $(this).jqGrid('footerData', 'set', { BrLength: totalBrLength });
                $(this).jqGrid('footerData', 'set', { RdAmt: totalRdAmt });
                $(this).jqGrid('footerData', 'set', { BrAmt: totalBrAmt });
                $(this).jqGrid('footerData', 'set', { StateRdAmt: totalStateRdAmt });
                $(this).jqGrid('footerData', 'set', { StateBrAmt: totalStateBrAmt });
                $(this).jqGrid('footerData', 'set', { MaintenenceAmt: totalMaintenenceAmt });

                $(this).jqGrid('footerData', 'set', { PopOver1000: totalPopOver1000 });
                $(this).jqGrid('footerData', 'set', { PopOver500: totalPopOver500 });
                $(this).jqGrid('footerData', 'set', { PopOver250: totalPopOver250 });
                $(this).jqGrid('footerData', 'set', { PopBelow250: totalPopBelow250 });
            }
            $('#tbForm4StateLevelReport_rn').html('Sr.<br/>No.');

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


    $("#tbForm4StateLevelReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
            { startColumnName: 'RdLength', numberOfColumns: 2, titleText: 'Length (Kms.)' },
            { startColumnName: 'RdAmt', numberOfColumns: 5, titleText: 'Amount (Rs. in Lacs)' },
            { startColumnName: 'PopOver1000', numberOfColumns: 4, titleText: 'Population' },
        ]
    });
}

function loadForm4DistrictLevelReportGrid(stateCode, stateName, proposalType, year, batch, collaboration) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#tbForm4StateLevelReport').jqGrid('setSelection', stateCode);
    $('#tbForm4StateLevelReport').jqGrid('setGridState', 'hidden');
    $("#tbForm4DistrictLevelReport").jqGrid('GridUnload');
    $("#tbForm4BlockLevelReport").jqGrid('GridUnload');
    $("#tbForm4FinalLevelReport").jqGrid('GridUnload');

    jQuery("#tbForm4DistrictLevelReport").jqGrid({
        url: '/FormReports/Form4DistrictLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        //colNames: ["District", "Total Proposals", "Road Length", "Bridge Length", "Road Amount", "Bridge Amount", "State Road Amount", "State Bridge Amount", "Maintenence Amount", "Population (Over 1000)", "Population (Over 500)", "Population (Over 250)", "Population (Upto 250)"],
        colNames: ["District", "Total Proposals", "Road", "Bridge", "Road", "Bridge", "Road (State)", "Bridge (State)", "Maintenence", "1000 +", "500 to 999", "250 to 499", "< 250"],
        colModel: [
                        { name: 'DistrictName', index: 'DistrictName', width: 200, sortable: true, align: "left" },
                        { name: 'TotProposals', index: 'TotProposals', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'RdLength', index: 'RdLength', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BrLength', index: 'BrLength', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'RdAmt', index: 'RdAmt', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BrAmt', index: 'BrAmt', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'StateRdAmt', index: 'StateRdAmt', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'StateBrAmt', index: 'StateBrAmt', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'MaintenenceAmt', index: 'MaintenenceAmt', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver1000', index: 'PopOver1000', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver500', index: 'PopOver500', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver250', index: 'PopOver250', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopBelow250', index: 'PopBelow250', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "stateCode": stateCode, "proposalType": proposalType, "batch": batch, "year": year, "collaboration": collaboration },
        pager: jQuery('#dvForm4DistrictLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;State - " + stateName,
        //height: ($("#tblRptContents").height() - 185),
        autowidth: true,
        sortname: 'StateName',
        height:470,
        rownumbers: true,
        footerrow: true,

        loadComplete: function () {
            //$("#gview_tbForm1StateLevelReport > .ui-jqgrid-titlebar").hide();

            if ($("#hdnLevelId").val() != 5) //for district level, no nned to show totals
            {
                var totalProposals = $(this).jqGrid('getCol', 'TotProposals', false, 'sum');
                var totalRdLength = $(this).jqGrid('getCol', 'RdLength', false, 'sum');
                totalRdLength = parseFloat(totalRdLength).toFixed(3);
                var totalBrLength = $(this).jqGrid('getCol', 'BrLength', false, 'sum');
                totalBrLength = parseFloat(totalBrLength).toFixed(3);
                var totalRdAmt = $(this).jqGrid('getCol', 'RdAmt', false, 'sum');
                totalRdAmt = parseFloat(totalRdAmt).toFixed(2);
                var totalBrAmt = $(this).jqGrid('getCol', 'BrAmt', false, 'sum');
                totalBrAmt = parseFloat(totalBrAmt).toFixed(2);
                var totalStateRdAmt = $(this).jqGrid('getCol', 'StateRdAmt', false, 'sum');
                totalStateRdAmt = parseFloat(totalStateRdAmt).toFixed(2);
                var totalStateBrAmt = $(this).jqGrid('getCol', 'StateBrAmt', false, 'sum');
                totalStateBrAmt = parseFloat(totalStateBrAmt).toFixed(2);
                var totalMaintenenceAmt = $(this).jqGrid('getCol', 'MaintenenceAmt', false, 'sum');
                totalMaintenenceAmt = parseFloat(totalMaintenenceAmt).toFixed(2);

                var totalPopOver1000 = $(this).jqGrid('getCol', 'PopOver1000', false, 'sum');
                var totalPopOver500 = $(this).jqGrid('getCol', 'PopOver500', false, 'sum');
                var totalPopOver250 = $(this).jqGrid('getCol', 'PopOver250', false, 'sum');
                var totalPopBelow250 = $(this).jqGrid('getCol', 'PopBelow250', false, 'sum');

                $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });

                $(this).jqGrid('footerData', 'set', { TotProposals: totalProposals });
                $(this).jqGrid('footerData', 'set', { RdLength: totalRdLength });
                $(this).jqGrid('footerData', 'set', { BrLength: totalBrLength });
                $(this).jqGrid('footerData', 'set', { RdAmt: totalRdAmt });
                $(this).jqGrid('footerData', 'set', { BrAmt: totalBrAmt });
                $(this).jqGrid('footerData', 'set', { StateRdAmt: totalStateRdAmt });
                $(this).jqGrid('footerData', 'set', { StateBrAmt: totalStateBrAmt });
                $(this).jqGrid('footerData', 'set', { MaintenenceAmt: totalMaintenenceAmt });

                $(this).jqGrid('footerData', 'set', { PopOver1000: totalPopOver1000 });
                $(this).jqGrid('footerData', 'set', { PopOver500: totalPopOver500 });
                $(this).jqGrid('footerData', 'set', { PopOver250: totalPopOver250 });
                $(this).jqGrid('footerData', 'set', { PopBelow250: totalPopBelow250 });
            }
            $('#tbForm4DistrictLevelReport_rn').html('Sr.<br/>No.');

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


    $("#tbForm4DistrictLevelReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
            { startColumnName: 'RdLength', numberOfColumns: 2, titleText: 'Length (Kms.)' },
            { startColumnName: 'RdAmt', numberOfColumns: 5, titleText: 'Amount (Rs. in Lacs)' },
            { startColumnName: 'PopOver1000', numberOfColumns: 4, titleText: 'Population' },
        ]
    });

}

function loadForm4BlockLevelReportGrid(stateCode, districtCode, districtName, proposalType, year, batch, collaboration) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#tbForm4DistrictLevelReport').jqGrid('setSelection', districtCode);
    $('#tbForm4DistrictLevelReport').jqGrid('setGridState', 'hidden');
    $('#tbForm4StateLevelReport').jqGrid('setGridState', 'hidden');

    $("#tbForm4BlockLevelReport").jqGrid('GridUnload');
    $("#tbForm4FinalLevelReport").jqGrid('GridUnload');

    jQuery("#tbForm4BlockLevelReport").jqGrid({
        url: '/FormReports/Form4BlockLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        //colNames: ["Block", "Road Name", "Bridge Name", "Carriage Width", "Is Staged", "No. Of CD Works", "Upgrade Connect", "Road Length", "Bridge Length", "Road Amount", "Bridge Amount", "State Road Amount", "State Bridge Amount", "Maintenence Amount", "Population (Over 1000)", "Population (Over 500)", "Population (Over 250)", "Population (Upto 250)"],
        colNames: ["Block", "Total Proposals", "Road", "Bridge", "Road", "Bridge", "Road (State)", "Bridge (State)", "Maintenence", "1000 +", "500 to 999", "250 to 499", "< 250"],
        colModel: [
                        { name: 'BlockName', index: 'DistrictName', width: 200, sortable: true, align: "left" },
                        { name: 'TotProposals', index: 'TotProposals', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'RdLength', index: 'RdLength', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BrLength', index: 'BrLength', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'RdAmt', index: 'RdAmt', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BrAmt', index: 'BrAmt', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'StateRdAmt', index: 'StateRdAmt', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'StateBrAmt', index: 'StateBrAmt', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'MaintenenceAmt', index: 'MaintenenceAmt', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver1000', index: 'PopOver1000', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver500', index: 'PopOver500', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver250', index: 'PopOver250', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopBelow250', index: 'PopBelow250', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "stateCode": stateCode, "districtCode": districtCode, "proposalType": proposalType, "batch": batch, "year": year, "collaboration": collaboration },
        pager: jQuery('#dvForm4BlockLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;District - " + districtName,
        //height: ($("#tblRptContents").height() - 185),
        autowidth: true,
        sortname: 'BlockName',
        height:420,
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {
            //$("#gview_tbForm1StateLevelReport > .ui-jqgrid-titlebar").hide();

            var totalProposals = $(this).jqGrid('getCol', 'TotProposals', false, 'sum');
            var totalRdLength = $(this).jqGrid('getCol', 'RdLength', false, 'sum');
            totalRdLength = parseFloat(totalRdLength).toFixed(3);
            var totalBrLength = $(this).jqGrid('getCol', 'BrLength', false, 'sum');
            totalBrLength = parseFloat(totalBrLength).toFixed(3);
            var totalRdAmt = $(this).jqGrid('getCol', 'RdAmt', false, 'sum');
            totalRdAmt = parseFloat(totalRdAmt).toFixed(2);
            var totalBrAmt = $(this).jqGrid('getCol', 'BrAmt', false, 'sum');
            totalBrAmt = parseFloat(totalBrAmt).toFixed(2);
            var totalStateRdAmt = $(this).jqGrid('getCol', 'StateRdAmt', false, 'sum');
            totalStateRdAmt = parseFloat(totalStateRdAmt).toFixed(2);
            var totalStateBrAmt = $(this).jqGrid('getCol', 'StateBrAmt', false, 'sum');
            totalStateBrAmt = parseFloat(totalStateBrAmt).toFixed(2);
            var totalMaintenenceAmt = $(this).jqGrid('getCol', 'MaintenenceAmt', false, 'sum');
            totalMaintenenceAmt = parseFloat(totalMaintenenceAmt).toFixed(2);

            var totalPopOver1000 = $(this).jqGrid('getCol', 'PopOver1000', false, 'sum');
            var totalPopOver500 = $(this).jqGrid('getCol', 'PopOver500', false, 'sum');
            var totalPopOver250 = $(this).jqGrid('getCol', 'PopOver250', false, 'sum');
            var totalPopBelow250 = $(this).jqGrid('getCol', 'PopBelow250', false, 'sum');

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { TotProposals: totalProposals });
            $(this).jqGrid('footerData', 'set', { RdLength: totalRdLength });
            $(this).jqGrid('footerData', 'set', { BrLength: totalBrLength });
            $(this).jqGrid('footerData', 'set', { RdAmt: totalRdAmt });
            $(this).jqGrid('footerData', 'set', { BrAmt: totalBrAmt });
            $(this).jqGrid('footerData', 'set', { StateRdAmt: totalStateRdAmt });
            $(this).jqGrid('footerData', 'set', { StateBrAmt: totalStateBrAmt });
            $(this).jqGrid('footerData', 'set', { MaintenenceAmt: totalMaintenenceAmt });

            $(this).jqGrid('footerData', 'set', { PopOver1000: totalPopOver1000 });
            $(this).jqGrid('footerData', 'set', { PopOver500: totalPopOver500 });
            $(this).jqGrid('footerData', 'set', { PopOver250: totalPopOver250 });
            $(this).jqGrid('footerData', 'set', { PopBelow250: totalPopBelow250 });

            $('#tbForm4BlockLevelReport_rn').html('Sr.<br/>No.');
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

    $("#tbForm4BlockLevelReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
            { startColumnName: 'RdLength', numberOfColumns: 2, titleText: 'Length (Kms.)' },
            { startColumnName: 'RdAmt', numberOfColumns: 5, titleText: 'Amount (Rs. in Lacs)' },
            { startColumnName: 'PopOver1000', numberOfColumns: 4, titleText: 'Population' },
        ]
    });

}

function loadForm4FinalLevelReportGrid(stateCode, districtCode, blockCode, blockName, proposalType, year, batch, collaboration) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbForm4BlockLevelReport").jqGrid('setSelection', blockCode);
    $("#tbForm4BlockLevelReport").jqGrid('setGridState', 'hidden'); //block
    $('#tbForm4StateLevelReport').jqGrid('setGridState', 'hidden'); //State
    $("#tbForm4DistrictLevelReport").jqGrid('setGridState', 'hidden'); //District

    $("#tbForm4FinalLevelReport").jqGrid('GridUnload');
    jQuery("#tbForm4FinalLevelReport").jqGrid({
        url: '/FormReports/Form4FinalLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
         colNames: ["Block","Package No.", "Road Name", "Road Number","Bridge Name", "Carriage Width", "Is Staged", "No. Of CD Works", "Upgrade Connect", "Road", "Bridge", "Road", "Bridge", "Road (State)", "Bridge (State)", "Maintenence", "1000 +", "500 to 999", "250 to 499", "< 250"],
        colModel: [
                        { name: 'BlockName', index: 'BlockName', width: 150, sortable: true, align: "left", hidden: true },
                        { name: 'PackageNo', index: 'PackageNo', width: 180, sortable: false, align: "left" },
                        { name: 'RdName', index: 'RdName', width: 250, sortable: false, align: "left", search: false },
                        { name: 'RdCode', index: 'RdCode', width: 200, sortable: false, align: "left", search: false },
                        { name: 'BrName', index: 'BrName', width: 150, sortable: false, align: "left", search: false },
                        { name: 'CARRIAGE_WIDTH', index: 'CARRIAGE_WIDTH', width: 120, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'IS_STAGED', index: 'IS_STAGED', width: 120, sortable: false, align: "right", search: false },
                        { name: 'IMS_NO_OF_CDWORKS', index: 'IMS_NO_OF_CDWORKS', width: 120, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'UpgradeConnect', index: 'UpgradeConnect', width: 120, sortable: false, align: "left", search: false },
                        { name: 'RdLength', index: 'RdLength', width: 120, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BrLength', index: 'BrLength', width: 120, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'RdAmt', index: 'RdAmt', width: 120, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BrAmt', index: 'BrAmt', width: 120, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'StateRdAmt', index: 'StateRdAmt', width: 120, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'StateBrAmt', index: 'StateBrAmt', width: 120, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'MaintenenceAmt', index: 'MaintenenceAmt', width: 120, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver1000', index: 'PopOver1000', width: 120, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver500', index: 'PopOver500', width: 120, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver250', index: 'PopOver250', width: 120, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopBelow250', index: 'PopBelow250', width: 120, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "stateCode": stateCode, "districtCode": districtCode, "blockCode": blockCode, "proposalType": proposalType, "batch": batch, "year": year, "collaboration": collaboration },
        pager: jQuery('#dvForm4FinalLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Block - " + blockName,
        //height: ($("#tblRptContents").height() - 185),
        autowidth: true,
        sortname: 'BlockName',
        height:370,
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {
            //$("#gview_tbForm1StateLevelReport > .ui-jqgrid-titlebar").hide();

            var totalProposals = $(this).jqGrid('getCol', 'TotProposals', false, 'sum');
            var totalRdLength = $(this).jqGrid('getCol', 'RdLength', false, 'sum');
            totalRdLength = parseFloat(totalRdLength).toFixed(3);
            var totalBrLength = $(this).jqGrid('getCol', 'BrLength', false, 'sum');
            totalBrLength = parseFloat(totalBrLength).toFixed(3);
            var totalRdAmt = $(this).jqGrid('getCol', 'RdAmt', false, 'sum');
            totalRdAmt = parseFloat(totalRdAmt).toFixed(2);
            var totalBrAmt = $(this).jqGrid('getCol', 'BrAmt', false, 'sum');
            totalBrAmt = parseFloat(totalBrAmt).toFixed(2);
            var totalStateRdAmt = $(this).jqGrid('getCol', 'StateRdAmt', false, 'sum');
            totalStateRdAmt = parseFloat(totalStateRdAmt).toFixed(2);
            var totalStateBrAmt = $(this).jqGrid('getCol', 'StateBrAmt', false, 'sum');
            totalStateBrAmt = parseFloat(totalStateBrAmt).toFixed(2);
            var totalMaintenenceAmt = $(this).jqGrid('getCol', 'MaintenenceAmt', false, 'sum');
            totalMaintenenceAmt = parseFloat(totalMaintenenceAmt).toFixed(2);

            var totalPopOver1000 = $(this).jqGrid('getCol', 'PopOver1000', false, 'sum');
            var totalPopOver500 = $(this).jqGrid('getCol', 'PopOver500', false, 'sum');
            var totalPopOver250 = $(this).jqGrid('getCol', 'PopOver250', false, 'sum');
            var totalPopBelow250 = $(this).jqGrid('getCol', 'PopBelow250', false, 'sum');

            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { TotProposals: totalProposals });
            $(this).jqGrid('footerData', 'set', { RdLength: totalRdLength });
            $(this).jqGrid('footerData', 'set', { BrLength: totalBrLength });
            $(this).jqGrid('footerData', 'set', { RdAmt: totalRdAmt });
            $(this).jqGrid('footerData', 'set', { BrAmt: totalBrAmt });
            $(this).jqGrid('footerData', 'set', { StateRdAmt: totalStateRdAmt });
            $(this).jqGrid('footerData', 'set', { StateBrAmt: totalStateBrAmt });
            $(this).jqGrid('footerData', 'set', { MaintenenceAmt: totalMaintenenceAmt });

            $(this).jqGrid('footerData', 'set', { PopOver1000: totalPopOver1000 });
            $(this).jqGrid('footerData', 'set', { PopOver500: totalPopOver500 });
            $(this).jqGrid('footerData', 'set', { PopOver250: totalPopOver250 });
            $(this).jqGrid('footerData', 'set', { PopBelow250: totalPopBelow250 });
            $('#tbForm4FinalLevelReport_rn').html('Sr.<br/>No.');

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

    $("#tbForm4FinalLevelReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
            { startColumnName: 'RdLength', numberOfColumns: 2, titleText: 'Length (Kms.)' },
            { startColumnName: 'RdAmt', numberOfColumns: 5, titleText: 'Amount (Rs. in Lacs)' },
            { startColumnName: 'PopOver1000', numberOfColumns: 4, titleText: 'Population' }, ]
    });

}

