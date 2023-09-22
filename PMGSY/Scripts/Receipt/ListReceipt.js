var isGridLoaded = false;
var isValid = undefined;
$(document).ready(function () {
    //Added By Abhishek kamble 3-jan-2014 start change    
    GetClosedMonthAndYear();
    //Added By Abhishek kamble 3-jan-2014 end change
    LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), null,null,0,'view');
    isGridLoaded = true;
    
    $("#btnView").click(function () {
       LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), null, null, 0, 'view');       
    });

    
    $("#txtFromDate").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonText:"From Date",
        buttonImageOnly: true
    });

    $("#txtToDate").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonText: "To Date",
        buttonImageOnly: true
    });

    $("#searchReceipt").click(function () {
        $("#tblViewDetails").hide('slow');
        $("#tblSearch").show('slow');        
    });

    $("#btnSearchCancel").click(function () {
        $("#tblSearch").hide('slow');
        $("#tblViewDetails").show('slow');
    });
    
    $("#iconClose").click(function () {
        $("#btnSearchCancel").trigger('click');
    });

    $("#AddReceipt").click(function () {
        //$("#mainDiv").load("/Receipt/AddEditReceipt/" + $("#ddlMonth").val()+"/"+ $("#ddlYear").val());
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Receipt/AddEditReceipt/",
            type: "GET",
            async: false,
            cache: false,
            data:
                {
                    "Month" : $("#ddlMonth").val(),
                    "Year"  : $("#ddlYear").val()
                },
            success: function (data) {
                $("#mainDiv").html(data);
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
        return false;
    });

    $("#btnSearch").click(function () {
        LoadGrid(0, 0, $("#txtFromDate").val(), $("#txtToDate").val(), $("#ddlMasterTrans").val(), 'search');
    });

    //new change done by Vikram for adding the changed month and year in session
    $("#ddlMonth").change(function () {
        UpdateAccountSession($("#ddlMonth").val(),$("#ddlYear").val());
    });

    $("#ddlYear").change(function () {
        UpdateAccountSession($("#ddlMonth").val(), $("#ddlYear").val());
    });
});

function EditReceipt(urlParam)
{
 
    //new change done by Vikram on 24 Jan 2014 for validating the transaction 
    ValidateCashChequeReciept(urlParam);
    if (isValid == true) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/Receipt/AddEditReceipt/" + urlParam,
            type: "GET",
            async: false,
            cache: false,
            data:
                {
                    "Month": $("#ddlMonth").val(),
                    "Year": $("#ddlYear").val()
                },
            success: function (data) {
                $("#mainDiv").html(data);
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    }
    else {
      //isValid = false;
        return false;
    }
}

function DeleteReceipt(urlParam) {

    var rowid = parseInt(urlParam.split(',')[1]) - 1;
    
    urlParam = urlParam.split(',')[0];
    
    var varMsg = null;
    if ($("#tblReceiptList").find("tr:eq(" + rowid + ")").find('td:eq(10)').find('div:eq(1)').attr('title') == "Rs 0 entered") {
        varMsg = "Are you sure to Delete Receipt?";        
    }
    else {
        varMsg = "Details present. Are you sure to delete All Details?";
    }
   
    if (confirm(varMsg)) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Receipt/DeleteReceipt/" + urlParam,
            type: "POST",
            async: false,
            cache: false,
            success: function (data) {
                $.unblockUI();

                if (data.success) {
                    alert("Receipt Deleted");
                    if ($("#tblViewDetails").is(":visible")) {
                        LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), null, null, 0, 'view');
                    }
                    else {
                        LoadGrid(0, 0, $("#txtFromDate").val(), $("#txtToDate").val(), $("#ddlMasterTrans").val(), 'search');
                    }
                    return false;
                }
                else {
                    alert(data.message);
                    return false;
                }
            }
        });
    }
    else {
        return false;
    }
}

function LockReceipt(urlParam) {

    if (confirm("Are you sure to Finalize Receipt?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Receipt/FinalizeReceipt/" + urlParam,
            type: "POST",
            async: false,
            cache: false,
            success: function (data) {
                $.unblockUI();

                if (data.success) {
                    alert("Receipt Finalized");
                    if ($("#tblViewDetails").is(":visible")) {
                        LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), null, null, 0, 'view');
                    }
                    else {
                        LoadGrid(0, 0, $("#txtFromDate").val(), $("#txtToDate").val(), $("#ddlMasterTrans").val(), 'search');
                    }
                    return false;
                }
                else {
                    alert(data.message);
                    return false;
                }
            }
        });
    }
    else {
        return false;
    }
}

