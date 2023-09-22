jQuery.validator.addMethod("monthvalidator", function (value, element, param) {

    var IsMonthly = $('#rdbMonth').val();
    var month = $('#Month').val();

    if ($("#rdbMonth").is(":checked")) {
        if (IsMonthly == 2 && month == 0) {
            return false;
        }
        else {
            return true;
        }
    } else {
        return true;
    }
});

jQuery.validator.unobtrusive.adapters.addBool("monthvalidator");
$(document).ready(function () {

    var data;

    //Added By Abhishek kamble 16-Apr-2014
    $(function () {
        $("#spanSrrda").hide();
    });

//    $("#spanPIU").show();
    //$(function () {

    //    $("#rdbState").attr("value", "2");
    //    $(".rdbClassReportLevel").trigger("change");
    //    $("#spanPIU").show();
    //    $("#rdbState").trigger("click");
    //});

    $("#rdbState").click(function () {
//        $("#rdbState").attr("value", "2");
        //$("#spanPIU").show();
        $("#spanPIU").hide();        
        $("#Piu").val(0);
    });

    $("#btnView").click(function () {
        var reportLevel = 2;
        $("#lblAgency").text($("#Agency option:selected").text());

        if ($("#rdbPiu").is(":checked")) {

            if ($("#Piu option:selected").text() == "Select Department") {
                $("#lblPIUName").text(" - ");
            }
            else {
                $("#lblDPIU").text($("#Piu option:selected").text());
            }
        }
        else {
            $("#lblDPIU").text(" - ");
        }

        //added by abhishek kamble 3-dec-2013
        if ($("#rdbMonth").is(":checked")) {
            if ($("#Month").val() == 0) {
                return false;
            }
        }
        if ($("#Year").val() == 0) {
            return false;
        }

        if ($("#rdbMonth").is(":checked")) {
            $(function () {
                //added by abhishek kamble 3-dec-2013
                if ($("#Month").val() == 0) {
                    return false;
                }

                $("#tdlblYear").hide();
                $("#tdlblMonthYear").show();

                $("#lblMonthYear").text(($("#Month option:selected").text()) + '-' + ($("#Year option:selected").text()));
            });
        }
        if ($("#rdbAnnual").is(":checked")) {
            $(function () {
                //added by abhishek kamble 3-dec-2013
                if ($("#Year").val() == 0) {
                    return false;
                }

                $("#tdlblMonthYear").hide();
                $("#tdlblYear").show();

                $("#lblYear").text($("#Year option:selected").text());
            });
        }


        if ($("#rdbState").is(":checked") && $("#rdbAnnual").is(":checked")) {
            if ($("#Piu option:selected").val() == 0) {
                data = { month: 0, Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 4, AllPiu: 1, DPIU:"-", NodalAgency: $("#Agency option:selected").text() };
            }
            else {
                data = { month: 0, Year: $("#Year option:selected").val(), ndcode: $("#Piu option:selected").val(), rlevel: 4, AllPiu: 0, DPIU: "-", NodalAgency: $("#Agency option:selected").text() };
            }
            ValidateForm(data);
        }

        if ($("#rdbState").is(":checked") && $("#rdbMonth").is(":checked")) {
            //var NdCode = $("#Agency option:selected").val();
            if ($("#Piu option:selected").val() == 0) {
                data = { Month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndCode: $("#Agency option:selected").val(), rlevel: 4, allPiu: 1, DPIU: "-", NodalAgency: $("#Agency option:selected").text() };
            }
            else {
                data = { Month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndCode: $("#Piu option:selected").val(), rlevel: 4, allPiu: 0, DPIU: "-", NodalAgency: $("#Agency option:selected").text() };
            }
            ValidateForm(data);
        }

        if ($("#rdbPiu").is(":checked") && $("#rdbAnnual").is(":checked")) {
            if ($("#Piu option:selected").val() == 0) {
                data = { month: 0, Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 2, AllPiu: 1, DPIU: $("#Piu option:selected").text(), NodalAgency: $("#Agency option:selected").text() };
            }
            else {
                data = { month: 0, Year: $("#Year option:selected").val(), ndcode: $("#Piu option:selected").val(), rlevel: 2, AllPiu: 0, DPIU: $("#Piu option:selected").text(), NodalAgency: $("#Agency option:selected").text() };
            }
            ValidateForm(data);
        }

        if ($("#rdbPiu").is(":checked") && $("#rdbMonth").is(":checked")) {
            if ($("#Piu option:selected").val() == 0) {
                data = { month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 2, AllPiu: 1, DPIU: $("#Piu option:selected").text(), NodalAgency: $("#Agency option:selected").text() };
            }
            else {
                data = { month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndcode: $("#Piu option:selected").val(), rlevel: 2, AllPiu: 0, DPIU: $("#Piu option:selected").text(), NodalAgency: $("#Agency option:selected").text() };
            }
            ValidateForm(data);
        }

        if ($("#LevelId").val() == 5 && $("#rdbAnnual").is(":checked")) {
            data = { month: 0, Year: $("#Year option:selected").val(), ndcode: $("#AdminNdCode").val(), rlevel: 2, AllPiu: 0 };
            ValidateForm(data);
        }

        if ($("#LevelId").val() == 5 && $("#rdbMonth").is(":checked")) {
            data = { month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndcode: $("#AdminNdCode").val(), rlevel: 2, AllPiu: 0 };
            ValidateForm(data);
        }
        // alert($("#Agency option:selected").val());
        // alert($("#Piu option:selected").val());
    });

    $('#btnView').trigger('click');

    $("#Month").change(function () {

        UpdateAccountSession($("#Month").val(), $("#Year").val());

    });

    $("#Year").change(function () {

        UpdateAccountSession($("#Month").val(), $("#Year").val());

    });


});

