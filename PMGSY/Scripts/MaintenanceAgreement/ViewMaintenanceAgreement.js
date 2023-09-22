/// <reference path="../jquery-1.9.1-vsdoc.js" />
$(document).ready(function () {

    var RoadCode = $('#RoadCode').val();
    var RoadLegth = $('#RoadLength').val();
    LoadPeriodicMaintenanceList(RoadCode, RoadLegth);

});

function LoadPeriodicMaintenanceList(RoadCode, RoadLegth) {

    jQuery("#tbMaintenanceList").jqGrid({
        url: '/MaintenanceAgreement/ViewPeriodicMaintenanceList?RoadCode=' + RoadCode + '&Roadlength=' + RoadLegth,
        datatype: "json",
        mtype: "POST",
        postData: { sanctionedYear: $('#ddlFinancialYears option:selected').val(), blockCode: $('#ddlBlocks option:selected').val(), packageID: $('#ddlPackages option:selected').val(), batch: $('#ddlBatchs option:selected').val(), collaboration: $('#ddlCollaborations option:selected').val(), upgradationType: $('#ddlUpgradations option:selected').val() },
        colNames: ['Year', 'Month', 'Is Performace Incentive', 'Performance Intensive Year', 'Length', 'Technology', 'Start Chainage', 'End Chainage', 'Renewal Type', 'Profile Correction Cost', 'Maintanenace Cost', 'Other Cost', 'Total Cost', 'Maintenance Completion Date', 'Edit', 'Delete'],
        colModel: [
                            { name: 'Year', index: 'Year', height: 'auto', width: 100, sortable: true, align: "center" },
                            { name: 'Month', index: 'Month', width: 100, sortable: true, align: "center" },
                            { name: 'IsInsentive', index: 'IsInsentive', width: 108, sortable: true, align: "center" },
                            { name: 'insentiveYear', index: 'insentiveYear', height: 'auto', width: 100, align: "center", sortable: false },
                            { name: 'Length', index: 'Length', height: 'auto', width: 100, sortable: true, align: "center" },
                            { name: 'Technology', index: 'Technology', width: 150, sortable: true },
                            { name: 'StartChainage', index: 'StartChainage', width: 80, sortable: false, align: "right" },
                            { name: 'EndChainage', index: 'EndChainage', width: 80, sortable: false, align: "right" },
                            { name: 'RenewalType', index: 'RenewalType', width: 100, sortable: false, resize: false, align: "center" },
                            { name: 'ProfileCost', index: 'ProfileCost', width: 100, sortable: false, resize: false, align: "center" },
                            { name: 'MaintenanceCost', index: 'MaintenanceCost', width: 100, sortable: true, align: "center" },
                            { name: 'OtherCost', index: 'OtherCost', width: 100, sortable: true, align: "center" },
                            { name: 'TotalCost', index: 'TotalCost', width: 100, sortable: true, align: "center" },
                            { name: 'CompletionDate', index: 'CompletionDate', width: 100, sortable: false, align: "center" },
                            { name: 'Edit', index: 'Edit', width: 100, sortable: false, align: "center" },
                            { name: 'Delete', index: 'Delete', width: 100, sortable: false, align: "center" },

                           // { name: 'a', width: 80, sortable: false, resize: false, formatter: FormatColumnView, align: "center", sortable: false }
        ],
        pager: jQuery('#dvMaintenancePager'),
        rowNum: 10,
        rowList: [10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Periodic Maintenance List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: true,
        sortname: 'Year,Month',
        sortorder: "asc",
        onSelectRow: function (rowid, status, e) {

        },
        loadComplete: function (data) {

            var reccount = $('#tbMaintenanceList').getGridParam('reccount');
            
            // $('#dvMaintenancePager_left').html("<input type='button' style='margin-left:40px' id='btnSubmit' title='Add periodic maintenance' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddPeriodicMaintenanceDetails();return false;' value='Add Periodic Maintenance'/>");
            $('#dvMaintenancePager_left').html(data.addbutton);

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
              //  window.location.href = "/Login/Login";
            }
            else {
                // alert(xhr.responseText);
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }

    }); //end of grid
}

function AddPeriodicMaintenanceDetails(urlparameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/MaintenanceAgreement/AddPeriodicMaintenance/"+urlparameter ,
        type: "GET",
        async: false,
        cache: false,

        success: function (data) {

            $("#dvViewMaintenanceAgreementAgainstRoad").html(data);
            $('#accordion').show('slow');
            $('#dvViewMaintenanceAgreementAgainstRoad').show('slow');

            if ($("#dvSearchProposedRoad").is(":visible")) {
                $('#dvSearchProposedRoad').hide('slow');
            }
            $('#tbMaintenanceList').jqGrid("setGridState", "hidden");

            $('.ui-datepicker-trigger').hide(); //hide calener image initially because it is conditional
            $.unblockUI();


        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });
}
function EditPeriodicMaintenanceDetails(parameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/MaintenanceAgreement/EditPeriodicMaintenance/" + parameter,
        type: "GET",
        async: false,
        cache: false,

        success: function (data) {

            $("#dvViewMaintenanceAgreementAgainstRoad").html(data);
            $('#accordion').show('slow');
            $('#dvViewMaintenanceAgreementAgainstRoad').show('slow');

            if ($("#dvSearchProposedRoad").is(":visible")) {
                $('#dvSearchProposedRoad').hide('slow');
            }
            $('#tbMaintenanceList').jqGrid("setGridState", "hidden");
            //hide calener image initially because it is conditional
          

            //for restrict date for edit
            var selectedYear = $('#ddlMaintanenaceYeaelst option:selected').val();
            var selectedmonth = $('#ddlMaintanenaceMonthlst option:selected').val();
            var selectedDate = new Date(selectedYear, parseInt(selectedmonth) - 1, 1);
            var Cur = new Date();
            var diff = new Date(Cur - selectedDate);
            var days = diff / 1000 / 60 / 60 / 24;
            $("#ddlMaintanenaceYeaelst").prop("disabled", true);
            $("#ddlMaintanenaceMonthlst").prop("disabled", true);

            $("#maintananceDate").datepicker('option', { minDate: "-" + Math.floor(days) + "D" });

            if ($('#rdoIsCompletedYes').prop('checked')) {
                $('.ui-datepicker-trigger').show('slow');
            }
            else {
              
                $('.ui-datepicker-trigger').hide();
            }
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });
}

function DeletePeriodicMaintenanceDetails(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    if (confirm('Are you sure to delete record?')) {
        $.ajax({
            url: "/MaintenanceAgreement/DeletePeriodicMaintenanceDetails/" + urlparameter,
            type: "POST",
            async: false,
            cache: false,

            success: function (data) {
                alert(data.message);

                $('#tbMaintenanceList').trigger("reloadGrid");;
                //var rowid = $("#tbProposedRoadList").jqGrid('getGridParam', 'selrow');
                //var rowdata = $("#tbProposedRoadList").jqGrid('getRowData', rowid);
                //alert(rowid);
                //console.log(rowdata.RoadName);
                //alert(rowdata["RoadName"])
                //$("tr:first").html("<td>" + rowdata.SanctionedYear + "</td>" + "<td>" + rowdata.Package + "</td>" + "<td>" + rowdata.RoadName + "<td>" + "<td>" + rowdata.CompletionDate + "</td>");
                $.unblockUI();


            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }

        });
    } else {
        $.unblockUI();
        return;
    }

}