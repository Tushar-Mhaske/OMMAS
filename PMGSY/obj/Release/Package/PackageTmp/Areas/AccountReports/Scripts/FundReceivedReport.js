
//validation for month
jQuery.validator.addMethod("monthvalidator", function (value, element, param) {

    var IsMonthly = $('#rdbMonth').val();
    var month = $('#Month').val();

    if ($("#rdbMonth").is(":checked")) {
        if (IsMonthly == 2 && month == 0) {
            return false;
        }
        else {
            return true;
        }
    } else {
        return true;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("monthvalidator");



$(document).ready(function () {

    $("#spnFundReceived").click(function () {
        $("#spnLedger").toggleClass("ui-icon ui-icon-circle-triangle-n").toggleClass("ui-icon ui-icon-circle-triangle-s");
        $("#dvFilterForm").slideToggle();
    });

    $(function () {

        if (($("#rdbState").is(":checked")) || ($("#rdbSrrda").is(":checked")) || ($("#rdbPiu").is(":checked"))) {
            $("#Agency").focus();
            $("#Year").focus();
            $("#Agency").focus();

            if (($("#rdbSrrda").is(":checked"))) {
                $("#spanPIU").hide();
            }

        }
        else {
            $("#Agency").focus();
            $("#Piu").focus();
            $("#Agency").focus();
            if ($("#spanState").is(":hidden")) {
                $("#rdbSrrda").attr("checked", true);
                $("#rdbSrrda").trigger("click");
            }
            else {
                $("#rdbState").attr("checked", true);
                $("#rdbState").trigger("click");
            }
        }

        if ($("#rdbPiu").is(":checked")) {
            $("#spanPIU").show();
            $("#Agency").trigger("change");
            setTimeout(function () {
                $("#Piu").val($("#DpiuCode").val());
            }, 300);
        }
        setTimeout(function () {

            if ($("#rdbAnnual").is(":checked")) {
                $("#rdbAnnual").trigger("change");
                //      $("#btnView").trigger("click");
            }
            else if ($("#rdbMonth").is(":checked")) {
                //    $("#btnView").trigger("click");
            }
            else {
                $("#rdbMonth").attr("checked", "checked");
                $("#Year").focus();
                $("#Month").focus();
                $("#Year").focus();
            }
        }, 500);
    });
    var ndlevel = $("#LevelId").val();
    $("#Agency").change(function () {
        var ndcode = $(this).val();

        $.getJSON("/AccountReports/Account/GetDPIUOfSRRDA/", { ndcode: ndcode }, function (responseData) {
            var strOption = "";
            $("#Piu").empty();
            if (responseData.length > 1) {
                strOption += "<option value='0' selected>All DPIU</option>";
            }
            $.each(responseData, function (index, record) {

                strOption += "<option value='" + record.Value + "'>" + record.Text + "</option>";
            });

            $("#Piu").append(strOption);

        });

    });
    $(".rdbClassReportLevel").change(function () {
        var reportlevel = $(this).val();
        if (reportlevel == 2) {
            $("#spanPIU").show();
        }
        else {
            $("#spanPIU").hide();
        }

    });
    $(".rdbClassReportType").change(function () {

        var date = new Date();
        var month = date.getMonth() + 1;
        var reportType = $(this).val();

        if (reportType == 1) {

            $("#Month").val(0);
            $("#spanMonth").hide();
            $("#Year option").each(function () {
                var currentYear = $(this).val();
                if (currentYear != 0) {
                    $(this).text(currentYear + "-" + (parseInt(currentYear) + 1));
                }
            });
        }
        else {
            $("#Year option").each(function () {
                var currentYear = $(this).val();
                if (currentYear != 0) {
                    $(this).text(currentYear);
                }
            });
            $("#spanMonth").show();
        }
    });
    //added by abhishek kamble 24-dec-2013
    if (($("#rdbSrrda").is(":checked")) && ($("#spanSrrda").is(":hidden"))) {
        $("#rdbState").attr("checked", true);
    }
    $("#btnView").click(function () {

        if ($("#rdbMonth").is(":checked")) {
            if (($("#Month option:selected").val() == "0")) {
                return false;
            }
        }

        if ($("#Year option:selected").val() == "0") {
            return false;
        }
        else {


            //hide
            $("#spnFundReceived").trigger('click');
            if ($("#rdbSrrda").is(":checked") && $("#rdbAnnual").is(":checked")) {
                data = { month: 0, Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 1, allpiu: 0, srrda_dpiu: "S", SrrdaNdCode: $("#Agency option:selected").val() };
                ValidateForm(data);
            }

            if ($("#rdbSrrda").is(":checked") && $("#rdbMonth").is(":checked")) {
                data = { month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 1, allpiu: 0, srrda_dpiu: "S", SrrdaNdCode: $("#Agency option:selected").val() };
                ValidateForm(data);
            }

            if ($("#rdbPiu").is(":checked") && $("#rdbAnnual").is(":checked")) {
                if ($("#Piu option:selected").val() == 0) {
                    data = { month: 0, Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 2, allpiu: 1, srrda_dpiu: "D", SrrdaNdCode: $("#Agency option:selected").val() };
                }
                else {
                    data = { month: 0, Year: $("#Year option:selected").val(), ndcode: $("#Piu option:selected").val(), rlevel: 2, allpiu: 0, srrda_dpiu: "D", SrrdaNdCode: $("#Agency option:selected").val() };
                }
                ValidateForm(data);
            }

            if ($("#rdbPiu").is(":checked") && $("#rdbMonth").is(":checked")) {

                if ($("#Piu option:selected").val() == 0) {
                    data = { month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 2, allpiu: 1, srrda_dpiu: "D", SrrdaNdCode: $("#Agency option:selected").val() };
                }
                else {
                    data = { month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndcode: $("#Piu option:selected").val(), rlevel: 2, allpiu: 0, srrda_dpiu: "D", SrrdaNdCode: $("#Agency option:selected").val() };
                }
                ValidateForm(data);
            }

            if ($("#LevelId").val() == 5 && $("#rdbAnnual").is(":checked")) {
                data = { month: 0, Year: $("#Year option:selected").val(), ndcode: $("#AdminNdCode").val(), rlevel: 2, allpiu: 0, srrda_dpiu: "D" };
                ValidateForm(data);
            }
            if ($("#LevelId").val() == 5 && $("#rdbMonth").is(":checked")) {
                data = { month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndcode: $("#AdminNdCode").val(), rlevel: 2, allpiu: 0, srrda_dpiu: "D" };
                ValidateForm(data);
            }
        }

    });

    $("#Month").change(function () {
        UpdateAccountSession($("#Month").val(), $("#Year").val());
    });

    $("#Year").change(function () {
        UpdateAccountSession($("#Month").val(), $("#Year").val());
    });

});


function ValidateForm(param) {
    $.ajax({
        url: "/AccountReports/Account/ValidateParameter",
        type: "POST",
        data: param,
        success: function (data) {
            if (data.success == false) {
                $("#dvError").show();
                $("#dvError").html('<strong>Alert : </strong>' + data.message);

                return false;
            }
            else if (data.success == true) {
                loadFundReceivedReport(param)
            }

        }
    });
}

function loadFundReceivedReport(data) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: '/AccountReports/Account/FundReceivedReport/',
        type: 'POST',
        catche: false,
        async: false,
        data: data,
        error: function (xhr, status, error) {
            alert('An Error occured while processig your request.');
            $.unblockUI();
            return false;
        },
        success: function (data) {
            //Load Report
            $("#dvLoadReport").html(data);
            //unblockPage();
            $.unblockUI();

        }
    });

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
