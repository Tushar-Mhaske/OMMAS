var StateSrrdsPIU;

$(document).ready(function () {
    $.unblockUI()
    

    $(function () {
        $("#ddlNodalAgency").trigger("change");
    });

    $("#btnViewBalanceSheetDetails").click(function () {
                
        //validation start        
        
        if ($("#rdYearly").is(":checked")) {
            if ($("#ddlBalYear").val() == -1) {
                alert("Please select Year.");
                return false;
            }
        }
        if ($("#rdMonthly").is(":checked")) {
            if ($("#ddlBalMonth").val() == 0) {
                alert("Please select Month.");
                return false;
            }
            if ($("#ddlBalYear").val() == 0) {
                alert("Please select Year.");
                return false;
            }
        }
        if ($("#ddlFundTypeBalSheet").val() == 0) {
            alert("Please select Fund Type.");
            return false;
        }
     
        if ($("#LevelIdBalSheet").val() == 5) {
            if ($("#ddlPIUList").val() == 0) {
                alert("Please select DPIU.");
                return false;
            }
        }

        
        

        //validation end

        var StateSrrdsPIU;
        var MonthlyYearly;

            if ($("#rdbState").is(":checked")) {
                StateSrrdsPIU = $("#rdbState").val();
            } else if ($("#rdbSRRDA").is(":checked")) {
                StateSrrdsPIU = $("#rdbSRRDA").val();
            } else if ($("#rdbAllDPIU").is(":checked")) {
                StateSrrdsPIU = $("#rdbAllDPIU").val();
            }

            if ($("#rdMonthly").is(":checked")) {
                MonthlyYearly = $("#rdMonthly").val();
            } else if ($("#rdYearly").is(":checked")) {
                MonthlyYearly = $("#rdYearly").val();
            }
            


            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

            $("#loadReport").html("");

            $("#loadReport").load("/AccountReports/Account/BalanceSheetReport/" +
                                       $("#LevelIdBalSheet").val() + "$" +
                                       StateSrrdsPIU+"$"+
                                       $("#ddlNodalAgency").val() + "$" +
                                       $("#ddlPIUList").val() + "$" +
                                       $("#ddlBalMonth").val() + "$" +
                                       $("#ddlBalYear").val() + "$" +
                                       "P" + "$" +
                                       MonthlyYearly
                                       ,
                                   $.unblockUI());
    });


    //STATE ddl change
    $("#ddlNodalAgency").change(function () {

        var adminNdCode = $("#ddlNodalAgency option:selected").val();

        $.ajax({
            type: 'POST',
            url: '/Account/PopulateDPIU?id=' + adminNdCode,
            async: false,
            cache: false,
            success: function (data) {
                $("#ddlPIUList").empty();
                $.each(data, function () {
                    $("#ddlPIUList").append("<option value=" + this.Value + ">" + this.Text + "</option>");
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

    $("#rdbAllDPIU").click(function () {

            $("#ddlNodalAgency").trigger('change');
            $("#ddlPIUList").show('slow');
            $("#lblShowDPIU").show('slow');        
    });

    $("#rdbState").click(function () {

        $("#ddlNodalAgency").trigger('change');        
        $("#ddlPIUList").hide('slow');        
        $("#lblShowDPIU").hide('slow');
    });

    $("#rdbSRRDA").click(function () {

        $("#ddlNodalAgency").trigger('change');        
        $("#ddlPIUList").hide('slow');
        $("#lblShowDPIU").hide('slow');
    });



    $("#rdMonthly").click(function () {

        $("#trMonthYear").show();
        $("#trTempddlMonthShow").hide();

        if ($("#rdYearly").is(':checked')) {
            $("#rdYearly").attr('checked', false);
        }

        if ($("#LevelIdBalSheet").val() == 6 || $("#LevelIdBalSheet").val() == 4 || $("#LevelIdBalSheet").val() == 5) {
            HideAllMonthYears();
            $("#ddlBalMonth").show();
            $("#ddlBalYear").show();
            $("#trlblMonth").show();
            $("#trddlMonth").show();
        }

        FillInCascadeDropdown(null, "#ddlBalYear", "/AccountReports/Account/PopulateYears/");

    });

    $("#rdYearly").click(function () {

        $("#trMonthYear").show('slow');
        $("#trlblMonth").hide();
        $("#trddlMonth").hide();
        $("#trTempddlMonthShow").show();

        trTempddlMonthShow

        if ($("#rdMonthly").is(':checked')) {
            $("#rdMonthly").attr('checked', false);
        }

        if ($("#LevelIdBalSheet").val() == 6 || $("#LevelIdBalSheet").val() == 4 || $("#LevelIdBalSheet").val() == 5) {
            HideAllMonthYears();
            $("#ddlBalMonth").hide('slow');
            $("#ddlBalYear").show('slow');
        }

        FillInCascadeDropdown(null, "#ddlBalYear", "/AccountReports/Account/PopulateFinancialYears/");
    });
    function FillInCascadeDropdown(map, dropdown, action) {

        $(dropdown).empty();
        $.post(action, map, function (data) {

            $.each(data, function () {                
                if (this.Selected == true)
                { $(dropdown).append("<option value='" + this.Value + "' selected =" + this.Selected + ">" + this.Text + "</option>"); }
                else { $(dropdown).append("<option value='" + this.Value + "'>" + this.Text + "</option>"); }
            });
        }, "json");
    }

    function HideAllMonthYears() {
        $("#ddlBalMonth").hide();
        $("#ddlBalYear").hide();
        $("#ddlYear").hide();
    }
});

