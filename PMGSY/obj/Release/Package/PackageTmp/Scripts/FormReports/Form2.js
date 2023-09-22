/*
    * Project Id    :
    * Project Name  :   OMMAS II
    * Name          :   Form2.js
    * Description   :   Form2 Reports Details in jqGrid, accordingly populate further details as per District, Block etc.
    * Author        :   Shyam Yadav
    * Creation Date :   05/Sep/2013  
    * Depend on Files:  ReportsLayout.js, ReportsMenu.js
*/

$(document).ready(function () {


    loadLevelWiseGrid();
    $('#ddMAST_CONSTITUENCY_TYPE_Form2').change(function () {
       
        if ($("#MAST_STATE_CODE").val() > 0) {
            if ($("#hdnLevelId").val() == 4 || $("#hdnLevelId").val() == 6) //state
            {
              //  viewForm2DistrictLevelReport($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val());
                loadForm2DistrictLevelReportGrid($("#MAST_STATE_CODE").val(),$("#STATE_NAME").val(), $("#ddMAST_CONSTITUENCY_TYPE_Form2").val(), 0);

            }
            else if ($("#hdnLevelId").val() == 5) //District
            {
                //viewForm2ConstituencyLevelReport($("#MAST_STATE_CODE").val(), $("#MAST_CONSTITUENCY_CODE").val(), $("#CONST_NAME").val());
                loadForm2ConstituencyLevelReportGrid($("#MAST_STATE_CODE").val(), $("#MAST_CONSTITUENCY_CODE").val(), $("#CONST_NAME").val(), $("#ddMAST_CONSTITUENCY_TYPE_Form2").val());

            }
        }
    });

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');



});

