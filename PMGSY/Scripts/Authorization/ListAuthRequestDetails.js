var isGridLoaded = false;
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
});

function process(date) {
    var parts = date.split("/");
    return new Date(parts[2], parts[1] - 1, parts[0]);
}
$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmAuthRequestList'));

    if ($("#DATE_OF_OPERATION").val() == "") {
        $("#DATE_OF_OPERATION").datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            showOn: 'button',
            buttonImage: '/Content/images/calendar_2.png',
            buttonImageOnly: true
        }).datepicker('setDate', new Date());
    }
    else {
        $("#DATE_OF_OPERATION").datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            showOn: 'button',
            buttonImage: '/Content/images/calendar_2.png',
            buttonImageOnly: true
        });
    }

    $("#ADMIN_ND_CODE").change(function () {
        if ($("#ADMIN_ND_CODE").val() != "0") {
            $("#ADMIN_ND_CODE").removeClass("input-validation-error").addClass("input-validation-valid");
            $("#tdDpiu").find('span:eq(0)').text("").hide().removeClass("field-validation-error").addClass("field-validation-valid");
        }
        else {
            $("#ADMIN_ND_CODE").removeClass("input-validation-valid").addClass("input-validation-error");
            $("#tdDpiu").find('span:eq(0)').text("Please select DPIU").show().removeClass("field-validation-valid").addClass("field-validation-error");
        }
    });

    $("#btnView").click(function () {
        
        //Modified by Abhishek kamble 26-oct-2013
        if ($('#frmAuthRequestList').valid()) {
            LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), $("#ADMIN_ND_CODE").val());            
        } 

        //old code 
        //if ($("#ADMIN_ND_CODE").val() == "0") {
        //    $("#ADMIN_ND_CODE").removeClass("input-validation-valid").addClass("input-validation-error");
        //    $("#tdDpiu").find('span:eq(0)').text("Please select DPIU").show().removeClass("field-validation-valid").addClass("field-validation-error");            
        //}
        //else {
        //    $("#ADMIN_ND_CODE").removeClass("input-validation-error").addClass("input-validation-valid");
        //    $("#tdDpiu").find('span:eq(0)').text("").hide().removeClass("field-validation-error").addClass("field-validation-valid");
            
        //    LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), $("#ADMIN_ND_CODE").val());
        //}
    });

    $("#divAuthRequestConfirm").dialog({
        resizable: false,
        closeOnEscape: true,
        height: 'auto',
        width: 380,
        modal: true,
        autoOpen: false,
        open: function () {
            $(this).parent().appendTo($('#frmAuthRequestList'));
        }
        //buttons: [{
        //    text: "Confirm",
        //    click: function () {
        //        //$("#test").dialog("close");
        //        alert('Confirm');
        //    },
        //},
        //{
        //    text: "Cancel",
        //    click: function () {
        //        alert('Cancel');
        //        $("#test").dialog("close");
        //    },
        //}]
    });

    $("#btnConfirm").click(function (evt) {
        evt.preventDefault();
        $.validator.unobtrusive.parse($('#frmAuthRequestList'));
        if ($('#frmAuthRequestList').valid()) {

            if (confirm("All Authorization Requests will be processed on Confirm. Are you sure to proceed?")) {
                $.ajax({
                    url: "/Authorization/AddRequestTrackingDetails",
                    type: "POST",
                    async: false,
                    cache: false,
                    data: $("#frmAuthRequestList").serialize(),
                    success: function (data) {

                        if (!data.success) {
                            if (data.message == "undefined" || data.message == null) {
                                $("#ListAuthRequestDetails").html(data);
                            }
                            else {
                                $("#divAuthRequestError").show("slide");
                                $("#divAuthRequestError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                            }
                        }
                        else {
                            $("#divAuthRequestError").hide("slide");
                            $("#divAuthRequestError span:eq(1)").html('');
                            $("#divAuthRequestConfirm").dialog('close');
                            LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), $("#ADMIN_ND_CODE").val());
                            if (data.message == "A") {
                                alert("Authorization request approved");
                            }
                            else {
                                alert("Authorization request rejected");
                            }
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.responseText);
                    }
                });
            }
            else
            {
                $("#DATE_OF_OPERATION").val();
                $("#REMARKS").val("");
                $("#divAuthRequestConfirm").dialog('close');
            }
        }
    });

    $("#btnCancel").click(function () {
        $("#DATE_OF_OPERATION").val();
        $("#REMARKS").val("");
        $("#divAuthRequestConfirm").dialog('close');
    });

}); //Document.ready ends here

//function arrtSetting(rowId, val, rawObject, cm) {
//    var result;
//    alert(rawObject[cm.name]);
//    if (rawObject[0].toLowerCase() == "assets") {
//        result = ' rowspan=2';
//    } else if (rawObject[0].toLowerCase() == "liabilities") {
//        result = ' style="display: none';
//    }
//    return result;
//};

