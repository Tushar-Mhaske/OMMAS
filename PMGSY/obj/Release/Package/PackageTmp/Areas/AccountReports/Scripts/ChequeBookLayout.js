jQuery.validator.addMethod("reqiredmonthyear", function (value, element, param) {
    var IsMonthWise;
    var IsChqWise;
    if ($("#rdoMonthWise").is(":checked")) {
        IsMonthWise = $('#rdoMonthWise').val();
        var MonthInfo = $('#ddlMonth option:selected').val();
        var YearInfo = $('#ddlYear option:selected').val();
        //  if ($("#rdoMonthWise").is(":checked")) {
        if (IsMonthWise == "M" && MonthInfo == "" && YearInfo == "") {
            return false;
        }
        else {
            return true;
        }
        //  }
    }
    else if ($("#rdoCheckbookWise").is(":checked")) {
        IsChqWise = $('#rdoCheckbookWise').val();
        var chqseries = $("#ddlCheckBookSeries option:selected").val();
        if (IsChqWise == "C" && chqseries == "") {
            return false;
        }
        else {
            return true
        }

    }




});

jQuery.validator.unobtrusive.adapters.addBool("reqiredmonthyear");



$(document).ready(function () {

    $(function () {

        //get current month
        var date = new Date();
        var month = $("#Month").val();
        var year = $("#Year").val();
        $('#ddlDPIU option:nth(1)').attr("selected", "selected");
        //        $("#ddlMonth option:nth(" + month + ")").attr("selected", "selected");
        //$("#ddlYear option:nth(" + year + ")").attr("selected", "selected");
        //$('#ddlYear option:last').attr("selected", "selected");
        $('#ddlYear').val(year);
//        $('#ddlDPIU').trigger("change");
        //alert("test 1");
        if ($("#LevelId").val() == 4) {
            $("#ddlSRRDA").val($("#AdminNdCode").val());
        }

        $('#ddlSRRDA').trigger("change");

        if ((($("#LevelId").val() == 4) || ($("#LevelId").val() == 5)) && ($("#FundType").val() == "A")) {
             $(".thDPIUShow").hide();
        }
        $("#rdoDPIU").click(function () {
            $(".thDPIUShow").show();
            PopulateChequeBooks($('#ddlDPIU option:selected').val(), "D");
        });
        $("#rdoSRRDA").click(function () {
            $(".thDPIUShow").hide();
            PopulateChequeBooks($('#ddlSRRDA option:selected').val(), "S");
            $('#ddlDPIU').val(0);
        });
       
    });

    //Added by abhishek kamble 8-oct-2013
    $("#ddlSRRDA").change(function () {

        var adminNdCode = $('#ddlSRRDA option:selected').val();

        $.ajax({
            url: '/AccountReports/Account/PopulateDPIU/' + adminNdCode,
            type: 'POST',
            catche: false,
            error: function (xhr, status, error) {
                alert('An Error occured while processig your request.')
                return false;
            },
            success: function (data) {

                $('#ddlDPIU').empty();

                $.each(data, function () {
                    $('#ddlDPIU').append("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
               // alert("test 2");

                setTimeout(function () {
                    $('#ddlDPIU').trigger("change");
                }, 1000);

            }
        });

    });


    if ($("#rdoMonthWise").is(":checked")) {
        resetMonthWise();
    }

    $("#idFilterDiv").click(function () {
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvChequebookDetails").slideToggle("slow");
    });

    //enable/disable month-checkbookwise ddl
    $("#rdoMonthWise").click(function () {
        resetMonthWise();
    });

    //enable/disable month-checkbookwise ddl
    $("#rdoCheckbookWise").click(function () {
        resetCheckbookWise();
    });

    $("#btnViewDetails").click(function () {
        
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#dvError").hide();
        $("#dvError").html("");

        // $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#ddlMonth").val() != 0) {
            $("#MonthName").val($("#ddlMonth option:selected").text());
        } else {
            $("#MonthName").val("-");
        }

        if ($("#ddlYear").val() != 0) {
            $("#YearName").val($("#ddlYear option:selected").text());
        } else {
            $("#YearName").val("-");
        }

        if ($("#ddlCheckBookSeries").val() != 0) {
            $("#CheckbookSeriesName").val($("#ddlCheckBookSeries option:selected").text());
        } else {
            $("#CheckbookSeriesName").val("-");
        }
        if ($("#rdoMonthWise").is(":checked")) {
            $("#MonthlyOrChequebookWiseSelection").val("Month Wise");
        }

        if ($("#rdoCheckbookWise").is(":checked")) {
            $("#MonthlyOrChequebookWiseSelection").val("Cheque Book Wise");
        }

        $("#Month").val($("#ddlMonth option:selected").val());
        $("#Year").val($("#ddlYear option:selected").val());

        // if (ValidateForm() == true) {
        $.ajax({
            type: 'POST',
            url: '/AccountReports/Account/ShowChequebookReport/',
            data: $("#frmCheckbookDetails").serialize(),
            async: false,
            cache: false,
            success: function (data) {

                if (data.success == false) {
                    $("#dvError").show('slow');
                    $("#dvError").html(data.message);
                }
                else {

                    $("#dvShowCheckBookDetails").html('');
                    $("#dvShowCheckBookDetails").html(data);

                    if ($("#rdoCheckbookWise").is(":checked")) {
                        $(".tdChequebookSeriesNo").show();
                        $(".tdMonthYearName").hide();

                    }
                    if ($("#rdoMonthWise").is(":checked")) {
                        $(".tdMonthYearName").show();
                        $(".tdChequebookSeriesNo").hide();
                    }

                    $("#dvChequebookDetails").hide("slow");
                }
                $.unblockUI();
                // $.unblockUI();
            },

            error: function () {
                //  $.unblockUI();
                alert("Request can not be processed at this time.");
                $.unblockUI();
            }
        })
        // } else {
        // $.unblockUI();
        // }
    });

    //DUIU ddl change
    $("#ddlDPIU").change(function () {

       // alert("test ");

        //var AdminNdCode = $("#ddlDPIU option:selected").val();
        var AdminNdCode = 0;
        var IsSRRDA_DPIU = "";

        var DPIUNdCode = $("#ddlDPIU option:selected").val();        

        if ($("#rdoSRRDA").is(":checked")) {//SRRDA code
            AdminNdCode = $("#ddlSRRDA option:selected").val()
            IsSRRDA_DPIU = "S";
        } else {//DPIU code
            AdminNdCode = DPIUNdCode;
            IsSRRDA_DPIU = "D";
        }

        PopulateChequeBooks(AdminNdCode,IsSRRDA_DPIU);
       
    });

    $("#ddlMonth").change(function () {

        //alert($("#ddlMonth option:selected").val());        

        //$('#ddlMonth option:selected').removeAttr('selected');
        //$("#ddlMonth option:nth(" + $("#ddlMonth").val() + ")").attr("selected", "selected");
        //alert($("#ddlMonth option:selected").val());


        UpdateAccountSession($("#ddlMonth").val(), $("#ddlYear").val());

    });

    $("#ddlYear").change(function () {

        UpdateAccountSession($("#ddlMonth").val(), $("#ddlYear").val());

    });



});

function resetMonthWise() {

    //disable checkbook series ddl
    $("#ddlCheckBookSeries").attr("disabled", true);
    //$('#ddlCheckBookSeries option:nth(0)').attr("selected", "selected");

    //disable year and month ddl
    $("#ddlMonth").attr("disabled", false);
    $("#ddlYear").attr("disabled", false);

    //hide chechbook series ddl
    $("#trCheckbookWise").hide("slow");

    //show Month/Year ddl
    $("#trMonthWise").show("slow");

    //set Month to first value and Year to current Year
    if (($("#ddlMonth option:selected").val() == 0) && ($("#ddlYear").val() == 0)) {

        var date = new Date();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();
        //$("#ddlMonth option:nth(" + month + ")").attr("selected", "selected");
        //$('#ddlYear option:eq(1)').attr("selected", "selected");
        //$("#ddlYear option:nth(" + year + ")").attr("selected", "selected");
    }
}

function resetCheckbookWise() {
    $("#ddlCheckBookSeries").attr("disabled", false);

    $("#ddlMonth").attr("disabled", true);
    $("#ddlYear").attr("disabled", true);
    //$('#ddlMonth option:nth(0)').attr("selected", "selected");
    //$('#ddlYear option:nth(0)').attr("selected", "selected");

    //show chechbook series ddl
    $("#trCheckbookWise").show("slow");

    //hide Month/Year ddl
    $("#trMonthWise").hide("slow");

    //set cheque book series 
    if ($("#ddlCheckBookSeries").val() == 0) {
        $('#ddlCheckBookSeries option:nth(1)').attr("selected", "selected");
    }
}

//function ValidateForm() {

//    if ($("#ddlDPIU").val() == 0) {
//        alert("Please select DPIU");
//        return false;
//    }

//    if ($("#rdoMonthWise").is(":checked")) {
//        if ($("#ddlMonth").val() == 0) {
//            alert("Please select month");
//            return false;
//        }

//        if ($("#ddlYear").val() == 0) {
//            alert("Please select year");
//            return false;
//        }
//    }

//    if ($("#rdoCheckbookWise").is(":checked")) {
//        if ($("#ddlCheckBookSeries").val() == 0) {
//            alert("Please select cheque book series");
//            return false;
//        }
//    }
//    return true;
//}

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
            // return false;
        },
        error: function () { }
    });
    //return false;
}


function PopulateChequeBooks(AdminNdCode, IsSRRDA_DPIU)
{   
    $.ajax({
        type: 'POST',
        url: '/AccountReports/Account/GetChequebookSeriesByAdminNdCode?paramAdminNdCode=' + AdminNdCode,
        async: false,
        cache: false,
        data: { IsSRRDA_DPIU: IsSRRDA_DPIU },
        success: function (data) {
            $("#ddlCheckBookSeries").empty();
            $.each(data, function () {
                $("#ddlCheckBookSeries").append("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
        },
        error: function () {
            alert("Request can not be processed at this time.");
        }
    });
}
