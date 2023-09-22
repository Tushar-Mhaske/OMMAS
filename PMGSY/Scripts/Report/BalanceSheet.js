
//Added By Abhishek kamble 3-dec-2013
jQuery.validator.addMethod("monthvalidator", function (value, element, param) {

    var IsMonthly = $('#rdMonthly').val();
    var month = $('#ddlBalMonth').val();

    if ($("#rdMonthly").is(":checked")) {
        if (month == 0) {
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

    //added by abhishek kamble 15-jan-2014
    //alert($("#SelectedYear").val());
    //$("#ddlBalYear").val($("#SelectedYear").val());

    //Added By Abhishek kamble 31-jan-2014 start

    $(function () {
        if ($("#BalSheetReportLevel").val() == "A")
        {
            //$('#rdbAllDPIU').attr('checked', 'checked');
            $("#rdbAllDPIU").trigger("click");

            if ($("#BalSheetSelectedDPIU").val() != undefined && $("#BalSheetSelectedDPIU").val() != 0)
            {
                setTimeout(function () {
                    $("#ddlPIUList").val($("#BalSheetSelectedDPIU").val());
                }, 500);
            }

        } else if ($("#BalSheetReportLevel").val() == "O") {
            $('#rdbSRRDA').attr('checked', 'checked');
            $("#rdbSRRDA").trigger("click");
        } else {
            $("#rdbState").trigger("click");      
        }
        
        setTimeout(function () {
            $("#ddlBalMonth").val($("#BalSheetSelectedMonth").val());
            $("#ddlBalYear").val($("#BalSheetSelectedYear").val());

            $("#btnViewBalanceSheetDetails").trigger("click");
        }, 1000);
       

    });

    $(function () {
        //commented by Abhishek 31-jan-2014
        //$("#rdbState").trigger("click");

    });

    //Added By Abhishek kamble 31-jan-2014 start


    //added by abhishek kamble 5-dec-2013
    $('.saveButton').click(function () {
        alert("test");

        $(this).closest('form')[0].submit();
    });

    $("#idFilterDiv").click(function () {
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");

        
        if ($("#dvShowHideBalanceSheet").is(":hidden")) {
            $("#dvShowHideBalanceSheet").show("slow");
            $("#dvShowBalancesheetSchedules").hide("slow");
            
        }
        else {
            $("#dvShowHideBalanceSheet").hide("slow");
        }

        if ($("#dvBalanceSheetDetails").is(":hidden")) {
            $("#dvBalanceSheetDetails").show("slow");
        }
        else {
            $("#dvBalanceSheetDetails").hide("slow");
        }

        if ($("#tblMaintenanceFundBalanceSheet_wrapper").is(":hidden")) {
            $("#tblMaintenanceFundBalanceSheet_wrapper").show("slow");
        }
        else {
            $("#tblMaintenanceFundBalanceSheet_wrapper").hide("slow");
        }
        $("#dvShowBalancesheetSchedules").html('');
        
      


    });

   

   
   // alert("Ready");
   $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

   $.validator.unobtrusive.parse($('#frmMaintenanceBalanceSheet'));

   $(function () {

       $("#rdMonthly").trigger('click');
   });

   $('#btnViewBalanceSheetDetails').click(function () {
        
       SearchBalancesheet();
        
    });

    //$('#btnViewDetails').trigger('click');

    $("#ddlDPIUList").hide();
    $("#ddlPIUList").append('<option value=' + 0 + '>All PIUs</option>');


    //new change done by Vikram

    levelId = $("#Level").val();
    if (levelId == 6) {
        HideAllSections();
        HideAllMonthYears();
        $("#trStateDPIU").hide();
        $("#trMonthYear").hide();
    }

    if (levelId == 4 || levelId == 5) {
        $("#trMonthYear").hide();
    }


    $("#rdbSRRDA").click(function () {
        //modofied by abhishek kamble 23-dec-2013
        $("#ddlDPIUList option:eq(0)").attr("selected", "selected");

        if (levelId == 6) {
            HideAllSections();
            $("#trStateDPIU").show('slow');
            $("#lblShowDPIU").hide();
            $("#ddlNodalAgency").show('slow');
            $("#lblState").html('SRRDA');
        }
        else {
            $("#ddlDPIUList").hide('slow');
        }
    });

    $("#rdbState").click(function () {
        //modofied by abhishek kamble 23-dec-2013
        $("#ddlDPIUList option:eq(0)").attr("selected", "selected");

        if (levelId == 6) {
            HideAllSections();
            $("#ddlNodalAgency").show('slow');
            $("#trStateDPIU").show('slow');
            $("#lblShowDPIU").hide();
            $("#lblState").html('State');
        }
        else {
            $("#ddlDPIUList").hide('slow');
        }
    });

    $("#rdMonthly").click(function () {

        $("#trMonthYear").show('slow');
        if ($("#rdYearly").is(':checked')) {
            $("#rdYearly").attr('checked', false);
        }

        if (levelId == 6 || levelId == 4 || levelId == 5) {
            HideAllMonthYears();
            $("#ddlBalMonth").show('slow');
            $("#ddlBalYear").show('slow');
            $("#trlblMonth").show('slow');
            $("#trddlMonth").show('slow');
        }

        FillInCascadeDropdown(null, "#ddlBalYear", "/Reports/PopulateYears/");

    });

    $("#rdYearly").click(function () {

        $("#trMonthYear").show('slow');
        $("#trlblMonth").hide();
        $("#trddlMonth").hide();


        if ($("#rdMonthly").is(':checked')) {
            $("#rdMonthly").attr('checked', false);
        }

        if (levelId == 6 || levelId == 4 || levelId == 5) {
            HideAllMonthYears();
            $("#ddlBalMonth").hide('slow');
            $("#ddlBalYear").show('slow');
        }

        FillInCascadeDropdown(null, "#ddlBalYear", "/Reports/PopulateFinancialYears/");
    });


    $("#rdbAllDPIU").click(function () {

        if (levelId == 6) {
            HideAllSections();
            $("#ddlNodalAgency").trigger('change');
            $("#trStateDPIU").show('slow');
            $("#ddlPIUList").show('slow');
            $("#ddlNodalAgency").show('slow');
            $("#lblState").html('SRRDA');
            $("#lblShowDPIU").show('slow');
        }
        else {
            $("#ddlDPIUList").show('slow');
        }
    });

    //$("#rdbSRRDA").click(function () {

    //    $("#ddlDPIU").hide('slow');
    //    $("#trddlNodalAgency").show('slow');
    //    $("#ddlNodalAgency").show('slow');
    //    $("#ddlSRRDA").hide('slow');

    //});

    $("#rdbState").click(function () {
        $("#ddlDPIUList").hide('slow');
    });

    $("#ddlNodalAgency").change(function () {

        FillInCascadeDropdown(null, "#ddlPIUList", "/Reports/PopulateDPIU/" + $("#ddlNodalAgency").val());

    });

    $("#ddlBalMonth").change(function () {

        UpdateAccountSession($("#ddlBalMonth").val(), $("#ddlBalYear").val());

    });

    $("#ddlBalYear").change(function () {

        UpdateAccountSession($("#ddlBalMonth").val(), $("#ddlBalYear").val());

    });


});





function SearchBalancesheet() {
   
    if ($('#frmBalanceSheet').valid()) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        //new change done by Vikram on 06-09-2013
        if (levelId == 6 || levelId == 4 || levelId == 5) {
            if ($("#rdYearly").is(':checked') == false && $("#rdMonthly").is(':checked') == false) {
                alert('Please select Monthly or Yearly');
                return false;
            }

            if ($("#rdYearly").is(':checked')) {
                $("#ddlBalMonth").val(0);
                //added by abhishek kamble 3-dec-2013
                $("#IsMonthlyYearly").val(1);
            }

            if ($("#rdMonthly").is(':checked')) {
                $("#ddlBalMonth").val($("#ddlBalMonth").val());
                $("#ddlBalYear").val($("#ddlBalYear").val());
                //added by abhishek kamble 3-dec-2013
                $("#IsMonthlyYearly").val(2);
                $("#showMonthName").val($("#ddlBalMonth option:selected").text());
            }
        }
        //end of change
        //added by abhishek kamble 3-dec-2013
        if ($("#rdbAllDPIU").is(":checked") && ($("#Level").val()==6)) {
            $("#showDPIUName").val($("#ddlPIUList option:selected").text());
        }
        else {
            $("#showDPIUName").val($("#ddlDPIUList option:selected").text());
        }

      //  $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Reports/GetBalanceSheet",
            type: "POST",
            async: false,
            cache: false,
            data: $("#frmBalanceSheet").serialize(),
            success: function (data) {

                $('#divBalanceSheet').html(data);
                                
                $("#dvBalanceSheetDetails").show();
               
                $.unblockUI();

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }

        });
       // $.unblockUI();
    }
    //else {
    //    alert('Please select proper options.');
    //}
   
}
function HideAllSections() {
    $("#ddlMonth").hide();
    $("#ddlSRRDA").hide();
    $("#ddlNodalAgency").hide();
    $("#ddlPIUState").hide();
    $("#ddlPIUList").hide();

}
function HideAllMonthYears() {
    $("#ddlBalMonth").hide();
    $("#ddlBalYear").hide();
    $("#ddlYear").hide();
}
function FillInCascadeDropdown(map, dropdown, action) {

    $(dropdown).empty();
    $.post(action, map, function (data) {

        $.each(data, function () {
            // alert('TEST' + this.Selected);
            // alert("fillCascaded =" + this.Value);
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