$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmQMInspectionFrequencyMonitor'));


    $("#btnViewInspectionDetails").click(function () {
        var QmType = $("#ddlQMType").val();
        var state = $("#ddlState").val();
        var RoleID = $("#RoleID").val();
        
        if (QmType == "S" && state == "0" && RoleID == 5) {
            alert("Select State.");
        }
        else {
          
            if ($('#frmQMInspectionFrequencyMonitor').valid()) {
               
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
                $.ajax({
                    url: '/QMSSRSReports/QMSSRSReports/InspectionFrequencyMonitorReport/',
                    type: 'POST',
                    catche: false,
                    data: $("#frmQMInspectionFrequencyMonitor").serialize(),
                    async: false,
                    success: function (response) {
                        $.unblockUI();
                        $("#dvInspectionFrequencyMonitorReport").html(response);
                    },
                    error: function () {
                        $.unblockUI();
                        alert("Error ocurred");
                        return false;
                    },
                });

            }
            else {
                alert("Not Valid");
            }
        }
    });


    $("#ddlQMType").change(function () {
        var QmType = $(this).val();
        if (QmType == "I") {
            $('#ddlState option:contains("Select")').text('All');
          
        }
        else if (QmType == "S") {
            $('#ddlState option:contains("All")').text('Select');
        }
    
    });

});

