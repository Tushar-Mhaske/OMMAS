$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmPhaseProfile'));

    $("#ddlPhProState").change(function () {
        $("#ddlPhProBlock").empty();
        $("#ddlPhProBlock").append("<option value='0'>All Blocks</option>");


        $('#ddlPhProDistrict').empty();
        FillInCascadeDropdown({ userType: $("#ddlPhProDistrict").find(":selected").val() },
                    "#ddlPhProDistrict", "/MaintenanceSSRSReport/MaintenanceSSRSReport/PopulateDistricts?param=" + $('#ddlPhProState option:selected').val());

        $('#ddlPhProAgency').empty();
        FillInCascadeDropdown({ userType: $("#ddlPhProAgency").find(":selected").val() },
                    "#ddlPhProAgency", "/MaintenanceSSRSReport/MaintenanceSSRSReport/PopulateAgencies?param=" + $('#ddlPhProState option:selected').val());

        $('#ddlPhProCollab').empty();
        FillInCascadeDropdown({ userType: $("#ddlPhProCollab").find(":selected").val() },
                    "#ddlPhProCollab", "/MaintenanceSSRSReport/MaintenanceSSRSReport/PopulateCollaborations?param=" + $('#ddlPhProState option:selected').val());
    });

    $('#ddlPhProDistrict').change(function () {
        $('#ddlPhProBlock').empty();

        FillInCascadeDropdown({ userType: $("#ddlPhProDistrict").find(":selected").val() },
                    "#ddlPhProBlock", "/MaintenanceSSRSReport/MaintenanceSSRSReport/PopulateBlocks?param=" + $('#ddlPhProDistrict option:selected').val());

    }); //end function District change

    function FillInCascadeDropdown(map, dropdown, action) {
        var message = '';

        $(dropdown).empty();
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.post(action, map, function (data) {
            $.each(data, function () {
                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
        }, "json");
        $.unblockUI();

    } //end FillInCascadeDropdown()

    $("#btnviewPhaseProfile").click(function () {
        if ($('#frmPhaseProfile').valid()) {
            $("#divLoadReport").html("");

            $("#StateName").val($("#ddlPhProState option:selected").text());
            $("#DistrictName").val($("#ddlPhProDistrict option:selected").text());
            $("#BlockName").val($("#ddlPhProBlock option:selected").text());
            $("#CollabName").val($("#ddlPhProCollab option:selected").text());
            $("#YearName").val($("#ddlPhProYear option:selected").text());
            $("#MonthName").val($("#ddlPhProMonth option:selected").text());
            $("#AgencyName").val($("#ddlPhProAgency option:selected").text());

            //if ($("#ddlPhProState").is(":visible")) {
            //    //alert("1");
            //    $("#StateName").val($("#ddlPhProState option:selected").text());
            //}

            //if ($("#ddlPhProDistrict").is(":visible")) {

            //    //$('#DistrictList_AnaAvgLengthDetail').attr("disabled", false);
            //    $("#DistName").val($("#ddlECDistrict option:selected").text());
            //}


            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/PhaseProfileReport/',
                type: 'POST',
                catche: false,
                data: $("#frmPhaseProfile").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadReport").html(response);
                    closableNoteDiv("divCommonReport", "spnCommonReport");
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });

        }
        else {

        }
    });

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    //closableNoteDiv("divCommonReport", "spnCommonReport");
});