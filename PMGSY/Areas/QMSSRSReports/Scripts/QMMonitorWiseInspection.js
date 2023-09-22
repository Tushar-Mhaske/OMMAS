$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmMonitorWiseInspection'));


    $("#btnViewInspectionDetails").click(function () {
        var QmType = $("#ddlQMType").val();
        var state = $("#ddlState").val();
        var RoleID = $("#RoleID").val();

        if (QmType == "S" && state == "0" && RoleID == 5) {
            alert("Select State.");
        }
        else {
            if ($('#frmMonitorWiseInspection').valid()) {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
                $.ajax({
                    url: '/QMSSRSReports/QMSSRSReports/QMMonitorWiseInspectionReport/',
                    type: 'POST',
                    catche: false,
                    data: $("#frmMonitorWiseInspection").serialize(),
                    async: false,
                    success: function (response) {
                        $.unblockUI();
                        $("#dvQMMonitorInspectionReport").html(response);
                    },
                    error: function () {
                        $.unblockUI();
                        alert("Error ocurred");
                        return false;
                    },
                });
            }
        }
    });


    $("#ddlQMType").change(function () {
        var QmType = $(this).val();
        var RoleID = $("#RoleID").val();
        var state = $("#ddlState").val();

        if (RoleID == 5) {
            if (QmType == "I") {

                $("#ddlState option:first").text("All")
            }
            else if (QmType == "S") {
                $("#ddlState option:first").text("Select");
            }
        }



    });

});

