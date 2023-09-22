/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QualityFilters.js
        * Description   :   Handles events for Filters in Quality module for Tour Details
        * Author        :   Shyam Yadav 
        * Creation Date :   12/Feb/2015
 **/

$(document).ready(function () {
    $.validator.unobtrusive.parse($('#tourFiltersForm'));


    selectedNameVal = 0;
    $("#ddlTourState").change(function () {

        $("#ddlTourMonitorId").empty();
        populateNQMs();
        if ($(this).val() == 0) {

        }
        else {
            if ($("#ddlTourMonitorId").length > 0) {

                $.ajax({
                    url: '/QualityMonitoring/GetNQMNames',
                    type: 'POST',
                    data: { selectedState: $("#ddlTourState").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#ddlTourMonitorId").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }
    });


    $('#btnViewTourDetails').click(function () {

        loadTourListGridForCQC($("#ddlTourState").val(), $("#ddlTourMonitorId").val(), $("#ddlTourFrmMonth").val(), $("#ddlTourFrmYear").val(), $("#ddlTourToMonth").val(), $("#ddlTourToYear").val());

    });

    loadTourListGridForCQC($("#ddlTourState").val(), $("#ddlTourMonitorId").val(), $("#ddlTourFrmMonth").val(), $("#ddlTourFrmYear").val(), $("#ddlTourToMonth").val(), $("#ddlTourToYear").val());

});//doc.ready ends here


function populateNQMs() {
    $.ajax({
        url: '/QualityMonitoring/PopulateNQM',
        type: 'POST',
        data: { frmMonth: $("#ddlTourFrmMonth").val(), frmYear: $("#ddlTourFrmYear").val(), toMonth: $("#ddlTourToMonth").val(), toYear: $("#ddlTourToYear").val(), value: Math.random() },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                $("#ddlTourMonitorId").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}



function loadTourListGridForCQC(state, monitorCode, frmMonth, frmYear, toMonth, toYear) {

    $("#tbTourList").jqGrid('GridUnload');

    jQuery("#tbTourList").jqGrid({
        url: '/QualityMonitoring/QMTourList?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "State", "Month and Year", "Arrival Date and Time", "Departure Date and Time", "Visiting District", "DeFinalize"],
        colModel: [
                            { name: 'Monitor', index: 'Monitor', width: 90, sortable: true, align: "left" },
                            { name: 'State', index: 'State', width: 90, sortable: false, align: "left" },
                            { name: 'MonthYear', index: 'MonthYear', width: 100, sortable: false, align: "center", search: false },
                            { name: 'ArrivalDateTime', index: 'ArrivalDateTime', width: 100, sortable: false, align: "left", search: false },
                            { name: 'DepartureDateTime', index: 'DepartureDateTime', width: 100, sortable: false, align: "left", search: false },
                            { name: 'Districts', index: 'Districts', width: 100, sortable: false, align: "left", search: false },
                            { name: 'DeFinalize', index: 'DeFinalize', width: 40, sortable: false, align: "left", search: false }
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
        sortname: 'Monitor',
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


function QMDeFinalizeTourDetails(tourId) {
    if (confirm("Are you sure to DeFinalize the tour details?")) {
        $.ajax({
            url: '/QualityMonitoring/DeFinalizeTourDetails/',
            type: 'POST',
            data: { tourId: tourId, value: Math.random() },
            success: function (response) {
                if (response.Success) {
                    alert("Tour Details Definalized successfully");
                    $("#tbTourList").trigger("reloadGrid");
                    //closeMonitorsScheduleDetails();
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