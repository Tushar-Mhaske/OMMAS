$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmAddNQMNAForInspectionLayout');


    
    //------------------ MultiSelect Code ----------------------------

    var arrMonitors = [];
    var arrDistCnt = 0;
    selectedNameVal = 0; //for selected Monitor Value
    //Selected items in District List
    $('#AdminQmCode').multiSelect({
        selectableHeader: "<div class='ui-widget-header ui-corner-top' style='text-align:center'><strong>Monitors to Map</strong></div>",
        selectionHeader: "<div class='ui-widget-header ui-corner-top' style='text-align:center'><strong>Monitors to be Unmapped</strong></div>",

        afterInit: function (values) {
            $('#AdminQmCode').multiSelect('deselect_all');
        },
        afterSelect: function (values) {
            arrMonitors.push(values);
        },
        afterDeselect: function (values) {
            arrMonitors.pop(values);
        }

    });
    //$('.ms-list').css('width', '300px');
    //setTimeout(function () {
    //    $('.ms-list').css('width', '300px');
    //}, 3000);
    

    $('#AdminQmCode').multiSelect('refresh');

    $('.ms-list').css('width', '300px');

    //$('#btnAddNQMDetails').click(function () {

    //    if (!$('#frmAddNQMNAForInspectionLayout').valid()) {
    //        return false;
    //    }

    //});


    //button Create Click
    $('#btnAddNQMDetails').click(function (evt) {

        //evt.preventDefault();
        if ($('#frmAddNQMNAForInspectionLayout').valid()) {
            if (validate(arrMonitors)) {
                $.ajax({
                    url: '/QualityMonitoring/AddNQMNAForInspection',
                    type: "POST",
                    cache: false,
                    data: $("#frmAddNQMNAForInspectionLayout").serialize(),
                    beforeSend: function () {
                        blockPage();
                    },
                    error: function (xhr, status, error) {
                        unblockPage();
                        alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },
                    success: function (response) {
                        if (response.success) {
                            //alert("Schedule created succesfully.");
                            alert(response.message);
                            resetMonitorList();
                            $('#btnSearch').trigger('click');
                        }
                        else {
                            //$("#divError").show("slow");
                            //$("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                            alert(response.message);
                        }
                        unblockPage();
                    }
                });
            }
            else {
                $('.qtip').show();
            }
        }
    });//btnAddNQMDetails ends here
});



//resets comma seperated District List
function resetMonitorList() {
    arrMonitors = [];
    $('#AdminQmCode').multiSelect('deselect_all');
    $('#AdminQmCode').multiSelect('refresh');
    $("#AdminQmCode").val("");
}


function validate(arrMonitors) {
    //arrMonitors.forEach(alert(elem));
    
    if (arrMonitors.length == 0) {
        //$("#showDistrictError").html("Select at least one of the Monitor");
        //$("#showDistrictError").addClass("field-validation-error");

        alert("Select at least one of the Monitor");
        return false;
    }

    //if ($("#hdnRoleCode").val() == 5)

    //if ($("#hdnRoleCode").val() == 9) {
    //    if (arrMonitors.length > 3) {
    //        $("#showDistrictError").html("Number of district to be visited should be 3 or less than 3");
    //        $("#showDistrictError").addClass("field-validation-error");
    //        return false;
    //    }
    //}
    //else if ($("#hdnRoleCode").val() == 8 || $("#hdnRoleCode").val() == 69) {
    //    if (arrMonitors.length > 1) {
    //        $("#showDistrictError").html("In a schedule, only one district can be assigned to SQM");
    //        $("#showDistrictError").addClass("field-validation-error");
    //        return false;
    //    }
    //}

    $("#showDistrictError").html("");
    $("#showDistrictError").removeClass("field-validation-error");

    //$("#monitorCode").val(selectedNameVal).val(); //assign selected monitorCode

    var assignedDist = "";
    for (var i = 0; i < arrMonitors.length; ++i) {
        if (i == 0) {
            $("#ASSIGNED_lstAdminQmCode").val(arrMonitors[i]);
        }
        else {
            $("#ASSIGNED_lstAdminQmCode").val($("#ASSIGNED_lstAdminQmCode").val() + "," + arrMonitors[i]);
        }

    }

    return true;
}