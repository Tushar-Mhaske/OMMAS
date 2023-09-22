/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMAssignRoads.js
        * Description   :   Handles events, grids in schedule assign process for Monitors
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/

var arrIsEnquiry = [];
var arrAddRoad = [];
$(document).ready(function () {

    //alert("techid" + $("#techid").val())
    $("#techid").change(function () {
        RoadListGrid($("#MAST_DISTRICT_CODE").val(), $("#ADMIN_SCHEDULE_CODE").val(), $("#techid").val());
    });

    if ($("#MAST_DISTRICT_CODE").val() > 0) {
        RoadListGrid($("#MAST_DISTRICT_CODE").val(), $("#ADMIN_SCHEDULE_CODE").val(), $("#techid").val(), $("#techid").val());
    }

    //District Code Change
    $("#MAST_DISTRICT_CODE").change(function () {

        if ($("#MAST_DISTRICT_CODE").val() > 0) {
            RoadListGrid($("#MAST_DISTRICT_CODE").val(), $("#ADMIN_SCHEDULE_CODE").val());
        }
        //$("#IMS_YEAR").val(0);
        //$("#IMS_YEAR").empty();
        //if ($(this).val() == 0) {
        //    $("#IMS_YEAR").append("<option value='0'>Select Year</option>");
        //    $("#tbSanctionRoadList").jqGrid('GridUnload');
        //}

        //if ($("#MAST_DISTRICT_CODE").val() > 0) {

        //    if ($("#IMS_YEAR").length > 0) {

        //        $.ajax({
        //            url: '/QualityMonitoring/PopulateYears',
        //            type: 'POST',
        //            data: { selectedState: $("#stateCode").val(), value: Math.random() },
        //            success: function (jsonData) {
        //                for (var i = 0; i < jsonData.length; i++) {
        //                    $("#IMS_YEAR").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
        //                }
        //            },
        //            error: function (xhr, ajaxOptions, thrownError) {
        //                alert(xhr.status);
        //                alert(thrownError);
        //            }
        //        });
        //    }
        //}

    });//District Code Change Ends here


    //Sanction Year Change
    //$("#IMS_YEAR").change(function () {

    //    if ($("#IMS_YEAR").val() > 0) {
    //        RoadListGrid($("#MAST_DISTRICT_CODE").val(), $("#IMS_YEAR").val(), $("#ROAD_STATUS").val(), $("#ADMIN_SCHEDULE_CODE").val());
    //    }
    //    else {
    //        $("#tbSanctionRoadList").jqGrid('GridUnload');
    //    }
    //});

    ////Road Status Change
    //$("#ROAD_STATUS").change(function () {

    //    if ($("#IMS_YEAR").val() > 0) {
    //        RoadListGrid($("#MAST_DISTRICT_CODE").val(), $("#IMS_YEAR").val(), $("#ROAD_STATUS").val(), $("#ADMIN_SCHEDULE_CODE").val());
    //    }
    //    else {
    //        $("#tbSanctionRoadList").jqGrid('GridUnload');
    //    }
    //});


}); //doc.ready ends here




////Road List 
//function RoadListGrid(districtCode, sanctionYear, rdStatus, adminSchCode) {
//    //alert(districtCode + " " + sanctionYear + " " + rdStatus + " " + adminSchCode);
//    $("#tbSanctionRoadList").jqGrid('GridUnload');


//    //getColumnIndexByName = function (columnName) {
//    //    var cm = jQuery("#tbSanctionRoadList").jqGrid('getGridParam', 'colModel'), i, l = cm.length;
//    //    for (i = 0; i < l; i++) {
//    //        if (cm[i].name === columnName) {
//    //            return i; // return the index
//    //        }
//    //    }
//    //}    

