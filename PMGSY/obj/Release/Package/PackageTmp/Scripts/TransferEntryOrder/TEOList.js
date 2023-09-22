var isGridLoaded = false;

$(document).ready(function () {
    //Added By Abhishek kamble 3-jan-2014 start change
    
    GetClosedMonthAndYear();
    LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), null, null, 0, 'view');
    isGridLoaded = true;

    $("#btnView").click(function () {
        LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), null, null, 0, 'view');
    });


    $("#txtFromDate").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonImageOnly: true,
        buttonText: 'From Date'
    });

    $("#txtToDate").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonImageOnly: true,
        buttonText: 'To Date'
    });

    $("#searchTEO").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#tblViewDetails").hide('slow');
        $("#tblSearch").show('slow');
        $.unblockUI();

    });

    $("#btnSearchCancel").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#tblSearch").hide('slow');
        $("#tblViewDetails").show('slow');
        $("#ddlMasterTrans").val("");
        $("#txtToDate").val("");
        $("#txtFromDate").val("");
        $.unblockUI();

    });

    $("#iconClose").click(function () {
        $("#btnSearchCancel").trigger('click');
    });

    $("#AddTEO").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        //$("#mainDiv").load("/TEO/TEOEntry/");
        $.ajax({
            url: "/TEO/TEOEntry/",
            type: "GET",
            async: false,
            cache: false,
            data:
                {
                    "Month": $("#ddlMonth").val(),
                    "Year": $("#ddlYear").val()
                },
            success: function (data) {
                $.unblockUI();

                $("#mainDiv").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
        return false;
    });

    $("#ImprestTEO").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/TEO/TEOImprest/",
            type: "GET",
            async: false,
            cache: false,
            data:
                {
                    "Month": $("#ddlMonth").val(),
                    "Year": $("#ddlYear").val()
                },
            success: function (data) {
                $.unblockUI();

                $("#mainDiv").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                $.unblockUI();

                alert(xhr.responseText);
            }
        });
      
        return false;
    });

    $("#btnSearch").click(function () {
        LoadGrid(0, 0, $("#txtFromDate").val(), $("#txtToDate").val(), $("#ddlMasterTrans").val(), 'search');
    });

    //new change done by Vikram for adding the changed month and year in session
    $("#ddlMonth").change(function () {
        UpdateAccountSession($("#ddlMonth").val(), $("#ddlYear").val());
    });

    $("#ddlYear").change(function () {
        UpdateAccountSession($("#ddlMonth").val(), $("#ddlYear").val());
    });

});

function EditTEO(urlParam) {
    //$("#mainDiv").load("/TEO/TEOEntry/" + urlParam);
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/TEO/TEOEntry/"+urlParam,
        type: "GET",
        async: false,
        cache: false,
        data:
            {
                "Month": $("#ddlMonth").val(),
                "Year": $("#ddlYear").val()
            },
        success: function (data) {
            $.unblockUI();

            $("#mainDiv").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();

        }
    });
   
    return false;
}

function DeleteTEO(urlParam, CreditDebitAmt) {

    

    var CrDrAmount = CreditDebitAmt.split('$');
    var varMsg = null;
    if (parseFloat(CrDrAmount[0]) > 0 || parseFloat(CrDrAmount[1] > 0)) {
        varMsg = "Details present. Are you sure to delete TEO?";        
    }
    else {
        varMsg = "Are you sure to Delete TEO?";
    }    
    if (confirm(varMsg)) {
        $.ajax({
            url: "/TEO/DeleteTEOMaster/" + urlParam,
            type: "POST",
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert("TEO deleted");
                    if ($("#tblViewDetails").is(":visible")) {
                        LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), null, null, 0, 'view');
                    }
                    else {
                        LoadGrid(0, 0, $("#txtFromDate").val(), $("#txtToDate").val(), $("#ddlMasterTrans").val(), 'search');
                    }
                    return false;
                }
                else {
                    alert(data.message);
                    return false;
                }
            }
        });
    }
    else {
        return false;
    }
}

function LockTEO(urlParam) {

    if (confirm("Are you sure to finalize TEO?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/TEO/FinalizeTEO/" + urlParam.split("$")[0],
            type: "POST",
            async: false,
            cache: false,
            success: function (data) {
                $.unblockUI();

                if (data.success) {
                    alert("TEO finalized");
                    if ($("#tblViewDetails").is(":visible")) {
                        LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), null, null, 0, 'view');
                    }
                    else {
                        LoadGrid(0, 0, $("#txtFromDate").val(), $("#txtToDate").val(), $("#ddlMasterTrans").val(), 'search');
                    }

                    //new change done by Abhishek kamble on 16-Oct-2014                    
                    if (levelId == 4 && parseInt(urlParam.split("$")[1])==1530) {
                        $.ajax({
                            url: '/TEO/AddAutoEntryTOB/' + urlParam.split("$")[0],
                            type: "POST",
                            async: false,
                            cache: false,
                            //data:
                            //    {
                            //        "Month": month,
                            //        "Year": year
                            //    },
                            success: function () {
                                unblockPage();
                                //$("#mainDiv").html(data);
                                return true;
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                unblockPage();
                                alert(xhr.responseText);
                            }
                        });
                    }
                    //end of change

                    return false;
                }
                else {
                    alert(data.message);
                    return false;
                }
            }
        });
        $.unblockUI();
    }
    else {
        return false;
    }
  
}

