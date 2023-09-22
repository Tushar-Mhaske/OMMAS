var isGridLoaded = false;
var lastsel3 = 0;
$(document).ready(function () {

    var enteredReconciliationUnreconciliationDate;

    $.validator.unobtrusive.parse($('#searchReconcilationList'));


    // Search Date Added By Abhishek kamble 17 Sep 2014 start

    $("#txtSearchBillDate").datepicker({
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        changeMonth: true,
        buttonImageOnly: true,
        buttonText: 'Date',
        changeYear: true,
        dateFormat: "dd/mm/yy",
        maxDate: new Date(),
    });
    $("#txtSearchBillDate").datepicker("setDate", $("#currentDate").val());
    //$("#txtSearchBillDate").attr('readonly', 'readonly');

    $("#rdoMonthWise").click(function () {
        if ($("#rdoDPIU").is(":checked")) {
            $(".tdSelectDPIU").show();
            $(".tdAllDPIU").hide();
        }
        $(".tdSearchMonthYear").show();
        $(".tdSearchChqEpayDate").hide();
    });

    $("#rdoDateWise").click(function () {
        if ($("#rdoDPIU").is(":checked")) {
            $(".tdAllDPIU").show();
            $(".tdSelectDPIU").hide();
        }
        $(".tdSearchMonthYear").hide();
        $(".tdSearchChqEpayDate").show();
    });

    // Search Date Added By Abhishek kamble 17 Sep 2014 end

    $("#rdoSRRDA").click(function () {
        $("#tdDPIU").hide();
        //$(".tdSearchMonthYear").hide();
        //$(".tdSearchBillDate").hide();

        $(".tdSelectDPIU").hide();
        $(".tdAllDPIU").hide();


    });

    $("#rdoDPIU").click(function () {
        $("#tdDPIU").show();

        if ($("#rdoMonthWise").is(":checked")) {
            $(".tdSelectDPIU").show();
        } else {
            $(".tdAllDPIU").show();
        }

        //$(".tdSearchMonthYear").show()

        //$(".tdSearchMonthYear").show();        
        //$(".tdSearchBillDate").show();
    });

    var date = new Date();
    var month = date.getMonth() + 1;

    $("#BILL_MONTH option:nth(" + month + ")").attr("selected", "selected");

    $("#BILL_YEAR option:eq(1)").attr("selected", "selected");

    $("#btnView").click(function () {
        if ($('#searchReconcilationList').valid()) {
            if ($("#rdoCheque").is(":checked")) {
                $("#tblBankReconPFMS").GridUnload();
                LoadGrid($("#BILL_MONTH").val(), $("#BILL_YEAR").val(), $("#ADMIN_ND_CODE").val());
            }
            else {
                $("#tblBankRecon").GridUnload();
                LoadGridPFMS($("#BILL_MONTH").val(), $("#BILL_YEAR").val(), $("#ADMIN_ND_CODE").val());
            }
        }
    });

    $.jgrid.jqModal = $.extend($.jgrid.jqModal || {}, {
        beforeOpen: centerInfoDialog
    });

    $("#dialog-message").dialog({
        resizable: false,
        closeOnEscape: true,
        height: 'auto',
        width: 290,
        modal: true,
        autoOpen: false,
        caption: "Error",
        //open: function () {
        //    $(this).parent().appendTo($('#frmAuthRequestList'));
        //},
        buttons: {
            "Close": function () {
                $("#dialog-message").dialog("close");
            }
        }
    });


    //Dialog box initalization 30-june-2014
    $(function () {

        //Eapyment Dialog
        $("#epaymentDialogBox").dialog({
            autoOpen: false,
            // height:550,
            width: 600,
            modal: true,
            show: {
                effect: "blind",
                duration: 1000
            },
            hide: {
                effect: "explode",
                duration: 1000
            }

        })//.css("font-size", ".75em").css("font-family", "Trebuchet MS,Tahoma,Verdana,Arial,sans-serif");

        //Eremittances Dialog
        $("#PaymentEremDialogForMaster").dialog({
            autoOpen: false,
            // height:550,
            width: 650,
            modal: true,
            show: {
                effect: "blind",
                duration: 1000
            },
            hide: {
                effect: "explode",
                duration: 1000
            }

        });



    });

    //Added By Abhishek kamble 30-June-2014
    $("#btnPrintEpaymentDetails").click(function () {
        PrintEpaymentDetails("#dvPrintEpaymentDetails");

    });


    $("#dvPrintEremDetailsForBank").click(function () {
        PrintEremDetails("#dvPrintEremittanceOrderDetails");
    });

}); //End of Document.Ready

function centerInfoDialog() {
    var $infoDlg = $("#info_dialog");
    var $parentDiv = $infoDlg.parent();
    var dlgWidth = $infoDlg.width();
    var parentWidth = $parentDiv.width();

    $infoDlg[0].style.left = Math.round((parentWidth - dlgWidth) / 2) + "px";
}