//    jQuery("#tbSanctionRoadList").jqGrid({
//        url: '/QualityMonitoring/GetRoadListToAssign?' + Math.random(),
//        datatype: "json",
//        mtype: "POST",
//        colNames: ["Block", "Package", "Road / Bridge", "Type", "Length (Road-Km / LSB-Mtr)", "Status", "NQM Inspection Count",
//                    "SQM Inspection Count", "Scheme", "Assigned By", "IsAlreadyAssigend", "Is Enquiry", "Add Road"],
//        colModel: [
//                            { name: 'Block', index: 'Block', width: 120, sortable: false, align: "left" },
//                            { name: 'Package', index: 'Package', width: 120, sortable: false, align: "center"},
//                            { name: 'Road', index: 'Road', width: 300, sortable: false, align: "left" },
//                            { name: 'Type', index: 'Type', width: 100, sortable: false, align: "left" },
//                            { name: 'RdLength', index: 'RdLength', width: 100, sortable: false, align: "center", search: false },
//                            { name: 'RdStatus', index: 'RdStatus', width: 100, sortable: false, align: "center", search: false },
//                            { name: 'NQMInspCount', index: 'NQMInspCount', width: 100, sortable: false, align: "center", search: false },
//                            { name: 'SQMInspCount', index: 'SQMInspCount', width: 100, sortable: false, align: "center", search: false },
//                            { name: 'PMGSYScheme', index: 'PMGSYScheme', width: 80, sortable: false, align: "center", search: false },
//                            { name: 'AssignedBy', index: 'AssignedBy', width: 80, sortable: false, align: "center", search: false },
//                            { name: 'IsEnquiry', index: 'IsEnquiry', width: 80, sortable: false, align: "center", search: false, hidden: true },
//                            {
//                                name: 'chkboxEnquiry', index: 'chkboxEnquiry', sortable: true, width: 80, align: 'center',
//                                formatter: "checkbox", formatoptions: { disabled: false }, editable: true, search: false,
//                                edittype: "checkbox", editoptions: { value: "Y:N" }
//                            },
//                            { name: 'AddRoad', index: 'AddRoad', width: 70, sortable: false, align: "center", search: false }
//        ],
//        postData: { "districtCode": districtCode, "sanctionYear": sanctionYear, "rdStatus": rdStatus, "adminSchCode": adminSchCode },
//        pager: jQuery('#dvSanctionRoadListPager'),
//        rowNum: 10000,
//        pgbuttons: false,
//        pgtext: null,
//        sortorder: "asc",
//        sortname: "Block",
//        viewrecords: true,
//        recordtext: '{2} records found',
//        caption: "&nbsp;&nbsp;Road List",
//        height: '300',
//        rownumbers: true,

//        loadComplete: function () {
//            $("#gview_tbSanctionRoadList > .ui-jqgrid-titlebar").hide();
//            $("#tbSanctionRoadList").jqGrid('setGridWidth', $("#tblAddRoads").width()+25, true);

//            var ids = jQuery("#tbSanctionRoadList").jqGrid('getDataIDs');
//            for (var i = 0; i < ids.length; i++) {
//                var curretRowId = ids[i];
//                var rowData = jQuery("#tbSanctionRoadList").getRowData(curretRowId);
//                var isEnquiry = rowData['IsEnquiry'];
//                var isAlreadyAssigned = rowData['AssignedBy'];

//                //alert(isEnquiry);
//                if (isEnquiry == "Y" && isAlreadyAssigned != "--")
//                {
//                    //$("#tbSanctionRoadList").jqGrid('setRowData', curretRowId, { 'chkboxEnquiry': 'Y' });  //can set valye as Y to Checkbox
//                    jQuery(this).find('#' + curretRowId + ' input[type=checkbox]').prop('checked', true);
//                    jQuery(this).find('#' + curretRowId + ' input[type=checkbox]').prop('disabled', true);
//                }
//                else if (isAlreadyAssigned != "--")
//                {
//                    jQuery(this).find('#' + curretRowId + ' input[type=checkbox]').prop('disabled', true);
//                }

//              }
//            unblockPage();
//        },
//        loadError: function (xhr, status, error) {

//            if (xhr.responseText == "session expired") {
//                window.location.href = "/Login/SessionExpire";
//            }
//            else {
//                window.location.href = "/Login/SessionExpire";
//            }
//        }


//    }); //end of grid

//    $("#tbSanctionRoadList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });

//}//Schedule Road Grid Ends Here

function closeDivError() {
    $("#divError").hide();
}

