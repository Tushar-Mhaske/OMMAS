//this javascript file is used for imprest settlement


var isImprestMasterLoaded = false;
var isDetailsGridLoaded = false;
var PBillID = 0;
var PTxnNo = 0;
var amountValC = 0;
var amountValD = 0;


$(document).ready(function () {

    //Added By Abhishek kamble 20-jan-2014 start    
    var currentDate = $("#CURRENT_DATE").val().split("/");
    var currentDay = currentDate[0];
    var ModifiedCurrentDate = ModifiedDate(currentDate[0], $("#BILL_MONTH").val(), $("#BILL_YEAR").val());
    //Added By Abhishek kamble 20-jan-2014 end

    //Added By Abhishek kamble 7-jan-2014
    //function to get the account  Close month and year
    GetClosedMonthAndYear();

    month = $("#BILL_MONTH").val();
    year = $("#BILL_YEAR").val();



    if ($("#BILL_DATE").val() == "") {
        $("#BILL_DATE").datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            showOn: 'button',
            buttonImage: '/Content/images/calendar_2.png',
            buttonImageOnly: true
            // }).datepicker('setDate', new Date());
        }).datepicker('setDate', process(ModifiedCurrentDate));
    }
    else {
        $("#BILL_DATE").datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            showOn: 'button',
            buttonImage: '/Content/images/calendar_2.png',
            buttonImageOnly: true
        });
    }

    $("#BILL_MONTH").change(function () {
        if ($(this).val() != "0") {
            month = $(this).val();
        }
        //new change done by Vikram on 31-Dec-2013
        UpdateAccountSession($("#BILL_MONTH").val(), $("#BILL_YEAR").val());

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            type: "POST",
            url: "/payment/GenerateVoucherNo/J$" + $("#BILL_MONTH").val() + '$' + $("#BILL_YEAR").val(),
            async: false,
            // data: $("#authSigForm").serialize(),
            error: function (xhr, status, error) {
                alert(xhr.responseText);
                $.unblockUI();
            },
            success: function (data) {
                unblockPage();
                if (data != "") {
                    $.unblockUI();
                    $("#BILL_NO").val("");
                    $("#BILL_NO").val(data.strVoucherNumber);
                    $("#BILL_NO").attr('readonly', true);

                }
            }
        });

        //new change done by Abhishek kamble on 21-jan-2014 start
        if ($("#BILL_MONTH").val() == 0 || $("#BILL_YEAR").val() == 0) {
            $("#BILL_DATE").datepicker('setDate', process($("#CURRENT_DATE").val()));
        } else {
            if ($("#BILL_DATE").val() != '') {
                var selectedDate = $("#BILL_DATE").val().split('/');
                var day = selectedDate[0];
                ModifiedCurrentDate = ModifiedDate(day, $("#BILL_MONTH").val(), $("#BILL_YEAR").val());
                $("#BILL_DATE").datepicker('setDate', process(ModifiedCurrentDate));

            } else {
                ModifiedCurrentDate = ModifiedDate(currentDate[0], $("#BILL_MONTH").val(), $("#BILL_YEAR").val());
                $("#BILL_DATE").datepicker('setDate', process(ModifiedCurrentDate));
            }
        }
        //new change done by Abhishek kamble on 21-jan-2014 end
    });

    $("#BILL_YEAR").change(function () {
        if ($(this).val() != "0") {
            year = $(this).val();
        }
        //new change done by Vikram on 31-Dec-2013
        UpdateAccountSession($("#BILL_MONTH").val(), $("#BILL_YEAR").val());

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            type: "POST",
            url: "/payment/GenerateVoucherNo/J$" + $("#BILL_MONTH").val() + '$' + $("#BILL_YEAR").val(),
            async: false,
            // data: $("#authSigForm").serialize(),
            error: function (xhr, status, error) {
                alert(xhr.responseText);
                $.unblockUI();
            },
            success: function (data) {
                unblockPage();
                if (data != "") {
                    $.unblockUI();
                    $("#BILL_NO").val("");
                    $("#BILL_NO").val(data.strVoucherNumber);
                    $("#BILL_NO").attr('readonly', true);

                }
            }
        });

        //new change done by Abhishek kamble on 21-jan-2014 start
        if ($("#BILL_MONTH").val() == 0 || $("#BILL_YEAR").val() == 0) {
            $("#BILL_DATE").datepicker('setDate', process($("#CURRENT_DATE").val()));
        } else {
            if ($("#BILL_DATE").val() != '') {
                var selectedDate = $("#BILL_DATE").val().split('/');
                var day = selectedDate[0];
                ModifiedCurrentDate = ModifiedDate(day, $("#BILL_MONTH").val(), $("#BILL_YEAR").val());
                $("#BILL_DATE").datepicker('setDate', process(ModifiedCurrentDate));

            } else {
                ModifiedCurrentDate = ModifiedDate(currentDate[0], $("#BILL_MONTH").val(), $("#BILL_YEAR").val());
                $("#BILL_DATE").datepicker('setDate', process(ModifiedCurrentDate));
            }
        }
        //new change done by Abhishek kamble on 21-jan-2014 end
    });

    $("#btnViewImprest").click(function () {
        month = $("#BILL_MONTH").val();
        year = $("#BILL_YEAR").val();
        LoadImprestMasterGrid();
    });

    //to populate the imprest master list as per month and yea selection
    $("#btnImprestList").click(function () {
        LoadImprestMasterGrid();
    })

    LoadImprestMasterGrid();

    //if (billId == 0) {
    //    $.ajax({
    //        url: "/TEO/ImprestMaster/",
    //        type: "GET",
    //        async: false,
    //        cache: false,
    //        data:
    //            {
    //                "Month": month,
    //                "Year": year
    //            },
    //        success: function (data) {
    //            $("#loadImprestMaster").html(data);
    //        },
    //        error: function (xhr, ajaxOptions, thrownError) {
    //            alert(xhr.responseText);
    //        }
    //    });
    //}
    //else {
    //    LoadImprestMasterGrid(billId);
    //    LoadImprestDetailsGrid(billId);
    //}


    $("#btnSaveImprestMaster").click(function (evt) {

        evt.preventDefault();
        $.validator.unobtrusive.parse($('#frmImprestAddMaster'));
        var tmpBillId = PBillID;
        var tmpTxnId = PTxnNo;
        //new change done by Vikram 

        $("#TXN_NO").val(tmpTxnId);

        //end of change


        if ($('#frmImprestAddMaster').valid()) {
            
            blockPage();
            $.ajax({
                url: "/TEO/AddImprestMaster/" + PBillID+"/"+tmpTxnId,
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmImprestAddMaster").serialize(),
                success: function (data) {
                    
                    if (!data.success) {
                        if (data.message == "undefined" || data.message == null) {
                            $("#mainDiv").html(data);
                            PBillID = tmpBillId;
                            PTxnNo = tmpTxnId;
                            $("#tblTEOMaster").show();
                        }
                        else {
                            $("#divTEOImprestError").show("slide");
                            $("#divTEOImprestError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        }
                        unblockPage();
                        return false;
                    }
                    else {
                        var masterid = data.message;
                        $("#divTEOImprestError").hide("slide");
                        $("#divTEOImprestError span:eq(1)").html('');
                        $("#btnResetImprestMaster").trigger('click');
                        $("#BILL_NO").attr('readonly', true);
                        $("#BILL_DATE").attr('readonly', true);
                        unblockPage();
                        alert("Imprest Master Added");
                        blockPage();

                        //$.ajax({
                        //    url: "/TEO/TEODetails/" + masterid + "/C",
                        //    type: "POST",
                        //    async: false,
                        //    cache: false,
                        //    success: function (data) {
                        //        $("#loadImprestCreditDetails").html(data);
                        //        $.each($("select"), function () {
                        //            if ($(this).find('option').length >= 1) {
                        //                $('#tr' + $(this).attr('id')).show();
                        //            }
                        //        });
                        //        $.ajax({
                        //            url: "/TEO/TEODetails/" + masterid + "/D",
                        //            type: "POST",
                        //            async: false,
                        //            cache: false,
                        //            success: function (data) {
                        //                $("#loadImprestDebitDetails").html(data);
                        //                $.each($("select"), function () {
                        //                    if ($(this).find('option').length >= 1) {
                        //                        $('#tr' + $(this).attr('id')).show();
                        //                    }
                        //                });
                        //                unblockPage();
                        //                return false;
                        //            },
                        //            error: function (xhr, ajaxOptions, thrownError) {
                        //                alert(xhr.responseText);
                        //                unblockPage();
                        //            }
                        //        });
                        //    },
                        //    error: function (xhr, ajaxOptions, thrownError) {
                        //        alert(xhr.responseText);
                        //        unblockPage();
                        //    }
                        //});

                        //return false;
                
                        $.ajax({
                            url: "/TEO/TEOEntry/" + masterid,
                            type: "GET",
                            async: false,
                            cache: false,
                            data:
                                {
                                    "Month": $("#BILL_MONTH").val(),
                                    "Year": $("#BILL_YEAR").val(),
                                    'ImprestEntry': true
                                },
                            success: function (data) {
                                $("#mainDiv").html(data);

                                //Added By Abhishek kamble 5-Mar-2014
                                $("#PBillId").val(PBillID);

                                unblockPage();
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                alert(xhr.responseText);
                                unblockPage();
                            }
                        });
                        return false;
                        unblockPage();
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("Error while processing request");
                    $.unblockUI();

                }
            });
           
        }
    });


    $("#lblBackToList").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/TEO/TEOList/",
            type: "GET",
            async: false,
            cache: false,
            data:
                {
                    "Month": $("#BILL_MONTH").val(),
                    "Year": $("#BILL_YEAR").val()
                },
            success: function (data) {
                $.unblockUI();

                $("#mainDiv").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    });

    $.unblockUI();

});

