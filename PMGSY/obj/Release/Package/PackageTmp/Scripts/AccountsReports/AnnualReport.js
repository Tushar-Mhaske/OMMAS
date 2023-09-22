jQuery.validator.addMethod("requireddpiuinfo", function (value, element, param) {
   
    if ($("#rdoDPIU").is(":checked")) {
        var dpiuCode = $('#ddlDPIU option:selected').val();
        var isDPIUSel = $("#rdoDPIU").val();


        if (isDPIUSel == "D" && dpiuCode == "") {
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

jQuery.validator.unobtrusive.adapters.addBool("requireddpiuinfo");


//added by abhishek kamble 17-dec-2013

jQuery.validator.addMethod("reqiredstate", function (value, element, param) {
    if (($("#LevelId").val() != 5) && ($("#ddlState").val() == 0)) {
        return false;
    }
    else {
        return true;
    }
});

jQuery.validator.unobtrusive.adapters.addBool("reqiredstate");


$(document).ready(function () {
    $.validator.unobtrusive.parse("#frmAnnualAccount");

    //$("#Month option:nth(" + month + ")").attr("selected", "selected");
    //$("#ddlYear option:nth(1)").attr("selected", "selected");
    //$("#ddlBalance option:nth(1)").attr("selected", "selected");

    $("#spnhdAnnualAccount").click(function () {
       // $("#spnhdAnnualAccount").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
        $("#spnhdAnnualAccount").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvAnnualAccount").slideToggle("slow");
        $("#dvBalance").slideToggle("slow");
    });



    $("#tdState").show("slow");
    $("#tdSRRDA").show("slow");
    $("#tdYear").show("slow");
    $("#tdBalance").show("slow");
    $("#rdoState").click(function () {
        $("#tdState").show("slow");
        $("#tdSRRDA").show("slow");
        $("#tdYear").show("slow");
        $("#tdBalance").show("slow");
        $("#tdDPIU").hide("slow");
    });
    $("#rdoSRRDA").click(function () {
        $("#tdState").show("slow");
        $("#tdSRRDA").show("slow");
        $("#tdYear").show("slow");
        $("#tdValid").hide('slow');
        $("#tdEmpty").hide('slow');
        $("#tdBalance").show("slow");
        $("#tdDPIU").hide("slow");
    });

    $("#rdoDPIU").click(function () {
        $("#tdState").show("slow");
        $("#tdSRRDA").show("slow");
        $("#tdValid").show('slow');
        $("#tdEmpty").show('slow');
        $("#tdDPIU").show("slow");
        if ($("#ddlState").val() != 0) {
            $("#ddlState").trigger("change");
        }
    });

   




    $("#btnView").click(function () {

         if ($("#frmAnnualAccount").valid()) {
             $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#ddlState").attr("disabled", false);
            $.ajax({
                url: "/AccountsReports/AnnualAccountDetails/",
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmAnnualAccount").serialize(),
                success: function (data) {
                    $("#AccountDetails").html(data);
                    $("#ToolTables_tblMonthlyAccount_0").removeClass('DTTT_button').removeClass('ui-state-default').removeClass('DTTT_button_collection');
                    $("#ToolTables_tblMonthlyAccount_1").removeClass('DTTT_button').removeClass('ui-state-default').removeClass('DTTT_button_collection');
                    $("#tblAnnualAccount_info").css('text-align', 'left');
                    if ($("#LevelId").val()== 4) {
                        $("#ddlState").attr("disabled", true);
                    }
                    $.unblockUI();
                },
                error: function (data) {
                    $("#ddlState").attr("disabled", false);
                    $.unblockUI();
            }
            });
        }
    });
       
  

    //$("#ddlState").change(function () {
    //    $.blockUI({ message: '<h4><label style="font-weight:normal">loading Aggrements...</label> ' });
    //    var val = $("#ddlState").val();
    //    $.ajax({
    //        type: 'POST',
    //        url: "/AccountsReports/PopulateSRRDA?id=" + val,
    //        async: false,
    //        success: function (data) {
    //            $.unblockUI();
    //            $("#ddlSRRDA").empty();
    //            $.each(data, function () {
    //                $("#ddlSRRDA").append("<option value=" + this.Value + ">" +
    //                                                        this.Text + "</option>");

    //            });

    //            $.unblockUI();
    //        }
    //    });

    //    });

    

    $("#ddlState").change(function () {
        if ($("#rdoDPIU").is(":checked")) {
            $.blockUI({ message: '<h4><label style="font-weight:normal">loading DPIU...</label> ' });
            var val = $("#ddlState").val() +"$"+ "A";
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
        }

        });

   

   
});

function isValid() {
  
    if ($("#rdoState").is(":checked")) {
        if ($("#ddlState").val() == 0) {
            alert("Please select State");
            return false;
        }
        else if ($("#ddlSRRDA").val() == 0) {
            alert("Please select SRRDA");
            return false;
        }
        else if ($("#ddlYear").val() == 0) {
            alert("Please select Year");
            return false;
        }
        else if ($("#ddlBalance").val() == "%") {
            alert("Please select Balance");
            return false;
        }

    }

    if ($("#rdoSRRDA").is(":checked")) {
        if ($("#ddlState").val() == 0) {
            alert("Please select State");
            return false;
        }
        else if ($("#ddlSRRDA").val() == 0) {
            alert("Please select SRRDA");
            return false;
        }
        else if ($("#ddlYear").val() == 0) {
            alert("Please select Year");
            return false;
        }
        else if ($("#ddlBalance").val() == "%") {
            alert("Please select Balance");
            return false;
        }
    }

    if ($("#rdoDPIU").is(":checked")) {
        if ($("#ddlState").val() == 0) {
            alert("Please select State"); 
            return false;
        }
        else if ($("#ddlSRRDA").val() == 0) {
            alert("Please select SRRDA");
            return false;
        }
        //else if ($("#ddlDPIU").val() == 0) {
        //    alert("Please select DPIU");
        //    return false;
        //}
        else if ($("#ddlYear").val() == 0) {
            alert("Please select Year");
            return false;
        }
        else if ($("#ddlBalance").val() == "%") {
            alert("Please select Balance");
            return false;
        }

    }

    if ($("#LevelId").val() == 5) {

        if ($("#ddlYear").val() == 0) {
            alert("Please select Year");
            return false;
        }
        else if ($("#ddlBalance").val() == "%") {
            alert("Please select Balance");
            return false;
        }

    }

    return true;


}