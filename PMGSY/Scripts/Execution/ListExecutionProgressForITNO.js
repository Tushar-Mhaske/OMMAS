$(document).ready(function () {

    //disabled enter key
    $("input").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#ddlDistricts").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlDistricts").find(":selected").val() },
                   "#ddlPackage", "/Execution/GetPackageByState?yearCode=" + $('#ddlImsYear option:selected').val() + "&districtCode=" + $("#ddlDistricts option:selected").val());

        FillInCascadeDropdown({ userType: $("#ddlDistricts").find(":selected").val() },
                   "#ddlMastBlockCode", "/Execution/GetBlockByDistrict?districtCode=" + $("#ddlDistricts option:selected").val());
    });

    //list button click
    $("#btnListExecution").click(function () {

        $("#tbExecutionList").jqGrid('GridUnload');
        LoadExecutionGrid();
        if($("#divPhysicalDetails").is(':visible'))
        {
            $("#divPhysicalDetails").hide();
        }

        if ($("#divLSBPhysicalDetails").is(':visible')) {
            $("#divLSBPhysicalDetails").hide();
        }
    });

    $("#ddlImsYear").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlImsYear").find(":selected").val() },
                    "#ddlImsPackages", "/Execution/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlImsYear option:selected').val() + "&blockCode=" + $('#ddlMastBlockCode option:selected').val());
    });

    $("#ddlMastBlockCode").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlMastBlockCode").find(":selected").val() },
                    "#ddlImsPackages", "/Execution/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlImsYear option:selected').val() + "&blockCode=" + $('#ddlMastBlockCode option:selected').val());
    });


});

