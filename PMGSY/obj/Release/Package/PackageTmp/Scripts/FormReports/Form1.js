/*
    * Project Id    :
    * Project Name  :   OMMAS II
    * Name          :   Form1StateLevel.js
    * Description   :   Form1 Reports Details in jqGrid, accordingly populate further details as per District, Block etc.
    * Author        :   Shyam Yadav
    * Creation Date :   28/August/2013  
    * Depend on Files:  ReportsLayout.js, ReportsMenu.js
*/

$(document).ready(function () {

    if ($("#hdnLevelId").val() == 6) //mord
    {
        loadForm1StateLevelReportGrid();
    }
    else if ($("#hdnLevelId").val() == 4) //state
    {
        loadForm1DistrictLevelReportGrid($("#MAST_STATE_CODE").val());
    }
    else if ($("#hdnLevelId").val() == 5) //District
    {
        loadForm1BlockLevelReportGrid($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val());
    }

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

});


function loadForm1StateLevelReportGrid() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tbForm1StateLevelReport").jqGrid('GridUnload');
    $("#tbForm1DistrictLevelReport").jqGrid('GridUnload');
    $("#tbForm1BlockLevelReport").jqGrid('GridUnload');
    $("#tbForm1VillageLevelReport").jqGrid('GridUnload');

    jQuery("#tbForm1StateLevelReport").jqGrid({
        url: '/FormReports/Form1StateLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "State Type", "No. of Districts", "No. of Blocks", "No. of Villages", "1000 +", "500 to 999", "250 to 499", "<250"],
        colModel: [
                        { name: 'StateName', index: 'StateName', width: 200, sortable: true, align: "left" },
                        { name: 'StateType', index: 'StateType', width: 150, sortable: true, align: "left" },
                        { name: 'NoOfDistricts', index: 'DistrictName', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'NoOfBlocks', index: 'NoOfBlocks', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'NoOfVillages', index: 'NoOfVillages', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver1000', index: 'PopOver1000', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver500', index: 'PopOver500', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver250', index: 'PopOver250', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopBelow250', index: 'PopBelow250', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        pager: jQuery('#dvForm1StateLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;All States",
        autowidth: true,
        sortname: 'StateName',
      //  rowList: [20, 30, 40],
        rownumbers: true,
        height:600,       
        footerrow: true,
        loadComplete: function () {
            //$("#gview_tbForm1StateLevelReport > .ui-jqgrid-titlebar").hide();

            if ($("#hdnLevelId").val() != 5) //for district level, no nned to show totals
            {
                var NoOfDistrictsTotal = $(this).jqGrid('getCol', 'NoOfDistricts', false, 'sum');
                var NoOfBlocksTotal = $(this).jqGrid('getCol', 'NoOfBlocks', false, 'sum');
                var NoOfVillagesTotal = $(this).jqGrid('getCol', 'NoOfVillages', false, 'sum');

                var PopOver1000Total = $(this).jqGrid('getCol', 'PopOver1000', false, 'sum');
                var PopOver500Total = $(this).jqGrid('getCol', 'PopOver500', false, 'sum');
                var PopOver250Total = $(this).jqGrid('getCol', 'PopOver250', false, 'sum');
                var PopBelow250Total = $(this).jqGrid('getCol', 'PopBelow250', false, 'sum');

                $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });

                $(this).jqGrid('footerData', 'set', { NoOfDistricts: NoOfDistrictsTotal });
                $(this).jqGrid('footerData', 'set', { NoOfBlocks: NoOfBlocksTotal });
                $(this).jqGrid('footerData', 'set', { NoOfVillages: NoOfVillagesTotal });

                $(this).jqGrid('footerData', 'set', { PopOver1000: PopOver1000Total });
                $(this).jqGrid('footerData', 'set', { PopOver500: PopOver500Total });
                $(this).jqGrid('footerData', 'set', { PopOver250: PopOver250Total });
                $(this).jqGrid('footerData', 'set', { PopBelow250: PopBelow250Total });
            }
            $('#tbForm1StateLevelReport_rn').html('Sr.<br/>No.');

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


    $("#tbForm1StateLevelReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'PopOver1000', numberOfColumns: 4, titleText: 'Population' }
        ]
    });
}



function loadForm1DistrictLevelReportGrid(stateCode) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#tbForm1StateLevelReport').jqGrid('setSelection', stateCode);
    $('#tbForm1StateLevelReport').jqGrid('setGridState', 'hidden');

    $("#tbForm1DistrictLevelReport").jqGrid('GridUnload');
    $("#tbForm1BlockLevelReport").jqGrid('GridUnload');
    $("#tbForm1VillageLevelReport").jqGrid('GridUnload');
    jQuery("#tbForm1DistrictLevelReport").jqGrid({
        url: '/FormReports/Form1DistrictLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["District", "Is IAP", "No. of Blocks", "No. of Villages", "1000 +", "500 to 999", "250 to 499", "<250"],
        colModel: [
                        { name: 'DistrictName', index: 'DistrictName', width: 200, sortable: true, align: "left" },
                        { name: 'IAPDistrict', index: 'IAPDistrict', width: 150, sortable: false, align: "center", search: false },
                        { name: 'NoOfBlocks', index: 'NoOfBlocks', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'NoOfVillages', index: 'NoOfVillages', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver1000', index: 'PopOver1000', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver500', index: 'PopOver500', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver250', index: 'PopOver250', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopBelow250', index: 'PopBelow250', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        postData: { "stateCode": stateCode },
        pager: jQuery('#dvForm1DistrictLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;State - " + $("#STATE_NAME").val(),
        //height: ($("#tblRptContents").height() - 150),
        autowidth: true,
        sortname: 'StateName',
        // rowList: [20, 30, 40],
        height:550,
        rownumbers: true,
        footerrow: true,

        loadComplete: function () {
            //$("#gview_tbForm1StateLevelReport > .ui-jqgrid-titlebar").hide();

            if ($("#hdnLevelId").val() != 5) //for district level, no nned to show totals
            {
                var NoOfBlocksTotal = $(this).jqGrid('getCol', 'NoOfBlocks', false, 'sum');
                var NoOfVillagesTotal = $(this).jqGrid('getCol', 'NoOfVillages', false, 'sum');

                var PopOver1000Total = $(this).jqGrid('getCol', 'PopOver1000', false, 'sum');
                var PopOver500Total = $(this).jqGrid('getCol', 'PopOver500', false, 'sum');
                var PopOver250Total = $(this).jqGrid('getCol', 'PopOver250', false, 'sum');
                var PopBelow250Total = $(this).jqGrid('getCol', 'PopBelow250', false, 'sum');

                $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });

                $(this).jqGrid('footerData', 'set', { NoOfBlocks: NoOfBlocksTotal });
                $(this).jqGrid('footerData', 'set', { NoOfVillages: NoOfVillagesTotal });

                $(this).jqGrid('footerData', 'set', { PopOver1000: PopOver1000Total });
                $(this).jqGrid('footerData', 'set', { PopOver500: PopOver500Total });
                $(this).jqGrid('footerData', 'set', { PopOver250: PopOver250Total });
                $(this).jqGrid('footerData', 'set', { PopBelow250: PopBelow250Total });
            }
            $('#tbForm1DistrictLevelReport_rn').html('Sr.<br/>No.');

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

    $("#tbForm1DistrictLevelReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'PopOver1000', numberOfColumns: 4, titleText: 'Population' }
        ]
    });
}



function loadForm1BlockLevelReportGrid(stateCode, districtCode) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#tbForm1DistrictLevelReport').jqGrid('setSelection', districtCode);
    $('#tbForm1StateLevelReport').jqGrid('setGridState', 'hidden');
    $('#tbForm1DistrictLevelReport').jqGrid('setGridState', 'hidden');
    $("#tbForm1VillageLevelReport").jqGrid('GridUnload');


    $("#tbForm1BlockLevelReport").jqGrid('GridUnload');

    jQuery("#tbForm1BlockLevelReport").jqGrid({
        url: '/FormReports/Form1BlockLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Block", "Is Desert / Is Tribal", "No. of Villages", "1000 +", "500 to 999", "250 to 499", "<250"],
        colModel: [
                        { name: 'BlockName', index: 'BlockName', width: 200, sortable: true, align: "left" },
                        { name: 'IsDesertIsTribal', index: 'IsTribal', width: 150, sortable: false, align: "center", search: false },
                        { name: 'NoOfVillages', index: 'NoOfVillages', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver1000', index: 'PopOver1000', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver500', index: 'PopOver500', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopOver250', index: 'PopOver250', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'PopBelow250', index: 'PopBelow250', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        postData: { "stateCode": stateCode, "districtCode": districtCode },
        pager: jQuery('#dvForm1BlockLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;District - " + $("#DISTRICT_NAME").val(),
        //height: ($("#tblRptContents").height() - 150),
        autowidth: true,
        sortname: 'BlockName',
      //  rowList: [20, 30, 40],
        rownumbers: true,
        footerrow: true,
        height:500,
        loadComplete: function () {
            //$("#gview_tbForm1StateLevelReport > .ui-jqgrid-titlebar").hide();

            var NoOfVillagesTotal = $(this).jqGrid('getCol', 'NoOfVillages', false, 'sum');
            var PopOver1000Total = $(this).jqGrid('getCol', 'PopOver1000', false, 'sum');
            var PopOver500Total = $(this).jqGrid('getCol', 'PopOver500', false, 'sum');
            var PopOver250Total = $(this).jqGrid('getCol', 'PopOver250', false, 'sum');
            var PopBelow250Total = $(this).jqGrid('getCol', 'PopBelow250', false, 'sum');

            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { NoOfVillages: NoOfVillagesTotal });
            $(this).jqGrid('footerData', 'set', { PopOver1000: PopOver1000Total });
            $(this).jqGrid('footerData', 'set', { PopOver500: PopOver500Total });
            $(this).jqGrid('footerData', 'set', { PopOver250: PopOver250Total });
            $(this).jqGrid('footerData', 'set', { PopBelow250: PopBelow250Total });
            $('#tbForm1BlockLevelReport_rn').html('Sr.<br/>No.');

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

    $("#tbForm1BlockLevelReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'PopOver1000', numberOfColumns: 4, titleText: 'Population' }
        ]
    });
}



function loadForm1VillageLevelReportGrid(stateCode, districtCode, blockCode) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#tbForm1BlockLevelReport').jqGrid('setSelection', blockCode);
    $('#tbForm1StateLevelReport').jqGrid('setGridState', 'hidden');
    $('#tbForm1DistrictLevelReport').jqGrid('setGridState', 'hidden');
    $('#tbForm1BlockLevelReport').jqGrid('setGridState', 'hidden');
    $("#tbForm1VillageLevelReport").jqGrid('GridUnload');

    jQuery("#tbForm1VillageLevelReport").jqGrid({
        url: '/FormReports/Form1VillageLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Village", "Habitation", "Is Schedule5", "Habitation Population", "Habitation Connected"],
        colModel: [
                        { name: 'VillageName', index: 'VillageName', width: 200, sortable: true, align: "left" },
                        { name: 'HabName', index: 'HabName', width: 150, sortable: false, align: "left", search: false },
                        { name: 'IsSchedule5', index: 'IsSchedule5', width: 150, sortable: false, align: "center", search: false },
                        { name: 'HabPopulation', index: 'HabPopulation', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'HabConnected', index: 'HabConnected', width: 150, sortable: false, align: "center", search: false }
        ],
        postData: { "stateCode": stateCode, "districtCode": districtCode, "blockCode": blockCode },
        pager: jQuery('#dvForm1VillageLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Block - " + $("#BLOCK_NAME").val(),
        //height: ($("#tblRptContents").height() - 150),
        autowidth: true,
        sortname: 'VillageName',
       // rowList: [20, 30, 40],
        rownumbers: true,
        footerrow: true,
        height:480,
        loadComplete: function () {
            //$("#gview_tbForm1StateLevelReport > .ui-jqgrid-titlebar").hide();

            var TotalHabPopulation = $(this).jqGrid('getCol', 'HabPopulation', false, 'sum');

            $(this).jqGrid('footerData', 'set', { IsSchedule5: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { HabPopulation: TotalHabPopulation });

            $('#tbForm1VillageLevelReport_rn').html('Sr.<br/>No.');
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




function viewForm1DistrictLevelReport(stateCode, stateName) {
    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    //$('#dvLoadReport').html('');
    //$('#dvLoadReport').load("/FormReports/Form1DistrictLevel/" + stateCode, function (e) {
    //    $.unblockUI();
    //});


    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbForm1BlockLevelReport").jqGrid('GridUnload');
    $("#tbForm1VillageLevelReport").jqGrid('GridUnload');

    $("#STATE_NAME").val(stateName);

    loadForm1DistrictLevelReportGrid(stateCode);
    $.unblockUI();

}



function viewForm1BlockLevelReport(stateCode, districtCode, districtName) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbForm1VillageLevelReport").jqGrid('GridUnload');

    $("#DISTRICT_NAME").val(districtName);

    loadForm1BlockLevelReportGrid(stateCode, districtCode);
    $.unblockUI();

}



function viewForm1VillageLevelReport(stateCode, districtCode, blockCode, blockName) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#BLOCK_NAME").val(blockName);
    loadForm1VillageLevelReportGrid(stateCode, districtCode, blockCode);
    $.unblockUI();
}