function headerDeSelcheckBox(e) {

    e = e || event; /* get IE event ( not passed ) */
    e.stopPropagation ? e.stopPropagation() : e.cancelBubble = true;
    if ($("#headerCheckDeSelect").is(':checked')) {

        //$('#tblBankRecon').setColProp('CHQEPAY_RECONCILE_DATE', { editable: false });
        //$('#tblBankRecon').setColProp('CHQ_RECONCILE_REMARKS', { editable: false });
        var rowID = $('#tblBankRecon').jqGrid('getGridParam', 'selrow');

        //$(rowID + "_CHQEPAY_RECONCILE_DATE").hide();
        //$(rowID + "_CHQEPAY_RECONCILE_DATE .ui-datepicker-trigger").hide();
        //$(rowID + "_CHQ_RECONCILE_REMARKS").hide();

        jQuery('#tblBankRecon').jqGrid('restoreRow', rowID);

        $('#tblBankRecon').jqGrid('resetSelection');

        $("#headerCheckSelect").attr('checked', false);
        $("#headerDate").datepicker({
            showOn: "button",
            buttonImage: "/Content/Images/calendar_2.png",
            showButtonText: 'Choose a date',
            changeMonth: true,
            buttonImageOnly: true,
            buttonText: 'Date',
            changeYear: true,
            dateFormat: "dd/mm/yy",
            maxDate: new Date(),
        });
        $("#headerDate").css('display', 'inline');
        $("#headerRemark").css('display', 'block');
        $("#tblBankRecon td[aria-describedby='tblBankRecon_IS_CHQ_RECONCILE']").each(function () {
            $(this).html('No');
        });

        $("#tblBankRecon").find(':checkbox').removeAttr('disabled');

        //added by abhishek
        $(".ui-datepicker-trigger").css('display', 'inline');
    }
    else {
        LoadGrid($("#BILL_MONTH").val(), $("#BILL_YEAR").val(), $("#ADMIN_ND_CODE").val());
    }
}

function headerSelcheckBox(e) {

    e = e || event; /* get IE event ( not passed ) */
    e.stopPropagation ? e.stopPropagation() : e.cancelBubble = true;
    if ($("#headerCheckSelect").is(':checked')) {

        var rowID = $('#tblBankRecon').jqGrid('getGridParam', 'selrow');
        jQuery('#tblBankRecon').jqGrid('restoreRow', rowID);

        //$('#tblBankRecon').setColProp('CHQEPAY_RECONCILE_DATE', { editable: false });
        //$('#tblBankRecon').setColProp('CHQ_RECONCILE_REMARKS', { editable: false });

        $('#tblBankRecon').jqGrid('resetSelection');
        $("#headerCheckDeSelect").attr('checked', false);
        $("#headerDate").datepicker({
            showOn: "button",
            buttonImage: "/Content/Images/calendar_2.png",
            showButtonText: 'Choose a date',
            changeMonth: true,
            buttonImageOnly: true,
            buttonText: 'Reconcile/UnReconcile Date',
            changeYear: true,
            dateFormat: "dd/mm/yy",
            maxDate: new Date(),
        });
        $("#headerDate").css('display', 'inline');
        $("#headerRemark").css('display', 'block');
        $("#tblBankRecon td[aria-describedby='tblBankRecon_IS_CHQ_RECONCILE']").each(function () {
            $(this).html('Yes');
        });

        $("#tblBankRecon").find(':checkbox').removeAttr('disabled');

        //added by abhishek
        $(".ui-datepicker-trigger").css('display', 'inline');
    }
    else {
        LoadGrid($("#BILL_MONTH").val(), $("#BILL_YEAR").val(), $("#ADMIN_ND_CODE").val());
    }
}

function IsValidDate(value, colname) {
    var regExValidDate = /^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))?$/;
    if ((!regExValidDate.test(value))) {
        return [false, "Please Enter Valid Reconcile Date"];
    }
    else {
        return [true, ""];
    }
}

function IsValidRemarks(value, colname) {
    if (!/^[A-Za-z0-9.\/\-\s\(\)]+$/i.test(value))
        return [false, "Please Enter Valid Remarks"];
    else
        return [true, ""];
}