// to load execution details
function LoadExecutionGrid() {

    jQuery("#tbExecutionList").jqGrid({
        url: '/Execution/GetExecutionProgressListForITNO',
        datatype: "json",
        mtype: "POST",
        postData: { districtCode: $("#ddlDistricts option:selected").val(), yearCode: $("#ddlImsYear option:selected").val(), blockCode: $('#ddlMastBlockCode option:selected').val(), packageCode: $("#ddlImsPackages option:selected").val(), proposalCode: $("#ddlImsProposalTypes").val(), batch: $('#ddlBatchs option:selected').val(), collaboration: $('#ddlCollaborations option:selected').val(), upgradationType: $('#ddlUpgradations option:selected').val() },
        colNames: ['District', 'Block', 'Year', 'Batch', 'Package No.', 'Road/Bridge Name', 'Road/LSB Cost (In Lacs)', 'Road Length(in Kms)/LSB Length(int Mtrs)', 'Maintenance Cost(in Lacs)', 'Physical', 'Financial'],
        colModel: [
                            { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 80, align: "left", search: false },
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 80, align: "left", search: false },
                            { name: 'Year', index: 'Year', width: 70, sortable: true, align: "center" },
                            { name: 'Batch', index: 'Batch', width: 70, sortable: true, align: "center" },
                            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'PLAN_RD_NAME', index: 'PLAN_RD_NAME', height: 'auto', width: 250, align: "left", search: true },
                            { name: 'ROAD_COST', index: 'ROAD_COST', height: 'auto', width: 100, align: "right", search: true },
                            { name: 'ROAD_LENGTH', index: 'ROAD_LENGTH', height: 'auto', width: 100, align: "right", search: true },
                            { name: 'MAINTENANCE_COST', index: 'MAINTENANCE_COST', height: 'auto', width: 100, align: "right", search: true },
                            { name: 'a', width: 50, sortable: false, resize: false, align: "center", search: false, hidden:true },
                            { name: 'b', width: 50, sortable: false, align: "center", search: false, hidden: true },

        ],
        pager: jQuery('#pagerExecution').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "PLAN_RD_NAME",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Execution Details List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
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

//Load the Physical details of road
function LoadPhysicalRoadDetails(IMS_ROAD_CODE) {

    jQuery("#tbPhysicalRoadList").jqGrid('GridUnload');

    jQuery("#tbPhysicalRoadList").jqGrid({
        url: '/Execution/GetRoadPhysicalProgressListForITNO',
        datatype: "json",
        mtype: "POST",
        postData: { roadCode: IMS_ROAD_CODE },
        colNames: ['Month', 'Year', 'Month', 'Year', 'Work Status', 'Preparatory Work (Length in Km.)', 'Subgrade Stage (Length in Km.)', 'Subbase (Length in Km.)', 'Base Course (Length in Km.)', 'Surface Course (Length in Km.)', 'Road Signs Stones (in Nos.)', 'CDWorks (in Nos.)', 'LS Bridges (in Nos.)', 'Miscellaneous (Length in Km.)', 'Completed (Length in Km.)', 'Edit', "Action", 'Delete'],
        colModel: [
                            { name: 'YEAR', index: 'YEAR', height: 'auto', width: 55, align: "center", search: false, editable: true, hidden: true },
                            { name: 'MONTH', index: 'MONTH', height: 'auto', width: 60, align: "center", search: false, editable: true, hidden: true },
                            
                            { name: 'EXEC_PROG_MONTH', index: 'EXEC_PROG_MONTH', height: 'auto', width: 80, align: "center", search: false, editable: false },
                            { name: 'EXEC_PROG_YEAR', index: 'EXEC_PROG_YEAR', height: 'auto', width: 80, align: "left", search: false, editable: false },
                            { name: 'EXEC_ISCOMPLETED', index: 'EXEC_ISCOMPLETED', height: 'auto', width: 120, align: "center", search: true, editable: false },
                            { name: 'EXEC_PREPARATORY_WORK', index: 'EXEC_PREPARATORY_WORK', height: 'auto', width: 80, align: "center", search: false, editable: true, editoptions: { maxlength: 10 }, editrules: { custom: true, custom_func: ValidateLength } },
                            { name: 'EXEC_EARTHWORK_SUBGRADE', index: 'EXEC_EARTHWORK_SUBGRADE', height: 'auto', width: 80, align: "center", search: false, editable: true },
                            { name: 'EXEC_SUBBASE_PREPRATION', index: 'EXEC_SUBBASE_PREPRATION', height: 'auto', width: 70, align: "center", search: false, editable: true },
                            { name: 'EXEC_BASE_COURSE', index: 'EXEC_BASE_COURSE', height: 'auto', width: 80, align: "center", search: false, editable: true },
                            { name: 'EXEC_SURFACE_COURSE', index: 'EXEC_SURFACE_COURSE', height: 'auto', width: 80, align: "center", search: false, editable: true },
                            { name: 'EXEC_SIGNS_STONES', index: 'EXEC_SIGNS_STONES', height: 'auto', width: 80, align: "center", search: false, editable: true },
                            { name: 'EXEC_CD_WORKS', index: 'EXEC_CD_WORKS', height: 'auto', width: 60, align: "center", search: false, editable: true },
                            { name: 'EXEC_LSB_WORKS', index: 'EXEC_LSB_WORKS', height: 'auto', width: 60, align: "center", search: false, editable: true },
                            { name: 'EXEC_MISCELANEOUS', index: 'EXEC_MISCELANEOUS', height: 'auto', width: 70, align: "center", search: false, editable: true },
                            { name: 'EXEC_COMPLETED', index: 'EXEC_COMPLETED', height: 'auto', width: 80, align: "center", search: false, editable: true },
                            { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", editable: false },
                            { name: 'Save', index: 'Save', width: 40, sortable: false, align: "center", editable: false, hidden: true },
                            { name: 'b', width: 40, align: "center", search: false, sortable: false },

        ],
        pager: jQuery('#pagerPhysicalRoadList').width(20),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'EXEC_PROG_MONTH,EXEC_PROG_YEAR',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Physical Road Progress List",
        height: 'auto',
        //autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {

        },
        editData: {
            IMS_ROAD_CODE: IMS_ROAD_CODE
        },
        editurl: "/Execution/UpdateRoadProgressDetails",
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });

}

/*Added for Editing Grid Row*/
function EditRoadProgressDetails(paramFileID) {
    jQuery("#tbPhysicalRoadList").editRow(paramFileID);
    $('#tbPhysicalRoadList').jqGrid('showCol', 'Save');
    //alert(paramFileID);
}

function SaveRoadProgressDetails(paramFileID) {
    //alert(paramFileID);
    var id = new Array();
    id = paramFileID.split('$');
    //$('#YEAR').val(id[1]);
    //$('#MONTH').val(id[2]);
    jQuery("#tbPhysicalRoadList").saveRow(id[0], checksave);
    jQuery("#tbPhysicalRoadList").trigger('reload');
}

function CancelRoadProgressDetails(paramFileID) {
    var id = new Array();
    id = paramFileID.split('$');

    $('#tbPhysicalRoadList').jqGrid('hideCol', 'Save');
    jQuery("#tbPhysicalRoadList").restoreRow(id[0]);
    //alert(paramFileID);
}

function checksave(result) {
    $('#tbPhysicalRoadList').jqGrid('hideCol', 'Save');
    if (result.responseText == "true") {
        alert('Details Updated Successfully.');
        return true;
    }
    else if (result.responseText != "") {
        alert(result.responseText.replace('"', "").replace('"', ""));
        return false;
    }
}

function ValidateLength(value, colname) {

    if (value.trim().length == 0) {
        return ["Please Enter "+ colname +"."];
    }
    else if (!value.match(/^[0-9]+(\.[0-9][0-9]?)?$/)) {
        return [" Decimal Numbers are allowed upto 2 digits."];
    }
    else {
        return [true, ""];
    }
}
/*Editing Grid Row Ends*/

function LoadFinancialDetails(IMS_ROAD_CODE) {

    jQuery("#tbFinancialRoadList").jqGrid('GridUnload');

    jQuery("#tbFinancialRoadList").jqGrid({
        url: '/Execution/GetFinancialProgressListForITNO',
        datatype: "json",
        mtype: "POST",
        postData: { roadCode: IMS_ROAD_CODE },
        colNames: ['Year', 'Month', 'Upto Last Month', 'During This Month', 'Total', 'Upto Last Month', 'During This Month', 'Total', 'Is Final Payment Made', 'Date', 'Delete'],
        colModel: [
                            { name: 'EXEC_PROG_YEAR', index: 'EXEC_PROG_YEAR', height: 'auto', width: 55, align: "center", search: false },
                            { name: 'EXEC_PROG_MONTH', index: 'EXEC_PROG_MONTH', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_VALUEOFWORK_LASTMONTH', index: 'EXEC_VALUEOFWORK_LASTMONTH', height: 'auto', width: 100, align: "center", search: true },
                            { name: 'EXEC_VALUEOFWORK_THISMONTH', index: 'EXEC_VALUEOFWORK_THISMONTH', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'TOTAL', index: 'TOTAL', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'EXEC_PAYMENT_LASTMONTH', index: 'EXEC_PAYMENT_LASTMONTH', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'EXEC_PAYMENT_THISMONTH', index: 'EXEC_PAYMENT_THISMONTH', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'TOTAL_PAYMENT', index: 'TOTAL_PAYMENT', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'EXEC_FINAL_PAYMENT_FLAG', index: 'EXEC_FINAL_PAYMENT_FLAG', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'EXEC_FINAL_PAYMENT_DATE', index: 'EXEC_FINAL_PAYMENT_DATE', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'b', width: 50, align: "center", search: false },

        ],
        pager: jQuery('#pagerFinancialRoadList'),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "EXEC_PROG_YEAR,EXEC_PROG_MONTH",
        sortorder: "desc",
        caption: "&nbsp;&nbsp; Financial Progress List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });

    jQuery("#tbFinancialRoadList").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'EXEC_VALUEOFWORK_LASTMONTH', numberOfColumns: 3, titleText: 'Value of Work Done(Rs. in Lakh)' },
          { startColumnName: 'EXEC_PAYMENT_LASTMONTH', numberOfColumns: 3, titleText: 'Payment Made(Rs. in Lakh)' }
        ]
    });

}

