/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMTourDetails.js
        * Description   :   Handles events, grids in  QMTourDetails process
        * Author        :   Shyam Yadav 
        * Creation Date :   02/Feb/2015
 **/

$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmFillObservations'));

    $("#tourSubmissionDate").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        showOn: 'button',
        buttonImage: '../../Content/images/calendar_2.png',
        buttonImageOnly: true,
        minDate: $("#FlightDepartureDate").val(),
        onClose: function () {
            $(this).focus().blur();
        }
    }).attr('readonly', 'readonly');

    $("#FlightArrivalDate").addClass("pmgsy-textbox");

    $("#FlightArrivalDate").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        showOn: 'button',
        buttonImage: '../../Content/images/calendar_2.png',
        buttonImageOnly: true,
        onClose: function () {
            $(this).focus().blur();
        }
    }).attr('readonly', 'readonly');


    $("#FlightArrivalDate").datepicker("option", "minDate", $("#ScheduleMonthYearStartDate").val());


    // Commented on 09 Nov 2020 so allow future dates also
   // $("#FlightArrivalDate").datepicker("option", "maxDate", $("#CurrentDate").val());


    $('#FlightArrivalTime').timepicker({
        showLeadingZero: true,
        //onHourShow: tpStartOnHourShowCallback,
        //onMinuteShow: tpStartOnMinuteShowCallback,
        showDeselectButton: true,
        showOn: 'button',
        button: "#tmArrival",
        showCloseButton: true,
        minutes: {
            starts: 0,
            ends: 59,
            interval: 1
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });


    $("#FlightDepartureDate").addClass("pmgsy-textbox");

    $("#FlightDepartureDate").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        showOn: 'button',
        buttonImage: '../../Content/images/calendar_2.png',
        buttonImageOnly: true,
        onClose: function () {
            $(this).focus().blur();
        }
    }).attr('readonly', 'readonly');


    $("#FlightDepartureDate").datepicker("option", "minDate", $("#ScheduleMonthYearStartDate").val());


    // Commented on 09 Nov 2020 so allow future dates also
    //$("#FlightDepartureDate").datepicker("option", "maxDate", $("#CurrentDate").val());


    $('#FlightDepartureTime').timepicker({
        showLeadingZero: true,
        //onHourShow: tpStartOnHourShowCallback,
        //onMinuteShow: tpStartOnMinuteShowCallback,
        showDeselectButton: true,
        showOn: 'button',
        button: "#tmDeparture",
        showCloseButton: true,
        minutes: {
            starts: 0,
            ends: 59,
            interval: 1
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });
    
    if ($("#RoleCode").val() == 5) {
        //loadTourListGridForCQC($("#ddlTourState").val(), $("#ddlTourMonitorId").val(), $("#ddlTourFrmMonth").val(), $("#ddlTourFrmYear").val(), $("#ddlTourToMonth").val(), $("#ddlTourToYear").val());
    }
    else {
        loadTourListGrid('0', '0', $("#ddlViewScheduleMonth").val(), $("#ddlViewScheduleYear").val(), $("#ddlViewScheduleMonth").val(), $("#ddlViewScheduleYear").val());
    }

    $("#btnCancelTourDetails").click(function () {
        closeMonitorsScheduleDetails();
    });


    $("#btnSaveTourDetails").click(function () {

        if (confirm("Are you sure to submit details?")) {

            $.ajax({
                url: '/QualityMonitoring/QMTourDetails',
                type: "POST",
                cache: false,
                data: $("#tourDetailsForm").serialize(),
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
                        alert("Tour Details saved successfully.");
                        $("#tbMonitorsScheduleList").trigger('reloadGrid');
                        closeMonitorsScheduleDetails();
                    }
                    else {
                        $("#divTourDetailsError").show("slow");
                        $("#divTourDetailsError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                    }
                    unblockPage();
                }
            });//ajax call ends here

        }//confirm ends here
        else {
            return false;
        }
    });//btnSave Ends Here

   
    $("#btnUpdateTourDetails").click(function (event) {

        if (confirm("Are you sure to update details?")) {

            event.stopPropagation(); // Stop stuff happening call double avoid to action
            event.preventDefault(); // call double avoid to action
            var form_data = new FormData();

            //$.each($("input[type='file']"), function () {

            //    //var id = $(this).attr('id');
            //    var id = $('#TourReport');
            //    var objFiles = $("#id").prop("files");
            //    form_data.append(id, (objFiles[0]));
            //});

            var objTourReport = $("input#TourReport").prop("files");
            console.log(objTourReport[0]);
            form_data.append("TourReportfile", objTourReport[0]);

            var data = $("#tourDetailsForm").serializeArray();

            for (var i = 0; i < data.length; i++) {
                form_data.append(data[i].name, data[i].value);
            }

            $.ajax({
                url: '/QualityMonitoring/QMEditTourDetails',
                type: "POST",
                cache: false,
                data: form_data,//$("#tourDetailsForm").serialize(),
                contentType: false,
                processData: false,
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
                        alert("Tour Details updated successfully.");
                        $("#tbMonitorsScheduleList").trigger('reloadGrid');
                        closeMonitorsScheduleDetails();
                    }
                    else {
                        $("#divTourDetailsError").show("slow");
                        $("#divTourDetailsError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                    }
                    unblockPage();
                }
            });//ajax call ends here

        }//confirm ends here
        else {
            return false;
        }
    });//btnSave Ends Here

});


