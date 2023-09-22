jQuery.validator.addMethod("customlengthvalidator", function (value, element, param) {

    var a = $("#startChainage").val();
    var b = $("#endChainage").val();

    if (a == "" && b == "") {
        $("#lblErrMsg").show();
        $("#lblErrMsg").text("Please enter start and end chainage");
        return false;
    }
    else if (parseFloat($('#txtCompletedLength')) < parseFloat(a)) {

        $("#lblErrMsg").show();
        $("#lblErrMsg").text("Please select Search By Details Type");
    }

    else if (parseFloat($('#txtCompletedLength')) > parseFloat(b)) {
        $("#lblErrMsg").show();
        $("#lblErrMsg").text("Telephone number should be a 9-13 digit number");
        return false;
    }
    else {
        return true;
    }
    return false;
});

jQuery.validator.unobtrusive.adapters.addBool("customlengthvalidator"/*, function (options) {
    options.rules["customvalidator"] = "#" + options.params.requiredif;
    options.messages["customvalidator"] = options.message;
}*/);

$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmEditTechnologyProgressDetails');

    var date = new Date();
    //var startDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);

    var cur = date.getDate();
    //var startDate = parseInt(currDay) <= 5 ? new Date(currentDate.getFullYear(), parseInt(currentDate.getMonth()) == 1 ? 12 : parseInt(currentDate.getMonth() - 1), 1) : new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
    //var startDate = parseInt(currDay) <= 10 ? new Date(currentDate.getFullYear(), parseInt(currentDate.getMonth()) == 1 ? 12 : parseInt(currentDate.getMonth() - 1), 1) : new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);

    //Change by Avinash15 on 11/04/2019 Relaxation for April Month to 15..Prev it was 10.
    //var startDate = parseInt(currDay) <= 10 ? new Date(parseInt(currentDate.getMonth()) == 0 ? parseInt(currentDate.getMonth() - 1) : currentDate.getFullYear(), parseInt(currentDate.getMonth()) == 0 ? 12 : parseInt(currentDate.getMonth() - 1), 1) : new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);

    var startDate = date.getMonth() == 3 ? (parseInt(cur) <= 10 ? new Date(parseInt(date.getMonth()) == 0 ? parseInt(date.getMonth() - 1) : date.getFullYear(), parseInt(date.getMonth()) == 0 ? 12 : parseInt(date.getMonth() - 1), 1) : new Date(date.getFullYear(), date.getMonth(), 1))
        : (parseInt(cur) <= 5 ? new Date(parseInt(date.getMonth()) == 0 ? parseInt(date.getMonth() - 1) : date.getFullYear(), parseInt(date.getMonth()) == 0 ? 12 : parseInt(date.getMonth() - 1), 1) : new Date(date.getFullYear(), date.getMonth(), 1));
 /*   alert("startDate " + startDate)*/
  //  var startDate = parseInt(currDay) <= 30 ? new Date(currentDate.getFullYear(), parseInt(currentDate.getMonth()) == 1 ? 12 : parseInt(currentDate.getMonth() - 1), 1) : new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);

    var lastDay = new Date(date.getFullYear(), date.getMonth(), 0);
 /*   alert("lastDay " + lastDay)*/
    var maxDate = (date.getMonth() == 3 && parseInt(cur) <= 13) ? lastDay : 0;    // change
 /*   alert("maxDate " + maxDate)*/

    //var currentYear = currentDate.getFullYear();
    //var currentMonth = currentDate.getMonth();

    var currentTime = new Date();
    // First Date Of the month 
    var startDateFrom = new Date(currentTime.getFullYear(), currentTime.getMonth(), 1);
    //// Last Date Of the Month 
    var startDateToend = new Date(currentTime.getFullYear(), currentTime.getMonth() + 1, 0);

    if (currentTime.getMonth() + 1 == 4 && currentTime.getDate() > 13)    // change
    {

        $('#Date').datepicker({
            dateFormat: 'dd/mm/yy',
            showOn: "button",
            buttonImage: "/Content/Images/calendar_2.png",
            showButtonText: 'Choose a start date',
            buttonImageOnly: true,
            buttonText: 'Start Date',
            changeMonth: true,
            changeYear: true,
            //minDate: $('#agreementDate').val(),
            //maxDate: new Date(),
            maxDate: 0,
            minDate: startDateFrom,
            //minDate: new Date(currentYear, currentMonth, currentDate),
            onSelect: function (selectedDate) {
                //$("#txtNewsPublishEnd").datepicker("option", "minDate", selectedDate);
                //$(function () {
                //    $('#txtNewsPublishSt').focus();
                //    $('#txtNewsPublishEnd').focus();
                //})
                $('#Date').trigger('blur');
            }
        });
    }
    else
    {

             $('#Date').datepicker({
            dateFormat: 'dd/mm/yy',
            showOn: "button",
            buttonImage: "/Content/Images/calendar_2.png",
            showButtonText: 'Choose a start date',
            buttonImageOnly: true,
            buttonText: 'Start Date',
            changeMonth: true,
            changeYear: true,
            //minDate: $('#agreementDate').val(),
            //maxDate: new Date(),
            maxDate: maxDate,
            minDate: lastDay,
            //minDate: new Date(currentYear, currentMonth, currentDate),
            //onSelect: function (selectedDate) {
            //    //$("#txtNewsPublishEnd").datepicker("option", "minDate", selectedDate);
            //    //$(function () {
            //    //    $('#txtNewsPublishSt').focus();
            //    //    $('#txtNewsPublishEnd').focus();
            //    //})
            //    $('#Date').trigger('blur');
            //}
        });
    }


    //$('#Date').datepicker({
    //    dateFormat: 'dd/mm/yy',
    //    showOn: "button",
    //    buttonImage: "/Content/Images/calendar_2.png",
    //    showButtonText: 'Choose a start date',
    //    buttonImageOnly: true,
    //    buttonText: 'Start Date',
    //    changeMonth: true,
    //    changeYear: true,
    //    //minDate: $('#agreementDate').val(),
    //    //maxDate: new Date(),
    //    maxDate: maxDate,
    //    minDate: startDate,
    //    //minDate: new Date(currentYear, currentMonth, currentDate),
    //    onSelect: function (selectedDate) {
    //        //$("#txtNewsPublishEnd").datepicker("option", "minDate", selectedDate);
    //        //$(function () {
    //        //    $('#txtNewsPublishSt').focus();
    //        //    $('#txtNewsPublishEnd').focus();
    //        //})
    //        $('#Date').trigger('blur');
    //    }
    //});



    //$(function () {
    //    $("#Date").datepicker("option", "minDate", $('#agreementDate').val());
    //    $("#Date").datepicker("option", "maxDate", new Date());
    //});

    if ($('#flg').val() == 'True' && $('#Operation').val() == 'A') {
        $('#divAddEditTechnologyProgressDetails').hide('slow');
    }

    //if ($('#Operation').val() != 'E') {
    //LoadExecTechnologyList($('#IMS_PR_ROAD_CODE').val());
    //}

    $('#ddlYear').change(function () {
        var curDate = new Date();
        var year = curDate.getFullYear();
        var month = curDate.getMonth();
        month = parseInt(month) + 1;
        if (parseInt($('#ddlYear').val()) == parseInt(year) - 1) {
            $('#ddlMonth').val(parseInt(month == 1 ? 12 : parseInt(month) - 1));
        }
        else {
            $('#ddlMonth').val(parseInt(month));
        }
    });

    $('#ddlMonth').change(function () {
        var curDate = new Date();
        var year = curDate.getFullYear();
        var month = curDate.getMonth();
        month = parseInt(month) + 1;

        if (parseInt(month) == 1) {
            if (parseInt($('#ddlMonth option:selected').val()) == 12) {
                $('#ddlYear').val(parseInt(year) - 1);
            }
            else {
                $('#ddlYear').val(parseInt(year));
            }
        }
    });

    $('#btnAddTechnologyDetails').click(function () {
        //alert($('#EncryptedRoadCode').val());
        //alert($('#frmEditTechnologyProgressDetails').valid());

        var status = $('#ddlStatus option:selected').val();

        if ($('#frmEditTechnologyProgressDetails').valid()) {
            $.ajax({
                url: "/Execution/AddTechnologyProgressDetails/",
                cache: false,
                type: "POST",
                async: false,
                data: $("#frmEditTechnologyProgressDetails").serialize(),
                success: function (data) {

                    if (data.success == true) {
                        alert(data.message);

                        $('#tbExecTechnologyList').trigger('reloadGrid');
                        $('#divAddEditTechnologyProgressDetails').html('');
                        $('#divAddEditTechnologyProgressDetails').hide('slow');
                        //alert($('#EncryptedRoadCode').val());
                        //LoadExecTechnologyList($('#IMS_PR_ROAD_CODE').val());
                        //alert(status);

                        //setTimeout(function () {
                        //    if (status == 'C') {
                        //        $('#idAddTechnology').css("display", "none");
                        //    }
                        //    else {
                        //        $('#idAddTechnology').css("display", "block");
                        //    }
                        //}, 2000);
                        
                    }
                    else {
                        //alert("Error occured while adding Technology Progress Details.");
                        //alert(data.message);
                        $("#divError").show();
                        $("#divError").html('<strong>Alert : </strong>' + data.message);
                    }
                },
                error: function () {
                    alert("error");
                }
            })
        }
    });

    $('#btnUpdateTechnologyDetails').click(function () {
        var status = $('#ddlStatus option:selected').val();
        if ($('#frmEditTechnologyProgressDetails').valid()) {
            $.ajax({
                url: "/Execution/EditTechnologyProgressDetails/",
                cache: false,
                type: "POST",
                async: false,
                data: $("#frmEditTechnologyProgressDetails").serialize(),
                success: function (data) {
                    //alert(data.success);
                    if (data.success == true) {
                        alert(data.message);

                        $('#tbExecTechnologyList').trigger('reloadGrid');
                        $('#divAddEditTechnologyProgressDetails').html('');
                        $('#divAddEditTechnologyProgressDetails').hide('slow');
                        //alert($('#IMS_PR_ROAD_CODE').val());
                        //LoadExecTechnologyList($('#IMS_PR_ROAD_CODE').val());

                        //alert(status);
                        //setTimeout(function () {
                        //    if (status == 'C') {
                        //        $('#idAddTechnology').css("display", "none");
                        //    }
                        //    else {
                        //        $('#idAddTechnology').css("display", "block");
                        //    }
                        //}, 2000);

                    }
                    else {
                        //alert("Error occured while adding Technology Progress Details.");
                        $("#divError").show();
                        $("#divError").html('<strong>Alert : </strong>' + data.message);
                    }

                },
                error: function () {
                    alert("error");
                }
            })
        }
    });

    $('#imgCloseProgressDetails').click(function () {
        $('#divAddEditTechnologyProgressDetails').hide();
        $('#divAddEditTechnologyProgressDetails').html('');
    });

    $('#btnCancelTechnologyDetails').click(function () {
        $('#divAddEditTechnologyProgressDetails').hide();
        $('#divAddEditTechnologyProgressDetails').html('');
    });

    //$('#ddlStatus').change(function () {
    //    alert($('#idAddTechnology').is('visible'));
    //    alert($('#ddlStatus').val());
    //    //if ($('#ddlStatus option:selected') === 'C') {
    //    if ($('#ddlStatus').val() === 'C') {
    //        $('#idAddTechnology').css("display", "none");
    //        //$('.jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only').hide('slow');
    //    }
    //    else {
    //        //$('.jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only').show('slow');
    //        $('#idAddTechnology').css("display", "block");
    //    }
    //});
});