function loadLevelWiseGrid() {
    if ($("#hdnLevelId").val() == 6) //mord
    {
        loadForm2StateLevelReportGrid($("#ddMAST_CONSTITUENCY_TYPE_Form2").val(), 0);
    }
    else if ($("#hdnLevelId").val() == 4) //state
    {
        loadForm2DistrictLevelReportGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), $("#ddMAST_CONSTITUENCY_TYPE_Form2").val(), 0);
    }
    else if ($("#hdnLevelId").val() == 5) //District
    {
        loadForm2ConstituencyLevelReportGrid($("#MAST_STATE_CODE").val(), $("#MAST_CONSTITUENCY_CODE").val(), $("#CONST_NAME").val(), $("#ddMAST_CONSTITUENCY_TYPE_Form2").val());
    }

}
function loadForm2StateLevelReportGrid(constType, constCode) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tbForm2StateLevelReport").jqGrid('GridUnload');
    $("#tbForm2DistrictLevelReport").jqGrid('GridUnload');
    $("#tbForm2ConstituencyLevelReport").jqGrid('GridUnload');

    jQuery("#tbForm2StateLevelReport").jqGrid({
        url: '/FormReports/Form2StateLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "Total MP Constituency", "Total MLA Constituency"],
        colModel: [
                        { name: 'StateName', index: 'StateName', width: 200, sortable: true, align: "left" },
                        { name: 'MPConst', index: 'MPConst', width: 150, sortable: false, align: "right", formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'MLAConst', index: 'MLAConst', width: 150, sortable: false, align: "right", search: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        postData: { "constType": constType, "constCode": constCode },

        pager: jQuery('#dvForm2StateLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;All States",
        //height: ($("#tblRptContents").height() - 150),
        autowidth: true,
        sortname: 'StateName',
        //rowList: [20, 30, 40],
        height:520,
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {
            //$("#gview_tbForm1StateLevelReport > .ui-jqgrid-titlebar").hide();

            var MPConstTotal = $(this).jqGrid('getCol', 'MPConst', false, 'sum');
            var MLAConstTotal = $(this).jqGrid('getCol', 'MLAConst', false, 'sum');

            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { MPConst: MPConstTotal });
            $(this).jqGrid('footerData', 'set', { MLAConst: MLAConstTotal });
            $('#tbForm2StateLevelReport_rn').html('Sr.<br/>No.');

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


function loadForm2DistrictLevelReportGrid(stateCode, stateName, constType, constCode) {
    $("#MAST_STATE_CODE").val(stateCode);
    $("#STATE_NAME").val(stateName);
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#tbForm2StateLevelReport').jqGrid('setSelection', stateCode);
    $('#tbForm2StateLevelReport').jqGrid('setGridState', 'hidden');

    $("#tbForm2DistrictLevelReport").jqGrid('GridUnload');
    $("#tbForm2ConstituencyLevelReport").jqGrid('GridUnload');

    jQuery("#tbForm2DistrictLevelReport").jqGrid({
        url: '/FormReports/Form2DistrictLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Constituency", "Total Districts", "Total Blocks"],
       // colNames: ["Constituency", "Total Districts"],
        colModel: [
                        { name: 'ConstName', index: 'ConstName', width: 200, sortable: true, align: "left" },
                        { name: 'TotalDist', index: 'TotalDist', width: 150, sortable: false, align: "right" , formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                        { name: 'TotalBlocks', index: 'TotalBlocks', width: 150, sortable: false, align: "right" , formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },                      
              
        ],
        postData: { "stateCode": stateCode, "constType": constType, "constCode": constCode },
        pager: jQuery('#dvForm2DistrictLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;State - " + stateName + " ,Constituency Type -" + $("#ddMAST_CONSTITUENCY_TYPE_Form2 option:selected").text(),
        //height: ($("#tblRptContents").height() - 150),
        autowidth: true,
        sortname: 'ConstName',
        height:470,
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {
            //$("#gview_tbForm1StateLevelReport > .ui-jqgrid-titlebar").hide();

            var distTotal = $(this).jqGrid('getCol', 'TotalDist', false, 'sum');
            var blocksTotal = $(this).jqGrid('getCol', 'TotalBlocks', false, 'sum');
        

            $(this).jqGrid('footerData', 'set', { ConstName: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { TotalDist: distTotal });
            $(this).jqGrid('footerData', 'set', { TotalBlocks: blocksTotal });
      
            $('#tbForm2DistrictLevelReport_rn').html('Sr.<br/>No.');


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


function loadForm2ConstituencyLevelReportGrid(stateCode, constCode, constName, constType) {

    $("#MAST_STATE_CODE").val(stateCode);
    $("#MAST_CONSTITUENCY_CODE").val(constCode);
    $("#CONST_NAME").val(constName);
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#tbForm2StateLevelReport').jqGrid('setSelection', stateCode);
    $('#tbForm2DistrictLevelReport').jqGrid('setSelection', constCode);
    $('#tbForm2StateLevelReport').jqGrid('setGridState', 'hidden');
    $('#tbForm2DistrictLevelReport').jqGrid('setGridState', 'hidden');
    $("#tbForm2ConstituencyLevelReport").jqGrid('GridUnload');

    jQuery("#tbForm2ConstituencyLevelReport").jqGrid({
        url: '/FormReports/Form2ConstituencyLevelListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Constituency", "District", "Block"],
        //colNames: ["District", "Block"],
        colModel: [
                        { name: 'ConstName', index: 'ConstName', width: 200, sortable: true, align: "left" },
                        { name: 'District', index: 'District', width: 150, sortable: false, align: "left" },
                        { name: 'Block', index: 'Block', width: 150, sortable: false, align: "left" }
        ],
        postData: { "stateCode": stateCode, "constCode": constCode, "constType": constType },
        pager: jQuery('#dvForm2ConstituencyLevelReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Constituency - " + constName + ", Constituency Type -" + $("#ddMAST_CONSTITUENCY_TYPE_Form2 option:selected").text(),
        //height: ($("#tblRptContents").height() - 150),
        autowidth: true,
        sortname: 'District',
        rownumbers: true,
        height:420,
        //footerrow: true,
        loadComplete: function () {
            $('#tbForm2ConstituencyLevelReport_rn').html('Sr.<br/>No.');
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









