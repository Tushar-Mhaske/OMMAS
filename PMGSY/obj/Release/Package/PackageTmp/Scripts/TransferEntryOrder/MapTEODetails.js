var isValid = true;
$(document).ready(function () {

    var month = $("#ddlMonth option:selected").val();
    var year = $("#ddlYear option:selected").val();

    LoadTEORecieptDetails(month, year);

    //loads the list of TEO and Reciept details
    $("#btnViewDetails").click(function () {

        month = $("#ddlMonth option:selected").val();
        year = $("#ddlYear option:selected").val();
        LoadTEORecieptDetails(month, year);
    });

    $("#lblBackToList").click(function () {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/TEO/OldImprestPayments/",
            type: "GET",
            async: false,
            cache: false,
            data:
                {
                    "Month": month,
                    "Year": year
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


    });


    $("#btnMapDetails").click(function () {

        var selRows = $("#tblTEORecieptList").jqGrid('getGridParam', 'selarrrow');
        if (selRows == '')
        {
            alert('Please select imprests to map.');
            return false;
        }
        ValidatePaymentAmount(selRows);
        if (isValid == false) {
            return false;
        }
    });


});
//method for populating the list of unmapped TEO and Reciept details
function LoadTEORecieptDetails(month,year)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#tblTEORecieptList").jqGrid('GridUnload');

    jQuery("#tblTEORecieptList").jqGrid({
        url: '/TEO/ListSettledTEORecieptDetails',
        datatype: "json",
        mtype: "POST",
        colNames: ['Reciept/TEO No','Reciept/TEO Date','Month-Year', 'Cheque/Epayment', 'Cheque No.', 'Description', 'Amount', 'Narration'],//,'Select'],
        colModel: [
                            { name: 'BILL_NO', index: 'BILL_NO', width: 80, align: 'center', sortable: true },
                            { name: 'BILL_DATE', index: 'BILL_DATE', width: 80, align: 'center', sortable: true },
                            { name: 'MonthYear', index: 'MonthYear', width: 80, align: 'center', sortable: true },
                            { name: 'CHQ_EPAY', index: 'CHQ_EPAY', width: 80, align: 'center', sortable: true },
                            { name: 'CHQ_NO', index: 'CHQ_NO', width: 80, align: 'left', sortable: true },
                            { name: 'TXN_DESC', index: 'TXN_DESC', width: 150, align: 'left', sortable: true },
                            { name: 'AMOUNT', index: 'AMOUNT', width: 80, align: 'right', sortable: true },
                            { name: 'NARRATION', index: 'NARRATION', width: 150, align: 'left', sortable: true },
                            //{
                            //    name: 'a', index: 'a', width: 50, align: 'center', sortable: false, editable: true,
                            //    edittype: "checkbox", formatter: 'checkbox', formatoptions: { disabled: false },
                            //},
        ],
        pager: jQuery('#pgTEORecieptList'),
        rowNum: 10,
        postData: {
            'month': month,
            'year': year,
        },
        //altRows: true,
        rowList: [10, 20, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'BILL_NO',
        sortorder: "asc",
        caption: "Imprest Settlement Details",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        multiselect:true,
        loadComplete: function () {
            $.unblockUI();

            $('#tblTEORecieptList_rn').html('Sr.<br/>No.');
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

    }); //end of payments grid
}
//function for validating the payment amount with the selected TEO and Reciept vouchers
function ValidatePaymentAmount(s_billIds)
{
    
    $.ajax({

        type: 'POST',
        url: '/TEO/ValidatePaymentAmount',
        data:
        {
            S_BILL_ID: s_billIds,
            P_BILL_ID: $("#P_BILL_ID").val(),
        },
        async: false,
        cache: false,
        success: function (data)
        {
            if (data.IsValid == true) {
                isValid = true;
                $("#tblTEORecieptList").trigger('reloadGrid');
                alert(data.Message);
            }
            else if(data.IsValid == false)
            {
                isValid = false;
                alert(data.Message);
            }
            return true;
        },
        error: function ()
        {
            return false;
        }

    });
}