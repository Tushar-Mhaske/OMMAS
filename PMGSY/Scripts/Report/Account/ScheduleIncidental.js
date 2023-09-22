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
//    SetOnDocumentReady();
    //alert("Ready");

    //Added By Abhishek kamble 11-mar-2014 start
    $(function () {

        $("#rdbPiu").click(function () {
            $("#Agency").trigger("change");
        });
    });
    //Added By Abhishek kamble 11-mar-2014 end


    $("#btnView").click(function () {
        //alert("OK");
        var levelId = $("#LevelId").val();
        ShowReport(levelId);
    });

    //$("#btnViewDPIU").click(function () {
    //    alert("OK");
    //    var levelId = $("#LevelId").val();
    //    ShowReport(levelId);
    //});

    $("#Month").change(function () {

        UpdateAccountSession($("#Month").val(), $("#Year").val());

    });

    $("#Year").change(function () {

        UpdateAccountSession($("#Month").val(), $("#Year").val());

    });


});

function SetOnDocumentReady() {
    $("#spanState").hide();
    
}

function ShowReport(levelId) {

    $("#tblHeading").show();

    //alert("OK");
    var month = $("#Month").val();
    var year = $("#Year").val();
    var ndcode = $("#Piu").val();
    var isYear = $("#rdbAnnual").is(":checked");
    var allpiu = 0;
    //  var isStatePiu = $("#rdbState").is(":checked");
    var reportLevel = 2;
    if (levelId != 5) {
        reportLevel = $("input[type='radio'].rdbClassReportLevel:checked").val();
    }

    if (reportLevel == 2 && ndcode == 0) {
        ndcode = $("#Agency").val();
        allpiu = 1;

        //Added By Abhishek kamble 28-Apr-2014
        if (levelId == 5) {
            allpiu = 0;
        } else {
            allpiu = 1;
        }
    }
    else if (levelId == 4 || levelId == 6) {


        if (reportLevel != 2) {

            ndcode = $("#Agency").val();
        }



    }
    
    if (levelId != 5 &&  ndcode == 0) {
        //alert("Select Agency.");
        return false;
    }
    //else if (month == 0 && !isYear) {
    //    //alert("Select Accounting month.");
    //    return false;
    //}
    //else 
    if ($("#rdbMonth").is(":checked")) {
        if (($("#Month option:selected").val() == "0")) {
            //alert("Select Accounting month.");
            return false;
        }
    }

    if (year == 0) {
        //alert("Select Accounting Year");
        return false;
    }
    else {

        lblYear = $("#Year option:selected").text();
        if (levelId != 5) {
            if (reportLevel == 2) {

                if ($("#Piu option:selected").text() == "Select Department") {
                    $("#lblPIUName").text(" - ");
                }
                else {
                    $("#lblPIUName").text($("#Piu option:selected").text());
                }
            }
            else {
                $("#lblPIUName").text(" - ");
            }
        }
        if (month == 0) {
            $("#lblMonthYear").html("<span style='color:green'> Year: </span>" + lblYear);
        }
        else {

            $("#lblMonthYear").html(" <span style='color:green'> Month-Year: </span>" + $("#Month option:selected").text() + "- " + lblYear);
        }

        var data = { month: month, year: year, ndcode: ndcode, rlevel: reportLevel, allpiu: allpiu };
         //alert("month :" + month + " year: " + year + "  ndcode:" + ndcode + "  reportLevel:" + reportLevel + " allpiu: "+allpiu);

        if ($("#tbPmgsyScheduleList") != undefined) {
            $("#tbPmgsyScheduleList").GridUnload();

            //alert(state);
        }

        //ListSchedule(data);

        ValidateForm(data);

    }
}

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
                ListSchedule(param);
            }

        }
    });
}