function AddEditTechnologyDetails(urlparamater) {
    //jQuery("#tbExecTechnologyList").jqGrid('GridUnload');
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/Execution/AddEditTechnologyProgressDetails/" + urlparamater,
        type: "GET",
        dataType: "html",
        async: false,
        cache: false,
        data: { Operation: 'E', },
        success: function (data) {
            //$('#divAddEditTechnologyProgressDetails').html('');
            //$('#divAddEditTechnologyProgressDetails').html(data);
            //$('#divAddEditTechnologyProgressDetails').show('slow');

            $('#tbExecTechnologyList').trigger('reloadGrid');

            $('#divAddTechnologyProgressDetails').html('');
            $('#divAddTechnologyProgressDetails').html(data);
            $('#divAddTechnologyProgressDetails').show('slow');
            //$('#idAddTechnology').hide('slow');
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }
    });
}

function DeleteTechnologyDetails(urlparamater) {

    //alert(paramater);
    var status = $('#ddlStatus option:selected').val();

    if (confirm("Are you sure you want to delete Technology details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Execution/DeleteTechnologyDetailsViewModel/" + urlparamater,
            type: "POST",
            dataType: "json",
            //data: $("form").serialize(),
            success: function (data) {

                if (data.success) {
                    alert(data.message);
                    //$('#tbStateList').trigger('reloadGrid');

                    $('#tbExecTechnologyList').trigger('reloadGrid');
                    $('#divAddEditTechnologyProgressDetails').html('');

                    //$('#divAddEditTechnologyProgressDetails').hide('slow');
                    //alert(status);
                    //setTimeout(function () {
                    //    if (status == 'C') {
                    //        $('#idAddTechnology').css("display", "none");
                    //    }
                    //    else {
                    //        $('#idAddTechnology').css("display", "block");
                    //    }
                    //}, 2000);
                }
                else {
                    alert(data.message);
                }

                $.unblockUI();

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }

        });
    }
    else {
        return false;
    }
}