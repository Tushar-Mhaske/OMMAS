/*
    * Project Id    :
    * Project Name  :   OMMAS II
    * Name          :   Form7.js
    * Description   :   Form7 Reports Details in jqGrid, accordingly populate further details as per District, Block etc.
    * Author        :   Shyam Yadav
    * Creation Date :   13/Sep/2013  
    * Depend on Files:  ReportsLayout.js, ReportsMenu.js
*/

$(document).ready(function () {

 


    $("#btnViewForm7Details").click(function () {

        var proposalType = $("#ddlMAST_PROPOSAL_TYPESearchForm7").val();
        var year = $("#ddlMAST_YEARSearchForm7").val();
        var batch = $("#ddlIMS_BATCHSearchForm7").val();
        var collaboration = $("#ddlIMS_COLLABORATIONSearchForm7").val();
      
        if ($("#hdnLevelId").val() == 6) //mord
        {
            //  $("#tbForm7StateLevelReport").jqGrid('GridUnload');
            $("#tbForm7DistrictLevelReport").jqGrid('GridUnload');
            $("#tbForm7BlockLevelReport").jqGrid('GridUnload');
            $("#tbForm4FinalLevelReport").jqGrid('GridUnload');
            loadForm7StateLevelReportGrid(proposalType, year, batch, collaboration);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
            // $("#tbForm7DistrictLevelReport").jqGrid('GridUnload');
            $("#tbForm7BlockLevelReport").jqGrid('GridUnload');
            $("#tbForm4FinalLevelReport").jqGrid('GridUnload');
            loadForm7DistrictLevelReportGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), proposalType, year, batch, collaboration);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
            //  $("#tbForm7BlockLevelReport").jqGrid('GridUnload');
            $("#tbForm4FinalLevelReport").jqGrid('GridUnload');
            loadForm7BlockLevelReportGrid($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), proposalType, year, batch, collaboration);
        }

    }); 

    $("#btnViewForm7Details").trigger('click');

    //$('#tblRptContents').bind('resize', function () {
    //    resizeJqGrid();
    //}).trigger('resize');
});



