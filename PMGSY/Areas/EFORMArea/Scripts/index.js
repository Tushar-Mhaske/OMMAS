

$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

$(document).ready(function () {

    $.unblockUI();

  //  alert($('#ticker_02 > li').length);
    if ($('#ticker_02 > li').length > 5) {

        startTimer();
    }

    //to stop the ticker on mouse hower
    $('#ticker_02 li').mouseenter(function () {
       // $(this).css('cursor','progress');
        if ($('#ticker_02 > li').length > 5) {
            clearInterval(autoInterval, startTimer);
        }
    }).mouseleave(function () {
       // $(this).css('cursor','pointer');
        if ($('#ticker_02 > li').length > 5) {
            startTimer();
        }
    });

    //example for alert ticker from server
    /*
   
    $.ajax({
        url:"your url here",
        timeout: 10000,
        success: function (data) {
            if (!data.results) {
                return false;
            }

            //create each li for the entry in the list
            for (var i in data.results)
            {
                var result = data.results[i];
                var $res = $("<li />");
                $res.append(result.text);
                console.log(data.results[i]);
                $res.appendTo($('#ticker_02'));
            }
            setInterval(function () { tick2() }, 4000);

           

        }
    });*/
   
    $("#alert").click(function ()
    {
        Alert("Customised alerts");
        window.location.replace("/menu/GetMenuLevelMapping");
    });

    //apply custom tooltip to title


    $('#prompt').click(function () {
        Prompt('How would you describe qTip2?', 'Awesome!', function (response) {
            Alert("this is response " + response);
        });
    });

    $('#Confirm').click(function () {
        
        Confirm('Are you sure you want to see this ?', function (value) {
            Alert("you clicked " + value);
        });
    });
 
    $("#DigReceiptPayment").dialog({
        resizable: false,
        closeOnEscape: true,
        title: "Receipt/Payment Entry Screen",
        height: 'auto',
        width: '400',
        modal: true,
        autoOpen: false,
        open: function () {
            $(this).parent().appendTo($('#frmAddReceipt'));
        }
    });

    
    var levelID = $('#Level').val();
    var fundType=$('#fundType').val();
    //alert(levelID);
    if (levelID == '4' ) {
      
        $('#spnBankAuthDetails').html('Latest DPIU Transaction Details');
        LoadLatestDPIUTransactionDetails();
    }
    if (levelID == '5' && (fundType == 'P' || fundType == 'M')) {
        $('#spnBankAuthDetails').html('Bank Authorization Request Details');
        LoadPFAuthorizationGrid();
        LoadDPIUAutorizationIssuedDetails();

    }
    else if (levelID == '5' && (fundType == 'A')) {
        $('#spnBankAuthDetails').html('DPIU Authorization Issued Details');
        LoadDPIUAutorizationIssuedDetails();
    }

    //load lettest Transaction details Table
    LoadlettestTransactionGrid()
    

    $("#tabs").tabs();

}); //doc.Ready ends here



