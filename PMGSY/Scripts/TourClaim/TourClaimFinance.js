$(document).ready(function () {
    showNQMScheduleListGrid($("#ddlNQMScheduleMonth").val(), $("#ddlNQMScheduleYear").val());

    $('#btnViewMonitorsList ').click(function () {
        showNQMScheduleListGrid($("#ddlNQMScheduleMonth").val(), $("#ddlNQMScheduleYear").val());
    });
});

function showNQMScheduleListGrid(month, year) {

    $('#divFinanceMonitor').show();
    $("#tbFinanceMonitorList").jqGrid('GridUnload');

    jQuery("#tbFinanceMonitorList").jqGrid({
        url: '/TourClaim/GetFinanceMonitorList?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ['Tour Claim Id', 'Month & Year of visit', 'Monitor Name', 'Date of Claim', 'Total Claim Amount', 'Total Proposed Amount by CQC', 'Status', 'View/Edit/Forward'],
        colModel: [
            { name: 'TourClaimId', index: 'TourClaimId', width: 100, sortable: false, align: "center", search: false, hidden: true },
            { name: 'MonthYearOfVisit', index: 'MonthYearOfVisit', width: 100, sortable: false, align: "center", search: false },
            { name: 'MonitorName', index: 'MonitorName', width: 100, sortable: false, align: "center", search: false },
            { name: 'Date', index: 'Date', width: 130, sortable: false, align: "center", search: false },
            { name: 'TotalClaim', index: 'TotalClaim', width: 130, sortable: false, align: "center", search: false },
            { name: 'TotalPass', index: 'TotalPass', width: 130, sortable: false, align: "center", search: false },
            { name: 'Status', index: 'Status', width: 120, sortable: false, align: "center", search: false },
            { name: 'ViewEdit', index: 'ViewEdit', width: 120, sortable: false, align: "center", search: false },
        ],
        postData: { "month": month, "year": year },
        pager: jQuery('#pagerFinanceMonitor'),
        rowNum: 10,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Tour Claim Details",
        height: 'auto',
        autowidth: true,
        sortname: 'Monitor',
        rowList: [10, 20, 25],
        loadComplete: function () {
            $("#tbFinanceMonitorList #pagerFinanceMonitor").css({ height: '40px' });

            var windowWidth = window.innerWidth;
            var grid = $("#tbFinanceMonitorList");
            grid.setGridWidth(windowWidth - 100);
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


function ViewEditFinance(tourClaimId) {

    $("#divFinanceFilters").hide();
    $("#divFinanceMonitor").hide();

    $("#accordionDetails div").html("");

    $("#accordionDetails h3").html(
        "<a href='#' style= 'font-size:large;padding:1rem;margin-left:50rem' >Tour Claim Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0;" class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeTourClaimList();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionDetails').show('slow', function () {

        blockPage();

        $("#divFinanceDetails").load('/TourClaim/ViewEditTourFinance/?tourClaimId=' + tourClaimId, function () {

            document.getElementById("defaultOpen").click();
            unblockPage();
        });
    });

    $('#divFinanceDetails').show('slow');
    $("#divFinanceDetails").css('height', 'auto');
}

function closeTourClaimList() {
    $('#showMainForm').hide();
    $('.tab').hide();
    $('#accordionDetails').hide();

    $('#divFinanceFilters').show();
    $('#divFinanceMonitor').show('slow');
    $("#tbFinanceMonitorList").trigger('reload');
}

