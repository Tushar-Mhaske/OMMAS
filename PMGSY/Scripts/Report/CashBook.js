var pdfMessage = "";
$(document).ready(function () {
    
    $("#dvDetails").hide();

    //added by abhishek kamble 3-oct-2013 start
    if ($("#levelId").val() == 4) {
        $("#ddlSRRDA").val($("#AdminNdCode").val());
    }
    $(function () {
        if ($("#rdoSRRDA").is(":checked")) {
            $('#ddlSRRDA').trigger('change');
            $("#ddlDPIU").hide();
        }
    });

    $("#rdoSRRDA").click(function () {        
        $("#ddlDPIU").hide("slow");
        $("#ddlDPIU option:first").attr("selected","selected");
    });

    $("#rdoDPIU").click(function () {
        $("#ddlDPIU").show("slow");
    });


    $("#ddlSRRDA").change(function () {
        
        var adminNdCode = $('#ddlSRRDA option:selected').val();

        $.ajax({
            url: '/Reports/PopulateDPIUForCashBook/' + adminNdCode,
            type: 'GET',
            catche: false,
            error: function (xhr, status, error) {
                alert('An Error occured while processig your request.')
                return false;
            },
            success: function (data) {

                $('#ddlDPIU').empty();
                
                $.each(data, function () {
                    
                    $('#ddlDPIU').append("<option value="+this.Value+">"+this.Text+"</option>");

                });
            }
        });

    });

    //added by abhishek kamble 3-oct-2013 end

    //$('#tblCBPayment').fixheadertable({
    //    caption: 'My header is fixed !',
    //    height: 300,
    //    resizeCol : true,

    //});
    //var oTable = $('#tblCBPayment').dataTable();
    //new FixedHeader(oTable);
   var oPayCB = $('#tblCBPayment').dataTable({
        "bJQueryUI": true,
        "bFilter": false,
       //"sPaginationType": "full_numbers",
        "sScrollY": "250px",
         "bPaginate": false,
        "bScrollInfinite": true,
        "bScrollCollapse": true,
        //"bAutoWidth": false,
        "sDom": '<"H"Tfr>t<"F"ip>',
        "oTableTools": {
            "aButtons": [
				//{
				//    "sExtends": "pdf",
				//    "sPdfOrientation": "landscape",
                //    "sButtonText": "<img src='../../content/images/Reader.gif' alt='pdf' style='margin-right:20px'></img>",
				//    "sPdfMessage": "Your custom message would go here.",
				//    "sFileName": "CB-Payment" + $('#Month').children('option').filter(':selected').text() + "-" + $("#Year").val() + ".pdf"
				//},
                //{
                //    "sExtends": "xls",
                //    "bBomInc": true,
                //    "sButtonText": "<img src='../../content/images/Excel.gif' alt='Excel'></img>",
                //    "sFileName": "CB-Payment" + $('#Month').children('option').filter(':selected').text() + "-" + $("#Year").val() + ".xls"
                //    //"sButtonClass": "ui-icon ui-icon-circle-close"
                //}

            ]
        }
    });
    //$("#ToolTables_tblCBPayment_0").removeClass('ui-button').removeClass('DTTT_button').removeClass('ui-state-default').removeClass('DTTT_button_collection');
    //$("#ToolTables_tblCBPayment_1").removeClass('ui-button').removeClass('DTTT_button').removeClass('ui-state-default').removeClass('DTTT_button_collection');


    //$('#tblCBReceipt').fixheadertable({
    //    caption: 'My header is fixed !',
    //    height: 300
    //});
    //var oTable = $('#tblCBReceipt').dataTable();
    //new FixedHeader(oTable);
    var oRecCB = $('#tblCBReceipt').dataTable({
        "bJQueryUI": true,
        //"sPaginationType": "full_numbers",
        "bFilter": false,
        "sScrollY": "250px",
        "bPaginate": false,
        "bScrollInfinite": true,
        "bScrollCollapse": true,
        //"bAutoWidth": false,
        "sDom": '<"H"Tfr>t<"F"ip>',
        "oTableTools": {
            "aButtons": [
				//{
				//    "sExtends": "pdf",
				//    "sPdfOrientation": "landscape",
				//    "sButtonText": "<img src='../../content/images/Reader.gif' alt='pdf' style='margin-right:20px'></img>",
				//    "sPdfMessage": "Your custom message would go here.",
				//    "sFileName": "CB-Receipt" + $('#Month').children('option').filter(':selected').text() + "-" + $("#Year").val() + ".pdf"
				//},
                //{
                //    "sExtends": "xls",
                //    "bBomInc": true,
                //    "sButtonText": "<img src='../../content/images/Excel.gif' alt='Excel'></img>",
                //    "sFileName": "CB-Receipt" + $('#Month').children('option').filter(':selected').text() + "-" + $("#Year").val() + ".xls"
                //    //"sButtonClass": "ui-icon ui-icon-circle-close"
                //}

            ]
        }
    });
    //alert($("#ToolTables_tblCBReceipt_0").attr('class'));
    //$("#ToolTables_tblCBReceipt_0").removeClass('ui-button').removeClass('DTTT_button').removeClass('ui-state-default').removeClass('DTTT_button_collection');

    //var oTable = $('#example').dataTable();

    ///* Apply the tooltips */
    //oTable.$('tr').tooltip({
    //    "delay": 0,
    //    "track": true,
    //    "fade": 250
    //});

    pdfMessage = $("#rptCBAnnex").find('td:eq(0)').text() + ": " + $("#rptCBAnnex").find('td:eq(1)').find('span:eq(1)').text() + "\t\t\t" + $("#rptCBAnnex").find('td:eq(2)').text() + ": " + $("#rptCBAnnex").find('td:eq(3)').text();

    var oSingleCB = $('#tblSingleCB').dataTable({
        "bJQueryUI": true,
        "bFilter": false,
        //"sPaginationType": "full_numbers",
        "bSort": false,
        "bHeader": true,
        "sScrollY": "250px",
        "bPaginate": false,
        "bScrollInfinite": true,
        "bScrollCollapse": true,
        //"bAutoWidth": false,
        "sDom": '<"H"Tfr>t<"F"ip>',
        "oTableTools": {
            "aButtons": [
				//{
                //    "sExtends": "pdf",
                //    "sPdfOrientation": "landscape",
                //    "sTitle": $("#rptCBAnnex").find('td:eq(1)').find('span:eq(0)').text(),
                //    "sButtonText": "<img src='../../content/images/Reader.gif' alt='pdf' style='margin-right:20px'></img>",
                //    "sPdfMessage": pdfMessage,
                //    "sFileName": "Cashbook" + $('#Month').children('option').filter(':selected').text() + "-" + $("#Year").val() + ".pdf"
                //},
                //{
                //    "sExtends": "xls",
                //    "bBomInc": true,
                //    "sButtonText": "<img src='../../content/images/Excel.gif' alt='Excel'></img>",
                //    "sFileName": "Cashbook" + $('#Month').children('option').filter(':selected').text() + "-" + $("#Year").val() + ".xls"
                //    //"sButtonClass": "ui-icon ui-icon-circle-close"
                //}
                
            ]
        }
    });
    


    $('#pay_ico').click(function () {
        $("#singleCB").hide();
        $("#receiptCB").hide();
        $("#paymentCB").show();
        $("#ToolTables_tblCBPayment_0").removeClass('DTTT_button').removeClass('ui-state-default').removeClass('DTTT_button_collection');
        $("#ToolTables_tblCBPayment_1").removeClass('DTTT_button').removeClass('ui-state-default').removeClass('DTTT_button_collection');
        oPayCB.fnAdjustColumnSizing();
    });

    $('#rec_ico').click(function () {
        $("#singleCB").hide();
        $("#paymentCB").hide();
        $("#receiptCB").show();
        $("#ToolTables_tblCBReceipt_0").removeClass('DTTT_button').removeClass('ui-state-default').removeClass('DTTT_button_collection');
        $("#ToolTables_tblCBReceipt_1").removeClass('DTTT_button').removeClass('ui-state-default').removeClass('DTTT_button_collection');
        oRecCB.fnAdjustColumnSizing();
        
    });

    $('#singleCB_ico').click(function () {
        
        $("#receiptCB").hide();
        $("#paymentCB").hide();
        $("#singleCB").show();
        $("#ToolTables_tblSingleCB_0").removeClass('DTTT_button').removeClass('ui-state-default').removeClass('DTTT_button_collection');
        $("#ToolTables_tblSingleCB_1").removeClass('DTTT_button').removeClass('ui-state-default').removeClass('DTTT_button_collection');
        oSingleCB.fnAdjustColumnSizing();

    });   

    $("#btnView").click(function () {
        //blockPage();
        if($("#rdoDPIU").is(":checked"))
        {
            if ($("#ddlDPIU").val() == "0")
            {
                alert('Please Select DPIU');
                return false;
            }
        }

        if ($("#Month").val() == "0") {
            alert('Please Select Month');
            return false;
        }
        else if ($("#Year").val() == "0") {
            alert('Please Select Year');
            return false;
        }
       
        else {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            //blockPage();
            var DPIU = $("#ddlDPIU option:selected").val();
            
            if ($("#levelId").val() == 4) {
                $("#ddlSRRDA").attr("disabled", false);
            }

            $.ajax({
                url: "/Reports/CashBook/",
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmCashBook").serialize(),
                success: function (data) {
                    $("#mainDiv").html(data);
                    $('#singleCB_ico').trigger('click');
                    $("#cat_icon").show();

                    if ($("#levelId").val() == 4) {
                        $("#ddlSRRDA").attr("disabled", true);
                    }

                    //unblockPage();
                    $.unblockUI();
                },
                //complete: function (xhr, status) {

                //    if (status == "success") {
                //        $(function () {
                //            $("#ddlDPIU").val(DPIU);
                //        });
                //    }
                    
                //},
                error: function (xhr, status, error) {                    
                    if ($("#levelId").val() == 4) {
                        $("#ddlSRRDA").attr("disabled", true);
                    }
                    $.unblockUI();
                    //unblockPage();
                }
            });
            $("#dvDetails").show();
        } $.unblockUI();
    });  

    $("#Month").change(function () {

        UpdateAccountSession($("#Month").val(), $("#Year").val());

    });

    $("#Year").change(function () {

        UpdateAccountSession($("#Month").val(), $("#Year").val());

    });

});
function UpdateAccountSession(month, year) {
    $.ajax({
        url: "/Reports/UpdateAccountSession",
        type: "GET",
        async: false,
        cache: false,
        data:
            {
                "Month": month,
                "Year": year
            },
        success: function (data) {
            return false;
        },
        error: function () { }
    });
    return false;
}













































