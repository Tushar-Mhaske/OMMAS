$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

$(document).ready(function () {
    LoadGrid();
    $.unblockUI();

    $('#btnAddNewDetails').click(function (e) {

        $("#dvContractorBankDetails").load('/Master/AddBankDetails/' + $("#MAST_CON_ID").val() + "$" + $("#RegStateCode").val());
        $("#dvContractorBankDetails").show('slow');
    });

    $("#btnCancelBankDetails").click(function (e) {

        if ($("#dvlstBankDetails").is(":visible")) {
            $("#dvlstBankDetails").hide('slow');
        }
        if ($('#tblBankDetailsList').is(":visible")) {
            $('#tblBankDetailsList').hide();
        }
        $('#tblstContractorReg').jqGrid("setGridState", "visible");

        $("#btnCreateNew").show();
        $("#btnSearch").hide();
        $("#dvSearchContractorReg").show("slow");

        $("#mainDiv").animate({
            scrollTop: 0
        });
    });
});
function FormatColumn(cellvalue, options, rowObject) {
    if (cellvalue.toString() == "") {
        return "<center><table><tr><td style='border:none;'><span>-</span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none;'><span class='ui-icon ui-icon-pencil' title='Edit Bank Details' onClick ='editDataBankDetails(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;'><span class='ui-icon ui-icon-trash' title='Delete Bank Details' onClick ='deleteDataBankDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
        //return "<center><table><tr><td  style='border:none;'><span class='ui-icon ui-icon-pencil' title='Edit Bank Details' onClick ='editDataBankDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }

}

//function FormatColumnDelete(cellvalue, options, rowObject) {
//    if (cellvalue.toString() == "") {
//        return "<center><table><tr><td style='border:none;'><span>-</span></td></tr></table></center>";
//    }
//    else {
//        //return "<center><table><tr><td  style='border:none;'><span class='ui-icon ui-icon-pencil' title='Edit Bank Details' onClick ='editDataBankDetails(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;'><span class='ui-icon ui-icon-trash' title='Delete Bank Details' onClick ='deleteDataBankDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
//        return "<center><table><tr><td  style='border:none;'><span class='ui-icon ui-icon-pencil' title='Edit Bank Details' onClick ='deleteDataBankDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
//    }

//}

//added by PP (01-05-2018)
function FormatColumnFinalize(cellvalue, options, rowObject) {
    if (cellvalue.toString() == "") {
        return "<center><table><tr><td style='border:none;'><span class='ui-icon ui-icon-locked' title='Bank Details are finalised'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none;'><span class='ui-icon ui-icon-plusthick' title='Finalize Bank Details' onClick ='finalizeBankDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }

}


function editDataBankDetails(id) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/EditBankDetails/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            $("#dvContractorBankDetails").show();
            $("#dvContractorBankDetails").html(data);
            $.unblockUI();
        },
        error: function (xht, ajaxOptions, throwError) {
            alert(xht.responseText);
            $.unblockUI();
        }
    });
}

function deleteDataBankDetails(urlParam) {
    if (confirm("Are you sure you want to delete Bank details?")) {
        $.ajax({
            url: "/Master/DeleteBankDetails/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $('#tblBankDetailsList').trigger('reloadGrid');
                }
                else {
                    $('#tblBankDetailsList').trigger('reloadGrid');
                    alert(data.message);
                }

            },
            error: function (xht, ajaxOptions, throwError)
            { alert(xht.responseText); }
        });
    }
    else {
        return false;
    }


}

function finalizeBankDetails(urlParam) {

    if (confirm("Are you sure you want to finalize Bank details?")) {
        $.ajax({
            url: "/Master/FinalizeBankDetails/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $('#tblBankDetailsList').trigger('reloadGrid');
                }
                else {
                    $('#tblBankDetailsList').trigger('reloadGrid');
                    alert(data.message);
                }

            },
            error: function (xht, ajaxOptions, throwError)
            { alert(xht.responseText); }
        });
    }
    else {
        return false;
    }
}


function activateStatus(urlParam) {

    if (confirm("Are you sure you want to Activate Bank Account Status ?")) {
        $.ajax({
            url: "/Master/ActivateBankAccountStatus/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $('#tblBankDetailsList').trigger('reloadGrid');
                }
                else {
                    $('#tblBankDetailsList').trigger('reloadGrid');
                    alert(data.message);
                }

            },
            error: function (xht, ajaxOptions, throwError)
            { alert(xht.responseText); }
        });
    }
    else {
        return false;
    }
}



function LoadGrid() {
    $('#tblBankDetailsList').jqGrid({
        url: '/Master/GetContractorBankDetails/',
        datatype: 'json',
        mtype: "POST",
        postData: { ContractorCode: $('#EncryptedContractorId').val() },
        colNames: ['Contractor Name', 'District Name', 'State Name', 'Account Number', 'Bank Name', 'IFSC Code', 'Status','Activate Account Status', 'Action',/*'Edit', 'Delete',*/ 'Finalize', 'PFMS Status'],
        colModel: [
        { name: 'ContName', index: 'ContName', height: 'auto', width: 140, align: "left", sortable: true },
        { name: 'District', index: 'District', height: 'auto', width: 100, align: "left", sortable: true },
        { name: 'State', index: 'State', height: 'auto', width: 100, align: "left", sortable: true },
        { name: 'AccNumber', index: 'AccNumber', height: 'auto', width: 80, align: "left", sortable: true },
        { name: 'BankName', index: 'BankName', height: 'auto', width: 160, align: "left", sortable: true },
        { name: 'IfscCode', index: 'IfscCode', height: 'auto', width: 80, align: "left", sortable: true },
        { name: 'AccStatus', index: 'AccStatus', height: 'auto', width: 80, align: "center", sortable: true },
        { name: 'b', width: 60, sortable: false, resize: false, /*formatter: FormatColumnFinalize,*/ align: "center", sortable: false },

        { name: 'a', width: 60, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false },
        //{ name: 'Delete', width: 60, sortable: false, resize: false, formatter: FormatColumnDelete, align: "center", sortable: false },
        { name: 'a', width: 60, sortable: false, resize: false, /*formatter: FormatColumnFinalize,*/ align: "center", sortable: false }, //added by PP (01-05-2018),
        { name: 'PFMSStatus', index: 'PFMSStatus', height: 'auto', width: 90, align: "center", sortable: true }
        ],
        pager: jQuery('#dvPagerBankDetails'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'BankName',
        sortorder: "asc",
        caption: 'Bank List',
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
                alert("Invalid Data. Please Check and Try Again.");
            }
        }

    });

}