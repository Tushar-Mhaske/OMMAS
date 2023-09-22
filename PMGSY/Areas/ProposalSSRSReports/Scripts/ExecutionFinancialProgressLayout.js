$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmExecutionFinancialProgressLayout'));

    if ($("#State").val() > 0) {

        $("#StateList_ExecFinProgressDetails").attr("disabled", "disabled");
    }
    $("#StateList_ExecFinProgressDetails").change(function () {

        $("#DistrictList_ExecFinProgressDetails").val(0);
        $("#DistrictList_ExecFinProgressDetails").empty();
        $("#BlockList_ExecFinProgressDetails").val(0);
        $("#BlockList_ExecFinProgressDetails").empty();
        $("#BlockList_ExecFinProgressDetails").append("<option value='0'>All Blocks</option>");

        if ($(this).val() > 0) {

            if ($("#DistrictList_ExecFinProgressDetails").length > 0) {
                $.ajax({
                    url: '/ProposalSSRSReports/ProposalSSRSReports/AllDistrictDetails',
                    type: 'POST',
                    data: { "StateCode": $(this).val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#DistrictList_ExecFinProgressDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }

                        if ($("#District").val() > 0) {
                            $("#DistrictList_ExecFinProgressDetails").val($("#District").val());
                            $("#DistrictList_ExecFinProgressDetails").attr("disabled", "disabled");
                            $("#DistrictList_ExecFinProgressDetails").trigger('change');

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
            $("#DistrictList_ExecFinProgressDetails").append("<option value='0'>All Districts</option>");
            $("#BlockList_ExecFinProgressDetails").empty();
            $("#BlockList_ExecFinProgressDetails").append("<option value='0'>All Blocks</option>");
        }
    });

    $("#DistrictList_ExecFinProgressDetails").change(function () {

        $("#BlockList_ExecFinProgressDetails").val(0);
        $("#BlockList_ExecFinProgressDetails").empty();


        if ($(this).val() > 0) {
            if ($("#BlockList_ExecFinProgressDetails").length > 0) {
                $.ajax({
                    url: '/ProposalSSRSReports/ProposalSSRSReports/AllBlockDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_ExecFinProgressDetails").val(), "DistrictCode": $("#DistrictList_ExecFinProgressDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#BlockList_ExecFinProgressDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        } else {
            $("#BlockList_ExecFinProgressDetails").append("<option value='0'>All Blocks</option>");
        }
    });

    $("#ExecFinProgressDetailsButton").click(function () {

        $("#State").val($("#StateList_ExecFinProgressDetails option:selected").val());
        $("#District").val($("#DistrictList_ExecFinProgressDetails option:selected").val());
        $("#Block").val($("#BlockList_ExecFinProgressDetails option:selected").val());
        $("#Year").val($("#YearList_ExecFinProgressDetails option:selected").val());
        $("#Batch").val($("#BatchList_ExecFinProgressDetails option:selected").val());
        $("#Collaboration").val($("#CollaborationList_ExecFinProgressDetails option:selected").val());
        $("#Type").val($("#TypeList_ExecFinProgressDetails option:selected").val());

        $("#StateName").val($("#StateList_ExecFinProgressDetails option:selected").text());
        $("#DistName").val($("#DistrictList_ExecFinProgressDetails option:selected").text());
        $("#BlockName").val($("#BlockList_ExecFinProgressDetails option:selected").text());
        $("#CollabName").val($("#CollaborationList_ExecFinProgressDetails option:selected").text());

        if ($('#frmExecutionFinancialProgressLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/ExecutionFinancialProgressReport/',
                type: 'POST',
                catche: false,
                data: $("#frmExecutionFinancialProgressLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    if (response.message != undefined) {
                        alert(response.message);
                    }
                    $("#dvLoadExecutionFinancialProgressReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
        else {
            alert(response.message);
        }
    });

    $("#StateList_ExecFinProgressDetails").trigger('change');
    //$("#ExecFinProgressDetailsButton").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvExecutionFinancialProgressLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});
