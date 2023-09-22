
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
    SetOnDocumentReady();
    //alert("Ready");
   /* $(".rdbClassYear").change(function () {
       
        var isYear = $(this).val();
       // alert(isYear=="True");
        if (isYear == "True") {
            $("#Month").val(0);
            $("#spanMonth").hide();
            //alert("Yearly");
            $("#Year option").each(function () {
                var currentYear = $(this).val();
                if (currentYear != 0) {
                    $(this).text(currentYear+"-"+(parseInt(currentYear)+1));
                }
                //alert();
            });
        }
        else {
            $("#Year option").each(function () {
                var currentYear = $(this).val();
                if (currentYear != 0) {
                    $(this).text(currentYear);
                }
                //alert();
            });
            $("#spanMonth").show();
            
        }
    });
    $(".rdbClassType").change(function () {

        var isSrrdaPiu = $(this).val();
        // alert(isYear=="True");
        if (isSrrdaPiu == "True") {
            $("#spanPiu").show();
            
        }
        else {
            $("#spanPiu").hide();

        }
    });*/
    $("#btnView").click(function () {
        var levelId = $("#LevelId").val();
        ShowReport(levelId);
    });

    $("#Month").change(function () {

        UpdateAccountSession($("#Month").val(), $("#Year").val());

    });

    $("#Year").change(function () {

        UpdateAccountSession($("#Month").val(), $("#Year").val());

    });


});

function SetOnDocumentReady() {
    //$("#spanSrrda").hide();
    
}


function ShowReport(levelId) {
   
    $("#tblHeading").show();

    var month = $("#Month").val();
    var year = $("#Year").val();
    var ndcode = $("#Agency").val();
    var isYear = $("#rdbAnnual").is(":checked"); 
    var allpiu = 0;
    var reportLevel=0;
    //  var isStatePiu = $("#rdbState").is(":checked");
    if (levelId != 5) {
        reportLevel = $("input[type='radio'].rdbClassReportLevel:checked").val();
    }
    else {
         reportLevel = 2;
    }
    
    //alert(reportLevel);
    
    if (reportLevel == 4 && ndcode == 0) {
        ndcode = $("#Agency").val();
        //allpiu = 1;
    }
    else if (levelId == 4 || levelId == 6) {
        if (reportLevel != 4) {
            
            if ($("#Piu").val() != 0) {
                ndcode = $("#Piu").val();
            } else {

                if (reportLevel == 2) {
                    allpiu = 1;
                }

                ndcode = $("#Agency").val();
            }            
        }
    }
    if (levelId == 5)
    {
        ndcode=$("#ndCode").val();
    }

    
   
        
    
    //if (month == 0 && !isYear) {
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

            if ($("#rdbPiu").is(":checked")) {

                if ($("#Piu option:selected").text() == "Select Department") {
                    $("#lblPIUName").text(" - ");
                } else {
                    $("#lblPIUName").text($("#Piu option:selected").text());
                }
            }
            else {
                $("#lblPIUName").text(" - ");
            }
        }


        if (month == 0) {
            $("#lblMonthYear").html("<span style='color:green'> Year:</span>" + lblYear);
        }
        else {

            $("#lblMonthYear").html("<span style='color:green'> Month-Year: </span>" + $("#Month option:selected").text() + "- " + lblYear);
        }

       var data = { month: month, year: year, ndcode: ndcode, rlevel: reportLevel, allpiu: allpiu };
        // alert("month :" + month + " year: " + year + "  ndcode:" + ndcode + "  reportLevel:" + reportLevel + " allpiu: "+allpiu);

        if ($("#tbPmgsyScheduleList") != undefined) {
            $("#tbPmgsyScheduleList").GridUnload();

            //alert(state);
        }
        
     //  ListSchedule(data);
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
                $("#dvError").html('<strong>Alert : </strong>'+data.message);
              
                return false;
            }
            else if(data.success==true) {
                ListSchedule(param);
            }

        }
    });
}

function ListSchedule(data) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
   
    //blockPage();
   // var data = { ndcode: 525, month: 3, year: 2012, balance: "C" };
    jQuery("#tbPmgsyScheduleList").jqGrid({
        url: "/Reports/GetSchedule",
        datatype: "json",
        mtype: "POST",
        colNames: [ "Head of Account","Current Month", "Previous Month"],
        colModel: [
                   
                    { name: 'ITEM_HEADING', index: 'ITEM_HEADING', width: 400, sortable: false, align: "left" },
                    { name: 'CURRENT_AMT', index: 'CURRENT_AMT', width: 200, sortable: false, align: "right", editable: true, formatter: "number", formatoptions: { decimalPlaces: 2 } },
                    { name: 'PREVIOUS_AMT', index: 'PREVIOUS_AMT', width: 200, sortable: false, align: "right", editable: true, formatter: "number", formatoptions: { decimalPlaces: 2 } }
        ],
        rowNum: 1020,
        viewrecords: true,
        postData:data,
        caption: "&nbsp;&nbsp;Schedule of Deposit Repayable",
        height: 'auto',
      //  maxheight: 100,
        shrinktofit: false,
        width: 'auto',
        autowidth: true,
        rownumbers: true,
        //footerrow: true,
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

                //alert(jsondata.error.message);
                //return false;



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


                    //alert($("#Year option[value=" + year + "]").text());
                    jQuery("#tbPmgsyScheduleList").jqGrid('setLabel', 'CURRENT_AMT', "Current Year: " + $("#Year option:selected").text());
                    jQuery("#tbPmgsyScheduleList").jqGrid('setLabel', 'PREVIOUS_AMT', "Previous Year: " + yearPrev);
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
                    //commented by abhishek kamble 11-dec-2013
                    //jQuery("#tbPmgsyScheduleList").jqGrid('setLabel', 'CURRENT_AMT', "Upto " + $("#Month option:selected").text() + ", " + year);
                    //jQuery("#tbPmgsyScheduleList").jqGrid('setLabel', 'PREVIOUS_AMT', "Upto " + $("#Month option[value=" + month + "]").text() + ", " + yearPrev);

                    jQuery("#tbPmgsyScheduleList").jqGrid('setLabel', 'CURRENT_AMT', "Current Month and Year : " + $("#Month option:selected").text() + " - " + year);
                    jQuery("#tbPmgsyScheduleList").jqGrid('setLabel', 'PREVIOUS_AMT', "Previous Month and Year : " + $("#Month option[value=" + month + "]").text() + " - " + yearPrev);

                }
                //jQuery("#tbPmgsyScheduleList").jqGrid('setCell', 'Programme Fund ( MoRD)', 'ITEM_HEADING', "", { 'font-weight': 'bold' });
    
                jQuery("#tbPmgsyScheduleList").jqGrid('setCell', 'Total', 'ITEM_HEADING', "", { 'font-weight': 'bold', 'font-size': '12px' });
                jQuery("#tbPmgsyScheduleList").jqGrid('setCell', 'Total', 'CURRENT_AMT', "", { 'font-weight': 'bold', 'font-size': '12px' });
                jQuery("#tbPmgsyScheduleList").jqGrid('setCell', 'Total', 'PREVIOUS_AMT', "", { 'font-weight': 'bold', 'font-size': '12px' });

                //jQuery("#tbPmgsyScheduleList").jqGrid('setCell', '2', 'CURRENT_AMT', "", { 'font-weight': 'bold' });
                //unblockPage();
                $.unblockUI();
        },
        loadError: function (xhr, status, error) {
            unblockPage();
            alert(xhr.responseText);
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