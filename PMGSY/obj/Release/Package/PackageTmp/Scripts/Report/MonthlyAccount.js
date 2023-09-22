
jQuery.validator.addMethod("dpiuvalidator", function (value, element, param) {

   // var IsMonthly = $('#rdbMonth').val();
    var DPIU = $('#Dpiu').val();

    if ($("#rdoDpiu").is(":checked")) {
        if (DPIU == 0) {
            return false;
        }
        else {
            return true;
        }
    } else {
        return true;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("dpiuvalidator");

$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmMonthlyAccount");

    //Added By Abhishek kamble 30-jan-2014 start
    $(function () {        
        $("#State").trigger("change");

        if ($("#CreditDebit").val() != "D") {
            $("#CreditDebit option:nth(1)").attr('selected', 'selected');
        }

    });
    //Added By Abhishek kamble 30-jan-2014 end

    if ($("#State").is(":disabled"))
    {   
        $("#State").trigger("change");
    }

    //added by abhishek kamble start 12-9-2013
    if ($("#rdoDpiu").is(":checked"))
    {
        $("#Dpiu").show();
        $("#lblSelectDpiu").show();

        //$("#rdoDpiu").trigger("click");
    }

    $("#spnMonthlyStateSRRDA").click(function () {
        $("#spnMonthlyStateSRRDA").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triagle-n");
        $("#dvMonthlyAccountSearch").slideToggle("slow");
    });


    $("#spnMonthlyDetailsStateSRRDA").click(function () {
        $("#spnMonthlyDetailsStateSRRDA").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#tblMonthlyAccount_wrapper").slideToggle("slow");

    });

    $("#spnMonthlyDetailsDPIU").click(function () {
        
        $("#spnMonthlyDetailsDPIU").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");

        $("#tblMonthlyAccountForPIU_wrapper").slideToggle("slow");

        $("#tblMonthlyAccount_wrapper").slideToggle("slow");
    });


    //added by abhishek kamble end 12-9-2013


    var oSingleCB = $('#tblMonthlyAccount').dataTable({
        "bJQueryUI": true,
        "bFilter": false,
        "bSort": false,
        "bHeader": true,
        "sScrollY": "320px",
        "bPaginate": false,
        "bScrollInfinite": true,
        "bScrollCollapse": true,
        "sDom": '<"H"Tfr>t<"F"ip>',
        "oTableTools": {
            "aButtons": [
				//{
				//    "sExtends": "pdf",
				//    "sPdfOrientation": "landscape",
				//    "sTitle": $("#rptMAAnnex").find('td:eq(1)').text(),
				//    "sButtonText": "<img src='../../content/images/Reader.gif' alt='pdf' style='margin-right:20px'></img>",
				//    //"sPdfMessage": pdfMessage,
				//    "sFileName": "MonthlyAccount" + $('#Month').children('option').filter(':selected').text() + "-" + $("#Year").val() + ".pdf"
				//},
                //{
                //    "sExtends": "xls",
                //    "bBomInc": true,
                //    "sButtonText": "<img src='../../content/images/Excel.gif' alt='Excel'></img>",
                //    "sFileName": "MonthlyAccount" + $('#Month').children('option').filter(':selected').text() + "-" + $("#Year").val() + ".xls"                    
                //}

            ]
        }
    });


    var oSingleCB = $('#tblMonthlyAccountForPIU').dataTable({
        "bJQueryUI": true,
        "bFilter": false,
        "bSort": false,
        "bHeader": true,
        "sScrollY": "300px",
        "bPaginate": false,
        "bScrollInfinite": true,
        "bScrollCollapse": true,
        "sDom": '<"H"Tfr>t<"F"ip>',
        "oTableTools": {
            "aButtons": [
				//{
				//    "sExtends": "pdf",
				//    "sPdfOrientation": "landscape",
				//    "sTitle": $("#rptMAAnnex").find('td:eq(1)').text(),
				//    "sButtonText": "<img src='../../content/images/Reader.gif' alt='pdf' style='margin-right:20px'></img>",
				//    //"sPdfMessage": pdfMessage,
				//    "sFileName": "MonthlyAccount" + $('#Month').children('option').filter(':selected').text() + "-" + $("#Year").val() + ".pdf"
				//},
                //{
                //    "sExtends": "xls",
                //    "bBomInc": true,
                //    "sButtonText": "<img src='../../content/images/Excel.gif' alt='Excel'></img>",
                //    "sFileName": "MonthlyAccount" + $('#Month').children('option').filter(':selected').text() + "-" + $("#Year").val() + ".xls"
                //}

            ]
        }
    });



    $("#btnView").click(function () {

        //$("#dvMonthlyAccountSearch").hide("slow");

        if ($("#Month").val() == "0")
        {
            //alert('Please Select Month');
            return false;
        }
        else if ($("#Year").val() == "0") {
            //alert('Please Select Year');
            return false;
        }
        else if ($("#CreditDebit").val() == "0") {
            //alert('Please Select Balance Type');
            return false;
        }

        if ($("#frmMonthlyAccount").valid()) { 
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        //else {

            if ($("#State").is(":disabled")) {
                $("#State").attr("disabled", false);
            }

            $("#MonthName").val($("#Month option:selected").html());
            $("#BalanceName").val($("#CreditDebit option:selected").html());
            //$("#State").val($("#StateSRRDAName option:selected").html());   

            //alert($("#Dpiu").val());
            //alert($("#NodalAgency").val());

            $.ajax({
                url: "/Reports/MonthlyAccount/",
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmMonthlyAccount").serialize(),
                success: function (data) {
                    //$("#mainDiv").html(data);
                    $("#ToolTables_tblMonthlyAccount_0").removeClass('DTTT_button').removeClass('ui-state-default').removeClass('DTTT_button_collection');
                    $("#ToolTables_tblMonthlyAccount_1").removeClass('DTTT_button').removeClass('ui-state-default').removeClass('DTTT_button_collection');
                    $("#tblMonthlyAccount_info").css('text-align', 'left');
                    $("#tblMonthlyAccountForPIU_info").css('text-align', 'left');

                    //$("#mainDiv").html(data);
                    $("#dvShowMonthlyAccountDetails").html(data);

                    $("#dvMonthlyAccountDetails").show();

                    if ($("#State").is(":disabled")) {
                        $("#State").attr("disabled", true);
                    }

                    $("#spnMonthlyStateSRRDA").trigger("click");

                    $.unblockUI();

                },
                error: function (data) {
                    if ($("#State").is(":disabled")) {
                        $("#State").attr("disabled", true);
                    }
                    $.unblockUI();
                }
            });
        }
    });


    //STATE ddl change
    $("#State").change(function () {

        var adminNdCode = $("#State option:selected").val();

        $.ajax({
            type: 'POST',
            url: '/Reports/PopulateDPIU?id=' + adminNdCode,
            async: false,
            cache: false,
            success: function (data) {
                $("#Dpiu").empty();
                $.each(data, function () {
                    $("#Dpiu").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            },
            error: function () {
                alert("Request can not be processed at this time.");
            }, complete: function () {
                $("#Dpiu").val($("#DPIUId").val());

                
                //setTimeout(function () {
                //    if ($("#frmMonthlyAccount").valid()) {
                //        alert("valid");

                //        $("#btnView").trigger("click");
                //    } else {
                //        alert("Invalid");
                //    }
                //    }, 1000);
            }
        });
    });

    //SRRDA ddl change
    //$("#Srrda").change(function () {

    //    var adminNdCode = $("#Srrda option:selected").val();

    //    $.ajax({
    //        type: 'POST',
    //        url: '/Reports/PopulateDPIU?id=' + adminNdCode,
    //        async: false,
    //        cache: false,
    //        success: function (data) {
    //            $("#Dpiu").empty();
    //            $.each(data, function () {
    //                $("#Dpiu").append("<option value=" + this.Value + ">" + this.Text + "</option>");
    //            });
    //        },
    //        error: function () {
    //            alert("Request can not be processed at this time.");
    //        }
    //    });
    //});
    
    $("#rdoDpiu").click(function () {

        $("#State").trigger("change");
        $("#Dpiu").show();
        $("#lblSelectDpiu").show();
    });

    $("#rdoState").click(function () {
        $("#Dpiu").hide();
        $("#lblSelectDpiu").hide();
    });

    $("#rdoSrrda").click(function () {
        $("#Dpiu").hide();
        $("#lblSelectDpiu").hide();
    });


    $("#Month").change(function () {

        UpdateAccountSession($("#Month").val(), $("#Year").val());

    });

    $("#Year").change(function () {

        UpdateAccountSession($("#Month").val(), $("#Year").val());

    });


});


//function ValidateForm()
//{
//    if ($("#rdoDpiu").is(":checked"))
//    {     
//        if ($("#Dpiu").val()=="0")
//        {
//            alert("Please select DPIU");
//            return false;
//        }
//    }
//    return true;
//}

function ShowMonthlyLedgerDetails(HeadCode,ReportType)
{
    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
 
    //if (($("#SelectedReportType").val() == "STATE") && ReportType == "D")
    //{
    //    $("#SelectedReportType").val("DPIU");
    //}
    //$("#HEAD").val(HeadCode);

    //if (($("#SRRDA_DPIU").val() == "STATE") || ($("#SRRDA_DPIU").val() == "SRRDA"))
    //{
    //    $("#SRRDA_DPIU").val("S");
    //} else if (($("#SRRDA_DPIU").val() == "DPIU")) {
    //    $("#SRRDA_DPIU").val("D");
    //}

    //$.ajax({
    //    type: 'GET',
    //    url: '/Reports/Ledger/',
    //    async: false,
    //    data: $("#frmShowMonthlyLedger").serialize(),
    //    success: function (response) {
    //        $("#dvShowMonthlyLedgerDetails").html(response);
    //    },
    //    error: function (xhr, ajaxOptions, status) {
    //        alert(xhr.responseText);
    //        $.unblockUI();
    //    }
    //});//end of Ajax
    //$.unblockUI();
}
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