function LoadGrid(month, year, dpiu) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    //Added By Abhishek kamble 17 Sep 2014 start

    var MonthDateWise;
    var dpiuCode;
    if ($("#rdoMonthWise").is(":checked")) {
        MonthDateWise = "M";
        dpiuCode = dpiu;
    } else if ($("#rdoDateWise").is(":checked")) {
        //dpiu = 
        dpiuCode = $("#DateWiseADMIN_ND_CODE").val();
        MonthDateWise = "D";
    }

    //Added By Abhishek kamble 17 Sep 2014 end
    // alert(dpiuCode);

    if (isGridLoaded) {
        $("#tblBankRecon").GridUnload();
        isGridLoaded = false;
    }

    var SRRDADpiu;

    if ($("#rdoSRRDA").is(":checked")) {
        SRRDADpiu = "S";
    }
    else if ($("#rdoDPIU").is(":checked")) {
        SRRDADpiu = "D";
    }

    jQuery("#tblBankRecon").jqGrid({
        url: '/Bank/BankReconciliationList',
        datatype: "json",
        mtype: "POST",
        colNames: [
                            'Cheque/EPay/Advice Number',
                            'Cheque/EPay/Advice Date',
                            'Payee Name',
                            'Amount (In Rs.)',
                            "Reconcile All&nbsp;&nbsp;&nbsp;&nbsp;<input title='Reconcile All' type='checkbox' id ='headerCheckSelect' class='chkHeader' onclick='headerSelcheckBox(event)' style='margin-left:5px' /><br> UnReconcile All<input title='UnReconcile All' type='checkbox' id ='headerCheckDeSelect' class='chkHeader' onclick='headerDeSelcheckBox(event)' style='margin-left:5px' /> ",
                            "Reconcile/UnReconcile Date <input type='text' id='headerDate' style='display:none' readonly='readonly'/>",
                            "Remarks <br> <center><textarea id='headerRemark' style='display:none; height:20px' rows='2' cols='20'/></center>",
                            'billid'
        ],
        colModel: [
                        { name: 'CHQ_NO', index: 'CHQ_NO', width: 80, align: 'center', sortable: true },
                        { name: 'CHQEPAY_DATE', index: 'CHQEPAY_DATE', width: 40, align: 'center', sortable: true },
                        { name: 'PAYEE_NAME', index: 'PAYEE_NAME', width: 110, align: 'left', sortable: false },
                        { name: 'CHQ_AMOUNT', index: 'CHQ_AMOUNT', width: 40, align: 'right', sortable: true },
                        {
                            name: 'IS_CHQ_RECONCILE', index: 'IS_CHQ_RECONCILE', width: 50, align: 'center', sortable: false, editable: true, edittype: "checkbox", editoptions: {
                                value: "Yes:No"/*, dataEvents: [{ type: 'click', data: { i: 7 }, fn: function (e) { alert(e.data.i); } }]*/
                            }
                        },
                        {
                            name: 'CHQEPAY_RECONCILE_DATE', index: 'CHQEPAY_RECONCILE_DATE', width: 80, align: 'center', sortable: false, editable: true, sorttype: "date", editoptions: { size: "20" }, //editrules: { required: true, date: true }
                            editrules: { required: true, custom: true, custom_func: IsValidDate }
                        },
                        {
                            name: 'CHQ_RECONCILE_REMARKS', index: 'CHQ_RECONCILE_REMARKS', width: 120, align: 'center', sortable: false, editable: true, edittype: "textarea", editoptions: { rows: "1", cols: "30", maxlength: 255 },
                            editrules: { required: true, custom: true, custom_func: IsValidRemarks }
                        },
                        { name: 'ENC_BILL_ID', index: 'ENC_BILL_ID', width: 0, align: 'center', hidden: true }

        ],
        pager: jQuery('#divBankReconPager'),
        rowNum: 0,
        pginput: false,
        pgbuttons: false,
        postData: {
            'Month': month,
            'Year': year,
            'DPIU': dpiuCode,
            'MonthDateWise': MonthDateWise, //Added By Abhishek kamble for search details date Wise 17 Sep 2014
            'SearchBillDate': $("#txtSearchBillDate").val(),
            'SRRDADpiu': SRRDADpiu,
        },
        editurl: "clientArray",
        cellSubmit: 'clientArray',
        // altRows: true,
        toppager: true,
        //  rowList: [100, 200, 300],
        viewrecords: true,
        recordtext: '{2} records found',
        //sortname: 'CHQ_DATE',
        sortname: 'CHQEPAY_DATE,CHQ_NO',
        // sortorder: "desc",
        caption: "Bank Reconciliation Details",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        loadComplete: function () {
            $.unblockUI();

            isGridLoaded = true;
            $("#tblBankRecon_rn").find('div:eq(0)').html("Sr No");
            if ($('#tblBankRecon').jqGrid('getGridParam', 'reccount') > 0) {

                var topPagerDiv = $("#pg_tblBankRecon_toppager")[0];

                $("#tblBankRecon_toppager_center", topPagerDiv).remove();

                $("#divBankReconPager_center").html("<span style='float:left;' id='spnButton' class='ui-icon ui-icon-info'></span><span style='text-align:left;color:green;float:left'>For Single Cheque <font color='#1C94C4'>Reconcile</font>/ <font color='#1C94C4'>UnReconcile</font> Select Cheque/Advice.</span>");

                $("#tblBankRecon_toppager_right").html("<span style='text-align:left;color:green;float:right'>To Reconcile/UnReconcile All Cheques, Use <font color='#1C94C4'>Reconcile All</font>/ <font color='#1C94C4'>UnReconcile All</font> Selection</span><span style='float:right' class='ui-icon ui-icon-info'></span>");
                $("#tblBankRecon_toppager_left").html("<span style='float:left' id='spnButton' class='ui-icon ui-icon-info'></span><span style='text-align:left;color:green;float:left'>After all selection is done Click on <font color='#1C94C4'><b>Save</b></font> button to confirm changes</span>");
            }
            else {

                var topPagerDiv = $("#pg_tblBankRecon_toppager")[0];
                $("#tblBankRecon_toppager_center", topPagerDiv).remove();
                $("#tblBankRecon_toppager_right", topPagerDiv).remove();
                $("#jqgh_tblBankRecon_IS_CHQ_RECONCILE").html("Reconcile");
            }
            $("#tblBankRecon").parents('div.ui-jqgrid-bdiv').css("max-height", '400px');

            var recordCount = $('#tblBankRecon').jqGrid('getGridParam', 'reccount');
            if (recordCount > 0) {
                navBankReconGrid();
            }

        },
        onSelectRow: function (id) {

            //added by abhishek
            //$(".ui-datepicker-trigger").css('display', 'none');
            $("#jqgh_tblBankRecon_CHQEPAY_RECONCILE_DATE .ui-datepicker-trigger").css('display', 'none');

            //added by koustubh nakate on 25/07/2013 to reset header selection
            $("#headerCheckDeSelect").attr('checked', false);
            $("#headerCheckSelect").attr('checked', false);
            $("#headerDate").hide();
            $("#headerRemark").hide();
            jQuery('#tblBankRecon').jqGrid('restoreRow', lastsel3);

            // alert(id + "" + lastSelected);
            // if (id && id!=lastsel3 ) {

            jQuery("#tblBankRecon").saveRow(lastsel3, false, 'clientArray');
            //jQuery('#tblBankRecon').jqGrid('restoreRow', lastsel3);
            jQuery('#tblBankRecon').jqGrid('editRow', id, true, pickdates);
            lastsel3 = id;

            // }
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

    });

}

function pickdates(id) {
    $("#" + id + "_CHQEPAY_RECONCILE_DATE", "#tblBankRecon").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonImageOnly: true,
        buttonText: 'Reconcile/Unconcile Date',
        showButtonText: 'Select date',
        maxDate: new Date(),
    });
}


