$.validator.addMethod("comparepreviousstatus", function (value, element, params) {

    var newStatus = $("#ddlWorkStatus").val();
    var oldStatus = $("#CompleteStatus").val();
    var operation = $("#Operation").val();

    if (operation == "E") {
        return true;
    }

    if (oldStatus == "C") {
        return false;
    }

    return false;
});
jQuery.validator.unobtrusive.adapters.addBool("comparepreviousstatus");


$.validator.unobtrusive.adapters.add('comparepreviouslength', ['previousval'], function (options) {
    options.rules['comparepreviouslength'] = options.params;
    options.messages['comparepreviouslength'] = options.message;
});

$.validator.addMethod("comparepreviouslength", function (value, element, params) {

    if ($("#" + params.previousval).val() == "") {
        return true;
    }
    if (parseFloat($("#" + params.previousval).val()) <= parseFloat(value))
        return true;
    else
        return false;
});

$.validator.addMethod("compareisstage", function (value, element, params) {

    var stageValue = $("#IsStage").val();
    var earthWorkValue = $("#EXEC_EARTHWORK_SUBGRADE").val();
    var completedValue = $("#EXEC_COMPLETED").val();

    if (stageValue == "") {
        return true;
    }
    else if (stageValue == "S1") {

        if (parseFloat(completedValue) >= parseFloat(earthWorkValue)) {
            return true;
        }
        else {
            return false;
        }
    }
    else {
        return true;
    }
    return false;
});
jQuery.validator.unobtrusive.adapters.addBool("compareisstage");

$.validator.addMethod("comparenostage", function (value, element, params) {

    var stageValue = $("#IsStage").val();
    var surfaceValue = $("#EXEC_SURFACE_COURSE").val();
    var completedValue = $("#EXEC_COMPLETED").val();


    if (stageValue == "S2" || stageValue == null || stageValue == "") {
        if (parseFloat(surfaceValue) <= parseFloat(completedValue)) {
            return true;
        }
        else {
            return false;
        }
    }
    else {
        return true;
    }

    return false;
});
jQuery.validator.unobtrusive.adapters.addBool("comparenostage");


