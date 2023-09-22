$(document).ready(function () {

    //disable enter key
    $(":input").bind('keypress', function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    })

    var IMS_ROAD_CODE = $("#prRoadCode").val();

    //function call for loading the list of physical road progress
    $("#tbExecutingOfficerList").jqGrid('GridUnload');

    LoadExecutingOfficerDetails(IMS_ROAD_CODE);

});

function LoadExecutingOfficerDetails(IMS_ROAD_CODE) {

    jQuery("#tbExecutingOfficerList").jqGrid({
        url: '/Execution/GetExecutingOfficerList',
        datatype: "json",
        mtype: "POST",
        postData: { roadCode: IMS_ROAD_CODE },
        colNames: ['Month', 'Year', 'Designation', 'Executing Officer', 'Edit', 'Delete'],
        colModel: [
                            { name: 'EXEC_MONTH', index: 'EXEC_MONTH', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'EXEC_YEAR', index: 'EXEC_YEAR', height: 'auto', width: 200, align: "left", search: false },
                            { name: 'MAST_DESIG_CODE', index: 'MAST_DESIG_CODE', height: 'auto', width: 200, align: "left", search: true },
                            { name: 'MAST_OFFICER_CODE', index: 'MAST_OFFICER_CODE', height: 'auto', width: 350, align: "left", search: true },
                            { name: 'edit', width: 50, align: "center", search: false, sortable: false },
                            { name: 'delete', width: 50, align: "center", search: false, sortable: false },
        ],
        pager: jQuery('#pagerExecutingOfficerList').width(20),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'EXEC_MONTH,EXEC_YEAR',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Executing Officer List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        width: '100%',
        cmTemplate: { title: false },
        loadComplete: function (data) {
            //$("#gview_tbExecutingOfficerList > .ui-jqgrid-titlebar").hide();
            $("#tbExecutingOfficerList #pagerExecutingOfficerList").css({ height: '30px' });
            $("#pagerExecutingOfficerList_left").html("<input type='button' style='margin-left:27px' id='btnAddExecutingOficer' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddExecutingOfficer(" + IMS_ROAD_CODE + ");return false;' value='Add Executing Officer'/>")
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

function EditExecutingOfficerDetails(urlparameter) {

    
    $("#divExecutingOfficer").load("/Execution/EditExecutingOfficerDetails/" + urlparameter, function (data) {
        if (data.success == false) {
            alert('Error occurred while processing your request.');
        }

        var code = $("#IMS_PR_ROAD_CODE").val() + "," + $('#ddlDesignation option:selected').val();

        FillInExecutingOfificerDropdown({ userType: $("#ddlDesignation").find(":selected").val() },
                 "#ddlExecutingOfficer", "/Execution/GetExecutingOfficerByDesig?imsPrRoadCode_DesignationCode=" + code);
    });
    $("#divExecutingOfficer").show();
}

function DeleteExecutingOfficerDetails(urlparameter) {

    if (confirm("Are you sure you want to delete Executing Officer details?")) {

        $.ajax({
            type: 'POST',
            url: '/Execution/DeleteExecutingOfficerDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert("Executing Officer details deleted successfully");
                    $("#tbExecutingOfficerList").trigger('reloadGrid');
                    $("#divExecutingOfficer").html('');
                }
                else if (data.success == false) {
                    alert("Executing Officer details is in use and can not be deleted.");
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
function AddExecutingOfficer(IMS_ROAD_CODE) {

    $("#divExecutingOfficer").load("/Execution/AddExecutingOfficerDetails/" + IMS_ROAD_CODE, function () {
        $.validator.unobtrusive.parse($('#divExecutingOfficer'));
    });
    $("#divExecutingOfficer").show();
}
