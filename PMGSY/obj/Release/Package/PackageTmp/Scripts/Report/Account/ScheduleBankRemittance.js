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

    $("#spanPIU").show();
    $("#rdbState").click(function () {
        $("#rdbState").attr("value", "2");        
        $("#spanPIU").show();      

    });   
    

    $("#btnView").click(function () {
        var reportLevel = 2;
        $("#lblAgency").text($("#Agency option:selected").text());

        //Modified by abhishek kamble 13-dec-2013
        if ($("#rdbPiu").is(":checked")) {

            if ($("#Piu option:selected").text() == "Select Department") {
                $("#lblDPIU").text(" - ");
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
            if (($("#Month option:selected").val() == "0")) {
                return false;
            }
        }
        if (($("#Year option:selected").val() == "0")) {
            return false;
        }

        if ($("#rdbMonth").is(":checked")) {
            $(function () {
                //added by abhishek kamble 3-dec-2013
                if (($("#Month option:selected").val() == "0")) {
                    return false;
                }
                if (($("#Year option:selected").val() == "0")) {
                    return false;
                }                
                $("#tdlblYear").hide();
                $("#tdlblMonthYear").show();
               
                $("#lblMonthYear").text(($("#Month option:selected").text()) + '-' + ($("#Year option:selected").text()));
            });
        }
        if ($("#rdbAnnual").is(":checked")) {
            $(function () {
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
                data = { month: 0, Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 4, AllPiu: 1 };
            }
            else {
                data = { month: 0, Year: $("#Year option:selected").val(), ndcode: $("#Piu option:selected").val(), rlevel: 4, AllPiu: 0 };
            }
            ValidateForm(data);
        }

        if ($("#rdbState").is(":checked") && $("#rdbMonth").is(":checked")) {            
            //var NdCode = $("#Agency option:selected").val();
            if ($("#Piu option:selected").val() == 0) {
                data = { Month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndCode: $("#Agency option:selected").val(), rlevel: 4, allPiu: 1 };
            }
            else {
                data = { Month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndCode: $("#Piu option:selected").val(), rlevel: 4, allPiu: 0 };
            }
            ValidateForm(data);
        }

        if ($("#rdbPiu").is(":checked") && $("#rdbAnnual").is(":checked")) {
            if ($("#Piu option:selected").val() == 0) {
                data = { month: 0, Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 2, AllPiu: 1 };
            }
            else {
                data = { month: 0, Year: $("#Year option:selected").val(), ndcode: $("#Piu option:selected").val(), rlevel: 2, AllPiu: 0 };
            }
            ValidateForm(data);
        }

        if ($("#rdbPiu").is(":checked") && $("#rdbMonth").is(":checked")) {
            if ($("#Piu option:selected").val() == 0) {
                data = { month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 2, AllPiu: 1 };
            }
            else {
                data = { month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndcode: $("#Piu option:selected").val(), rlevel: 2, AllPiu: 0 };
            }
            ValidateForm(data);
        }

        if ($("#LevelId").val() == 5 && $("#rdbAnnual").is(":checked")) {           
            data = { month: 0, Year: $("#Year option:selected").val(),ndcode:$("#AdminNdCode").val(),rlevel:2,AllPiu:0 };
            ValidateForm(data);
        }

        if ($("#LevelId").val() == 5 && $("#rdbMonth").is(":checked")) {           
            data = { month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndcode: $("#AdminNdCode").val(), rlevel: 2, AllPiu: 0 };
            ValidateForm(data);
        }
        // alert($("#Agency option:selected").val());
        // alert($("#Piu option:selected").val());
    });  
   

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
                loadGrid(param);
            }

        }
    });
}