function LoadPFAuthorizationGrid() {
    //var isPFAuthGridLoaded = false;
    //if (isPFAuthGridLoaded) {
    //    $("#tblPFAuthList").GridUnload();
    //    isPFAuthGridLoaded = false;
    //}

    jQuery("#tblPFAuthList").jqGrid({
        url: '/Accounts/GetPFAuthorizationList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Authorization Number', 'Authorization Date', 'Date Of Operation', 'Amount (In Rs.)', 'Action'],
        colModel: [
                            { name: 'AuthNumber', index: 'AuthNumber', width: 100, align: 'left', sortable: true, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } },
                            { name: 'AuthDate', index: 'AuthDate', width: 80, align: 'left', sortable: true, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } },
                            { name: 'OpDate', index: 'OpDate', width: 90, align: 'left', sortable: true },
                            { name: 'Amount', index: 'Amount', width: 90, align: 'right', sortable: true },
                            { name: 'Action', index: 'Action', width: 50, align: 'center' },
        ],
        pager: jQuery('#divPFAuthPager'),
        rowNum: 5,
        //rowList: [5],
        pginput: false,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'AuthDate',
        sortorder: "asc",
        caption: "Authorization Details",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        loadComplete: function () {

            $("#gview_tblPFAuthList > .ui-jqgrid-titlebar").hide();

            $("#tblPFAuthList").jqGrid('setLabel', "rn", "Sr.</br> No");

            isPFAuthGridLoaded = true;
            $('a.ui-qtp-dig').qtip({
                content: {
                    text: $('a.ui-qtp-dig').attr('title'),
                    title: {
                        text: 'Authorization request rejected', // Give the tooltip a title using each elements text
                        button: true
                    }
                },
                position: {
                    at: 'bottom center', // Position the tooltip above the link
                    my: 'top center',
                    viewport: $(window), // Keep the tooltip on-screen at all times
                    effect: false // Disable positioning animation
                },
                show: {
                    event: 'click',
                    solo: true // Only show one tooltip at a time
                },
                hide: 'unfocus',
                style: {
                    classes: 'ui-state-default'
                }
            });

            $('a.ui-qtp-dig-bill-details').qtip({
                content: {
                    text: $('a.ui-qtp-dig-bill-details').attr('title'),
                    title: {
                        text: 'Bill Details', // Give the tooltip a title using each elements text
                        button: true
                    }
                },
                position: {
                    at: 'bottom center', // Position the tooltip above the link
                    my: 'top left',
                    viewport: $(window), // Keep the tooltip on-screen at all times
                    effect: false // Disable positioning animation
                },
                show: {
                    event: 'click',
                    solo: true // Only show one tooltip at a time
                },
                hide: 'unfocus',
                style: {
                    classes: 'ui-state-default'
                }
            });


            $('a.ui-qtp-dig-receipt-details').qtip({
                content: {
                    text: $('a.ui-qtp-dig-receipt-details').attr('title'),
                    title: {
                        text: 'Receipt Details', // Give the tooltip a title using each elements text
                        button: true
                    }
                },
                position: {
                    at: 'bottom center', // Position the tooltip above the link
                    my: 'top left',
                    viewport: $(window), // Keep the tooltip on-screen at all times
                    effect: false // Disable positioning animation
                },
                show: {
                    event: 'click',
                    solo: true // Only show one tooltip at a time
                },
                hide: 'unfocus',
                style: {
                    classes: 'ui-state-default'
                }
            });

            $('span.ui-qtp-dig').qtip({
                content: {
                    text: $('span.ui-qtp-dig').attr('title'),
                    title: {
                        text: 'Authorization Amount Details', // Give the tooltip a title using each elements text
                        button: true
                    }
                },
                position: {
                    at: 'bottom center', // Position the tooltip above the link
                    my: 'top center',
                    viewport: $(window), // Keep the tooltip on-screen at all times
                    effect: false // Disable positioning animation
                },
                show: {
                    event: 'click',
                    solo: true // Only show one tooltip at a time
                },
                hide: 'unfocus',
                style: {
                    classes: 'ui-state-default'
                }
            });


            //if ($('#tblOBList').jqGrid('getGridParam', 'reccount') > 0) {
            //    $("#AddOBMaster").hide();
            //}
            //$("#divOBListPager_right").html("<span style='float:right'>Status represents <font color='green'>OB Details Entered</font> and <font color='#b83400'>OB Details Remained</font> Amount</span><span style='float:right' class='ui-icon ui-icon-info'></span>");
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }

    });//end of documents grid
}

function LoadlettestTransactionGrid() {
    
    jQuery("#tblLettestTransactionList").jqGrid({
        url: '/Accounts/GetLettestTransactionsList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Voucher Number', 'Voucher Date', 'Transaction Type', 'Cheque Number','Payee Name','Gross Amount (In Rs.)'],
        colModel: [
                            { name: 'VoucherNumber', index: 'VoucherNumber', width: 40, align: 'left'},
                            { name: 'VoucherDate', index: 'VoucherDate', width: 60, align: 'left' },
                            { name: 'TransactionType', index: 'TransactionType', width: 150, align: 'left' },
                            { name: 'ChequeNumber', index: 'ChequeNumber', width: 50, align: 'left' },
                            { name: 'PayeeName', index: 'PayeeName', width: 80, align: 'left' },
                            { name: 'GrossAmount', index: 'GrossAmount', width: 60, align: 'right' }
        ],
        pager: jQuery('#divLettestTransactionPager'),
        rowNum: 5,
        //rowList: [5],
        pginput: false,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'AuthDate',
        sortorder: "asc",
        caption: "Lettest Transaction Details",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        loadComplete: function () {

            $("#gview_tblLettestTransactionList > .ui-jqgrid-titlebar").hide();
            $("#tblLettestTransactionList").jqGrid('setLabel', "rn", "Sr.</br> No");
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }

    });//end of documents grid
}