//Road List 
function RoadListGrid(districtCode, adminSchCode, techid) {
    $("#tbSanctionRoadList").jqGrid('GridUnload');


    jQuery("#tbSanctionRoadList").jqGrid({
        url: '/QualityMonitoring/GetRoadListToAssign?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Work Priority", "Block", "Package", "Sanction Year", "Road / Bridge", "Type", "Length (Road-Km / LSB-Mtr)", "Contractor Name (Work Award Date)", "Commenced Date", "Completed Date",
            "Scheme", "Assigned By", "NQM Inspection Date & Count", "SQM Inspection Date & Count", "Technology Name & Length", "Progress Status", "IsEnquiryHdn", "Is Ground Verification", "AddWorkHdn", "Add Work", "color",],
        colModel: [
            { name: 'WorkPriority', index: 'WorkPriority', hidden: true, width: 120, sortable: false, align: "left" },
            { name: 'Block', index: 'Block', width: 120, sortable: false, align: "left" },
            { name: 'Package', index: 'Package', width: 80, sortable: false, align: "center" },
            { name: 'SanctionYear', index: 'SanctionYear', width: 80, sortable: false, align: "center", search: false },

            { name: 'Road', index: 'Road', width: 300, sortable: false, align: "left" },
            { name: 'Type', index: 'Type', width: 80, sortable: false, align: "left" },
            { name: 'RdLength', index: 'RdLength', width: 100, sortable: false, align: "center", search: false },
            { name: 'CONTRACTOR_NAME', index: 'CONTRACTOR_NAME', width: 300, sortable: false, align: "left", search: false },


            { name: 'CommencedOrCompDate', index: 'CommencedOrCompDate', width: 100, sortable: false, align: "center", search: false },
            { name: 'COMPLETED_DATE', index: 'COMPLETED_DATE', width: 100, sortable: false, align: "center", search: false }, // Added by Sachin on 3 March 2020

            { name: 'PMGSYScheme', index: 'PMGSYScheme', width: 80, sortable: false, align: "center", search: false },
            { name: 'AssignedBy', index: 'AssignedBy', width: 80, sortable: false, align: "center", search: false },
            { name: 'NQMInspCount', index: 'NQMInspCount', width: 100, sortable: false, align: "center", search: false },
            { name: 'SQMInspCount', index: 'SQMInspCount', width: 100, sortable: false, align: "center", search: false },

            { name: 'TECHNOLOGY', index: 'TECHNOLOGY', width: 140, sortable: false, align: "center", search: false }, // Added by Sachin on 3 March 2020

            { name: 'ViewProgress', index: 'ViewProgress', width: 70, sortable: false, align: "center", search: false },

            { name: 'IsEnquiry', index: 'IsEnquiry', width: 40, sortable: false, align: "center", search: false, hidden: true },
            {
                name: 'chkboxEnquiry', index: 'chkboxEnquiry', sortable: true, width: 80, align: 'center',
                formatter: "checkbox", formatoptions: { disabled: false }, editable: true, search: false,
                edittype: "checkbox", editoptions: { value: "Y:N" }
            },
            { name: 'AddRoad', index: 'AddRoad', width: 70, sortable: false, align: "center", search: false, hidden: true },
            {
                name: 'chkboxAdd', index: 'chkboxAdd', sortable: true, width: 70, align: 'center',
                //formatter: "checkbox", formatoptions: { disabled: false },
                editable: true, search: false,
                edittype: "checkbox", editoptions: { value: "Y:N" }
            },

            { name: 'colorFlag', index: 'colorFlag', width: 70, sortable: false, align: "center", search: false, hidden: true },
        ],
        postData: { "districtCode": districtCode, "adminSchCode": adminSchCode, "techid": techid },
        pager: jQuery('#dvSanctionRoadListPager'),
        rowNum: 10000,
        pgbuttons: false,
        pgtext: null,
        sortorder: "asc",
        sortname: "Block",
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Road List",
        height: '300',
        grouping: true,
        groupingView: {
            groupField: ['WorkPriority'],
            groupText: ['<b>{0}</b>'],
            groupColumnShow: [false]
        },
        loadComplete: function () {
            $("#gview_tbSanctionRoadList > .ui-jqgrid-titlebar").hide();
            $("#tbSanctionRoadList").jqGrid('setGridWidth', $("#tblAddRoads").width() + 10, true);

            var ids = jQuery("#tbSanctionRoadList").jqGrid('getDataIDs');
            for (var i = 0; i < ids.length; i++) {
                var curretRowId = ids[i];
                var rowData = jQuery("#tbSanctionRoadList").getRowData(curretRowId);
                var isEnquiry = rowData['IsEnquiry'];
                var isAlreadyAssigned = rowData['AssignedBy'];
                var colorflag = rowData['colorFlag'];

                //console.log($('#' + curretRowId).children().length);
                if (isEnquiry == "Y" && isAlreadyAssigned != "--") {
                    //jQuery(this).find('#' + curretRowId + ' input[type=checkbox]').prop('checked', true);
                    jQuery(this).find('#' + curretRowId + ' input[type=checkbox]').prop('disabled', true);
                    jQuery(this).find('#' + curretRowId + ' :nth-child(15) input[type=checkbox]').prop('checked', true);
                    jQuery(this).find('#' + curretRowId + ' :nth-child(17) input[type=checkbox]').prop('checked', true);
                }
                else if (isAlreadyAssigned != "--") {
                    jQuery(this).find('#' + curretRowId + ' input[type=checkbox]').prop('disabled', true);
                    jQuery(this).find('#' + curretRowId + ' :nth-child(17) input[type=checkbox]').prop('checked', true);
                }

                if (colorflag == "Y") {
                    $('#' + ids[i]).find("td").css("background-color", "#fdebb4");
                }

                $("#dvSanctionRoadListPager_left").html("<input type='button' style='margin-left:5px' class='jqueryButton ui-button ui-widget ui-state-active ui-corner-all ui-button-text-only' onClick = 'addWorks();return false;' value='Assign Work'/>")
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

    $("#tbSanctionRoadList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });


}//Schedule Road Grid Ends Here


function ShowTechDetailsAgainstRoad(roadCode1) {

    window.open("/QualityMonitoring/ShowTechDetailsAgainstRoad?roadCode1=" + roadCode1).focus();

}


$("#tbSanctionRoadList").click(function (e) {

    var el = e.target; // DOM of the HTML element which was clicked
    if (el.nodeName !== "TD") {
        // in case of the usage of the custom formatter we should go to the next
        // parent TD element
        el = $(el, this.rows).closest("td");
        var iCol = $(el).index();
        var row = $(el, this.rows).closest("tr.jqgrow");
        var rowId = row[0].id;
        // now you can do what you need. You have iCol additionally to rowId
        if ($(el, this.rows).closest("td").attr('aria-describedby') === "tbSanctionRoadList_chkboxEnquiry")//column for IsEnquiry
        {
            if ($(el, this.rows).closest("td").find('input[type=checkbox]').prop('checked')) {
                arrIsEnquiry.push(rowId);
            }
            else {
                arrIsEnquiry = jQuery.grep(arrIsEnquiry, function (value) {
                    return value != rowId;
                });
            }
        }
        else if ($(el, this.rows).closest("td").attr('aria-describedby') === "tbSanctionRoadList_chkboxAdd")//column for Add Road
        {

            if ($(el, this.rows).closest("td").find('input[type=checkbox]').prop('checked')) {
                arrAddRoad.push(rowId);
            }
            else {
                arrAddRoad = jQuery.grep(arrAddRoad, function (value) {
                    return value != rowId;
                });
            }
        }
    }
});//Grid Click ends here

function QMViewProgress(prRoadCode) {
    $("#dlgPhyProgress").load("/QualityMonitoring/ViewPhysicalProgress/" + prRoadCode, function () {

        setTimeout(function () {
            $("#dlgPhyProgress").dialog({
                autoOpen: true,
                modal: true,
                height: 500,
                width: 1050,
                title: "Physical Progress"
            });
            $("#dlgPhyProgress").show();
        }, 100);
    });
}

function addWorks() {
    if (confirm('Are you sure to assign selected works?')) {
        $.ajax({
            url: '/QualityMonitoring/QMAssignWorks',
            type: 'POST',
            data: { arrWorks: arrAddRoad, arrEnquiry: arrIsEnquiry, adminSchCode: $("#ADMIN_SCHEDULE_CODE").val(), value: Math.random() },
            success: function (response) {
                if (response.Success) {
                    alert("Works assigned successfully in schedule");
                    $("#tbSanctionRoadList").trigger("reloadGrid");
                    QMAssignRoads($("#ADMIN_SCHEDULE_CODE").val());
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

function QMAddRoadToSchedule(prRoadCode) {
    var isEnquiry = jQuery('#tbSanctionRoadList').jqGrid('getCell', prRoadCode, 'chkboxEnquiry');
    if (confirm('Are you sure to assign this road?')) {
        $.ajax({
            url: '/QualityMonitoring/QMAssignRoads',
            type: 'POST',
            data: { prRoadCode: prRoadCode, adminSchCode: $("#ADMIN_SCHEDULE_CODE").val(), isEnquiry: isEnquiry, value: Math.random() },
            success: function (response) {
                if (response.Success) {
                    alert("Road added successfully in schedule");
                    $("#tbSanctionRoadList").trigger("reloadGrid");
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




