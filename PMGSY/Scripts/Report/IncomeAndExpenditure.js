

$(document).ready(function () {
    var data;

    if ($("#LevelId").val() == 4 || $("#LevelId").val() == 6)
    {
        $('#spanPIU').hide();
    }

    if ($("#LevelId").val() == 4 || $("#LevelId").val()==5)
    {
        $('#spanState').hide();
        $('#rdbSrrda').attr('checked',true);
    }


    $('#rdbSrrda').click(function () {
        $('#Piu option:first').attr('selected','selected');        
    });

    $('#rdbState').click(function () {
        $('#Piu option:first').attr('selected', 'selected');
    });

    $('#Year').change(function () {

        if ($('#divError').is(':visible'))
        {
            $('#divError').hide('slow');
            $('#divError').html('');
        }
        UpdateAccountSession($("#Month").val(), $("#Year").val());
    });

    $('#Month').change(function () {
        if ($('#divError').is(':visible')) {
            $('#divError').hide('slow');
            $('#divError').html('');
        }
        UpdateAccountSession($("#Month").val(), $("#Year").val());
    });



    $("#btnView").click(function () {

        if ($('#divError').is(':visible')) {
            $('#divError').hide('slow');
            $('#divError').html('');
        }
        $("#lblAgency").text($("#Agency option:selected").text());


        if (($('#Piu option:selected').val() == '0') && ($("#rdbPiu").is(":checked")))
        {
            $("#lblDPIU").text($("#Piu option:selected").text());
        } else if (($('#Piu option:selected').val() == '0') && !($("#rdbPiu").is(":checked")))
        {   
            $("#lblDPIU").text(' - ');
        }
        else {
            $("#lblDPIU").text($("#Piu option:selected").text());
        }

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

        // selection- state & Annual
        if ($("#rdbState").is(":checked") && $("#rdbAnnual").is(":checked")) {           
            if ($("#Piu option:selected").val() == 0) {
                data = { month: 1, Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 4, AllPiu: 0,duration:1};
            }
            else {
                data = { month: 1, Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 4, AllPiu: $("#Piu option:selected").val(),duration:1};
            }
            ValidateForm(data);
        }
        
        // selection- state & Monthly
        if ($("#rdbState").is(":checked") && $("#rdbMonth").is(":checked")) {                        
            if ($("#Piu option:selected").val() == 0) {
                data = { Month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndCode: $("#Agency option:selected").val(), rlevel: 4, allPiu: 0, duration: 2 };
            }
            else {
                data = { Month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndCode: $('#Agency option:selected').val(), rlevel: 4, allPiu: $("#Piu option:selected").val(), duration: 2 };
            }
            ValidateForm(data);
        }

        // selection- SRRDA & Annual
        if ($("#rdbSrrda").is(":checked") && $("#rdbAnnual").is(":checked")) {            
            data = { month: 1, Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 1, AllPiu: 0, duration: 1 };
                ValidateForm(data);
        }
        // selection- state & Monthly
        if ($("#rdbSrrda").is(":checked") && $("#rdbMonth").is(":checked")) {            
            data = { Month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndCode: $("#Agency option:selected").val(), rlevel: 1, allPiu: 0, duration: 2 };
            ValidateForm(data);
        }
        // selection- DPIU & Annual
        if ($("#rdbPiu").is(":checked") && $("#rdbAnnual").is(":checked")) {
            if ($("#Piu option:selected").val() == 0) {
                data = { month: 1, Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 2, AllPiu: 1, duration: 1 };
            }
            else {
                data = { month: 1, Year: $("#Year option:selected").val(), ndcode: $("#Piu option:selected").val(), rlevel: 2, AllPiu: 0, duration: 1 };
            }
            ValidateForm(data);
        }

        // selection- DPIU & Monthly
        if ($("#rdbPiu").is(":checked") && $("#rdbMonth").is(":checked")) {
            if ($("#Piu option:selected").val() == 0) {
                data = { month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 2, AllPiu: 1, duration: 2 };
            }
            else {
                data = { month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndcode: $("#Piu option:selected").val(), rlevel: 2, AllPiu: 0, duration: 2 };
            }
            ValidateForm(data);
        }

        // Login DPIU & selection- Annual
        if ($("#LevelId").val() == 5 && $("#rdbAnnual").is(":checked")) {           
            data = { month: 1, Year: $("#Year option:selected").val(), ndcode: $("#AdminNdCode").val(), rlevel: 2, AllPiu: 0, duration: 1 };
            ValidateForm(data);
        }

        // Login DPIU & selection- Monthly
        if ($("#LevelId").val() == 5 && $("#rdbMonth").is(":checked")) {           
            data = { month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndcode: $("#AdminNdCode").val(), rlevel: 2, AllPiu: 0, duration: 2 };
            ValidateForm(data);
        }
    });  
});


