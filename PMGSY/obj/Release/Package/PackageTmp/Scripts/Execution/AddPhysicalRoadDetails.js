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
    //alert($('#changed_SanctionedLength').val());
    
    //if ($("#" + params.previousval).val() == "" && (parseFloat(value) <= (parseFloat($('#changed_SanctionedLength').val()) + (parseFloat($('#changed_SanctionedLength').val()) * 0.1)) )) {
    //    return true;
    //}
    ////if ((parseFloat($("#" + params.previousval).val()) <= parseFloat(value)) && (parseFloat(value) <= parseFloat($('#changed_SanctionedLength').val())) )
    //if ((parseFloat($("#" + params.previousval).val()) <= parseFloat(value)) && (parseFloat(value) <= (parseFloat($('#changed_SanctionedLength').val()) + (parseFloat($('#changed_SanctionedLength').val()) * 0.1))))
    //    return true;
    //else
    //    return false;

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
    //var startDate = parseInt(currDay) <= 5 ? new Date(currentDate.getFullYear(), parseInt(currentDate.getMonth()) == 1 ? 12 : parseInt(currentDate.getMonth() - 1), 1) : new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
    // changed by PP as per suggestion by pankaj Sir 28-03-2018(5==>10)
    //var startDate = parseInt(currDay) <= 10 ? new Date(currentDate.getFullYear(), parseInt(currentDate.getMonth()) == 1 ? 12 : parseInt(currentDate.getMonth() - 1), 1) : new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);

    //Change by Avinash15 on 11/04/2019 Relaxation for April Month to 15..Prev it was 10.
    var startDate = parseInt(currDay) <= 30 ? new Date(currentDate.getFullYear(), parseInt(currentDate.getMonth()) == 1 ? 12 : parseInt(currentDate.getMonth() - 1), 1) : new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
    var date = new Date();
    var firstDay = new Date(date.getFullYear(), date.getMonth() - 1, 1);
    var lastDay = new Date(date.getFullYear(), date.getMonth(), 0);

    //if max Date is Zero it is defaultly taking Today's Date
    //Change by Avinash15 on 11/04/2019 Relaxation for April Month to 15..Prev it was 10.

    //var maxDate = (date.getMonth() == 3 && parseInt(currDay) <= 10) ? lastDay : 0;
    var maxDate = (date.getMonth() == 3 && parseInt(currDay) <= 30) ? lastDay : 0;


    //alert("lastDay " + lastDay)
    //alert("firstDay " + firstDay)
    //alert("date " + date)
    //alert("maxDate " + maxDate)


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

    $('#ddlYear').change(function () {
        var curDate = new Date();
        var year = curDate.getFullYear();
        var month = curDate.getMonth();
        month = parseInt(month) + 1;
        if (parseInt($('#ddlYear').val()) == parseInt(year) - 1) {
            //$('#ddlMonth option[value=' + parseInt(month) + ']').remove();
            $('#ddlMonth').val(parseInt(month == 1 ? 12 : parseInt(month) - 1));

            //$("#ddlMonth").empty();
            //$("#ddlMonth").append("<option value=" + parseInt(month == 1 ? 12 : parseInt(month) - 1) + ">" + $('#prevmonthName').val() + "</option>");
        }
        else {
            //$('#ddlMonth').attr("disabled", false);

            //$("#ddlMonth").empty();
            //$("#ddlMonth").append("<option value=" + parseInt(month) + ">" + $('#currmonthName').val() + "</option>");
            //$('#ddlMonth option[value=' + (parseInt(month) - 1) + ']').remove();
            $('#ddlMonth').val(parseInt(month));
        }
    });

    $('#ddlMonth').change(function () {
        var curDate = new Date();
        var year = curDate.getFullYear();
        var month = curDate.getMonth();
        month = parseInt(month) + 1;

        if (parseInt(month) == 1) {
            //alert($('#ddlMonth option:selected').val());
            if (parseInt($('#ddlMonth option:selected').val()) == 12) {
                $('#ddlYear').val(parseInt(year) - 1);
            }
            else {
                $('#ddlYear').val(parseInt(year));
            }
        }
    });

    $("#lblCompletionDate").hide();
    $("#lblRequired").hide();
    $("#tdCompletionDate").hide();
    //$("#txtCompletionDate").hide();
    $('#imgCloseProgressDetails').click(function () {

        $("#divAddRoadProgress").hide("slow");
        $("#divError").hide("slow");

    });

    //NEW CODE Avinash ...Edit Case...make all text Box Non readOnly for three drop Down
    if ($("#Operation").val() == "E") {
        if ($("#ddlWorkStatus").val() == "A" || $("#ddlWorkStatus").val() == "F" || $("#ddlWorkStatus").val() == "L") {

            //$("#EXEC_EARTHWORK_SUBGRADE").attr('readonly', 'readonly');
            //$("#EXEC_PREPARATORY_WORK").attr('readonly', 'readonly');
            //$("#EXEC_SUBBASE_PREPRATION").attr('readonly', 'readonly');
            //$("#EXEC_BASE_COURSE").attr('readonly', 'readonly');
            //$("#EXEC_SURFACE_COURSE").attr('readonly', 'readonly');
            //$("#EXEC_SIGNS_STONES").attr('readonly', 'readonly');
            //$("#EXEC_CD_WORKS").attr('readonly', 'readonly');
            //$("#EXEC_LSB_WORKS").attr('readonly', 'readonly');
            //$("#EXEC_MISCELANEOUS").attr('readonly', 'readonly');
            //$("#EXEC_COMPLETED").attr('readonly', 'readonly');

            $("#EXEC_EARTHWORK_SUBGRADE").attr('readonly', false);
            $("#EXEC_PREPARATORY_WORK").attr('readonly', false);
            $("#EXEC_SUBBASE_PREPRATION").attr('readonly', false);
            $("#EXEC_BASE_COURSE").attr('readonly', false);
            $("#EXEC_SURFACE_COURSE").attr('readonly', false);
            $("#EXEC_SIGNS_STONES").attr('readonly', false);
            $("#EXEC_CD_WORKS").attr('readonly', false);
            $("#EXEC_LSB_WORKS").attr('readonly', false);
            $("#EXEC_MISCELANEOUS").attr('readonly', false);
            $("#EXEC_COMPLETED").attr('readonly', false);



            //DisableValues();
        }
    }


    //OLD CODE Before 18/03/2019
    //if ($("#Operation").val() == "E") {
    //    if ($("#ddlWorkStatus").val() == "A" || $("#ddlWorkStatus").val() == "F" || $("#ddlWorkStatus").val() == "L") {
    //        DisableValues();
    //    }
    //}



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



        //Avinash ..Save Zero For three dropDown..First Time..
        if ($("#ddlWorkStatus").val() == "F" || $("#ddlWorkStatus").val() == "A" || $("#ddlWorkStatus").val() == "L") {

            $.ajax({
                type: 'POST',
                url: '/Execution/AddPhysicalRoadDetails/',
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


        } else {

            if ($("#frmAddPhysicalRoad").valid()) {
                $.ajax({
                    type: 'POST',
                    url: '/Execution/AddPhysicalRoadDetails/',
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
        }

    });

    
    //OLD CODE before 18/03/2018

    //    if ($("#frmAddPhysicalRoad").valid()) {

    //        $.ajax({
    //            type: 'POST',
    //            url: '/Execution/AddPhysicalRoadDetails/',
    //            data: $("#frmAddPhysicalRoad").serialize(),
    //            async: false,
    //            cache: false,
    //            success: function (data) {
    //                if (data.success == true) {
    //                    alert(data.message);
    //                    $("#tbPhysicalRoadList").trigger('reloadGrid');
    //                    $("#tbExecutionList").trigger('reloadGrid');
    //                    $("#btnResetRoadDetails").trigger('click');
    //                    $("#divAddPhysicalRoad").hide();
    //                    if (isCompleted == "C") {
    //                        $("#idAddPhysicaRoad").hide();
    //                    }
    //                    else {
    //                        $("#idAddPhysicaRoad").show();
    //                    }
    //                }
    //                else if (data.success == false) {
    //                    $("#divError").show();
    //                    $("#divError").html('<strong>Alert : </strong>' + data.message);
    //                }
    //            },
    //            error: function () {
    //                alert("Request can not be processed at this time.");
    //            }

    //        })
    //    }

    //});

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
                //DisableValues();

                if ($("#Operation").val() == "A") {
                    $.ajax({
                        type: 'POST',
                        url: '/Execution/PopulateLatestPhysicalRoadProgress?RoadCode=' + $("#IMS_PR_ROAD_CODE").val(),
                        async: false,
                        cache: false,
                        dataType: 'json',
                        success: function (data) {
                            if (data.success) {

                                $("#EXEC_PREPARATORY_WORK").val(data.EXEC_PREPARATORY_WORK);
                                $("#EXEC_EARTHWORK_SUBGRADE").val(data.EXEC_EARTHWORK_SUBGRADE);
                                $("#EXEC_SUBBASE_PREPRATION").val(data.EXEC_SUBBASE_PREPRATION);
                                $("#EXEC_BASE_COURSE").val(data.EXEC_BASE_COURSE);
                                $("#EXEC_SURFACE_COURSE").val(data.EXEC_SURFACE_COURSE);
                                $("#EXEC_SIGNS_STONES").val(data.EXEC_SIGNS_STONES);
                                $("#EXEC_CD_WORKS").val(data.EXEC_CD_WORKS);
                                $("#EXEC_LSB_WORKS").val(data.EXEC_LSB_WORKS);
                                $("#EXEC_MISCELANEOUS").val(data.EXEC_MISCELANEOUS);
                                $("#EXEC_COMPLETED").val(data.EXEC_COMPLETED);

                                //Avinash   Do Not Disable in Case of F:Pending:Forest Clearance   A:Pending:Land Acquisition  L:Pending:Legal cases
                                $("#EXEC_EARTHWORK_SUBGRADE").attr('readonly', 'readonly');
                                $("#EXEC_PREPARATORY_WORK").attr('readonly', 'readonly');
                                $("#EXEC_SUBBASE_PREPRATION").attr('readonly', 'readonly');
                                $("#EXEC_BASE_COURSE").attr('readonly', 'readonly');
                                $("#EXEC_SURFACE_COURSE").attr('readonly', 'readonly');
                                $("#EXEC_SIGNS_STONES").attr('readonly', 'readonly');
                                $("#EXEC_CD_WORKS").attr('readonly', 'readonly');
                                $("#EXEC_LSB_WORKS").attr('readonly', 'readonly');
                                $("#EXEC_MISCELANEOUS").attr('readonly', 'readonly');
                                $("#EXEC_COMPLETED").attr('readonly', 'readonly');

                            }
                            else {
                                alert("Error in processing,Please try Again");
                            }
                        },
                        error: function () {
                            alert("Error in processing,Please try Again");
                        }
                    });

                } else {



                    //$("#EXEC_EARTHWORK_SUBGRADE").attr('readonly', 'readonly');
                    //$("#EXEC_PREPARATORY_WORK").attr('readonly', 'readonly');
                    //$("#EXEC_SUBBASE_PREPRATION").attr('readonly', 'readonly');
                    //$("#EXEC_BASE_COURSE").attr('readonly', 'readonly');
                    //$("#EXEC_SURFACE_COURSE").attr('readonly', 'readonly');
                    //$("#EXEC_SIGNS_STONES").attr('readonly', 'readonly');
                    //$("#EXEC_CD_WORKS").attr('readonly', 'readonly');
                    //$("#EXEC_LSB_WORKS").attr('readonly', 'readonly');
                    //$("#EXEC_MISCELANEOUS").attr('readonly', 'readonly');
                    //$("#EXEC_COMPLETED").attr('readonly', 'readonly');

                    $("#EXEC_EARTHWORK_SUBGRADE").attr('readonly',false);
                    $("#EXEC_PREPARATORY_WORK").attr('readonly', false);
                    $("#EXEC_SUBBASE_PREPRATION").attr('readonly', false);
                    $("#EXEC_BASE_COURSE").attr('readonly', false);
                    $("#EXEC_SURFACE_COURSE").attr('readonly', false);
                    $("#EXEC_SIGNS_STONES").attr('readonly', false);
                    $("#EXEC_CD_WORKS").attr('readonly', false);
                    $("#EXEC_LSB_WORKS").attr('readonly', false);
                    $("#EXEC_MISCELANEOUS").attr('readonly', false);
                    $("#EXEC_COMPLETED").attr('readonly', false);


                }

                break;
            case 'A':
                HideDateOption();
                //DisableValues();
                if ($("#Operation").val() == "A") {
                    $.ajax({
                        type: 'POST',
                        url: '/Execution/PopulateLatestPhysicalRoadProgress?RoadCode=' + $("#IMS_PR_ROAD_CODE").val(),
                        async: false,
                        cache: false,
                        dataType: 'json',
                        success: function (data) {
                            if (data.success) {

                                $("#EXEC_PREPARATORY_WORK").val(data.EXEC_PREPARATORY_WORK);
                                $("#EXEC_EARTHWORK_SUBGRADE").val(data.EXEC_EARTHWORK_SUBGRADE);
                                $("#EXEC_SUBBASE_PREPRATION").val(data.EXEC_SUBBASE_PREPRATION);
                                $("#EXEC_BASE_COURSE").val(data.EXEC_BASE_COURSE);
                                $("#EXEC_SURFACE_COURSE").val(data.EXEC_SURFACE_COURSE);
                                $("#EXEC_SIGNS_STONES").val(data.EXEC_SIGNS_STONES);
                                $("#EXEC_CD_WORKS").val(data.EXEC_CD_WORKS);
                                $("#EXEC_LSB_WORKS").val(data.EXEC_LSB_WORKS);
                                $("#EXEC_MISCELANEOUS").val(data.EXEC_MISCELANEOUS);
                                $("#EXEC_COMPLETED").val(data.EXEC_COMPLETED);

                                //Avinash   Do Not Disable in Case of F:Pending:Forest Clearance   A:Pending:Land Acquisition  L:Pending:Legal cases
                                $("#EXEC_EARTHWORK_SUBGRADE").attr('readonly', 'readonly');
                                $("#EXEC_PREPARATORY_WORK").attr('readonly', 'readonly');
                                $("#EXEC_SUBBASE_PREPRATION").attr('readonly', 'readonly');
                                $("#EXEC_BASE_COURSE").attr('readonly', 'readonly');
                                $("#EXEC_SURFACE_COURSE").attr('readonly', 'readonly');
                                $("#EXEC_SIGNS_STONES").attr('readonly', 'readonly');
                                $("#EXEC_CD_WORKS").attr('readonly', 'readonly');
                                $("#EXEC_LSB_WORKS").attr('readonly', 'readonly');
                                $("#EXEC_MISCELANEOUS").attr('readonly', 'readonly');
                                $("#EXEC_COMPLETED").attr('readonly', 'readonly');

                            }
                            else {
                                alert("Error in processing,Please try Again");
                            }
                        },
                        error: function () {
                            alert("Error in processing,Please try Again");
                        }
                    });

                } else {

                    //$("#EXEC_EARTHWORK_SUBGRADE").attr('readonly', 'readonly');
                    //$("#EXEC_PREPARATORY_WORK").attr('readonly', 'readonly');
                    //$("#EXEC_SUBBASE_PREPRATION").attr('readonly', 'readonly');
                    //$("#EXEC_BASE_COURSE").attr('readonly', 'readonly');
                    //$("#EXEC_SURFACE_COURSE").attr('readonly', 'readonly');
                    //$("#EXEC_SIGNS_STONES").attr('readonly', 'readonly');
                    //$("#EXEC_CD_WORKS").attr('readonly', 'readonly');
                    //$("#EXEC_LSB_WORKS").attr('readonly', 'readonly');
                    //$("#EXEC_MISCELANEOUS").attr('readonly', 'readonly');
                    //$("#EXEC_COMPLETED").attr('readonly', 'readonly');

                    $("#EXEC_EARTHWORK_SUBGRADE").attr('readonly', false);
                    $("#EXEC_PREPARATORY_WORK").attr('readonly', false);
                    $("#EXEC_SUBBASE_PREPRATION").attr('readonly', false);
                    $("#EXEC_BASE_COURSE").attr('readonly', false);
                    $("#EXEC_SURFACE_COURSE").attr('readonly', false);
                    $("#EXEC_SIGNS_STONES").attr('readonly', false);
                    $("#EXEC_CD_WORKS").attr('readonly', false);
                    $("#EXEC_LSB_WORKS").attr('readonly', false);
                    $("#EXEC_MISCELANEOUS").attr('readonly', false);
                    $("#EXEC_COMPLETED").attr('readonly', false);

                }
                break;

            case 'L':
                HideDateOption();
                //DisableValues();
                if ($("#Operation").val() == "A") {
                    $.ajax({
                        type: 'POST',
                        url: '/Execution/PopulateLatestPhysicalRoadProgress?RoadCode=' + $("#IMS_PR_ROAD_CODE").val(),
                        async: false,
                        cache: false,
                        dataType: 'json',
                        success: function (data) {
                            if (data.success) {

                                $("#EXEC_PREPARATORY_WORK").val(data.EXEC_PREPARATORY_WORK);
                                $("#EXEC_EARTHWORK_SUBGRADE").val(data.EXEC_EARTHWORK_SUBGRADE);
                                $("#EXEC_SUBBASE_PREPRATION").val(data.EXEC_SUBBASE_PREPRATION);
                                $("#EXEC_BASE_COURSE").val(data.EXEC_BASE_COURSE);
                                $("#EXEC_SURFACE_COURSE").val(data.EXEC_SURFACE_COURSE);
                                $("#EXEC_SIGNS_STONES").val(data.EXEC_SIGNS_STONES);
                                $("#EXEC_CD_WORKS").val(data.EXEC_CD_WORKS);
                                $("#EXEC_LSB_WORKS").val(data.EXEC_LSB_WORKS);
                                $("#EXEC_MISCELANEOUS").val(data.EXEC_MISCELANEOUS);
                                $("#EXEC_COMPLETED").val(data.EXEC_COMPLETED);

                                //Avinash   Do Not Disable in Case of F:Pending:Forest Clearance   A:Pending:Land Acquisition  L:Pending:Legal cases
                                $("#EXEC_EARTHWORK_SUBGRADE").attr('readonly', 'readonly');
                                $("#EXEC_PREPARATORY_WORK").attr('readonly', 'readonly');
                                $("#EXEC_SUBBASE_PREPRATION").attr('readonly', 'readonly');
                                $("#EXEC_BASE_COURSE").attr('readonly', 'readonly');
                                $("#EXEC_SURFACE_COURSE").attr('readonly', 'readonly');
                                $("#EXEC_SIGNS_STONES").attr('readonly', 'readonly');
                                $("#EXEC_CD_WORKS").attr('readonly', 'readonly');
                                $("#EXEC_LSB_WORKS").attr('readonly', 'readonly');
                                $("#EXEC_MISCELANEOUS").attr('readonly', 'readonly');
                                $("#EXEC_COMPLETED").attr('readonly', 'readonly');

                            }
                            else {
                                alert("Error in processing,Please try Again");
                            }
                        },
                        error: function () {
                            alert("Error in processing,Please try Again");
                        }
                    });

                } else {
                    //$("#EXEC_EARTHWORK_SUBGRADE").attr('readonly', 'readonly');
                    //$("#EXEC_PREPARATORY_WORK").attr('readonly', 'readonly');
                    //$("#EXEC_SUBBASE_PREPRATION").attr('readonly', 'readonly');
                    //$("#EXEC_BASE_COURSE").attr('readonly', 'readonly');
                    //$("#EXEC_SURFACE_COURSE").attr('readonly', 'readonly');
                    //$("#EXEC_SIGNS_STONES").attr('readonly', 'readonly');
                    //$("#EXEC_CD_WORKS").attr('readonly', 'readonly');
                    //$("#EXEC_LSB_WORKS").attr('readonly', 'readonly');
                    //$("#EXEC_MISCELANEOUS").attr('readonly', 'readonly');
                    //$("#EXEC_COMPLETED").attr('readonly', 'readonly');

                    $("#EXEC_EARTHWORK_SUBGRADE").attr('readonly', false);
                    $("#EXEC_PREPARATORY_WORK").attr('readonly', false);
                    $("#EXEC_SUBBASE_PREPRATION").attr('readonly', false);
                    $("#EXEC_BASE_COURSE").attr('readonly', false);
                    $("#EXEC_SURFACE_COURSE").attr('readonly', false);
                    $("#EXEC_SIGNS_STONES").attr('readonly', false);
                    $("#EXEC_CD_WORKS").attr('readonly', false);
                    $("#EXEC_LSB_WORKS").attr('readonly', false);
                    $("#EXEC_MISCELANEOUS").attr('readonly', false);
                    $("#EXEC_COMPLETED").attr('readonly', false);


                }
                break;
            case 'C':
                ShowDateOption();

                if ($("#Operation").val() == "A") {
                    $("#EXEC_EARTHWORK_SUBGRADE").val('');
                    $("#EXEC_PREPARATORY_WORK").val('');
                    $("#EXEC_SUBBASE_PREPRATION").val('');
                    $("#EXEC_BASE_COURSE").val('');
                    $("#EXEC_SURFACE_COURSE").val('');
                    $("#EXEC_SIGNS_STONES").val('');
                    $("#EXEC_CD_WORKS").val('');
                    $("#EXEC_LSB_WORKS").val('');
                    $("#EXEC_MISCELANEOUS").val('');
                    $("#EXEC_COMPLETED").val('');

                    $("#EXEC_EARTHWORK_SUBGRADE").attr("readonly", false);
                    $("#EXEC_PREPARATORY_WORK").attr("readonly", false);
                    $("#EXEC_SUBBASE_PREPRATION").attr("readonly", false);
                    $("#EXEC_BASE_COURSE").attr("readonly", false);
                    $("#EXEC_SURFACE_COURSE").attr("readonly", false);
                    $("#EXEC_SIGNS_STONES").attr("readonly", false);
                    $("#EXEC_CD_WORKS").attr("readonly", false);
                    $("#EXEC_LSB_WORKS").attr("readonly", false);
                    $("#EXEC_MISCELANEOUS").attr("readonly", false);
                    $("#EXEC_COMPLETED").attr("readonly", false);

                } else {
                    $("#EXEC_EARTHWORK_SUBGRADE").attr("readonly", false);
                    $("#EXEC_PREPARATORY_WORK").attr("readonly", false);
                    $("#EXEC_SUBBASE_PREPRATION").attr("readonly", false);
                    $("#EXEC_BASE_COURSE").attr("readonly", false);
                    $("#EXEC_SURFACE_COURSE").attr("readonly", false);
                    $("#EXEC_SIGNS_STONES").attr("readonly", false);
                    $("#EXEC_CD_WORKS").attr("readonly", false);
                    $("#EXEC_LSB_WORKS").attr("readonly", false);
                    $("#EXEC_MISCELANEOUS").attr("readonly", false);
                    $("#EXEC_COMPLETED").attr("readonly", false);
                }

                EnableValues();
                break;
            case 'P':
                HideDateOption();

                if ($("#Operation").val() == "A") {
                    $("#EXEC_EARTHWORK_SUBGRADE").val('');
                    $("#EXEC_PREPARATORY_WORK").val('');
                    $("#EXEC_SUBBASE_PREPRATION").val('');
                    $("#EXEC_BASE_COURSE").val('');
                    $("#EXEC_SURFACE_COURSE").val('');
                    $("#EXEC_SIGNS_STONES").val('');
                    $("#EXEC_CD_WORKS").val('');
                    $("#EXEC_LSB_WORKS").val('');
                    $("#EXEC_MISCELANEOUS").val('');
                    $("#EXEC_COMPLETED").val('');

                    $("#EXEC_EARTHWORK_SUBGRADE").attr("readonly", false);
                    $("#EXEC_PREPARATORY_WORK").attr("readonly", false);
                    $("#EXEC_SUBBASE_PREPRATION").attr("readonly", false);
                    $("#EXEC_BASE_COURSE").attr("readonly", false);
                    $("#EXEC_SURFACE_COURSE").attr("readonly", false);
                    $("#EXEC_SIGNS_STONES").attr("readonly", false);
                    $("#EXEC_CD_WORKS").attr("readonly", false);
                    $("#EXEC_LSB_WORKS").attr("readonly", false);
                    $("#EXEC_MISCELANEOUS").attr("readonly", false);
                    $("#EXEC_COMPLETED").attr("readonly", false);

                } else {
                    $("#EXEC_EARTHWORK_SUBGRADE").attr("readonly", false);
                    $("#EXEC_PREPARATORY_WORK").attr("readonly", false);
                    $("#EXEC_SUBBASE_PREPRATION").attr("readonly", false);
                    $("#EXEC_BASE_COURSE").attr("readonly", false);
                    $("#EXEC_SURFACE_COURSE").attr("readonly", false);
                    $("#EXEC_SIGNS_STONES").attr("readonly", false);
                    $("#EXEC_CD_WORKS").attr("readonly", false);
                    $("#EXEC_LSB_WORKS").attr("readonly", false);
                    $("#EXEC_MISCELANEOUS").attr("readonly", false);
                    $("#EXEC_COMPLETED").attr("readonly", false);
                }
                EnableValues();
                break;
        }
    });



    //OLD CODE Before 18/03/2019
    //$("#ddlWorkStatus").change(function () {

    //    var workStatus = $("#ddlWorkStatus option:selected").val();

    //    switch (workStatus) {
    //        case 'F':
    //            HideDateOption();
    //            DisableValues();
    //            break;
    //        case 'A':
    //            HideDateOption();
    //            DisableValues();
    //            break;
    //        case 'L':
    //            HideDateOption();
    //            DisableValues();
    //            break;
    //        case 'C':
    //            ShowDateOption();
    //            EnableValues();
    //            break;
    //        case 'P':
    //            HideDateOption();
    //            EnableValues();
    //            break;
    //    }
    //});

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
    var currentTime = new Date();


    // First Date Of the month 
    var startDateFrom = new Date(currentTime.getFullYear(), currentTime.getMonth(), 1);
    // Last Date Of the Month 
    var startDateTo = new Date(currentTime.getFullYear(), currentTime.getMonth() + 1, 0);

    if (currentTime.getMonth() + 1 == 4 && currentTime.getDate() >= 5)
    {

    //alert("A")
    //alert("Month " + (currentTime.getMonth() + 1))
    //alert("Day " + currentTime.getDate())

    $('#txtCompletionDate').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        //stepMonths: 0,
        maxDate: 0, //maxDate,
        minDate: startDateFrom, //null, //startDate,
        onSelect: function (selectedDate) {
            //
        }
    });
    }
    else

    {

        //alert("B")

        //alert("Month in B" + (currentTime.getMonth() + 1))
        //alert("Day in B" + currentTime.getDate())


        $('#txtCompletionDate').datepicker({

            dateFormat: 'dd/mm/yy',
            showOn: "button",
            buttonImage: "/Content/Images/calendar_2.png",
            showButtonText: 'Choose a date',
            buttonImageOnly: true,
            changeMonth: true,
            changeYear: true,
            //stepMonths: 0,
            maxDate: maxDate, //maxDate,
            minDate: lastDay, //null, //startDate,
            //onSelect: function (selectedDate) {
            //    //
            //}
        });

      

    }



    //$('#txtCompletionDate').datepicker({

    //    dateFormat: 'dd/mm/yy',
    //    showOn: "button",
    //    buttonImage: "/Content/Images/calendar_2.png",
    //    showButtonText: 'Choose a date',
    //    buttonImageOnly: true,
    //    changeMonth: true,
    //    changeYear: true,
    //    //stepMonths: 0,
    //    maxDate: 0, //maxDate,
    //    minDate: startDateFrom, //null, //startDate,
    //    onSelect: function (selectedDate)
    //    {
    //        //
    //    }
    //});


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