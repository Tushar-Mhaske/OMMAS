/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMCreateSchedule.js
        * Description   :   Handles events, grids in Creating Schedule Process
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/

$(document).ready(function () {
  
    $.validator.unobtrusive.parse($('#frmCreateSchedule'));

    
    $("#btnCancel").click(function () {
        CloseScheduleDetails();
    });

    //------------------ MultiSelect Code ----------------------------

    var arrDistricts = [];
    var arrDistCnt = 0;
    selectedNameVal = 0; //for selected Monitor Value
    //Selected items in District List
    $('#DISTRICT_LIST').multiSelect({
        selectableHeader: "<div class='ui-widget-header ui-corner-top' style='text-align:center'><strong>District to Map</strong></div>",
        selectionHeader: "<div class='ui-widget-header ui-corner-top' style='text-align:center'><strong>District to be Unmapped</strong></div>",

        afterInit: function (values) {
            $('#DISTRICT_LIST').multiSelect('deselect_all');
        },
        afterSelect: function (values) {
            arrDistricts.push(values);
        },
        afterDeselect: function (values) {
            arrDistricts.pop(values);
        }

    });

   
    $('#DISTRICT_LIST').multiSelect('refresh');


    //-------------------------------------------------------------
  

    $("#ADMIN_IM_MONTH").change(function () {

        $("#createSchMonitorCode").val(0);
        $("#createSchMonitorCode").empty();
        
        $("#createSchMonitorCode").append("<option value='0'>Select Monitor</option>");

        if ($("#ADMIN_IM_MONTH").val() > 0) {

            if ($("#createSchMonitorCode").length > 0) {

                $.ajax({
                    url: '/QualityMonitoring/GetMonitors',
                    type: 'POST',
                    data: { month: $("#ADMIN_IM_MONTH").val(), year: $("#ADMIN_IM_YEAR").val(),  value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#createSchMonitorCode").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }
    });//ADMIN_IM_MONTH change ends here


    $("#ADMIN_IM_YEAR").change(function () {

        $("#createSchMonitorCode").val(0);
        $("#createSchMonitorCode").empty();
       
        $("#createSchMonitorCode").append("<option value='0'>Select Monitor</option>");

        if ($("#ADMIN_IM_YEAR").val() > 0) {

            if ($("#createSchMonitorCode").length > 0) {

                $.ajax({
                    url: '/QualityMonitoring/GetMonitors',
                    type: 'POST',
                    data: { month: $("#ADMIN_IM_MONTH").val(), year: $("#ADMIN_IM_YEAR").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#createSchMonitorCode").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }
    });//ADMIN_IM_YEAR change ends here

   
    $("#createSchMonitorCode").change(function () {
        
        $("#stateCode").val(0);
        $("#stateCode").empty();
        if ($(this).val() == 0) {
            $("#stateCode").append("<option value='0'>Select State</option>");
        }

        //$("#showMonitorError").html("");
        //$("#showMonitorError").removeClass("field-validation-error");

        if ($("#createSchMonitorCode").val() > 0) {

            if ($("#stateCode").length > 0) {

                $.ajax({
                    url: '/QualityMonitoring/GetMonitorStates',
                    type: 'POST',
                    data: { selectedMonitor: $("#createSchMonitorCode").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#stateCode").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }

    });//monitorCode change ends here


  
    //State Code Change
    $("#stateCode").change(function () {

        $("#DISTRICT_LIST").val(0);
        $("#DISTRICT_LIST").empty();
        if ($(this).val() == 0) {
            $("#DISTRICT_LIST").append("<option value='0'></option>");
            $('#DISTRICT_LIST').multiSelect('deselect_all');
            $('#DISTRICT_LIST').multiSelect('refresh');
        }

        if ($("#stateCode").val() > 0) {

            if ($("#DISTRICT_LIST").length > 0) {

                $.ajax({
                    url: '/QualityMonitoring/GetDistricts',
                    type: 'POST',
                    data: { selectedState: $("#stateCode").val(), month: $("#ADMIN_IM_MONTH").val(), year: $("#ADMIN_IM_YEAR").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#DISTRICT_LIST").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }

                        //$('#MAST_DISTRICT_CODE').multiSelect('select_all');
                        $('#DISTRICT_LIST').multiSelect('deselect_all');
                        $('#DISTRICT_LIST').multiSelect('refresh');
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }

    });//stateCode Change Ends here

    


    //button Create Click
    $('#btnCreate').click(function (evt) {
        evt.preventDefault();
        if ($('#frmCreateSchedule').valid()) {
            if (validate(arrDistricts)) {
                if ($("#OPERATION").val() == "C") {
                    $.ajax({
                        url: '/QualityMonitoring/QMCreateSchedule',
                        type: "POST",
                        cache: false,
                        data: $("#frmCreateSchedule").serialize(),
                        beforeSend: function () {
                            blockPage();
                        },
                        error: function (xhr, status, error) {
                            unblockPage();
                            alert("Request can not be processed at this time,please try after some time!!!");
                            return false;
                        },

                        success: function (response) {
                            if (response.Success) {
                                alert("Schedule created succesfully.");
                                $("#tb3TierScheduleList").trigger("reloadGrid");
                                resetDistrictList();
                                //CloseScheduleDetails();
                            }
                            else {
                                $("#divError").show("slow");
                                $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                            }
                            unblockPage();
                        }
                    });
                }
            }
            else {
                $('.qtip').show();
            }
        }
    });//btnCreate ends here


}); //doc.ready ends here


//resets comma seperated District List
function resetDistrictList()
{
    arrDistricts = [];
    $('#DISTRICT_LIST').multiSelect('deselect_all');
    $('#DISTRICT_LIST').multiSelect('refresh');
    $("#ASSIGNED_DISTRICT_LIST").val("");
}


function validate(arrDistricts)
{
    //arrDistricts.forEach(alert(elem));
    if (arrDistricts.length == 0)
    {
        $("#showDistrictError").html("Select at least one of the Districts");
        $("#showDistrictError").addClass("field-validation-error");
        return false;
    }

    //if ($("#hdnRoleCode").val() == 5)
    if ($("#hdnRoleCode").val() == 9)
    {
        if (arrDistricts.length > 3) {
            $("#showDistrictError").html("Number of district to be visited should be 3 or less than 3");
            $("#showDistrictError").addClass("field-validation-error");
            return false;
        }
    }
    else if ($("#hdnRoleCode").val() == 8 || $("#hdnRoleCode").val() == 69) {
        if (arrDistricts.length > 2) {
            $("#showDistrictError").html("In a schedule, only two district can be assigned to SQM");
            $("#showDistrictError").addClass("field-validation-error");
            return false;
        }
    }
    

    //if (selectedNameVal == 0) {
    //    $("#showMonitorError").html("The Monitor field is required.");
    //    $("#showMonitorError").addClass("field-validation-error");
    //    return false;
    //}
    //else {
    //    $("#showMonitorError").html("");
    //    $("#showMonitorError").removeClass("field-validation-error");
    //}

    $("#showDistrictError").html("");
    $("#showDistrictError").removeClass("field-validation-error");

    //$("#monitorCode").val(selectedNameVal).val(); //assign selected monitorCode

    var assignedDist = "";
    for (var i = 0; i < arrDistricts.length; ++i) {
        if(i == 0)
        {
            $("#ASSIGNED_DISTRICT_LIST").val(arrDistricts[i]);
        }
        else
        {
            $("#ASSIGNED_DISTRICT_LIST").val($("#ASSIGNED_DISTRICT_LIST").val() + "," + arrDistricts[i]);
        }
       
    }

    return true;
}