function loadGrid(data) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    jQuery("#tbScheduleRemittanceList").jqGrid("GridUnload");
    $("#tbScheduleRemittanceList").jqGrid({

        url: '/Reports/GetBankRemittenceDetails',
        datatype: 'json',
        mtype: 'POST',
        colNames: ["Id","Description", "Current Month / Year","Previous Month / Year"],
        colModel: [
             { name: 'LineNo', index: 'LineNo', width: 400, sortable: false, align: "left", hidden: true },
             { name: 'Desc', index: 'Desc', width: 400, sortable: false, align: "left" },
             { name: 'This_Amt', index: 'This_Amt', width: 200, sortable: false, align: "right", editable: true, formatter: "number", formatoptions: { decimalPlaces: 2 } },
            
             { name: 'Prev_Amt', index: 'Prev_Amt', width: 200, sortable: false, align: "right", editable: true, formatter: "number", formatoptions: { decimalPlaces: 2 } }
        ],
        rowNum: 1020,
        viewrecords: true,
        postData: data,
        caption: "&nbsp;&nbsp;Schedule of Bank Remittance Reconciliation Statement",
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
        loadComplete: function (jsonData) {
            $("#tblHeading").show();

            $("#lblHeaderFormNumber").text(jsonData.reportHeader.formNumber);
            $("#lblHeaderFundType").text(jsonData.reportHeader.fundType);
            $("#lblReportHeader").text(jsonData.reportHeader.reportHeading);
            $("#lblReportReference").text(jsonData.reportHeader.refference);

            //commented by abhishek kamble 11-dec-2013
            //$("#tbScheduleRemittanceList").jqGrid("setLabel", "Desc", jsonData.reportHeader.scheduleNo);
            $("#tbScheduleRemittanceList_rn").html("Line <br/> No.");
           
            jQuery("#tbScheduleRemittanceList").jqGrid('setCell', '3', 'Desc', "", { 'font-weight': 'bold' });
            jQuery("#tbScheduleRemittanceList").jqGrid('setCell', '3', 'This_Amt', "", { 'font-weight': 'bold' });
            jQuery("#tbScheduleRemittanceList").jqGrid('setCell', '3', 'Prev_Amt', "", { 'font-weight': 'bold' });

            jQuery("#tbScheduleRemittanceList").jqGrid('setCell', '5', 'Desc', "", { 'font-weight': 'bold' });
            jQuery("#tbScheduleRemittanceList").jqGrid('setCell', '5', 'This_Amt', "", { 'font-weight': 'bold' });
            jQuery("#tbScheduleRemittanceList").jqGrid('setCell', '5', 'Prev_Amt', "", { 'font-weight': 'bold' });

            var month = $("#Month").val();
            var year = $("#Year").val();
            var yearPrev = "1999-2000";

            var isYear = $("#rdbAnnual").is(":checked");
            if (isYear) {
                if (year != 2000) {
                    year = year - 1;
                    yearPrev = $("#Year option[value=" + year + "]").text();
                }
                //Modified by abhishek kamble 11-dec-2013
                //jQuery("#tbScheduleRemittanceList").jqGrid('setLabel', 'This_Amt', "Upto " + $("#Year option:selected").text());
                //jQuery("#tbScheduleRemittanceList").jqGrid('setLabel', 'Prev_Amt', "Upto " + yearPrev);
                
                $("#tbScheduleRemittanceList").jqGrid("setLabel", "Desc", "Year : " + $("#Year option:selected").text());
            }
            else {

                if (month == 1) {
                    month = 12;
                    yearPrev = year - 1;
                }
                else {
                    yearPrev = year;
                    month = month - 1;
                }
                //modified by abhishek kamble 11-dec-2013
                //jQuery("#tbScheduleRemittanceList").jqGrid('setLabel', 'This_Amt', "Upto " + $("#Month option:selected").text() + ", " + year);
                //jQuery("#tbScheduleRemittanceList").jqGrid('setLabel', 'Prev_Amt', "Upto " + $("#Month option[value=" + month + "]").text() + ", " + yearPrev);
                
                $("#tbScheduleRemittanceList").jqGrid("setLabel", "Desc", "Month and Year : " +$("#Month option:selected").text() +" - "+ $("#Year option:selected").text());
            }
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

    });

    jQuery("#tbScheduleRemittanceList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
          { startColumnName: 'This_Amt', numberOfColumns: 2, titleText: 'Account of Bank Authorisations Received and Utilised' },
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