function LoadGrid(month, year, fromDate, toDate, transType, mode) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    if (isGridLoaded) {
        $("#tblTEOList").GridUnload();
        isGridLoaded = false;
    }

    jQuery("#tblTEOList").jqGrid({
        url: '/TEO/GetTEOList',
        datatype: "json",
        mtype: "POST",
        colNames: ['TEO Number', 'TEO Date', 'Transaction Name', 'Gross Amount', 'Details Amount', 'View', 'Edit', 'Delete','Status'],
        colModel: [
                            { name: 'TEONumber', index: 'ReceiptNumber', width: 80, align: 'center', sortable: true },
                            { name: 'TEODate', index: 'ReceiptDate', width: 80, align: 'center', sortable: true },
                            { name: 'TransactionName', index: 'TransactionName', width: 200, align: 'left', sortable: true },
                            { name: 'MasterAmount', index: 'MasterAmount', width: 80, align: 'right', sortable: true },
                            { name: 'DetailsAmount', index: 'DetailsAmount', width: 80, align: 'right', sortable: true, hidden: true },
                            { name: 'Finalize', index: 'Finalize', width: 70, align: 'center', sortable: false },
                            { name: 'Edit', index: 'Edit', width: 50, align: 'center', sortable: false },
                            { name: 'Delete', index: 'Delete', width: 50, align: 'center', sortable: false },
                            { name: 'Status', index: 'Status', width: 50, align: 'left', sortable: false }
        ],
        pager: jQuery('#divTEOListPager'),
        rowNum: 10,
        postData: {
            'month': month,
            'year': year,
            'fromdate': fromDate,
            'toDate': toDate,
            'transType': transType,
            'mode': mode
        },
        altRows: true,
        rowList: [10, 20, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'TEODate',
        sortorder: "desc",
        caption: "TEO Details",
        height: 'auto',
        //width: '1080px',
        autowidth: true,
        rownumbers: true,
        //hidegrid: false,
        loadComplete: function () {
            $.unblockUI();

            isGridLoaded = true;
            //$("#divTEOListPager_left").html("<span class='ui-state-default'>Note: <span class='ui-widget-content'>Status represents Credit and Debit Amount</span></span>")
            if ($('#tblTEOList').jqGrid('getGridParam', 'reccount') > 0) {
                $("#divTEOListPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> Status represents <font color='green'>Credit</font> and <font color='red'>Debit</font> Amount");
            }

            //Added By Abhishek Kamlbe 11-Nov-2013
            $('#tblTEOList_rn').html('Sr.<br/>No.');

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

    }); //end of documents grid
  
}

function ViewTEO(urlParam) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/TEO/TEOEntry/" + urlParam,
        type: "GET",
        async: false,
        cache: false,
        data:
            {
                "Month": $("#ddlMonth").val(),
                "Year": $("#ddlYear").val()
            },
        success: function (data) {
            $.unblockUI();

            $("#mainDiv").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();

        }
    });
    return false;
}

//new method added by Vikram on 01-Jan-2014
function UpdateAccountSession(month, year) {
    $.ajax({
        url: "/Receipt/UpdateAccountSession",
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

//Added By Abhishek kamble 3-jan-2014
//function to get the account  Close month and year
function GetClosedMonthAndYear() {
    blockPage();

    $.ajax({
        type: "POST",
        url: "/MonthlyClosing/GetClosedMonthandYear/",
        // async: false,

        error: function (xhr, status, error) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');

            return false;

        },
        success: function (data) {
            unblockPage();
            $('#divError').hide('slow');
            $('#errorSpan').html("");
            $('#errorSpan').hide();

            if (data.monthClosed) {
                $("#lblMonth").text(data.month);
                $("#lblYear").text(data.year);

                $("#TrMonthlyClosing").show('Slow');
                $("#AccountNotClosedTr").hide('Slow');
                return false;
            }
            else if (data.monthClosed == false) {
                $("#AccountNotClosedTr").show('Slow');
                $("#TrMonthlyClosing").hide('Slow');
                return false;
            }
            else {

                alert("Error While getting Monthly Closing Details");
                return false;
            }

        }
    });


}
