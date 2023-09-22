$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
    $.ajax({
        url: '/Master/SearchInfoDetails',
        type: "Get",
        dataType: "html",
        success: function (data) {
            $("#dvSearchInfoDetails").html(data);
          
            $.unblockUI();
        },
        error: function (xhr, opt, error) {
            $.unblockUI();

            alert("An Error occured while proccessing your request.");
            return false;
        }

    });

    $("#btnCreateNew").click(function () {
        if ($("#dvAddInfoDetails").is(':hidden')) {
            $("#dvSearchInfoDetails").hide();

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $("#dvAddInfoDetails").load('/Master/AddEditInfoDetails?id='+$("#InfoType").val()+'',function () {
                $.unblockUI();
            });
            $("#dvAddInfoDetails").show('slow');            
            $("#btnCreateNew").hide('slow');
            $("#trSearchDetails").show();

        }
    });

    $("#btnSearchInfo").click(function () {
        if ($("#dvSearchInfoDetails").is(':hidden')) {

            $("#dvSearchInfoDetails").show();
            $("#dvAddInfoDetails").hide('slow');
            $("#btnCreateNew").show();
            $("#trSearchDetails").hide();

        }
    });

    if (!$("#dvSearchInfoDetails").is(":visible")) {
        LoadInfoDetailsList();
    }
});
function LoadInfoDetailsList() {
    $("#tblInfoDetails").jqGrid('GridUnload');
    $('#tblInfoDetails').jqGrid({
        url: '/Master/InfoDetailsList',
        datatype: "json",
        mtype: "POST",
        postData: { Type: $("#InfoType").val(), StateCode: $("#ddlAllState").val() },
        colNames: ['Name', 'Designation', 'Office', 'State', 'Office Telephone', 'Residential Telephone', 'Mobile', 'Fax', 'Email', 'Type','Sort Order', 'Status', 'Edit', 'Delete', 'Change Status'],
        colModel: [
                      { name: 'MAST_INFO_NAME', index: 'MAST_INFO_NAME', height: 'auto', width: 50, align: "left", sortable: true },
                      { name: 'MAST_INFO_DESIGNATION', index: 'MAST_INFO_DESIGNATION', height: 'auto', width: 50, align: "left", sortable: true },
                      { name: 'MAST_INFO_OFFICE', index: 'MAST_INFO_OFFICE', height: 'auto', width: 80, align: "left", sortable: true },
                      { name: 'MAST_STATE_CODE', index: 'MAST_STATE_CODE', height: 'auto', width: 40, align: "left", sortable: false },
                      { name: 'MAST_INFO_TELE_OFF', index: 'MAST_INFO_TELE_OFF', height: 'auto', width: 40, align: "left", sortable: true },
                      { name: 'MAST_INFO_TELE_RES', index: 'MAST_INFO_TELE_RES', height: 'auto', width: 40, align: "left", sortable: true },
                      { name: 'MAST_INFO_MOBILE', index: 'MAST_INFO_MOBILE', height: 'auto', width: 40, align: "left", sortable: true },
                      { name: 'MAST_INFO_FAX', index: 'MAST_INFO_FAX', height: 'auto', width: 40, align: "left", sortable: true },
                      { name: 'MAST_INFO_EMAIL', index: 'MAST_INFO_EMAIL', height: 'auto', width: 50, align: "left", sortable: true },
                      { name: 'MAST_INFO_TYPE', index: 'MAST_INFO_TYPE', height: 'auto', width: 25, align: "left", sortable: true },
                      { name: 'MAST_INFO_SORT', index: 'MAST_INFO_SORT', height: 'auto', width: 25, align: "center", sortable: true },
                      { name: 'MAST_INFO_ACTIVE', index: 'MAST_INFO_ACTIVE', height: 'auto', width: 25, align: "left", sortable: true },

                      { name: 'edit', width: 20, sortable: false, resize: false, formatter: FormatColumnEdit, align: "center", sortable: false },
                      { name: 'delete', width: 20, sortable: false, resize: false, formatter: FormatColumnDelete, align: "center", sortable: false },
                      { name: 'changeStatus', width: 25, sortable: false, resize: false, formatter: FormatColumnChangeStatus, align: "center", sortable: false }
        ],
        pager: jQuery('#dvPagerInfoDetails'),
        rowNum: 10,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_INFO_SORT',
        sortorder: "asc",
        caption: $("#InfoType").val() == "A" ? "Account Trainers List" : $("#InfoType").val() == "M" ? "MRD Details List" : $("#InfoType").val() == "N" ? "NRRDA Details List" : "Trainers Details List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: true,
        shrinkToFit: true,
        loadComplete: function () {

            $("#jqgh_tblInfoDetails_rn").html("Sr.<br/> No");
            
        }
    });

}

function FormatColumnEdit(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Information Details' onClick ='EditInformationDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function FormatColumnDelete(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Information Details' onClick ='DeleteInformationDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function FormatColumnChangeStatus(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-refresh' title='Change Status' onClick ='ChangeInformationStatus(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function EditInformationDetails(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Master/GetInfoDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#dvAddInfoDetails").html(data);
            $("#dvAddInfoDetails").show();
            $("#btnCreateNew").hide('slow');
            if ($("#dvSearchInfoDetails").is(":visible")) {
                $('#dvSearchInfoDetails').hide('slow');
            }
            $("#trSearchDetails").show();
            if (data.success == false) {
                alert('Error occurred while processing your request.');
            }
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
            alert("An error occured while processing your request.");
            return false;
        }
    })
}

function DeleteInformationDetails(urlparameter) {
    if (confirm("Are you sure you want to delete information details?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteInfoDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {

                    //var infoType = $("#InfoType").val();
                    //var type;
                    //if (infoType == "A")
                    //{
                    //    type = "Account Trainers";
                    //}
                    //else if (infoType == "M")
                    //{
                    //    type = "MRD";
                    //}
                    //else if (infoType == "N") {
                    //    type = "NRRDA";
                    //}
                    //else if(infoType=="T") {
                    //    type = "Trainers";
                    //}

                    alert('Information details deleted successfully.');
                    //$('#tblInfoDetails').trigger('reloadGrid');
                    //$("#dvAddInfoDetails").load('/Master/AddEditInfoDetails?id=' + $("#InfoType").val() + '');
                    $("#dvSearchInfoDetails").show();
                    $("#dvAddInfoDetails").hide('slow');
                    $("#btnCreateNew").show();
                    $("#trSearchDetails").hide();
                    $('#tblInfoDetails').trigger('reloadGrid');
                }
                else if (data.success == false) {
                    alert('Information details is in use and can not be deleted.');
                    return false;
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.responseText);
                alert("An error occured while proccessing your request.");
                return false;
            }
        });
    }
    else {
        return false;
    }
}

function ChangeInformationStatus(urlparameter) {
    if (confirm("Are you sure you want to change status?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/ChangeInfoStatus/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert('Status details changed successfully.');
                    //$('#tblInfoDetails').trigger('reloadGrid');                   
                    //$("#dvAddInfoDetails").load('/Master/AddEditInfoDetails?id=' + $("#InfoType").val() + '');
                    $("#dvSearchInfoDetails").show();
                    $("#dvAddInfoDetails").hide('slow');
                    $("#btnCreateNew").show();
                    $("#trSearchDetails").hide();
                    $('#tblInfoDetails').trigger('reloadGrid');
                }
                else if (data.success == false) {
                    alert('An error occued while proccessing your request.');
                    return false;
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("An error occured while processing your request.");
                return false;
                //alert(xhr.responseText);
            }
        });
    }
    else {
        return false;
    }
}

