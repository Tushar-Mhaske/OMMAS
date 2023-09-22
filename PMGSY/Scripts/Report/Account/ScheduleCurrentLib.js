
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

        $("#spanSrrda").show();

    });
    $("#spanPIU").show();
    $("#rdbState").click(function () {
        $("#rdbState").attr("value", "2"); lblDPIU
        $("#spanPIU").show();

    });
    $("#btnView").click(function () {
        // var reportLevel = 2;

        if ($("#rdbMonth").is(":checked"))
        {
            if (($("#Month option:selected").val() == "0")) {
                //alert("Select Accounting month.");
                return false;
            }
        }

        //if (($("#rdbMonth").val() == 2) && ($("#Month option:selected").val() == "0")) {

        //     alert("Select Accounting month.");
        //    return false;
        //} else

        if ($("#Year option:selected").val() == "0") {
            //alert("Select Accounting Year");
            return false;
        }
        else {
            $("#lblAgency").text($("#Agency option:selected").text());

            //added by abhishek kamble 13-dec-2013
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


            if ($("#rdbState").is(":checked") && $("#rdbAnnual").is(":checked")) {
                if ($("#Piu option:selected").val() == 0) {
                    data = { month: 0, Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 4, allpiu: 1 };
                }
                else {
                    data = { month: 0, Year: $("#Year option:selected").val(), ndcode: $("#Piu option:selected").val(), rlevel: 4, allpiu: 0 };
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

            if ($("#rdbSrrda").is(":checked") && $("#rdbAnnual").is(":checked")) {
                //if ($("#Piu option:selected").val() == 0) {
                data = { month: 0, Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 1, allpiu: 0 };
                // }
                // else {
                //     data = { month: 0, Year: $("#Year option:selected").val(), ndcode: $("#Piu option:selected").val(), rlevel: 1, allpiu: 0 };
                // }
                ValidateForm(data);
            }

            if ($("#rdbSrrda").is(":checked") && $("#rdbMonth").is(":checked")) {
                // if ($("#Piu option:selected").val() == 0) {
                data = { month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 1, allpiu: 0 };
                //  }
                // else {
                // data = { month: 0, Year: $("#Year option:selected").val(), ndcode: $("#Piu option:selected").val(), rlevel: 1, allpiu: 0 };
                // }
                ValidateForm(data);
            }

            if ($("#rdbPiu").is(":checked") && $("#rdbAnnual").is(":checked")) {
                if ($("#Piu option:selected").val() == 0) {
                    data = { month: 0, Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 2, allpiu: 1 };
                }
                else {
                    data = { month: 0, Year: $("#Year option:selected").val(), ndcode: $("#Piu option:selected").val(), rlevel: 2, allpiu: 0 };
                }
                ValidateForm(data);
            }

            if ($("#rdbPiu").is(":checked") && $("#rdbMonth").is(":checked")) {
                if ($("#Piu option:selected").val() == 0) {
                    data = { month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 2, allpiu: 1 };
                }
                else {
                    data = { month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndcode: $("#Piu option:selected").val(), rlevel: 2, allpiu: 0 };
                }
                ValidateForm(data);
            }

            if ($("#LevelId").val() == 5 && $("#rdbAnnual").is(":checked")) {
                data = { month: 0, Year: $("#Year option:selected").val(), ndcode: $("#AdminNdCode").val(), rlevel: 2, allpiu: 0 };
                ValidateForm(data);
            }

            if ($("#LevelId").val() == 5 && $("#rdbMonth").is(":checked")) {
                data = { month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndcode: $("#AdminNdCode").val(), rlevel: 2, allpiu: 0 };
                ValidateForm(data);
            }
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
                loadCurrLibGrid(param)
            }

        }
    });
}




