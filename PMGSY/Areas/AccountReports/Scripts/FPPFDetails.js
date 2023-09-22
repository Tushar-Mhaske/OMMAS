$(document).ready(function () {
    $.validator.unobtrusive.parse("#frmStateAccountMonitoringDetails");

    $("#btnView").click(function () {
        //alert("Clicked");

        $("#loadReport").html("");

        //$("#loadReport").load("/AccountArea/Account/StateAccountMonitoringReport/" +
        //                           $("#ddlStateStateAccountMonitoring").val() + "$" +
        //                           $("#ddlAgencyStateAccountMonitoring").val() + "$" +
        //                           $("#ddlFundTypeStateAccountMonitoring").val() + "$" +
        //                          "S" + "$" +
        //                          4                                    
        //                      , $.unblockUI());







        //public string DisplayStateName { get; set; }
        //public string DisplayAgencyName { get; set; }
        //public string FundingAgencyName { get; set; }
        //public string PeriodicName { get; set; }





        if ($("#frmStateAccountMonitoringDetails").valid()) 
        {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

            $("#ddlStateStateAccountMonitoring").attr('disabled', false);

            //$("#AgencyName").val($("#ddlAgencyStateAccountMonitoring option:selected").text());

            $("#Year").val($("#Year option:selected").text());


            //Ajax Call
            $.ajax({
                url: "/AccountReports/Account/FPPReport/",
                type: "POST",
                data: $("#frmStateAccountMonitoringDetails").serialize(),
                success: function (data) {

                    if ($("#LevelCode").val() != 1) {
                       // $("#ddlStateStateAccountMonitoring").attr('disabled', true);
                    }

                    $.unblockUI();

                    if (data.success == false) {
                        $("#dvLoadReport").html(data);
                        return false;
                    } else {
                        $("#dvLoadReport").html(data);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    if ($("#LevelCode").val() != 1) {
                       // $("#ddlStateStateAccountMonitoring").attr('disabled', true);
                    }
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            });
        }

    });




    ////Populate Agency
    $("#ddlStateStateAccountMonitoring").change(function () {

        $.blockUI({ message: '<h4><label style-"font-weight:normal">Loadding Agency...</label></h4>' });

        $.ajax({
            url: '/Account/PopulateAgencyUsingStateCode/' + $("#ddlStateStateAccountMonitoring option:selected").val(),
            type: 'POST',
            catche: false,
            async: false,
            error: function (xhr, status, error) {
                $.unblockUI();
                alert("An error occured while proccessing your request.");
                return false;
            },
            success: function (response) {
                $("#ddlAgencyStateAccountMonitoring").empty();
                $.each(response, function () {
                    $("#ddlAgencyStateAccountMonitoring").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
                $.unblockUI();
            }
        });
        $.unblockUI();

    });//End of populate Agency

});