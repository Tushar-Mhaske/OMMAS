$.validator.unobtrusive.adapters.add('isvalidstartchainage', ['pavlength'], function (options) {
    options.rules['isvalidstartchainage'] = options.params;
    options.messages['isvalidstartchainage'] = options.message;
});

$.validator.addMethod("isvalidstartchainage", function (value, element, params) {

    var rdLength = parseFloat($("#IMS_PAV_LENGTH").val());
    var endChainage = parseFloat($("#txtToChainage").val());
    var diff = endChainage - parseFloat(value);

    if ((parseFloat(value) >= 0) && (parseFloat(value) < rdLength) && (parseFloat(value) < endChainage) && (parseFloat(diff, 3) <= parseFloat(3.000))) {
        return true;
    }
    return false;
});


$.validator.unobtrusive.adapters.add('isvalidendchainage', ['pavlength'], function (options) {
    options.rules['isvalidendchainage'] = options.params;
    options.messages['isvalidendchainage'] = options.message;
});

$.validator.addMethod("isvalidendchainage", function (value, element, params) {

    var rdLength = parseFloat($("#IMS_PAV_LENGTH").val());
    var startChainage = parseFloat($("#txtFromChainage").val());
    var diff = parseFloat(value) - startChainage;

    console.log(rdLength);
    //console.log($("#FromChainage").val());
    //console.log($("#txtFromChainage").val());
    console.log(startChainage);
    console.log(value);
    console.log(diff);

    //alert("startChainage : " + startChainage + " Diff : " + parseFloat(diff, 3));
    if ((parseFloat(value) > 0) && (parseFloat(value) <= rdLength) && (parseFloat(value) > startChainage) && (parseFloat(diff, 3) <= parseFloat(3.000))) {
        return true;
    }
    return false;
});


$(document).ready(function () {

    // #Added For Searchable Dropdown on 24-01-2023 SQC 3RD tier
    $("#ddlMonitors").chosen();

    $.validator.unobtrusive.parse($('#frmMainatenanceInspection'));

    $("#ddlStates").change(function () {
        FillInCascadeDropdown({ userType: $("#ddlStates").find(":selected").val() },
                    "#ddlDistricts", "/QualityMonitoring/PopulateDistricts?selectedState=" + $('#ddlStates option:selected').val());

        $('#ddlDistricts').append("<option selected value=0>Select District</option>");
    });


    $('#txtInspectionDate').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        maxDate: $("#MaxInspectionDate").val(),
        onSelect: function (selectedDate) {
        }
    }); 

    $("#ddlProposalType,#ddlDistricts,#ddlYears").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlProposalType").find(":selected").val() },
                    "#ddlProposals", "/QualityMonitoring/PopulateRoads?id=" + $('#ddlStates option:selected').val() + "$" + $("#ddlDistricts option:selected").val() + "$" +$("#ddlYears option:selected").val() + "$" + $("#ddlProposalType option:selected").val());

    });



    $("#btnSubmit").click(function () {

        if ($('#frmMaintenanceInspection').valid()) {
            $.ajax({
                type: 'POST',
                url: '/QualityMonitoring/MaintenanceInspection',
                async: false,
                cache: false,
                data: $('#frmMaintenanceInspection').serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert("Inspection details saved successfully.");
                        blockPage();
                        $("#div3TierAddMaintenanceInspection").load('/QualityMonitoring/MaintenanceInspection', function () {
                            $.validator.unobtrusive.parse($('#frmMaintenanceInspection'));
                            unblockPage();
                        });
                    }
                    else {
                        $("#divMaintenanceError").show();
                        $("#divMaintenanceError").html('<strong>Alert : </strong>' + data.message);
                    }
                },
                error: function () {
                    alert('Request can not be processed at this time. Please try after some time.');
                }
            });
        }
        else {
            return false;
        }
    });

    $("#ddlProposals").change(function () {

        $.ajax({

            type: 'POST',
            url: '/QualityMonitoring/GetProposalLength',
            data: { 'proposalCode': $("#ddlProposals option:selected").val() },
            async: false,
            cache: false,
            success: function (data) {
                $("#IMS_PAV_LENGTH").val(data);
                $("#txtFromChainage").val(0);
                $("#txtToChainage").val(data);
            },
            error: function () { }
        });

    });

    $("#ddlStates").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlStates").find(":selected").val() },
                    "#ddlPackages", "/QualityMonitoring/PopulatePackage?id=" + $('#ddlStates option:selected').val() + "$" + $('#ddlDistricts option:selected').val() + "$" + $('#ddlYears option:selected').val());

    });

    $("#ddlDistricts").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlDistricts").find(":selected").val() },
                    "#ddlPackages", "/QualityMonitoring/PopulatePackage?id=" + $('#ddlStates option:selected').val() + "$" + $('#ddlDistricts option:selected').val() + "$" + $('#ddlYears option:selected').val());

    });

    $("#ddlYears").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlStates").find(":selected").val() },
                    "#ddlPackages", "/QualityMonitoring/PopulatePackage?id=" + $('#ddlStates option:selected').val() + "$" + $('#ddlDistricts option:selected').val() + "$" + $('#ddlYears option:selected').val());

    });


    $("#ddlPackages").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlPackages").find(":selected").val() },
                    "#ddlProposals", "/QualityMonitoring/PopulateRoadsByPackage?id=" + $('#ddlPackages option:selected').val());

    });




});
function FillInCascadeDropdown(map, dropdown, action) {
    var message = '';

    $(dropdown).empty();

    $.post(action, map, function (data) {
        $.each(data, function () {

            if (this.Selected == true) {
                $(dropdown).append("<option selected value=" + this.Value + ">" + this.Text + "</option>");
            }
            else {
                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            }
        });

    }, "json");
}