function ValidateForm(param) {

    $.ajax({
        url: "/Reports/ValidateParameter",
        type: "POST",
        data: param,
        success: function (data) {
            if (data.success == false) {
                $("#dvError").show();
                $("#dvError").html('<strong>Alert : </strong>' + data.message);

                return false;
            }
            else if (data.success == true) {
                //if ($("#rdbMonth").is(":checked")) {
                //    loadMonthlyGrid(param);
                //}
                //else {
                //    loadYearlyGrid(param);
                //}
                loadReport(param);
            }
        }
    });
}

function loadReport(param) {


    $.ajax({
        url: "/AccountReports/Account/GetUtilizationReport",
        type: "POST",
        data: param,
        success: function (data) {
            $("#dvScheduleUtilizationList").html(data);
        }
    });
}

function loadYearlyGrid(data) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    var month = $("#Month").val();
    var year = $("#Year").val();
    var yearPrev = "1999-2000";

    var isYear = $("#rdbAnnual").is(":checked");
    if (isYear) {
        if (year != 2000) {
            year = year - 1;
            yearPrev = $("#Year option[value=" + year + "]").text();
        }
    }


    jQuery("#tbScheduleUtilizationList").jqGrid("GridUnload");

    jQuery("#tbScheduleUtilizationList").jqGrid({
        url: "/Reports/GetUtilizationDetails",
        datatype: "json",
        mtype: "POST",
        //colNames: ["Description", "Upto " + $("#Year option:selected").text(), "Line No", "Upto " + yearPrev],
        colNames: ["Description", "Amount Inner Column", "Line No", "Amount Outer Column"],
        colModel: [

                    { name: 'Desc', index: 'Desc', width: 400, sortable: false, align: "left" },
                    { name: 'InnerCol', index: 'InnerCol', width: 200, sortable: false, align: "right", editable: true, formatter: "number", formatoptions: { decimalPlaces: 2 } },
                    { name: 'LineNo', index: 'LineNo', width: 400, sortable: false, align: "left", hidden: true },
                    { name: 'OuterCol', index: 'OuterCol', width: 200, sortable: false, align: "right", editable: true, formatter: "number", formatoptions: { decimalPlaces: 2 } }
        ],
        rowNum: 1020,
        viewrecords: true,
        postData: data,
        caption: "&nbsp;&nbsp;Schedule of Bank Authorization Utilization and Reconciliation",
        height: 'auto',
        //  maxheight: 100,
        shrinktofit: false,
        width: 'auto',
        autowidth: true,
        rownumbers: true,
        footerrow: true,
        userDataOnFooter: true,
        jsonReader: {
            repeatitems: false,
            total: "total",
            records: "records",
            page: "page",
            root: "rows",
            cell: "",
            id: "0"
        },
        loadComplete: function (jsondata) {
            $("#tblHeading").show();

            $("#lblHeaderFormNumber").text(jsondata.reportHeader.formNumber);
            $("#lblHeaderFundType").text(jsondata.reportHeader.fundType);
            $("#lblReportHeader").text(jsondata.reportHeader.reportHeading);
            $("#lblReportReference").text(jsondata.reportHeader.refference);

            //jQuery("#tbScheduleUtilizationList").jqGrid('setLabel', 'Desc', jsondata.reportHeader.scheduleNo);
            jQuery("#tbScheduleUtilizationList").jqGrid('setLabel', 'Desc', "Year: " + $("#Year option:selected").text());
            $("#jqgh_tbScheduleUtilizationList_rn").text("Line No.");

            //$(this).jqGrid('setRowData', 4, false, { 'font-weight': 'bold' });
            //var allRowInGrid= jQuery("#tbScheduleUtilizationList").jqGrid("getDataIDs");
            //for (i = 0; i < allRowInGrid.length; i++) {
            //   var rowId= allRowInGrid[i];
            //   if (rowId == "Total") {
            //       jQuery("#tbScheduleUtilizationList").jqGrid('setCell', 'Total',{ 'font-weight': 'bold' });
            //   }
            //}
            jQuery("#tbScheduleUtilizationList").jqGrid('setCell', 'Total', 'Desc', "", { 'font-weight': 'bold' });
            jQuery("#tbScheduleUtilizationList").jqGrid('setCell', 'Total', 'InnerCol', "", { 'font-weight': 'bold' });
            jQuery("#tbScheduleUtilizationList").jqGrid('setCell', 'Total', 'OuterCol', "", { 'font-weight': 'bold' });
            jQuery("#tbScheduleUtilizationList").jqGrid('setCell', 'Closing Balance of Bank Authorisation as per Cash Book (It should equal (Line No.4 – Line No.5))', 'Desc', "", { 'font-weight': 'bold' });
            jQuery("#tbScheduleUtilizationList").jqGrid('setCell', 'Closing Balance of Bank Authorisation as per Cash Book (It should equal (Line No.4 – Line No.5))', 'InnerCol', "", { 'font-weight': 'bold' });
            jQuery("#tbScheduleUtilizationList").jqGrid('setCell', 'Closing Balance of Bank Authorisation as per Cash Book (It should equal (Line No.4 – Line No.5))', 'OuterCol', "", { 'font-weight': 'bold' });

            $.unblockUI();
        },
        loadError: function (xhr, status, error) {
            //unblockPage();
            alert("Error");
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }
    }); //end of grid

    jQuery("#tbScheduleUtilizationList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
          { startColumnName: 'InnerCol', numberOfColumns: 3, titleText: 'Account of Bank Authorisations Received and Utilized' },
          //{ startColumnName: 'Debits', numberOfColumns: 2, titleText: 'Gross Transaction' }
        ]
    });


}

