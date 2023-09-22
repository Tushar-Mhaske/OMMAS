$(document).ready(function () {

    //disable enter key
    $(":input").bind('keypress', function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    })

    var IMS_ROAD_CODE = $("#prRoadCode").val();


    LoadListRemarks(IMS_ROAD_CODE);
    
    //$("#divAddRemarks").hide();
    
});
function LoadListRemarks(IMS_PR_ROAD_CODE) {
    jQuery("#tbListRemarks").jqGrid({
        url: '/Execution/GetRemarksList',
        datatype: "json",
        mtype: "POST",
        postData: { proposalCode: IMS_PR_ROAD_CODE },
        colNames: ['Remarks', 'Edit', 'Delete'],
        colModel: [
                            { name: 'IMS_PROG_REMARKS', index: 'IMS_PROG_REMARKS', height: 'auto', width: 500, align: "left", search: false },
                            { name: 'a', width: 200, align: "center", formatter: FormatColumn, search: false },
                            { name: 'b', width: 200, align: "center", formatter: FormatColumn1, search: false },

        ],
        pager: jQuery('#pagerRemarks'),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "IMS_PROG_REMARKS",
        sortorder: "desc",
        caption: "&nbsp;&nbsp; Remarks",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
            
            if (data["records"] == 0)
            {
                $("#divAddRemarks").show();
                LoadAddView(IMS_PR_ROAD_CODE);
                $("#dvListremarks").hide();
            }
            else
            {
                $("#divAddRemarks").hide();
                $("#dvListremarks").show();
            }
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

function FormatColumn1(cellvalue, options, rowObject) {

    return "<center><span class='ui-icon ui-icon-trash' title='Delete Remark' onClick ='DeleteRemark(\"" + cellvalue.toString() + "\");'></span></center>";
}

function FormatColumn(cellvalue, options, rowObject) {

    return "<center><span class='ui-icon ui-icon-pencil' title='Edit Remark' onClick ='EditRemark(\"" + cellvalue.toString() + "\");'></span></center>";
}

function DeleteRemark(urlparameter) {
    if (confirm("Are you sure you want to delete Remark?")) {
        $.ajax({
            url: '/Execution/DeleteRemark',
            type: 'POST',
            beforeSend: function () {
                blockPage();
            },
            data: { IMS_PR_ROAD_CODE: urlparameter },
            success: function (response) {
                unblockPage();
                if (response.success == true) {
                    alert('Remark deleted successfully.');
                    $("#tbListRemarks").trigger('reloadGrid');
                }
                else {
                    alert("The Remarks is in use and can not be deleted.");
                }
            },
            error: function (xhr, AjaxOptions, thrownError) {
                alert("Error occured while processing the request.");
                unblockPage();
            }
        });
    }
    else {
        return false;
    }
}

function EditRemark(urlparameter) {

    $("#divAddRemarks").unload();
    $("#divAddRemarks").load('/Execution/EditRemark/' + urlparameter, function () {
        $.validator.unobtrusive.parse($('#divAddRemarks'));
        $("#divAddRemarks").show();
    });
    
}

function LoadAddView(urlparameter)
{
    $("#divAddRemarks").load('/Execution/AddProgressRemarks/' + urlparameter, function () {

        $.validator.unobtrusive.parse($('#divAddRemarks'));
        
    });
    
}