function navBankReconGrid() {

    jQuery("#tblBankRecon").navGrid('#divBankReconPager', { edit: false, add: false, del: false, search: false, refresh: false })
           .navButtonAdd('#divBankReconPager', {
               caption: "Save",
               id: "btnSave",
               title: "Click here to save details",
               buttonicon: "ui-icon ui-icon-folder-collapsed",
               onClickButton: function () {
                   var selectedRow = $("#tblBankRecon").jqGrid('getGridParam', 'selrow');
                   var rowId = parseInt(selectedRow);
                   //alert(rowId);

                   //If added by Abhishek kamlbe 19-Aug-2014
                   if (!($("#headerCheckSelect").is(":checked")) && !($("#headerCheckDeSelect").is(":checked"))) {
                       if (isNaN(rowId)) {
                           $(".ui-dialog-title").html("Required");
                           $('.ui-button-text').each(function (i) {
                               $(this).html($(this).parent().attr('text'))
                           })
                           $("#dialog-message").html("<center>Reconcile/UnReconcile All Date and Remark<br/>:Field is Required</center>");
                           $("#dialog-message").dialog('open');
                           return false;
                       }
                   }
                   //if (recCount =) {
                   //        $("#dialog-message").html("<center>Reconcile/UnReconcile All Date<br/>:Field is Required</center>");
                   //        $("#dialog-message").dialog('open');
                   //        return false;
                   //    }

                   if ($('#tblBankRecon').jqGrid('getGridParam', 'reccount') > 0) {

                       var headerDate = null;
                       var headerRemark = null;
                       var headerIsReconcile = null;
                       var postData = null;
                       if ($("#headerCheckSelect").is(':checked') || $("#headerCheckDeSelect").is(':checked')) {

                           headerDate = $("#headerDate").val();
                           headerRemark = $("#headerRemark").val();
                           if ($("#headerCheckSelect").is(':checked')) {
                               headerIsReconcile = "yes";
                           }
                           else if ($("#headerCheckDeSelect").is(':checked')) {
                               headerIsReconcile = "no";
                           }

                           var regExValidDate = /^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))?$/;
                           if ($("#headerDate").val().replace(new RegExp(" ", "g"), "") == "") {
                               $(".ui-dialog-title").html("Required");
                               $('.ui-button-text').each(function (i) {
                                   $(this).html($(this).parent().attr('text'))
                               })
                               $("#dialog-message").html("<center>Reconcile/UnReconcile All Date<br/>:Field is Required</center>");
                               $("#dialog-message").dialog('open');
                               return false;
                           }
                           else if ((!regExValidDate.test(headerDate))) {
                               $(".ui-dialog-title").html("Error");
                               $('.ui-button-text').each(function (i) {
                                   $(this).html($(this).parent().attr('text'))
                               })
                               $("#dialog-message").html("<center>Reconcile/UnReconcile All Date<br/>:Invalid Date</center>");
                               $("#dialog-message").dialog('open');

                               return false;
                           }
                           else if ($("#headerRemark").val().replace(new RegExp(" ", "g"), "") == "") {
                               $(".ui-dialog-title").html("Required");
                               $('.ui-button-text').each(function (i) {
                                   $(this).html($(this).parent().attr('text'))
                               })
                               $("#dialog-message").html("<center>Reconcile/UnReconcile All Remarks<br/>:Field is Required</center>");
                               $("#dialog-message").dialog('open');

                               return false;
                           }
                           else if (!/^[A-Za-z0-9.\/\-\s\(\)]+$/i.test(headerRemark)) {
                               $(".ui-dialog-title").html("Error");
                               $('.ui-button-text').each(function (i) {
                                   $(this).html($(this).parent().attr('text'))
                               })
                               $("#dialog-message").html("<center>Remark contails invalid character.</center>");
                               $("#dialog-message").dialog('open');

                               return false;
                           }

                           //new added by abhishek start 21-8-2013

                           if ($("#headerCheckSelect").is(':checked') || $("#headerCheckDeSelect").is(':checked')) {
                               enteredReconciliationUnreconciliationDate = $("#headerDate").val();
                           }
                           else {
                               enteredReconciliationUnreconciliationDate = reconcileDate;
                           }

                           var todaysDate = $("#currentDate").val();

                           var frommonthfield = enteredReconciliationUnreconciliationDate.split("/")[1];
                           var fromdayfield = enteredReconciliationUnreconciliationDate.split("/")[0];
                           var fromyearfield = enteredReconciliationUnreconciliationDate.split("/")[2];

                           var tomonthfield = todaysDate.split("/")[1];
                           var todayfield = todaysDate.split("/")[0];
                           var toyearfield = todaysDate.split("/")[2];

                           var reconciliationDate = new Date(fromyearfield, frommonthfield, fromdayfield);
                           var currentDate = new Date(toyearfield, tomonthfield, todayfield);

                           if (reconciliationDate > currentDate) {
                               $("#dialog-message").html("<center>Reconcile/UnReconcile date must be less than or equal to todays date.</center>");
                               $("#dialog-message").dialog('open');
                               return false;
                           }

                           //new added by abhishek end




                       }
                       else {

                           //alert('else');
                           //added by koustubh nakate on 25/07/2013 for validation on row level

                           var rowID = $('#tblBankRecon').jqGrid('getGridParam', 'selrow');
                           var reconcileDate = $('#' + rowID + '_CHQEPAY_RECONCILE_DATE').val();
                           var reconcileRemark = $('#' + rowID + '_CHQ_RECONCILE_REMARKS').val();
                           //  var chqEpayDate = $('#' + rowID + '_CHQEPAY_DATE').val();



                           //  var grid = jQuery('#tblBankRecon');
                           //  var sel_id = grid.jqGrid('getGridParam', 'selrow');
                           //  var chqEpayDate = grid.jqGrid('getCell', sel_id, 'CHQEPAY_DATE');
                           ////  alert(myCellData);

                           //  if (reconcileDate < chqEpayDate) {
                           //      $("#dialog-message").html("<center>Reconcile Date<br/>:Reconsile Date should be greater than Cheque/Epay Date.</center>");
                           //      $("#dialog-message").dialog('open');
                           //      return false;
                           //  }



                           // alert(reconcileDate + ' ' + reconcileRemark);

                           //$(rowID + "_CHQEPAY_RECONCILE_DATE").hide();
                           //$(rowID + "_CHQEPAY_RECONCILE_DATE .ui-datepicker-trigger").hide();
                           //$(rowID + "_CHQ_RECONCILE_REMARKS").hide();

                           if (reconcileDate === undefined) {
                               return false;
                           }

                           var regExValidDate = /^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))?$/;
                           if (reconcileDate.replace(new RegExp(" ", "g"), "") == "") {
                               $(".ui-dialog-title").html("Required");
                               $('.ui-button-text').each(function (i) {
                                   $(this).html($(this).parent().attr('text'))
                               })
                               $("#dialog-message").html("<center>Reconcile/UnReconcile Date<br/>:Field is Required</center>");
                               $("#dialog-message").dialog('open');
                               return false;
                           }
                           else if ((!regExValidDate.test(reconcileDate))) {
                               $(".ui-dialog-title").html("Error");
                               $('.ui-button-text').each(function (i) {
                                   $(this).html($(this).parent().attr('text'))
                               })
                               $("#dialog-message").html("<center>Reconcile/UnReconcile Date<br/>:Invalid Date</center>");
                               $("#dialog-message").dialog('open');

                               return false;
                           }
                           else if (reconcileRemark.replace(new RegExp(" ", "g"), "") == "") {
                               $(".ui-dialog-title").html("Required");
                               $('.ui-button-text').each(function (i) {
                                   $(this).html($(this).parent().attr('text'))
                               })
                               $("#dialog-message").html("<center>Reconcile/UnReconcile Remark<br/>:Field is Required</center>");
                               $("#dialog-message").dialog('open');

                               return false;
                           }
                           else if (!/^[A-Za-z0-9.\/\-:\s\(\)]+$/i.test(reconcileRemark)) {
                               $(".ui-dialog-title").html("Error");
                               $('.ui-button-text').each(function (i) {
                                   $(this).html($(this).parent().attr('text'))
                               })
                               $("#dialog-message").html("<center>Reconcile/UnReconcile Remark<br/>:Invalid Remark</center>");
                               $("#dialog-message").dialog('open');

                               return false;
                           }

                           //new added by abhishek start 21-8-2013



                           if ($("#headerCheckSelect").is(':checked') || $("#headerCheckDeSelect").is(':checked')) {
                               enteredReconciliationUnreconciliationDate = $("#headerDate").val();
                           }
                           else {
                               enteredReconciliationUnreconciliationDate = reconcileDate;
                           }

                           //alert(reconcilationDate);

                           var todaysDate = $("#currentDate").val();
                           var frommonthfield = enteredReconciliationUnreconciliationDate.split("/")[1];
                           var fromdayfield = enteredReconciliationUnreconciliationDate.split("/")[0];
                           var fromyearfield = enteredReconciliationUnreconciliationDate.split("/")[2];

                           var tomonthfield = todaysDate.split("/")[1];
                           var todayfield = todaysDate.split("/")[0];
                           var toyearfield = todaysDate.split("/")[2];

                           var reconciliationDate = new Date(fromyearfield, frommonthfield, fromdayfield);
                           var currentDate = new Date(toyearfield, tomonthfield, todayfield);

                           if (reconciliationDate > currentDate) {
                               $("#dialog-message").html("<center>Reconcile/UnReconcile Date<br/>:Reconcile/UnReconcile date must be less than or equal to todays date.</center>");
                               $("#dialog-message").dialog('open');
                               return false;
                           }

                           //new added by abhishek end


                           // var gridData = jQuery("#tblBankRecon").getRowData(rowID);

                           if ($('#' + rowID + '_IS_CHQ_RECONCILE').is(':checked')) {
                               headerIsReconcile = "yes";

                               // alert($("#tblBankRecon").jqGrid('getCell', rowID, 'IS_CHQ_RECONCILE'));
                           }
                           else {//if ($('#' + rowID + '_IS_CHQ_RECONCILE').is(':checked')) {
                               //  alert($('#' + rowID + '_IS_CHQ_RECONCILE').offval());
                               // alert($("#tblBankRecon").jqGrid('getCell', rowID, 'IS_CHQ_RECONCILE'))
                               headerIsReconcile = "no";
                           }

                           //var gridData = $("#tblBankRecon").jqGrid('getRowData', rowID);
                           headerDate = reconcileDate;
                           headerRemark = reconcileRemark;
                           postData = $("#tblBankRecon").jqGrid('getCell', rowID, 'ENC_BILL_ID');
                           //postData = JSON.stringify(gridData);

                       }
                       var varMsg = null;
                       if (true) {
                           varMsg = "Are you sure you want Save Details?";
                       }
                       else {
                           varMsg = "Are you sure you want to Save all details?";
                       }

                       //Added By Abhishek kamble 17 Sep 2014 start

                       var MonthDateWise;
                       var dpiuCode;

                       if ($("#rdoMonthWise").is(":checked")) {
                           MonthDateWise = "M";
                           dpiuCode = $('#ADMIN_ND_CODE option:selected').val();

                       } else if ($("#rdoDateWise").is(":checked")) {
                           MonthDateWise = "D";
                           dpiuCode = $("#DateWiseADMIN_ND_CODE option:selected").val();
                       }
                       //Added By Abhishek kamble 17 Sep 2014 end

                       if (confirm(varMsg)) {

                           var SRRDADpiu;
                           if ($("#rdoSRRDA").is(":checked")) {
                               SRRDADpiu = "S";
                           }
                           else if ($("#rdoDPIU").is(":checked")) {
                               SRRDADpiu = "D";
                           }

                           //added by abhishek kamble 22-8-2013 start
                           if ($("#headerCheckDeSelect").is(':checked')) {

                               if (confirm("Only those cheques are UnReconcile which are reconcile")) {
                                   $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

                                   $.ajax({
                                       type: "POST",
                                       async: false,
                                       cashe: false,
                                       url: "/Bank/postGridData/",
                                       data: {
                                           jqGridData: postData,
                                           jqGridHeaderDate: headerDate,
                                           jqGridHeaderRemark: headerRemark,
                                           jqGridHeaderReconcile: headerIsReconcile,
                                           month: $('#BILL_MONTH option:selected').val(),
                                           year: $('#BILL_YEAR option:selected').val(),
                                           // dpiu: $('#ADMIN_ND_CODE option:selected').val(),
                                           dpiu: dpiuCode,
                                           'MonthDateWise': MonthDateWise, //Added By Abhishek kamble for search details date Wise 17 Sep 2014
                                           'SearchBillDate': $("#txtSearchBillDate").val(),
                                           'SRRDADpiu': SRRDADpiu
                                       },
                                       //contentType: 'application/json;charset=utf-8',
                                       success: function (data) {
                                           $.unblockUI();

                                           if (data.success == true) {
                                               //alert("Chqeues Reconciled Successfully");
                                               alert(data.message);
                                               LoadGrid($("#BILL_MONTH").val(), $("#BILL_YEAR").val(), $("#ADMIN_ND_CODE").val());
                                           }
                                           else if (data.success == false) {
                                               alert(data.message);
                                           }


                                       },
                                       error: function (xhr, textStatus, errorThrown) {
                                           $.unblockUI();

                                           alert(xhr.responseText);
                                       }
                                   });

                               }

                           } else if ($("#headerCheckSelect").is(':checked')) {

                               if (confirm("Only those cheques are Reconcile which are Unreconcile")) {
                                   $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

                                   $.ajax({
                                       type: "POST",
                                       async: false,
                                       cashe: false,
                                       url: "/Bank/postGridData/",
                                       data: {
                                           jqGridData: postData,
                                           jqGridHeaderDate: headerDate,
                                           jqGridHeaderRemark: headerRemark,
                                           jqGridHeaderReconcile: headerIsReconcile,
                                           month: $('#BILL_MONTH option:selected').val(),
                                           year: $('#BILL_YEAR option:selected').val(),
                                           //dpiu: $('#ADMIN_ND_CODE option:selected').val(),
                                           dpiu: dpiuCode,
                                           'MonthDateWise': MonthDateWise, //Added By Abhishek kamble for search details date Wise 17 Sep 2014
                                           'SearchBillDate': $("#txtSearchBillDate").val(),
                                           'SRRDADpiu': SRRDADpiu
                                       },
                                       //contentType: 'application/json;charset=utf-8',
                                       success: function (data) {
                                           $.unblockUI();

                                           if (data.success == true) {
                                               //alert("Chqeues Reconciled Successfully");
                                               alert(data.message);
                                               LoadGrid($("#BILL_MONTH").val(), $("#BILL_YEAR").val(), $("#ADMIN_ND_CODE").val());
                                           }
                                           else if (data.success == false) {
                                               alert(data.message);
                                           }


                                       },
                                       error: function (xhr, textStatus, errorThrown) {
                                           $.unblockUI();

                                           alert(xhr.responseText);
                                       }
                                   });
                               }

                           }
                           else {

                               $.ajax({
                                   type: "POST",
                                   async: false,
                                   cashe: false,
                                   url: "/Bank/postGridData/",
                                   data: {
                                       jqGridData: postData,
                                       jqGridHeaderDate: headerDate,
                                       jqGridHeaderRemark: headerRemark,
                                       jqGridHeaderReconcile: headerIsReconcile,
                                       month: $('#BILL_MONTH option:selected').val(),
                                       year: $('#BILL_YEAR option:selected').val(),
                                       //dpiu: $('#ADMIN_ND_CODE option:selected').val(),
                                       dpiu: dpiuCode,
                                       'MonthDateWise': MonthDateWise, //Added By Abhishek kamble for search details date Wise 17 Sep 2014
                                       'SearchBillDate': $("#txtSearchBillDate").val(),
                                       'SRRDADpiu': SRRDADpiu
                                   },
                                   //contentType: 'application/json;charset=utf-8',
                                   success: function (data) {

                                       if (data.success == true) {
                                           alert(data.message);
                                           LoadGrid($("#BILL_MONTH").val(), $("#BILL_YEAR").val(), $("#ADMIN_ND_CODE").val());
                                       }
                                       else if (data.success == false) {
                                           alert(data.message);
                                       }


                                   },
                                   error: function (xhr, textStatus, errorThrown) {
                                       alert(xhr.responseText);
                                   }
                               });

                           }

                           //added by abhishek kamble 22-8-2013 end

                       }
                       else {
                           return false;
                       }
                   }
                   else {
                       $('.ui-button-text').each(function (i) {
                           $(this).html($(this).parent().attr('text'))
                       })
                       $("#dialog-message").html("<center>Cheques not present to Reconcile/UnReconcile</center>");
                       $("#dialog-message").dialog('open');
                   }
               },
               position: "first"
           });



}