function loadMonthlyGrid(data) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    var month = $("#Month").val();
    var year = $("#Year").val();
    var yearPrev = "1999-2000";

    var isYear = $("#rdbAnnual").is(":checked");

    if (month == 1) {
        month = 12;
        yearPrev = year - 1;
    }
    else {
        yearPrev = year;
        month = month - 1;
    }
    jQuery("#tbScheduleUtilizationList").jqGrid("GridUnload");

    jQuery("#tbScheduleUtilizationList").jqGrid({
        url: "/Reports/GetUtilizationDetails",
        datatype: "json",
        mtype: "POST",
        //commented by abhishek kamble 11-dec-2013
        //colNames: ["Description", "Upto " + $("#Month option:selected").text() + "," + year, "Line No", "Upto " + $("#Month option[value=" + month + "]").text()+","+ yearPrev],
        colNames: ["Description", "Inner Column", "Line No", "Outer Column"],
        colModel: [

                    { name: 'Desc', index: 'Desc', width: 400, sortable: false, align: "left" },
                    { name: 'InnerCol', index: 'InnerCol', width: 200, sortable: false, align: "right", editable: true, formatter: "number", formatoptions: { decimalPlaces: 2 } },
                    { name: 'LineNo', index: 'LineNo', width: 400, sortable: false, align: "left", hidden: true },
                    { name: 'OuterCol', index: 'OuterCol', width: 200, sortable: false, align: "right", editable: true, formatter: "number", formatoptions: { decimalPlaces: 2 } }
        ],
        rowNum: 1020,
        viewrecords: true,
        postData: data,
        caption: "&nbsp;&nbsp;Schedule of Bank Authorization Utilization and Reconciliation",
        height: 'auto',
        //  maxheight: 100,
        shrinktofit: false,
        width: 'auto',
        autowidth: true,
        rownumbers: true,
        footerrow: true,
        userDataOnFooter: true,
        jsonReader: {
            repeatitems: false,
            total: "total",
            records: "records",
            page: "page",
            root: "rows",
            cell: "",
            id: "0"
        },
        loadComplete: function (jsondata) {
            $("#tblHeading").show();

            $("#lblHeaderFormNumber").text(jsondata.reportHeader.formNumber);
            $("#lblHeaderFundType").text(jsondata.reportHeader.fundType);
            $("#lblReportHeader").text(jsondata.reportHeader.reportHeading);
            $("#lblReportReference").text(jsondata.reportHeader.refference);
            //commented by abhishek kamble 11-dec-2013
            //jQuery("#tbScheduleUtilizationList").jqGrid('setLabel', 'Desc', jsondata.reportHeader.scheduleNo);
            jQuery("#tbScheduleUtilizationList").jqGrid('setLabel', 'Desc', "Month and Year : " + $("#Month option:selected").text() + " - " + year);
            $("#jqgh_tbScheduleUtilizationList_rn").text("Line No.");

            //$(this).jqGrid('setRowData', 4, false, { 'font-weight': 'bold' });
            //var allRowInGrid= jQuery("#tbScheduleUtilizationList").jqGrid("getDataIDs");
            //for (i = 0; i < allRowInGrid.length; i++) {
            //   var rowId= allRowInGrid[i];
            //   if (rowId == "Total") {
            //       jQuery("#tbScheduleUtilizationList").jqGrid('setCell', 'Total',{ 'font-weight': 'bold' });
            //   }
            //}
            jQuery("#tbScheduleUtilizationList").jqGrid('setCell', 'Total', 'Desc', "", { 'font-weight': 'bold' });
            jQuery("#tbScheduleUtilizationList").jqGrid('setCell', 'Total', 'InnerCol', "", { 'font-weight': 'bold' });
            jQuery("#tbScheduleUtilizationList").jqGrid('setCell', 'Total', 'OuterCol', "", { 'font-weight': 'bold' });
            jQuery("#tbScheduleUtilizationList").jqGrid('setCell', 'Closing Balance of Bank Authorisation as per Cash Book (It should equal (Line No.4 – Line No.5))', 'Desc', "", { 'font-weight': 'bold' });
            jQuery("#tbScheduleUtilizationList").jqGrid('setCell', 'Closing Balance of Bank Authorisation as per Cash Book (It should equal (Line No.4 – Line No.5))', 'InnerCol', "", { 'font-weight': 'bold' });
            jQuery("#tbScheduleUtilizationList").jqGrid('setCell', 'Closing Balance of Bank Authorisation as per Cash Book (It should equal (Line No.4 – Line No.5))', 'OuterCol', "", { 'font-weight': 'bold' });

            $.unblockUI();
        },
        loadError: function (xhr, status, error) {
            //unblockPage();
            alert("Error");
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }
    }); //end of grid

    jQuery("#tbScheduleUtilizationList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
          { startColumnName: 'InnerCol', numberOfColumns: 3, titleText: 'Account of Bank Authorisations Received and Utilized' },
          //{ startColumnName: 'Debits', numberOfColumns: 2, titleText: 'Gross Transaction' }
        ]
    });


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
