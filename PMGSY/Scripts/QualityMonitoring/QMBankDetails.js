$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

$(document).ready(function () {
    LoadGrid();
    $.unblockUI();

    $('#spCollapseIconCNBDQM').click(function () {
        //$('#dvBankListQM').
        $("#spCollapseIconCNBDQM").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvCreateNewBankDetailQM").toggle("slow");
    });

    
});
function FormatColumn(cellvalue, options, rowObject) {
    if (cellvalue.toString() == "") {
        return "<center><table><tr><td style='border:none;'><span>-</span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none;'></td><td style='border:none;'><span class='ui-icon ui-icon-trash' title='Delete Bank Details' onClick ='deleteDataBankDetailsQM(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }

}
function editDataBankDetailsQM(id) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/QualityMonitoring/QMEditBankDetails/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            $("#dvBankDetailsQM").show();
            $("#dvBankDetailsQM").html(data);
            $.unblockUI();
        },
        error: function (xht, ajaxOptions, throwError) {
            alert(xht.responseText);
            $.unblockUI();
        }
    });
}

function deleteDataBankDetailsQM(urlParam) {
    if (confirm("Are you sure you want to delete Bank details?")) {
        $.ajax({
            url: "/QualityMonitoring/DeleteBankDetailsQM/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $('#tblBankDetailsListQM').trigger('reloadGrid');
                }
                else {
                    $('#tblBankDetailsListQM').trigger('reloadGrid');
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
    $('#tblBankDetailsListQM').jqGrid('GridUnload');
    $('#tblBankDetailsListQM').jqGrid({
        url: '/QualityMonitoring/GetContractorBankDetailsQM/',
        datatype: 'json',
        mtype: "POST",
        postData: { adminQMCode: $('#ADMIN_QM_CODE').val() },
        colNames: ['Monitor', 'District Name', 'State Name', 'Account Number', 'Bank Name', 'IFSC Code', 'Status'/*, 'Action'*/],
        colModel: [
        { name: 'Monitor', index: 'Monitor', height: 'auto', width: 140, align: "left", sortable: true },
        { name: 'District', index: 'District', height: 'auto', width: 100, align: "left", sortable: true },
        { name: 'State', index: 'State', height: 'auto', width: 100, align: "left", sortable: true },
        { name: 'AccNumber', index: 'AccNumber', height: 'auto', width: 120, align: "left", sortable: true },
        { name: 'BankName', index: 'BankName', height: 'auto', width: 160, align: "left", sortable: true },
        { name: 'IfscCode', index: 'IfscCode', height: 'auto', width: 120, align: "left", sortable: true },
        { name: 'AccStatus', index: 'AccStatus', height: 'auto', width: 80, align: "center", sortable: true },
        //{ name: 'a', width: 60, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#dvPagerBankDetailsQM'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'BankName',
        sortorder: "asc",
        caption: 'Bank List',
        height: 'auto',
        //autowidth: true,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function () {
            var recordCount = jQuery('#tblBankDetailsListQM').jqGrid('getGridParam', 'reccount');
            //alert(recordCount);
            if (recordCount == 0) {
                var button = '<input type="button" id="btnAddNewDetailsQM" name="btnAddNewDetailsQM" value="Add Bank Details" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" title="Add Bank Detail" tabindex="200" style="font-size:1em; margin-left:25px" onclick="AddBankDetails()" />'
                $('#dvPagerBankDetailsQM_left').html(button);
                //$('#btnAddNewDetailsQM').show('slow');
            }
            else {
                $('#dvPagerBankDetailsQM_left').html("");
            }
        },
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

function AddBankDetails() {

    $("#dvBankDetailsQM").load('/QualityMonitoring/AddBankDetailsQM/' + $("#ADMIN_QM_CODE").val());
    $("#dvBankDetailsQM").show('slow');
};