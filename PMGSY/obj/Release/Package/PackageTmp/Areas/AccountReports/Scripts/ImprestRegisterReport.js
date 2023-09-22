var adminCode;

$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmImprestSettlement');

    $("#spnImprestSettlement").click(function () {
        $("#spnImprestSettlement").toggleClass("ui-icon ui-icon-circle-triangle-n").toggleClass("ui-icon ui-icon-circle-triangle-s");
        $("#tblImprestSettlement").slideToggle("slow");
    });

    $(".trSrrdaDpiu").hide();
    $(".tdDpiu").hide();

    $("#rdbSrrda").click(function () {
        $(".trSrrdaDpiu").show('slow');
        if ($(".tdDpiu").is(":visible")) {
            $(".tdDpiu").hide('slow');
        }
    });

    $("#rdbDpiu").click(function () {
        $("#ddlSrrda").trigger('click');
        $(".tdDpiu").show('slow');
    });

    $("#ddlSrrda").change(function () {
        if ($("#LevelId").val() != 5) {
            FillInCascadeDropdown(null, "#ddlDpiu", "/AccountReports/Account/PopulateDPIUOfSRRDA?id=" + $("#ddlSrrda").val());
        }

    });

    $("#btnView").click(function () {

        if ($("#frmImprestSettlement").valid()) {

            if ($("#LevelId").val() != 5) {

                $("#NodalAgency").val($("#ddlSrrda option:selected").text());
                if ($("#rdbDpiu").is(":checked")) {
                    $("#DPIUName").val($("#ddlDpiu option:selected").text());
                }

                if ($("#rdbDpiu").is(":checked") && $("#ddlDpiu option:selected").val() == 0) {
                    alert("Please select DPIU");
                    return false;
                }
            }
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            if ($("#rdbSrrda").is(":checked")) {
                adminCode = $("#ddlSrrda option:selected").val();
            }
            else if ($("#rdbDpiu").is(":checked")) {
                adminCode = $("#ddlDpiu option:selected").val();
            }
            $("#ddlSrrda").attr('disabled', false);

            $.ajax({

                type: 'POST',
                url: '/AccountReports/Account/ImprestRegisterReport',
                data: $("#frmImprestSettlement").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    $.unblockUI();
                    $("#dvLoadReport").show();
                    $("#dvLoadReport").html(data);
                    if ($("#LevelId").val() == 4) {
                        $("#ddlSrrda").attr('disabled', 'disabled');
                    }
                    if ($("#LevelId").val() != 5) {
                        $("#spnImprestSettlement").trigger("click");
                    }
                },
                error: function () {
                    $.unblockUI();
                }
            });
        }
    });

    if ($("#LevelId").val() != 5) {
        $("#rdbSrrda").trigger('click');
    }

});

function FillInCascadeDropdown(map, dropdown, action) {

    $(dropdown).empty();
    $.post(action, map, function (data) {
        $.each(data, function () {
            if (this.Value != 0) {
                if (this.Selected == true) {
                    $(dropdown).append("<option value='" + this.Value + "' selected =" + this.Selected + ">" + this.Text + "</option>");
                }
                else { $(dropdown).append("<option value='" + this.Value + "'>" + this.Text + "</option>"); }
            }
            else {
                if (this.Text == "Select Department") {
                    $(dropdown).append("<option value='" + this.Value + "'>" + this.Text + "</option>");
                }
            }
        });
    }, "json");
}

