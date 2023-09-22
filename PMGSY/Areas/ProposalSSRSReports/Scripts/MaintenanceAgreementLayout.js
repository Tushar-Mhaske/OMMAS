$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmMaintenanceAgreementLayout'));

    if ($("#State").val() > 0) {

        $("#StateList_MaintenAgreementDetails").attr("disabled", "disabled");
    }
    $("#StateList_MaintenAgreementDetails").change(function () {

        $("#DistrictList_MaintenAgreementDetails").val(0);
        $("#DistrictList_MaintenAgreementDetails").empty();
        $("#BlockList_MaintenAgreementDetails").val(0);
        $("#BlockList_MaintenAgreementDetails").empty();
        $("#BlockList_MaintenAgreementDetails").append("<option value='0'>All Blocks</option>");
        // $("#DistrictList").append("<option value='0'>Select District</option>");

        if ($(this).val() > 0) {

            if ($("#DistrictList_MaintenAgreementDetails").length > 0) {
                $.ajax({
                    url: '/ProposalReports/AllDistrictDetails',
                    type: 'POST',
                    data: { "StateCode": $(this).val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#DistrictList_MaintenAgreementDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                        //$('#DistrictList_MaintenAgreementDetails').find("option[value='0']").remove();
                        //$("#DistrictList_MaintenAgreementDetails").append("<option value='0'>Select District</option>");
                        //$('#DistrictList_MaintenAgreementDetails').val(0);

                        //For Disable if District Login
                        if ($("#District").val() > 0) {
                            $("#DistrictList_MaintenAgreementDetails").val($("#District").val());
                            $("#DistrictList_MaintenAgreementDetails").attr("disabled", "disabled");
                            $("#DistrictList_MaintenAgreementDetails").trigger('change');
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

            $("#DistrictList_MaintenAgreementDetails").append("<option value='0'>All Districts</option>");
            $("#BlockList_MaintenAgreementDetails").empty();
            $("#BlockList_MaintenAgreementDetails").append("<option value='0'>All Blocks</option>");

        }
    });

    $("#DistrictList_MaintenAgreementDetails").change(function () {

        $("#BlockList_MaintenAgreementDetails").val(0);
        $("#BlockList_MaintenAgreementDetails").empty();


        if ($(this).val() > 0) {
            if ($("#BlockList_MaintenAgreementDetails").length > 0) {
                $.ajax({
                    url: '/ProposalReports/AllBlockDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_MaintenAgreementDetails").val(), "DistrictCode": $("#DistrictList_MaintenAgreementDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#BlockList_MaintenAgreementDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }

                        //$('#BlockList_MaintenAgreementDetails').find("option[value='0']").remove();
                        //$("#BlockList_MaintenAgreementDetails").append("<option value='0'>Select Block</option>");
                        //$('#BlockList_MaintenAgreementDetails').val(0);


                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        } else {
            $("#BlockList_MaintenAgreementDetails").append("<option value='0'>All Blocks</option>");
        }
    });

    $("#MaintenAgreementDetailsButton").click(function () {
        $("#State").val($("#StateList_MaintenAgreementDetails option:selected").val());
        $("#District").val($("#DistrictList_MaintenAgreementDetails option:selected").val());
        $("#Block").val($("#BlockList_MaintenAgreementDetails option:selected").val());
        $("#Year").val($("#YearList_MaintenAgreementDetails option:selected").val());
        $("#Batch").val($("#BatchList_MaintenAgreementDetails option:selected").val());
        $("#Collaboration").val($("#CollaborationList_MaintenAgreementDetails option:selected").val());
        $("#Status").val($("#StatusList_MaintenAgreementDetails option:selected").val());


        $("#StateName").val($("#StateList_MaintenAgreementDetails option:selected").text());
        $("#DistName").val($("#DistrictList_MaintenAgreementDetails option:selected").text());
        $("#BlockName").val($("#BlockList_MaintenAgreementDetails option:selected").text());
        $("#CollabName").val($("#CollaborationList_MaintenAgreementDetails option:selected").text());
        $("#StatusName").val($("#StatusList_MaintenAgreementDetails option:selected").text());

        //if ($('#frmMaintenanceAgreementLayout').Valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/MaintenanceAgreementReport/',
                type: 'POST',
                catche: false,
                data: $("#frmMaintenanceAgreementLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadMaintenanceAgreementReport").html(response);

                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        //}
    });

    $("#StateList_MaintenAgreementDetails").trigger('change');

    //$("#MaintenAgreementDetailsButton").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvMaintenanceAgreementLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});


