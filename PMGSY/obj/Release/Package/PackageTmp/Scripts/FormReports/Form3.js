/*
    * Project Id    :
    * Project Name  :   OMMAS II
    * Name          :   Form3.js
    * Description   :   Form3 Reports Details in jqGrid, accordingly populate further details as per District, Block etc.
    * Author        :   Shyam Yadav
    * Creation Date :   10/Sep/2013  
    * Depend on Files:  ReportsLayout.js, ReportsMenu.js
*/

$(document).ready(function () {

    if ($("#hdnLevelId").val() == 6) //mord
    {
        loadForm3StateLevelReportGrid();
    }
    else if ($("#hdnLevelId").val() == 4) //state
    {
        loadForm3DistrictLevelReportGrid($("#MAST_STATE_CODE").val());
    }
    else if ($("#hdnLevelId").val() == 5) //District
    {
        loadForm3BlockLevelReportGrid($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val());
    }


    //$('#tblRptContents').bind('resize', function () {
    //    resizeJqGrid();
    //}).trigger('resize');

});



function loadForm3StateLevelReportGrid() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tbForm3StateLevelReport").jqGrid('GridUnload');
    $("#tbForm3DistrictLevelReport").jqGrid('GridUnload');
    $("#tbForm3BlockLevelReport").jqGrid('GridUnload');
    $("#tbForm3FinalLevelReport").jqGrid('GridUnload');

    jQuery("#tbForm3StateLevelReport").jqGrid({
        url: '/FormReports/Form3StateLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        // colNames: ["State", "Total Population Over 1000", "Total Population Over 500", "Total Population Over 250", "Total Population Upto 250", "Population For Unconnected Habs (Over 1000)", "Population For Unconnected Habs (Over 500)", "Population For Unconnected Habs (Over 250)", "Population For Unconnected Habs (Upto 250)", "Population For Benefitted Habs (Over 1000)", "Population For Benefitted Habs (Over 500)", "Population For Benefitted Habs (Over 250)", "Population For Benefitted Habs (Upto 250)", "Balance Population (Over 1000)", "Balance Population (Over 500)", "Balance Population (Over 250)", "Balance Population (Upto 250)"],
        colNames: ["State", "1000 +", "500 to 999", "250 to 499", "< 250", "1000 +", "500 to 999", "250 to 499", "< 250", "1000 +", "500 to 999", "250 to 499", "< 250", "1000 +", "500 to 999", "250 to 499", "< 250"],
        colModel: [
                        { name: 'StateName', index: 'StateName', width:150, sortable: true, align: "left" },
                        { name: 'TPopOver1000', index: 'TPopOver1000', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TPopOver500', index: 'TPopOver500', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TPopOver250', index: 'TPopOver250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TPopBelow250', index: 'TPopBelow250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'UPopOver1000', index: 'UPopOver1000', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'UPopOver500', index: 'UPopOver500', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'UPopOver250', index: 'UPopOver250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'UPopBelow250', index: 'UPopBelow250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BPopOver1000', index: 'BPopOver1000', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BPopOver500', index: 'BPopOver500', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BPopOver250', index: 'BPopOver250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BPopBelow250', index: 'BPopBelow250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver1000', index: 'PopOver1000', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver500', index: 'PopOver500', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver250', index: 'PopOver250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopBelow250', index: 'PopBelow250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        pager: jQuery('#dvForm3StateLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;All States",
        //height: ($("#tblRptContents").height() - 175),    
        autowidth: false,      
        width: '1120',
        shrinkToFit: false,
        sortname: 'StateName',
        //rowList: [20, 30, 40],
        rownumbers: true,
        height:550,
        footerrow: true,
        loadComplete: function () {
            //$("#gview_tbForm1StateLevelReport > .ui-jqgrid-titlebar").hide();

            if ($("#hdnLevelId").val() != 5) //for district level, no need to show totals
            {
                var totalTPopOver1000 = $(this).jqGrid('getCol', 'TPopOver1000', false, 'sum');
                var totalTPopOver500 = $(this).jqGrid('getCol', 'TPopOver500', false, 'sum');
                var totalTPopOver250 = $(this).jqGrid('getCol', 'TPopOver250', false, 'sum');
                var totalTPopBelow250 = $(this).jqGrid('getCol', 'TPopBelow250', false, 'sum');

                var totalUPopOver1000 = $(this).jqGrid('getCol', 'UPopOver1000', false, 'sum');
                var totalUPopOver500 = $(this).jqGrid('getCol', 'UPopOver500', false, 'sum');
                var totalUPopOver250 = $(this).jqGrid('getCol', 'UPopOver250', false, 'sum');
                var totalUPopBelow250 = $(this).jqGrid('getCol', 'UPopBelow250', false, 'sum');

                var totalBPopOver1000 = $(this).jqGrid('getCol', 'BPopOver1000', false, 'sum');
                var totalBPopOver500 = $(this).jqGrid('getCol', 'BPopOver500', false, 'sum');
                var totalBPopOver250 = $(this).jqGrid('getCol', 'BPopOver250', false, 'sum');
                var totalBPopBelow250 = $(this).jqGrid('getCol', 'BPopBelow250', false, 'sum');

                var totalPopOver1000 = $(this).jqGrid('getCol', 'PopOver1000', false, 'sum');
                var totalPopOver500 = $(this).jqGrid('getCol', 'PopOver500', false, 'sum');
                var totalPopOver250 = $(this).jqGrid('getCol', 'PopOver250', false, 'sum');
                var totalPopBelow250 = $(this).jqGrid('getCol', 'PopBelow250', false, 'sum');

                $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });

                $(this).jqGrid('footerData', 'set', { TPopOver1000: totalTPopOver1000 });
                $(this).jqGrid('footerData', 'set', { TPopOver500: totalTPopOver500 });
                $(this).jqGrid('footerData', 'set', { TPopOver250: totalTPopOver250 });
                $(this).jqGrid('footerData', 'set', { TPopBelow250: totalTPopBelow250 });

                $(this).jqGrid('footerData', 'set', { UPopOver1000: totalUPopOver1000 });
                $(this).jqGrid('footerData', 'set', { UPopOver500: totalUPopOver500 });
                $(this).jqGrid('footerData', 'set', { UPopOver250: totalUPopOver250 });
                $(this).jqGrid('footerData', 'set', { UPopBelow250: totalUPopBelow250 });

                $(this).jqGrid('footerData', 'set', { BPopOver1000: totalBPopOver1000 });
                $(this).jqGrid('footerData', 'set', { BPopOver500: totalBPopOver500 });
                $(this).jqGrid('footerData', 'set', { BPopOver250: totalBPopOver250 });
                $(this).jqGrid('footerData', 'set', { BPopBelow250: totalBPopBelow250 });

                $(this).jqGrid('footerData', 'set', { PopOver1000: totalPopOver1000 });
                $(this).jqGrid('footerData', 'set', { PopOver500: totalPopOver500 });
                $(this).jqGrid('footerData', 'set', { PopOver250: totalPopOver250 });
                $(this).jqGrid('footerData', 'set', { PopBelow250: totalPopBelow250 });

            }
            $('#tbForm3StateLevelReport_rn').html('Sr.<br/>No.');
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


    $("#tbForm3StateLevelReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'TPopOver1000', numberOfColumns: 4, titleText: 'Total Population' },
                       { startColumnName: 'UPopOver1000', numberOfColumns: 4, titleText: 'Unconnected Habs Population' },
                       { startColumnName: 'BPopOver1000', numberOfColumns: 4, titleText: 'Benefitted Habs Population' },
                       { startColumnName: 'PopOver1000', numberOfColumns: 4, titleText: 'Balance Population' }
        ]
    });
}