function LoadGrid(month, year, fromDate, toDate, transType, mode) {
    if (isGridLoaded)
    {
        $("#tblReceiptList").GridUnload();
        isGridLoaded = false;
    }
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#tblReceiptList").jqGrid({
        url: '/Receipt/GetReceiptList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Receipt Number', 'Receipt Date', 'Cash/Cheque', 'Transaction Name', 'Cheque No', 'Cheque Date', 'Cheque Amount', 'Cash Amount', 'Gross Amount', 'Status', 'Edit', 'Delete', 'Action'],
        colModel: [
                            { name: 'ReceiptNumber', index: 'ReceiptNumber', width: 80, align: 'center', sortable: true },
                            { name: 'ReceiptDate', index: 'ReceiptDate', width: 80, align: 'center', sortable: true },
                            { name: 'Cash/Cheque', index: 'Cash/Cheque', width: 70, align: 'center', sortable: true },
                            { name: 'TransactionName', index: 'TransactionName', width: 200, align: 'left', sortable: true },
                            { name: 'ChequeNo', index: 'ChequeNo', width: 80, align: 'center', sortable: true },
                            { name: 'ChequeDate', index: 'ChequeDate', width: 80, align: 'center', sortable: true },
                            { name: 'ChequeAmount', index: 'ChequeAmount', width: 0, align: 'center', sortable: true, hidden:true },
                            { name: 'CashAmount', index: 'CashAmount', width: 0, align: 'center', sortable: true, hidden: true },
                            { name: 'GrossAmount', index: 'GrossAmount', width: 80, align: 'right', sortable: true },
                            { name: 'Finalize', index: 'Finalize', width: 70, align: 'center', sortable: false},
                            { name: 'Edit', index: 'Edit', width: 50, align: 'center', sortable: false },
                            { name: 'Delete', index: 'Delete', width: 50, align: 'center', sortable: false },
                            { name: 'Action', index: 'Action', width: 50, align: 'center'/*, hidden: true*/ }
        ],
        pager: jQuery('#divReceiptListPager'),
        rowNum: 10,
        postData: {
            'month': month,
            'year': year,
            'fromdate': fromDate,
            'toDate': toDate,
            'transType': transType,
            'mode' : mode
        },
        altRows: true,
        rowList: [10, 20, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'ReceiptDate',
        sortorder: "desc",
        caption: "Receipt Details",
        height: 'auto',
        //width: '1080px',
        autowidth: true,
        rownumbers: true,
        //hidegrid: false,
        loadComplete: function () {
            isGridLoaded = true;
            if ($('#tblReceiptList').jqGrid('getGridParam', 'reccount') > 0) {
                $("#divReceiptListPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span>Use Mouse over on <font color='#4eb305' style='font-weight:bold'>Action</font> column to check Data Entry Status");
            }
            $('#tblReceiptList_rn').html('Sr.<br/>No.');
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

function ViewReceipt(urlParam)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Receipt/AddEditReceipt/" + urlParam,
        type: "GET",
        async: false,
        cache: false,
        data: 
            {
                "Month": $("#ddlMonth").val(),
                "Year": $("#ddlYear").val()
            },
        success: function (data) {
            $("#mainDiv").html(data);
            $.unblockUI();
            return false;
        }
    });
    // $("#mainDiv").load("/Receipt/AddEditReceipt/" + urlParam);
    return false;
}
function UpdateAccountSession(month,year)
{
    $.ajax({
        url: "/Receipt/UpdateAccountSession",
        type: "GET",
        async: false,
        cache: false,
        data:
            {
                "Month": month,
                "Year": year
            },
        success: function (data) {
            return false;
        },
        error: function (){}
    });
    return false;
}

//Added By Abhishek kamble 3-jan-2014
//function to get the account  Close month and year
function GetClosedMonthAndYear() {
    blockPage();
    $.ajax({
        type: "POST",
        url: "/MonthlyClosing/GetClosedMonthandYear/",
        // async: false,
        error: function (xhr, status, error) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        success: function (data) {
            unblockPage();
            $('#divError').hide('slow');
            $('#errorSpan').html("");
            $('#errorSpan').hide();

            if (data.monthClosed) {
                $("#lblMonth").text(data.month);
                $("#lblYear").text(data.year);
                //$("#TrAccountStatus").hide();
                $("#TrMonthlyClosing").show('Slow');
                $("#AccountNotClosedTr").hide('Slow');
                return false;
            }
            else if (data.monthClosed == false) {
                $("#AccountNotClosedTr").show('Slow');
                $("#TrMonthlyClosing").hide('Slow');
                return false;
            }
            else {
                alert("Error While getting Monthly Closing Details");
                return false;
            }
        }
    });
}

//function for validation of transaction that mismatch with the screen design parameters
function ValidateCashChequeReciept(urlParam)
{
    blockPage();
    $.ajax({
        type: "POST",
        url: "/Receipt/IsValidTransaction/" + urlParam,
        cache: false,
        async: false,
        error: function (xhr, status, error) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            isValid = false;
            return false;
        },
        success: function (data) {
            unblockPage();
            if (data.success == true) {
                isValid = true;
            }
            else if (data.success == false) {
                alert(data.message);
                isValid = false;
            }
        },
    });
}