//function to show epayment order Added By Abhishek kamble 30-June-2014 
function ViewEpayOrder(urlParam1) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: "POST",
        url: "/payment/GetEpaymentOrderDetails/" + urlParam1,
        //async: false,
        error: function (xhr, status, error) {
            unblockPage();
            $.unblockUI();

            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            $('#errorSpan').show();
            return false;
        },
        success: function (data) {
            $.unblockUI();
            unblockPage();
            $('#divError').hide('slow');
            $('#errorSpan').html("");
            $('#errorSpan').hide();

            if (data.Success) {

                $("#EmailRecepient").text(data.EmailRecepient).css("font-weight", "bold");
                $("#DPIUName").text(data.DPIUName).css("font-weight", "bold");
                $("#STATEName").text(data.STATEName).css("font-weight", "bold");
                $("#EmailDate").text(data.EmailDate).css("font-weight", "bold");
                $("#Bankaddress").text(data.Bankaddress).css("font-weight", "bold");
                $("#BankAcNumber").text(data.BankAcNumber).css("font-weight", "bold");
                $("#EpayNumber").text(data.EpayNumber).css("font-weight", "bold");
                $("#EpayDate").text(data.EpayDate).css("font-weight", "bold");
                $("#EpayState").text(data.EpayState).css("font-weight", "bold");
                $("#EpayDPIU").text(data.EpayDPIU).css("font-weight", "bold");
                $("#EpayVNumber").text(data.EpayVNumber).css("font-weight", "bold");
                $("#EpayVDate").text(data.EpayVDate).css("font-weight", "bold");
                $("#EpayVPackages").text(data.EpayVPackages).css("font-weight", "bold");
                $("#EpayConName").text(data.EpayConName).css("font-weight", "bold");
                $("#EpayConAcNum").text(data.EpayConAcNum).css("font-weight", "bold");
                $("#EpayConBankName").text(data.EpayConBankName).css("font-weight", "bold");
                $("#EpayConBankIFSCCode").text(data.EpayConBankIFSCCode).css("font-weight", "bold");
                $("#EpayAmount").text(data.EpayAmount).css("font-weight", "bold");
                $("#EpayNo").text(data.EpayNumber).css("font-weight", "bold");
                $("#EpayAmountInWord").text(data.EpayAmountInWord).css("font-weight", "bold");
                $("#urlParam").val(urlParam1);
                //Added by Abhishek kamble 29-May-2014                
                if (data.EpayContLegalHeirName == "" || data.EpayContLegalHeirName == null) {
                    $("#trContLegalHeirDetails").hide();
                }
                else {
                    $("#trContLegalHeirDetails").show();
                }
                $("#EpayConLegalHeirName").text(data.EpayContLegalHeirName).css("font-weight", "bold");

                if (data.ShowPassword == "N") {
                    //alert("Y");
                    //Modified By Abhishek kamble 28-May-2014
                    //$("#paaswordDiv").remove();
                    $("#paaswordDiv").hide();
                    $("#dvPrintEpayDetails").show();
                }
                else {
                    //alert("N");

                    $("#paaswordDiv").show();
                    $("#dvPrintEpayDetails").hide();
                }

                $("#epaymentDialogBox").dialog("open");

                return false;
            }
        }
    });
}

