$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

$(document).ready(function () {
    LoadGrid();
    $.unblockUI();

    $('#btnAddNewDetailsNO').click(function (e) {
 
        $("#dvContractorBankDetailsNO").load('/Master/AddBankDetailsNO/' + $("#NodalOfficerCode").val());
        $("#dvContractorBankDetailsNO").show('slow');
    });

    $("#btnCancelBankDetailsNO").click(function (e) {

        if ($("#dvlstBankDetailsNO").is(":visible")) {
            $("#dvlstBankDetailsNO").hide('slow');
        }
        if ($('#tblBankDetailsListNO').is(":visible")) {
            $('#tblBankDetailsListNO').hide();
        }
        //$('#tblstContractorRegNO').jqGrid("setGridState", "visible");
        $('#tblList').jqGrid("setGridState", "visible");
        
        $("#btnCreateNew").show();
        $("#btnSearchView").hide();
        $("#dvSearchNodalOfficer").show();
        //$("#dvSearchContractorRegNO").show("slow");
        $("#dvDetailsNodalOfficer").show("slow");

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
        return "<center><table><tr><td  style='border:none;'><span class='ui-icon ui-icon-pencil' title='Edit Bank Details' onClick ='editDataBankDetailsNO(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;'><span class='ui-icon ui-icon-trash' title='Delete Bank Details' onClick ='deleteDataBankDetailsNO(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }

}
function editDataBankDetailsNO(id) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/EditBankDetailsNO/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
           
            $("#dvContractorBankDetailsNO").show();
            $("#dvContractorBankDetailsNO").html(data);
            $.unblockUI();
        },
        error: function (xht, ajaxOptions, throwError) {
            alert(xht.responseText);
            $.unblockUI();
        }
    });
}

function deleteDataBankDetailsNO(urlParam) {
    if (confirm("Are you sure you want to delete Bank details?")) {
        $.ajax({
            url: "/Master/DeleteBankDetailsNO/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $('#tblBankDetailsListNO').trigger('reloadGrid');
                }
                else {
                    $('#tblBankDetailsListNO').trigger('reloadGrid');
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
$('#tblBankDetailsListNO').jqGrid({
        url: '/Master/GetContractorBankDetailsNO/',
        datatype: 'json',
        mtype: "POST",
        postData: { NodalOfcCode: $('#encrNodalOfficerCode').val() },
        colNames: ['Nodal Officer Name', 'District Name', 'State Name', 'Account Number', 'Bank Name', 'IFSC Code', 'Status', 'Action'],
        colModel: [
        { name: 'ContName', index: 'ContName', height: 'auto', width: 140, align: "left", sortable: true },
        { name: 'District', index: 'District', height: 'auto', width: 100, align: "left", sortable: true },
        { name: 'State', index: 'State', height: 'auto', width: 100, align: "left", sortable: true },
        { name: 'AccNumber', index: 'AccNumber', height: 'auto', width: 120, align: "left", sortable: true },
        { name: 'BankName', index: 'BankName', height: 'auto', width: 160, align: "left", sortable: true },
        { name: 'IfscCode', index: 'IfscCode', height: 'auto', width: 120, align: "left", sortable: true },
        { name: 'AccStatus', index: 'AccStatus', height: 'auto', width: 80, align: "center", sortable: true },
        { name: 'a', width:60, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
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