function AddImprestMaster(urlParam,transId)
{
    PBillID = urlParam;
    PTxnNo = transId;
    $("#ENC_PBILL_ID").val(PBillID);
    //$("#TXN_NO").val(transId);
    $("#tblTEOMaster").show();

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: "POST",
        url: "/payment/GenerateVoucherNo/J$" + $("#BILL_MONTH").val() + '$' + $("#BILL_YEAR").val(),
        async: false,
        // data: $("#authSigForm").serialize(),
        error: function (xhr, status, error) {
            alert(xhr.responseText);
            $.unblockUI();
        },
        success: function (data) {
            unblockPage();
            if (data != "") {
                $.unblockUI();
                $("#BILL_NO").val("");
                $("#BILL_NO").val(data.strVoucherNumber);
                $("#BILL_NO").attr('readonly', true);

            }
        }
    });

}

function AddImprestDetails(urlParam)
{
    var pBillId = $('#PBillId').val();
    blockPage();
    $.ajax({
        url: "/TEO/TEOEntry/" + urlParam,
        type: "GET",
       // async: false,
        cache: false,
        data:
            {
                "Month": $("#BILL_MONTH").val(),
                "Year": $("#BILL_YEAR").val(),
                'ImprestEntry':true
            },
        success: function (data) {
          
            $("#mainDiv").html(data);

            $('#PBillId').val(pBillId);


            unblockPage();
        },
        error: function (xhr, ajaxOptions, thrownError) {
           
            alert(xhr.responseText);
            unblockPage();
        }
    });
   

}


