var ackVoucherFinalized = "N";
var finaliseFalg = "N"
var month;
var year;
var adminNdCode;
var btnClick;
jQuery.validator.addMethod("DateInMonthYear", function (value, element) {


    //alert(value);

    if (value == "") {
        return false;
    }

    var dateParam = value.split('/');
    var month = dateParam[1];
    var year = parseInt(dateParam[2])

    if (parseInt($("#BILL_MONTH_VOUCHER").val()) != month) {
        return false;

    }
    else if (parseInt($("#BILL_YEAR_VOUCHER").val()) != year) {
        return false;

    }
    else {
        return true;
    }


}, "");


jQuery.validator.addMethod("LastDayOfMonth", function (value, element) {

    if (value == "") {
        return false;
    }
    var dateParam = value.split('/');

    var day = parseInt(dateParam[1]);


    var lastDay = new Date($("#BILL_YEAR_VOUCHER").val(), $("#BILL_MONTH_VOUCHER").val(), 0);


    if (lastDay.getDate() == day) {
        return true;
    }
    else {
        return true;
    }


}, "");

jQuery.validator.addMethod("duplicatevoucher", function (value, element) {


    if ($("#BILL_YEAR_VOUCHER").val() && $("#BILL_MONTH_VOUCHER").val() && $("#ackVoucherFinalized").val())
        return false;
    else
        return true;

}, "");

jQuery.validator.unobtrusive.adapters.addBool("duplicatevoucher");


