var amountVal = 0;
var isMasterGridLoaded = false;
var isDetailsGridLoaded = false;
var masterBillID = null;

$(document).ready(function () {
    //Added By Abhishek kamble 3-jan-2014 start change
    GetClosedMonthAndYear();
    //Added By Abhishek kamble 3-jan-2014 end change

    //alert('isMasterGridLoaded  ' + isMasterGridLoaded);
    //alert('isDetailsGridLoaded  ' + isDetailsGridLoaded);



    if (billId == 0) {
        //$("#loadReceiptMaster").load("/Receipt/ReceiptMaster/");
        $.ajax({
            url: "/Receipt/ReceiptMaster/",
            type: "GET",
            async: false,
            cache: false,
            data:
                {
                    "Month": month,
                    "Year": year
                },
            success: function (data) {
                $("#loadReceiptMaster").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }
        });
    }
    else if (isFinalize == 'N') {
        LoadMasterReceiptGrid(billId);

     //   alert(isMulTxn);
 
        if (isMulTxn == "N") {
            LoadDetailsReceiptGrid(billId);            
        }
        else {
            $("#loadReceiptDetails").load("/Receipt/ReceiptDetails/" + billId);
            LoadDetailsReceiptGrid(billId);
        }
    }
    else if (isFinalize == 'Y') {
        LoadMasterReceiptGrid(billId);
        LoadDetailsReceiptGrid(billId);
        $("#divFinalizeReceipt").hide('slow');
        $("#btnFinalizeReceipt").hide('slow');
        
    }

    $("#lblBackToList").click(function () {
        //$("#mainDiv").load("/Receipt/ListReceipt/");
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Receipt/ListReceipt/",
            type: "GET",
            async: false,
            cache: false,
            data:
                {
                    "Month": month,
                    "Year": year
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
        
    });

    $("#btnFinalizeReceipt").click(function () {


        if (confirm("Are you sure to Finalize Receipt?")) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/Receipt/FinalizeReceipt/" + $("#tblMasterReceiptList").getDataIDs()[0],
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {
                    $.unblockUI();

                    if (data.success) {
                        alert("Receipt Finalized");
                        //$("#mainDiv").load("/Receipt/AddEditReceipt/" + data.message);
                        $.ajax({
                            url: "/Receipt/AddEditReceipt/" + data.message,
                            type: "GET",
                            async: false,
                            cache: false,
                            data:
                                {
                                    "Month": month,
                                    "Year": year
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
                    }
                    else {
                        alert(data.message);
                        return false;
                    }
                    $.unblockUI();
                }
            });
        }
        else {
            $.unblockUI();
            return false;
        }

    });

    
}); // Document.ready ends here

function EditMasterReceipt(urlParam) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#loadReceiptMaster").load("/Receipt/ReceiptMaster/" + urlParam, function () {
        //$("#BILL_NO").attr("readonly", "readonly");

        //added by Koustubh Nakate on 10/10/2013 to make receipt mode readonly 
        $('#rdoCash').attr('disabled', true);
        $('#rdoCheque').attr('disabled', true);
        $.unblockUI();
    });
}

function EditReceiptDetails(urlParam) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    //alert($("#tblDetailsReceiptList").getGridParam('selrow'));
    $("#loadReceiptDetails").load("/Receipt/ReceiptDetails/" + urlParam, function () { //changes by koustubh nakate on 18/07/2013 

        $("#ddlTransDetails").trigger('change');

        ////get the imprest amount remaining for amount validation
        //$.ajax({
        //    url: "/Receipt/GetImprestAmount/" + $("#ddlUnSettledVouchers").val(),
        //    type: "POST",
        //    async: false,
        //    cache: false,
        //    success: function (data) {
               
        //        imprestAmount = data
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }
        //});

        $.unblockUI();
    });
}


