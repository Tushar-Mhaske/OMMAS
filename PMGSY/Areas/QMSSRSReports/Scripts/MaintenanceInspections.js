$(document).ready(function () {

    var d = new Date();
    var n = d.getMonth();

    var element = $('#ddlFromYearInsp').val('2000');
    var element2 = $('#ddlToMonthInsp').val(n);

    LoadReport();

    $('#ddlStateCompleted').change(function () {

        $('#ddlDistMaintenanceInsp').empty();
        var option = document.createElement("option");
        var stateCode = parseInt($('#ddlStateCompleted').val());
        if (stateCode == 0) {
            //alert("i alert");
            $('#ddlDistMaintenanceInsp').empty(); // remove old options
            $('#ddlDistMaintenanceInsp').append($("<option></option>").attr("value", '0').text('All Districts'));
           // $("#ddlDistMaintenanceInsp").val("0");
        }
        else {
            loadDistrict(stateCode);
        }
       
    });
    
    $('#btn_View_Report').click(function () {
        LoadReport();
    });

});

function LoadReport() {
    if (CheckToFromPeriod()) {
        $.ajax({
            url: '/QMSSRSReports/QMSSRSReports/LoadMaintenanceInspReport',
            type: 'POST',
            data:  $('#frmQMMaintenanceInspDetails').serialize(),
            success: function (response) {
                if (response != null) {
                    $("#loadReport2").html(response);
                }
                else {
                    alert("An error occurred during processing");
                }
            }
            // $("#ddlDistMaintenanceInsp").remove(0);
        });
    }
    else {
        alert("Selected period of inspection is not valid");
    }
}


function loadDistrict(statCode) {
    $("#ddlDistMaintenanceInsp").empty();
    $("#ddlDistMaintenanceInsp").text('All Districts');
    $("#ddlDistMaintenanceInsp").val(0);
    if (statCode > 0) {   
            $.ajax({
                url: '/QMSSRSReports/QMSSRSReports/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    if (jsonData != null) {
                        $("#ddlDistMaintenanceInsp").append("<option value='" + 0 + "'>" + "All Districts" + "</option>");
                        for (var i = 1; i < jsonData.length; i++) {
                            $("#ddlDistMaintenanceInsp").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    }
                    else {
                        alert("Something went wrong. Please try again.");
                    }
                  
                   // $("#ddlDistMaintenanceInsp").remove(0);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
function CheckToFromPeriod() {

    var fromMonth = parseInt($('#ddlFromMonthInsp').val());
    var toMonth = parseInt($('#ddlToMonthInsp').val());

    var fromYear = parseInt($('#ddlFromYearInsp').val());
    var toYear = parseInt($('#ddlToYearInsp').val());

    if (fromYear == toYear) {
        if (fromMonth > toMonth) {
            return false;
        }
        else {
            return true;
        }
    }
    else {
        if (fromYear > toYear) {
            return false;
        }
        else {
            return true;
        }
    }
}