function LoadGrid(month, year, districtCode) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    //alert('a');

    if (isGridLoaded) {
        $("#tblAuthRequestGrid").GridUnload();
        isGridLoaded = false;
    }

    jQuery("#tblAuthRequestGrid").jqGrid({
        url: '/Authorization/ListAuthorizationRequest',
        datatype: "json",
        mtype: "POST",
        colNames: [/*'AuthId', */'Authorization Number', 'Authorization Date', 'Transaction Type ', 'Aggrement Number', 'Package', 'Name of work', 'Payee Name', 'Bank Name', 'Account Number', 'Sanctioned Amount </br>(in Lacs)', 'Expenditure Amount</br>(in Lacs)', 'Amount Payable'],
        colModel: [
                          //  { name: 'AuthId', index: 'AuthId', hidden:true },
                            { name: 'AuthtNumber', index: 'AuthtNumber', width: 80, align: 'center', sortable: true },
                            { name: 'AuthDate', index: 'AuthDate', width: 75, align: 'center', sortable: true },
                            { name: 'TransType', index: 'TransType', width: 120, align: 'center', sortable: false },
                            { name: 'AggNumber', index: 'AggNumber', width: 150, align: 'left', sortable: true },
                            { name: 'Package', index: 'Package', width: 80, align: 'center', sortable: true },
                            { name: 'RoadName', index: 'RoadName', width: 120, align: 'center', sortable: true, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } },
                            { name: 'PayeeName', index: 'PayeeName', width: 100, align: 'center', sortable: true },
                            { name: 'BankName', index: 'BankName', width: 100, align: 'center', sortable: true, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } },
                            { name: 'AccountNo', index: 'AccountNo', width: 100, align: 'center', sortable: true },
                            { name: 'Sanctioned', index: 'Sanctioned', width: 80, align: 'right', sortable: false },
                            { name: 'Expenditure', index: 'Expenditure', width: 80, align: 'right', sortable: false },
                            { name: 'Payable', index: 'Payable', width: 80, align: 'right', sortable: false }
        ],
        pager: jQuery('#divAuthRequestPager'),
        rowNum: 1000,
        pginput: false,
        pgbuttons:false,
        postData: {
            'month': month,
            'year': year,
            'dpiu': districtCode
        },
       // altRows: true,
       // rowList: [10, 20, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'ReceiptDate',
        sortorder: "desc",
        multiselect: true,
        toppager: true,
        caption: "Authorization Request List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        loadComplete: function () {
            $.unblockUI();

            $("#tblAuthRequestGrid_rn").find('div:eq(0)').append("Sr No");
            var topPagerDiv = $("#pg_tblAuthRequestGrid_toppager")[0];
            $("#tblAuthRequestGrid_toppager_center", topPagerDiv).remove();
            $("#tblAuthRequestGrid_toppager_right", topPagerDiv).remove();
            isGridLoaded = true;
        },
        grouping: true,
        groupingView:
            {
                groupField: ['AuthtNumber'],
                groupColumnShow: [true],
                groupText: ['<b>{0}</b>'],
                //groupText: ['<span class="groupText"></span>' +
                //'<span class="group-span">' + 
                //'<input type="checkbox" class="grouping" id ="{0}" onClick="SelectGroup({0})">' + 
                //'<label class="grouping-label"> {0}</label>' +
                //'</span>'],  

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
        },

    }).navGrid('#divAuthRequestPager', { edit: false, add: false, del: false, search: false, refresh: false })
        .navButtonAdd('#divAuthRequestPager', {
            caption: "Approve",
            title:"Click here to Approve Authorization Request",
            buttonicon: "ui-icon ui-icon-circle-check",
            onClickButton: function () {
                var requestIds = $("#tblAuthRequestGrid").jqGrid('getGridParam', 'selarrrow');
                if (requestIds == "") {
                    $("#tblAuthRequestGrid_toppager_left").html("<span style='float:left' class='ui-icon ui-icon-alert'></span><font color='#b83400'><b>Please Select atleast one Request</b></font>")
                }
                else {
                    if ($("#cb_tblAuthRequestGrid").prop('checked') == true) {
                        requestIds = "A_Y_" + requestIds;
                    }
                    else {
                        requestIds = "A_N_" + requestIds;
                    }

                    $("#tblAuthRequestGrid_toppager_left").html("");
                    
                    $("#REQUEST_ID_LIST").val(requestIds);

                    $("#divAuthRequestConfirm").show();
                    // This code is used to display Button text in UI Dialog
                    //$('.ui-button-text').each(function (i) {
                    //    $(this).html($(this).parent().attr('text'))
                    //})
                    $("#divAuthRequestConfirm").dialog('open');

                    $('#ui-id-4').html('Authorization Request Approval Details.');
                }
            },
            position: "first"
        }).navButtonAdd('#divAuthRequestPager', {
            caption: "Reject",
            title: "Click here to Reject Authorization Request",
            buttonicon: "ui-icon ui-icon-circle-close",
            onClickButton: function () {
                var requestIds = $("#tblAuthRequestGrid").jqGrid('getGridParam', 'selarrrow');
                if (requestIds == "") {
                    $("#tblAuthRequestGrid_toppager_left").html("<span style='float:left' class='ui-icon ui-icon-alert'></span><font color='#b83400'><b>Please Select atleast one Request</b></font>")
                }
                else {
                    if ($("#cb_tblAuthRequestGrid").prop('checked') == true) {
                        requestIds = "C_Y_" + requestIds;
                    }
                    else {
                        requestIds = "C_N_" + requestIds;
                    }

                    $("#tblAuthRequestGrid_toppager_left").html("");

                    $("#REQUEST_ID_LIST").val(requestIds);
                    $("#divAuthRequestConfirm").show();
                    $("#divAuthRequestConfirm").dialog('open');

                    $('#ui-id-4').html('Authorization Request Rejection Details.');

                }
            },
            position: "Second"
        });

 //   $.unblockUI();

    //end of documents grid
}

