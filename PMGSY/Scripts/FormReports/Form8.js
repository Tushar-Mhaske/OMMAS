/*
    * Project Id    :
    * Project Name  :   OMMAS II
    * Name          :   Form8.js
    * Description   :   Form8 Reports Details in jqGrid, accordingly populate further details as per District, Block etc.
    * Author        :   Shyam Yadav
    * Creation Date :   13/Sep/2013  
    * Depend on Files:  ReportsLayout.js, ReportsMenu.js
*/

$(document).ready(function () {

   


    $("#btnViewForm8Details").click(function () {    

        var proposalType = $("#ddlMAST_PROPOSAL_TYPESearchForm8").val();
        var year = $("#ddlMAST_YEARSearchForm8").val();
        var batch = $("#ddlIMS_BATCHSearchForm8").val();
        var collaboration = $("#ddlIMS_COLLABORATIONSearchForm8").val();

    
        if ($("#hdnLevelId").val() == 6) //mord
        {
            //  $("#tbForm7StateLevelReport").jqGrid('GridUnload');
            $("#tbForm8DistrictLevelReport").jqGrid('GridUnload');
            $("#tbForm8BlockLevelReport").jqGrid('GridUnload');
            $("#tbForm8FinalLevelReport").jqGrid('GridUnload');
            loadForm8StateLevelReportGrid(proposalType, year, batch, collaboration);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
            // $("#tbForm8DistrictLevelReport").jqGrid('GridUnload');
            $("#tbForm8BlockLevelReport").jqGrid('GridUnload');
            $("#tbForm8FinalLevelReport").jqGrid('GridUnload');
            loadForm8DistrictLevelReportGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), proposalType, year, batch, collaboration);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
            //  $("#tbForm8BlockLevelReport").jqGrid('GridUnload');
            $("#tbForm8FinalLevelReport").jqGrid('GridUnload');
            loadForm8BlockLevelReportGrid($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), proposalType, year, batch, collaboration);
        }
    });

 
    $("#btnViewForm8Details").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});



