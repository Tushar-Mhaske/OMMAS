﻿
$(document).ready(function () {
    $.validator.unobtrusive.parse("#frmSheduleView");

    //Added by Abhishke kamble 24-dec-2013 S1tart    
    if ($("#SelectedReport").val() == "O") {
    
        $("#ddlMonth").val($("#SelectedMonth").val());
        $("#ddlYear").val($("#SelectedYear").val());

        $("#ddlYear").focus();
        $("#ddlMonth").focus();
        $("#ddlYear").focus();

        if ($("#SelectedHead").val() == 0) {
            $("#ddlHead option:eq(1)").attr("selected", "selected");
        } else {
            $("#ddlHead").val($("#SelectedHead").val());
        }

        $("#ddlState").val($("#SelectedState").val());

        setTimeout(function () {
            $("#ddlState").trigger("change");
        }, 200);

        if ($("#SelectedDPIU").val() != 0) {
            setTimeout(function () {

                $("#ddlDPIU").val($("#SelectedDPIU").val());

            }, 300);
        }
        if ($("#SelectedDPIU").val() != 0) {
            setTimeout(function () {
                $("#btnViewDetails").trigger("click");
            }, 400);
        }
    }//Added by Abhishke kamble 6-jan-2014 Start    
    else if ($("#SelectedReport").val() == "N")
    {
        $("#ddlMonth").val($("#SelectedMonth").val());
        $("#ddlYear").val($("#SelectedYear").val());

        $("#ddlYear").focus();
        $("#ddlMonth").focus();
        $("#ddlYear").focus();

        if ($("#SelectedHead").val() == 0) {
            $("#ddlHead option:eq(1)").attr("selected", "selected");
        } else {
            $("#ddlHead").val($("#SelectedHead").val());
        }

        $("#ddlState").val($("#SelectedState").val());

        setTimeout(function () {
            $("#ddlState").trigger("change");
        }, 200);

        if ($("#SelectedDPIU").val() != 0) {
            setTimeout(function () {

                $("#ddlDPIU").val($("#SelectedDPIU").val());

            }, 300);
        }
        if ($("#SelectedDPIU").val() != 0) {
            setTimeout(function () {
                $("#btnViewDetails").trigger("click");
            }, 400);
        }    
    }
    else if ($("#SelectedReport").val() == "U") {
        $("#ddlMonth").val($("#SelectedMonth").val());
        $("#ddlYear").val($("#SelectedYear").val());

        $("#ddlYear").focus();
        $("#ddlMonth").focus();
        $("#ddlYear").focus();

        if ($("#SelectedHead").val() == 0) {
            $("#ddlHead option:eq(1)").attr("selected", "selected");
        } else {
            $("#ddlHead").val($("#SelectedHead").val());
        }

        $("#ddlState").val($("#SelectedState").val());

        setTimeout(function () {
            $("#ddlState").trigger("change");
        }, 200);

        if ($("#SelectedDPIU").val() != 0) {
            setTimeout(function () {

                $("#ddlDPIU").val($("#SelectedDPIU").val());

            }, 300);
        }
        if ($("#SelectedDPIU").val() != 0) {
            setTimeout(function () {
                $("#btnViewDetails").trigger("click");
            }, 400);
        }
    }
    //Added by Abhishke kamble 6-jan-2014 end

    //Added by Abhishke kamble 24-dec-2013 End
    $("#btnViewDetails").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#HeadName").val($("#ddlHead option:selected").text());
        $("#PiuName").val($("#ddlDPIU option:selected").text());
        $("#YearName").val($("#ddlYear option:selected").text());
        $("#MonthName").val($("#ddlMonth option:selected").text());
        $("#AgencyName").val($("#ddlAgency option:selected").text());
        $("#StateName").val($("#ddlState option:selected").text());
        if ($("#frmSheduleView").valid()) {
            $.ajax({
                url: "/AccountsReports/ScheduleDetails/",
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmSheduleView").serialize(),
                success: function (data) {

                    $("#ScheduleDetails").html(data);

                    $(function () {
                        $("#spnhdSchedule").trigger('click');
                    });
                    $.unblockUI();
                },
                error: function (data) {
                    $("#ddlState").attr("disabled", false);
                    $.unblockUI();
                }
            });
        } $.unblockUI();
    });

    if ($("#LevelId").val() == 4) {
        $("#ddlState").val($("#AdminNdCode").val());
        
        $(function () {
            $("#ddlState").trigger("change");
        });
    }


    $("#ddlState").change(function () {

        //alert("Change");

        $.blockUI({ message: '<h4><label style="font-weight:normal">loading DPIU...</label> ' });
        var val = $("#ddlState").val() + "$" + "S";
        $.ajax({
            type: 'POST',
            url: "/AccountsReports/PopulateDPIU?id=" + val,
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


    $("#spnhdSchedule").click(function () {
        // $("#spnhdAnnualAccount").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
        $("#spnhdSchedule").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvScheduleView").slideToggle("slow");        
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