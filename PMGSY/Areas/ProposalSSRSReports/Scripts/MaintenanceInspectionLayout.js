$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmMaintenanceInspectionLayout'));

    if ($("#State").val() > 0) {

        $("#StateList_MaintenInspectionDetails").attr("disabled", "disabled");
    }
    $("#StateList_MaintenInspectionDetails").change(function () {

        $("#DistrictList_MaintenInspectionDetails").val(0);
        $("#DistrictList_MaintenInspectionDetails").empty();
        $("#BlockList_MaintenInspectionDetails").val(0);
        $("#BlockList_MaintenInspectionDetails").empty();
        $("#BlockList_MaintenInspectionDetails").append("<option value='0'>All Blocks</option>");
        // $("#DistrictList").append("<option value='0'>Select District</option>");

        if ($(this).val() > 0) {

            if ($("#DistrictList_MaintenInspectionDetails").length > 0) {
                $.ajax({
                    url: '/ProposalReports/AllDistrictDetails',
                    type: 'POST',
                    data: { "StateCode": $(this).val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#DistrictList_MaintenInspectionDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                        //$('#DistrictList_MaintenInspectionDetails').find("option[value='0']").remove();
                        //$("#DistrictList_MaintenInspectionDetails").append("<option value='0'>Select District</option>");
                        //$('#DistrictList_MaintenInspectionDetails').val(0);

                        //For Disable if District Login
                        if ($("#District").val() > 0) {
                            $("#DistrictList_MaintenInspectionDetails").val($("#District").val());
                            $("#DistrictList_MaintenInspectionDetails").attr("disabled", "disabled");
                            $("#DistrictList_MaintenInspectionDetails").trigger('change');
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

            $("#DistrictList_MaintenInspectionDetails").append("<option value='0'>All Districts</option>");
            $("#BlockList_MaintenInspectionDetails").empty();
            $("#BlockList_MaintenInspectionDetails").append("<option value='0'>All Blocks</option>");

        }
    });

    $("#DistrictList_MaintenInspectionDetails").change(function () {

        $("#BlockList_MaintenInspectionDetails").val(0);
        $("#BlockList_MaintenInspectionDetails").empty();

        if ($(this).val() > 0) {
            if ($("#BlockList_MaintenInspectionDetails").length > 0) {
                $.ajax({
                    url: '/ProposalReports/AllBlockDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_MaintenInspectionDetails").val(), "DistrictCode": $("#DistrictList_MaintenInspectionDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#BlockList_MaintenInspectionDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }

                        //$('#BlockList_MaintenInspectionDetails').find("option[value='0']").remove();
                        //$("#BlockList_MaintenInspectionDetails").append("<option value='0'>Select Block</option>");
                        //$('#BlockList_MaintenInspectionDetails').val(0);


                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        } else {
            $("#BlockList_MaintenInspectionDetails").append("<option value='0'>All Blocks</option>");
        }
    });

    $("#MaintenInspectionDetailsButton").click(function () {
        $("#State").val($("#StateList_MaintenInspectionDetails option:selected").val());
        $("#District").val($("#DistrictList_MaintenInspectionDetails option:selected").val());
        $("#Block").val($("#BlockList_MaintenInspectionDetails option:selected").val());
        $("#Year").val($("#YearList_MaintenInspectionDetails option:selected").val());
        $("#Batch").val($("#BatchList_MaintenInspectionDetails option:selected").val());
        $("#Collaboration").val($("#CollaborationList_MaintenInspectionDetails option:selected").val());
        $("#Type").val($("#TypeList_MaintenInspectionDetails option:selected").val());

        $("#StateName").val($("#StateList_MaintenInspectionDetails option:selected").text());
        $("#DistName").val($("#DistrictList_MaintenInspectionDetails option:selected").text());
        $("#BlockName").val($("#BlockList_MaintenInspectionDetails option:selected").text());
        $("#CollabName").val($("#CollaborationList_MaintenInspectionDetails option:selected").text());

        if ($('#frmMaintenanceInspectionLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/MaintenanceInspectionReport/',
                type: 'POST',
                catche: false,
                data: $("#frmMaintenanceInspectionLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadMaintenanceInspectionReport").html(response);

                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });


    $("#StateList_MaintenInspectionDetails").trigger('change');
    //$("#MaintenInspectionDetailsButton").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvMaintenanceInspectionLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