function CollapseAllOtherRowsSubGrid(rowid) {
    var rowIds = $("#tblImprestMasterGrid").getDataIDs();
    $.each(rowIds, function (index, rowId) {
        $("#tblImprestMasterGrid").collapseSubGridRow(rowId);
    });
}


///function to delete the imprest settlement master details teo
function DeleteTEO(urlParam, CreditDebitAmt) {
   
    varMsg = "Are you sure to Delete TEO?";

    if (confirm(varMsg)) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/TEO/DeleteTEOMaster/" + urlParam,
            type: "POST",
            async: false,
            cache: false,
            success: function (data) {
                $.unblockUI();

                if (data.success) {
                    alert("TEO has been deleted");
                    LoadImprestMasterGrid();
                   
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

///function to loaf the imprest master list
function LoadImprestMasterGrid() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    if (isImprestMasterLoaded) {
        $("#tblImprestMasterGrid").GridUnload();
        isImprestMasterLoaded = false;
    }

    jQuery("#tblImprestMasterGrid").jqGrid({
        url: '/TEO/ImprestMasterList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Voucher Number', 'Voucher Date', 'Cheque No', 'Cheque Date', 'Payee Name', 'Gross Amount', 'Amount Adjusted <br>by TEO', 'Amount Adjusted </br>by Receipt', 'Total Amount </br> Adjusted', 'Status', 'Add Settlement Details', 'encryptedId'],
        colModel: [
                            { name: 'VoucherNumber', index: 'VoucherNumber', width: 100, align: 'center', sortable: true },
                            { name: 'VoucherDate', index: 'VoucherDate', width: 100, align: 'center', sortable: true },
                            { name: 'ChequeNo', index: 'ChequeNo', width: 100, align: 'center', sortable: true },
                            { name: 'ChequeDate', index: 'ChequeDate', width: 100, align: 'center', sortable: true },
                            { name: 'PayeeName', index: 'PayeeName', width: 100, align: 'center', sortable: false },
                            { name: 'GrossAmount', index: 'GrossAmount', width: 100, align: 'right', sortable: true },
                            { name: 'TEOAmount', index: 'TEOAmount', width: 100, align: 'right', sortable: true },
                            { name: 'ReceiptAmount', index: 'ReceiptAmount', width: 100, align: 'right', sortable: true },
                            { name: 'AmountSettled', index: 'AmountSettled', width: 100, align: 'center', sortable: false },
                            { name: 'Status', index: 'Status', width: 100, align: 'center', sortable: false },
                            { name: 'AddDetails', index: 'AddDetails', width: 50, align: 'center', sortable: false },
                            { name: 'encryptedId', index: 'encryptedId', width: 0, align: 'center', sortable: false,hidden:true }
        ],
        pager: jQuery('#divImprestMasterPager'),
        postData: {
            'month': month,
            'year': year
        },
        rowNum: 10,
       // jsonReader: {repeatitems: false},
        rowList: [10, 20, 50],
        altRows: true,
        sortname: 'VoucherDate',
        viewrecords: true,
        caption: "Imprest issued to staff",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        //onSelectRow: function (id) {
           
        //},
        subGrid : true,
        subGridRowExpanded: function (subgrid_id, row_id) {
            // we pass two parameters
            // subgrid_id is a id of the div tag created whitin a table data
            // the id of this elemenet is a combination of the "sg_" + id of the row
            // the row_id is the id of the row
            // If we wan to pass additinal parameters to the url we can use
            // a method getRowData(row_id) - which returns associative array in type name-value
            // here we can easy construct the flowing

            //alert(subgrid_id);

           // alert(row_id);
            //var b = getRowData(row_id);
            /* var a=$('#adminCategory').getRowData(row_id);
             alert(a['ADMIN_ND_NAME']);*/
            $('#tblImprestMasterGrid').jqGrid('setSelection', row_id, false);

            CollapseAllOtherRowsSubGrid(row_id);

            var subgrid_table_id, pager_id;
            subgrid_table_id = subgrid_id + "_t";
            pager_id = "p_" + subgrid_table_id;
            //alert($('#tblImprestMasterGrid').getCell(row_id, 'encryptedId'));
            $("#" + subgrid_id).html("<table id='" + subgrid_table_id + "'></table><div id='" + pager_id + "' ></div>");
            jQuery("#" + subgrid_table_id).jqGrid({
                url: '/TEO/ImprestSettlementMasterList/',
                //postData: { AgreementCode: row_id, IMSPRRoadCode: $('#EncryptedIMSPRRoadCode').val() },
                postData: { imprestCode: $('#tblImprestMasterGrid').getCell(row_id, 'encryptedId') },
                datatype: "json",
                mtype: "POST",
                colNames: ['TEO Number', 'TEO Date', /*'Transaction Name',*/ 'Gross Amount','Settled Amount', 'Status', 'Edit', 'Delete'],
                colModel: [
                             { name: 'TEONumber', index: 'TEONumber', height: 'auto', width: 100, align: "left", sortable: true },
                             { name: 'TEODate', index: 'TEODate', height: 'auto', width: 80, align: "left", sortable: true },
                            /* { name: 'TransactionName', index: 'TransactionName', height: 'auto', width: 80, align: "right", sortable: false },*/
                             { name: 'GrossAmount', index: 'GrossAmount', height: 'auto', width: 80, align: "right", sortable: false },
                             { name: 'SettledAmount', index: 'SettledAmount', height: 'auto', width: 80, align: "right", sortable: false },
                            { name: 'Status', index: 'Status', height: 'auto', width: 120, sortable: false, align: "right" },
                             { name: 'Edit', index: 'Edit', height: 'auto', width: 120, sortable: false, align: "right" },
                             { name: 'Delete', index: 'Delete', height: 'auto', width: 100, sortable: false, align: "left" }
                             

                ],
                rowNum:15000,
                pginput:false,
                pager: pager_id,
                height: 'auto',
                autowidth: true,
                rownumbers: true,
               // rowList: [5, 10],
                viewrecords: true,
                sortname: 'TEODate',
                sortorder: "asc",
                recordtext: '{2} records found',
                onSelectRow: function ()
                {
                    
                }, loadComplete: function ()
                {
                    jQuery("#" + subgrid_table_id).jqGrid('setLabel', "rn", "Sr.</br> No");
                    $('#PBillId').val(row_id);
               },

            });

        },
         subGridOptions: {
            "plusicon": "ui-icon-triangle-1-s",
            "minusicon": "ui-icon-triangle-1-n",
            "openicon": "ui-icon-arrowreturn-1-e",
            //expand all rows on load
            "expandOnLoad": false
        },
         loadComplete: function () {
             $.unblockUI();

            isImprestMasterLoaded = true;
            $("#tblImprestMasterGrid").jqGrid('setLabel', "rn", "Sr.</br> No");
           // $("#tblImprestMasterGrid").jqGrid('setLabel', "subgrid", "Click To.</br>Expand");
        },
        loadError: function (xhr, ststus, error) {
            $.unblockUI();

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }

    }); //end of documents grid
   

}

function LoadTEODetailsGrid(MasterId) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    if (isDetailsGridLoaded) {
        $("#tblTEODetailsGrid").GridUnload();
        isDetailsGridLoaded = false;
    }
    jQuery("#tblTEODetailsGrid").jqGrid({
        url: '/TEO/TEODetailsList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Transaction Number', 'Type', 'Head code', 'Head Name', 'Contractor Name', 'Agreement', 'Road Name', 'DPIU', 'Credit Amount', 'Debit Amount', 'Narration', 'Edit', 'Delete'],
        colModel: [
                            { name: 'TransactionNumber', index: 'TransactionNumber', width: 0, align: 'center', sortable: true, hidden: true },
                            { name: 'CreditDebit', index: 'CreditDebit', width: 60, align: 'center'/*, sortable: true */ },
                            { name: 'AccHeadcode', index: 'AccHeadcode', width: 0, align: 'center', sortable: true, hidden: true },
                            { name: 'HeadName', index: 'HeadName', width: 200, align: 'left', sortable: true },
                            { name: 'Contractor', index: 'Contractor', width: 125, align: 'left', sortable: true, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } },
                            { name: 'Agreement', index: 'Agreement', width: 125, align: 'left', sortable: true, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } },
                            { name: 'Road', index: 'Road', width: 100, align: 'left', sortable: true, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } },
                            { name: 'DPIU', index: 'DPIU', width: 100, align: 'left', sortable: true, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } },
                            { name: 'CAmount', index: 'CAmount', width: 70, align: 'right', sortable: true },
                            { name: 'DAmount', index: 'DAmount', width: 70, align: 'right', sortable: true },
                            { name: 'Narration', index: 'Narration', width: 100, align: 'left', sortable: false },
                            { name: 'Edit', index: 'Edit', width: 30, align: 'center', sortable: false },
                            { name: 'Delete', index: 'Delete', width: 35, align: 'center', sortable: false }
        ],
        pager: jQuery('#divTEODetailsPager'),
        rowNum: 10,
        postData: {
            'masterId': MasterId
        },
        altRows: true,
        rowList: [10, 20, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        emptyrecords: 'No records to view',
        sortname: 'CreditDebit',
        sortorder: "asc",
        caption: "TEO Details",
        height: 'auto',
        autowidth: true,//'770px',
        rownumbers: true,
        footerrow: true,
        userDataOnFooter: true,
        loadComplete: function () {
            $.unblockUI();

            isDetailsGridLoaded = true;
            $("#tblTEODetailsGrid").find('a').click(function () {
                var selRowId = $(this).parents('tr').attr('id');
                var data = $("#tblTEODetailsGrid").getRowData(selRowId);
                amountValC = data['CAmount'];
                amountValD = data['DAmount'];
            });
            // This code is to show Finalize button
            var userdata = $("#tblTEODetailsGrid").getGridParam('userData');
            
            //if (userdata.isFinalize == "Y" && isFinalize != 'Y') {
            //    $("#loadTEOCreditDetails").html('');
            //    $("#loadTEODebitDetails").html('');
            //    $("#divFinalizeTEO").show('slow');
            //    $("#btnFinalizeTEO").show('slow');

            //}
            //else if (isFinalize == 'Y') {
            //    $("#loadTEOCreditDetails").html('');
            //    $("#loadTEODebitDetails").html('');
            //}
            //else {
            //    $("#divFinalizeTEO").hide('slow');
            //    $("#btnFinalizeTEO").hide('slow');
            //    $("#loadTEOCreditDetails").load("/TEO/TEODetails/" + billId + "/C", function () {
            //        $("#loadTEODebitDetails").load("/TEO/TEODetails/" + billId + "/D", function () {
            //            $.each($("select"), function () {
            //                if ($(this).find('option').length >= 1) {
            //                    $('#tr' + $(this).attr('id')).show();
            //                }
            //            });
            //        });
            //        $.each($("select"), function () {
            //            if ($(this).find('option').length >= 1) {
            //                $('#tr' + $(this).attr('id')).show();
            //            }
            //        });
            //    });
            //}
        },
        loadError: function (xhr, ststus, error) {
            $.unblockUI();

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }

    }); //end of documents grid
   

}

//function to view imprest details
function ViewTEO(urlParam) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


    $.ajax({
        url: "/TEO/TEOEntry/" + urlParam,
        type: "GET",
        async: false,
        cache: false,
        data:
            {
                "Month": $("#BILL_MONTH").val(),
                "Year": $("#BILL_YEAR").val(),
                'ImprestEntry': true
            },
        success: function (data) {
            $.unblockUI();

            $("#mainDiv").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();

            alert(xhr.responseText);
        }
    });
   

    return false;
}


//Added By Abhishek kamble 7-jan-2014
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
function UpdateAccountSession(month, year) {
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
        error: function () { }
    });
    return false;
}

function process(date) {
    var parts = date.split(' ')[0].split("/");
    return new Date(parts[2], parts[1] - 1, parts[0]);
}

function ModifiedDate(day, month, year) {
    return day + "/" + month + "/" + year;
}