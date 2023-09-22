
$(document).ready(function () {

    //added by abhishek kamble 20-dec-2013

    

    $(function () {

        if (($("#rdbState").is(":checked")) || ($("#rdbSrrda").is(":checked")) || ($("#rdbPiu").is(":checked"))) {
            $("#Agency").focus();
            $("#Year").focus();
            $("#Agency").focus();

            if (($("#rdbSrrda").is(":checked")))
            {   
                $("#spanPIU").hide();
            }
         
        }
        else {

           
            $("#Agency").focus();
            $("#Piu").focus();
            $("#Agency").focus();
            
            //added by abhishek kamble 24-dec-2013
            //to set radio button checked
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
                $("#btnView").trigger("click");
            }
            else if ($("#rdbMonth").is(":checked")) {
                $("#btnView").trigger("click");
            }
            else {
                
                $("#rdbMonth").attr("checked", "checked");
//                $("#rdbState").attr("checked", "checked");

                //$(".rdbClassReportType").trigger("change");
                //$("#Year option:eq(1)").attr("selected", "selected"); //commented by Vikram
                $("#Year").focus();
                $("#Month").focus();
                $("#Year").focus();
            }
        }, 500);
    });

   // $("#spanPIU").hide();
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
            //$("#Month option:nth("+month+")").attr("selected", "selected"); //commented by Vikram
            
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
    
    //added by abhishek kamble 24-dec-2013
    if (($("#rdbSrrda").is(":checked")) && ($("#spanSrrda").is(":hidden"))) {
        $("#rdbState").attr("checked",true);
    }

});