function LoadLatestDPIUTransactionDetails() {

    jQuery("#tblTransactionDetailsList").jqGrid({
        url: '/Accounts/GetLatestDPIUTransactionsDetailsList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Bill Number', 'Bill Date', 'Cheque E-Pay Number', 'Transaction Type', 'Amount (In Rs.)', 'DPIU Name'],
        colModel: [
                            { name: 'BillNumber', index: 'BillNumber', width: 40, align: 'left' },
                            { name: 'BillDate', index: 'BillDate', width: 60, align: 'left' },
                            { name: 'ChequeNumber', index: 'ChequeNumber', width: 80, align: 'left', sortable:false },
                            { name: 'TransactionType', index: 'TransactionType', width: 120, align: 'left', sortable:false },
                            { name: 'Amount', index: 'Amount', width: 60, align: 'right' },
                            { name: 'DPIUName', index: 'DPIUName', width: 80, align: 'left' }
                            
        ],
        pager: jQuery('#divTransactionDetailsPager'),
        rowNum: 5,
        //rowList: [5],
        pginput: false,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'BillDate',
        sortorder: "desc",
        caption: "Latest Transaction Details",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        loadComplete: function () {

            $("#gview_tblTransactionDetailsList  > .ui-jqgrid-titlebar").hide();
            $("#tblTransactionDetailsList").jqGrid('setLabel', "rn", "Sr.</br> No");
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }

    });//end of documents grid
}



function LoadDPIUAutorizationIssuedDetails() {

    jQuery("#tblAuthIssuedList").jqGrid({
        url: '/Accounts/GetDPIUAutherizationIssuedDetailsList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Reference Number', 'Bill Date', 'Transaction Type', 'Amount (In Rs.)'],
        colModel: [
                            { name: 'BillNumber', index: 'BillNumber', width: 50, align: 'left' },
                            { name: 'BillDate', index: 'BillDate', width: 60, align: 'left' },                           
                            { name: 'TransactionType', index: 'TransactionType', width: 120, align: 'left', sortable: false },
                            { name: 'Amount', index: 'Amount', width: 60, align: 'right' },
                          //  { name: 'BillDate1', index: 'BillDate1', width: 80, align: 'left' }

        ],
        pager: jQuery('#divAuthIssuedPager'),
        rowNum: 5,
        //rowList: [5],
        pginput: false,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'BillDate',
        sortorder: "desc",
       // caption: "Latest Transaction Details",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        loadComplete: function () {

            $("#gview_tblAuthIssuedList  > .ui-jqgrid-titlebar").hide();
            $("#tblAuthIssuedList").jqGrid('setLabel', "rn", "Sr.</br> No");
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }

    });//end of documents grid
}

function AddAuthDetails(urlParam) {
    
    $("#DigReceiptPayment").load("/Authorization/AddReceiptPaymentDetails/" + urlParam, function () {
        $("#DigReceiptPayment").show();
        $("#DigReceiptPayment").dialog('open');
    });
}

function ViewAuthDetails(urlParam) {
    $("#DigReceiptPayment").load("/Authorization/ViewReceiptPaymentDetails/" + urlParam, function () {
        $("#DigReceiptPayment").show();
        $("#DigReceiptPayment").dialog('open');
    });
}

function tick2() {

    $('#ticker_02 li:first').slideUp(function () { $(this).appendTo($('#ticker_02')).slideDown(); });

}


function startTimer() {
    autoInterval = setInterval(function () { tick2() }, 3000);
}