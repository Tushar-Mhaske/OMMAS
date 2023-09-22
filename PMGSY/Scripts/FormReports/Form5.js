/*
    * Project Id    :
    * Project Name  :   OMMAS II
    * Name          :   Form5.js
    * Description   :   Form5 Reports Details in jqGrid, accordingly populate further details as per District, Block etc.
    * Author        :   Shyam Yadav
    * Creation Date :   11/Sep/2013  
    * Depend on Files:  ReportsLayout.js, ReportsMenu.js
*/

$(document).ready(function () {

    loadLevelWiseGrid();

        $('#ddMAST_CONSTITUENCY_TYPEForm5').change(function () {

        if ($("#MAST_STATE_CODE").val() > 0) {
            if ($("#hdnLevelId").val() == 4 || $("#hdnLevelId").val() == 6) //state
            {
                loadForm5DistrictLevelReportGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), $("#ddlMAST_YearForm5").val(), $("#ddMAST_CONSTITUENCY_TYPEForm5").val(), 0);

            }
            else if ($("#hdnLevelId").val() == 5) //District
            {
                 loadForm5ConstituencyLevelReportGrid($("#ddlMAST_YearForm5").val(), $("#MAST_STATE_CODE").val(), $("#MAST_CONSTITUENCY_CODE").val(), $("#CONST_NAME").val(), $("#ddMAST_CONSTITUENCY_TYPEForm5").val());

            }
        }
    });


    $('#ddlMAST_YearForm5').change(function () {

        loadLevelWiseGrid();      

    });


    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');



});
function loadLevelWiseGrid() {
    if ($("#hdnLevelId").val() == 6) //mord
    {
        loadForm5StateLevelReportGrid($("#ddlMAST_YearForm5").val(), $("#ddMAST_CONSTITUENCY_TYPEForm5").val());
    }
    else if ($("#hdnLevelId").val() == 4) //state
    {
          loadForm5DistrictLevelReportGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), $("#ddlMAST_YearForm5").val(), $("#ddMAST_CONSTITUENCY_TYPEForm5").val(), 0);

    }
    else if ($("#hdnLevelId").val() == 5) //District
    {
       
        loadForm5ConstituencyLevelReportGrid($("#ddlMAST_YearForm5").val(), $("#MAST_STATE_CODE").val(), $("#MAST_CONSTITUENCY_CODE").val(), $("#CONST_NAME").val(), $("#ddMAST_CONSTITUENCY_TYPEForm5").val());

    }
}

function loadForm5StateLevelReportGrid(year, constType) {
    
   // alert(constType);
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tbForm5StateLevelReport").jqGrid('GridUnload');
    $("#tbForm5DistrictLevelReport").jqGrid('GridUnload');
    $("#tbForm5ConstituencyLevelReport").jqGrid('GridUnload');
    $("#tbForm5FinalLevelReport").jqGrid('GridUnload');

    jQuery("#tbForm5StateLevelReport").jqGrid({
        url: '/FormReports/Form5StateLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "MLA", "MP", "MLA", "MP", "MLA", "MP"],
        colModel: [
                        { name: 'StateName', index: 'StateName', width: 200, sortable: true, align: "left" },
                        { name: 'MLA', index: 'MLA', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'MP', index: 'MP', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'MLA_CN', index: 'MLA_CN', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'MP_CN', index: 'MP_CN', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'MLA_PR', index: 'MLA_PR', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'MP_PR', index: 'MP_PR', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        postData: { "year": year,"constType": constType },
        pager: jQuery('#dvForm5StateLevelReportPager'),
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
                var totalMLA = $(this).jqGrid('getCol', 'MLA', false, 'sum');
                var totalMP = $(this).jqGrid('getCol', 'MP', false, 'sum');
                var totalMLACN = $(this).jqGrid('getCol', 'MLA_CN', false, 'sum');
                var totalMPCN = $(this).jqGrid('getCol', 'MP_CN', false, 'sum');
                var totalMLAPR = $(this).jqGrid('getCol', 'MLA_PR', false, 'sum');
                var totalMPPR = $(this).jqGrid('getCol', 'MP_PR', false, 'sum');

                $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });

                $(this).jqGrid('footerData', 'set', { MLA: totalMLA });
                $(this).jqGrid('footerData', 'set', { MP: totalMP });
                $(this).jqGrid('footerData', 'set', { MLA_CN: totalMLACN });
                $(this).jqGrid('footerData', 'set', { MP_CN: totalMPCN });
                $(this).jqGrid('footerData', 'set', { MLA_PR: totalMLAPR });
                $(this).jqGrid('footerData', 'set', { MP_PR: totalMPPR });
            }
            $('#tbForm5StateLevelReport_rn').html('Sr.<br/>No.');
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

    $("#tbForm5StateLevelReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
            { startColumnName: 'MLA', numberOfColumns: 2, titleText: 'Proposed By' },
            { startColumnName: 'MLA_CN', numberOfColumns: 2, titleText: 'Included in Core Network ' },
            { startColumnName: 'MLA_PR', numberOfColumns: 2, titleText: 'Sanctioned' },
        ]
    });
}


