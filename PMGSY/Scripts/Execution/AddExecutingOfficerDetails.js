$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmAddExecutingOfficer'));

    $(":input").bind("keypress", function (e) {
        if (e.keyvalue == 13) {
            return false;
        }
    })

    //save the EO details
    $("#btnAddExecutingOfficerDetails").click(function () {

        var curDate = new Date();
        var month = curDate.getMonth();
        month = parseInt(month) + 1;
        var year = curDate.getFullYear();
        if ($("#ddlYear").val() > year) {
            alert('Year should not be greater than current year.');
            return false;
        }

        if ($("#ddlMonth").val() > month && $("#ddlYear").val() == year) {

            alert('Month and Year exceeds the current date.');
            return false;
        }

        if ($("#frmAddExecutingOfficer").valid()) {
            $.ajax({
                type: 'POST',
                url: '/Execution/AddExecutingOfficerDetails/',
                data: $("#frmAddExecutingOfficer").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success) {
                        alert(data.message);
                        $("#tbExecutingOfficerList").trigger('reloadGrid');
                        $("#btnResetExecutingOfficerDetails").trigger('click');
                        $("#divExecutingOfficer").html('');
                    }
                    else {
                        $("#divError").show("slow");
                        $("#divError").html(data.message);
                    }
                },
                error: function () {
                    alert("Request can not be processed at this time.");
                }
            })
        }
    });

    //update EO details button click
    $("#btnUpdateExecutingOfficerDetails").click(function () {

        var curDate = new Date();
        var month = curDate.getMonth();
        month = parseInt(month) + 1;
        var year = curDate.getFullYear();
        if ($("#ddlYear").val() > year) {
            alert('Year should not be greater than current year.');
            return false;
        }

        if ($("#ddlMonth").val() > month && $("#ddlYear").val() == year) {

            alert('Month and Year exceeds the current date.');
            return false;
        }

        if ($("#frmAddExecutingOfficer").valid()) {
            $.ajax({
                type: 'POST',
                url: '/Execution/EditExecutingOfficerDetails/',
                data: $("#frmAddExecutingOfficer").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success) {
                        alert(data.message);
                        $("#tbExecutingOfficerList").trigger('reloadGrid');
                        $("#divExecutingOfficer").html('');
                    }
                    else {
                        $("#divError").show("slow");
                        $("#divError").html(data.message);
                    }
                },
                error: function () {
                    alert("Request can not be processed at this time.");
                }
            })
        }
    });


    $("#btnAddCancelExecutingOfficerDetails").click(function () {
        $("#divExecutingOfficer").html('');
    });

    $("#btnEditCancelExecutingOfficerDetails").click(function () {
        var IMS_ROAD_CODE = $("#prRoadCode").val();
        $("#divExecutingOfficer").load("/Execution/AddExecutingOfficerDetails/" + IMS_ROAD_CODE, function () {
            $.validator.unobtrusive.parse($('#divExecutingOfficer'));
        });
        $("#divExecutingOfficer").show();
    });

    $("#ddlDesignation").change(
        function () {
            var code = $("#IMS_PR_ROAD_CODE").val() + "," + $('#ddlDesignation option:selected').val();

            FillInExecutingOfificerDropdown({ userType: $("#ddlDesignation").find(":selected").val() },
                     "#ddlExecutingOfficer", "/Execution/GetExecutingOfficerByDesig?imsPrRoadCode_DesignationCode=" + code);

        }
    );
});


function FillInExecutingOfificerDropdown(map, dropdown, action) {
    var message = '';

    if (dropdown == '#ddlExecutingOfficer') {
        message = '<h4><label style="font-weight:normal"> Loading Executing Officer... </label></h4>';
    }
    $(dropdown).empty();

    $(dropdown).append("<option value=0>Select Executing Officer </option>")

    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });

        $("#ddlExecutingOfficer").val($("#ExecutingOfficerId").val());
    }, "json");


    $.unblockUI();
}

function CloseExecutingOfficerForm()
{
    //$("#divExecutingOfficer").html('');
    $("#divExecutingOfficer").hide();
}