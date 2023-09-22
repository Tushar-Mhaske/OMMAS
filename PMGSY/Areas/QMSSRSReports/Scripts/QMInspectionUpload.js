$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmQMInspectionUpload'));


    $("#btnViewInspectionDetails").click(function () {
        var QmType = $("#ddlQMType").val();
        var state = $("#ddlState").val();
        var RoleID = $("#RoleID").val();
        
        if (QmType == "S" && state == "0" && RoleID == 5) {
            alert("Select State.");
        }
        else {
            if ($('#frmQMInspectionUpload').valid()) {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
                $.ajax({
                    url: '/QMSSRSReports/QMSSRSReports/QMInspectionUploadReport/',
                    type: 'POST',
                    catche: false,
                    data: $("#frmQMInspectionUpload").serialize(),
                    async: false,
                    success: function (response) {
                        $.unblockUI();
                        $("#dvQMInspectionUploadReport").html(response);
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
       

        if (RoleID == 5 && QmType == "I") {

            $("#spanState").hide();
            $("#ddlState").val(0);
            }
            else if (QmType == "S") {
                $("#spanState").show();
            }
    



    });

});

