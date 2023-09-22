
//jquery.unobstrusive.parse("#frmBankAuthrization")

$(document).ready(function () {
    $.validator.unobtrusive.parse("#frmBankAuthrization");

    var date = new Date();
    //var month = date.getMonth() + 1;
    var month = $("#SelectedMonth").val();
    var year = $("#SelectedYear").val();
    $("#ddlMonth option:nth(" + month + ")").attr("selected", "selected");
    //$("#ddlYear option:nth(1)").attr("selected", "selected");
    $("#ddlYear").val(year);

    if ($("#LevelId").val() == 4) {
        $(function () {
            $("#ddlSRRDA").trigger("change");
        });
    }

    $("#ddlSRRDA").change(function () {
       
        $.blockUI({ message: '<h4><label style="font-weight:normal">loading DPIU...</label> ' });
        var val = $("#ddlSRRDA").val()+"$"+"A";
        $.ajax({
            url: '/AccountsReports/PopulateDPIU?id=' + val,
            type: 'POST',
            async: false,
            cache: false,
            success: function (data) {
                $.unblockUI();
                $("#ddlDPIU").empty();
                $.each(data, function () {
                    $("#ddlDPIU").append("<option value=" + this.Value + ">" +
                                                            this.Text + "</option>");
                });
            },
            error: function (xhr, statuscode, error) {
                $.unblockUI();
                alert("Error occured while processing your request.")
                return false;
            }
            

        });
        $.unblockUI();
    });

    if ($("#LevelId").val() == 5) {
        $(function () {
            $("#btnViewDetails").trigger("click");
        });
    }

    $("#btnViewDetails").click(function () {
        $("#MonthName").val($("#ddlMonth option:selected").text());
        $("#YearName").val($("#ddlYear option:selected").text());
        $("#SRRDAName").val($("#ddlSRRDA option:selected").text());
        $("#DPIUName").val($("#ddlDPIU option:selected").text());
        if ($("#frmBankAuthrization").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#ddlSRRDA").attr("disabled", false);
            $.ajax({
                url: '/AccountsReports/BankAuthrizationDetails',
                data: $("#frmBankAuthrization").serialize(),
                type: 'POST',
                async: false,
                cache: false,
                success: function (data) {
                    $.unblockUI();

                    if ($("#LevelId").val() == 4) {
                        $("#ddlSRRDA").attr("disabled", true);
                    }
                    $("#BankAuthrizationDetails").html(data);
                },
                error: function (data) {
                    $.unblockUI();

                    alert("Error occured while processing your request.");
                    return false;
                }

            });
            $.unblockUI();

        }

    });

    $("#ddlMonth").change(function () {

        UpdateAccountSession($("#ddlMonth").val(), $("#ddlYear").val());

    });

    $("#ddlYear").change(function () {

        UpdateAccountSession($("#ddlMonth").val(), $("#ddlYear").val());

    });


});
function UpdateAccountSession(month, year) {
    $.ajax({
        url: "/Reports/UpdateAccountSession",
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