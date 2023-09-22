var oldAsset = $("#ASSET_GROSS_AMOUNT").val();
var oldLib = $("#LIB_GROSS_AMOUNT").val();

$(function () {
    $.validator.unobtrusive.adapters.add('isvalidobamount', ['obtype'], function (options) {
        options.rules['isvalidobamount'] = options.params;
        options.messages['isvalidobamount'] = options.message;
    });

    $.validator.addMethod("isvalidobamount", function (value, element, params) {
        return (parseFloat($('#' + params.obtype).val()) == parseFloat(value));
    }, function (params, element) {
        if (params.obtype == "ASSET_GROSS_AMOUNT") {
            return "Liability and Asset amount must be same";
        }
        else {
            return "Asset and Liability amount must be same";
        }
    });

    $.validator.unobtrusive.adapters.add('ismasteramountless', ['obtype'], function (options) {
        options.rules['ismasteramountless'] = options.params;
        options.messages['ismasteramountless'] = options.message;
    });

    $.validator.addMethod("ismasteramountless", function (value, element, params) {

        if (isGridLoaded && oldAsset != "" && oldLib != "") {
            var totalAsset = 0;
            var totalLib = 0;
            var firstRow = $("#tblOBList").getRowData($("#tblOBList").getDataIDs()[0]);
            var secondRow = $("#tblOBList").getRowData($("#tblOBList").getDataIDs()[1]);
            if (firstRow['OBType'] == "Assets") {
                totalAsset = firstRow["DetailsAmount"];
                totalLib = secondRow["DetailsAmount"];
            }
            else {
                totalAsset = secondRow["DetailsAmount"];
                totalLib = firstRow["DetailsAmount"];
            }

            if (params.obtype == "ASSET_GROSS_AMOUNT") {
                return (parseFloat(value) >= parseFloat(totalAsset));
            }
            else {
                return (parseFloat(value) >= parseFloat(totalLib));
            }
        }
        else {
            return true;
        }
    });
    //}, function (params, element) {
    //    if (params.obtype == "ASSET_GROSS_AMOUNT") {
    //        return "Asset master amount must be greater than details amount";
    //    }
    //    else {
    //        return "Liabitity master amount must be greater than details amount";
    //    }
    //});


    $.validator.unobtrusive.adapters.add('isvalidobdate', ['obtype'], function (options) {
        options.rules['isvalidobdate'] = options.params;
    });

    $.validator.addMethod("isvalidobdate", function (value, element, params) {
        return (process($('#' + params.obtype).val()).getTime() == process(value).getTime());
    }, function (params, element) { if (params.obtype == "ASSET_BILL_DATE") { return "Liability and Asset date must be same"; } else { return "Asset and Liability date must be same"; } });


    $.validator.unobtrusive.adapters.add('isdateafter', ['propertytested', 'allowequaldates'], function (options) {
        options.rules['isdateafter'] = options.params;
        options.messages['isdateafter'] = options.message;
    });

    $.validator.addMethod("isdateafter", function (value, element, params) {
        var fullDate = new Date();
        var twoDigitMonth = fullDate.getMonth() + 1 + ""; if (twoDigitMonth.length == 1) twoDigitMonth = "0" + twoDigitMonth;
        var twoDigitDate = fullDate.getDate() + ""; if (twoDigitDate.length == 1) twoDigitDate = "0" + twoDigitDate;
        var currentDate = twoDigitDate + "/" + twoDigitMonth + "/" + fullDate.getFullYear();
        return (params.allowequaldates) ? process(currentDate) >= process(value) : process(currentDate) > process(value);
    });


});

function process(date) {
    var parts = date.split("/");
    return new Date(parts[2], parts[1] - 1, parts[0]);
}