//Added By Abhishek kamble 30-June-2014
function PrintEpaymentDetails(elem) {
    //var size = $(elem).css('font-size');
    PrintEpayPopup($(elem).html());
}

function PrintEpayPopup(data) {


    if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {   // Chrome Browser Detected?

        var windowSummary = window.open('', 'Epayment', 'height=700,width=700');
        windowSummary.document.write('<html><head><title></title>');
        windowSummary.document.write('<style type="text/css">td, th{border:1px solid black;}#first {border-collapse:collapse;}th.ui-th-column div {white-space: normal !important;height: auto !important;padding: 2px;}</style>');

        windowSummary.document.write('</head><body>');
        windowSummary.document.write('<h3 style="text-align:center">Epayment Details</h3>');

        windowSummary.document.write(data);
        windowSummary.document.write('</body></html>');

        windowSummary.PPClose = false;                                     // Clear Close Flag
        windowSummary.onbeforeunload = function () {                         // Before Window Close Event
            if (windowSummary.PPClose === false) {                           // Close not OK?
                return 'Leaving this page will block the parent window!\nPlease select "Stay on this Page option" and use the \nCancel button instead to close the Print Preview Window.\n';
            }
        }

        windowSummary.print();
        //mywindow.close();
    } else {
        var windowSummary = window.open('', 'Epayment', 'height=700,width=700');
        windowSummary.document.write('<html><head><title></title>');
        windowSummary.document.write('<style type="text/css">td, th{border:1px solid black;}#first {border-collapse:collapse;}th.ui-th-column div {white-space: normal !important;height: auto !important;padding: 2px;}</style>');

        windowSummary.document.write('</head><body>');
        windowSummary.document.write('<h3 style="text-align:center">Epayment Details</h3>');

        windowSummary.document.write(data);
        windowSummary.document.write('</body></html>');

        windowSummary.print();
    }
    return true;
}


