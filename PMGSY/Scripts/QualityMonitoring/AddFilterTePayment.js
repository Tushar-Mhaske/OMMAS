$(document).ready(function () {
    LoadExecutionGrid();
});

function LoadExecutionGrid() {

    $('#divTePaymentList').show('slow');

    jQuery("#tblTePaymentListDetails").jqGrid('GridUnload');
    jQuery("#tblTePaymentListDetails").jqGrid({
        url: '/QualityMonitoring/TechnicalExpertPaymentDetails',
        datatype: "json",
        async: false,
        cache: false,
        colNames: ['Id', 'Name', 'PAN Number', 'Total Allocated Works', 'Total Reviewed works', 'Total Paid Works', 'Total Unpaid Works', 'Add Apyment'],
        colModel: [
            { name: 'ID', index: 'ID', height: 'auto', width: 80, align: "left", search: false, hidden: true },
            { name: 'NAME', index: 'NAME', height: 'auto', width: 100, align: "center" },
            { name: 'PAN_NUMBER', index: 'PAN_NUMBER', height: 'auto', width: 70, align: "center" },
            { name: 'TOTAL_ALLOCATED_WORKS', index: 'TOTAL_ALLOCATED_WORKS', height: 'auto', width: 70, align: "center" },
            { name: 'TOTAL_REVIEWED_WORKS', index: 'TOTAL_REVIEWED_WORKS', height: 'auto', width: 70, align: "center" },
            { name: 'TOTAL_PAID_WORKS', index: 'TOTAL_PAID_WORKS', height: 'auto', width: 70, align: "center" },
            { name: 'TOTAL_UNPAID_WORKS', index: 'TOTAL_UNPAID_WORKS', height: 'auto', width: 70, align: "center" },
            { name: 'ADD_PAYMENT', index: 'ADD_PAYMENT', height: 'auto', width: 50, align: "center" },
        ],
        pager: jQuery('#divPagerTePaymentDetails').width(20),
        rowNum: 20,
        rowList: [20, 30, 40],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "ID",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Technical Expert Payment Details",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
            $("#tblTePaymentListDetails #divPagerTePaymentDetails").css({ height: '40px' });

            var windowWidth = window.innerWidth;
            var grid = $("#tblTePaymentListDetails");
            grid.setGridWidth(windowWidth - 100);
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }
    });
}

function AddTechnicalExpertPayment(teId) {

    $('#divTePaymentList').hide('slow');
    $('#divAddTePaymentList').show('slow');
    $('#backButton').show('slow');
    $('#AddPaymentButton').show('slow');

    LoadPaymentDetailsList(teId);
}

//Added By Hrishikesh For Getting Information of Total Allocated Works in CQC login --Start--17-07-2023
function GetTotalAllocatedWorksDetails(teId) {
    $('#divTePaymentList').hide('slow');
    $('#divGetTotalAllocatedWorksDetails').show('slow');

    LoadGetTotalAllocatedWorksDetails(teId);
}

function LoadGetTotalAllocatedWorksDetails(teId) {

    jQuery("#tblGetTotalAllocatedWorksDetails").jqGrid("GridUnload");

    jQuery("#tblGetTotalAllocatedWorksDetails").jqGrid({
        url: '/QualityMonitoring/AddTechnicalExpertPaymentWiseList?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        colNames: ["MasterId", "Observation Id", "Monitor Name", "State", "District", "Block", "Road/Bridge", "Road/Bridge Name", "Package No", "Scheme", "Inspection Date"],
        colModel: [
            { name: 'MasterId', index: 'MasterId', width: 80, sortable: true, align: "left", hidden: true },
            { name: 'ObservationId', index: 'ObservationId', width: 80, sortable: false, align: "left", hidden: true },
            //{ name: 'TEName', index: 'TEName', width: 100, sortable: false, align: "left", search: false },
            { name: 'Monitor', index: 'Monitor', width: 80, sortable: false, align: "left" },
            { name: 'State', index: 'State', width: 70, sortable: false, align: "left", search: false },
            { name: 'District', index: 'District', width: 45, sortable: false, align: "left", search: false },
            { name: 'Block', index: 'Block', width: 50, sortable: false, align: "left", search: false },
            { name: 'PropType', index: 'PropType', width: 30, sortable: false, align: "left", search: false },
            { name: 'RoadName', index: 'RoadName', width: 160, sortable: false, align: "left", search: false },
            { name: 'Package', index: 'Package', width: 30, sortable: false, align: "left", search: false },
            { name: 'Scheme', index: 'Scheme', width: 40, sortable: false, align: "center", search: false },
            { name: 'InspDate', index: 'InspDate', width: 60, sortable: false, align: "center", search: false },
        ],
        postData: { "teId": teId },
        pager: jQuery('#divAddPagerGetTotalAllocatedWorksDetails').width(20),
        rowNum: 30,
        rowList: [20, 30, 40],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "MasterId",
        sortorder: "asc",
        caption: "&nbsp;&nbsp;Works Details",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        //        autoResizing: { adjustGridWidth: false },
        loadComplete: function () {
            var windowWidth = window.innerWidth;
            var grid = $("#tblGetTotalAllocatedWorksDetails");
            grid.setGridWidth(windowWidth - 100);

            unblockPage();
        },
        onHeaderClick: function () {
            $('#divTePaymentList').show('slow');
            $('#divGetTotalAllocatedWorksDetails').hide('slow');
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
        }
    });
}