function DeleteReceiptDetails(urlParam) {

    if (confirm("Are you sure to delete Receipt Details?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        var gridParams = $("#tblDetailsReceiptList").getGridParam('postData');

        var masterBillId = gridParams.masterId;

        $.ajax({
            url: "/Receipt/DeleteReceiptDetails/" + urlParam,
            type: "POST",
            async: false,
            cache: false,
            success: function (data) {
                $.unblockUI();
                if (data.success==true) {
                                     
                    alert("Receipt Details Deleted");
                                    
                    $("#loadReceiptDetails").load("/Receipt/ReceiptDetails/" + masterBillId);//billId

                   
                    LoadDetailsReceiptGrid(masterBillId);//billId
               
                   // return false;
                }
                else {
                    alert(data.message);
                    return false;
                }
                $.unblockUI();

            }
        });
    }
    else {
       
        return false;
    }
   
}

function LoadMasterReceiptGrid(MasterId) {

    if (isMasterGridLoaded) {
        $("#tblMasterReceiptList").GridUnload();
        isMasterGridLoaded = false;
    }

    jQuery("#tblMasterReceiptList").jqGrid({
        url: '/Receipt/ReceiptMasterList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Receipt Number', 'Receipt Date', 'Transaction Name', 'Cheque No', 'Cheque Date', 'Gross Amount', 'Edit'],
        colModel: [
                            { name: 'ReceiptNumber', index: 'ReceiptNumber', width: 100, align: 'center', sortable: true },
                            { name: 'ReceiptDate', index: 'ReceiptDate', width: 100, align: 'center', sortable: true },
                            { name: 'TransactionName', index: 'TransactionName', width: 200, align: 'center', sortable: true },
                            { name: 'ChequeNo', index: 'ChequeNo', width: 100, align: 'center', sortable: true },
                            { name: 'ChequeDate', index: 'ChequeDate', width: 100, align: 'center', sortable: true },
                            { name: 'GrossAmount', index: 'GrossAmount', width: 100, align: 'right', sortable: true },
                            { name: 'Edit', index: 'Edit', width: 50, align: 'center', sortable: false }
        ],
        //pager: jQuery('#divMasterReceiptListPager'),
        rowNum: 10,
        postData: {
            'masterId': MasterId
        },
        altRows: true,
        rowList: [10, 20, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        emptyrecords: 'No records to view',
        sortname: 'ReceiptDate',
        sortorder: "desc",
        caption: "Receipt Master",
        height: 'auto',
        autowidth: true,//'750px',
        rownumbers: true,
        hidegrid: false,
        loadComplete: function () {

            isMasterGridLoaded = true;
            var masterRow = $("#tblMasterReceiptList").getRowData($("#tblMasterReceiptList").getDataIDs()[0]);
            var transName = masterRow['TransactionName'];
            $('#tblMasterReceiptList_rn').html('Sr.<br/>No.');

            //Commented By Abhishek kamble 29-jan-2014
            //if (isMulTxn == "N" && isFinalize == "N")
            //{
            //    //setTimeout(function () {  }, 2000);
            //    //alert();
            //    $("#mulTxnMsg span:eq(1)").html('Multiple Transaction Entry Prohibited for Master Transaction <strong>\'' + transName + '\'</strong>');
            //    $("#mulTxnMsg").show('slide');
            //}

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
           // $.unblockUI();
        }
       
    }); //end of documents grid


}

function LoadDetailsReceiptGrid(MasterId) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    if (isDetailsGridLoaded) {
        $("#tblDetailsReceiptList").GridUnload();
        isDetailsGridLoaded = false;
    }
    jQuery("#tblDetailsReceiptList").jqGrid({
        url: '/Receipt/ReceiptDetailsList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Transaction Number', 'Transaction Name', 'Head Name', 'Head Code', 'Contractor Name', 'Agreement', 'DPIU', 'Amount', 'Narration', 'Edit', 'Delete','Status'],
        colModel: [
                            { name: 'TransactionNumber', index: 'TransactionNumber', width: 0, align: 'center', sortable: false, hidden: true },
                            { name: 'TransactionName', index: 'TransactionName', width: 150, align: 'left', sortable: false, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } },
                            { name: 'HeadName', index: 'HeadName', width: 200, align: 'left', sortable: false, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } },
                            { name: 'AccHeadcode', index: 'AccHeadcode', width: 0, align: 'left', sortable: false, hidden: true },
                            { name: 'Contractor', index: 'Contractor', width: 125, align: 'left', sortable: false,hidden:fundType == 'A'?true:false },//, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } 
                            { name: 'Agreement', index: 'Agreement', width: 125, align: 'left', sortable: false, hidden: fundType == 'A' ? true : false },
                            { name: 'DPIU', index: 'DPIU', width: 100, align: 'left', sortable: false, },
                            { name: 'Amount', index: 'Amount', width: 70, align: 'right', sortable: false, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } },
                            { name: 'Narration', index: 'Narration', width: 120, align: 'left', sortable: false, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } },
                            { name: 'Edit', index: 'Edit', width: 30, align: 'center', sortable: false },
                            { name: 'Delete', index: 'Delete', width: 35, align: 'center', sortable: false },
                            { name: 'Status', index: 'Status', width: 60, align: 'center', sortable: false }
        ],
        pager: jQuery('#divDetailsReceiptListPager'),
        //rowNum: 10,
        rowNum: 99999,
        postData: {
            'masterId': MasterId
        },
        //altRows: true,
        //rowList: [10, 20, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        emptyrecords: 'No records to view',
        sortname: 'TransactionNumber',
        sortorder: "desc",
        caption: "Receipt Details",
        footerrow: true,
        userDataOnFooter: true,
        height:'auto', //'250px', //'auto',
        autowidth: true,//'770px',
       // width:'1150',
        rownumbers: true,
        pgbuttons: false,/*added by Koustubh Nakate on 07/10/2013 to remove paging */
        pginput:false,
        hidegrid: false,
        loadComplete: function () { 
            isDetailsGridLoaded = true;
            $("#tblDetailsReceiptList").find('a').click(function () {
                var selRowId = $(this).parents('tr').attr('id');
                var data = $("#tblDetailsReceiptList").getRowData(selRowId);
                amountVal = data['Amount'];                
            });

            // This code is to show Finalize button
            var footer = $(this).footerData();
            //detailsTotal = footer["Amount"];
            var userdata = $("#tblDetailsReceiptList").getGridParam('userData');
            var detailsTotal = userdata.TotalAmount;

            //Added By Abhishek kamble 29-jan-2014 to get gross Amt.
            var masterTotal = userdata.ReceiptGrossAmount;          

            //Commented by abhishek kamble for error in getting master total from master Grid 
            var masterRow = $("#tblMasterReceiptList").getRowData($("#tblMasterReceiptList").getDataIDs()[0]);
            //masterTotal = masterRow['GrossAmount'];

            if (parseFloat(masterTotal) == parseFloat(detailsTotal) && isFinalize != 'Y') {

                $("#divFinalizeReceipt").show('slow');
                $("#btnFinalizeReceipt").show('slow');
                if (userdata.isMulTxn == "N" && $("#mulTxnMsg").css('display') == "none") {
                    var transName = masterRow['TransactionName'];
                    $("#loadReceiptDetails").html("");

                    //Modified By Abhishek kamble 3-jan-2014
                    if (parseFloat(masterTotal) == parseFloat(detailsTotal) && isFinalize != 'Y') {
                        $("#mulTxnMsg span:eq(1)").html('');
                        $("#mulTxnMsg").hide('slide');
                    } else {
                        $("#mulTxnMsg span:eq(1)").html('Multiple Transaction Entry Prohibited for Master Transaction <strong>\'' + transName + '\'</strong>');
                        $("#mulTxnMsg").show('slide');
                    }
                }
                else {
                    $("#mulTxnMsg span:eq(1)").html("");
                    $("#mulTxnMsg").hide('slide');
                }
                //Added by abhishek kamble 11-dec-2013
                $("#loadReceiptDetails").html("");
            }
            else {
                var transName = masterRow['TransactionName'];
                if (userdata.isMulTxn == "N" && $("#mulTxnMsg").css('display') == "none") {

                    $("#loadReceiptDetails").html("");

                    //Modified By Abhishek kamble 29-jan-2014
                    if (isFinalize == "Y") {
                        $("#mulTxnMsg span:eq(1)").html('');
                        $("#mulTxnMsg").hide('');
                    } else {
                        $("#mulTxnMsg span:eq(1)").html('Multiple Transaction Entry Prohibited for Master Transaction <strong>\'' + transName + '\'</strong>');
                        $("#mulTxnMsg").show('slide');
                    }
                }
                $("#divFinalizeReceipt").hide('slow');
                $("#btnFinalizeReceipt").hide('slow');    
            }

            var recCount = $("#tblDetailsReceiptList").getGridParam('reccount');

            if (recCount > 5) {

                $("#tblDetailsReceiptList").setGridHeight('250');
            }
            else {
                $('#tblDetailsReceiptList').setGridHeight('auto');
            }
            //added by abhishek kamble 11-nov-2013
            $('#tblDetailsReceiptList_rn').html('Sr.<br/>No.');

            $.unblockUI();

        },
        gridComplete: function () {
            $('#tblDetailsReceiptList').jqGrid('setColProp', 'HeadName', { width: 'auto' });
            $('#tblDetailsReceiptList').jqGrid('setColProp', 'TransactionName', { width: 250 });
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