$(document).ready(function () {

    $("#ASSET_BILL_DATE").blur(function () {
        $("#LIB_BILL_DATE").val($("#ASSET_BILL_DATE").val());
    });

    $("#LIB_BILL_DATE").blur(function () {
        $("#ASSET_BILL_DATE").val($("#LIB_BILL_DATE").val());
    });

    $("#ASSET_GROSS_AMOUNT").focus(function () {
        $("#LIB_BILL_DATE").val($("#ASSET_BILL_DATE").val());

        //added by abhishek kamble 29-8-2013
        AssetMessages();
        
    });

    $("#LIB_GROSS_AMOUNT").focus(function () {
        $("#ASSET_BILL_DATE").val($("#LIB_BILL_DATE").val());

        //added by abhishek kamble 29-8-2013
        LiabilitiesMessages();
       
    });

    $("#ASSET_GROSS_AMOUNT").blur(function () {
        $("#LIB_GROSS_AMOUNT").val($("#ASSET_GROSS_AMOUNT").val());

        //added by abhishek kamble 29-8-2013
        AssetMessages();
        
    });

    $("#LIB_GROSS_AMOUNT").blur(function () {
        $("#ASSET_GROSS_AMOUNT").val($("#LIB_GROSS_AMOUNT").val());
        
        //added by abhishek kamble 29-8-2013
        LiabilitiesMessages();

    });

    

    $.validator.unobtrusive.parse($('#frmOBAddMaster'));

    $("#DivIcoMasterOB").click(function () {
        if ($(this).hasClass('ui-icon-circle-triangle-n')) {
            $("#tblOBMaster").hide('slide');
            $(this).removeClass('ui-icon-circle-triangle-n');
            $(this).addClass('ui-icon-circle-triangle-s');
        }
        else {
            $("#tblOBMaster").show('slide');
            $(this).removeClass('ui-icon-circle-triangle-s');
            $(this).addClass('ui-icon-circle-triangle-n');
        }
    });

   
    if ($("#ASSET_BILL_DATE").val() == "") {
        $("#ASSET_BILL_DATE").datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            showOn: 'button',
            buttonImage: '/Content/images/calendar_2.png',
            buttonImageOnly: true,
            buttonText: "Asset Opening Balance Date",
            onClose: function (selectedDate) {

                $('#LIB_BILL_DATE').val(selectedDate);
                $(this).focus().blur();
            }
        }).datepicker('setDate', new Date());
    }
    else {
        $("#ASSET_BILL_DATE").datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            showOn: 'button',
            buttonImage: '/Content/images/calendar_2.png',
            buttonText:"Asset Opening Balance Date",
            buttonImageOnly: true,
            onClose: function (selectedDate) {

                $('#LIB_BILL_DATE').val(selectedDate);
            }
        });
    }

    //if ($("#LIB_BILL_DATE").val() == "") {
    //    $("#LIB_BILL_DATE").datepicker({
    //        changeMonth: true,
    //        changeYear: true,
    //        dateFormat: "dd/mm/yy",
    //        showOn: 'button',
    //        buttonImage: '/Content/images/calendar_2.png',
    //        buttonImageOnly: true
    //    }).datepicker('setDate', new Date());
    //}
    //else {
    //    $("#LIB_BILL_DATE").datepicker({
    //        changeMonth: true,
    //        changeYear: true,
    //        dateFormat: "dd/mm/yy",
    //        showOn: 'button',
    //        buttonImage: '/Content/images/calendar_2.png',
    //        buttonImageOnly: true
    //    });
    //}

    $("#btnSaveOBMaster").click(function (evt) {

        evt.preventDefault();
        if ($('#frmOBAddMaster').valid()) {
            //if (validateOBMaster($("#ASSET_GROSS_AMOUNT").val(), $("#LIB_GROSS_AMOUNT").val())) {
            //blockPage();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                $.ajax({
                    url: "/OB/AddOBMaster/",
                    type: "POST",
                    //async: false,
                    cache: false,
                    data: $("#frmOBAddMaster").serialize(),
                    success: function (data) {
                        $.unblockUI();
                        
                        if (data.message == "undefined" || data.message == null) {
                            $("#loadOBMaster").html(data);
                        }
                        else if (data.success) {
                            alert('Opening Balance Master Added');
                            //$("#btnResetOBMaster").trigger('click');
                            $("#loadOBMaster").html("");
                            LoadGrid();
                            unblockPage();
                            return false;
                        }
                        else {
                            $("#divOBMasterError").show("slide");
                            $("#divOBMasterError span:eq(1)").html(data.message);
                        }

                        return false;
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert("Error while processing request");
                        unblockPage();
                        $.unblockUI();

                    }
                });

            //unblockPage();
            }
        //}
    });
    $("#btnEditOBMaster").click(function (evt) {
        evt.preventDefault();
        if ($('#frmOBAddMaster').valid()) {
            //blockPage();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/OB/EditOBMaster/" + billId,
                type: "POST",
                //async: false,
                cache: false,
                data: $("#frmOBAddMaster").serialize(),
                success: function (data) {
                    $.unblockUI();

                    if (data.message == "undefined" || data.message == null) {
                        $("#loadOBMaster").html(data);
                    }
                    else if (data.success) {
                        alert('Opening Balance Master Updated');
                        unblockPage();
                        $("#btnlblBackToList").hide();
                        $("#btnCancelOBMaster").trigger('click');
                        $("#divOBMasterWrapper").show();
                        //$("#tblOBList").trigger('reloadGrid');

                        return false;
                    }
                    else {
                        $("#divOBMasterError").show("slide");
                        $("#divOBMasterError span:eq(1)").html(data.message);
                    }
                    //unblockPage();
                    $.unblockUI();
                    return false;
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("Error while processing request");
                    //unblockPage();
                    $.unblockUI();
                }
            });

        }
    });
    
    $("#btnCancelOBMaster").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#loadOBMaster").hide('slow');
        $("#loadOBMaster").html('');
        $("#mainDiv").load("/OB/OpeningBalance/");
        $.unblockUI();

    });
});