//Added By Hrishikesh For Getting Information of Total Allocated Works in CQC login --End--17-07-2023



function LoadPaymentDetailsList(teId) {

    jQuery("#tblAddTePaymentListDetails").jqGrid("GridUnload");

    jQuery("#tblAddTePaymentListDetails").jqGrid({
        url: '/QualityMonitoring/AddTechnicalExpertPaymentList?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        multiselect: true,
        colNames: ["MasterId", "Observation Id", "Technical Expert Name", "Monitor Name", "State", "District", "Block", "Road/Bridge", "Road/Bridge Name", "Package No", "Scheme", "Inspection Date"],
        colModel: [
            { name: 'MasterId', index: 'MasterId', width: 80, sortable: false, align: "left", hidden: true },
            { name: 'ObservationId', index: 'ObservationId', width: 80, sortable: false, align: "left", hidden: true },
            { name: 'TEName', index: 'TEName', width: 100, sortable: false, align: "left", search: false },
            { name: 'Monitor', index: 'Monitor', width: 80, sortable: false, align: "left" },
            { name: 'State', index: 'State', width: 70, sortable: false, align: "center", search: false },
            { name: 'District', index: 'District', width: 45, sortable: false, align: "left", search: false },
            { name: 'Block', index: 'Block', width: 50, sortable: false, align: "left", search: false },
            { name: 'PropType', index: 'PropType', width: 30, sortable: false, align: "left", search: false },
            { name: 'RoadName', index: 'RoadName', width: 160, sortable: false, align: "left", search: false },
            { name: 'Package', index: 'Package', width: 30, sortable: false, align: "left", search: false },
            { name: 'Scheme', index: 'Scheme', width: 40, sortable: false, align: "center", search: false },
            { name: 'InspDate', index: 'InspDate', width: 60, sortable: false, align: "center", search: false },
        ],
        postData: { "teId": teId },
        pager: jQuery('#dvInspListPager'),
        rowNum: 20000,
        viewrecords: true,
        pgbuttons: false,
        pgtext: null,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Add Technical Expert Payment List",
        height: '300',
        autowidth: true,
        autoResizing: { adjustGridWidth: false },
        grouping: true,
        groupingView: {
            groupField: ['TEName'],
            groupText: ['<b>{0}</b>'],
            groupColumnShow: [false],
            groupCollapse: false
        },
        loadComplete: function () {
            var windowWidth = window.innerWidth;
            var grid = $("#tblAddTePaymentListDetails");
            grid.setGridWidth(windowWidth - 100);

            var userdata = jQuery("#tblAddTePaymentListDetails").getGridParam('userData');
            var rocordCount = jQuery("#tblAddTePaymentListDetails").jqGrid('getGridParam', 'records');
            var count = 0;

            for (var i = 0; i < userdata.ids.length; i++) {

                if ($('#' + userdata.ids[i] + ' input[type=checkbox]').attr('checked', true)) {
                    jQuery("#jqg_tblAddTePaymentListDetails_" + userdata.ids[i]).attr("disabled", true);
                }
            }

            for (var i = 0; i < userdata.ids.length; i++) {
                if (($("#jqg_tblAddTePaymentListDetails_" + userdata.ids[i]).attr("disabled"))) {
                    count = count + 1;
                }
            }

            if (count == rocordCount) {
                $("#cb_tblAddTePaymentListDetails").attr('checked', true);
                $("#cb_tblAddTePaymentListDetails").attr('disabled', true);
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
    });
}

$('#btnBack').click(function () {
    LoadExecutionGrid();
    $('#divTePaymentList').show('slow');
    $('#divAddTePaymentList').hide('slow');
    $('#backButton').hide('slow');
    $('#AddPaymentButton').hide('slow');
});

function GetDataInArray() {

    var submitArray = [];

    var selRowIds = jQuery('#tblAddTePaymentListDetails').jqGrid('getGridParam', 'selarrrow');

    if (selRowIds.length > 0) {
        for (var i = 0; i < selRowIds.length; i++) {
            submitArray.push(selRowIds[i]);
        }
        AssignTechExpert(submitArray);
    }
    else
        alert("No records selected !!");
}

function AssignTechExpert(submitArray) {

    if (confirm("Do you want to add payment for the selected works ? ")) {

        $.ajax({
            type: "POST",
            url: "/QualityMonitoring/AddPaymentDetails/",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ 'submitarray': submitArray }),

            success: function (response) {
                unblockPage();
                alert(response.message);
                if (response.success) {
                    //alert("hello");
                    $("#btnBack").trigger('click');
                }
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            }
        });
    }
    else {
        return;
    }
}