function loadForm7StateLevelReportGrid(proposalType, year, batch, collaboration) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbForm7StateLevelReport").jqGrid('GridUnload');
    $("#tbForm7DistrictLevelReport").jqGrid('GridUnload');
    $("#tbForm7BlockLevelReport").jqGrid('GridUnload');
    $("#tbForm7FinalLevelReport").jqGrid('GridUnload');

    jQuery("#tbForm7StateLevelReport").jqGrid({
        url: '/FormReports/Form7StateLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "Road Proposal", "Road Cost", "Bridge Proposal", "Bridge Cost", "Road", "Bridge", "Road", "Bridge"],
        colModel: [
                        { name: 'StateName', index: 'StateName', width: 200, sortable: true, align: "left" },
                        { name: 'RdProposal', index: 'RdProposal', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'RdCost', index: 'RdCost', width: 100, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BridgeProposal', index: 'BridgeProposal', width: 120, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BridgeCost', index: 'BridgeCost', width: 100, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'RdAward', index: 'RdAward', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BridgeAward', index: 'BridgeAward', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'RdAwardedAmt', index: 'RdAwardedAmt', width: 100, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BridgeAwardedAmt', index: 'BridgeAwardedAmt', width: 100, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        postData: { "proposalType": proposalType, "batch": batch, "year": year, "collaboration": collaboration },
        pager: jQuery('#dvForm7StateLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;All States",
        //height: ($("#tblRptContents").height() - 175),
        autowidth: false,
        sortname: 'StateName',
        height: 530,
        width: '1120',
        shrinkToFit: false,
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {
            //$("#gview_tbForm1StateLevelReport > .ui-jqgrid-titlebar").hide();

            var totalRdProposal = $(this).jqGrid('getCol', 'RdProposal', false, 'sum');
            var totalRdCost = $(this).jqGrid('getCol', 'RdCost', false, 'sum');
            totalRdCost = parseFloat(totalRdCost).toFixed(2);
            var totalBridgeProposal = $(this).jqGrid('getCol', 'BridgeProposal', false, 'sum');
             var totalBridgeCost = $(this).jqGrid('getCol', 'BridgeCost', false, 'sum');
             totalBridgeCost = parseFloat(totalBridgeCost).toFixed(2);
            var totalRdAward = $(this).jqGrid('getCol', 'RdAward', false, 'sum');
            var totalBridgeAward = $(this).jqGrid('getCol', 'BridgeAward', false, 'sum');
            var totalRdAwardedAmt = $(this).jqGrid('getCol', 'RdAwardedAmt', false, 'sum');
            totalRdAwardedAmt = parseFloat(totalRdAwardedAmt).toFixed(2);
            var totalBridgeAwardedAmt = $(this).jqGrid('getCol', 'BridgeAwardedAmt', false, 'sum');
            totalBridgeAwardedAmt = parseFloat(totalBridgeAwardedAmt).toFixed(2)

            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { RdProposal: totalRdProposal });
            $(this).jqGrid('footerData', 'set', { RdCost: totalRdCost });
            $(this).jqGrid('footerData', 'set', { BridgeProposal: totalBridgeProposal });
            $(this).jqGrid('footerData', 'set', { BridgeCost: totalBridgeCost });

            $(this).jqGrid('footerData', 'set', { RdAward: totalRdAward });
            $(this).jqGrid('footerData', 'set', { BridgeAward: totalBridgeAward });
            $(this).jqGrid('footerData', 'set', { RdAwardedAmt: totalRdAwardedAmt });
            $(this).jqGrid('footerData', 'set', { BridgeAwardedAmt: totalBridgeAwardedAmt });
            $('#tbForm7StateLevelReport_rn').html('Sr.<br/>No.');

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


    $("#tbForm7StateLevelReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'RdAward', numberOfColumns: 2, titleText: 'Award' },
                       { startColumnName: 'RdAwardedAmt', numberOfColumns: 2, titleText: 'Awarded Amount' }
        ]
    });
}


function loadForm7DistrictLevelReportGrid(stateCode, stateName, proposalType, year, batch, collaboration) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#tbForm7StateLevelReport').jqGrid('setSelection', stateCode);
    $('#tbForm7StateLevelReport').jqGrid('setGridState', 'hidden');
    $("#tbForm7DistrictLevelReport").jqGrid('GridUnload');
    $("#tbForm7BlockLevelReport").jqGrid('GridUnload');
    $("#tbForm7FinalLevelReport").jqGrid('GridUnload');
    jQuery("#tbForm7DistrictLevelReport").jqGrid({
        url: '/FormReports/Form7DistrictLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["District", "Road Proposal", "Road Cost", "Bridge Proposal", "Bridge Cost", "Road", "Bridge", "Road", "Bridge"],
        colModel: [
                        { name: 'DistrictName', index: 'DistrictName', width: 200, sortable: true, align: "left" },
                        { name: 'RdProposal', index: 'RdProposal', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'RdCost', index: 'RdCost', width: 100, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BridgeProposal', index: 'BridgeProposal', width: 120, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BridgeCost', index: 'BridgeCost', width: 100, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'RdAward', index: 'RdAward', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BridgeAward', index: 'BridgeAward', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'RdAwardedAmt', index: 'RdAwardedAmt', width: 100, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BridgeAwardedAmt', index: 'BridgeAwardedAmt', width: 100, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        postData: { "stateCode": stateCode, "proposalType": proposalType, "batch": batch, "year": year, "collaboration": collaboration },
        pager: jQuery('#dvForm7DistrictLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;State - " + stateName,
        //height: ($("#tblRptContents").height() - 185),
        autowidth: false,
        sortname: 'StateName',
        height: 470,
        width: '1120',
        shrinkToFit: false,
        rownumbers: true,
        footerrow: true,

        loadComplete: function () {

            var totalRdProposal = $(this).jqGrid('getCol', 'RdProposal', false, 'sum');
            var totalRdCost = $(this).jqGrid('getCol', 'RdCost', false, 'sum');
            totalRdCost = parseFloat(totalRdCost).toFixed(2);
            var totalBridgeProposal = $(this).jqGrid('getCol', 'BridgeProposal', false, 'sum');
            var totalBridgeCost = $(this).jqGrid('getCol', 'BridgeCost', false, 'sum');
            totalBridgeCost = parseFloat(totalBridgeCost).toFixed(2);
            var totalRdAward = $(this).jqGrid('getCol', 'RdAward', false, 'sum');
            var totalBridgeAward = $(this).jqGrid('getCol', 'BridgeAward', false, 'sum');
            var totalRdAwardedAmt = $(this).jqGrid('getCol', 'RdAwardedAmt', false, 'sum');
            totalRdAwardedAmt = parseFloat(totalRdAwardedAmt).toFixed(2);
            var totalBridgeAwardedAmt = $(this).jqGrid('getCol', 'BridgeAwardedAmt', false, 'sum');
            totalBridgeAwardedAmt = parseFloat(totalBridgeAwardedAmt).toFixed(2)

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { RdProposal: totalRdProposal });
            $(this).jqGrid('footerData', 'set', { RdCost: totalRdCost });
            $(this).jqGrid('footerData', 'set', { BridgeProposal: totalBridgeProposal });
            $(this).jqGrid('footerData', 'set', { BridgeCost: totalBridgeCost });

            $(this).jqGrid('footerData', 'set', { RdAward: totalRdAward });
            $(this).jqGrid('footerData', 'set', { BridgeAward: totalBridgeAward });
            $(this).jqGrid('footerData', 'set', { RdAwardedAmt: totalRdAwardedAmt });
            $(this).jqGrid('footerData', 'set', { BridgeAwardedAmt: totalBridgeAwardedAmt });

            $('#tbForm7DistrictLevelReport_rn').html('Sr.<br/>No.');

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


    $("#tbForm7DistrictLevelReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'RdAward', numberOfColumns: 2, titleText: 'Award' },
                       { startColumnName: 'RdAwardedAmt', numberOfColumns: 2, titleText: 'Awarded Amount' }
        ]
    });
}


function loadForm7BlockLevelReportGrid(stateCode, districtCode,districtName, proposalType, year, batch, collaboration) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#tbForm7StateLevelReport').jqGrid('setSelection', stateCode);
    $('#tbForm7DistrictLevelReport').jqGrid('setSelection', districtCode);
    $('#tbForm7StateLevelReport').jqGrid('setGridState', 'hidden');
    $('#tbForm7DistrictLevelReport').jqGrid('setGridState', 'hidden');
    $("#tbForm7BlockLevelReport").jqGrid('GridUnload');
    $("#tbForm7FinalLevelReport").jqGrid('GridUnload');

    jQuery("#tbForm7BlockLevelReport").jqGrid({
        url: '/FormReports/Form7BlockLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Block", "Road Proposal", "Road Cost", "Bridge Proposal", "Bridge Cost", "Road", "Bridge", "Road", "Bridge"],
        colModel: [
                        { name: 'BlockName', index: 'DistrictName', width: 200, sortable: true, align: "left" },
                        { name: 'RdProposal', index: 'RdProposal', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'RdCost', index: 'RdCost', width: 100, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BridgeProposal', index: 'BridgeProposal', width: 120, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BridgeCost', index: 'BridgeCost', width: 100, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'RdAward', index: 'RdAward', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BridgeAward', index: 'BridgeAward', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'RdAwardedAmt', index: 'RdAwardedAmt', width: 100, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BridgeAwardedAmt', index: 'BridgeAwardedAmt', width: 100, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        postData: { "stateCode": stateCode, "districtCode": districtCode, "proposalType": proposalType, "batch": batch, "year": year, "collaboration": collaboration },
        pager: jQuery('#dvForm7BlockLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;District - " + districtName,
        //height: ($("#tblRptContents").height() - 175),
        autowidth: false,
        sortname: 'BlockName',
        height: 420,
        width: '1120',
        shrinkToFit: false,
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {
            var totalRdProposal = $(this).jqGrid('getCol', 'RdProposal', false, 'sum');
            var totalRdCost = $(this).jqGrid('getCol', 'RdCost', false, 'sum');
            totalRdCost = parseFloat(totalRdCost).toFixed(2);
            var totalBridgeProposal = $(this).jqGrid('getCol', 'BridgeProposal', false, 'sum');
            var totalBridgeCost = $(this).jqGrid('getCol', 'BridgeCost', false, 'sum');
            totalBridgeCost = parseFloat(totalBridgeCost).toFixed(2);
            var totalRdAward = $(this).jqGrid('getCol', 'RdAward', false, 'sum');
            var totalBridgeAward = $(this).jqGrid('getCol', 'BridgeAward', false, 'sum');
            var totalRdAwardedAmt = $(this).jqGrid('getCol', 'RdAwardedAmt', false, 'sum');
            totalRdAwardedAmt = parseFloat(totalRdAwardedAmt).toFixed(2);
            var totalBridgeAwardedAmt = $(this).jqGrid('getCol', 'BridgeAwardedAmt', false, 'sum');
            totalBridgeAwardedAmt = parseFloat(totalBridgeAwardedAmt).toFixed(2)

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { RdProposal: totalRdProposal });
            $(this).jqGrid('footerData', 'set', { RdCost: totalRdCost });
            $(this).jqGrid('footerData', 'set', { BridgeProposal: totalBridgeProposal });
            $(this).jqGrid('footerData', 'set', { BridgeCost: totalBridgeCost });

            $(this).jqGrid('footerData', 'set', { RdAward: totalRdAward });
            $(this).jqGrid('footerData', 'set', { BridgeAward: totalBridgeAward });
            $(this).jqGrid('footerData', 'set', { RdAwardedAmt: totalRdAwardedAmt });
            $(this).jqGrid('footerData', 'set', { BridgeAwardedAmt: totalBridgeAwardedAmt });
            $('#tbForm7BlockLevelReport_rn').html('Sr.<br/>No.');

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

    $("#tbForm7BlockLevelReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'RdAward', numberOfColumns: 2, titleText: 'Award' },
                       { startColumnName: 'RdAwardedAmt', numberOfColumns: 2, titleText: 'Awarded Amount' }
        ]
    });

}


function loadForm7FinalLevelReportGrid(stateCode, districtCode,blockCode, blockName, proposalType, year, batch, collaboration) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


    $('#tbForm7StateLevelReport').jqGrid('setSelection', stateCode);
    $('#tbForm7DistrictLevelReport').jqGrid('setSelection', districtCode);
    $('#tbForm7BlockLevelReport').jqGrid('setSelection', blockCode);
    $('#tbForm7StateLevelReport').jqGrid('setGridState', 'hidden');
    $('#tbForm7DistrictLevelReport').jqGrid('setGridState', 'hidden');
    $('#tbForm7BlockLevelReport').jqGrid('setGridState', 'hidden');
    $("#tbForm7FinalLevelReport").jqGrid('GridUnload');

    jQuery("#tbForm7FinalLevelReport").jqGrid({
        url: '/FormReports/Form7FinalLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Block", "Road Name", "Sanction Year", "Sanction Date", "Bridge Name", "Road", "Bridge", "Road", "Bridge", "Road (State)", "Bridge (State)", "Maintenence", "Agreement", "Agreement Maintenence", "Contractor", "Agreement Number", "Date of Agreement", "Start", "End", "Commencement", "Completion", "Award Work", "Work Order"],
        colModel: [
                        { name: 'BlockName', index: 'BlockName', width: 100, sortable: true, align: "left",hidden:true },
                        { name: 'RdName', index: 'RdName', width: 200, sortable: false, align: "left", search: false },
                        { name: 'SancYear', index: 'SancYear', width: 70, sortable: false, align: "left", search: false },
                        { name: 'SancDate', index: 'SancDate', width: 100, sortable: false, align: "left", search: false},
                        { name: 'BridgeName', index: 'BridgeName', width: 120, sortable: false, align: "left", search: false },
                        { name: 'RdLength', index: 'RdLength', width: 80, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BridgeLength', index: 'BridgeLength', width: 80, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'RdAmt', index: 'RdAmt', width: 80, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BridgeAmt', index: 'BridgeAmt', width: 80, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'StateRdAmt', index: 'StateRdAmt', width: 80, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'StateBrAmt', index: 'StateBrAmt', width: 80, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'MaintAmt', index: 'MaintAmt', width: 80, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'AgrAmt', index: 'AgrAmt', width: 80, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'AgrMaintAmt', index: 'AgrMaintAmt', width: 80, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'Contractor', index: 'Contractor', width: 150, sortable: false, align: "left", search: false },
                        { name: 'AgrNumber', index: 'AgrNumber', width: 150, sortable: false, align: "left", search: false },
                        { name: 'AgrDate', index: 'AgrDate', width: 80, sortable: false, align: "left", search: false },
                        { name: 'AgrStartDate', index: 'AgrStartDate', width: 80, sortable: false, align: "left", search: false },
                        { name: 'AgrEndDate', index: 'AgrEndDate', width: 80, sortable: false, align: "left", search: false },
                        { name: 'CommencementDate', index: 'CommencementDate', width: 80, sortable: false, align: "left", search: false },
                        { name: 'CompletionDate', index: 'CompletionDate', width: 80, sortable: false, align: "left", search: false },
                        { name: 'AwardWorkDate', index: 'AwardWorkDate', width: 80, sortable: false, align: "left", search: false },
                        { name: 'WorkOrderDate', index: 'WorkOrderDate', width: 80, sortable: false, align: "left", search: false }
        ],
        postData: { "stateCode": stateCode, "districtCode": districtCode, "blockCode": blockCode, "proposalType": proposalType, "batch": batch, "year": year, "collaboration": collaboration },
        pager: jQuery('#dvForm7FinalLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Block - " + blockName,
        //height: ($("#tblRptContents").height() - 175),
        autowidth: false,
        sortname: 'BlockName',
        height: 370,
        width: 1120,
        shrinkToFit: false,
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {
            //$("#gview_tbForm1StateLevelReport > .ui-jqgrid-titlebar").hide();

            var totalRdLength = $(this).jqGrid('getCol', 'RdLength', false, 'sum');
            totalRdLength = parseFloat(totalRdLength).toFixed(3);
            var totalBridgeLength = $(this).jqGrid('getCol', 'BridgeLength', false, 'sum');
            totalBridgeLength = parseFloat(totalBridgeLength).toFixed(3);
            var totalRdAmt = $(this).jqGrid('getCol', 'RdAmt', false, 'sum');
            totalRdAmt = parseFloat(totalRdAmt).toFixed(2);
            var totalBridgeAmt = $(this).jqGrid('getCol', 'BridgeAmt', false, 'sum');
            totalBridgeAmt = parseFloat(totalBridgeAmt).toFixed(2);
            var totalStateRdAmt = $(this).jqGrid('getCol', 'StateRdAmt', false, 'sum');
            totalStateRdAmt = parseFloat(totalStateRdAmt).toFixed(2);
            var totalStateBrAmt = $(this).jqGrid('getCol', 'StateBrAmt', false, 'sum');
            totalStateBrAmt = parseFloat(totalStateBrAmt).toFixed(2);
            var totalMaintAmt = $(this).jqGrid('getCol', 'MaintAmt', false, 'sum');
            totalMaintAmt = parseFloat(totalMaintAmt).toFixed(2);
            var totalAgrAmt = $(this).jqGrid('getCol', 'AgrAmt', false, 'sum');
            totalAgrAmt = parseFloat(totalAgrAmt).toFixed(2);
            var totalAgrMaintAmt = $(this).jqGrid('getCol', 'AgrMaintAmt', false, 'sum');
            totalAgrMaintAmt = parseFloat(totalAgrMaintAmt).toFixed(2);

            $(this).jqGrid('footerData', 'set', { RdName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { RdLength: totalRdLength });
            $(this).jqGrid('footerData', 'set', { BridgeLength: totalBridgeLength });
            $(this).jqGrid('footerData', 'set', { RdAmt: totalRdAmt });
            $(this).jqGrid('footerData', 'set', { BridgeAmt: totalBridgeAmt });
            $(this).jqGrid('footerData', 'set', { StateRdAmt: totalStateRdAmt });
            $(this).jqGrid('footerData', 'set', { StateBrAmt: totalStateBrAmt });
            $(this).jqGrid('footerData', 'set', { MaintAmt: totalMaintAmt });
            $(this).jqGrid('footerData', 'set', { AgrAmt: totalAgrAmt });
            $(this).jqGrid('footerData', 'set', { AgrMaintAmt: totalAgrMaintAmt });
            $('#tbForm7FinalLevelReport_rn').html('Sr.<br/>No.');

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

    $("#tbForm7FinalLevelReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'RdLength', numberOfColumns: 2, titleText: 'Length (Km)' },
                       { startColumnName: 'RdAmt', numberOfColumns: 7, titleText: 'Amount' },
                       { startColumnName: 'AgrStartDate', numberOfColumns: 6, titleText: 'Date' }
        ]
    });

}