$(document).ready(function () {
    $.validator.unobtrusive.parse($("#FrmSelectionOptions"));


    $(document).unbind('keydown').bind('keydown', function (event) {
        var doPrevent = false;
        if (event.keyCode === 8) {
            var d = event.srcElement || event.target;
            if ((d.tagName.toUpperCase() === 'INPUT' && (d.type.toUpperCase() === 'TEXT' || d.type.toUpperCase() === 'PASSWORD'))
                 || d.tagName.toUpperCase() === 'TEXTAREA') {
                doPrevent = d.readOnly || d.disabled;
            }
            else {
                doPrevent = true;
            }
        }

        if (doPrevent) {
            event.preventDefault();
        }
    })




    //event to button view click
    $("#btnViewDetails").click(function () {


        month = $("#BILL_MONTH option:selected").val();
        year = $('#BILL_YEAR option:selected').val();
        adminNdCode = $("#DPIU option:selected").val();

        //Added By Abhishek kamble 16-July-2014 to check PIU selected or not
        if ($("#DPIU option:selected").val() == 0) {
            alert("Please select DPIU");
            return false;
        }


        if ($("#FrmSelectionOptions").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            //get voucher form(added newly) Added by Ashish Markande on 21/08/2013

            $.ajax({
                url: "/ChequeAcknowledgement/CheckMonthClose",
                type: "POST",
                data: { BILL_MONTH: $("#BILL_MONTH option:selected").val(), BILL_YEAR: $('#BILL_YEAR option:selected').val(), DPIU: $("#DPIU option:selected").val() },
                success: function (data) {
                    $.unblockUI();

                    if (data.success == true) {
                        blockPage();
                        $("#ackVoucherDiv").load("/ChequeAcknowledgement/GetVoucherAckForm/", function () {


                            $('#divError').hide('slow');
                            $('#errorSpan').html("");
                            $('#errorSpan').hide();

                            $("#DPIU_CODE").val($("#DPIU").val());

                            $("#tblForm").show('slow');

                            $("#gridDiv").show();

                            $("#tblButtons").show('slow');

                            $('#ChequeList').jqGrid('GridUnload');

                            loadGrid($("#BILL_MONTH").val(), $("#BILL_YEAR").val(), $("#DPIU").val());

                            $("#BILL_MONTH_VOUCHER").val($("#BILL_MONTH").val());

                            $("#BILL_YEAR_VOUCHER").val($("#BILL_YEAR").val());

                            if ($("#BILL_DATE").val() == "") {
                                var lastDay = new Date($("#BILL_YEAR_VOUCHER").val(), $("#BILL_MONTH_VOUCHER").val(), 0);

                                var month = $("#BILL_MONTH_VOUCHER").val().length == 2 ? $("#BILL_MONTH_VOUCHER").val() : "0" + $("#BILL_MONTH_VOUCHER").val();

                                var lastDateOfMonth = lastDay.getDate() + "/" + month + "/" + $("#BILL_YEAR_VOUCHER").val();

                                $("#BILL_DATE").val(lastDateOfMonth);

                            }


                            unblockPage();
                        });
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            alert(data.message);
                        }
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            });


        }


    });

    $("#ChequeList").parents('div.ui-jqgrid-bdiv').css("max-height", "500Px");



    //event for submit button event
    $("#btnSubmit").click(function () {

        btnClick = "Submit";
        if ($("#FrmSelectionOptions").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $('#BILL_DATE').rules('add', {
                DateInMonthYear: true,
                LastDayOfMonth: true,
                messages:
                    {
                        DateInMonthYear: 'Voucher Date must be in selected month and year',
                        LastDayOfMonth: 'Voucher Date must be in last day of the month'
                    }
            });

            if ($("#chqAcknowledgement").valid()) {
                $.unblockUI();


                if (ackVoucherFinalized == "N") {
                    $("#Finalize").val(false);

                    SubmitChequeAckDetails(btnClick);
                }
                else {
                    alert("Cheque Acknowledgement voucher is already finalized. Please unlock the voucher first.");
                    return false;
                }
            }
            $.unblockUI();

        }
    });

    //event for finalize button event
    $("#btnSubmitFinalize").click(function () {

        btnClick = "SubmitNFinalise";

        if ($("#FrmSelectionOptions").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $('#BILL_DATE').rules('add', {
                DateInMonthYear: true,
                LastDayOfMonth: true,
                messages:
                    {
                        DateInMonthYear: 'Voucher Date must be in selected month and year',
                        LastDayOfMonth: 'Voucher Date must be in last day of the month'
                    }
            });

            if ($("#chqAcknowledgement").valid()) {
                $.unblockUI();

                if (ackVoucherFinalized == "N") {
                    $("#Finalize").val(true);
                    finaliseFalg = "Y";
                    SubmitChequeAckDetails(btnClick);
                }
                else {
                    alert("Cheque Acknowledgement voucher is already finalized. Please unlock the voucher first. ");
                    return false;
                }
            }
            $.unblockUI();
        }
    });

    //Changed by ashish markande on 23/10/2013
    $("#btnViewVouchers").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        var dpiu = $("#DPIU option:selected").val();
        var month = $("#BILL_MONTH option:selected").val();
        var year = $("#BILL_YEAR option:selected").val();
        $("#mainDiv").load('/ChequeAcknowledgement/ViewVoucherDetails', function () {
            $.unblockUI();

            $("#DPIU").val(dpiu);
            $("#BILL_MONTH").val(month);
            $("#BILL_YEAR").val(year);
            // $("#BILL_MONTH").val(0);
            $("#btnView").trigger('click');
        });

    });

    //Added By Ashish Markande on 10/10/2013
    $("#btnUnauth").click(function () {

        btnClick = "Unacknowledge"

        if ($("#FrmSelectionOptions").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $('#BILL_DATE').rules('add', {
                DateInMonthYear: true,
                LastDayOfMonth: true,
                messages:
                    {
                        DateInMonthYear: 'Voucher Date must be in selected month and year',
                        LastDayOfMonth: 'Voucher Date must be in last day of the month'
                    }
            });

            if ($("#chqAcknowledgement").valid()) {
                $.unblockUI();


                if (ackVoucherFinalized == "N") {
                    $("#Finalize").val(false);

                    SubmitChequeAckDetails(btnClick);
                }
                else {
                    alert("Cheque Acknowledgement voucher is already finalized. Please unlock the voucher first.");
                    return false;
                }
            }
            $.unblockUI();

        }


    });//End

});//dom ready




