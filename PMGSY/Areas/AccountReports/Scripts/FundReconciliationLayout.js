//added by abhishek kamble 3-dec-2013
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

    $(function () {

        $("#Piu").hide();
        $("#spanDPIU").hide();


    });

    $("#btnView").click(function () {

        var data;
        var reportLevel = 4;

        //added by abhishek kamble 
        if ($("#rdbMonth").is(":checked")) {
            if (($("#Month option:selected").val() == 0)) {
                return false;
            }
        }
        if (($("#Year option:selected").val() == 0)) {
            return false;
        }

        $("#lblAgency").text($("#Agency option:selected").text());

        if ($("#rdbMonth").is(":checked")) {
            $(function () {

                $("#tdlblYear").hide();
                $("#tdlblMonthYear").show();

                $("#lblMonthYear").text(($("#Month option:selected").text()) + '-' + ($("#Year option:selected").text()));

            });
        }
        if ($("#rdbAnnual").is(":checked")) {
            $(function () {
                $("#tdlblMonthYear").hide();
                $("#tdlblYear").show();

                $("#lblYear").text($("#Year option:selected").text());
            });
        }

        if ($("#rdbAnnual").is(":checked")) {
            data = { srrdaNDCode: $("#Agency option:selected").val(), month: 0, year: $("#Year option:selected").val(), rlevel: reportLevel, NodalAgency: $("#Agency option:selected").text() }
            loadReport(data);
        }

        if ($("#rdbMonth").is(":checked")) {
            data = { srrdaNDCode: $("#Agency option:selected").val(), month: $("#Month option:selected").val(), year: $("#Year option:selected").val(), rlevel: reportLevel, NodalAgency: $("#Agency option:selected").text() }
            loadReport(data);
        }
    });

    $('#btnView').trigger('click');

    $("#Month").change(function () {

        UpdateAccountSession($("#Month").val(), $("#Year").val());

    });

    $("#Year").change(function () {

        UpdateAccountSession($("#Month").val(), $("#Year").val());

    });

});

function loadReport(param) {


    $.ajax({
        url: "/AccountReports/Account/FundReconciliationReport/",
        type: "POST",
        data: param,
        success: function (data) {
            $("#dvScheduleFundRecon").html(data);
        }
    });
}