function loadForm3DistrictLevelReportGrid(stateCode) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#tbForm3StateLevelReport').jqGrid('setSelection', stateCode);
    $('#tbForm3StateLevelReport').jqGrid('setGridState', 'hidden');
    $("#tbForm3DistrictLevelReport").jqGrid('GridUnload');
    $("#tbForm3BlockLevelReport").jqGrid('GridUnload');
    $("#tbForm3FinalLevelReport").jqGrid('GridUnload');

    jQuery("#tbForm3DistrictLevelReport").jqGrid({
        url: '/FormReports/Form3DistrictLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        //colNames: ["District", "Total Population Over 1000", "Total Population Over 500", "Total Population Over 250", "Total Population Upto 250", "Population For Unconnected Habs (Over 1000)", "Population For Unconnected Habs (Over 500)", "Population For Unconnected Habs (Over 250)", "Population For Unconnected Habs (Upto 250)", "Population For Benefitted Habs (Over 1000)", "Population For Benefitted Habs (Over 500)", "Population For Benefitted Habs (Over 250)", "Population For Benefitted Habs (Upto 250)", "Balance Population (Over 1000)", "Balance Population (Over 500)", "Balance Population (Over 250)", "Balance Population (Upto 250)"],
        colNames: ["District", "1000 +", "500 to 999", "250 to 499", "< 250", "1000 +", "500 to 999", "250 to 499", "< 250", "1000 +", "500 to 999", "250 to 499", "< 250", "1000 +", "500 to 999", "250 to 499", "< 250"],
        colModel: [
                        { name: 'DistrictName', index: 'DistrictName', width: 150, sortable: true, align: "left" },
                        { name: 'TPopOver1000', index: 'TPopOver1000', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TPopOver500', index: 'TPopOver500', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TPopOver250', index: 'TPopOver250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TPopBelow250', index: 'TPopBelow250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'UPopOver1000', index: 'UPopOver1000', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'UPopOver500', index: 'UPopOver500', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'UPopOver250', index: 'UPopOver250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'UPopBelow250', index: 'UPopBelow250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BPopOver1000', index: 'BPopOver1000', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BPopOver500', index: 'BPopOver500', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BPopOver250', index: 'BPopOver250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BPopBelow250', index: 'BPopBelow250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver1000', index: 'PopOver1000', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver500', index: 'PopOver500', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver250', index: 'PopOver250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopBelow250', index: 'PopBelow250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "stateCode": stateCode },
        pager: jQuery('#dvForm3DistrictLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;State  - " + $("#STATE_NAME").val(),
        //height: ($("#tblRptContents").height() - 185),
        autowidth: false,
        width: '1120',
        shrinkToFit: false,
        sortname: 'StateName',
       // rowList: [50, 100, 150],
        rownumbers: true,
        footerrow: true,
        height:500,

        loadComplete: function () {
            //$("#gview_tbForm1StateLevelReport > .ui-jqgrid-titlebar").hide();

            if ($("#hdnLevelId").val() != 5) //for district level, no nned to show totals
            {
                var totalTPopOver1000 = $(this).jqGrid('getCol', 'TPopOver1000', false, 'sum');
                var totalTPopOver500 = $(this).jqGrid('getCol', 'TPopOver500', false, 'sum');
                var totalTPopOver250 = $(this).jqGrid('getCol', 'TPopOver250', false, 'sum');
                var totalTPopBelow250 = $(this).jqGrid('getCol', 'TPopBelow250', false, 'sum');

                var totalUPopOver1000 = $(this).jqGrid('getCol', 'UPopOver1000', false, 'sum');
                var totalUPopOver500 = $(this).jqGrid('getCol', 'UPopOver500', false, 'sum');
                var totalUPopOver250 = $(this).jqGrid('getCol', 'UPopOver250', false, 'sum');
                var totalUPopBelow250 = $(this).jqGrid('getCol', 'UPopBelow250', false, 'sum');

                var totalBPopOver1000 = $(this).jqGrid('getCol', 'BPopOver1000', false, 'sum');
                var totalBPopOver500 = $(this).jqGrid('getCol', 'BPopOver500', false, 'sum');
                var totalBPopOver250 = $(this).jqGrid('getCol', 'BPopOver250', false, 'sum');
                var totalBPopBelow250 = $(this).jqGrid('getCol', 'BPopBelow250', false, 'sum');

                var totalPopOver1000 = $(this).jqGrid('getCol', 'PopOver1000', false, 'sum');
                var totalPopOver500 = $(this).jqGrid('getCol', 'PopOver500', false, 'sum');
                var totalPopOver250 = $(this).jqGrid('getCol', 'PopOver250', false, 'sum');
                var totalPopBelow250 = $(this).jqGrid('getCol', 'PopBelow250', false, 'sum');

                $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });

                $(this).jqGrid('footerData', 'set', { TPopOver1000: totalTPopOver1000 });
                $(this).jqGrid('footerData', 'set', { TPopOver500: totalTPopOver500 });
                $(this).jqGrid('footerData', 'set', { TPopOver250: totalTPopOver250 });
                $(this).jqGrid('footerData', 'set', { TPopBelow250: totalTPopBelow250 });

                $(this).jqGrid('footerData', 'set', { UPopOver1000: totalUPopOver1000 });
                $(this).jqGrid('footerData', 'set', { UPopOver500: totalUPopOver500 });
                $(this).jqGrid('footerData', 'set', { UPopOver250: totalUPopOver250 });
                $(this).jqGrid('footerData', 'set', { UPopBelow250: totalUPopBelow250 });

                $(this).jqGrid('footerData', 'set', { BPopOver1000: totalBPopOver1000 });
                $(this).jqGrid('footerData', 'set', { BPopOver500: totalBPopOver500 });
                $(this).jqGrid('footerData', 'set', { BPopOver250: totalBPopOver250 });
                $(this).jqGrid('footerData', 'set', { BPopBelow250: totalBPopBelow250 });

                $(this).jqGrid('footerData', 'set', { PopOver1000: totalPopOver1000 });
                $(this).jqGrid('footerData', 'set', { PopOver500: totalPopOver500 });
                $(this).jqGrid('footerData', 'set', { PopOver250: totalPopOver250 });
                $(this).jqGrid('footerData', 'set', { PopBelow250: totalPopBelow250 });
            }
            $('#tbForm3DistrictLevelReport_rn').html('Sr.<br/>No.');

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


    $("#tbForm3DistrictLevelReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'TPopOver1000', numberOfColumns: 4, titleText: 'Total Population' },
                       { startColumnName: 'UPopOver1000', numberOfColumns: 4, titleText: 'Unconnected Habs Population' },
                       { startColumnName: 'BPopOver1000', numberOfColumns: 4, titleText: 'Benefitted Habs Population' },
                       { startColumnName: 'PopOver1000', numberOfColumns: 4, titleText: 'Balance Population' }
        ]
    });
}


function loadForm3BlockLevelReportGrid(stateCode, districtCode) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


    $('#tbForm3StateLevelReport').jqGrid('setSelection', stateCode);
    $('#tbForm3DistrictLevelReport').jqGrid('setSelection', districtCode);
    $('#tbForm3StateLevelReport').jqGrid('setGridState', 'hidden');
    $('#tbForm3DistrictLevelReport').jqGrid('setGridState', 'hidden');
    $("#tbForm3BlockLevelReport").jqGrid('GridUnload');
    $("#tbForm3FinalLevelReport").jqGrid('GridUnload');

    jQuery("#tbForm3BlockLevelReport").jqGrid({
        url: '/FormReports/Form3BlockLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        //colNames: ["Block", "Total Population Over 1000", "Total Population Over 500", "Total Population Over 250", "Total Population Upto 250", "Population For Unconnected Habs (Over 1000)", "Population For Unconnected Habs (Over 500)", "Population For Unconnected Habs (Over 250)", "Population For Unconnected Habs (Upto 250)", "Population For Benefitted Habs (Over 1000)", "Population For Benefitted Habs (Over 500)", "Population For Benefitted Habs (Over 250)", "Population For Benefitted Habs (Upto 250)", "Balance Population (Over 1000)", "Balance Population (Over 500)", "Balance Population (Over 250)", "Balance Population (Upto 250)"],
        colNames: ["Block", "1000 +", "500 to 999", "250 to 499", "< 250", "1000 +", "500 to 999", "250 to 499", "< 250", "1000 +", "500 to 999", "250 to 499", "< 250", "1000 +", "500 to 999", "250 to 499", "< 250"],
        colModel: [
                        { name: 'BlockName', index: 'BlockName', width: 150, sortable: true, align: "left" },
                        { name: 'TPopOver1000', index: 'TPopOver1000', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TPopOver500', index: 'TPopOver500', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TPopOver250', index: 'TPopOver250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TPopBelow250', index: 'TPopBelow250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'UPopOver1000', index: 'UPopOver1000', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'UPopOver500', index: 'UPopOver500', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'UPopOver250', index: 'UPopOver250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'UPopBelow250', index: 'UPopBelow250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BPopOver1000', index: 'BPopOver1000', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BPopOver500', index: 'BPopOver500', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BPopOver250', index: 'BPopOver250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'BPopBelow250', index: 'BPopBelow250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver1000', index: 'PopOver1000', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver500', index: 'PopOver500', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver250', index: 'PopOver250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopBelow250', index: 'PopBelow250', width: 100, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "stateCode": stateCode, "districtCode": districtCode },
        pager: jQuery('#dvForm3BlockLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;District - " + $("#DISTRICT_NAME").val(),
        //height: ($("#tblRptContents").height() - 175),
        autowidth: false,
        width: '1120',
        shrinkToFit: false,
        sortname: 'BlockName',
       height:450,
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {
            //$("#gview_tbForm1StateLevelReport > .ui-jqgrid-titlebar").hide();

            var totalTPopOver1000 = $(this).jqGrid('getCol', 'TPopOver1000', false, 'sum');
            var totalTPopOver500 = $(this).jqGrid('getCol', 'TPopOver500', false, 'sum');
            var totalTPopOver250 = $(this).jqGrid('getCol', 'TPopOver250', false, 'sum');
            var totalTPopBelow250 = $(this).jqGrid('getCol', 'TPopBelow250', false, 'sum');

            var totalUPopOver1000 = $(this).jqGrid('getCol', 'UPopOver1000', false, 'sum');
            var totalUPopOver500 = $(this).jqGrid('getCol', 'UPopOver500', false, 'sum');
            var totalUPopOver250 = $(this).jqGrid('getCol', 'UPopOver250', false, 'sum');
            var totalUPopBelow250 = $(this).jqGrid('getCol', 'UPopBelow250', false, 'sum');

            var totalBPopOver1000 = $(this).jqGrid('getCol', 'BPopOver1000', false, 'sum');
            var totalBPopOver500 = $(this).jqGrid('getCol', 'BPopOver500', false, 'sum');
            var totalBPopOver250 = $(this).jqGrid('getCol', 'BPopOver250', false, 'sum');
            var totalBPopBelow250 = $(this).jqGrid('getCol', 'BPopBelow250', false, 'sum');

            var totalPopOver1000 = $(this).jqGrid('getCol', 'PopOver1000', false, 'sum');
            var totalPopOver500 = $(this).jqGrid('getCol', 'PopOver500', false, 'sum');
            var totalPopOver250 = $(this).jqGrid('getCol', 'PopOver250', false, 'sum');
            var totalPopBelow250 = $(this).jqGrid('getCol', 'PopBelow250', false, 'sum');

            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { TPopOver1000: totalTPopOver1000 });
            $(this).jqGrid('footerData', 'set', { TPopOver500: totalTPopOver500 });
            $(this).jqGrid('footerData', 'set', { TPopOver250: totalTPopOver250 });
            $(this).jqGrid('footerData', 'set', { TPopBelow250: totalTPopBelow250 });

            $(this).jqGrid('footerData', 'set', { UPopOver1000: totalUPopOver1000 });
            $(this).jqGrid('footerData', 'set', { UPopOver500: totalUPopOver500 });
            $(this).jqGrid('footerData', 'set', { UPopOver250: totalUPopOver250 });
            $(this).jqGrid('footerData', 'set', { UPopBelow250: totalUPopBelow250 });

            $(this).jqGrid('footerData', 'set', { BPopOver1000: totalBPopOver1000 });
            $(this).jqGrid('footerData', 'set', { BPopOver500: totalBPopOver500 });
            $(this).jqGrid('footerData', 'set', { BPopOver250: totalBPopOver250 });
            $(this).jqGrid('footerData', 'set', { BPopBelow250: totalBPopBelow250 });

            $(this).jqGrid('footerData', 'set', { PopOver1000: totalPopOver1000 });
            $(this).jqGrid('footerData', 'set', { PopOver500: totalPopOver500 });
            $(this).jqGrid('footerData', 'set', { PopOver250: totalPopOver250 });
            $(this).jqGrid('footerData', 'set', { PopBelow250: totalPopBelow250 });
            $('#tbForm3BlockLevelReport_rn').html('Sr.<br/>No.');

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

    $("#tbForm3BlockLevelReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'TPopOver1000', numberOfColumns: 4, titleText: 'Total Population' },
                       { startColumnName: 'UPopOver1000', numberOfColumns: 4, titleText: 'Unconnected Habs Population' },
                       { startColumnName: 'BPopOver1000', numberOfColumns: 4, titleText: 'Benefitted Habs Population' },
                       { startColumnName: 'PopOver1000', numberOfColumns: 4, titleText: 'Balance Population' }
        ]
    });

}




function viewForm3DistrictLevelReport(stateCode, stateName) {
    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    //$('#dvLoadReport').html('');
    //$('#dvLoadReport').load("/FormReports/Form3DistrictLevel/" + stateCode, function (e) {
    //    $.unblockUI();
    //});

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbForm3BlockLevelReport").jqGrid('GridUnload');
    $("#STATE_NAME").val(stateName);
    loadForm3DistrictLevelReportGrid(stateCode);
    $.unblockUI();

}



function viewForm3BlockLevelReport(stateCode, districtCode, districtName) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#DISTRICT_NAME").val(districtName);
    loadForm3BlockLevelReportGrid(stateCode, districtCode);
    $.unblockUI();

}



