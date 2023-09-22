var StateSrrdsPIU;

$(document).ready(function () {
    $.unblockUI()
    $.validator.unobtrusive.parse("#frmMonthlyAccount");
    $(function () {
        $("#ddlSrrdaMonthlyAccount").trigger("change");
    });

    $("#btnViewMonthlyAcount").click(function () {

        var StateSrrdsPIU;

        if ($("#rdoStateMonthlyAccount").is(":checked")) {
            StateSrrdsPIU = $("#rdoStateMonthlyAccount").val();
        } else if ($("#rdoSrrdaMonthlyAccount").is(":checked")) {
            StateSrrdsPIU = $("#rdoSrrdaMonthlyAccount").val();
        } else if ($("#rdoDpiuMonthlyAccount").is(":checked")) {
            StateSrrdsPIU = $("#rdoDpiuMonthlyAccount").val();
        }

        if ($("#frmMonthlyAccount").valid())
        {                

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

        $("#loadReport").html("");
        $("#loadReport").load("/AccountReports/Account/MonthlyAcountLayoutReport/" +
                                   $("#LevelIdMonthlyAccount").val() + "$" +
                                   StateSrrdsPIU + "$" +
                                   $("#ddlSrrdaMonthlyAccount").val() + "$" +
                                   $("#ddlDpiuMonthlyAccount").val() + "$" +
                                   $("#ddlMonthMonthlyAccount").val() + "$" +
                                   $("#ddlYearMonthlyAccount").val() + "$" +
                                   $("#ddlCreditDebitMonthlyAccount").val() + "$",
                               $.unblockUI());
    }
    });


    //STATE ddl change
    $("#ddlSrrdaMonthlyAccount").change(function () {
        var adminNdCode = 0;

        if ($("#LevelIdMonthlyAccount").val() == 5) {
            adminNdCode = $("#PIUNdCode").val();

        } else {//PIU
            adminNdCode = $("#ddlSrrdaMonthlyAccount option:selected").val();
        }

        $.ajax({
            type: 'POST',
            url: '/Account/PopulateDPIU?id=' + adminNdCode,
            async: false,
            cache: false,
            success: function (data) {
                $("#ddlDpiuMonthlyAccount").empty();
                $.each(data, function () {
                    $("#ddlDpiuMonthlyAccount").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            },
            error: function () {
                alert("Request can not be processed at this time.");
            }
        });
    });


    $("#rdoDpiuMonthlyAccount").click(function () {

        $("#ddlSrrdaMonthlyAccount").trigger("change");
        $("#ddlDpiuMonthlyAccount").show();
        $("#lblSelectDpiu").show();
    });

    $("#rdoStateMonthlyAccount").click(function () {
        $("#ddlDpiuMonthlyAccount").hide();
        $("#lblSelectDpiu").hide();
    });

    $("#rdoSrrdaMonthlyAccount").click(function () {
        $("#ddlDpiuMonthlyAccount").hide();
        $("#lblSelectDpiu").hide();
    });


});

