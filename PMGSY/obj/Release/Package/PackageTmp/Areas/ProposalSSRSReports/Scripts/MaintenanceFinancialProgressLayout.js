$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmMaintenanceFinancialProgressLayout'));

    if ($("#State").val() > 0) {

        $("#StateList_MaintenFinProgressDetails").attr("disabled", "disabled");
    }
    $("#StateList_MaintenFinProgressDetails").change(function () {

        $("#DistrictList_MaintenFinProgressDetails").val(0);
        $("#DistrictList_MaintenFinProgressDetails").empty();
        $("#BlockList_MaintenFinProgressDetails").val(0);
        $("#BlockList_MaintenFinProgressDetails").empty();
        $("#BlockList_MaintenFinProgressDetails").append("<option value='0'>All Blocks</option>");
        // $("#DistrictList").append("<option value='0'>Select District</option>");

        if ($(this).val() > 0) {

            if ($("#DistrictList_MaintenFinProgressDetails").length > 0) {
                $.ajax({
                    url: '/ProposalSSRSReports/ProposalSSRSReports/AllDistrictDetails',
                    type: 'POST',
                    data: { "StateCode": $(this).val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#DistrictList_MaintenFinProgressDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                        //$('#DistrictList_MaintenFinProgressDetails').find("option[value='0']").remove();
                        //$("#DistrictList_MaintenFinProgressDetails").append("<option value='0'>Select District</option>");
                        //$('#DistrictList_MaintenFinProgressDetails').val(0);

                        //For Disable if District Login
                        if ($("#District").val() > 0) {
                            $("#DistrictList_MaintenFinProgressDetails").val($("#District").val());
                            $("#DistrictList_MaintenFinProgressDetails").attr("disabled", "disabled");
                            $("#DistrictList_MaintenFinProgressDetails").trigger('change');
                        }


                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }
        else {

            $("#DistrictList_MaintenFinProgressDetails").append("<option value='0'>All Districts</option>");
            $("#BlockList_MaintenFinProgressDetails").empty();
            $("#BlockList_MaintenFinProgressDetails").append("<option value='0'>All Blocks</option>");

        }
    });

    $("#DistrictList_MaintenFinProgressDetails").change(function () {

        $("#BlockList_MaintenFinProgressDetails").val(0);
        $("#BlockList_MaintenFinProgressDetails").empty();

        if ($(this).val() > 0) {
            if ($("#BlockList_MaintenFinProgressDetails").length > 0) {
                $.ajax({
                    url: '/ProposalSSRSReports/ProposalSSRSReports/AllBlockDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_MaintenFinProgressDetails").val(), "DistrictCode": $("#DistrictList_MaintenFinProgressDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#BlockList_MaintenFinProgressDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }

                        //$('#BlockList_MaintenFinProgressDetails').find("option[value='0']").remove();
                        //$("#BlockList_MaintenFinProgressDetails").append("<option value='0'>Select Block</option>");
                        //$('#BlockList_MaintenFinProgressDetails').val(0);


                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        } else {
            $("#BlockList_MaintenFinProgressDetails").append("<option value='0'>All Blocks</option>");
        }
    });

    $("#MaintenFinProgressDetailsButton").click(function () {
        $("#State").val($("#StateList_MaintenFinProgressDetails option:selected").val());
        $("#District").val($("#DistrictList_MaintenFinProgressDetails option:selected").val());
        $("#Block").val($("#BlockList_MaintenFinProgressDetails option:selected").val());
        $("#Year").val($("#YearList_MaintenFinProgressDetails option:selected").val());
        $("#Batch").val($("#BatchList_MaintenFinProgressDetails option:selected").val());
        $("#Collaboration").val($("#CollaborationList_MaintenFinProgressDetails option:selected").val());
        $("#Type").val($("#TypeList_MaintenFinProgressDetails").val());

        $("#StateName").val($("#StateList_MaintenFinProgressDetails option:selected").text());
        $("#DistName").val($("#DistrictList_MaintenFinProgressDetails option:selected").text());
        $("#BlockName").val($("#BlockList_MaintenFinProgressDetails option:selected").text());
        $("#CollabName").val($("#CollaborationList_MaintenFinProgressDetails option:selected").text());

        if ($('#frmMaintenanceFinancialProgressLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/MaintenanceFinancialProgressReport/',
                type: 'POST',
                catche: false,
                data: $("#frmMaintenanceFinancialProgressLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadMaintenanceFinancialProgressReport").html(response);

                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    $("#StateList_MaintenFinProgressDetails").trigger('change');
    //$("#MaintenFinProgressDetailsButton").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvMaintenanceFinancialProgressLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");

});