function ListSchedule(data) {


    //blockPage();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    // var data = { ndcode: 525, month: 3, year: 2012, balance: "C" };
    jQuery("#tbPmgsyScheduleList").jqGrid({
        url: "/Reports/GetScheduleIncidental",
        datatype: "json",
        mtype: "POST",
        colNames: ["Head of Account", "Inner Column", "Outer Column"],
        colModel: [

                    { name: 'ITEM_HEADING', index: 'ITEM_HEADING', width: 400, sortable: false, align: "left" },
                    { name: 'CURRENT_AMT', index: 'CURRENT_AMT', width: 200, sortable: false, align: "right", editable: true, formatter: "number", formatoptions: { decimalPlaces: 2 } },
                    { name: 'PREVIOUS_AMT', index: 'PREVIOUS_AMT', width: 200, sortable: false, align: "right", editable: true, formatter: "number", formatoptions: { decimalPlaces: 2 } }
        ],
        rowNum: 1020,
        viewrecords: true,
        postData: data,
        caption: "&nbsp;&nbsp;Schedule of Incidental Fund",
        height: 'auto',
        //  maxheight: 100,
        shrinktofit: false,
        width: 'auto',
        autowidth: true,
        rownumbers: true,
        //footerrow: true,
        //userDataOnFooter: true,
        pginput: false,
        pgbuttons:false,
        pager: jQuery('#pager'),
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
            $("#lblHeaderFormNumber").text(jsondata.reportHeader.formNumbr);
            $("#lblHeaderFundType").text(jsondata.reportHeader.fundType);
            $("#lblReportHeader").text(jsondata.reportHeader.heading);
            $("#lblReportReference").text(jsondata.reportHeader.referance);

            jQuery("#tbPmgsyScheduleList").jqGrid('setLabel', 'ITEM_HEADING', jsondata.reportHeader.scheduleNo);
            // alert(jsondata.records);
            //jQuery("#tbPmgsyScheduleList").jqGrid('footerData', 'set',
		    //    {
		    //        ITEM_HEADING: '<b>Total</b>',
		    //        CURRENT_AMT: jsondata.footerData.totCurrAmt,
		    //        PREVIOUS_AMT: jsondata.footerData.totPrevAmt
		    //    }

            //);

            // alert(jsondata.reportHeader.formNumbr);            
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
                //alert($("#Year option[value=" + year + "]").text());
                //jQuery("#tbPmgsyScheduleList").jqGrid('setLabel', 'CURRENT_AMT', "Upto " + $("#Year option:selected").text());
                //jQuery("#tbPmgsyScheduleList").jqGrid('setLabel', 'PREVIOUS_AMT', "Upto " + yearPrev);
                jQuery("#tbPmgsyScheduleList").jqGrid('setLabel', 'CURRENT_AMT', "Current Year : " + $("#Year option:selected").text());
                jQuery("#tbPmgsyScheduleList").jqGrid('setLabel', 'PREVIOUS_AMT', "Previous Year : " + yearPrev);
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
                //jQuery("#tbPmgsyScheduleList").jqGrid('setLabel', 'CURRENT_AMT', "Upto " + $("#Month option:selected").text() + ", " + year);
                //jQuery("#tbPmgsyScheduleList").jqGrid('setLabel', 'PREVIOUS_AMT', "Upto " + $("#Month option[value=" + month + "]").text() + ", " + yearPrev);
                jQuery("#tbPmgsyScheduleList").jqGrid('setLabel', 'CURRENT_AMT', "Current Month and Year : " + $("#Month option:selected").text() + " - " + year);
                jQuery("#tbPmgsyScheduleList").jqGrid('setLabel', 'PREVIOUS_AMT', "Previous Month and Year : " + $("#Month option[value=" + month + "]").text() + " - " + yearPrev);
            }
            jQuery("#tbPmgsyScheduleList").jqGrid('setCell', 'Total', "ITEM_HEADING", "", { 'font-size': '12px', 'font-weight': 'bold' });
            jQuery("#tbPmgsyScheduleList").jqGrid('setCell', 'Total', "CURRENT_AMT", "", { 'font-size': '12px', 'font-weight': 'bold' });
            jQuery("#tbPmgsyScheduleList").jqGrid('setCell', 'Total', "PREVIOUS_AMT", "", { 'font-size': '12px', 'font-weight': 'bold' });
            //("#Agency").addClass("valid");
            //$("#Agency").addClass("valid myClass yourClass");
            //unblockPage();
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
    }); //end of grid
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