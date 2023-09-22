
jQuery.validator.addMethod("dpiuvalidator", function (value, element, param) {

    if (($("#LevelID").val() == 4) || ($("#LevelID").val() == 6)) {

        if ($('#ddlDPIU').val() == 0) {
            return false;
        }
        else {
            return true;
        }
    }
    else {

        return true;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("dpiuvalidator");

jQuery.validator.addMethod("statevalidator", function (value, element, param) {

    if (($("#LevelID").val() == 4) || ($("#LevelID").val() == 6)) {

        if ($('#ddlSRRDA').val() == 0) {
            return false;
        }
        else {
            return true;
        }
    }
    else {

        return true;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("statevalidator");

$(document).ready(function () {
    $.validator.unobtrusive.parse("#frmFundTransfer");
    var date = new Date();
    //var month = date.getMonth() + 1;
    var month = $("#SelectedMonth").val();

    var year = $("#SelectedYear").val();
    $("#ddlMonth option:nth(" + month + ")").attr("selected", "selected");
    $("#ddlYear").val(year);
    //$("#ddlYear option:nth(1)").attr("selected", "selected");
    //$("#ddlBalance option:nth(1)").attr("selected", "selected");

    if ($("#LevelId").val() == 5) {
        $("#trDPIUSelect").hide();
    }

    if ($("#LevelId").val() == 4) {
        $(function () {
            $("#ddlSRRDA").trigger("change");
        });


    }

    $("#spCollapseIconS").click(function () {
        $("#spCollapseIconS").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvFundTransfer").slideToggle("slow");
        //$("#dvBalance").slideToggle("slow");
    });

    $("#btnViewDetails").click(function () {

        $("#DPIUName").val($("#ddlDPIU option:selected").text());
        $("#HeadName").val($("#ddlFund option:selected").text());
        $("#MonthName").val($("#ddlMonth option:selected").text());
        $("#StateName").val($("#ddlSRRDA option:selected").text());
        $("#YearName").val($("#ddlYear option:selected").text());


        //  if (isValid() == true) {
        if ($("#frmFundTransfer").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#ddlSRRDA").attr("disabled", false);
            $.ajax({
                url: "/AccountReports/Account/FundTransferReport/",
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmFundTransfer").serialize(),
                success: function (data) {
                    $.unblockUI();
                    if (data.success == false) {
                        $("#dvError").show();
                        $("#dvError").html(data.message);
                        return false;
                    }
                    else {
                        $("#FundTransferDetails").html(data);
                        if ($("#LevelId").val() == 4) {
                            $("#ddlSRRDA").attr("disabled", true);
                        }
                    }
                },
                error: function (data) {
                    $.unblockUI();
                    $("#ddlState").attr("disabled", false);
                }
            });
            $.unblockUI();
        }

    });


    $("#ddlSRRDA").change(function () {

        // alert("Change");        
        $.blockUI({ message: '<h4><label style="font-weight:normal">loading DPIU...</label> ' });
       // var val = $("#ddlSRRDA").val() + "$" + "A";
        $.ajax({
            type: 'POST',
            // url: "/AccountReports/Account/PopulateDPIU?id=" + val,
            url: "/AccountReports/Account/PopulateDPIU?id=" + $("#ddlSRRDA").val(),
            async: false,
            success: function (data) {
                $.unblockUI();
                $("#ddlDPIU").empty();
                $.each(data, function () {
                    $("#ddlDPIU").append("<option value=" + this.Value + ">" +
                                                            this.Text + "</option>");
                });

                $.unblockUI();
            }

        });

    });

    $("#ddlMonth").change(function () {

        UpdateAccountSession($("#ddlMonth").val(), $("#ddlYear").val());

    });

    $("#ddlYear").change(function () {

        UpdateAccountSession($("#ddlMonth").val(), $("#ddlYear").val());

    });

});

function isValid() {
    if ($("#ddlMonth").val() == 0) {
        alert("Please select Month");
        return false;
    }
    else if ($("#ddlYear").val() == 0) {
        alert("Please select Year");
        return false;
    }
    else if ($("#ddlFund").val() == 0) {
        alert("Please select Head");
        return false;
    }
    else if ($("#LevelId").val() != 5) {
        if ($("#ddlSRRDA").val() == 0) {
            alert("Please select state");
            return false;
        }
        else if ($("#ddlDPIU").val() == 0) {
            alert("Please select DPIU");
            return false;
        }
    }
    return true;
}
function UpdateAccountSession(month, year) {
    $.ajax({
        url: "/AccountReports/Account/UpdateAccountSession",
        type: "GET",
        async: false,
        cache: false,
        data:
            {
                "Month": month,
                "Year": year
            },
        success: function (data) {
            return false;
        },
        error: function () { }
    });
    return false;
}



