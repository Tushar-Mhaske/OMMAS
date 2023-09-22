var isGridLoaded = false;
var isValid = undefined;
$(document).ready(function () {

    LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), 0, 'view');
    isGridLoaded = true;

    $("#btnView").click(function () {
        LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), 0, 'view');
    });

});


function LoadGrid(month, year, transType, mode) {
    if (isGridLoaded) {
        $("#tblReceiptList").GridUnload();
        isGridLoaded = false;
    }
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#tblReceiptList").jqGrid({
        url: '/Reat/Reat/GetFundReceiptList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Bill Number', 'Bill Date',  'Cheque No', 'Cheque Date', 'Transaction', 'Cheque Amount', 'Cash Amount', 'Gross Amount', 'Send to PFMS'],
        colModel: [
                            { name: 'BillNumber', index: 'BillNumber', width: 80, align: 'center', sortable: false },
                            { name: 'BillDate', index: 'BillDate', width: 80, align: 'center', sortable: true },                          
                            { name: 'ChequeNo', index: 'ChequeNo', width: 80, align: 'center', sortable: false },
                            { name: 'ChequeDate', index: 'ChequeDate', width: 80, align: 'center', sortable: false },
                            { name: 'Transaction', index: 'Transaction', width: 150, align: 'left', sortable: false },
                            { name: 'ChequeAmount', index: 'ChequeAmount', width: 80, align: 'center', sortable: false/*, hidden: true*/ },
                            { name: 'CashAmount', index: 'CashAmount', width: 80, align: 'center', sortable: false/*, hidden: true */ },
                            { name: 'GrossAmount', index: 'GrossAmount', width: 80, align: 'right', sortable: false },
                            { name: 'SendToPFMS', index: 'SendToPFMS', width: 80, align: 'center', sortable: false }
        ],
        pager: jQuery('#divReceiptListPager'),
        rowNum: 10,
        postData: {
            'month': month,
            'year': year,
            'transType': transType,
            'mode': mode
        },
        altRows: false,
        rowList: [10, 20, 30 ,40 ,50],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'BillDate',
        sortorder: "desc",
        caption: " Fund Receipt Details",
        height: 'auto',
        //width: '1080px',
        autowidth: true,
        rownumbers: true,
        //hidegrid: false,
        loadComplete: function (data) {
            isGridLoaded = true;
            if (data.isReceiptEnable == true) {
                if ($('#tblReceiptList').jqGrid('getGridParam', 'reccount') > 0) {
                    $("#divReceiptListPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span>Use Cursor over on <font color='#4eb305' style='font-weight:bold'>plus icon</font>  to Generate Fund Receipt");
                }
                $('#tblReceiptList_rn').html('Sr.<br/>No.');
                $('#divReceiptDisable').hide();
            }
            else {             
                $('#tblReceiptList').GridUnload();
                $('#divReceiptDisable').html("<span class='ui-jqgrid-title' style='float: left'></span>" + data.message).show();
            }
            $.unblockUI();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
            $.unblockUI();
        }

    }); //end of documents grid

}



function GenerateFundXMLFile(id) {
    var token = $('input[name=__RequestVerificationToken]').val();
    //debugger;
    if (confirm("Are you sure you want to generate receipt?")) {
        $.ajax({
            url: '/Reat/GenerateXMLForFundReceipt/' + id,
            type: "POST",
            cache: false,
            async: false,
            data: { "__RequestVerificationToken": token },
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), 0, 'view');
               }
                else {
                    alert(data.ErrorMessage)
                    LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), 0, 'view');
                }
                $.unblockUI();
            },
            error: function () {
                $.unblockUI();
                alert("Error : " + error);
                return false;
            }
        });

    }
}
