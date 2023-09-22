$(function () {
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

    $.validator.unobtrusive.adapters.add('isdatebeforeob', ['propertytested'], function (options) {
        options.rules['isdatebeforeob'] = options.params;
        options.messages['isdatebeforeob'] = options.message;
    });

    $.validator.addMethod("isdatebeforeob", function (value, element, params) {
        return (process($('#' + params.propertytested).val()) <= process(value));
    });

    $.validator.unobtrusive.adapters.add('isvaliddate', ['month', 'year'], function (options) {
        options.rules['isvaliddate'] = options.params;
        options.messages['isvaliddate'] = options.message;
    });

    $.validator.addMethod("isvaliddate", function (value, element, params) {
        return (($('#' + params.month).val() == value.split('/')[1].replace(/^0+/, '')) && ($('#' + params.year).val() == value.split('/')[2]));
    }, "TEO Date must be in " + $("#BILL_MONTH option:selected").text() + " month and " + $('#BILL_YEAR').val() + " year");

});


$(document).ready(function () {

    //Added By Abhishek kamble 20-jan-2014 start    
    var currentDate = $("#CURRENT_DATE").val().split("/");
    var currentDay = currentDate[0];
    var ModifiedCurrentDate = ModifiedDate(currentDate[0], $("#BILL_MONTH").val(), $("#BILL_YEAR").val());
    //Added By Abhishek kamble 20-jan-2014 end

   // alert('a' + $('#hdnBillid').val());

    if (!($('#hdnBillid').val() == '')) {
       
    }
    else {
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

    //Added By Abhishek kamble 3-jan-2014 start change
    GetClosedMonthAndYear();
    //Added By Abhishek kamble 6-jan-2014 end change
    $.validator.unobtrusive.parse($('#frmTEOAddMaster'));

    //new change done by Vikram on 01-Jan-2014
    $("#BILL_NO").focus();
    //end of change

    if ($("#BILL_DATE").val() == "") {
        $("#BILL_DATE").datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            showOn: 'button',
            buttonImage: '/Content/images/calendar_2.png',
            buttonImageOnly: true,
            buttonText: 'TEO Date',
            onClose: function () {

            $(this).focus().blur();
        }
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
            buttonImageOnly: true,
            buttonText: 'TEO Date',
            onClose: function () {

                $(this).focus().blur();
            }
        });
    }

    $("#BILL_MONTH").change(function () {
        if ($(this).val() != "0") {
            month = $(this).val();
        }

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

        if (!($('#hdnBillid').val() == '')) {

        }
        else {
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

    });

    $("#BILL_YEAR").change(function () {
        if ($(this).val() != "0") {
            year = $(this).val();
        }

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

        if (!($('#hdnBillid').val() == '')) {
       
        }
        else {
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
        //new change done by Abhishek kamble on 21-jan-2014 end
    });

    $("#ddlTransMaster").change(function () {
        $("#trDetailsHeadDesc").text("");
        if ($(this).val() == "" || $(this).val() == "0") {
            $("#trddlSubTrans").hide();
        }
        else {
            if ($("#ddlTransMaster").val() != "") {
                FillInCascadeDropdown(null, '#ddlSubTrans', "/TEO/PopulateSubTransaction/" + $("#ddlTransMaster").val());
            }
            else {
                $("#ddlSubTrans").empty();
                $("#ddlSubTrans").append("<option value='0' selected=true>Select Sub Transaction</option>");
            }
        }
    });

    $("#ddlSubTrans").change(function () {
        $("#trDetailsHeadDesc").find('td:eq(3)').text("");
        if ($(this).val() != 0) {
            $.ajax({
                url: "/TEO/GetNarration/" + $("#ddlSubTrans").val(),
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {
                    $("#trDetailsHeadDesc").text(data.split('$')[1]).removeClass('ui-state-default');
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("Error while processing request");
                }
            });
        }
    });

    $("#btnSaveTEOMaster").click(function (evt) {
        evt.preventDefault();
        if ($('#frmTEOAddMaster').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/TEO/AddTEOMasterForTOB/",
                type: "POST",
                //async: false,
                cache: false,
                data: $("#frmTEOAddMaster").serialize(),
                success: function (data) {
                    $.unblockUI();

                    if (!data.success) {
                        if (data.message == "undefined" || data.message == null) {
                            $("#loadTEOMaster").html(data);
                            //added by amol Jadhav 23/07/2013
                            if ($("#ddlSubTrans").val() != 0) {
                                $("#ddlSubTrans").trigger('change');
                                $("#trddlSubTrans").show();
                            }
                        }
                        else {
                            $("#divTEOMasterError").show("slide");
                            $("#divTEOMasterError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        }
                        $.unblockUI();
                        return false;
                    }
                    else {
                        var masterid = data.message;
                        $("#divTEOMasterError").hide("slide");
                        $("#divTEOMasterError span:eq(1)").html('');
                      //  $("#btnReset").trigger('click');
                        $("#loadTEOMaster").html('');
                        LoadTEOMasterGrid(masterid);                      
                        alert("TEO Master Added");

                       // LoadTEODetailsGrid(billId);


                        //to load credit debit view
                        $.ajax({
                            url: "/TEO/TEODetailsForTOB/" + masterid ,
                            type: "POST",
                            async: false,
                            cache: false,
                            success: function (data) {
                                //$("#loadTEOCreditDetails").html(data);
                                $("#loadTEOCreditDebitDetails").html(data);
                                
                                $.each($("select"), function () {
                                    if ($(this).find('option').length >= 1) {
                                        $('#tr' + $(this).attr('id')).show();
                                    }
                                });

                                 
                                $.ajax({
                                    url: "/TEO/GetTEOValidationParams/" + masterid,
                                    type: "POST",
                                    async: false,
                                    cache: false,
                                    success: function (data) {
                                        vpCHead = data.CHead;
                                        vpDHead = data.DHead;
                                        vpDistRepeat = data.District;
                                        vpDPIURepeat = data.DPIU;
                                        vpContRepeat = data.Contractor;
                                        vpSupRepeat = data.Supplier;
                                        vpAggRepeat = data.Agreement;
                                        vpRoadRepeat = data.Road;
                                        vpHeadRepeat = data.Head;
                                        $.unblockUI();
 
                                    },
                                    error: function (xhr, ajaxOptions, thrownError) {
                                        alert(xhr.responseText);
                                        $.unblockUI();
                                    }
                                });

                                LoadTEODetailsGrid(masterid);

                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                alert(xhr.responseText);
                                $.unblockUI();
                            }
                        });

                        return false;
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();

                    alert("Error while processing request");
                    unblockPage();
                }
            });
        }
    });

    //method to edit the t e o
    $("#btnEditTEOMaster").click(function (evt) {
        evt.preventDefault();
        //check if details has been entered for the master entry do not allow the change of master transaction head 
        //by amol jadhav on 23072013

        if ($("#tblTEODetailsGrid").getGridParam("reccount") != 0 && $("#tblTEODetailsGrid").getGridParam("reccount") !== undefined) {
            alert("You can not update the TEO master details, because TEO details has been entered.");
            return false;
        }


        if ($('#frmTEOAddMaster').valid() && validateTEOMaster($("#GROSS_AMOUNT").val())) {
            $("#ddlTransMaster").removeAttr("disabled");
            $("#ddlSubTrans").removeAttr("disabled");
            $(".ui-datepicker-trigger").show();
            $("#BILL_DATE").removeAttr("readonly");

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/TEO/EditTEOMaster/" + $("#tblTEOMasterGrid").getDataIDs()[0],
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmTEOAddMaster").serialize(),
                success: function (data) {
                    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

                    if (!data.success) {
                        if (data.message == "undefined" || data.message == null) {
                            $("#loadTEOMaster").html(data);
                        }
                        else {
                            $("#divTEOMasterError").show("slide");
                            $("#divTEOMasterError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        }
                        $.unblockUI();
                        return false;
                    }
                    else {
                        var masterid = data.message;
                        $("#divTEOMasterError").hide("slide");
                        $("#divTEOMasterError span:eq(1)").html('');
                        $("#btnCancelTEOMaster").trigger('click');
                        alert("TEO Master updated");
                        $("#loadTEOMaster").html('');
                        LoadTEOMasterGrid($("#tblTEOMasterGrid").getDataIDs()[0]);

                        //Added by amol Jadhav 23072013
                       // blockPage();
                        $.ajax({
                            url: "/TEO/TEODetailsForTOB/" + masterid ,
                            type: "POST",
                            async: false,
                            cache: false,
                            success: function (data) {
                                $("#loadTEOCreditDebitDetails").html(data);
                                $.each($("select"), function () {
                                    if ($(this).find('option').length >= 1) {
                                        $('#tr' + $(this).attr('id')).show();
                                    }
                                });

                                $.ajax({
                                    url: "/TEO/GetTEOValidationParams/" + masterid,
                                    type: "POST",
                                    async: false,
                                    cache: false,
                                    success: function (data) {
                                        vpCHead = data.CHead;
                                        vpDHead = data.DHead;
                                        vpDistRepeat = data.District;
                                        vpDPIURepeat = data.DPIU;
                                        vpContRepeat = data.Contractor;
                                        vpSupRepeat = data.Supplier;
                                        vpAggRepeat = data.Agreement;
                                        vpRoadRepeat = data.Road;
                                        vpHeadRepeat = data.Head;
                                        $.unblockUI();

                                    },
                                    error: function (xhr, ajaxOptions, thrownError) {
                                        alert(xhr.responseText);
                                        $.unblockUI();
                                    }
                                });

                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                alert(xhr.responseText);
                                $.unblockUI();
                            }
                        });
                        $.unblockUI();
                        //added by amol jadhav
                        return false;
                    }

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("Error while processing request");
                    $.unblockUI();                   

                }
            });
        }
    });

    $("#btnCancelTEOMaster").click(function () {
        $("#loadTEOMaster").hide('slow');
        $("#loadTEOMaster").html('');
    });

}); // Document.ready ends here

function validateTEOMaster(amountToValidate) {
    $("#divTEOMasterError").hide("slide");
    $("#divTEOMasterError span:eq(1)").html('');

    var detailsAmountC = 0;
    var detailsAmountD = 0;
    if (isDetailsGridLoaded) {
        detailsAmountC = $("#tblTEODetailsGrid").jqGrid('getCol', 'CAmount', false, "sum");
        detailsAmountD = $("#tblTEODetailsGrid").jqGrid('getCol', 'DAmount', false, "sum");
    }

    if (parseFloat(detailsAmountC).toFixed(2) == 0 && parseFloat(detailsAmountD).toFixed(2) == 0) {
        $("#divTEOMasterError").hide("slide");
        $("#divTEOMasterError span:eq(1)").html('');
        return true;
    }
    else if (parseFloat(detailsAmountC) > parseFloat(amountToValidate)) {
        $("#divTEOMasterError").show("slide");
        $("#divTEOMasterError span:eq(1)").html('<strong>Alert: </strong> Amount must not be less than Rs.' + detailsAmountC);
        return false;
    }
    else if (parseFloat(detailsAmountD) > parseFloat(amountToValidate)) {
        $("#divTEOMasterError").show("slide");
        $("#divTEOMasterError span:eq(1)").html('<strong>Alert: </strong> Amount must not be less than Rs.' + detailsAmountD);
        return false;
    }
    else {
        $("#divTEOMasterError").hide("slide");
        $("#divTEOMasterError span:eq(1)").html('');
        return true;
    }
}

function FillInCascadeDropdown(map, dropdown, action) {

    $(dropdown).empty()

    $.post(action, map, function (data) {
        ddvalues = data;
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
        if (dropdown == "#ddlSubTrans") {
            if ($("#ddlSubTrans").find('option').length > 1) {
                $("#trddlSubTrans").show();
            }
            else {
                $("#trddlSubTrans").hide();
            }
        }
    }, "json");
}

function ModifiedDate(day, month, year) {
    return day + "/" + month + "/" + year;
}
function process(date) {
    var parts = date.split(' ')[0].split("/");
    return new Date(parts[2], parts[1] - 1, parts[0]);
}