function validateOBMaster(assetAmount, libAmount)
{
    $("#divOBMasterError").hide("slide");
    $("#divOBMasterError span:eq(1)").html('');

    //var detailsAssetAmount = 0;
    //var detailsLibAmount = 0;
    //if (isDetailsGridLoaded) {
    //    detailsAssetAmount = $("#tblOBDetailsGrid").jqGrid('getCol', 'AssetAmount', false, "sum");
    //    detailsLibAmount = $("#tblOBDetailsGrid").jqGrid('getCol', 'LibAmount', false, "sum");
    //}

    //if (parseFloat(detailsAmountC).toFixed(2) == 0 && parseFloat(detailsAmountD).toFixed(2) == 0) {
    //    $("#divTEOMasterError").hide("slide");
    //    $("#divTEOMasterError span:eq(1)").html('');
    //    return true;
    //}
    //else if (parseFloat(detailsAmountC) > parseFloat(amountToValidate)) {
    //    $("#divTEOMasterError").show("slide");
    //    $("#divTEOMasterError span:eq(1)").html('<strong>Alert: </strong> Amount must not be less than Rs.' + detailsAmountC);
    //    return false;
    //}
    //else if (parseFloat(detailsAmountD) > parseFloat(amountToValidate)) {
    //    $("#divTEOMasterError").show("slide");
    //    $("#divTEOMasterError span:eq(1)").html('<strong>Alert: </strong> Amount must not be less than Rs.' + detailsAmountD);
    //    return false;
    //}
    //else {
    //    $("#divTEOMasterError").hide("slide");
    //    $("#divTEOMasterError span:eq(1)").html('');
    //    return true;
    //}
    return true;

}

//added by abhishek kamble 29-8-2013
function AssetMessages()
{

    if ($("#ASSET_GROSS_AMOUNT").val() == $("#LIB_GROSS_AMOUNT").val()) {
        $("#spnAssetAmount").html('');
        $("#spnLibGrossAmount").html('');
        $("#ASSET_GROSS_AMOUNT").attr('class', 'pmgsy-textbox valid');
    }
    else {
        $("#ASSET_GROSS_AMOUNT").attr('class', 'pmgsy-textbox input-validation-error');
    }

}

//added by abhishek kamble 29-8-2013
function LiabilitiesMessages() {

    if ($("#ASSET_GROSS_AMOUNT").val() == $("#LIB_GROSS_AMOUNT").val()) {
        $("#spnAssetAmount").html('');
        $("#spnLibGrossAmount").html('');
        $("#LIB_GROSS_AMOUNT").attr('class', 'pmgsy-textbox valid');
    }
    else {
        $("#LIB_GROSS_AMOUNT").attr('class', 'pmgsy-textbox input-validation-error');

    }

}