function loadReconciliationGrid(data) {
    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#tbScheduleFundRecon").jqGrid("GridUnload");
    $("#tbScheduleFundRecon").jqGrid({

        url: '/Reports/GetFundReconciliationDetails',
        datatype: 'json',
        mtype: 'POST',
        colNames: ["Sr", "Particulars", "Is Amt", "Amount (In Rs.)", "Amount (In Rs.)", ],
        colModel: [
             { name: 'Sr', index: 'Sr', width: 400, sortable: false, align: "left", hidden: true },
             { name: 'Particulars', index: 'Particulars', width: 700, sortable: false, align: "left" },
             { name: 'IsAmt', index: 'IsAmt', width: 400, sortable: false, align: "left", hidden: true },
             { name: 'InnerAmt', index: 'InnerAmt', width: 200, sortable: false, align: "right" },
             { name: 'OuterAmt', index: 'OuterAmt', width: 200, sortable: false, align: "right", editable: true },


        ],
        rowNum: 1020,
        viewrecords: true,
        postData: data,
        caption: "&nbsp;&nbsp;Schedule of Fund Reconciliation Statement Between PIU & SRRDA",
        height: 'auto',
        //  maxheight: 100,
        shrinktofit: false,
        width: 'auto',
        toppager: true,
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
        loadComplete: function (jsonData) {
            $("#tblHeading").show();

            $("#lblHeaderFormNumber").text(jsonData.reportHeader.formNumber);
            $("#lblHeaderFundType").text(jsonData.reportHeader.fundType);
            $("#lblReportHeader").text(jsonData.reportHeader.reportHeading);
            $("#lblReportReference").text(jsonData.reportHeader.refference);

            //$("#tbScheduleRemittanceList").jqGrid("setLabel", "Desc", jsonData.reportHeader.scheduleNo);
            $("#jqgh_tbScheduleFundRecon_rn").html("Line <br/> No.");
            // jQuery("#tbScheduleFundRecon").jqGrid('setCell', '1', 'InnerAmt', "", { 'value': '' });

            var fundName;
            switch ($("#FundType").val()) {
                case "P":
                    fundName = "Programme Fund";
                    break;
                case "M":
                    fundName = "Maintainance Fund";
                    break;
                case "A":
                    fundName = "Administrative Fund";
                    break;
                default:
                    fundName = "Programme Fund";
                    break;
            }


            var rowData = $('#tbScheduleFundRecon').jqGrid('getRowData', '1');


            rowData.Particulars = rowData.Particulars + "" + fundName;

            if (rowData.IsAmt == "N") {
                rowData.InnerAmt = '';

                rowData.OuterAmt = '';
                $('#tbScheduleFundRecon').jqGrid('setRowData', '1', rowData);
            }

            var rowData = $('#tbScheduleFundRecon').jqGrid('getRowData', '5');
            if (rowData.IsAmt == "N") {
                rowData.InnerAmt = '';

                rowData.OuterAmt = '';
                $('#tbScheduleFundRecon').jqGrid('setRowData', '5', rowData);
            }


            var allRowInGrid = jQuery("#tbScheduleFundRecon").jqGrid("getDataIDs");
            for (i = 1; i < 4; i++) {
                var rowId = allRowInGrid[i];
                var rowData = $('#tbScheduleFundRecon').jqGrid('getRowData', rowId);

                if (i == 1 || i == 2) {
                    rowData.OuterAmt = "";
                }
                else {

                    if (rowData.OuterAmt == "") {
                        rowData.OuterAmt = "0.00";
                    } else {
                        rowData.OuterAmt = rowData.OuterAmt.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "" + ".00";
                    }
                }

                if (i == 3) {
                    rowData.InnerAmt = "";
                }
                else {
                    if (rowData.InnerAmt == "") {
                        rowData.InnerAmt = "0.00";
                    } else {
                        rowData.InnerAmt = rowData.InnerAmt.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "" + ".00";
                    }
                }

                //rowData.InnerAmt = rowData.InnerAmt + "" + ".00";

                $('#tbScheduleFundRecon').jqGrid('setRowData', rowId, rowData);

            }

            for (i = 5; i < 8; i++) {
                var rowId = allRowInGrid[i];
                var rowData = $('#tbScheduleFundRecon').jqGrid('getRowData', rowId);

                //modified by abhishek kamble 4-dec-2013
                //rowData.InnerAmt = rowData.InnerAmt + "" + ".00";
                //rowData.OuterAmt = rowData.OuterAmt + "" + ".00";

                if (i == 5 || i == 6) {
                    rowData.OuterAmt = "";
                }
                else {
                    if (rowData.OuterAmt == "") {
                        rowData.OuterAmt = "0.00";
                    } else {
                        rowData.OuterAmt = rowData.OuterAmt.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "" + ".00";
                    }
                }

                if (i == 7) {
                    rowData.InnerAmt = "";
                }
                else {
                    if (rowData.InnerAmt == "") {
                        rowData.InnerAmt = "0.00";
                    } else {
                        rowData.InnerAmt = rowData.InnerAmt.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "" + ".00";
                    }
                }

                $('#tbScheduleFundRecon').jqGrid('setRowData', rowId, rowData);

            }



            jQuery("#tbScheduleFundRecon").jqGrid('setCell', '1', 'Particulars', "", { 'font-weight': 'bold' });

            jQuery("#tbScheduleFundRecon").jqGrid('setCell', '5', 'Particulars', "", { 'font-weight': 'bold' });

            jQuery("#tbScheduleFundRecon").jqGrid('setCell', '4', 'InnerAmt', "", { 'font-weight': 'bold' });

            jQuery("#tbScheduleFundRecon").jqGrid('setCell', '4', 'OuterAmt', "", { 'font-weight': 'bold' });

            jQuery("#tbScheduleFundRecon").jqGrid('setCell', '8', 'InnerAmt', "", { 'font-weight': 'bold' });

            jQuery("#tbScheduleFundRecon").jqGrid('setCell', '8', 'OuterAmt', "", { 'font-weight': 'bold' });

            var topPagerDiv = $("#pg_tbScheduleFundRecon_toppager")[0];
            $("#tbScheduleFundRecon_toppager_center", topPagerDiv).remove();
            $("#tbScheduleFundRecon_toppager_right", topPagerDiv).remove();
            $("#tbScheduleFundRecon_toppager_left").html("<b>" + "Schedule No:" + jsonData.reportHeader.scheduleNo + "</b>");

            $.unblockUI();

        },
        loadError: function (xhr, status, error) {
            unblockPage();
            alert("Error");
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }

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