var isValid;

//$.validator.addMethod("comparetotalamount", function (value, element, params) {

//    var assetAmount = parseFloat($("#TotalAssetAmount").val()).toFixed(2);
//    var remainingAmount = parseFloat($("#TotalRemainingAmount").val()).toFixed(2);
//    var enteredAmount = parseFloat($("#Total_Amount").val()).toFixed(2);

//    var totalEnteredAmount = assetAmount - remainingAmount;

//    if ((totalEnteredAmount + enteredAmount) > assetAmount)
//    {
//        return false;
//    }
//    else
//    {
//        return true;
//    }
//    return false;
//});
//jQuery.validator.unobtrusive.adapters.addBool("comparetotalamount");

$(document).ready(function ()
{

    $(":input").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    })

    jQuery("#tbListDetails").jqGrid('GridUnload');
    LoadAssetDetailsList();

    $("#btnAddAssetDetails").click(function () {

        if ($("#frmAddAssetDetails").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                type: 'POST',
                url: '/Assets/AddAssetDetails/',
                data: $("#frmAddAssetDetails").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    $.unblockUI();

                    if (data.success === undefined) {
                        $("#divAddAssetDetails").html(data);
                    }
                    else if (data.success == true) {
                        alert(data.message);
                        //$("#tbListDetails").trigger('reloadGrid');
                        $("#tbListAssetDetails").trigger('reloadGrid');
                        $("#divAddAssets").load('/Assets/AddEditAssetDetails?urlparameter=' + $("#Urlparameter").val(), function (data) {
                            $.validator.unobtrusive.parse($('#divAddAssets'));
                            unblockPage();
                            if (data.success == false)
                            {
                                alert(data.message);
                            }
                        });
                        //$("#tbListDetails").trigger('reloadGrid');
                        //LoadAssetDetailsList();
                    }
                    else {
                        $("#divError").show();
                        $("#divError").html(data.message);
                    }
                },
                error: function () {
                    $.unblockUI();

                    alert("Request can not be processed at this time.");
                }
            })
        }
    });

    $("#btnUpdateAssetDetails").click(function () {

        //ValidateTotalAmount();
        //if (isValid == false) {
        //    alert('Total of Asset details amount exceeding the available total amount.');
        //    return false;

        //}

        if ($("#frmAddAssetDetails").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                type: 'POST',
                url: '/Assets/EditAssetDetails/',
                data: $("#frmAddAssetDetails").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    $.unblockUI();

                    if (data.success === undefined) {
                        $("#divAddAssetDetails").html(data);
                    }
                    else if (data.success == true) {
                        alert(data.message);
                        //$("#tbListDetails").trigger('reloadGrid');
                        $("#tbListAssetDetails").trigger('reloadGrid');
                        $("#divAddAssets").load('/Assets/AddEditAssetDetails?urlparameter=' + $("#Urlparameter").val(), function (data) {
                            $.validator.unobtrusive.parse($('#divAddAssets'));
                            unblockPage();
                            if (data.success == false) {
                                alert(data.message);
                            }
                        });
                        $("#btnCancelBankDetails").trigger('click');
                    }
                    else {
                        //alert(data.message);
                        $("#divError").show();
                        $("#divError").html(data.message);
                    }
                },
                error: function () {
                    $.unblockUI();
                    alert("Request can not be processed at this time.");
                }
            })
        }
    })

    $("#btnResetAssetDetails").click(function () {

        $("#divError").hide('slow');

    });
   

    $("#btnCancelAssetDetails").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#divAddAssets").load('/Assets/AddEditAssetDetails?urlparameter=' + $("#Urlparameter").val(), function (data) {
            $.validator.unobtrusive.parse($('#divAddAssets'));
            $.unblockUI();

        });
    });


    $("#Quantity").keypress(function (e) {

        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 46 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });


    $("#Rate").keypress(function (e) {

        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });


    $("#Quantity,#Rate").blur(function () {

        var rate = parseFloat($("#Rate").val());
        var quantity = parseInt($("#Quantity").val());
        if ($("#Rate").val() == '')
        {
            rate = 0;
        }
        if ($("#Quantity").val() == '') {
            quantity = 0;
        }
        var totalAmount = parseFloat(rate * quantity);
        $("#Total_Amount").val(totalAmount);
    });


    $('#Disposal_Date').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        minDate: new Date(),
        onSelect: function (selectedDate) {

        }
    });

    $("#finalize").click(function () {

        FinalizeDetails();
    });

});
function LoadAssetDetailsList(BillId, HeadId) {
    
    jQuery("#tbListDetails").jqGrid('GridUnload');

    jQuery("#tbListDetails").jqGrid({
        url: '/Assets/GetAssetDetailsList',
        datatype: "json",
        mtype: "POST",
        postData: { BillId: $("#Bill_Id").val(), HeadId: $("#Head_Id").val() },
        colNames: ['Asset Name', 'Serial No', 'Model No', 'Rate', 'Total Amount', 'Assigned Id', 'Disposal Date', 'Edit', 'Delete'],
        colModel: [
                            { name: 'ASSET_NAME', index: 'ASSET_NAME', height: 'auto', width: 200, align: "left", search: false },
                            { name: 'SERIAL_NO', index: 'SERIAL_NO', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'MODEL_NO', index: 'MODEL_NO', height: 'auto', width: 100, align: "left", search: true },
                            { name: 'RATE', index: 'RATE', height: 'auto', width: 100, align: "right", search: false },
                            { name: 'TOTAL_AMOUNT', index: 'TOTAL_AMOUNT', height: 'auto', width: 150, align: "right", search: false },
                            { name: 'ASSIGNED_ID', index: 'ASSIGNED_ID', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'DISPOSAL_DATE', index: 'DISPOSAL_DATE', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'a', width: 50, align: "center", search: false },
                            { name: 'b', width: 50, align: "center", search: false },
                            //{ name: 'FINALIZE', index: 'FINALIZE', height: 'auto', width: 0, align: "center", search: false ,hidden:true},


        ],
        pager: jQuery('#dvAssetPager'),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "ASSET_NAME",
        sortorder: "desc",
        caption: "&nbsp;&nbsp;Asset Details List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
            if ($('#tbListDetails').jqGrid('getGridParam', 'reccount') > 0) {
                $("#dvAssetPager_right").html("<span style='float:left' class='ui-icon ui-icon-info'></span>Total Asset Amount:" + $('#TotalAssetAmount').val() + "&nbsp;&nbsp;Total Available Amount:" + $("#TotalRemainingAmount").val());

                if (data.IsFinalize == true) {
                    $("#tblFinalize").hide();
                }
                else if ($("#TotalRemainingAmount").val() == 0) {
                    if ($("#tbAddAssetetails").is(":visible")) {
                        $("#tbAddAssetetails").hide('slow');
                    }
                    $("#tblFinalize").show('slow');
                }
                else
                {
                    if (!$("#tbAddAssetetails").is(":visible")) {
                        $("#tbAddAssetetails").show('slow');
                    }
                    $("#tblFinalize").hide('slow');
                }
            }

            //Added By Abhishek Kamble 11-nov-2013
            $('#tbListDetails_rn').html('Sr.<br/>No');
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
function DeleteAsset(urlparameter) {
    
    if (confirm("Are you sure you want to delete Asset details?")) {
        $.ajax({
            type: 'POST',
            url: '/Assets/DeleteAssetDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert("Asset details deleted successfully");
                    //$("#tbListDetails").trigger('reloadGrid');
                    $("#tbListAssetDetails").trigger('reloadGrid');
                    $("#divAddAssets").load('/Assets/AddEditAssetDetails?urlparameter=' + $("#Urlparameter").val(), function (data) {
                        $.validator.unobtrusive.parse($('#divAddAssets'));
                    });
                }
                else if (data.success == false) {
                    alert("Asset details is in use and can not be deleted.");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.responseText);
            }
        });
    }
    else {
        return false;
    }

}
function EditAsset(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    urlparameter = urlparameter + '$' + $('#TotalAssetAmount').val();
    $("#divAddAssetDetails").unload();
    $("#divAddAssetDetails").load("/Assets/GetAssetInformation?id=" + urlparameter, function (data) {
        $.unblockUI();

        if ((data.success == false)) {
            alert(data.message);
        }
        $("#divAddAssetDetails").show();
        $.validator.unobtrusive.parse($('#frmAddAssetDetails'));
    });
}
function ValidateTotalAmount()
{
    $.ajax({

        type: 'POST',
        url: '/Assets/CheckTotalAmount?' + $.param({ totalAmount: $("#Total_Amount").val() }),
        data: $("#frmSearchAssetDetails").serialize(),
        async: false,
        cache:false,
        success: function (data)
        {
            if (data.success == true)
            {
                isValid = true;
            }
            else if (data.success == false) {
                isValid = false;
            }
        },
        error: function (data)
        {
            isValid = false;
        }


    });
}
function FinalizeDetails()
{

    var final = confirm("Are you sure to Finalize the Asset Details?");
    if (final) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({

            type: 'POST',
            url: '/Assets/FinalizeAssetDetails/' + $("#Bill_Id").val() + "$" + $("#Head_Id").val(),
            async: false,
            cache: false,
            success: function (data) {
                $.unblockUI();

                if (data.success == true) {
                    $("#tblFinalize").hide('slow');
                    $("#tbListAssetDetails").trigger('reloadGrid');
                    $("#tbListDetails").trigger('reloadGrid');
                    alert('Details Finalized.');
                }
                else if (data.success == false) {
                    alert(data.message);
                }
            },
            error: function (data) {
                $.unblockUI();

            }
        });
    }
}
