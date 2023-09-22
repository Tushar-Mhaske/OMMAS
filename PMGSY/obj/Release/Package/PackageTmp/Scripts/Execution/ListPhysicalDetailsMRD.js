var isValid;
$(document).ready(function () {

    //disable enter key
    $(":input").bind('keypress', function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    })

    var IMS_ROAD_CODE = $("#prRoadCode").val();

    //function call for loading the list of physical road progress
    $("#tbPhysicalRoadList").jqGrid('GridUnload');
    LoadPhysicalRoadDetails(IMS_ROAD_CODE);

    $("#btnAddRoadProgress").click(function () {

        $("#divAddRoadProgress").load("/Execution/AddPhysicalRoadProgress/" + IMS_ROAD_CODE, function () {

            $.validator.unobtrusive.parse($('#divAddRoadProgress'));
        });
    });
});
//Load the Physical details of road
function LoadPhysicalRoadDetails(IMS_ROAD_CODE) {

    jQuery("#tbPhysicalRoadList").jqGrid({
        url: '/Execution/GetRoadPhysicalProgressListMRD',
        datatype: "json",
        mtype: "GET",
        postData: { roadCode: IMS_ROAD_CODE },
        colNames: ['Month', 'Year', 'Work Status', 'Preparatory Work (Length in Km.)', 'Subgrade Stage (Length in Km.)', 'Subbase (Length in Km.)', 'Base Course (Length in Km.)', 'Surface Course (Length in Km.)', 'Road Signs Stones (in Nos.)', 'CDWorks (in Nos.)', 'LS Bridges (in Nos.)', 'Miscellaneous (Length in Km.)', 'Completed (Length in Km.)', 'Edit', 'Delete'],
        colModel: [
                            { name: 'EXEC_PROG_MONTH', index: 'EXEC_PROG_MONTH', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_PROG_YEAR', index: 'EXEC_PROG_YEAR', height: 'auto', width: 50, align: "left", search: false },
                            { name: 'EXEC_ISCOMPLETED', index: 'EXEC_ISCOMPLETED', height: 'auto', width: 100, align: "left", search: true },
                            { name: 'EXEC_PREPARATORY_WORK', index: 'EXEC_PREPARATORY_WORK', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'EXEC_EARTHWORK_SUBGRADE', index: 'EXEC_EARTHWORK_SUBGRADE', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'EXEC_SUBBASE_PREPRATION', index: 'EXEC_SUBBASE_PREPRATION', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_BASE_COURSE', index: 'EXEC_BASE_COURSE', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_SURFACE_COURSE', index: 'EXEC_SURFACE_COURSE', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'EXEC_SIGNS_STONES', index: 'EXEC_SIGNS_STONES', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'EXEC_CD_WORKS', index: 'EXEC_CD_WORKS', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_LSB_WORKS', index: 'EXEC_LSB_WORKS', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_MISCELANEOUS', index: 'EXEC_MISCELANEOUS', height: 'auto', width: 70, align: "center", search: false },
                            { name: 'EXEC_COMPLETED', index: 'EXEC_COMPLETED', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'a', width: 40, align: "center", search: false, sortable: false },
                            { name: 'b', width: 40, align: "center", search: false, sortable: false, hidden: true },

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

            //$("#gview_tbPhysicalRoadList > .ui-jqgrid-titlebar").hide();
            $("#tbPhysicalRoadList #pagerPhysicalRoadList").css({ height: '40px' });
            $("#pagerPhysicalRoadList_left").html("<input type='button' style='margin-left:27px' id='idAddPhysicaRoad' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddPhysicalRoadProgress(" + IMS_ROAD_CODE + ");return false;' value='Add Road Progress'/>")

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

}
//returns the physical road details for updation
function EditRoadPhysicalProgress(urlparameter) {

    $("#divAddRoadProgress").load("/Execution/EditPhysicalRoadProgressMRD/" + urlparameter, function (data) {
        if (data.success == false) {
            alert('Error occurred while processing your request.');
        }
        $("#divAddRoadProgress").show();
        $.validator.unobtrusive.parse($('#divAddRoadProgress'));
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
// loads the add view for adding physical road progress
function AddPhysicalRoadProgress(IMS_ROAD_CODE) {

    ValidateRoadDetails(IMS_ROAD_CODE);
    if (isValid == true) {
        $("#divAddRoadProgress").load("/Execution/AddPhysicalRoadProgressMRD/" + IMS_ROAD_CODE, function (data) {
            if (data.success == false) {
                alert(data.message);
            }
            else {
                $("#divAddRoadProgress").show();
                $.validator.unobtrusive.parse($('#divAddRoadProgress'));
            }
        });
    }
    else if (isValid == false) {
        return false;
    }


}
function ValidateRoadDetails(IMS_PR_ROAD_CODE) {
    $.ajax({

        type: 'POST',
        url: '/Execution/CheckPhysicalRoadDetails/' + IMS_PR_ROAD_CODE,
        async: false,
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (data.success == false) {
                alert(data.message);
                isValid = false;
                return false;
            }
            else if (data.success == true) {
                isValid = true;
                return true;
            }
        },
        error: function () {
            alert('Error occurred while processing your request.');
        }
    });
}

