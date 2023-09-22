var isValidate;
$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmAddCDWorks'));

    $("input").bind('keypress', function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    })

    var IMS_PR_ROAD_CODE = $("#prRoadCode").val();
    $("#tbCDWorksList").jqGrid('GridUnload');

    LoadCdWorksList(IMS_PR_ROAD_CODE);

    $("#btnAddCDWorksProgress").click(function () {
        $("#divAddCDWorksProgress").load("/Execution/AddCDWorksProgress/" + IMS_PR_ROAD_CODE, function (data) {
            if (data.success == false) {
                alert('Error occurred while processing your request.');
                return false;
            }
            $.validator.unobtrusive.parse($('#frmAddCDWorks'));
        });
    });

});
//loads the CDWorks details list
function LoadCdWorksList(IMS_PR_ROAD_CODE) {

    jQuery("#tbCDWorksList").jqGrid({
        url: '/Execution/GetCDWorksList',
        datatype: "json",
        mtype: "POST",
        postData: { roadCode:IMS_PR_ROAD_CODE},
        colNames: ['Chainage', 'Type', 'Edit', 'Delete'],
        colModel: [
                            
                            { name: 'EXEC_RCD_CHAINAGE', index: 'EXEC_RCD_CHAINAGE', height: 'auto', width: 300, align: "center", search: false },
                            { name: 'MAST_CDWORKS_NAME', index: 'MAST_CDWORKS_NAME', height: 'auto', width: 355, align: "left", search: false },
                            { name: 'a', width: 200, sortable: false, resize: false, align: "center", search: false },
                            { name: 'b', width: 200, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerCDWorksList'),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'EXEC_RCD_CHAINAGE',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Execution Details List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
            //$("#gview_tbCDWorksList > .ui-jqgrid-titlebar").hide();
            $("#tbCoreNetworkList #pagerCDWorksList").css({ height: '40px' });
            $("#pagerCDWorksList_left").html("<input type='button' style='margin-left:27px' id='idAddCdWorks' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddCDWorks("+IMS_PR_ROAD_CODE+");return false;' value='Add CDWorks'/>")

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
function EditCDWorksDetails(urlparameter) {

    jQuery('#tbCDWorksList').jqGrid('setSelection', urlparameter);

    $("#divAddCDWorksProgress").load("/Execution/EditCDWorksDetails/" + urlparameter, function (data) {
        if (data.success == false) {
            alert(data.message);
        }
    });
}
//deletes the CDWorks details
function DeleteCDWorksDetails(urlparameter) {

    if (confirm("Are you sure you want to delete CDWorks details?")) {
        $.ajax({
            type: 'POST',
            url: '/Execution/DeleteCdWorksDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert("CDWorks details deleted successfully");
                    $("#tbCDWorksList").trigger('reloadGrid');
                }
                else if (data.success == false) {
                    alert("CDWorks details  is in use and can not be deleted.");
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
function AddCDWorks(IMS_PR_ROAD_CODE) {

    ValidateCDWorksCount(IMS_PR_ROAD_CODE);
    if (isValidate == true) {
    
        $("#divAddCDWorksProgress").load("/Execution/AddCDWorksProgress/" + IMS_PR_ROAD_CODE, function (data) {
            if (data.success == false) {
                alert('Error occurred while processing your request.');
                return false;
            }
            else {
                $.validator.unobtrusive.parse($('#frmAddCDWorks'));
            }
        });
        
    }
    else {
        return false;
    }

    
}
function ValidateCDWorksCount(IMS_PR_ROAD_CODE)
{
    $.ajax({

        type: 'POST',
        url: '/Execution/CheckCDWorksCount/' + IMS_PR_ROAD_CODE,
        async: false,
        cache: false,
        dataType:'json',
        success: function (data) {
            if (data.success == false) {
                alert(data.message);
                isValidate = false;
                return false;
            }
            else if (data.success == true) {
                isValidate = true;
                return true;
            }
        },
        error: function () {
            alert('Error occurred while processing your request.');
        }
    });
}