function loadForm5DistrictLevelReportGrid( stateCode,stateName,year, constType, constCode) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#MAST_STATE_CODE").val(stateCode);
    $('#tbForm5StateLevelReport').jqGrid('setSelection', stateCode);
    $('#tbForm5StateLevelReport').jqGrid('setGridState', 'hidden');

    $("#tbForm5DistrictLevelReport").jqGrid('GridUnload');
    $("#tbForm5ConstituencyLevelReport").jqGrid('GridUnload');
    $("#tbForm5FinalLevelReport").jqGrid('GridUnload');

    jQuery("#tbForm5DistrictLevelReport").jqGrid({
        url: '/FormReports/Form5DistrictLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Constituency Name", "Total Proposed", "Included in Core Network", "Sanctioned"],
        colModel: [
                    { name: 'ConstName', index: 'ConstName', width: 200, sortable: true, align: "left" },
                    { name: 'PROP', index: 'PROP', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'CN', index: 'CN', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'PR', index: 'PR', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }

        ],
        postData: { "year": year, "stateCode": stateCode, "constType": constType, "constCode": constCode },
        pager: jQuery('#dvForm5DistrictLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;State - " + stateName,
        //height: ($("#tblRptContents").height() - 175),
        autowidth: true,
        sortname: 'ConstName',
        height:470,
        rownumbers: true,
        footerrow: true,

        loadComplete: function () {
            //$("#gview_tbForm1StateLevelReport > .ui-jqgrid-titlebar").hide();

            if ($("#hdnLevelId").val() != 5) //for district level, no nned to show totals
            {
                var totalProp = $(this).jqGrid('getCol', 'PROP', false, 'sum');
                var totalCN = $(this).jqGrid('getCol', 'CN', false, 'sum');
                var totalPR = $(this).jqGrid('getCol', 'PR', false, 'sum');

                $(this).jqGrid('footerData', 'set', { ConstName: '<b>Total</b>' });

                $(this).jqGrid('footerData', 'set', { PROP: totalProp });
                $(this).jqGrid('footerData', 'set', { CN: totalCN });
                $(this).jqGrid('footerData', 'set', { PR: totalPR });

            }
            $('#tbForm5DistrictLevelReport_rn').html('Sr.<br/>No.');

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


    //$("#tbForm3DistrictLevelReport").jqGrid('setGroupHeaders', {
    //    useColSpanStyle: true,
    //    groupHeaders: [{ startColumnName: 'TPopOver1000', numberOfColumns: 4, titleText: 'Total Population' },
    //                   { startColumnName: 'UPopOver1000', numberOfColumns: 4, titleText: 'Unconnected Habs Population' },
    //                   { startColumnName: 'BPopOver1000', numberOfColumns: 4, titleText: 'Benefitted Habs Population' },
    //                   { startColumnName: 'PopOver1000', numberOfColumns: 4, titleText: 'Balance Population' }
    //    ]
    //});
}


function loadForm5ConstituencyLevelReportGrid(year, stateCode, constCode,constName, constType) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#tbForm5StateLevelReport').jqGrid('setSelection', stateCode);
    $('#tbForm5DistrictLevelReport').jqGrid('setSelection', constCode);
    $('#tbForm5StateLevelReport').jqGrid('setGridState', 'hidden');
    $('#tbForm5DistrictLevelReport').jqGrid('setGridState', 'hidden');
    $("#tbForm5ConstituencyLevelReport").jqGrid('GridUnload');
    $("#tbForm5FinalLevelReport").jqGrid('GridUnload');

    jQuery("#tbForm5ConstituencyLevelReport").jqGrid({
        url: '/FormReports/Form5ConstituencyLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Constituency", "Road Name", "Included In Core Network", "Reason 1", "Core Network Road", "Included In Proposals", "Reason 2", "Proposal Road"],
        colModel: [
                        { name: 'ConstName', index: 'ConstName', width: 150, sortable: true, align: "left" },
                        { name: 'RdName', index: 'RdName', width: 250, sortable: false, align: "left", search: false },
                        { name: 'IncludedInCN', index: 'IncludedInCN', width: 150, sortable: false, align: "left", search: false },
                        { name: 'Reason1', index: 'Reason1', width: 150, sortable: false, align: "left", search: false },
                        { name: 'PlanRoad', index: 'PlanRoad', width: 150, sortable: false, align: "left", search: false },
                        { name: 'IncludedInPR', index: 'IncludedInPR', width: 150, sortable: false, align: "left", search: false },
                        { name: 'Reason2', index: 'Reason2', width: 150, sortable: false, align: "left", search: false },
                        { name: 'ProposalRoad', index: 'ProposalRoad', width: 150, sortable: false, align: "left", search: false }
        ],
        postData: { "year": year, "stateCode": stateCode, "constCode": constCode, "constType": constType },
        pager: jQuery('#dvForm5ConstituencyLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Constituency - " + constName,
        //height: ($("#tblRptContents").height() - 185),
        autowidth: true,
        sortname: 'ConstName',
        height:420,
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {
            $('#tbForm5ConstituencyLevelReport_rn').html('Sr.<br/>No.');
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






