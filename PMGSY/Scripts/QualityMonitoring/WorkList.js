
var districtCode = $('#ddlDistrict option:selected').val();
var schCode = $('#ADMIN_SCHEDULE_CODE').val();

$("#btnListDetails").click(function () {

    if ($("#searchExecution").valid()) {
        $('#notice').show();
        districtCode = $('#ddlDistrict option:selected').val();
        techSelectrd = $('#ddlTech option:selected').val();
        LoadWorkListGrid(districtCode, techSelectrd);
    }
});

$("#btnListDetailsCQC").click(function () {

    if ($("#searchExecution").valid()) {
        $('#notice').show();
        districtCode = $('#ddlDistrict option:selected').val();
        techSelectrd = $('#ddlTech option:selected').val();
        LoadWorkListGrid(districtCode, techSelectrd);
    }
});



$('#ddlStates').change(function () {
    $("#ddlDistrict").empty();
    //alert("hi");

    $.ajax({
        url: '/QualityMonitoring/PopulateDistrictsbyStateCode',
        type: 'POST',
        async: false,
        cache: false,
        beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        },
        data: { stateCode: $("#ddlStates").val(), },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {

                $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");

            }
            districtCode = $('#ddlDistrict option:selected').val();
            $("#btnListDetails").trigger('click');
            //LoadWorkListGrid(districtCode);

            $.unblockUI();
        },
        error: function (err) {
            $.unblockUI();
        }
    });

});

var road_code;
$("#idFilterDiv").click(function () {
    $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-n").toggleClass("ui-icon-circle-triangle-s");
    $("#searchExecution").toggle("slow");
});

function LoadWorkListGrid(districtCode, techSelectrd, adminSchCode) {
    /*alert($('#ADMIN_SCHEDULE_CODE').val());*/
    //alert("techSelectrd" + techSelectrd);

    $("#tbExecutionList").jqGrid('GridUnload');

    jQuery("#tbExecutionList").jqGrid({
        url: '/QualityMonitoring/GetRoadList?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Work Priority", "State", "District", "Block", "Package", "Sanction Year", "Road / Bridge", "Type", "Length (Road-Km / LSB-Mtr)", "Contractor Name (Work Award Date)",

            "Road Status", "Enquiry Inspection", "NQM Inspection Date & Count", "Scheme", "Technology Name & Length", "color"],
        colModel: [
            { name: 'WorkPriority', index: 'WorkPriority', hidden: true, width: 120, sortable: false, align: "left" },
            { name: 'State', index: 'State', width: 100, sortable: false, align: "left", search: false },
            { name: 'District', index: 'District', width: 100, sortable: false, align: "left", search: false },
            { name: 'Block', index: 'Block', width: 100, sortable: false, align: "left" },
            { name: 'Package', index: 'Package', width: 80, sortable: false, align: "center" },
            { name: 'SanctionYear', index: 'SanctionYear', width: 80, sortable: false, align: "center", search: false },

            { name: 'Road', index: 'Road', width: 300, sortable: false, align: "left" },
            { name: 'Type', index: 'Type', width: 80, sortable: false, align: "left" },
            { name: 'RdLength', index: 'RdLength', width: 100, sortable: false, align: "center", search: false },
            { name: 'CONTRACTOR_NAME', index: 'CONTRACTOR_NAME', width: 300, sortable: false, align: "left", search: false },

            /*{ name: 'CommencedOrCompDate', index: 'CommencedOrCompDate', width: 100, sortable: false, align: "center", search: false },
            { name: 'COMPLETED_DATE', index: 'COMPLETED_DATE', width: 100, sortable: false, align: "center", search: false },*/
            { name: 'RoadStatus', index: 'RoadStatus', width: 100, sortable: false, align: "center", search: false },
            { name: 'EnquiryInspection', index: 'EnquiryInspection', width: 80, sortable: false, align: "center", search: false },

            /* { name: 'AssignedBy', index: 'AssignedBy', width: 80, sortable: false, align: "center", search: false },*/
            { name: 'NQMInspCount', index: 'NQMInspCount', width: 100, sortable: false, align: "center", search: false },
            { name: 'PMGSYScheme', index: 'PMGSYScheme', width: 80, sortable: false, align: "center", search: false },
            /*{ name: 'SQMInspCount', index: 'SQMInspCount', width: 100, sortable: false, align: "center", search: false },*/

            { name: 'TECHNOLOGY', index: 'TECHNOLOGY', width: 140, sortable: false, align: "center", search: false },

            { name: 'colorFlag', index: 'colorFlag', width: 70, sortable: false, align: "center", search: false, hidden: true },
        ],
        postData: { "districtCode": districtCode, "adminSchCode": adminSchCode, "ddlTech": techSelectrd }, /*Added for ddlTech by Shreyas*/
        pager: jQuery('#pagerExecution'),
        rowNum: 10000,
        pgbuttons: false,
        pgtext: null,
        sortorder: "asc",
        sortname: "Block",
        viewrecords: true,
        recordText: '{2} records found',
        caption: "&nbsp;&nbsp;Road List",
        height: '300',
        grouping: true,
        groupingView: {
            groupField: ['WorkPriority'],
            groupText: ['<b>{0}</b>'],
            groupColumnShow: [false]
        },
        loadComplete: function () {
            $("#divNote").show();
            $("#tbExecutionList > .ui-jqgrid-titlebar").hide();
            $("#tbExecutionList").jqGrid('setGridWidth', $("#tblAddRoads").width() + 1740, true);

            var ids = jQuery("#tbExecutionList").jqGrid('getDataIDs');
            for (var i = 0; i < ids.length; i++) {
                var curretRowId = ids[i];
                var rowData = jQuery("#tbExecutionList").getRowData(curretRowId);
                var colorflag = rowData['colorFlag'];

                if (colorflag == "Y") {
                    $('#' + ids[i]).find("td").css("background-color", "#fdebb4");
                }

                //$("#pagerExecution_left").html("<input type='button' style='margin-left:5px' class='jqueryButton ui-button ui-widget ui-state-active ui-corner-all ui-button-text-only' onClick = 'addWorks();return false;' value='Assign Work'/>")
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

    $("#tbExecutionList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });

}



