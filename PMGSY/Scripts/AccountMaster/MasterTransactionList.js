

$(document).ready(function () {
    
    //Cancel
    $("#btnCreateNew").click(function () {

        $("#btnCreateNew").hide();
        $("#btnSearch").show();

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


            $.ajax({
                type: 'GET',
                url: '/AccountMaster/AddEditMasterTransactionDetails/',                
                async: false,
                cache: false,
                success: function (data) {
                    $("#dvMasterTransactionAddEdit").html(data);
                    $.unblockUI();
                },
                error: function () {
                    $.unblockUI();
                    alert("Request can not be processed at this time.");
                }
            })
    });
    
    //Search
    $("#btnSearch").click(function () {
        $("#btnSearch").hide();
        $("#btnCreateNew").show();

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            type: 'GET',
            url: '/AccountMaster/SearchTransactionDetailsView/',
            async: false,
            cache: false,
            success: function (data) {
                $("#dvMasterTransactionAddEdit").html(data);
                $.unblockUI();
            },
            error: function () {
                $.unblockUI();
                alert("Request can not be processed at this time.");
            }
        })
    });


    //List
    LoadTransactionDetailsList();
});




function LoadTransactionDetailsList() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#tbTransactionDetailsList").jqGrid({
        url: '/AccountMaster/MasterTransactionList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Parent','Description', 'Narration', 'Cash/Cheque', 'Bill Type', 'Fund Type', 'Is Operational', 'Level', 'Is Req After Porting', 'Edit', 'Delete','Status'],
        colModel: [
                            { name: 'TXN_PARENT_ID', index: 'TXN_PARENT_ID', height: 'auto', width: 70, align: "center", search: false, sortable: false ,hidden:true},
                            { name: 'TXN_DESC', index: 'TXN_DESC', height: 'auto', width: 300, align: "left", search: false, sortable: false },
                            { name: 'TXN_NARRATION', index: 'TXN_NARRATION', height: 'auto', width: 265, align: "left", search: false, sortable: false },
                            { name: 'CASH_CHQ', index: 'CASH_CHQ', height: 'auto', width: 70, align: "center", search: false, sortable: false },
                            { name: 'BILL_TYPE', index: 'BILL_TYPE', height: 'auto', width: 100, align: "center", search: true, sortable: false },
                            { name: 'FUND_TYPE', index: 'FUND_TYPE', height: 'auto', width: 80, align: "center", search: true, sortable: false },
                            { name: 'IS_OPERATIONAL', index: 'IS_OPERATIONAL', height: 'auto', width: 60, align: "center", search: true, sortable: false },
                            { name: 'OP_LVL_ID', index: 'OP_LVL_ID', height: 'auto', width: 50, align: "center", search: true, sortable: false },
                            { name: 'IS_REQ_AFTER_PORTING', index: 'IS_REQ_AFTER_PORTING', height: 'auto', width: 70, align: "center", search: true, sortable: false },
                            { name: 'edit', width: 30, sortable: false, resize: false, formatter: FormatColumnEdit, align: "center", sortable: false },
                            { name: 'delete', width: 30, sortable: false, resize: false, formatter: FormatColumnDelete, align: "center", sortable: false },
                            { name: 'Status', index: 'Status', height: 'auto', width: 70, align: "center", search: true, sortable: false },

        ],
        pager: jQuery('#pagerTransaction'),
        rowNum: 0,
        //rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "TXN_CODE",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Transaction Details",
        height: 'auto',
        //width: "100%",
        //shrinkToFit:true,
        grouping:true, 
        groupingView : { 
            groupField : ['TXN_PARENT_ID'],
            groupDataSorted: true,
            groupText: '',
            groupColumnShow:false
        },
        hidegrid: true,
        rownumbers: true,
        loadComplete: function (data) {
            $('#tbTransactionDetailsList_rn').html('Sr.<br/>No.');
            $("#tbTransactionDetailsList").parents('div.ui-jqgrid-bdiv').css("max-height", "420px");
            $("#pagerTransaction_center").html('');
            //jQuery("#tbScheduleCurrentAssetsList").jqGrid('setCell', '(C) Other Items', 'Particulars', "", { 'font-size': '13px', 'font-weight': 'bold' });
            $.unblockUI();
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

function FormatColumnEdit(cellvalue, options, rowObject)
{
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Details' onClick ='EditDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function FormatColumnDelete(cellvalue, options, rowObject)
{
    if (cellvalue == "") {
        return "-";
    }
    else {
        return "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Details' onClick ='DeleteDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
}

function EditDetails(urlParam)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/AccountMaster/GetMasterTransactionDetails/' + urlParam,
        async: false,
        cache: false,
        success: function (data) {
            $("#dvMasterTransactionAddEdit").html(data);
            $.unblockUI();
        },
        error: function () {
            $.unblockUI();
            alert("Request can not be processed at this time.");
        }
    })
}


function DeleteDetails(urlParam) {

    if (confirm("Are you sure you want to delete Details")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/AccountMaster/DeleteMasterTransactionDetails/' + urlParam,
            async: false,
            cache: false,
            success: function (data) {
                $.unblockUI();
                if (data.success) {
                    alert(data.message);

                   // $("#tbTransactionDetailsList").trigger('reloadGrid');
               
                    if ($("btnSearch").is(":visible")) {

                        //alert("test : del");


                        $('#tbTransactionDetailsList').setGridParam({
                            url: '/AccountMaster/MasterTransactionList', datatype: 'json'
                        });
                        $('#tbTransactionDetailsList').jqGrid("setGridParam", { "postData": { IsSearch: false } });
                        $('#tbTransactionDetailsList').trigger("reloadGrid");
                        ResetForm();

                    }
                    else {

                     
                        var IsOperational;

                        if ($("#rdoOperationalYes").is(":checked")) {
                            IsOperational = true;
                        }
                        else if ($("#rdoOperationalNo").is(":checked")) {
                            IsOperational = false;
                        }

                        $('#tbTransactionDetailsList').setGridParam({
                            url: '/AccountMaster/MasterTransactionList', datatype: 'json'
                        });


                        $('#tbTransactionDetailsList').jqGrid("setGridParam", { "postData": { ParentTxn: $('#ddlParentTxn option:selected').val(), Level: $('#ddlLevel option:selected').val(), CashCheque: $('#ddlCashCheque option:selected').val(), BillType: $('#ddlBillType option:selected').val(), IsOperational: IsOperational, IsSearch: true } });
                        $('#tbTransactionDetailsList').trigger("reloadGrid");
                        
                    }

                    //$("#btnCancel").trigger('click');
                    
                }
                else {
                    //$("#divError").show();
                    //$("#errorSpan").html(data.message);
                    alert(data.message);
                }
            },
            error: function () {
                $.unblockUI();
                alert("Request can not be processed at this time.");
            }
        })
    }
}


