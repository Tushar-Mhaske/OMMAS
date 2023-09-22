// Added by Srishti 
$(document).ready(function () {
    $('#ddlBankName').chosen();
});

$.validator.unobtrusive.adapters.add('comparedate', ['startdate'], function (options) {
    options.rules['comparedate'] = options.params;
    options.messages['comparedate'] = options.message;
});

$.validator.addMethod("comparedate", function (value, element, params) {

    if (value == "") {
        return true;
    }

    if (process(value) < process($("#" + params.startdate).val()))
        return false;
    else
        return true;

});

$.validator.unobtrusive.adapters.add('compareagreementdate', ['startdate'], function (options) {
    options.rules['compareagreementdate'] = options.params;
    options.messages['compareagreementdate'] = options.message;
});

$.validator.addMethod("compareagreementdate", function (value, element, params) {

    if (value == "") {
        return true;
    }

    if (process(value) < process($("#" + params.startdate).val()))
        return false;
    else
        return true;

});

$.validator.addMethod("compareolddate", function (value, element, params) {

    var oldCloseDate = $("#OldCloseDate").val();
    if (process(oldCloseDate) < process($("#BANK_ACC_CLOSE_DATE").val())) {
        return true;
    }
    return false;
});
jQuery.validator.unobtrusive.adapters.addBool("compareolddate");

$.validator.addMethod("compareolddate", function (value, element, params) {

    var oldCloseDate = $("#OldCloseDate").val();
    if (process(oldCloseDate) < process($("#BANK_ACC_OPEN_DATE").val())) {
        return true;
    }
    return false;
});
jQuery.validator.unobtrusive.adapters.addBool("compareolddate");

$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmAddFinancialProgress'));

    $(":input").bind("keypress", function (e) {
        if (e.keyvalue == 13) {
            return false;
        }
    })

    LoadBankListAcc();
    LoadIfscList();

    $('#ddlBankName').change(function () {
        LoadIfscList();
    });

    $('#BANK_AGREEMENT_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        buttonImageOnly: true,
        //maxDate: new Date(),
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {

        }
    });

    $('#BANK_ACC_OPEN_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        changeMonth: true,
        changeYear: true,
        buttonImageOnly: true,
        maxDate: new Date(),
        onSelect: function (selectedDate) {
            //$("#BANK_ACC_CLOSE_DATE").datepicker("option", "minDate", selectedDate);

            $('#BANK_ACC_OPEN_DATE').focus();
            $('#BANK_ACC_CLOSE_DATE').focus();



        }
    });


    $('#BANK_ACC_CLOSE_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        maxDate: new Date(),
        onSelect: function (selectedDate) {
            //$("#BANK_ACC_OPEN_DATE").datepicker("option", "maxDate", selectedDate);
        }
    });

    $("#btnSaveBankDetails").click(function () {

        if ($("#frmAddBankDetails").valid()) {

            $.ajax({
                type: 'POST',
                url: '/Bank/AddBankDetails/',
                data: $("#frmAddBankDetails").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success === undefined) {


                        $("#divAddBankDetails").html(data);
                    }
                    else if (data.success) {
                        alert(data.message);
                        $("#tbBankDetailsList").trigger('reloadGrid');
                        $("#btnResetBankDetails").trigger('click');
                        LoadAddView();
                    }
                    else {
                        $("#divBankDetailsError").show();
                        $("#divBankDetailsError").html(data.message);
                    }
                },
                error: function () {
                    alert("Request can not be processed at this time.");
                }
            })
        }
    });

    $("#btnEditBankDetails").click(function () {

        if ($("#frmAddBankDetails").valid()) {

            $.ajax({
                type: 'POST',
                url: '/Bank/EditBankDetails/',
                data: $("#frmAddBankDetails").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success === undefined) {

                        $("#divAddBankDetails").html(data);
                        //$("#divBankDetailsError").show();
                        //$("#divBankDetailsError").html(data.message);
                    }
                    else if (data.success == true) {
                        alert(data.message);
                        $("#tbBankDetailsList").trigger('reloadGrid');
                        $("#btnCancelBankDetails").trigger('click');
                    }
                    else {
                        //alert(data.message);
                        $("#divBankDetailsError").show();
                        $("#divBankDetailsError").html(data.message);
                    }
                },
                error: function () {
                    alert("Request can not be processed at this time.");
                }
            })
        }
    })

    $("#btnCancelBankDetails").click(function () {

        $("#divAddBankDetails").hide('slow');
        $("#btnCreateNew").show('slow');

    });

    $("#imgCloseBankDetails").click(function () {

        $("#divAddBankDetails").hide('slow');
        $("#btnCreateNew").show('slow');

    });


});
function process(date) {
    if (date == "1/1/0001") {
        date = "01/01/1900";
    }
    var parts = date.split("/");
    return new Date(parts[2], parts[1] - 1, parts[0]);
}

function LoadBankListAcc() {

    $.ajax({
        url: "/Master/GetBankList",
        cache: false,
        type: "GET",
        async: false,
        success: function (data) {


            var rows = new Array();
            for (var i = 0; i < data.length; i++) {
                rows[i] = { data: data[i].Text, value: data[i].Text, id: data[i].Value };
            }

            $('#BANK_NAME').autocomplete({
                source: rows,
                dataType: 'json',
                formatItem: function (row, i, n) {
                    return row.Text;
                },
                width: 150,
                highlight: true,
                minChars: 3,
                selectFirst: true,
                max: 10,
                scroll: true,
                width: 100,
                maxItemsToShow: 10,
                maxCacheLength: 10,
                mustMatch: true
            })

        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert("An error occurred while executing this request.\n" + xhr.responseText);
            if (xhr.responseText == "session expired") {
                //$('#frmECApplication').submit();
                alert(xhr.responseText);
                window.location.href = "/Login/LogIn";
            }
        }
    })
}

function LoadIfscList() {
    if ($("#ddlBankName option:selected").val() != "") {
        $.ajax({
            url: "/Master/PopulateIfscByBankName",
            cache: false,
            type: "GET",
            async: false,
            data: { bankName: $("#ddlBankName option:selected").text(), },
            success: function (data) {

                var rows = new Array();
                for (var i = 0; i < data.length; i++) {
                    rows[i] = { data: data[i].Text, value: data[i].Text, id: data[i].Value };
                }

                $('#MAST_IFSC_CODE').autocomplete({
                    source: rows,
                    dataType: 'json',
                    formatItem: function (row, i, n) {
                        return row.Text;
                    },
                    width: 150,
                    highlight: true,
                    minChars: 3,
                    selectFirst: true,
                    max: 10,
                    scroll: true,
                    width: 100,
                    maxItemsToShow: 10,
                    maxCacheLength: 10,
                    mustMatch: true
                })

            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert("An error occurred while executing this request.\n" + xhr.responseText);
                if (xhr.responseText == "session expired") {
                    //$('#frmECApplication').submit();
                    //  alert(xhr.responseText);
                    alert('An Error occurred while processing your request');
                    window.location.href = "/Login/LogIn";
                }
            }
        })
    }
}