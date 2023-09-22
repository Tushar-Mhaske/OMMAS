$(document).ready(function () {
    //Added By Abhishek kamble 3-jan-2014 start change
    GetClosedMonthAndYear();

    var month = $("#ddlMonth option:selected").val();
    var year = $("#ddlYear option:selected").val();

    LoadOldImrestPayments(month, year);

    $("#btnView").click(function () {
        
        month = $("#ddlMonth option:selected").val();
        year = $("#ddlYear option:selected").val();
        LoadOldImrestPayments(month,year);
    });

    //new change done by Vikram for adding the changed month and year in session
    $("#ddlMonth").change(function () {
        UpdateAccountSession($("#ddlMonth").val(), $("#ddlYear").val());
    });

    $("#ddlYear").change(function () {
        UpdateAccountSession($("#ddlMonth").val(), $("#ddlYear").val());
    });

});
//this function loads the Imprest payment list in jqgrid
function LoadOldImrestPayments(month,year)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tblImprestPaymentList").jqGrid('GridUnload');

    jQuery("#tblImprestPaymentList").jqGrid({
        url: '/TEO/OldImprestPaymentList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Voucher No','Voucher Date', 'Month-Year', 'Cheque No', 'Payee Name', 'Gross Amount', 'Settelled Amount', 'Map TEO Details'],
        colModel: [
                            { name: 'BILL_NO', index: 'BILL_NO', width: 80, align: 'center', sortable: true },
                            { name: 'BILL_DATE', index: 'BILL_DATE', width: 80, align: 'center', sortable: true },
                            { name: 'MonthYear', index: 'MonthYear', width: 80, align: 'center', sortable: true },
                            { name: 'CHQ_NO', index: 'CHQ_NO', width: 80, align: 'left', sortable: true },
                            { name: 'PAYEE_NAME', index: 'PAYEE_NAME', width: 200, align: 'center', sortable: true },
                            { name: 'Advance_Amount', index: 'Advance_Amount', width: 80, align: 'right', sortable: true},
                            { name: 'Settled_Amount', index: 'Settled_Amount', width: 70, align: 'center', sortable: true },
                            { name: 'a', index: 'a', width: 50, align: 'center', sortable: false },
        ],
        pager: jQuery('#pgImprestPaymentList'),
        rowNum: 10,
        postData: {
            'month': month,
            'year': year,
        },
        altRows: true,
        rowList: [10, 20, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'BILL_DATE',
        sortorder: "desc",
        caption: "Imprest Payment Details",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        loadComplete: function () {
            $.unblockUI();

            $('#tblImprestPaymentsList_rn').html('Sr.<br/>No.');
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

function MapTEODetails(billId)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/TEO/MapTEODetails?id="+billId,
        type: "GET",
        async: false,
        cache: false,
        data:
            {
                "Month": $("#ddlMonth").val(),
                "Year": $("#ddlYear").val()
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

//new method added by Vikram on 01-Jan-2014
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
