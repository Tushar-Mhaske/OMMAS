$(document).ready(function () {

    if ($("#frmMasterContReg") != null) {
        $.validator.unobtrusive.parse("#frmMasterContReg");
    }
    $("#dvhdCreateNewContRegDetails").hide();
    
 


    $('#btnBackToContList').click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $('#dvMasterDataEntry').load('/Master/ListContractor', function (e) {
            $.unblockUI();
        });

    });
    $('#tblMasterContRegList').jqGrid({
        url: '/Master/GetMasterContractorRegList',
        datatype: 'json',
        mtype: "POST",
        colNames: ['State Name', 'Registration Number', 'Class Name', 'Valid From', 'Valid To', 'Registered Office', 'Status', 'Change Status', 'Action'],
        colModel: [
            { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 180, align: "left", sortable: true },
            { name: 'MAST_CON_REG_NO', index: 'MAST_CON_REG_NO', height: 'auto', width: 100, align: "left", sortable: true },
         { name: 'MAST_CON_CLASS_TYPE_NAME', index: 'MAST_CON_CLASS_TYPE_NAME', height: 'auto', width: 100, align: "left", sortable: true },
         { name: 'MAST_CON_VALID_FROM', index: 'MAST_CON_VALID_FROM', height: 'auto', width: 70, align: "left", sortable: true },
         { name: 'MAST_CON_VALID_TO', index: 'MAST_CON_VALID_TO', height: 'auto', width: 70, align: "left", sortable: true },
         { name: 'MAST_REG_OFFICE', index: 'MAST_REG_OFFICE', height: 'auto', width: 100, align: "left", sortable: true },

         { name: 'MAST_REG_STATUS', index: 'MAST_REG_STATUS', height: 'auto', width: 80, align: "left", sortable: true },
         { name: 'ChangeStatus', index: 'ChangeStatus', height: 'auto', width: 80, align: "center", sortable: false },
         { name: 'a', width: 80, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#divPagerMasterContReg'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_STATE_NAME',
        sortorder: "asc",
        caption: 'Contractor/Supplier Registration List',
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function () { },

        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {

                alert(xht.responseText);
                window.location.href = "Login/login";
            }
            else {
                alert("Invalid Data. Please Check and Try Again");
            }
        }
    });
});


function FormatColumn(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none;'><span class='ui-icon ui-icon-pencil' title='Edit Contrator/Supplier Registration Details' onClick ='editRegData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;'><span class='ui-icon ui-icon-trash' title='Delete Contrator/Supplier Registration Details' onClick ='deleteRegData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }
}

function editRegData(id) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/EditMasterContractorReg/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche:false,
        contentType: "application/json; charset=utf-8",       
        success: function (data) {        
            $("#dvContRegDetails").show();
            $("#dvContRegDetails").html(data);
            $.unblockUI();
        },
        error: function (xht, ajaxOptions, throwError)
        {
            alert(xht.responseText);
            $.unblockUI();
        }
    });
}

function deleteRegData(urlParam)
{  
    if (confirm("Are you sure you want to delete Contractor/Supplier registration details?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/Master/DeleteMasterContractorReg/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $("#tblMasterContRegList").trigger('reloadGrid');
                    $("#dvContRegDetails").load("/Master/AddEditMasterContractorReg");
                }
                else {
                    alert(data.message);
                }
                $.unblockUI();
            },
            error: function (xht, ajaxOptions, throwError)
            {
                alert(xht.responseText);
                $.unblockUI();
            }

        });
    }
    else {
        return false;
    }


}

function ChangeStatusToInActive(urlParam){

    if (confirm("Are you sure you want to 'DeActivate' Contractor/Supplier Registration details?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Master/ContractorRegChangeStatus_InActive/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $("#tblMasterContRegList").trigger('reloadGrid');
                }
                else {
                    alert(data.message);
                }
                $.unblockUI();
            },
            error: function (xht, ajaxOptions, throwError) {
                alert(xht.responseText);
                $.unblockUI();
            }

        });
    }
    else {
        return false;
    }

}


function ChangeStatusToActive(urlParam) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/ContractorRegChangeStatus_Active/" + urlParam,
        type: "POST",
        dataType: "json",
        success: function (data) {
            if (data.success) {
                alert(data.message);
                $("#tblMasterContRegList").trigger('reloadGrid');
            }
            else {
                alert(data.message);
            }
            $.unblockUI();
        },
        error: function (xht, ajaxOptions, throwError) {
            alert(xht.responseText);
            $.unblockUI();
        }

    });

}

function LoadContractRegistrationDetail()
{
  
}