//function to show Eremittance order 
function ViewEremOrder(urlParam) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: "POST",
        url: "/payment/GetERemOrderDetails/" + urlParam,
        //async: false,
        error: function (xhr, status, error) {
            $.unblockUI();
            unblockPage();

            $('#errorSpan2').text(xhr.responseText);
            $('#divError2').show('slow');
            $('#errorSpan2').show();
            return false;
        },
        success: function (data) {
            $.unblockUI();
            unblockPage();
            $('#divError2').hide('slow');
            $('#errorSpan2').html("");
            $('#errorSpan2').hide();


            if (data != "") {

                $("#ERemOrderDivForMaster").html(data);
                $("#urlParamRem").val(urlParam);
                $("#PaymentEremDialogForMaster").dialog("open");

                if ($("#RemShowPassword").val() == "N") {
                    //Modified by Abhishek kamble 28-May-2014
                    //$("#paaswordERemDiv").remove();
                    $("#paaswordERemDiv").hide();
                    $("#dvPrintEremDetails").show();
                }
                else {
                    $("#paaswordERemDiv").show();
                    $("#dvPrintEremDetails").hide();

                }
                //Hide Print dv 30-June-2014 by Abhishek kamble
                $("#PrintEremDetails").hide();

                return false;
            }
        }
    });
}


//Added By Abhishek kamble 26-May-2014
function PrintEremDetails(elem) {
    //var size = $(elem).css('font-size');
    PrintERemPopup($(elem).html());
}

