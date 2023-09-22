$(document).ready(function () {
    showNQMScheduleListGrid($("#ddlNQMScheduleMonth").val(), $("#ddlNQMScheduleYear").val());

    $('#btnViewMonitorsList ').click(function () {
        showNQMScheduleListGrid($("#ddlNQMScheduleMonth").val(), $("#ddlNQMScheduleYear").val());
    });
});

function showNQMScheduleListGrid(month, year) {
   
    $('#divFinance2Monitor').show();
    $("#tbFinance2MonitorList").jqGrid('GridUnload');

    jQuery("#tbFinance2MonitorList").jqGrid({
        url: '/TourClaim/GetFinance2MonitorList?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ['Tour Claim Id', 'Month & Year of visit', 'Monitor Name', 'Date of Claim', 'Total Claim Amount', 'Total Proposed Amount by CQC', 'Total Proposed Amount by Finance', 'View/Edit'],
        colModel: [
            { name: 'TourClaimId', index: 'TourClaimId', width: 100, sortable: false, align: "center", search: false, hidden: true },
            { name: 'MonthYearOfVisit', index: 'MonthYearOfVisit', width: 100, sortable: false, align: "center", search: false },
            { name: 'MonitorName', index: 'MonitorName', width: 100, sortable: false, align: "center", search: false },
            { name: 'Date', index: 'Date', width: 130, sortable: false, align: "center", search: false },
            { name: 'TotalClaim', index: 'TotalClaim', width: 130, sortable: false, align: "center", search: false },
            { name: 'TotalPass', index: 'TotalPass', width: 130, sortable: false, align: "center", search: false },
            { name: 'TotalPassFin1', index: 'TotalPassFin1', width: 130, sortable: false, align: "center", search: false },
            { name: 'ViewEdit', index: 'ViewEdit', width: 120, sortable: false, align: "center", search: false },
        ],
        postData: { "month": month, "year": year },
        pager: jQuery('#pagerFinance2Monitor'),
        rowNum: 10,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Tour Claim Details",
        height: 'auto',
        autowidth: true,
        sortname: 'Monitor',
        rowList: [10, 20, 25],
        loadComplete: function () {
            $("#tbFinance2MonitorList #pagerFinance2Monitor").css({ height: '40px' });

            var windowWidth = window.innerWidth;
            var grid = $("#tbFinance2MonitorList");
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


function ViewEditFinance2(tourClaimId) {

    $("#divFinance2Filters").hide();
    $("#divFinance2Monitor").hide();

    $("#accordionDetails2 div").html("");

    $("#accordionDetails2 h3").html(
        "<a href='#' style= 'font-size:large;padding:1rem;margin-left:50rem' >Tour Claim Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0;" class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeTourClaimList();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionDetails2').show('slow', function () {

        blockPage();

        $("#divFinance2Details").load('/TourClaim/ViewEditTourFinance2/?tourClaimId=' + tourClaimId, function () {

            document.getElementById("defaultOpen").click();
            unblockPage();
        });
    });

    $('#divFinance2Details').show('slow');
    $("#divFinance2Details").css('height', 'auto');
}

function closeTourClaimList() {
    $('#showMainForm').hide();
    $('.tab').hide();
    $('#accordionDetails2').hide();

    $('#divFinance2Filters').show();
    $('#divFinance2Monitor').show('slow');
    $("#tbFinance2MonitorList").trigger('reload');
}
