$(document).ready(function () {

    $(function () {
        if ($("#LevelID").val() == 4) {
            $("#ddlState").attr("disabled", true);
            $("#ddlState").trigger("change");
        } 
    });

    $.validator.unobtrusive.parse($('#frmStateAccountMonitoringDetails'));


    $("#dvFilterAccMonitoring").click(function () {
        $("#dvFilterAccMonitoring").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvHideShowStateAccMonitoringDetails").slideToggle();

        if ($("#dvMonthlyAccBalanceSheetDetails").is(":visible")) {
            $("#dvMonthlyAccBalanceSheetDetails").hide();
        }
        else {
            $("#dvMonthlyAccBalanceSheetDetails").show();
        }

    });


    //Populate Agency
    $("#ddlState").change(function () {
        $.blockUI({ message: '<h4><label style-"font-weight:normal">Loadding Agency...</label></h4>' });
        
        $.ajax({
            url: '/AccountsReports/PopulateAgencyByStateCode/' + $("#ddlState option:selected").val(),
            type: 'POST',
            catche: false,
            async: false,
            error: function (xhr, status, error) {
                $.unblockUI();
                alert("An error occured while proccessing your request.");
                return false;
            },
            success: function (response) {
                $("#ddlAgency").empty();
                $.each(response, function () {
                    $("#ddlAgency").append("<option value="+this.Value+">"+this.Text+"</option>");
                });
                $.unblockUI();
            }
        });
        $.unblockUI();

    });//End of populate Agency

    $("#btnViewAccMonitoringDetails").click(function () {
        $("#dvMonthlyAccBalanceSheetDetails").hide();
        if ($('#frmStateAccountMonitoringDetails').valid())
        {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            
            $("#DisplayStateName").val($("#ddlState option:selected").text());
            $('#DisplayAgencyName').val($('#ddlAgency option:selected').text());
            $('#DisplayFundType').val($('#ddlFundType option:selected').text());
           
            if ($("#LevelID").val() == 4) {
                $("#ddlState").attr("disabled", false);
            }
            //disable ddlState here
            $.ajax({
                url: '/AccountsReports/StateAccountMonitoringDetails/',
                type: 'POST',
                catche: false,
                async: false,
                data: $("#frmStateAccountMonitoringDetails").serialize(),
                error: function (xhr, status, error) {
                    $.unblockUI();
                    alert("An error occured while processing your request.");
                    if ($("#LevelID").val() == 4) {
                        $("#ddlState").attr("disabled", true);
                    }
                    return false;
                },
                success: function (response) {
                    $("#dvStateAccMonitoringDetails").html(response);
                    if ($("#LevelID").val() == 4) {
                        $("#ddlState").attr("disabled", true);
                    }
                    $.unblockUI();
                }
            });
           // $.unblockUI();
        }
    });

});//end of document.ready()