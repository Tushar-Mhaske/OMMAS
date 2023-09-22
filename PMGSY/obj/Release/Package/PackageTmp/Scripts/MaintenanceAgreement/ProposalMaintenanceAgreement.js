﻿$(document).ready(function () {

    LoadCompletedRoads();
    $("#spCollapseIconS").click(function () {

        if ($("#dvSearchParameter").is(":visible")) {

            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            //$(this).next("#dvSearchParameter").slideToggle(300);
            $("#dvSearchParameter").slideToggle(300);
        }

        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            //$(this).next("#dvSearchParameter").slideToggle(300);
            $("#dvSearchParameter").slideToggle(300);
        }
    });

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $('#btnSearch').click(function (e) {
        SearchDetails();
    });

    $('#btnSearch').trigger('click');

    $("#ddlFinancialYears").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlFinancialYears").find(":selected").val() },
                    "#ddlPackages", "/Agreement/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlFinancialYears option:selected').val() + "&blockCode=" + $('#ddlBlocks option:selected').val());



    }); //end function block change

    $("#ddlBlocks").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlBlocks").find(":selected").val() },
                    "#ddlPackages", "/Agreement/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlFinancialYears option:selected').val() + "&blockCode=" + $('#ddlBlocks option:selected').val());



    }); //end function block change

});


function FillInCascadeDropdown(map, dropdown, action) {

    //message = '<img src="/Content/images/busy.gif"/>';
    var message = '';
    message = '<h4><label style="font-weight:normal"> Loading Packages... </label></h4>';

    $(dropdown).empty();
    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
} //end FillInCascadeDropdown()

function LoadCompletedRoads() {

    jQuery("#tbProposedRoadList").jqGrid({
        url: '/MaintenanceAgreement/GetCompletedRoadList',
        datatype: "local",
        mtype: "POST",
        colNames: ['Block', 'Year', 'Batch','Package', 'Work Name', 'Road Length', 'Funding Agency', 'Sanctioned Cost', 'Maintenance Cost', 'View'],
        colModel: [
                            { name: 'Block', index: 'Block', width: 10, sortable: true, align: "center" },
                            { name: 'SanctionedYear', index: 'SanctionedYear', height: 'auto', width: 10, sortable: true, align: "center" },
                            { name: 'Batch', index: 'Batch', width: 10, sortable: true, align: "center" },
                            { name: 'Package', index: 'Package', width: 10, sortable: true, align: "center" },
                            { name: 'RoadName', index: 'RoadName', height: 'auto', width: 22, align: "left", sortable: true },
                            { name: 'RoadLength', index: 'RoadLength', height: 'auto', width: 10, sortable: true, align: "left" },
                            { name: 'Collaboration', index: 'Collaboration', width: 15, sortable: true },
                            { name: 'SanctionedCost', index: 'SanctionedCost', width: 13, sortable: false, align: "right" },
                            { name: 'MaintenanceCost', index: 'MaintenanceCost', width: 13, sortable: false, align: "right" },
                            { name: 'View', width: 7, sortable: false, resize: false, formatter: FormatColumnView, align: "center" }
                           // { name: 'a', width: 80, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#dvProposedRoadListPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Proposed Work List for Maintenance Agreement",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: true,
        sortname: 'SanctionedYear,Package,RoadName',
        sortorder: "asc",
        loadComplete: function () {

            var reccount = $('#tbProposedRoadList').getGridParam('reccount');
            if (reccount > 0) {
                $('#dvProposedRoadListPager_left').html('[<b> Note</b>: 1. All Amounts are in Lakhs. 2.All Lengths are in Kms ]');
            }


        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                // alert(xhr.responseText);
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }

    }); //end of grid
}

function SearchDetails() {

    $('#tbProposedRoadList').setGridParam({
        url: '/MaintenanceAgreement/GetCompletedRoadList',
        datatype: 'json'
    });

    $('#tbProposedRoadList').jqGrid("setGridParam", { "postData": { sanctionedYear: $('#ddlFinancialYears option:selected').val(), blockCode: $('#ddlBlocks option:selected').val(), packageID: $('#ddlPackages option:selected').val(), batch: $('#ddlBatchs option:selected').val(), collaboration: $('#ddlCollaborations option:selected').val(), upgradationType: $('#ddlUpgradations option:selected').val() } });
    $('#tbProposedRoadList').trigger("reloadGrid", [{ page: 1 }]);

}

function FormatColumnView(cellvalue, options, rowObject) {

    // return "<center><table><tr><td  style='border-color:white'><a href='#' title='View Agreement' onClick ='ViewAgreementDetails(\"" + cellvalue.toString() + "\");' >View</a></td></tr></table></center>";

    return "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-zoomin' title='View Maintenance Agreement' onClick ='ViewMaintenanceAgreementDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
}

function ViewMaintenanceAgreementDetails(parameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/MaintenanceAgreement/AddMaintenanceAgreementAgainstRoad/" + parameter,
        type: "GET",
        async: false,
        cache: false,

        success: function (data) {

            $("#dvAddMaintenanceAgreementAgainstRoad").html(data);
            $('#accordion').show('slow');
            $('#dvAddMaintenanceAgreementAgainstRoad').show('slow');

            if ($("#dvSearchProposedRoad").is(":visible")) {
                $('#dvSearchProposedRoad').hide('slow');
            }
            $('#tbProposedRoadList').jqGrid("setGridState", "hidden");
            $.unblockUI();
          

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });


}