$.validator.addMethod("comparemiscvalue", function (value, element, params) {

    var sancValue = parseFloat($("#changed_SanctionedLength").val()) > 0 ? $("#changed_SanctionedLength").val() : $("#Sanction_length").val();
    //alert(sancValue);
    var miscValue = $("#EXEC_MISCELANEOUS").val();
    if (parseFloat(sancValue) < parseFloat(miscValue)) {
        return false;
    }
    else {
        return true;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("comparemiscvalue");

$.validator.unobtrusive.adapters.add('comparevalue', [''], function (options) {
    options.rules['comparevalue'] = options.params;
    options.messages['comparevalue'] = options.message;
});

$.validator.addMethod("comparevalue", function (value, element, params) {

    if ($("#Operation").val() == "E") {
        if (value == "")
            return false;
        else
            return true;
    }
    else {
        if (value == "" || value == "0")
            return false;
        else
            return true;
    }
});

var isCompleted;
$(document).ready(function () {

    $.validator.unobtrusive.parse("frmAddPhysicalRoad");

    var currentDate = new Date();
    var currDay = currentDate.getDate();
    var startDate = parseInt(currDay) <= 5 ? new Date(currentDate.getFullYear(), parseInt(currentDate.getMonth()) == 1 ? 12 : parseInt(currentDate.getMonth() - 1), 1) : new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);

    $(":input").bind("keypress", function (e) {
        if (e.keyvalue == 13) {
            return false;
        }
    })

    $('#ddlYear').change(function () {
        //alert($('#ddlMonth option').length);
        if (parseInt($('#ddlYear').val()) == 2015 && $('#ddlMonth option').length == "13") {
            $('#ddlMonth option').slice(1, 4).remove();
        }
        else {
            //if (parseInt($('#ddlYear').val()) > 2015 && $('#ddlMonth option').length == "10") {
            if (parseInt($('#ddlYear').val()) != 2015 && $('#ddlMonth option').length == "10") {
                $("#ddlMonth option").slice(0, 1).remove();
                $("#ddlMonth").prepend('<option value="3"> March </option>');
                $("#ddlMonth").prepend('<option value="2"> February </option>');
                $("#ddlMonth").prepend('<option value="1"> Janaury </option>');
                $("#ddlMonth").prepend('<option selected="selected" value="0"> Select Month </option>');
            }
        }
    });

    $("#lblCompletionDate").hide();
    $("#lblRequired").hide();
    $("#tdCompletionDate").hide();
    
    $('#imgCloseProgressDetails').click(function () {

        $("#divAddRoadProgress").hide("slow");
        $("#divError").hide("slow");

    });

    if ($("#Operation").val() == "E") {
        if ($("#ddlWorkStatus").val() == "A" || $("#ddlWorkStatus").val() == "F" || $("#ddlWorkStatus").val() == "L") {
            DisableValues();
        }
    }

    //save the physical road progress details
    $("#btnAddRoadDetails").click(function () {

        var curDate = new Date();
        var month = curDate.getMonth();
        month = parseInt(month) + 1;
        var year = curDate.getFullYear();
        if ($("#ddlYear").val() > year) {
            alert('Year should not be greater than current year.');
            return false;
        }

        if ($("#ddlMonth").val() > month && $("#ddlYear").val() == year) {

            alert('Month and Year exceeds the current date.');
            return false;
        }

        isCompleted = $("#ddlWorkStatus option:selected").val();

        if (isCompleted == 'C') {
            if ($("#txtCompletionDate").val() == "") {
                $("#msgDateValidation").show();
                $("#msgDateValidation").html("<span style='color:red'>Please enter Progress Completion Date.</span>");
                return false;
            }
        }

        if (isCompleted == 'C') {
            if (validateDate() == false) {
                return false;
            }
        }

        if ($("#frmAddPhysicalRoad").valid()) {

            $.ajax({
                type: 'POST',
                url: '/Execution/AddPhysicalRoadDetailsMRD/',
                data: $("#frmAddPhysicalRoad").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $("#tbPhysicalRoadList").trigger('reloadGrid');
                        $("#tbExecutionList").trigger('reloadGrid');
                        $("#btnResetRoadDetails").trigger('click');
                        $("#divAddPhysicalRoad").hide();
                        if (isCompleted == "C") {
                            $("#idAddPhysicaRoad").hide();
                        }
                        else {
                            $("#idAddPhysicaRoad").show();
                        }
                    }
                    else if (data.success == false) {
                        $("#divError").show();
                        $("#divError").html('<strong>Alert : </strong>' + data.message);
                    }
                },
                error: function () {
                    alert("Request can not be processed at this time.");
                }
            })
        }
    });

    //update road details button click
    $("#btnUpdateRoadDetails").click(function () {

        var curDate = new Date();
        var month = curDate.getMonth();
        month = parseInt(month) + 1;
        var year = curDate.getFullYear();
        if ($("#ddlYear").val() > year) {
            alert('Year should not be greater than current year.');
            return false;
        }

        if ($("#ddlMonth").val() > month && $("#ddlYear").val() == year) {

            alert('Month and Year exceeds the current date.');
            return false;
        }

        isCompleted = $("#ddlWorkStatus option:selected").val();
        if (isCompleted == 'C') {
            if ($("#txtCompletionDate").val() == "") {
                $("#msgDateValidation").show();
                $("#msgDateValidation").html("<span style='color:red'>Please enter Progress Completion Date.</span>");
                return false;
            }
        }

        if (isCompleted == 'C') {
            if (validateDate() == false) {
                return false;
            }
        }

        if ($("#frmAddPhysicalRoad").valid()) {
            $.ajax({
                type: 'POST',
                url: '/Execution/EditPhysicalRoadDetails/',
                data: $("#frmAddPhysicalRoad").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $("#tbPhysicalRoadList").trigger('reloadGrid');
                        $("#tbExecutionList").trigger('reloadGrid');
                        $("#divAddPhysicalRoad").hide();
                        if (isCompleted == "C") {
                            $("#idAddPhysicaRoad").hide();
                        }
                        else {
                            $("#idAddPhysicaRoad").show();
                        }
                    }
                    else if (data.success == false) {
                        $("#divError").show();
                        var messages = [];
                        messages = data.message.split('$');
                        for (var i = 0; i < messages.length; i++) {
                            if (i == 0) {
                                $("#divError").html('<strong>Alert : </strong>' + messages[i]);
                            }
                            else {
                                $("#divError").append('<br/><strong>Alert : </strong>' + messages[i]);
                            }
                        }
                    }
                },
                error: function () {
                    alert("Request can not be processed at this time.");
                }
            })
        }
    });

    $("#btnCancelRoadDetails").click(function () {

        $("#divAddPhysicalRoad").hide('slow');

    });

    $("#btnResetRoadDetails").click(function () {

        $("#divError").hide('slow');
        EnableValues();
    })


    $("#ddlWorkStatus").change(function () {

        var workStatus = $("#ddlWorkStatus option:selected").val();

        switch (workStatus) {
            case 'F':
                HideDateOption();
                DisableValues();
                break;
            case 'A':
                HideDateOption();
                DisableValues();
                break;
            case 'L':
                HideDateOption();
                DisableValues();
                break;
            case 'C':
                ShowDateOption();
                EnableValues();
                break;
            case 'P':
                HideDateOption();
                EnableValues();
                break;
        }
    });

    $("#ddlWorkStatus").change(function () {

        if ($("#ddlWorkStatus option:selected").val() == 'C') {
            $.ajax({

                type: 'POST',
                url: '/Execution/CheckCDWorksCount/' + $("#IMS_PR_ROAD_CODE").val(),
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success == true) {
                        alert('All CDWorks details are not entered for this road.');
                    }
                    else if (data.success == false) {

                        return false;
                    }
                },
                error: function () {
                }
            });
        }
    });

    $('#txtCompletionDate').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        maxDate: new Date(),
        minDate: startDate,
        onSelect: function (selectedDate) {

        }
    });

    if ($("#Operation").val() == "E") {
        if ($("#ddlWorkStatus option:selected").val() == 'C') {
            $("#ddlWorkStatus").trigger('change');
        }
    }

});
function DisableValues() {
    $(".pmgsy-textbox").each(function () {

        $(this).val('');
        $(this).attr('disabled', 'disabled');

    });
}
function EnableValues() {
    $(".pmgsy-textbox").each(function () {

        $(this).attr('disabled', false);

    });
}

function validateDate() {

    var paymentDate = $("#txtCompletionDate").val();
    var dateParts = paymentDate.split('/');
    var month = parseInt(dateParts[1]);
    var year = parseInt(dateParts[2]);

    if (parseInt(month) != parseInt($("#ddlMonth").val())) {
        alert('Month must match the month of completion date.');
        return false;
    }

    if (parseInt(year) != parseInt($("#ddlYear").val())) {
        alert('Year must match the year of completion date.');
        return false;
    }
}
function ShowDateOption() {
    $("#lblCompletionDate").show('slow');
    $("#lblRequired").show('slow');
    $("#txtCompletionDate").show('slow');
    $("#tdCompletionDate").show('slow');
    $(".ui-datepicker-trigger").show('slow');
}
function HideDateOption() {
    $("#lblCompletionDate").hide();
    $("#lblRequired").hide();
    $("#txtCompletionDate").hide();
    $("#tdCompletionDate").hide();
    $(".ui-datepicker-trigger").hide();
}