$(document).ready(function () {
    $.validator.unobtrusive.parse("#frmStateAccountMonitoringDetails");

    $("#btnView").click(function () {
        //alert("Clicked");

        $("#loadReport").html("");


        if ($("#frmStateAccountMonitoringDetails").valid()) 
        {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

            $("#ddlStateStateAccountMonitoring").attr('disabled', false);


            //Ajax Call
            $.ajax({
                url: "/AccountReports/Account/AnnexureF_Report/",
                type: "POST",
                data: $("#frmStateAccountMonitoringDetails").serialize(),
                success: function (data) {
                    $.unblockUI();

                    if (data.success == false) {
                        $("#dvLoadReport").html(data);
                        return false;
                    } else {
                        $("#dvLoadReport").html(data);
                    }
                },
                    error: function (xhr, ajaxOptions, thrownError) {
                   
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            });
        }

    });



});