function loadForm8StateLevelReportGrid(proposalType, year, batch, collaboration) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tbForm8StateLevelReport").jqGrid('GridUnload');
    $("#tbForm8DistrictLevelReport").jqGrid('GridUnload');
    $("#tbForm8BlockLevelReport").jqGrid('GridUnload');
    $("#tbForm8FinalLevelReport").jqGrid('GridUnload');

    jQuery("#tbForm8StateLevelReport").jqGrid({
        url: '/FormReports/Form8StateLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "Proposals", "Award", "Completed Proposals", "Length Completed", "Payment Proposals", "Physical Length", "Expenditure"],
        colModel: [
                        { name: 'StateName', index: 'StateName', width: 200, sortable: true, align: "left" },
                        { name: 'TProposal', index: 'TProposal', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TAward', index: 'TAward', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TCompProposal', index: 'TCompProposal', width: 170, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TLengthComp', index: 'TLengthComp', width: 170, sortable: false, align: "right", search: false ,formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TPaymentProposal', index: 'TPaymentProposal', width: 170, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TPhyLength', index: 'TPhyLength', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TExp', index: 'TExp', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "proposalType": proposalType, "batch": batch, "year": year, "collaboration": collaboration },
        pager: jQuery('#dvForm8StateLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;All States",
        autowidth: true,
        sortname: 'StateName',
        height:520,
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {

            var totalTProposal = $(this).jqGrid('getCol', 'TProposal', false, 'sum');
            var totalTAward = $(this).jqGrid('getCol', 'TAward', false, 'sum');           
            var totalTCompProposal = $(this).jqGrid('getCol', 'TCompProposal', false, 'sum');
            var totalTLengthComp = $(this).jqGrid('getCol', 'TLengthComp', false, 'sum');
            totalTLengthComp = parseFloat(totalTLengthComp).toFixed(3);
            var totalTPaymentProposal = $(this).jqGrid('getCol', 'TPaymentProposal', false, 'sum');
            totalTPaymentProposal = parseFloat(totalTPaymentProposal).toFixed(2);
            var totalTPhyLength = $(this).jqGrid('getCol', 'TPhyLength', false, 'sum');
            totalTPhyLength = parseFloat(totalTPhyLength).toFixed(3);

            var totalTExp = $(this).jqGrid('getCol', 'TExp', false, 'sum');
            totalTExp = parseFloat(totalTExp).toFixed(2);

            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { TProposal: totalTProposal });
            $(this).jqGrid('footerData', 'set', { TAward: totalTAward });
            $(this).jqGrid('footerData', 'set', { TCompProposal: totalTCompProposal });
            $(this).jqGrid('footerData', 'set', { TLengthComp: totalTLengthComp });
            $(this).jqGrid('footerData', 'set', { TPaymentProposal: totalTPaymentProposal });
            $(this).jqGrid('footerData', 'set', { TPhyLength: totalTPhyLength });
            $(this).jqGrid('footerData', 'set', { TExp: totalTExp });
            $("#dvForm8StateLevelReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Amount Rs in Lacs & Length in Kms. </font>");
            $('#tbForm8StateLevelReport_rn').html('Sr.<br/>No.');

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


    $("#tbForm8StateLevelReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'TProposal', numberOfColumns: 7, titleText: 'Total' }]
    });
}


function loadForm8DistrictLevelReportGrid(stateCode,stateName, proposalType, year, batch, collaboration) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#tbForm8StateLevelReport').jqGrid('setSelection', stateCode);
    $('#tbForm8StateLevelReport').jqGrid('setGridState', 'hidden');
    $("#tbForm8DistrictLevelReport").jqGrid('GridUnload');
    $("#tbForm8BlockLevelReport").jqGrid('GridUnload');
    $("#tbForm8FinalLevelReport").jqGrid('GridUnload');
    jQuery("#tbForm8DistrictLevelReport").jqGrid({
        url: '/FormReports/Form8DistrictLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["District", "Proposals", "Award", "Completed Proposals", "Length Completed", "Payment Proposals", "Physical Length", "Expenditure"],
        colModel: [
                        { name: 'DistrictName', index: 'DistrictName', width: 200, sortable: true, align: "left" },
                        { name: 'TProposal', index: 'TProposal', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TAward', index: 'TAward', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TCompProposal', index: 'TCompProposal', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TLengthComp', index: 'TLengthComp', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TPaymentProposal', index: 'TPaymentProposal', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TPhyLength', index: 'TPhyLength', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TExp', index: 'TExp', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "stateCode": stateCode, "proposalType": proposalType, "batch": batch, "year": year, "collaboration": collaboration },

        pager: jQuery('#dvForm8DistrictLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;State - " + stateName,
        //height: ($("#tblRptContents").height() - 185),
        autowidth: true,
        sortname: 'DistrictName',
        height:470,
        rownumbers: true,
        footerrow: true,

        loadComplete: function () {

            var totalTProposal = $(this).jqGrid('getCol', 'TProposal', false, 'sum');
            var totalTAward = $(this).jqGrid('getCol', 'TAward', false, 'sum');
            var totalTCompProposal = $(this).jqGrid('getCol', 'TCompProposal', false, 'sum');
            var totalTLengthComp = $(this).jqGrid('getCol', 'TLengthComp', false, 'sum');
            totalTLengthComp = parseFloat(totalTLengthComp).toFixed(3);
            var totalTPaymentProposal = $(this).jqGrid('getCol', 'TPaymentProposal', false, 'sum');
            totalTPaymentProposal = parseFloat(totalTPaymentProposal).toFixed(2);
            var totalTPhyLength = $(this).jqGrid('getCol', 'TPhyLength', false, 'sum');
            totalTPhyLength = parseFloat(totalTPhyLength).toFixed(3);

            var totalTExp = $(this).jqGrid('getCol', 'TExp', false, 'sum');
            totalTExp = parseFloat(totalTExp).toFixed(2);

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { TProposal: totalTProposal });
            $(this).jqGrid('footerData', 'set', { TAward: totalTAward });
            $(this).jqGrid('footerData', 'set', { TCompProposal: totalTCompProposal });
            $(this).jqGrid('footerData', 'set', { TLengthComp: totalTLengthComp });
            $(this).jqGrid('footerData', 'set', { TPaymentProposal: totalTPaymentProposal });
            $(this).jqGrid('footerData', 'set', { TPhyLength: totalTPhyLength });
            $(this).jqGrid('footerData', 'set', { TExp: totalTExp });

            $("#dvForm8DistrictLevelReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Amount Rs in Lacs & Length in Kms. </font>");
            $('#tbForm8DistrictLevelReport_rn').html('Sr.<br/>No.');

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


    $("#tbForm8DistrictLevelReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'TProposal', numberOfColumns: 7, titleText: 'Total' }]
    });
}


function loadForm8BlockLevelReportGrid(stateCode, districtCode,districtName, proposalType, year, batch, collaboration) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $('#tbForm8StateLevelReport').jqGrid('setSelection', stateCode);
    $('#tbForm8DistrictLevelReport').jqGrid('setSelection', districtCode);
    $('#tbForm8StateLevelReport').jqGrid('setGridState', 'hidden');
    $('#tbForm8DistrictLevelReport').jqGrid('setGridState', 'hidden');
    $("#tbForm8BlockLevelReport").jqGrid('GridUnload');
    $("#tbForm8FinalLevelReport").jqGrid('GridUnload');

    jQuery("#tbForm8BlockLevelReport").jqGrid({
        url: '/FormReports/Form8BlockLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Block", "Proposals", "Award", "Completed Proposals", "Length Completed", "Payment Proposals", "Physical Length", "Expenditure"],
        colModel: [
                        { name: 'BlockName', index: 'StateName', width: 200, sortable: true, align: "left" },
                        { name: 'TProposal', index: 'TProposal', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TAward', index: 'TAward', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TCompProposal', index: 'TCompProposal', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TLengthComp', index: 'TLengthComp', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TPaymentProposal', index: 'TPaymentProposal', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TPhyLength', index: 'TPhyLength', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TExp', index: 'TExp', width: 150, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
        ],

        postData: { "stateCode": stateCode, "districtCode": districtCode, "proposalType": proposalType, "batch": batch, "year": year, "collaboration": collaboration },
        pager: jQuery('#dvForm8BlockLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;District - " + districtName,
        //height: ($("#tblRptContents").height() - 175),
        autowidth: true,
        sortname: 'BlockName',
        height:420,
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {
            //$("#gview_tbForm1StateLevelReport > .ui-jqgrid-titlebar").hide();

            var totalTProposal = $(this).jqGrid('getCol', 'TProposal', false, 'sum');
            var totalTAward = $(this).jqGrid('getCol', 'TAward', false, 'sum');
            var totalTCompProposal = $(this).jqGrid('getCol', 'TCompProposal', false, 'sum');
            var totalTLengthComp = $(this).jqGrid('getCol', 'TLengthComp', false, 'sum');
            totalTLengthComp = parseFloat(totalTLengthComp).toFixed(3);
            var totalTPaymentProposal = $(this).jqGrid('getCol', 'TPaymentProposal', false, 'sum');
            totalTPaymentProposal = parseFloat(totalTPaymentProposal).toFixed(2);
            var totalTPhyLength = $(this).jqGrid('getCol', 'TPhyLength', false, 'sum');
            totalTPhyLength = parseFloat(totalTPhyLength).toFixed(3);

            var totalTExp = $(this).jqGrid('getCol', 'TExp', false, 'sum');
            totalTExp = parseFloat(totalTExp).toFixed(2);

            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { TProposal: totalTProposal });
            $(this).jqGrid('footerData', 'set', { TAward: totalTAward });
            $(this).jqGrid('footerData', 'set', { TCompProposal: totalTCompProposal });
            $(this).jqGrid('footerData', 'set', { TLengthComp: totalTLengthComp });
            $(this).jqGrid('footerData', 'set', { TPaymentProposal: totalTPaymentProposal });
            $(this).jqGrid('footerData', 'set', { TPhyLength: totalTPhyLength });
            $(this).jqGrid('footerData', 'set', { TExp: totalTExp });
            $("#dvForm8BlockLevelReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Amount Rs in Lacs & Length in Kms. </font>");
            $('#tbForm8BlockLevelReport_rn').html('Sr.<br/>No.');

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

    $("#tbForm8BlockLevelReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'ExecRdLength', numberOfColumns: 9, titleText: 'Execution' }]
    });

}

function loadForm8FinalLevelReportGrid(stateCode, districtCode, blockCode, blockName, proposalType, year, batch, collaboration) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


    $('#tbForm8StateLevelReport').jqGrid('setSelection', stateCode);
    $('#tbForm8DistrictLevelReport').jqGrid('setSelection', districtCode);
    $('#tbForm8BlockLevelReport').jqGrid('setSelection', blockCode);

    $('#tbForm8StateLevelReport').jqGrid('setGridState', 'hidden');
    $('#tbForm8DistrictLevelReport').jqGrid('setGridState', 'hidden');
    $('#tbForm8BlockLevelReport').jqGrid('setGridState', 'hidden');
    $("#tbForm8FinalLevelReport").jqGrid('GridUnload');

    jQuery("#tbForm8FinalLevelReport").jqGrid({
        url: '/FormReports/Form8FinalLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Block", "Plan Road", "Road Name", "Sanction Year", "Bridge Name", "Length Completed", "Is Completed", "Completion Date", "Payment Last Month", "Payment This Month", "Value Of Work Last Month", "Value Of Work This Month", "Final Payment", "Final Payment Date"],
        colModel: [
                        { name: 'BlockName', index: 'BlockName', width: 100, sortable: true, align: "left",hidden:true },
                        { name: 'CNRdName', index: 'CNRdName', width: 200, sortable: false, align: "left", search: false },
                        { name: 'RdName', index: 'RdName', width: 200, sortable: false, align: "left", search: false },
                        { name: 'SancYear', index: 'SancYear', width: 100, sortable: false, align: "center", search: false },
                        { name: 'BridgeName', index: 'BridgeName', width: 180, sortable: false, align: "left", search: false },
                        { name: 'ExecRdLength', index: 'ExecRdLength', width: 100, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'ExecIsCompleted', index: 'ExecIsCompleted', width: 100, sortable: false, align: "left", search: false },
                        { name: 'ExecCompletionDate', index: 'ExecCompletionDate', width: 110, sortable: false, align: "right", search: false },
                        { name: 'ExecPaymentLastMonth', index: 'ExecPaymentLastMonth', width: 90, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'ExecPaymentThisMonth', index: 'ExecPaymentThisMonth', width: 90, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'ExecValueOfWorkLastMonth', index: 'ExecValueOfWorkLastMonth', width: 90, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'ExecValueOfWorkThisMonth', index: 'ExecValueOfWorkThisMonth', width: 90, sortable: false, align: "right", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'ExecFinalPayment', index: 'ExecFinalPayment', width: 100, sortable: false, align: "center", search: false },
                        { name: 'ExecFinalPaymentDate', index: 'ExecFinalPaymentDate', width: 110, sortable: false, align: "right", search: false }
        ],
        postData: { "stateCode": stateCode, "districtCode": districtCode, "blockCode": blockCode, "proposalType": proposalType, "batch": batch, "year": year, "collaboration": collaboration },
        pager: jQuery('#dvForm8FinalLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Block - " + blockName,
        //height: ($("#tblRptContents").height() - 175),
        autowidth: true,
        sortname: 'BlockName',
        height: 370,
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {
            //$("#gview_tbForm1StateLevelReport > .ui-jqgrid-titlebar").hide();

            var totalExecRdLength = $(this).jqGrid('getCol', 'ExecRdLength', false, 'sum');
            totalExecRdLength = parseFloat(totalExecRdLength).toFixed(3);
            var totalExecPaymentLastMonth = $(this).jqGrid('getCol', 'ExecPaymentLastMonth', false, 'sum');
            totalExecPaymentLastMonth = parseFloat(totalExecPaymentLastMonth).toFixed(2);
            var totalExecPaymentThisMonth = $(this).jqGrid('getCol', 'ExecPaymentThisMonth', false, 'sum');
            totalExecPaymentThisMonth = parseFloat(totalExecPaymentThisMonth).toFixed(2);
            var totalExecValueOfWorkLastMonth = $(this).jqGrid('getCol', 'ExecValueOfWorkLastMonth', false, 'sum');
            totalExecValueOfWorkLastMonth = parseFloat(totalExecValueOfWorkLastMonth).toFixed(2);
            var totalExecValueOfWorkThisMonth = $(this).jqGrid('getCol', 'ExecValueOfWorkThisMonth', false, 'sum');
            totalExecValueOfWorkThisMonth = parseFloat(totalExecValueOfWorkThisMonth).toFixed(2);


            $(this).jqGrid('footerData', 'set', { CNRdName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { ExecRdLength: totalExecRdLength });
            $(this).jqGrid('footerData', 'set', { ExecPaymentLastMonth: totalExecPaymentLastMonth });
            $(this).jqGrid('footerData', 'set', { ExecPaymentThisMonth: totalExecPaymentThisMonth });
            $(this).jqGrid('footerData', 'set', { ExecValueOfWorkLastMonth: totalExecValueOfWorkLastMonth });
            $(this).jqGrid('footerData', 'set', { ExecValueOfWorkThisMonth: totalExecValueOfWorkThisMonth });
            $("#dvForm8FinalLevelReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Amount Rs in Lacs & Length in Kms. </font>");
            $('#tbForm8FinalLevelReport_rn').html('Sr.<br/>No.');

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

    $("#tbForm8FinalLevelReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'ExecRdLength', numberOfColumns: 9, titleText: 'Execution' }]
    });

}