/*

$('#pay_ico').click(function () {

        if ($('#receiptside').is(':visible')) {
           
            $('#receiptside').toggle('slide', {
                direction: 'left'
            }, 500, function () { $('#receiptside').fadeOut(); });
            $('#paymentside').css('width', '98%').css('float', 'none');

            $('#paymentside').toggle('slide', {
                direction: 'right'
            }, 1000, function () { $('#paymentside').fadeIn(); });
           
        }
        else if ($('#paymentside').css('float')=='rigth')
        {
            $('#paymentside').css('width', '98%').css('float', 'none');
            $('#paymentside').toggle('slide', {
                direction: 'right'
            }, 1000, function () { $('#paymentside').fadeIn(); });
        }
    });

    $('#rec_ico').click(function () {
        if ($('#paymentside').is(':visible')) {
            $('#paymentside').toggle('slide', {
                direction: 'right'
            }, 500, function () { $('#paymentside').fadeOut(); });

            $('#receiptside').css('width', '98%').css('float', 'none');
           
            $('#receiptside').toggle('slide', {
                direction: 'left'
            }, 1000, function () { $('#receiptside').fadeIn(); });
        }
        else if ($('#receiptside').css('float') == 'left')
        {
            $('#receiptside').css('width', '98%').css('float', 'none');
            $('#receiptside').toggle('slide', {
                direction: 'left'
            }, 1000, function () { $('#receiptside').fadeIn(); });
        }
    });


    $('#singleCB_ico').click(function () {
        $('#receiptside').css('width', '50%').css('float', 'left');
        $('#paymentside').css('width', '50%').css('float', 'right');
       
        $('#receiptside').toggle('slide', {
            direction: 'left'
        }, 1000, function () { $('#receiptside').fadeIn(); });
        $('#paymentside').toggle('slide', {
            direction: 'right'
        }, 1000, function () { $('#paymentside').fadeIn(); });
    });

    $("#btnView").click(function () {
        $.ajax({
            url: "/Reports/CashBook/",
            type: "POST",
            async: false,
            cache: false,
            data: $("#frmCashBook").serialize(),
            success: function (data) {
                $("#mainDiv").html(data);
                $("#receiptCB").load("/Reports/ReceiptCashBook/" + $("#Month").val() + "/" + $("#Year").val(), function () {
                   
                });
                $("#paymentCB").load("/Reports/PaymentCashBook/" + $("#Month").val() + "/" + $("#Year").val(), function () {                   
                   
                });
                $('#singleCB_ico').trigger('click');
            }
        });       
    });


*/