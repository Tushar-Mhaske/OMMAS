

//new Creadit Heads -5,77,333 added by Abhishek to show all piu Option 21-June-2014
var piuDebitHead = [3, 5, 10, 72, 75, 77, 252, 256, 260, 264, 268, 272, 276, 333];
var roadsHead = [28, 29];
var levelId = 0;
$(function () {

    //clear error Messages
    $('.input-validation-error').removeClass('input-validation-error dropdown');
    $('.input-validation-error').addClass('input-validation-valid dropdown');
    $('.field-validation-error').html('');

    //Added By Abhishek kamble 4-oct-2013
    $(function () {
        if ($("#LevelID").val() == 4) {
            $("#ddlSRRDA").val($("#NDCode").val());
        }
        $("#ddlSRRDA").trigger("change");
        //$("#DPIULevel").hide("slow");
        $("#DPIULevel").attr("disabled", true);
    });

    levelId = $("#hdLevelId").val();

    $("#divRoadStatus").hide();
    $("#divPIU").hide();
    //$("#divSRRDA").hide();
    if (levelId == 4) {

        $("#piuTR").hide();

    }
    $.validator.unobtrusive.parse($("#LedgerForm"));

    //event
    $("#rdCredit").change(function () {
        if ($(this).val() == "C") {
            $("#divPIU").hide();
            if (levelId == 4) {
                $("#piuTR").hide();
            }
            $("#spnCreditDebitReport").text("Credit Balances");

            if ($("#LevelID").val() != 5) {

                var SRRDA_DPIU;

                if ($("#rdoSRRDA").is(":checked")) {
                    SRRDA_DPIU = "C$S";
                } else {
                    SRRDA_DPIU = "C$D";
                }
                FillInCascadeDropdown(null, "#HEAD", "/AccountReports/Account/GetLedgerHeadList/" + SRRDA_DPIU);
            }
            else {

                FillInCascadeDropdown(null, "#HEAD", "/AccountReports/Account/GetLedgerHeadList/C");
            }
        }
        $("#divRoadStatus").hide();
    });

    $("#rdDebit").change(function () {
        if ($(this).val() == "D") {
            $("#spnCreditDebitReport").text("Debit Balances");
            if ($("#LevelID").val() != 5) {

                var SRRDA_DPIU;

                if ($("#rdoSRRDA").is(":checked")) {
                    SRRDA_DPIU = "D$S";
                } else {
                    SRRDA_DPIU = "D$D";
                }
                FillInCascadeDropdown(null, "#HEAD", "/AccountReports/Account/GetLedgerHeadList/" + SRRDA_DPIU);
            }
            else {
                FillInCascadeDropdown(null, "#HEAD", "/AccountReports/Account/GetLedgerHeadList/D");
            }
        }
    });
    $("#rdoSRRDA").change(function () {

        if ($("#rdCredit").is(":checked")) {
            FillInCascadeDropdown(null, "#HEAD", "/AccountReports/Account/GetLedgerHeadList/C$S");
        }
        else {
            FillInCascadeDropdown(null, "#HEAD", "/AccountReports/Account/GetLedgerHeadList/D$S");
        }
        $("#DPIULevel option:first").attr("selected", "selected");
        $("#DPIULevel").attr("disabled", true);
        $("#divRoadStatus").hide();
    });

    $("#rdoDPIU").change(function () {

        $("#divPIU").hide();

        if ($("#rdCredit").is(":checked")) {
            FillInCascadeDropdown(null, "#HEAD", "/AccountReports/Account/GetLedgerHeadList/C$D");
        }
        else {
            FillInCascadeDropdown(null, "#HEAD", "/AccountReports/Account/GetLedgerHeadList/D$D");
        }
        $("#DPIULevel").attr("disabled", false);
    });

    //event of buttonClick
    $("#btnViewDetails").click(function () {

        if ($("#rdoDPIU").is(":checked")) {
            if ($("#DPIULevel").val() == "0") {
                alert("Please select DPIU");
                return false;
            }
        }
        var piuCode = -1;
        var piuName = ''
        if ($("#LedgerForm").valid()) {
            $("#LoadReport").html('');

            var crediDebit;

            if ($("#rdCredit").is(":checked")) {
                crediDebit = "C";

                //All PIU Option For creadit heads-P-5,A-77,M-333 21-June-2014 start
                if ($("#hdIsPIUHead").val() == "true") {
                    piuCode = $("#ddlDPIU").val();
                    piuName = $("#ddlDPIU option:selected").text();

                } else {
                    piuCode = -1;
                }
                //All PIU Option For creadit heads-P-5,A-77,M-333 21-June-2014 end

            }
            else {
                crediDebit = "D";
                if ($("#hdIsPIUHead").val() == "true") {
                    piuCode = $("#ddlDPIU").val();
                    piuName = $("#ddlDPIU option:selected").text();

                } else {
                    piuCode = -1;
                }

            }

            blockPage();
            var roadStatus = "N";

            if ($("#rdDebit").is(":checked") && ($("#HEAD").val() == 28 || $("#HEAD").val() == 29)) {

                if ($("#rdRoadCompleted").is(":checked")) {
                    roadStatus = "C";
                }
                else if ($("#rdRoadInprogress").is(":checked")) {
                    roadStatus = "P";
                }
            }

            var AdminNdcode;
            var SRRDA_DPIU;
            if ($("#rdoSRRDA").is(":checked")) {
                AdminNdcode = $("#ddlSRRDA").val();
                SRRDA_DPIU = "S";
            }
            else {
                AdminNdcode = $("#DPIULevel").val();
                SRRDA_DPIU = "D";
            }
            var parameters = $("#MONTH").val() + "$" + $("#YEAR").val() + "$" + crediDebit + "$" + $("#HEAD").val() + "$" + piuCode + "$" + roadStatus + "$" + AdminNdcode + "$" + SRRDA_DPIU + "$" + $("#HEAD option:selected").text() + "$" + $("#ddlSRRDA").val();

            $.ajax({
                url: '/AccountReports/Account/LedgerReport?id=' + parameters,
                type: 'POST',
                catche: false,
                async: false,
                error: function (xhr, status, error) {
                    alert('An Error occured while processig your request.');
                    unblockPage();
                    return false;
                },
                success: function (data) {
                    //Minimize filter form
                    $("#spnLedger").trigger("click");

                    //Load Report
                    $("#LoadReport").html(data);
                    $("#HeadDescTD").text($("#HEAD :selected").text()).css("text-align", 'center').css("font-weight", "bold");
                    if (levelId == 4) {
                        $("#piuTR td:eq(1)").html($("#ddlDPIU option:selected").text());
                    }
                    unblockPage();
                }
            });
        }
    });

    $("#HEAD").change(function () {
        var selectedHead = $(this).val();
        var flag = false;
        var flagForRoadStatus = false;
        if ($("#rdDebit").is(":checked")) {
            $.each(roadsHead, function (index, value) {
                if (parseInt(selectedHead) == parseInt(value)) {
                    flagForRoadStatus = true;
                }
            });
            if (flagForRoadStatus) {
                $("#divRoadStatus").show();
            }
            else {
                $("#divRoadStatus").hide();
            }
        }


        $.each(piuDebitHead, function (index, value) {
            if (parseInt(selectedHead) == parseInt(value)) {
                flag = true;
            }
        });
        $("#hdIsPIUHead").val(flag);

        if ($("#hdIsPIUHead").val() == "true") {
            $("#divPIU").show();
            $("#piuTR").show();
        }
        else {
            $("#divPIU").hide();
            if (levelId == 4) {
                $("#piuTR").hide();
            }
        }
    });

    $("#ddlSRRDA").change(function () {

        var adminNdCode = $('#ddlSRRDA option:selected').val();

        $.ajax({
            url: '/AccountReports/Account/PopulateDPIUOfSRRDA/' + adminNdCode,
            type: 'GET',
            catche: false,
            error: function (xhr, status, error) {
                alert('An Error occured while processig your request.')
                return false;
            },
            success: function (data) {
                var counter = 0;
                $('#ddlDPIU').empty();
                $('#DPIULevel').empty();
                $.each(data, function () {

                    if (this.Text != "Select Department") {
                        $('#ddlDPIU').append("<option value=" + this.Value + ">" + this.Text + "</option>");
                    }
                    if (counter == 0) {
                        $('#DPIULevel').append("<option value=0>Select DPIU</option>");
                    } else {
                        if (this.Text != "Select Department") {
                            $('#DPIULevel').append("<option value=" + this.Value + ">" + this.Text + "</option>");
                        }
                    }
                    counter++;
                });
            }
        });

    });

    $("#spnLedger").click(function () {
        $("#spnLedger").toggleClass("ui-icon ui-icon-circle-triangle-n").toggleClass("ui-icon ui-icon-circle-triangle-s");
        $("#dvFilterForm").slideToggle("slow");
    });


    $("#MONTH").change(function () {

        UpdateAccountSession($("#MONTH").val(), $("#YEAR").val());

    });

    $("#YEAR").change(function () {

        UpdateAccountSession($("#MONTH").val(), $("#YEAR").val());

    });

});

//function to fill the dropdownbox  dynamically
function FillInCascadeDropdown(map, dropdown, action) {

    $(dropdown).empty()
    $.post(action, map, function (data) {
        $.each(data, function () {
            if (this.Selected == true)
            { $(dropdown).append("<option value='" + this.Value + "' selected =" + this.Selected + ">" + this.Text + "</option>"); }
            else { $(dropdown).append("<option value='" + this.Value + "'>" + this.Text + "</option>"); }
        });
    }, "json");
}

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