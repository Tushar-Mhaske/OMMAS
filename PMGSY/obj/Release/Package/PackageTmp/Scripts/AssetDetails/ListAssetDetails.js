$(document).ready(function ()
{
    //$.validator.unobstrusive.parse('frmSearchAssetDetails');
    
    LoadAssetDetailsList();
    $("#btnView").click(function () {
       
        if ($("#frmSearchAssetDetails").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            SearchAssetDetails();
            $("#tbListAssetDetails").jqGrid('setGridState', 'visible');
            CloseAssetDetails();
            $.unblockUI();
        }
    });

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });
});

function LoadAssetDetailsList() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#tbListAssetDetails").jqGrid({
        url: '/Assets/GetAssetPaymentDetails',
        datatype: "json",
        mtype: "POST",
        postData: { Month: $("#ddlMonth option:selected").val(), Year: $('#ddlYear option:selected').val(), ChequeNo: $("#ChequeNo").val(), BillNo: $("#BillNo").val() },
        colNames: ['Voucher No', 'Voucher Date', 'Head Description', 'Cheque No', 'Cheque Date', 'Asset Amount (in Rs.)', 'Entered Asset Amount (in Rs.)', 'Payee Name', 'Action','Delete','Definalize'],
        colModel: [
                            { name: 'Voucher_No', index: 'Voucher_No', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'Voucher_date', index: 'Voucher_date', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'Head_Desc', index: 'Head_Desc', height: 'auto', width: 250, align: "left", search: true },
                            { name: 'Cheque_No', index: 'Cheque_No', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'Cheque_Date', index: 'Cheque_Date', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'Asset_Amount', index: 'Asset_Amount', height: 'auto', width: 100, align: "right", search: false },
                            { name: 'Entered_Asset_Amount', index: 'Entered_Asset_Amount', height: 'auto', width: 100, align: "right", search: false },
                            { name: 'Payee_Name', index: 'Payee_Name', height: 'auto', width: 150, align: "left", search: false },
                            { name: 'a', width: 50, sortable: false, resize: false, align: "center", search: false },
                            { name: 'b', width: 50, sortable: false, resize: false, align: "center", search: false, hidden: false },
                            { name: 'c', width: 0, sortable: false, resize: false, align: "center", search: false, hidden: true },
        ],
        pager: jQuery('#dvListAssetPager'),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "Voucher_date",
        sortorder: "desc",
        caption: "&nbsp;&nbsp; Asset Payment Details",
        height: 'auto',
        // autowidth: true,
        hidegrid: true,
        cmTemplate:{title:false},
        rownumbers: true,
        loadComplete: function (data) {
            $.unblockUI();

            $('#tbListAssetDetails_rn').html('Sr. <br/> No.');

        },
        loadError: function (xhr, ststus, error) {
            $.unblockUI();

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
function AddAssetDetails(urlparameter) {

    $('#Urlparameter').val(urlparameter);

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add/Edit Asset Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseAssetDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddAssets").unload();
        $("#divAddAssets").load('/Assets/AddEditAssetDetails?urlparameter=' + urlparameter, function (data) {
            $.validator.unobtrusive.parse($('#divAddAssets'));
            unblockPage();
            if (data.success == false) {
                alert(data.message);
            }
        });
        $('#divAddAssets').show('slow');
        $("#divAddAssets").css('height', 'auto');
    });
    $("#tbListAssetDetails").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}
function CloseAssetDetails() {

    $("#accordion").hide('slow');
    $("#tbListAssetDetails").jqGrid('setGridState', 'visible');
    $("#divSearchAsset").show('slow');
}

function SearchAssetDetails() {

    $('#tbListAssetDetails').setGridParam({
        url: '/Assets/GetAssetPaymentDetails'
    });
    $('#tbListAssetDetails').jqGrid("setGridParam", { "postData": { Month: $("#ddlMonth option:selected").val(), Year: $('#ddlYear option:selected').val(), ChequeNo: $("#ChequeNo").val(), BillNo: $("#BillNo").val() } });
    $('#tbListAssetDetails').trigger("reloadGrid", [{ page: 1 }]);

}
function DeleteAssetDetails(urlparameter) {

    if (confirm("Are you sure you want to delete Asset payment details?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/Assets/DeleteAssetPaymentDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                $.unblockUI();

                if (data.success == true) {
                    alert("Asset payment details deleted successfully");
                    $("#tbListAssetDetails").trigger('reloadGrid');
                    $("#tbExecutionList").trigger('click');
                }
                else if (data.success == false) {
                    alert("Asset payment details  is in use and can not be deleted.");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    else {
        return false;
    }

}
function DefinalizeAssetDetails(urlparameter)
{
    if (confirm("Are you sure you want to definalize Asset details?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/Assets/DefinalizeAssetDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                $.unblockUI();

                if (data.success == true) {
                    alert("Asset payment details definalized successfully");
                    $("#tbListAssetDetails").trigger('reloadGrid');
                }
                else if (data.success == false) {
                    alert("Error occurred while definalizing the details.");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    else {
        return false;
    }
}