//deletes the Physical road progress details
function DeleteRoadPhysicalProgress(urlparameter) {

    if (confirm("Are you sure you want to delete Physical Road details?")) {

        $.ajax({
            type: 'POST',
            url: '/Execution/DeletePhysicalRoadDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert("Physical road progress details deleted successfully");
                    $("#tbPhysicalRoadList").trigger('reloadGrid');
                    //$("#divAddRoadProgress").html('');
                    $("#tbExecutionList").trigger('click');
                }
                else if (data.success == false) {
                    alert("Physical road progress details  is in use and can not be deleted.");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }
        });
    }
    else {
        return false;
    }

}

function DeleteFinancialProgress(urlparameter) {

    if (confirm("Are you sure you want to delete Financial details?")) {
        $.ajax({
            type: 'POST',
            url: '/Execution/DeleteFinancialDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert("Financial progress details deleted successfully");
                    $("#tbFinancialRoadList").trigger('reloadGrid');
                }
                else if (data.success == false) {
                    alert("Financial progress details is in use and can not be deleted.");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.responseText);
            }
        });
    }
    else {
        return false;
    }

}

function LoadPhysicalLSBDetails(IMS_ROAD_CODE) {

    jQuery("#tbLSBPhysicalRoadList").jqGrid('GridUnload');

    jQuery("#tbLSBPhysicalRoadList").jqGrid({
        url: '/Execution/GetLSBPhysicalProgressListForITNO',
        datatype: "json",
        mtype: "POST",
        postData: { roadCode: IMS_ROAD_CODE },
        colNames: ['Month', 'Year', 'Work Status', 'Cutoff/raft/Individual footing', 'Floor Protection', 'Sinking', 'Bottom Pluggings', 'Top Pluggings', 'Well Caps', 'Pier/Abutment Shaft', 'Pier/Abutment Caps', 'Bearings', 'Deck Slab', 'Wearing Coat', 'Posts & Railing', 'Road Work', 'CD Work', 'Bridge Length Completed', 'Approach Work Completed', 'Delete'],
        colModel: [
                            { name: 'EXEC_PROG_MONTH', index: 'EXEC_PROG_MONTH', height: 'auto', width: 40, align: "center", search: false },
                            { name: 'EXEC_PROG_YEAR', index: 'EXEC_PROG_YEAR', height: 'auto', width: 40, align: "left", search: false },
                            { name: 'EXEC_ISCOMPLETED', index: 'EXEC_ISCOMPLETED', height: 'auto', width: 60, align: "left", search: true },
                            { name: 'EXEC_RAFT', index: 'EXEC_RAFT', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_FLOOR_PROTECTION', index: 'EXEC_FLOOR_PROTECTION', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_SINKING', index: 'EXEC_SINKING', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_BOTTOM_PLUGGING', index: 'EXEC_BOTTOM_PLUGGING', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_TOP_PLUGGING', index: 'EXEC_TOP_PLUGGING', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_WELL_CAP', index: 'EXEC_WELL_CAP', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_PIER_SHAFT', index: 'EXEC_PIER_SHAFT', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_PIER_CAP', index: 'EXEC_PIER_CAP', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_BEARINGS', index: 'EXEC_BEARINGS', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_DECK_SLAB', index: 'EXEC_DECK_SLAB', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_WEARING_COAT', index: 'EXEC_WEARING_COAT', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_POSTS_RAILING', index: 'EXEC_POSTS_RAILING', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_APP_ROAD_WORK', index: 'EXEC_APP_ROAD_WORK', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_APP_CD_WORKS', index: 'EXEC_APP_CD_WORKS', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_APP_COMPLETED', index: 'EXEC_APP_COMPLETED', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_BRIDGE_COMPLETED', index: 'EXEC_BRIDGE_COMPLETED', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'a', width: 40, align: "center", search: false },

        ],
        pager: jQuery('#pagerLSBPhysicalRoadList'),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'EXEC_PROG_YEAR,EXEC_PROG_MONTH',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; LSB Physical Progress List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
        },
        loadError: function (xhr, status, error) {

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
//deletes the physical lsb progress
function DeleteLSBPhysicalProgress(urlparameter) {

    if (confirm("Are you sure you want to delete Physical LSB details?")) {
        $.ajax({
            type: 'POST',
            url: '/Execution/DeletePhysicalLSBDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert("Physical progress details of LSB deleted successfully");
                    $("#tbLSBPhysicalRoadList").trigger('reloadGrid');
                }
                else if (data.success == false) {
                    alert("Physical progress details of LSB is in use and can not be deleted.");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }
        });
    }
    else {
        return false;
    }

}

//populates result according to changed value
function FillInCascadeDropdown(map, dropdown, action) {
    var message = '';

    $(dropdown).empty();

    $.post(action, map, function (data) {
        $.each(data, function () {

            if (this.Selected == true) {
                $(dropdown).append("<option selected value=" + this.Value + ">" + this.Text + "</option>");
            }
            else {
                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            }
        });

    }, "json");
}
//load the list for road progress
function AddPhysicalDetails(urlparameter)
{
    if ($("#divLSBPhysicalDetails").is(':visible')) {
        $("#divLSBPhysicalDetails").hide();
    }

    if ($("#divPhysicalDetails").is(':hidden')) {
        $("#divPhysicalDetails").show();
    }

    if ($("#divFinancialDetails").is(':visible')) {
        $("#divFinancialDetails").hide();
    }

    LoadPhysicalRoadDetails(urlparameter);
}
//load the list for lsb progress
function AddPhysicalLSBDetails(urlparameter) {
    
    if ($("#divLSBPhysicalDetails").is(':hidden')) {
        $("#divLSBPhysicalDetails").show();
    }

    if ($("#divPhysicalDetails").is(':visible')) {
        $("#divPhysicalDetails").hide();
    }

    if ($("#divFinancialDetails").is(':visible')) {
        $("#divFinancialDetails").hide();
    }

    LoadPhysicalLSBDetails(urlparameter);
}
function AddFinancialDetails(urlparameter) {

    if ($("#divFinancialDetails").is(':hidden')) {
        $("#divFinancialDetails").show();
    }

    if ($("#divLSBPhysicalDetails").is(':visible')) {
        $("#divLSBPhysicalDetails").hide();
    }

    if ($("#divPhysicalDetails").is(':visible')) {
        $("#divPhysicalDetails").hide();
    }

    LoadFinancialDetails(urlparameter);
}