function loadCurrLibGrid(data) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    jQuery("#tbScheduleCurrLibList").jqGrid("GridUnload");
    jQuery("#tbScheduleCurrLibList").jqGrid({
        url: "/Reports/GetCurrentLibDetails",
        datatype: "json",
        mtype: "POST",
        colNames: ["Head of Account", "Current Month", "Previous Month"],
        colModel: [

                    { name: 'ITEM_HEADING', index: 'ITEM_HEADING', width: 400, sortable: false, align: "left" },
                    { name: 'CURRENT_AMT', index: 'CURRENT_AMT', width: 200, sortable: false, align: "right", editable: true, formatter: formatCurAmt, },
                    { name: 'PREVIOUS_AMT', index: 'PREVIOUS_AMT', width: 200, sortable: false, align: "right", editable: true, formatter: formatPreAmt }
        ],
        rowNum: 1020,
        viewrecords: true,
        postData: data,
        caption: "&nbsp;&nbsp;Schedule of Current Liabilities",
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

            //alert(jsondata.error.message);
            //return false;
            $("#tblHeading").show();


            $("#lblHeaderFormNumber").text(jsondata.reportHeader.formNumbr);
            $("#lblHeaderFundType").text(jsondata.reportHeader.fundType);
            $("#lblReportHeader").text(jsondata.reportHeader.heading);
            $("#lblReportReference").text(jsondata.reportHeader.referance);

            jQuery("#tbScheduleCurrLibList").jqGrid('setLabel', 'ITEM_HEADING',"<b>"+jsondata.reportHeader.scheduleNo+"</b>");

            jQuery("#tbScheduleCurrLibList").jqGrid('setCell', 'Total', 'ITEM_HEADING', "", { 'font-weight': 'bold', 'text-align': 'left' ,'font-size':'11px'});

            jQuery("#tbScheduleCurrLibList").jqGrid('setCell', 'Total', 'CURRENT_AMT', "", { 'font-weight': 'bold', 'font-size': '11px' });

            jQuery("#tbScheduleCurrLibList").jqGrid('setCell', 'Total', 'PREVIOUS_AMT', "", { 'font-weight': 'bold', 'font-size': '11px' });

           // jQuery("#tbScheduleCurrLibList").jqGrid('setCell', 'footerData', 'ITEM_HEADING', "", { 'font-weight': 'bold', 'text-align': 'right' });
            
            jQuery("#tbScheduleCurrLibList").jqGrid('footerData', 'set',
                {
                    ITEM_HEADING: "<h4> GRAND TOTAL</h4>",
                    
                    CURRENT_AMT:"Rs. "+ jsondata.footerData.totCurrAmt.toFixed(2),
                    PREVIOUS_AMT: "Rs. " + jsondata.footerData.totPrevAmt.toFixed(2)
                }

            );

            //var allRowInGrid = jQuery("#tbScheduleCurrLibList").jqGrid("getDataIDs");
            //for (i = 1; i < 7; i++) {
            //    var rowId = allRowInGrid[i];
            //    var rowData = $('#tbScheduleCurrLibList').jqGrid('getRowData', rowId);
            //    if (rowData.CURRENT_AMT % 1 == 0) {
            //        rowData.CURRENT_AMT = rowData.CURRENT_AMT + "" + ".00";
            //        $('#tbScheduleCurrLibList').jqGrid('setRowData', rowId, rowData);
            //    }
            //    if (rowData.PREVIOUS_AMT % 1 == 0) {
            //        rowData.PREVIOUS_AMT = rowData.PREVIOUS_AMT + "" + ".00";
            //        $('#tbScheduleCurrLibList').jqGrid('setRowData', rowId, rowData);
            //    }
            //    else {
            //        $('#tbScheduleCurrLibList').jqGrid('setRowData', rowId, rowData);
            //    }
               

            //}


            //var grid = $("#tbScheduleCurrLibList"),
            //currMonSum = grid.jqGrid('getCol', 'CURRENT_AMT', false, 'sum');

                     
            //prevMonSum = grid.jqGrid('getCol', 'PREVIOUS_AMT', false, 'sum');


            //if (currMonSum % 1 == 0) {
            //    currMonSum = currMonSum + "" + ".00";
            //}
            //if (prevMonSum % 1 == 0) {
            //    prevMonSum = prevMonSum + "" + ".00";
            //}

            //grid.jqGrid('footerData', 'set', { ID: 'curTotal:', CURRENT_AMT: currMonSum });

            //grid.jqGrid('footerData', 'set', { ID: 'prevTotal:', PREVIOUS_AMT: prevMonSum });
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
                //commented by abhishek kamble 11-dec-2013
                //jQuery("#tbScheduleCurrLibList").jqGrid('setLabel', 'CURRENT_AMT', "Upto " + $("#Year option:selected").text());
                //jQuery("#tbScheduleCurrLibList").jqGrid('setLabel', 'PREVIOUS_AMT', "Upto " + yearPrev);
                //Added by abhishek kamble 11-dec-2013
                jQuery("#tbScheduleCurrLibList").jqGrid('setLabel', 'CURRENT_AMT', "Current Year : " + $("#Year option:selected").text());
                jQuery("#tbScheduleCurrLibList").jqGrid('setLabel', 'PREVIOUS_AMT', "Previous Year : " + yearPrev);
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
                //jQuery("#tbScheduleCurrLibList").jqGrid('setLabel', 'CURRENT_AMT', "Upto " + $("#Month option:selected").text() + ", " + year);
                //jQuery("#tbScheduleCurrLibList").jqGrid('setLabel', 'PREVIOUS_AMT', "Upto " + $("#Month option[value=" + month + "]").text() + ", " + yearPrev);

                //added by abhishek kamble 11-dec-2013
                jQuery("#tbScheduleCurrLibList").jqGrid('setLabel', 'CURRENT_AMT', "Current Month and Year : " + $("#Month option:selected").text() + " - " + year);
                jQuery("#tbScheduleCurrLibList").jqGrid('setLabel', 'PREVIOUS_AMT', "Previous Month and Year :  " + $("#Month option[value=" + month + "]").text() + " - " + yearPrev);
            }

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


function formatCurAmt(cellvalue, options, rowObject) {
    //alert(cellvalue);
    if (cellvalue == null) {
        return '';
    } else {

        if (cellvalue % 1 == 0) {
                cellvalue += ".00";
        }
        return cellvalue.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
}

function formatPreAmt(cellvalue, options, rowObject) {
    //alert(cellvalue);
    if (cellvalue == null) {
        return '';
    } else {
        if (cellvalue % 1 == 0) {
                cellvalue += ".00";
        }
        return cellvalue.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
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