function ValidateForm(param) {
    $.ajax({
        url: "/Reports/ValidateParameter",
        type: "POST",
        data: param,
        success: function (data) {
            if (data.success == false) {                
                $("#divError").html('<strong>Alert : </strong>' + data.message);
                $("#divError").show("slow");
                return false;
            }
            else if (data.success == true) {
                loadGrid(param);
            }

        }
    });
}

function loadGrid(data) {
   
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    if ($("#rdbState").is(":checked")) {
        loadSTATE_Grid(data);
    } else {
        loadSRRDA_DPIU_Grid(data);
    }
   
}



function loadSRRDA_DPIU_Grid(data)
{
    jQuery('#tbIncomeAndExpenditureList').jqGrid("GridUnload");

    $("#tbIncomeAndExpenditureList").jqGrid({

        url: '/Reports/ListIncomeAndExpenditureDetails',
        datatype: 'json',
        mtype: 'POST',
        colNames: ["Line No.", "Head Name", "Opening Balance <br/>( Amount In Rs. )", "Current Balance <br/>( Amount In Rs. )", "Total <br/>( Amount In Rs. )"],
        colModel: [
             { name: 'LineNo', index: 'LineNo', width: '10%', sortable: false, align: "center" },
             { name: 'HEAD_NAME', index: 'HEAD_NAME', width: '50%', sortable: false, align: "left" },
             { name: 'OB', index: 'OB', width: '30%', sortable: false, align: "right" ,formatter:formatColumnOB},
             { name: 'CA', index: 'CA', width: '30%', sortable: false, align: "right",formatter: formatColumnCA },
             { name: 'Total', index: 'Total', width: '30%', sortable: false, align: "right", formatter: "number", formatoptions: { decimalPlaces: 2 } },
        ],

        rowNum: 9999,
        viewrecords: true,
        postData: data,
        caption: "&nbsp;&nbsp;Income And Expenditure Details",
        height: 'auto',
        shrinktofit: true,
        width: 1200,
        scroll:false,
        autowidth: false,
        rownumbers: false,
        loadComplete: function (jsonData) {
            var recCount = $("#tbIncomeAndExpenditureList").jqGrid("getGridParam", "reccount");

            //set grid height
            if (recCount > 20) {
                $("#tbIncomeAndExpenditureList").jqGrid("setGridHeight", "380");
                $("#tbIncomeAndExpenditureList").jqGrid("setGridWidth", 1200);
            }
            else {
                $("#tbIncomeAndExpenditureList").jqGrid("setGridHeight", "auto");
            }
            $("#tblHeading").show();

            //set header info
            $("#lblHeaderFormNumber").text(jsonData.reportHeader.formNumber);
            $("#lblHeaderFundType").text(jsonData.reportHeader.fundType);
            $("#lblReportHeader").text(jsonData.reportHeader.reportHeading);
            $("#lblReportReference").text(jsonData.reportHeader.refference);
            
            setGridCellFontToBold();

            $.unblockUI();

        },
        loadError: function (xhr, status, error) {
            unblockPage();
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

function loadSTATE_Grid(data) {
    jQuery('#tbIncomeAndExpenditureList').jqGrid("GridUnload");

    $("#tbIncomeAndExpenditureList").jqGrid({

        url: '/Reports/ListIncomeAndExpenditureDetails',
        datatype: 'json',
        mtype: 'POST',
        colNames: ["Line No.", "Head Name", "SRRDA Opening Balance <Br/>( Amount In Rs. ) ", "SRRDA Current Balance <Br/>( Amount In Rs. ) ", "DPIU Opening Balance <br/>( Amount In Rs. )", "DPIU Current Balance <br/>( Amount In Rs. )", "Total <br/>( Amount In Rs. )"],
        colModel: [
             { name: 'LineNo', index: 'LineNo', width: '10%', sortable: false, align: "center" },
             { name: 'HEAD_NAME', index: 'HEAD_NAME', width: '80%', sortable: false, align: "left" },
             { name: 'SRRDA_OB', index: 'OB', width: '35%', sortable: false, align: "right" ,formatter:formatColumnSRRDAOB},
             { name: 'SRRDA_CA', index: 'OB', width: '35%', sortable: false, align: "right", formatter: formatColumnSRRDACA },
             { name: 'OB', index: 'OB', width: '35%', sortable: false, align: "right", formatter: "number", formatter: formatColumnDPIUOB },
             { name: 'CA', index: 'CA', width: '35%', sortable: false, align: "right", formatter: "number", formatter: formatColumnDPIUCA },
             { name: 'Total', index: 'Total', width: '35%', sortable: false, align: "right", formatter: "number", formatoptions: { decimalPlaces: 2 } },
        ],

        rowNum: 9999,
        viewrecords: true,
        postData: data,
        caption: "&nbsp;&nbsp;Income And Expenditure Details",
        height: 'auto',        
        width: '96%',
        autowidth: true,
        rownumbers: false,
        loadComplete: function (jsonData) {

            var recCount = $("#tbIncomeAndExpenditureList").jqGrid("getGridParam", "reccount");

            if (recCount > 20) {
                $("#tbIncomeAndExpenditureList").jqGrid("setGridHeight", "380");
                $("#tbIncomeAndExpenditureList").jqGrid("setGridWidth", 1200);
            }
            else {
                $("#tbIncomeAndExpenditureList").jqGrid("setGridHeight", "auto");
            }
            $("#tblHeading").show();

            //set font bold            
            jQuery("#tbIncomeAndExpenditureList").jqGrid('setCell', 'Expenses', 'headName', "", { 'font-weight': 'bold', 'text-align': 'right' });
            jQuery("#tbIncomeAndExpenditureList").jqGrid('setCell', 'Income', 'headName', "", { 'font-weight': 'bold', 'text-align': 'right' });
            
            //set header info
            $("#lblHeaderFormNumber").text(jsonData.reportHeader.formNumber);
            $("#lblHeaderFundType").text(jsonData.reportHeader.fundType);
            $("#lblReportHeader").text(jsonData.reportHeader.reportHeading);
            $("#lblReportReference").text(jsonData.reportHeader.refference);

            setGridCellFontToBold();

            $.unblockUI();


        },
        loadError: function (xhr, status, error) {
            unblockPage();
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



function setGridCellFontToBold()
{
    
    var recCount = $("#tbIncomeAndExpenditureList").jqGrid("getGridParam", "reccount");
    //set grid cell font to bold
    var allRowInGrid = jQuery("#tbIncomeAndExpenditureList").jqGrid("getDataIDs");

    if ($("#rdbAnnual").is(":checked")) {

        var counter = 0;
        for (i = 0; i < recCount; i++) {


            var rowId = allRowInGrid[i];

            if (rowId == -1 || rowId == 54 || rowId == 55 || rowId == 60 || rowId == 131) {
                var rowData = $('#tbIncomeAndExpenditureList').jqGrid('getRowData', rowId);
                if (rowData.HEAD_NAME == "Income" || rowData.HEAD_NAME == "Expenses") {
                    rowData.HEAD_NAME = "<h4>" + rowData.HEAD_NAME + "</h4>";
                    rowData.SRRDA_OB = "";
                    rowData.SRRDA_CA = "";
                    rowData.OB = "";
                    rowData.CA = "";
                }
                var rowData = $('#tbIncomeAndExpenditureList').jqGrid('getRowData', rowId);
                rowData.HEAD_NAME = "<h4>" + rowData.HEAD_NAME + "</h4>";
                $('#tbIncomeAndExpenditureList').jqGrid('setRowData', rowId, rowData);

                counter = 0;
            }
            else {

                counter++;

                var rowData = $('#tbIncomeAndExpenditureList').jqGrid('getRowData', rowId);
                rowData.LineNo = counter;

                $('#tbIncomeAndExpenditureList').jqGrid('setRowData', rowId, rowData);
            }
        }
    } 
    else if ($("#rdbMonth").is(":checked")) {
      
        var counter = 0;
        for (i = 0; i < recCount; i++) {
            var rowId = allRowInGrid[i];
            if (rowId == -1 || rowId == 54 || rowId == 55 || rowId == 60 ||rowId == 131) {
                var rowData = $('#tbIncomeAndExpenditureList').jqGrid('getRowData', rowId);
                if (rowData.HEAD_NAME == "Income" || rowData.HEAD_NAME == "Expenses") {
                    rowData.HEAD_NAME = "<h4>" + rowData.HEAD_NAME + "</h4>";
                    rowData.SRRDA_OB = "";
                    rowData.SRRDA_CA = "";
                    rowData.OB = "";
                    rowData.CA = "";
                }
                rowData.HEAD_NAME = "<h4>" + rowData.HEAD_NAME + "</h4>";
                $('#tbIncomeAndExpenditureList').jqGrid('setRowData', rowId, rowData);

                counter = 0;
            }
            else {
                counter++;
                var rowData = $('#tbIncomeAndExpenditureList').jqGrid('getRowData', rowId);
                rowData.LineNo = counter;
                $('#tbIncomeAndExpenditureList').jqGrid('setRowData', rowId, rowData);
            }
        }
    }
}

function formatColumnSRRDAOB(cellvalue, options, rowObject)
{
    if (cellvalue == "")
    {
        return "0.00";
    
    }else{
        return cellvalue;
    }
}

function formatColumnSRRDACA(cellvalue, options, rowObject) {
    if (cellvalue == "") {
        return "0.00";

    } else {
        return cellvalue;
    }
}

function formatColumnDPIUOB(cellvalue, options, rowObject) {
    if (cellvalue == "") {
        return "0.00";

    } else {
        return cellvalue;
    }
}

function formatColumnDPIUCA(cellvalue, options, rowObject) {
    if (cellvalue == "") {
        return "0.00";

    } else {
        return cellvalue;
    }
}

function formatColumnOB(cellvalue, options, rowObject) {
    if (cellvalue == "") {
        return "0.00";

    } else {
        return cellvalue;
    }
}

function formatColumnCA(cellvalue, options, rowObject) {
    if (cellvalue == "") {
        return "0.00";

    } else {
        return cellvalue;
    }
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