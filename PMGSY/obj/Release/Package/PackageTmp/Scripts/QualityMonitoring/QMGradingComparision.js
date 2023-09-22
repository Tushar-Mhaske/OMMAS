/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMGradingComparision.js
        * Description   :   Handles events, grids in GradingComparision Report
        * Author        :   Shyam Yadav 
        * Creation Date :   02/Dec/2013
 **/
$(document).ready(function () {

    loadQMGradingComparisionGrid();

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');


    $("#btnViewGradingComparision").click(function () {

        loadQMGradingComparisionGrid();
    });


    populateDistricts();

});


function populateDistricts()
{
    $("#ddlStateGradingComparision").change(function () {

        $("#ddlDistrictGradingComparision").val(0);
        $("#ddlDistrictGradingComparision").empty();
        if ($(this).val() == 0) {
            $("#ddlDistrictGradingComparision").append("<option value='0'>All</option>");
        }

        if ($("#ddlStateGradingComparision").val() > 0) {

            if ($("#ddlDistrictGradingComparision").length > 0) {

                $.ajax({
                    url: '/QualityMonitoring/PopulateDistricts/',
                    type: 'GET',
                    data: { selectedState: $("#ddlStateGradingComparision").val() },
                    success: function (jsonData) {
                        $("#ddlDistrictGradingComparision").append("<option value='0'>All</option>");
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#ddlDistrictGradingComparision").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }
    });//stateCode Change Ends here
}

function loadQMGradingComparisionGrid() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tbGradingComparisionReport").jqGrid('GridUnload');

    jQuery("#tbGradingComparisionReport").jqGrid({
        url: '/QualityMonitoring/QMGradingComparisionListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["District", "Block", "Sanction Year", "Package", "Road Name", "Chainage", "NQM Grade", "SQM Grade", "NQM Count", "SQM Count"],
        colModel: [
                    { name: 'DistrictName', index: 'StateName', width: 100, sortable: true, align: "left" },
                    { name: 'BlockName', index: 'BlockName', width: 100, sortable: true, align: "left" },
                    { name: 'SanctionYear', index: 'SanctionYear', width: 100, sortable: false, align: "left", search: false },
                    { name: 'PackageId', index: 'PackageId', width: 100, sortable: false, align: "left", search: false },
                    { name: 'RoadName', index: 'RoadName', width: 100, sortable: false, align: "left", search: false },
                    { name: 'Chainage', index: 'Chainage', width: 100, sortable: false, align: "left", search: false },
                    { name: 'NqmGrade', index: 'NqmGrade', width: 100, sortable: false, align: "left", search: false },
                    { name: 'SqmGrade', index: 'SqmGrade', width: 100, sortable: false, align: "left", search: false },
                    { name: 'NQMCount', index: 'NQMCount', width: 100, sortable: false, align: "center", search: false },
                    { name: 'SQMCount', index: 'SQMCount', width: 100, sortable: false, align: "center", search: false }
        ],
        postData: { 'state': $("#ddlStateGradingComparision").val(), 'district': $("#ddlDistrictGradingComparision").val(), 'year': $("#ddlYearGradingComparision").val(), 'month': $("#ddlMonthGradingComparision").val() },
        pager: jQuery('#dvGradingComparisionPager'),
        rowNum: 1000,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Comparision of Grading by NQM and SQM",
        autowidth: true,
        height: 520,
        sortname: 'StateName',
        //rowList: [20, 30, 40],
        rownumbers: true,
        loadComplete: function () {

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