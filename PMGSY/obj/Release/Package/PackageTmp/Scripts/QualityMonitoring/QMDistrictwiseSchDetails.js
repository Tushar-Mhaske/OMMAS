/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMDistrictwiseSchDetails.js
        * Description   :   Handles events, grids in districtwise Schedule Details
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/


$(document).ready(function () {
    //alert("1");
    var scheduleCode = $("#ADMIN_SCHEDULE_CODE").val()
    //alert(scheduleCode)
    $.ajax({
        url: '/QualityMonitoring/CheckDeleteStatus?scheduleCode=' + scheduleCode,
        type: 'POST',

        contentType: false,
        processData: false,
        cache: false,
        success: function (response) {


            if (response.Success) {
                if (response.status == "Y")
                    districtwiseScheduleDetails();
                else
                    districtwiseScheduleDetails1();
            }
            else {
                //alert("There is an error while processing your request.");
                districtwiseScheduleDetails1();
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });


    $("#tbDistrictwiseSchList").jqGrid('GridUnload');
    //districtwiseScheduleDetails();


});


function districtwiseScheduleDetails() {

    $("#tbDistrictwiseSchList").jqGrid('GridUnload');
    jQuery("#tbDistrictwiseSchList").jqGrid({
        url: '/QualityMonitoring/QMViewScheduleDetails?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        multiselect: true,
        colNames: ["Select", "District", "Block", "Package", "Sanction Year", "Road Name", "Type", "Length (Road-Km / LSB-Mtr)", "Contractor Name (Work Award Date)", "Road Status", "Enquiry Inspection",
                    "Assigned By", "NQM Inspection Date & Count", "Inspection Status", "Scheme", "Delete", "Finalize", "Select"],
        colModel: [
                    { name: 'IMS_PR_ROAD_CODE', index: 'IMS_PR_ROAD_CODE', width: 90, sortable: false, align: "left", hidden: true, key: true },
                    { name: 'District', index: 'District', width: 90, sortable: false, align: "left" },
                    { name: 'Block', index: 'Block', width: 70, sortable: false, align: "center" },
                    { name: 'Package', index: 'Package', width: 60, sortable: false, align: "center" },
                    { name: 'Year', index: 'Year', width: 60, sortable: false, align: "center", search: false },
                    { name: 'Road', index: 'Road', width: 210, sortable: false, align: "left" },
                    { name: 'PropType', index: 'PropType', width: 60, sortable: false, align: "left", search: false },
                    { name: 'RdLength', index: 'RdLength', width: 80, sortable: false, align: "center", search: false },
                    { name: 'CONTRACTOR_NAME', index: 'CONTRACTOR_NAME', width: 150, sortable: false, align: "left", search: false },
                    { name: 'RdStatus', index: 'RdStatus', width: 130, sortable: false, align: "left", search: false },
                    { name: 'IsEnquiry', index: 'IsEnquiry', width: 60, sortable: false, align: "center", search: false },
                    { name: 'AssignedBy', index: 'AssignedBy', width: 60, sortable: false, align: "center", search: false },
                    { name: 'NQMInspCount', index: 'NQMInspCount', width: 100, sortable: false, align: "center", search: false },
                    { name: 'InspStatus', index: 'InspStatus', width: 70, sortable: false, align: "center", search: false },
                    { name: 'Scheme', index: 'Scheme', width: 70, sortable: false, align: "center", search: false },
                    { name: 'Delete', index: 'Delete', width: 60, sortable: false, align: "center", search: false, hidden: true },
                    { name: 'Finalize', index: 'Finalize', width: 60, sortable: false, align: "center", search: false },
                    { name: 'Select', index: 'Select', width: 70, sortable: false, align: "center", search: false, hidden: true, formatter: setCheckBox },
                    //{
                    //    name: 'chkboxSelect', index: 'chkboxSelect', sortable: true, width: 40, align: 'center',
                    //    //formatter: "checkbox", formatoptions: { disabled: false },
                    //    editable: true, search: false,
                    //    edittype: "checkbox", editoptions: { value: "Y:N" }
                    //}
        ],
        postData: { "adminSchCode": $("#ADMIN_SCHEDULE_CODE").val(), value: Math.random() },
        pager: jQuery('#dvDistrictwiseSchListPager'),
        rowNum: 10000,
        sortorder: "asc",
        sortname: "District",
        viewrecords: true,
        pgbuttons: false,
        pgtext: null,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Road List",
        height: '300',
        autowidth: true,
        grouping: true,
        groupingView: {
            groupField: ['District'],
            groupText: ['<b>{0}</b>'],
            groupColumnShow: [false]
        },

        jsonReader: {
            Id: "IMS_PR_ROAD_CODE",
        },

        //beforeSelectRow: function (rowId, e)  {
        //    alert(aRowids);
        //}
        //,

        loadComplete: function () {


            //if (cellValue == "N") {
            //    $("#cb_tbDistrictwiseSchList").attr('checked', true);
            //    $("#cb_tbDistrictwiseSchList").attr('disabled', true);
            //    $('#btndeleteall').hide();
            //}


            $("#gview_tbDistrictwiseSchList > .ui-jqgrid-titlebar").hide();
            if ($("#hdnRoleCodeDistwiseSch").val() == 5) //for SQC hide these two columns
            {
                $('#tbDistrictwiseSchList').jqGrid('hideCol', 'Finalize');
                $('#tbDistrictwiseSchList').setGridWidth(($('#dvDistrictwiseSchDetailsId').width() - 10), true);
            }

            if ($("#hdnRoleCodeDistwiseSch").val() == 9) {
                $('#tbDistrictwiseSchList').jqGrid('hideCol', 'Finalize');
                $('#tbDistrictwiseSchList').setGridWidth(($('#divScheduleDetails').width() - 10), true);
            }

            if ($("#hdnRoleCodeDistwiseSch").val() == 8) //for SQC only show Finalize All button
            {
                if ($("#INSP_STATUS_FLAG").val() != "UPGF") {       //If Schedule not forwared to monitor then only show this button

                    var recordCnt = $("#tbDistrictwiseSchList").jqGrid('getGridParam', 'records');
                    if (recordCnt > 0) {
                        $("#tbDistrictwiseSchList #dvDistrictwiseSchListPager").css({ height: '35px' });
                        $("#dvDistrictwiseSchListPager_left").html("<input type='button' style='margin-left:1px' id='idFinalizeAllRoads' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'FinalizeSchRoad(0, true);return false;' value='Finalize All'/>");
                    }
                }
            }

            if ($("#hdnRoleCodeDistwiseSch").val() == 22) //for DPIU hide Finalize column
            {
                $('#tbDistrictwiseSchList').jqGrid('hideCol', 'Finalize');
                $('#tbDistrictwiseSchList').setGridWidth(($('#divQualityLayoutPIU').width() - 100), true);

            }

            if ($('#tbDistrictwiseSchList').jqGrid('getGridParam', 'records')) {
                $("#dvDistrictwiseSchListPager_left").html("<input type='button' style='margin-left:27px;margin-top:2px;' id='btndeleteall' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'GetDataInArray();return false;' value='Delete'/>");
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

// Added by  pk

function districtwiseScheduleDetails1() {

    $("#tbDistrictwiseSchList").jqGrid('GridUnload');
    jQuery("#tbDistrictwiseSchList").jqGrid({
        url: '/QualityMonitoring/QMViewScheduleDetails?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Select", "District", "Block", "Package", "Sanction Year", "Road Name", "Type", "Length (Road-Km / LSB-Mtr)", "Contractor Name (Work Award Date)", "Road Status", "Enquiry Inspection",
                    "Assigned By", "NQM Inspection Date & Count", "Inspection Status", "Scheme", "Delete", "Finalize", "Select"],
        colModel: [
                    { name: 'IMS_PR_ROAD_CODE', index: 'IMS_PR_ROAD_CODE', width: 90, sortable: false, align: "left", hidden: true, key: true },
                    { name: 'District', index: 'District', width: 90, sortable: false, align: "left" },
                    { name: 'Block', index: 'Block', width: 70, sortable: false, align: "center" },
                    { name: 'Package', index: 'Package', width: 60, sortable: false, align: "center" },
                    { name: 'Year', index: 'Year', width: 60, sortable: false, align: "center", search: false },
                    { name: 'Road', index: 'Road', width: 210, sortable: false, align: "left" },
                    { name: 'PropType', index: 'PropType', width: 60, sortable: false, align: "left", search: false },
                    { name: 'RdLength', index: 'RdLength', width: 80, sortable: false, align: "center", search: false },
                    { name: 'CONTRACTOR_NAME', index: 'CONTRACTOR_NAME', width: 150, sortable: false, align: "left", search: false },
                    { name: 'RdStatus', index: 'RdStatus', width: 130, sortable: false, align: "left", search: false },
                    { name: 'IsEnquiry', index: 'IsEnquiry', width: 60, sortable: false, align: "center", search: false },
                    { name: 'AssignedBy', index: 'AssignedBy', width: 60, sortable: false, align: "center", search: false },
                    { name: 'NQMInspCount', index: 'NQMInspCount', width: 100, sortable: false, align: "center", search: false },
                    { name: 'InspStatus', index: 'InspStatus', width: 70, sortable: false, align: "center", search: false },
                    { name: 'Scheme', index: 'Scheme', width: 70, sortable: false, align: "center", search: false },
                    { name: 'Delete', index: 'Delete', width: 60, sortable: false, align: "center", search: false },
                    { name: 'Finalize', index: 'Finalize', width: 60, sortable: false, align: "center", search: false },
                    { name: 'Select', index: 'Select', width: 70, sortable: false, align: "center", search: false, hidden: true, formatter: setCheckBox },
                    //{
                    //    name: 'chkboxSelect', index: 'chkboxSelect', sortable: true, width: 40, align: 'center',
                    //    //formatter: "checkbox", formatoptions: { disabled: false },
                    //    editable: true, search: false,
                    //    edittype: "checkbox", editoptions: { value: "Y:N" }
                    //}
        ],
        postData: { "adminSchCode": $("#ADMIN_SCHEDULE_CODE").val(), value: Math.random() },
        pager: jQuery('#dvDistrictwiseSchListPager'),
        rowNum: 10000,
        sortorder: "asc",
        sortname: "District",
        viewrecords: true,
        pgbuttons: false,
        pgtext: null,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Road List",
        height: '300',
        autowidth: true,
        grouping: true,
        groupingView: {
            groupField: ['District'],
            groupText: ['<b>{0}</b>'],
            groupColumnShow: [false]
        },

        jsonReader: {
            Id: "IMS_PR_ROAD_CODE",
        },

        //beforeSelectRow: function (rowId, e)  {
        //    alert(aRowids);
        //}
        //,

        loadComplete: function () {


            //if (cellValue == "N") {
            //    $("#cb_tbDistrictwiseSchList").attr('checked', true);
            //    $("#cb_tbDistrictwiseSchList").attr('disabled', true);
            //    $('#btndeleteall').hide();
            //}


            $("#gview_tbDistrictwiseSchList > .ui-jqgrid-titlebar").hide();
            if ($("#hdnRoleCodeDistwiseSch").val() == 5) //for SQC hide these two columns
            {
                $('#tbDistrictwiseSchList').jqGrid('hideCol', 'Finalize');
                $('#tbDistrictwiseSchList').setGridWidth(($('#dvDistrictwiseSchDetailsId').width() - 10), true);
            }

            if ($("#hdnRoleCodeDistwiseSch").val() == 9) {
                $('#tbDistrictwiseSchList').jqGrid('hideCol', 'Finalize');
                $('#tbDistrictwiseSchList').setGridWidth(($('#divScheduleDetails').width() - 10), true);
            }

            if ($("#hdnRoleCodeDistwiseSch").val() == 8) //for SQC only show Finalize All button
            {
                if ($("#INSP_STATUS_FLAG").val() != "UPGF") {       //If Schedule not forwared to monitor then only show this button

                    var recordCnt = $("#tbDistrictwiseSchList").jqGrid('getGridParam', 'records');
                    if (recordCnt > 0) {
                        $("#tbDistrictwiseSchList #dvDistrictwiseSchListPager").css({ height: '35px' });
                        $("#dvDistrictwiseSchListPager_left").html("<input type='button' style='margin-left:1px' id='idFinalizeAllRoads' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'FinalizeSchRoad(0, true);return false;' value='Finalize All'/>");
                    }
                }
            }

            if ($("#hdnRoleCodeDistwiseSch").val() == 22) //for DPIU hide Finalize column
            {
                $('#tbDistrictwiseSchList').jqGrid('hideCol', 'Finalize');
                $('#tbDistrictwiseSchList').setGridWidth(($('#divQualityLayoutPIU').width() - 100), true);

            }

            if ($('#tbDistrictwiseSchList').jqGrid('getGridParam', 'records')) {
                $("#dvDistrictwiseSchListPager_left").html("<input type='button' style='margin-left:27px;margin-top:2px;' id='btndeleteall' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'GetDataInArray();return false;' value='Delete'/>");
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

//end

function QMDeleteRoadFromSchedule(prRoadCode) {
    if (confirm('Are you sure to delete this road?')) {
        $.ajax({
            url: '/QualityMonitoring/QMDeleteSchRoads/',
            type: 'POST',
            data: { prRoadCode: prRoadCode, adminSchCode: $("#ADMIN_SCHEDULE_CODE").val(), value: Math.random() },
            success: function (response) {
                if (response.Success) {
                    alert("Road deleted successfully from schedule");
                    $("#tbDistrictwiseSchList").trigger("reloadGrid");
                }
                else {
                    $("#divError").show("slow");
                    $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
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

function FinalizeSchRoad(prRoadCode, finalizeAll) {

    var message = "Are you sure to finalize?";
    var alertMsg = "Schedule finalized successfully";
    if (finalizeAll == true) {
        message = "Are you sure to finalize all roads under the schedule?";
        alertMsg = "All scheduled roads finalized successfully";
    }
    else {
        message = "Are you sure to finalize this road?";
        alertMsg = "Road finalized successfully";
    }

    if (confirm(message)) {
        $.ajax({
            url: '/QualityMonitoring/FinalizeRoad/',
            type: 'POST',
            data: { prRoadCode: prRoadCode, adminSchCode: $("#ADMIN_SCHEDULE_CODE").val(), "isFinalizeAllRoads": finalizeAll, value: Math.random() },
            success: function (response) {
                if (response.Success) {
                    alert(alertMsg);
                    $("#tbDistrictwiseSchList").trigger("reloadGrid");
                }
                else {
                    $("#divError").show("slow");
                    $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
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

function GetDataInArray() {

    //var submitArray1 = { roadcode: null, adminSchCode: null };
    var submitArray = [];
    var selRowIds = jQuery('#tbDistrictwiseSchList').jqGrid('getGridParam', 'selarrrow');

    if (selRowIds.length > 0) {
        for (var i = 0; i < selRowIds.length; i++) {

            rowdata = jQuery("#tbDistrictwiseSchList").getRowData(selRowIds[i]);

            if (!$("#jqg_tbDistrictwiseSchList_" + selRowIds[i]).attr("disabled")) {
                submitArray.push({ prRoadCode: rowdata["IMS_PR_ROAD_CODE"], scheduleCode: $("#ADMIN_SCHEDULE_CODE").val() });
            }
        }
        //alert(JSON.stringify(submitArray));
        if (submitArray.length > 0) {
            DeleteSelectedRoad(submitArray);
        }
        else {
            alert("No records selected to delete..");
        }
        //FreezeDetails(submitArray);
    }
    else
        alert("No records to delete");

}


function DeleteSelectedRoad(submitArray) {
    if (confirm("Are you sure you want to delete the selected details ? ")) {

        $.ajax({
            type: "POST",
            url: '/QualityMonitoring/QMDeleteSelectedRoads/',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ 'submitarray': submitArray }),
            success: function (response) {
                unblockPage(); //alert(JSON.stringify(response));
                if (response.Success) {
                    alert("Details deleted successfully.");

                    $("#tbDistrictwiseSchList").trigger('reloadGrid');

                    closeMonitorsInspectionDetails();
                    $(".ui-icon-closethick").trigger("click");

                }
                else {
                    alert("Something went wrong.");
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

function setCheckBox(cellValue, options, rowObject) {
    //alert(cellValue)
    //var id = $(this).closest("tr").find('td:eq(2)').text();
    //alert(id);
    if (cellValue == "N") {
        $("#cb_tbDistrictwiseSchList").attr('checked', true);
        $("#cb_tbDistrictwiseSchList").attr('disabled', true);
        $('#dvDistrictwiseSchListPager_left').hide();
        $("#cb_tbDistrictwiseSchList").hide();
        $("#tbDistrictwiseSchList_cb").hide();
        //$(".cbox").attr('disabled', true);
        // $(".cbox:checkbox").filter(":checked");
        //$(".cbox:checkbox").attr('checked', true);
        //$("#tbDistrictwiseSchList").jqGrid("multiselect" : false);
        //$("#tbDistrictwiseSchList").hideCol("")
    }
}

function deletestatus() {

    $.ajax({
        url: '/QualityMonitoring/QMDeleteSchRoads/',
        type: 'POST',
        data: { prRoadCode: prRoadCode, adminSchCode: $("#ADMIN_SCHEDULE_CODE").val(), value: Math.random() },
        success: function (response) {
            if (response.Success) {
                alert("1");
            }
            else {
                alert("2");
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}
