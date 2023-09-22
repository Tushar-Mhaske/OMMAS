$(function () {
    $.validator.unobtrusive.adapters.add('isstartleafgreater', ['propertytested'], function (options) {
        options.rules['isstartleafgreater'] = options.params;
        options.messages['isstartleafgreater'] = options.message;
    });

    $.validator.addMethod("isstartleafgreater", function (value, element, params) {
        var startdatevalue = $('#' + params.propertytested).val();
        return startdatevalue < value;
    });

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

    $.validator.unobtrusive.adapters.add('isbankdetails', ['bankcode'], function (options) {
        options.rules['isbankdetails'] = options.params;
        options.messages['isbankdetails'] = options.message;
    });

    $.validator.addMethod("isbankdetails", function (value, element, params) {
       // alert("test : " + params.bankcode);

        if (params.bankcode == 0) {
            return false;
        }
        else {
            return true;
        }
    });

});

function process(date) {
    var parts = date.split("/");
    return new Date(parts[2], parts[1] - 1, parts[0]);
}

//new change done by Vikram on 12-09-2013

jQuery.validator.addMethod("isIssueDateAfterAccOpenDate", function (value, element) {

    if (process($("#ACC_OPEN_DATE").val()) <= process($("#ISSUE_DATE").val())) {
        return true;
    }
    else {
        return false;
    }

}, "Cheque issue date must be greater than bank account opening date.");


//end of change



$(document).ready(function () {    
    $("#ISSUE_DATE").addClass("pmgsy-textbox");
    $.validator.unobtrusive.parse($('#frmChequeBookDetails'));
    $("#ISSUE_DATE").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonImageOnly: true
    });


    $(function () {
        $("#ddlDPIU").trigger("change");
        
        $("#ddlDPIU").change(function () {
            $("#spnDPIUName").html("");
            $("#spnDPIUName").html($("#ddlDPIU option:selected").text());

            LoadGrid(0, 0, 0, $("#ddlDPIU").val(), $("#ddlDPIU option:selected").text());

        });
    });
    
    $("#rdoSRRDA").click(function () {
        $(".tdDPIU").hide();
        LoadGrid(0, 0, 0, 0, "SRRDA");
    });
    $("#rdoDPIU").click(function () {
        $(".tdDPIU").show();
        LoadGrid(0, 0, 0, $("#ddlDPIU").val(), $("#ddlDPIU option:selected").text());
    });

    $("#btnSave").click(function (evt) {

        evt.preventDefault();
        if ($(this).val() == "Save") {

            $('#ISSUE_DATE').rules('add', {
                isIssueDateAfterAccOpenDate: true,
                messages: {
                    isIssueDateAfterAccOpenDate: 'Issue date must be greater than Bank account opening date.',
                }
            });

            if ($('#frmChequeBookDetails').valid()) {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


                $.ajax({
                    url: "/ChequeBook/Create/",
                    type: "POST",
                    //dataType: "json",
                    async: false,
                    cache: false,
                    data: $("#frmChequeBookDetails").serialize(),
                    success: function (data) {
                        $.unblockUI();

                        if (!data.success) {
                            if (data.message == "undefined" || data.message == null) {
                                $("#divError").hide("slide");
                                $("#divError span:eq(1)").html('');
                                $("#loadPage").html(data);
                            }
                            else {
                                $("#divError").show("slide");
                                $("#divError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                            }
                            return false;
                        }
                        else {
                            //$("#btnReset").trigger('click');
                            ClearForm();
                            //$("#tblChequeBookList").trigger("reloadGrid");
                            if ($("#rdoSRRDA").is(":checked")) {
                                LoadGrid(0, 0, 0, 0, "SRRDA");
                            } else {
                                LoadGrid(0, 0, 0, $("#ddlDPIU").val(), $("#ddlDPIU option:selected").text());
                            }
                            alert("Cheque Book details added.");
                            return false;
                        }
                        //$("#loadPage").html(data);

                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.responseText);
                        $.unblockUI();

                    }
                });
                
            }

        }
        else {

            $('#ISSUE_DATE').rules('add', {
                isIssueDateAfterAccOpenDate: true,
                messages: {
                    isIssueDateAfterAccOpenDate: 'Issue date must be greater than Bank account opening date.',
                }
            });

            if ($('#frmChequeBookDetails').valid()) {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

                $("#ddlDPIU").attr("disabled",false);

                $.ajax({
                    url: "/ChequeBook/Edit/",
                    type: "POST",
                    //dataType: "json",
                    async: false,
                    cache: false,
                    data: $("#frmChequeBookDetails").serialize(),
                    success: function (data) {
                        $("#ddlDPIU").attr("disabled", true);
                        $.unblockUI();

                        if (!data.success) {
                            if (data.message == "undefined" || data.message == null) {
                                $("#loadPage").html(data);
                            }
                            else {
                                $("#divError").show("slide");
                                $("#divError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                            }

                            return false;
                        }
                        else {
                            $("#tblChequeBookList").trigger("reloadGrid");
                            alert("Cheque Book details updated.");
                            $("#btnCancel").trigger('click');
                            return false;
                        }
                        //$("#loadPage").html(data);

                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        $("#ddlDPIU").attr("disabled", true);
                        alert(xhr.responseText);
                        $.unblockUI();
                    }
                });
            }
        }
    });

    $("#btnCancel").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        //$("#loadPage").load('/ChequeBook/AddEditChequeBook');
        $("#AddChequeBook").show("slow");
        $("#loadPage").hide();
        $("#divError span:eq(1)").html("");
        $("#divError").hide("slide");

        $("#ISSUE_DATE").val("");
        $("#LEAF_START").val("");
        $("#LEAF_END").val("");
        $.unblockUI();

    });

    $("#btnReset").click(function () {
        $("#divError span:eq(1)").html("");
        $("#divError").hide("slide");

    });

    $("#divIcoAddChequeBook").click(function () {
        if ($(this).hasClass('ui-icon-circle-triangle-n')) {
            $("#tblReceiptMaster").hide('slide');
            $(this).removeClass('ui-icon-circle-triangle-n');
            $(this).addClass('ui-icon-circle-triangle-s');
        }
        else {
            $("#tblReceiptMaster").show('slide');
            $(this).removeClass('ui-icon-circle-triangle-s');
            $(this).addClass('ui-icon-circle-triangle-n');
        }
    });

    //new change done by Vikram on 12-09-2013

    $("#ISSUE_DATE").blur(function () {


    });

    //

});


function ClearForm()
{
    $("#ISSUE_DATE").val('');
    $("#LEAF_START").val('');
    $("#LEAF_END").val('');
}