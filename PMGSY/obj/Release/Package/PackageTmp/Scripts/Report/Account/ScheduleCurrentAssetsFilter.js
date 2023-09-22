
$(document).ready(function () {
    // $("#spanPIU").hide();

    //added by abhishek kamble 3-dec-2013
    //$("#rdbAnnual").click(function () {
    //    $(function () {
    //        $("#spanMonthlyYear").show();
    //        $("#spanYear").hide();
    //    });
    //});

    //$("#rdbMonth").click(function () {
    //    $(function () {
    //        $("#spanMonthlyYear").hide();
    //        $("#spanYear").show();
    //    });
    //});


    //added by abhishek kamble 20-dec-2013
    $(function () {



        if (($("#rdbState").is(":checked")) || ($("#rdbSrrda").is(":checked")) || ($("#rdbPiu").is(":checked"))) {
            $("#Agency").focus();
            $("#Year").focus();
            $("#Agency").focus();
        }
        else {
            $("#Agency").focus();
            $("#Piu").focus();
            $("#Agency").focus();
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
                $("#btnView").trigger("click");
            }
            else if ($("#rdbMonth").is(":checked")) {

                $("#btnView").trigger("click");
            }
            else {
                $("#rdbMonth").attr("checked", "checked");
                $("#rdbState").attr("checked", "checked");

                $("#rdbMonth").trigger("change");
                //$("#Year option:eq(1)").attr("selected", "selected");
                $("#Year").focus();
                $("#Month").focus();
                $("#Year").focus();
            }
        }, 500);
    });

    var ndlevel = $("#LevelId").val();
    $("#Agency").change(function () {
        var ndcode = $(this).val();
        
        $.getJSON("/Reports/GetDPIUOfSRRDA/", { ndcode: ndcode }, function (responseData) {
            var strOption = "";
            $("#Piu").empty();
            if (responseData.length > 1) {
                strOption += "<option value='0' selected>All DPIU</option>";
            }
            $.each(responseData, function (index, record) {
           
                strOption+="<option value='"+ record.Value +"'>"+ record.Text +"</option>";
            });
           
            $("#Piu").append(strOption);

        });
        
    });
    $(".rdbClassReportLevel").change(function () {
        //alert("OK");
        var reportlevel = $(this).val();
       // alert(reportlevel);
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
                    $(this).text(currentYear+"-"+(parseInt(currentYear)+1));
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
            //$("#Month option:nth("+month+")").attr("selected", "selected");
            
        }
    });

    //if ($("#rdbAnnual").is(":checked") && $("#LevelId").val() == 5) {
    //    alert("call");
    //    $("#tdBtnViewDetails").show('slow');
    //    $("#tdOtherBthViewDetails").hide('slow');
    //}

    //$("#rdbAnnual").click(function () {        
    //    if ($("#LevelId").val() == 5) {           
    //        $("#spanMonth").hide('slow');            
    //           $("#tdBtnViewDetails").show('slow');
    //           $("#tdOtherBthViewDetails").hide('slow');
    //       }
    //});
    
});