function closeDivError()
{
    $("#divTourDetailsError span:eq(1)").html('');
    $("#divTourDetailsError").hide('slow');
}


function loadTourListGrid(state, monitorCode, frmMonth, frmYear, toMonth, toYear) {

    $("#tbTourList").jqGrid('GridUnload');

    jQuery("#tbTourList").jqGrid({
        url: '/QualityMonitoring/QMTourList?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "Month and Year", "Arrival Date and Time", "Departure Date and Time", "Visiting District", "Expenditure", "Tour Claim", "Submission Date", "Edit", "Delete", "Finalize"],
        colModel: [
                            { name: 'State', index: 'State', width: 90, sortable: true, align: "left" },
                            { name: 'MonthYear', index: 'MonthYear', width: 100, sortable: false, align: "center", search: false },
                            { name: 'ArrivalDateTime', index: 'ArrivalDateTime', width: 100, sortable: false, align: "left", search: false },
                            { name: 'DepartureDateTime', index: 'DepartureDateTime', width: 100, sortable: false, align: "left", search: false },
                            { name: 'Districts', index: 'Districts', width: 100, sortable: false, align: "left", search: false },

                            { name: 'Expenditure', index: 'Expenditure', width: 70, sortable: true, align: "left" },
                            { name: 'TourReport', index: 'TourReport', width: 100, sortable: false, align: "center", search: false, formatter: AnchorFormatter },
                            { name: 'SubmissionDate', index: 'SubmissionDate', width: 70, sortable: false, align: "center", search: false },

                            { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", search: false },
                            { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "left", search: false },
                            { name: 'Finalize', index: 'Finalize', width: 40, sortable: false, align: "center", search: false }
        ],
        postData: { "state": state, "monitorCode": monitorCode, "frmMonth": frmMonth, "frmYear": frmYear, "toMonth": toMonth, "toYear": toYear },
        pager: jQuery('#dvTourListPager'),
        rowNum: 10000,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Tour List",
        pgbuttons: false,
        pgtext: null,
        height: '200',
        autowidth: true,
        sortname: 'State',
        //grouping: true,
        //groupingView: {
        //    groupField: ['State', 'District'],
        //    groupText: ['<b>{0}</b>', '<b>{0}</b>'],
        //    groupColumnShow: [false, false],
        //    groupCollapse: false
        //},
        loadComplete: function () {
            unblockPage();
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
        }
    }); //end of grid
}


function AnchorFormatter(cellvalue, options, rowObject) {
    var url = "/QualityMonitoring/DownloadFileTour/" + cellvalue;
    return cellvalue == "-" ? "-" : "<a href='#' onclick=downloadFileFromAction('" + url + "'); return false;> <img style='height:16px;width:16px' height='16' width='16' border=0 src='../../Content/images/PDF.ico' /> </a>";
}

function downloadFileFromAction(paramurl) {
    //window.location = paramurl;

    $.get(paramurl).done(function (response) {
        if (response.Success == 'false') {
            alert('File Not Found.');
            return false;

        }
        else if (response.Success === undefined) {
            window.location = paramurl;
        }
    });
}

function QMUpdateTourDetails(tourId)
{
    jQuery('#tbMonitorsScheduleList').jqGrid('setSelection', tourId);

    $("#accordionMonitorsSchedule div").html("");
    $("#accordionMonitorsSchedule h3").html(
            "<a href='#' style= 'font-size:.9em;' >Tour Details</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeMonitorsScheduleDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordionMonitorsSchedule').show('slow', function () {
        blockPage();
        $("#divMonitorsScheduleDetails").load('/QualityMonitoring/QMEditTourDetails/' + tourId, function () {
            unblockPage();
        });
    });

    $('#divMonitorsScheduleDetails').show('slow');
    $("#divMonitorsScheduleDetails").css('height', 'auto');
}


function QMDeleteTourDetails(tourId) {
    if (confirm("Are you sure to delete the tour details?")) {
        $.ajax({
            url: '/QualityMonitoring/DeleteTourDetails/',
            type: 'POST',
            data: { tourId: tourId, value: Math.random() },
            success: function (response) {
                if (response.Success) {
                    alert("Tour Details deleted successfully");
                    $("#tbMonitorsScheduleList").trigger("reloadGrid");
                    closeMonitorsScheduleDetails();
                }
                else {
                    $("#divTourDetailsError").show("slow");
                    $("#divTourDetailsError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
    else {
        return false;
    }
}



function QMFinalizeTourDetails(tourId) {
    if (confirm("Are you sure to finalize the tour details?")) {
        $.ajax({
            url: '/QualityMonitoring/FinalizeTourDetails/',
            type: 'POST',
            data: { tourId: tourId, value: Math.random() },
            success: function (response) {
                if (response.Success) {
                    alert("Tour Details finalized successfully");
                    $("#tbMonitorsScheduleList").trigger("reloadGrid");
                    closeMonitorsScheduleDetails();
                }
                else {
                    $("#divTourDetailsError").show("slow");
                    $("#divTourDetailsError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
    else {
        return false;
    }
}



