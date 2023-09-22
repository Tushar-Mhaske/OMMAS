var isValid;
$(document).ready(function () {

     //disable enter key
    $(":input").bind('keypress', function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    })

    var IMS_ROAD_CODE = $("#prRoadCode").val();

    //load LSB Physical details
    $("#tbLSBPhysicalRoadList").jqGrid('GridUnload');
    LoadPhysicalLSBDetails(IMS_ROAD_CODE);


    $("#btnAddLSBProgress").click(function () {
        
        $("#divAddLSBProgress").load("/Execution/AddPhysicalLSBProgress/" + IMS_ROAD_CODE, function () {

            $.validator.unobtrusive.parse($('#divAddLSBProgress'));
        });
        
    });
});
function LoadPhysicalLSBDetails(IMS_ROAD_CODE) {

    jQuery("#tbLSBPhysicalRoadList").jqGrid({
        url: '/Execution/GetLSBPhysicalProgressList',
        datatype: "json",
        mtype: "POST",
        postData: { roadCode: IMS_ROAD_CODE },
        colNames: ['Month', 'Year', 'Work Status', 'Cutoff/raft/Individual footing', 'Floor Protection', 'Sinking', 'Bottom Pluggings', 'Top Pluggings', 'Well Caps', 'Pier/Abutment Shaft', 'Pier/Abutment Caps', 'Bearings', 'Deck Slab', 'Wearing Coat', 'Posts & Railing', 'Road Work', 'CD Work', 'Bridge Length Completed', 'Approach Work Completed', 'Edit','Delete'],
        colModel: [
                            { name: 'EXEC_PROG_MONTH', index: 'EXEC_PROG_MONTH', height: 'auto', width: 40, align: "center", search: false },
                            { name: 'EXEC_PROG_YEAR', index: 'EXEC_PROG_YEAR', height: 'auto', width: 40, align: "left", search: false },
                            { name: 'EXEC_ISCOMPLETED', index: 'EXEC_ISCOMPLETED', height: 'auto', width: 50, align: "left", search: true },
                            { name: 'EXEC_RAFT', index: 'EXEC_RAFT', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_FLOOR_PROTECTION', index: 'EXEC_FLOOR_PROTECTION', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_SINKING', index: 'EXEC_SINKING', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_BOTTOM_PLUGGING', index: 'EXEC_BOTTOM_PLUGGING', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_TOP_PLUGGING', index: 'EXEC_TOP_PLUGGING', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_WELL_CAP', index: 'EXEC_WELL_CAP', height: 'auto', width: 40, align: "center", search: false },
                            { name: 'EXEC_PIER_SHAFT', index: 'EXEC_PIER_SHAFT', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_PIER_CAP', index: 'EXEC_PIER_CAP', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_BEARINGS', index: 'EXEC_BEARINGS', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_DECK_SLAB', index: 'EXEC_DECK_SLAB', height: 'auto', width: 40, align: "center", search: false },
                            { name: 'EXEC_WEARING_COAT', index: 'EXEC_WEARING_COAT', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_POSTS_RAILING', index: 'EXEC_POSTS_RAILING', height: 'auto', width: 40, align: "center", search: false },
                            { name: 'EXEC_APP_ROAD_WORK', index: 'EXEC_APP_ROAD_WORK', height: 'auto', width: 40, align: "center", search: false },
                            { name: 'EXEC_APP_CD_WORKS', index: 'EXEC_APP_CD_WORKS', height: 'auto', width: 40, align: "center", search: false },
                            { name: 'EXEC_APP_COMPLETED', index: 'EXEC_APP_COMPLETED', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_BRIDGE_COMPLETED', index: 'EXEC_BRIDGE_COMPLETED', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'a', width: 30, align: "center", search: false },
                            { name: 'b', width: 40, align: "center", search: false ,hidden:true},

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

            //$("#gview_tbLSBPhysicalRoadList > .ui-jqgrid-titlebar").hide();
            $("#tbLSBPhysicalRoadList #pagerLSBPhysicalRoadList").css({ height: '40px' });
            $("#pagerLSBPhysicalRoadList_left").html("<input type='button' style='margin-left:27px' id='idAddLSBProgress' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddLSBProgress(" + IMS_ROAD_CODE + ");return false;' value='Add LSB Progress'/>")
        },
        loadError: function (xhr, status, error) {

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
function EditLSBPhysicalProgress(urlparameter) {

    $("#divAddLSBProgress").load("/Execution/EditLSBPhysicalProgress/" + urlparameter, function (data) {
        if (data.success == false) {
            alert(data.message);
        }
        else
        {
            $("#divAddLSBProgress").show('slow');
            $.validator.unobtrusive.parse($('#divAddLSBProgress'));
        }
    });

}

function DeleteLSBPhysicalProgress(urlparameter) {

    if (confirm("Are you sure you want to delete Physical LSB details?")) {
        $.ajax({
            type: 'POST',
            url: '/Execution/DeletePhysicalLSBDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success==true) {
                    alert("Physical road progress details deleted successfully");
                    $("#tbLSBPhysicalRoadList").trigger('reloadGrid');
                }
                else if (data.success == false) {
                    alert("Physical LSB progress details is in use and can not be deleted.");
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
function AddLSBProgress(IMS_ROAD_CODE) {

    ValidateLSBDetails(IMS_ROAD_CODE);

    if (isValid == true)
    {
        $("#divAddLSBProgress").load("/Execution/AddPhysicalLSBProgress/" + IMS_ROAD_CODE, function () {

            $.validator.unobtrusive.parse($('#divAddLSBProgress'));
            $("#divAddLSBProgress").show('slow');
        });
    }
    else if (isValid == false)
    {
        return false;
    }
}
function ValidateLSBDetails(IMS_PR_ROAD_CODE) {
    $.ajax({

        type: 'POST',
        url: '/Execution/CheckPhysicalLSBDetails/' + IMS_PR_ROAD_CODE,
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