function SubmitChequeAckDetails(btnClick) {

   // alert(btnClick);
    var encryptedRowId = [];
    //get all selected rows
    var selRowIds = jQuery('#ChequeList').jqGrid('getGridParam', 'selarrrow');

 
    //get encrypted id
    for (var i = 0; i < selRowIds.length; i++) {

        rowdata = jQuery("#ChequeList").getRowData(selRowIds[i]);
        if (rowdata === undefined || rowdata == null) {
            alert("invalid Cheque Data");
            return false;

        }
        encryptedRowId.push(rowdata["BillId"]);
    }

    //$("#BILL_DATE").datepicker("enable").removeAttr("readonly");

    if (btnClick == 'Submit') {
        if (selRowIds.length == 0) {

            if ($("#AckUnackFlag").val() == "A") {
                alert("Please select the row for Acknowledgement");
            }
            else {
                alert("Please select the row for UnAcknowledgement");
            }
            return false;
        }
    }

    //validation Added by Abhishek kamble to check row is selected or not 26-Aug-2014 start
    if ((($("#AckBillIDArray").val() != "") && ($("#AckBillIDArray").val() != null)) && (!($("#AckBillIDArray").val() === undefined))) {
        if (((selRowIds.length) == ($("#AckBillIDArray").val().split(',').length)) && (btnClick == "Submit")) {
            alert("Please select the row for Acknowledgement");
            return false;
        }
    }
    //validation Added by Abhishek kamble to check row is selected or not 26-Aug-2014 end

    //save selected encrypted id into array
    $("#SelectedIDArray").val(encryptedRowId);
    //alert(encryptedRowId);
    //alert($("#SelectedIDArray").val());

    $.ajax({
        type: "POST",
        url: "/ChequeAcknowledgement/SubmitChequesForAcknowledgement?id=" + btnClick,
        // async: false,
        error: function (xhr, status, error) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        cache: false,
        data: $("#chqAcknowledgement").serialize(),
        success: function (data) {
            unblockPage();
            $('#divError').hide('slow');
            $('#errorSpan').html("");
            $('#errorSpan').hide();

            if (data.Success === undefined) {

                $("#ackVoucherDiv").html(data);
                $("#gridDiv").show();
                $("#tblForm").show('slow');
                $("#tblButtons").show('slow');

                return false;

            }
            if (data.Success) {
                if (data.statusCode == "111") {
                    alert("Selected Cheques has been unacknowledged Successfully.");
                }
                else {
                    if (btnClick == "SubmitNFinalise") {
                        alert("Selected Cheques has been Finalized Successfully.");
                    } else if (btnClick == "Submit") {
                        alert("Selected Cheques has been Acknowledged Successfully.");
                    }
                }
                $('#ChequeList').jqGrid('GridUnload');

                //Added By Ashish Markande                        
                if (finaliseFalg == "Y") {
                    $("#tblForm").hide();

                    $("#tblButtons").hide();//Added
                    // $("#divVoucherView").html("");
                    //$("#divVoucherView").load('/ChequeAcknowledgement/ViewVoucherDetails');
                    //$(function () {
                    //    $("#DPIU").val(919);
                    //    $("#BILL_MONTH").val(2);
                    //    $("#BILL_YEAR").val(2013);                              

                    //    $("#btnView").trigger("click");
                    //});
                    $("#btnViewVouchers").trigger("click");
                }
                else {
                    $("#tblForm").show();
                    loadGrid($("#BILL_MONTH").val(), $("#BILL_YEAR").val(), $("#DPIU").val());
                }

                return false;
            }
            else if (!data.Success) {

                if (data.statusCode == "-111") {
                    alert("Cheque Acknowledgement voucher is already finalized. Please unlock the voucher first. ");
                    return false;

                } else if (data.statusCode == "-222") {
                    alert("Selected month is not closed.Please close it first. ");
                    return false;

                }
                else if (data.statusCode == "-333") {
                    alert("Voucher date is not equal to last date of the month");
                    return false;

                }
                else if (data.statusCode == "-444") {
                    alert("Voucher is already unacknowledge");
                    return false;
                }
                else if (data.statusCode == "-555") {
                    alert("Please acknowledge previous month cheque first.");
                    return false;
                }//Added By Abhishek kamble 23-Aug-2014
                else if (data.statusCode == "-999") {
                    alert("An error occured while processing your request.");
                    return false;
                }
                    //Added By Abhishek kamble 17-June-2015
                else if (data.statusCode == "-777") {
                    alert("An error occured while processing your request. Invalid Session.");
                    window.location.href = "/Home/Index";
                    return false;
                }
                //else if (data.voucher != null) {
                //    $("#errorSpan").html("Voucher No:" + data.voucher + " is not finalize yet,Acknowledge cheque against this voucher only");
                //    $("#divError").show('slow');
                //    $("#errorSpan").show('slow');
                //    return false;
                //}

            }
            else {
                alert("error occured while Cheques  acknowledgement.");
                return false;
            }
        }
    });
}


