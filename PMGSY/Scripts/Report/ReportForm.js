
$(document).ready(function () {
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
            $("#Month option:nth("+month+")").attr("selected", "selected");
            
        }
    });
});