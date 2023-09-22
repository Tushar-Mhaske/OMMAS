$('#btnViewNQMSchedule').click(function () {

    $('#accordionNQMSchedule').hide('slow');
    $('#divNQMScheduleDetails').hide('slow');

    showNQMScheduleListGrid($("#ddlNQMScheduleMonth").val(), $("#ddlNQMScheduleYear").val());
});

function showNQMScheduleListGrid(month, year) {
   
    $('#divMonitorsSchedulePreparation').show();
    $("#tbNQMScheduleList").jqGrid('GridUnload');

    jQuery("#tbNQMScheduleList").jqGrid({
        url: '/TourClaim/GetNQMCurrScheduleList?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Month & Year of visit", "State", "District 1", "District 2", "District 3", "Inspection Status", "Tour Claim Details"],
        colModel: [
            { name: 'MonthYearOfVisit', index: 'MonthYearOfVisit', width: 250, sortable: false, align: "center", search: false },
            { name: 'State', index: 'State', width: 150, sortable: false, align: "left", search: false },
            { name: 'District1', index: 'District1', width: 130, sortable: false, align: "left", search: false },
            { name: 'District2', index: 'District2', width: 130, sortable: false, align: "left", search: false },
            { name: 'District3', index: 'District3', width: 130, sortable: false, align: "left", search: false },
            { name: 'InspStatus', index: 'InspStatus', width: 120, sortable: false, align: "center", search: false },
            { name: 'TourDetails', index: 'TourDetails', width: 100, sortable: false, align: "center", search: false },
        ],
        postData: { "month": month, "year": year },
        pager: jQuery('#dvNQMScheduleListPager'),
        rowNum: 10,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Schedule List",
        height: 'auto',
        autowidth: true,
        sortname: 'Monitor',
        rowList: [5, 10, 15],
        loadComplete: function () {
            if ($("#hdnRoleCodeOnMonitorLayout").val() == 7)  //for SQM
            {
                $('#tbNQMScheduleList').jqGrid('hideCol', 'District2');
                $('#tbNQMScheduleList').jqGrid('hideCol', 'District3');

                $('#tbNQMScheduleList').setGridWidth(($('#divNQMSchedulePreparation').width() - 10), true);
            }

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