var total = 0;
var allChequesSelected = false;
//function to load the grid
function loadGrid(month, year, dpiu) {
    //alert("test:: " + $("#AckUnackFlag").val());
    var totalAmt;
    totalAmt = 0;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $grid = $("#ChequeList"), idsOfSelectedRows = [];


    $grid.jqGrid({

        url: '/ChequeAcknowledgement/ListChequeForAcknowledgment/' + month + "$" + year + "$" + dpiu,
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: -1,
        postData: { 'months': $('#BILL_MONTH').val(), 'year': $('#BILL_YEAR').val(), 'DPIU': $('#DPIU').val(), 'AckUnackFlag': $("#AckUnackFlag").val() },
        rownumbers: true,
        autowidth: true,
        pginput: false,
        multiselect: true,
        pgbuttons: false,
        colNames: ['Cheque / Advice Issued Date', 'Cheque / Advice Date', 'Transaction Type', 'Cheque / Advice Number', 'Payee Name', 'Cheque Amount (In Rs.)', 'Acknowledge', "", ""],
        colModel: [
                        {
                            name: 'Cheque_Issued_Date',
                            index: 'Auth_Number',
                            width: 50,
                            align: "center",
                        },
                        {
                            name: 'Chequ_Date',
                            index: 'Chequ_Date',
                            width: 90,
                            align: "center",
                            hidden: true
                        },
                         {
                             name: 'Trans_Type',
                             index: 'Trans_Type',
                             width: 120,
                             align: "left"
                         },
                        {
                            name: 'ChequeNumber',
                            index: 'ChequeNumber',
                            width: 80,
                            align: "left"
                        },
                        {
                            name: 'Payee_name',
                            index: 'Payee_name',
                            width: 80,
                            align: "left"

                        },
                        {
                            name: 'ChequeAmount',
                            index: 'ChequeAmount',
                            width: 80,
                            align: "right"
                        },
                        {
                            name: 'Acknowledment',
                            index: 'Acknowledment',
                            width: 80,
                            align: "center",
                            editable: true,
                            edittype: 'checkbox',
                            editoptions: { value: "True:False" },
                            formatter: "checkbox",
                            formatoptions: { disabled: false },
                            hidden: true
                        },
                        {
                            name: 'AckCode',
                            index: 'AckCode',
                            width: 0,
                            align: "right",
                            hidden: true
                        },
                         {
                             name: 'BillId',
                             index: 'BillId',
                             width: 0,
                             align: "right",
                             hidden: true
                         }
        ],
        pager: "#Chequepager",
        viewrecords: true,
        footerrow: true,
        userDataOnFooter: true,
        loadError: function (xhr, st, err) {
            unblockPage();
            $.unblockUI();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        onSelectAll: function (aRowids, status) {
            var amtTotal = 0;
          
            if (status) {
                for (var i = 0; i < aRowids.length; i++) {
                    var rowData = $("#ChequeList").getRowData(aRowids[i]);
                   
                    amtTotal = parseFloat(amtTotal) + parseFloat(rowData.ChequeAmount);
                    //$("#jqg_ChequeList_" + aRowids[i]).attr("checked", true);
                    
                    //if (aRowids.length == $("#ChequeList").jqGrid('getGridParam', 'records')) {
                    //    $('.cbox').attr('checked', true);
                    //    jQuery('.cbox').attr("disabled", true);

                    //}
                }
            }
            else {

                var summaryRow = $("#ChequeList").footerData('get', { name: "ChequeAmount" }, false);
                //amtTotal = parseFloat(summaryRow.ChequeAmount)
                for (var i = 0; i < aRowids.length; i++) {

                    var rowData = $("#ChequeList").getRowData(aRowids[i]);
                   // amtTotal = parseFloat(amtTotal) - parseFloat(rowData.ChequeAmount);

                    if ($("#AckUnackFlag").val() == "A") {
                        if ($("#jqg_ChequeList_" + aRowids[i]).attr("disabled")) {
                            //alert($("#jqg_ChequeList_" + aRowids[i]).attr("disabled"));
                            amtTotal = parseFloat(amtTotal) + parseFloat(rowData.ChequeAmount);
                            $("#jqg_ChequeList_" + aRowids[i]).attr("checked", true);
                            
                        }
                        total = amtTotal;
                    }

                    if ($("#AckUnackFlag").val() == "U")
                    {

                        amtTotal = rowData.ChequeAmount -(parseFloat(amtTotal) + parseFloat(rowData.ChequeAmount));
                        total = amtTotal;
                    }
                }
            }
           // alert('total amount ' + amtTotal);
            
            $("#ChequeList").footerData('set', { "ChequeAmount": amtTotal.toFixed(2) }, true);  //set total amount in footer
        },
        onSelectRow: function (id, isSelected) {
         
            var rowData = $("#ChequeList").getRowData(id);

            
            if (isSelected) {
                var selectedRowIds = jQuery('#ChequeList').jqGrid('getGridParam', 'selarrrow');
                if (selectedRowIds.length == $("#ChequeList").jqGrid('getGridParam', 'records')) {
                    $('.cbox').attr('checked', true);
                }

                if (rowData.ChequeAmount != undefined) {
                    total = parseFloat(total) + parseFloat(rowData.ChequeAmount);
                }
               
            }
            else {
                var summaryRow = $("#ChequeList").footerData('get', { name: "ChequeAmount" }, false);
                total = parseFloat(summaryRow.ChequeAmount)
                total = parseFloat(total) - parseFloat(rowData.ChequeAmount);
                
            }
          
            //set footer data
            $("#ChequeList").footerData('set', { "ChequeAmount": total.toFixed(2) }, true);

        },
        loadComplete: function (xhr, st, err) {
            $.unblockUI();

            var total = 0.00;
            //get the ack code 
            var col_NA_BILL_ID = $('#ChequeList').jqGrid('getCol', 'AckCode', false);

            if (col_NA_BILL_ID.length > 0) {

                for (var i = 0; i < col_NA_BILL_ID.length; i++) {

                    if (col_NA_BILL_ID[i] != "") {

                        $("#STR_NA_BILL_ID").val(col_NA_BILL_ID[i]);
                        break;

                    }
                }
            }

            if ($("#STR_NA_BILL_ID").val() != "") {
                $("#BILL_NO").attr('readonly', true);
                GetAcknowledgementDetails($("#STR_NA_BILL_ID").val());
                $("#BILL_NO").attr('readonly', true);
            }
            else {//else added BY Abhishek kamble to set BILL ID 16-July-2014

                $.unblockUI();
                //Voucher  not present
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                $.ajax({
                    type: "POST",
                    url: "/payment/GenerateVoucherNo/V$" + $("#BILL_MONTH").val() + '$' + $("#BILL_YEAR").val(),
                    async: false,

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


                var userdata = jQuery("#ChequeList").getGridParam('userData');
                $("#STR_NA_BILL_ID").val($("#hdBillID").val());
                if ($("#hdBillID").val() != "") {
                    GetAcknowledgementDetails($("#hdBillID").val());
                } else if (userdata.billID != "") {
                    $("#STR_NA_BILL_ID").val(userdata.billID);
                    GetAcknowledgementDetails(userdata.billID);
                }
                $("#hdBillID").val("");
            }

            $("#ChequeList").jqGrid('setLabel', "rn", "Sr.</br> No");

            //set selected rows
            var userdata = jQuery("#ChequeList").getGridParam('userData');

            idsOfSelectedRows = userdata.ids;

            //Added 'AckBillIDArray' By Abhishek kamble 23-Aug-2014 to get Acknowledged Bill Id's
            $("#AckBillIDArray").val(idsOfSelectedRows);

           

            for (var i = 0; i < userdata.ids.length; i++) {
                jQuery("#ChequeList").setSelection(userdata.ids[i], true);

                if ($("#AckUnackFlag").val() == "A") {
                    jQuery("#jqg_ChequeList_" + userdata.ids[i]).attr("disabled", true);
                }
                var rowData = $("#ChequeList").getRowData(userdata.ids[i]);

                if (rowData.ChequeAmount != undefined) {
                    total = parseFloat(total) + parseFloat(rowData.ChequeAmount);
                }
            }

            //if all roras selected  check header checkbox
            if (userdata.ids.length == $("#ChequeList").jqGrid('getGridParam', 'records')) {
                $('.cbox').attr('checked', true);
            }

          

           // $("#ChequeList").footerData('set', { "ChequeAmount": total.toFixed(2) }, true);  //set footer data

           
            var reccount = $('#ChequeList').getGridParam('reccount');
            if (reccount > 0) {
                $('#Chequepager_left').html('[Note: Select only those voucher that you wish to unacknowledge/acknowledge.]');
            }


            

            if (($("#ChequeList").getGridParam("reccount") > 14)) {
                $('#ChequeList').jqGrid('setGridHeight', "360px");
            }

            if ($("#AckUnackFlag").val() == "A") {
                $("#cb_ChequeList").trigger('click');
                $('.cbox').attr('checked', true);
            }


        },

        //Disable cheque box added By Abhishek kamble 26-Aug-2014
        beforeSelectRow: function (rowId, e) {
            if ($("#jqg_ChequeList_" + rowId).attr("disabled")) {
                return false;
            }
            return true;
        },
        sortname: 'Check_Issued_Date',
        sortorder: "asc",
        caption: "Cheque Details"
    });

}

//get the acknowledment details of already ack cheques
function GetAcknowledgementDetails(urlParam) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: "POST",
        url: "/ChequeAcknowledgement/GetAcknowledgedChequeDetails/" + urlParam,
        async: false,
        error: function (xhr, status, error) {
            unblockPage();
            $.unblockUI();

            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            $("#errorSpan").show('slow');
            return false;
        },
        success: function (data) {
            unblockPage();
            $.unblockUI();

            $('#divError').hide('slow');
            $('#errorSpan').html("");
            $('#errorSpan').hide();

            if (data.Success != "") {

                var details = data.Success.toString().split('$');
                $("#BILL_NO").val(details[1])
                $("#BILL_DATE").val(details[0]);
                ackVoucherFinalized = details[2];
                if (details[2] == "Y") {

                    $("#BILL_NO").attr('readonly', 'readonly');
                    $("#tblButtons").hide();//Added

                }
                else {

                    $("#BILL_NO").removeAttr('readonly', 'readonly');
                }


                return false;
            }
            else {
                alert("Error While getting cheque acknowledgement details ");
                return false;
            }
        }
    });

}

function ValidatePreviousCheques() {
    var result;
    $.ajax({
        type: "POST",
        url: "/ChequeAcknowledgement/ValidatePreviousCheques/",
        // async: false,
        error: function (xhr, status, error) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        cache: false,
        async: false,
        data: $("#FrmSelectionOptions").serialize(),
        success: function (data) {

            result = data.status;

            unblockPage();
            $('#divError').hide('slow');
            $('#errorSpan').html("");
            $('#errorSpan').hide();

            if (data.status === undefined) {

                $("#ackVoucherDiv").html(data);
                $("#gridDiv").show();
                $("#tblForm").show('slow');
                $("#tblButtons").show('slow');

                return false;

            }
            else if (data.status == true) {
                return true;
            }
            else if (data.status == false) {
                if (data.statusCode == "222") {
                    alert("Cheque Acknowledgement voucher of previous month are not finalized. Please finalize previous voucher. ");
                    return false;
                }
            }
            else {
                alert("error occured while Cheques  acknowledgement.");
                return false;
            }
        }
    });
    return result;
}