function PrintERemPopup(data) {


    if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {   // Chrome Browser Detected?

        var windowSummary = window.open('', 'ERemittace', 'height=800,width=700');
        windowSummary.document.write('<html><head><title></title>');
        windowSummary.document.write('<style type="text/css">td, th{border:1px solid black;}#first {border-collapse:collapse;}th.ui-th-column div {white-space: normal !important;height: auto !important;padding: 2px;}</style>');

        windowSummary.document.write('</head><body>');
        windowSummary.document.write('<h3 style="text-align:center">E-Remittance Details</h3>');
        windowSummary.document.write(data);
        windowSummary.document.write('</body></html>');

        windowSummary.PPClose = false;                                     // Clear Close Flag
        windowSummary.onbeforeunload = function () {                         // Before Window Close Event
            if (windowSummary.PPClose === false) {                           // Close not OK?
                return 'Leaving this page will block the parent window!\nPlease select "Stay on this Page option" and use the \nCancel button instead to close the Print Preview Window.\n';
            }
        }
        windowSummary.print();
        //mywindow.close();
    } else {
        var windowSummary = window.open('', 'ERemittace', 'height=800,width=700');
        windowSummary.document.write('<html><head><title></title>');
        windowSummary.document.write('<style type="text/css">td, th{border:1px solid black;}#first {border-collapse:collapse;}th.ui-th-column div {white-space: normal !important;height: auto !important;padding: 2px;}</style>');

        windowSummary.document.write('</head><body>');
        windowSummary.document.write('<h3 style="text-align:center">E-Remittance Details</h3>');
        windowSummary.document.write(data);
        windowSummary.document.write('</body></html>');

        windowSummary.print();
    }
    return true;
}




function LoadGridPFMS(month, year, dpiu) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    //Added By Abhishek kamble 17 Sep 2014 start

    var MonthDateWise;
    var dpiuCode;
    if ($("#rdoMonthWise").is(":checked")) {
        MonthDateWise = "M";
        dpiuCode = dpiu;
    } else if ($("#rdoDateWise").is(":checked")) {
        //dpiu = 
        dpiuCode = $("#DateWiseADMIN_ND_CODE").val();
        MonthDateWise = "D";
    }

    //Added By Abhishek kamble 17 Sep 2014 end
    // alert(dpiuCode);


    $("#tblBankReconPFMS").GridUnload();


    var SRRDADpiu;

    if ($("#rdoSRRDA").is(":checked")) {
        SRRDADpiu = "S";
    }
    else if ($("#rdoDPIU").is(":checked")) {
        SRRDADpiu = "D";
    }

    jQuery("#tblBankReconPFMS").jqGrid({
        url: '/Bank/BankReconciliationList',
        datatype: "json",
        mtype: "POST",
        colNames: [
                            'Cheque/EPay/Advice Number',
                            'Cheque/EPay/Advice Date',
                            'Payee Name',
                            'Amount (In Rs.)',
                            "Reconcile All&nbsp;&nbsp;&nbsp;&nbsp;<input title='Reconcile All' type='checkbox' id ='headerCheckSelect' class='chkHeader' onclick='headerSelcheckBox(event)' style='margin-left:5px' /><br> UnReconcile All<input title='UnReconcile All' type='checkbox' id ='headerCheckDeSelect' class='chkHeader' onclick='headerDeSelcheckBox(event)' style='margin-left:5px' /> ",
                            "Reconcile/UnReconcile Date <input type='text' id='headerDate' style='display:none' readonly='readonly'/>",
                            "Remarks <br> <center><textarea id='headerRemark' style='display:none; height:20px' rows='2' cols='20'/></center>",
                            'billid'
        ],
        colModel: [
                        { name: 'CHQ_NO', index: 'CHQ_NO', width: 80, align: 'center', sortable: true },
                        { name: 'CHQEPAY_DATE', index: 'CHQEPAY_DATE', width: 40, align: 'center', sortable: true },
                        { name: 'PAYEE_NAME', index: 'PAYEE_NAME', width: 110, align: 'left', sortable: false },
                        { name: 'CHQ_AMOUNT', index: 'CHQ_AMOUNT', width: 40, align: 'right', sortable: true },
                        {
                            name: 'IS_CHQ_RECONCILE', index: 'IS_CHQ_RECONCILE', width: 50, align: 'center', sortable: false, editable: true, edittype: "checkbox", hidden: true,
                            editoptions: {value: "Yes:No"/*, dataEvents: [{ type: 'click', data: { i: 7 }, fn: function (e) { alert(e.data.i); } }]*/}
                        },
                        {
                            name: 'CHQEPAY_RECONCILE_DATE', index: 'CHQEPAY_RECONCILE_DATE', width: 80, align: 'center', sortable: false, editable: true, sorttype: "date", hidden: true, editoptions: { size: "20" }, //editrules: { required: true, date: true }
                            editrules: { required: true, custom: true, custom_func: IsValidDate }
                        },
                        {
                            name: 'CHQ_RECONCILE_REMARKS', index: 'CHQ_RECONCILE_REMARKS', width: 120, align: 'center', sortable: false, editable: true, edittype: "textarea", editoptions: { rows: "1", cols: "30", maxlength: 255 },
                            editrules: { required: true, custom: true, custom_func: IsValidRemarks }
                        },
                        { name: 'ENC_BILL_ID', index: 'ENC_BILL_ID', width: 0, align: 'center', hidden: true }

        ],
        pager: jQuery('#divBankReconPFMSPager'),
        rowNum: 0,
        pginput: false,
        pgbuttons: false,
        postData: {
            'Month': month,
            'Year': year,
            'DPIU': dpiuCode,
            'MonthDateWise': MonthDateWise, //Added By Abhishek kamble for search details date Wise 17 Sep 2014
            'SearchBillDate': $("#txtSearchBillDate").val(),
            'SRRDADpiu': SRRDADpiu,
            'ChequeEpay': 'E'
        },
        editurl: "clientArray",
        cellSubmit: 'clientArray',
        // altRows: true,
        toppager: true,
        //  rowList: [100, 200, 300],
        viewrecords: true,
        recordtext: '{2} records found',
        //sortname: 'CHQ_DATE',
        sortname: 'CHQEPAY_DATE,CHQ_NO',
        // sortorder: "desc",
        caption: "Bank Reconciliation Details",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        loadComplete: function () {
            $.unblockUI();

        },
        onSelectRow: function (id) {

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

    });

}