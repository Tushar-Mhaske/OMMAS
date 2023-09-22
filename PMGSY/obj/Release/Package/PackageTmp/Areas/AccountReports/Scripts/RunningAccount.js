var reportType;
$(document).ready(function () {
  
    $(function () {
        $("#ddlYear").trigger("change");
    });

    $.validator.unobtrusive.parse($('#frmRunningAccount'));

    $("#ddlYear").change(function () {

        if ($("#rdState").is(':checked')) {
            reportType = "S";
        }
        else if ($("#rdDistrict").is(':checked')) {
            reportType = "D";
        }

        FillInCascadeDropdown(null, "#ddlMonth", "/AccountReports/Account/PopulateRunningMonthsByYear?id=" + $("#ddlYear").val() + "$" + reportType + "$" + $("#ddlDPIU option:selected").val()); //+ reportType == "D"?"$"+$("#ddlPIU option:selected").val():"");

    });

    $("#ddlDPIU").change(function () {

        if ($("#rdState").is(':checked')) {
            reportType = "S";
        }
        else if ($("#rdDistrict").is(':checked')) {
            reportType = "D";
        }
        FillInCascadeDropdown(null, "#ddlYear", "/AccountReports/Account/PopulateRunningAccYear?id=" + reportType + "$" + $("#ddlDPIU option:selected").val()); //+ reportType == "D"?"$"+$("#ddlPIU option:selected").val():"");
    });

    $("#spnMonthlyStateSRRDA").click(function () {
        $("#spnMonthlyStateSRRDA").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triagle-n");
        $("#dvFilterForm").slideToggle("slow");
    });

    $("#btnView").click(function () {

        if ($("#rdState").is(':checked')) {
            reportType = "S";
        }
        else if($("#rdDistrict").is(':checked'))
        {
            reportType = "D";
        }

        if ($("#frmRunningAccount").valid()) {
            //alert($('#ddlMonth option:selected').val());

            $.ajax({

                type: 'POST',
                url: '/AccountReports/Account/RunningAccountReport?' + $.param({ Month: $('#ddlMonth option:selected').val(), Year: $('#ddlYear option:selected').val(), Balance: $("#Balance option:selected").val(), ReportType: reportType, DPIU: $("#ddlDPIU option:selected").val() }),
                async: false,
                cache: false,
                //data: $("#frmRunningAccount").serialize(),
                success: function (data) {
                    $("#LoadDiv").html(data);
                    $("#rptHeaderDesc").show();
                    if ($("#rdDistrict").is(':checked')) {
                        $("#rdDistrict").trigger('click');
                    }

                    //LoadRunningAccountList(data);
                   
                    $("#spnMonthlyStateSRRDA").trigger("click");
                    $.unblockUI();
                },
                //complete: function () {
                //    //alert($("#selectedMonth").val());  
                //    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

                //    setTimeout(function () {

                //        LoadRunningAccountList();

                //    }, 3000);
                //},
                error: function () {
                    alert('Error occurred while processing your request.');
                    $.unblockUI();
                }
            });

        }

    });

    $("#rdDistrict").click(function () {

        $("#ddlPIU").show('slow');

       
            if ($("#rdState").is(':checked')) {
                reportType = "S";
            }
            else if ($("#rdDistrict").is(':checked')) {
                reportType = "D";
            }
            FillInCascadeDropdown(null, "#ddlYear", "/AccountReports/Account/PopulateRunningAccYear?id=" + reportType + "$" + $("#ddlDPIU option:selected").val()); //+ reportType == "D"?"$"+$("#ddlPIU option:selected").val():"");

    });

    $("#rdState").click(function () {

        if ($("#ddlPIU").is(':visible')) {
            $("#ddlPIU").hide('slow');
        }

      
        if ($("#rdState").is(':checked')) {
            reportType = "S";
        }
        else if ($("#rdDistrict").is(':checked')) {
            reportType = "D";
        }
        FillInCascadeDropdown(null, "#ddlYear", "/AccountReports/Account/PopulateRunningAccYear?id=" + reportType + "$" + $("#ddlDPIU option:selected").val()); //+ reportType == "D"?"$"+$("#ddlPIU option:selected").val():"");

    });

});
function FillInCascadeDropdown(map, dropdown, action) {

    $(dropdown).empty();
    $.post(action, map, function (data) {

        $.each(data, function () {
            if (this.Selected == true)
            { $(dropdown).append("<option value='" + this.Value + "' selected =" + this.Selected + ">" + this.Text + "</option>"); }
            else { $(dropdown).append("<option value='" + this.Value + "'>" + this.Text + "</option>"); }
        });
    }, "json").complete(function () {

        //alert("loaded");
        if (dropdown == "#ddlYear") {
            //alert("t");

            $(function () {
                $("#ddlYear").trigger("change");                
            });
        } 
    });

}
function LoadRunningAccountList()
{
    $("#tbRunningAccountList").jqGrid('GridUnload');

    setTimeout(function () {        
        jQuery("#tbRunningAccountList").jqGrid({
            url: '/Reports/RunningAccountList',
            datatype: "json",
            mtype: "POST",
            postData: { Month: $("#ddlMonth").val(), Year: $('#ddlYear option:selected').val(), Balance: $("#Balance option:selected").val(), ReportType: reportType, DPIU: $("#ddlDPIU option:selected").val() },
            colNames: ['Account Code No.', 'Head of Account', 'Status', 'Funding Agency', 'To End of ' + $("#PreMonth").val() + ' Month', 'For the ' + $("#CurMonth").val() + ' Month', 'To End of ' + $("#CurMonth").val() + ' Month'],
            colModel: [
                                { name: 'HEAD_CODE', index: 'HEAD_CODE', height: 'auto', width: 17, align: "center", cellattr: arrtSetting0, search: false },
                                { name: 'HEAD_NAME', index: 'HEAD_NAME', height: 'auto', width: 100, align: "left", cellattr: arrtSetting, search: false },
                                { name: 'HEAD_COMP_PROGRESS', index: 'HEAD_COMP_PROGRESS', height: 'auto', width: 40, cellattr: arrtSetting1, align: "left", hidden: $("#Balance option:selected").val() == "C" ? true : false, search: false },
                                { name: 'IMS_COLLABORATION', index: 'IMS_COLLABORATION', height: 'auto', width: 50, align: "left", search: false, hidden: $("#Balance option:selected").val() == "C" ? true : false },
                                { name: 'OB_BALANCE_AMT', index: 'OB_BALANCE_AMT', height: 'auto', width: 50, align: "right", search: false, formatter: "number", formatoptions: { decimalPlaces: 2 } },
                                { name: 'MONTHLY_BALANCE_AMT', index: 'MONTHLY_BALANCE_AMT', height: 'auto', width: 50, align: "right", search: false, formatter: "number", formatoptions: { decimalPlaces: 2 } },
                                { name: 'MONTH_END_AMOUNT', index: 'MONTH_END_AMOUNT', height: 'auto', width: 50, align: "right", search: false, formatter: "number", formatoptions: { decimalPlaces: 2 } },

            ],
            //pager: jQuery('#pagerRunningAccount').width(20),
            rowNum: 999999,
            //rowList: [10, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: "HEAD_CODE",
            sortorder: "asc",
            caption: "&nbsp;&nbsp; Monthly Account - Running",
            height: 'auto',
            autowidth: true,
            hidegrid: true,
            rownumbers: true,
            gridview: true,
            hoverrows: false,
            autoencode: true,
            ignoreCase: true,
            footerrow: true,
            userDataOnFooter: true,
            cmTemplate: { title: false },
            loadComplete: function (data) {
                $.unblockUI();

                $("#tbRunningAccountList").parents('div.ui-jqgrid-bdiv').css("max-height", "325px");
                grid = $("#tbRunningAccountList").jqGrid();
                var beforeAmount = grid.jqGrid('getCol', 'OB_BALANCE_AMT', false, 'sum');
                var currentAmount = grid.jqGrid('getCol', 'MONTHLY_BALANCE_AMT', false, 'sum');
                var afterAmount = grid.jqGrid('getCol', 'MONTH_END_AMOUNT', false, 'sum');
                grid.jqGrid('footerData', 'set', { HEAD_NAME: 'TOTAL', OB_BALANCE_AMT: parseFloat(beforeAmount).toFixed(2).toLocaleString(), MONTHLY_BALANCE_AMT: parseFloat(currentAmount).toFixed(2).toLocaleString(), MONTH_END_AMOUNT: parseFloat(afterAmount).toFixed(2).toLocaleString() });

            },
            gridComplete: function () {
                var grid = this;

                $('td[rowspan="1"]', grid).each(function () {
                    var spans = $('td[rowspanid="' + this.id + '"]', grid).length + 1;

                    if (spans > 1) {
                        $(this).attr('rowspan', spans).css('text-align', 'center');
                    }
                });

                //Added By Abhishek Kamble 11-Nov-2013
                $('#tbRunningAccountList_rn').html('Sr.<br/>No');
            },
            loadError: function (xhr, ststus, error) {

                if (xhr.responseText == "session expired") {
                    alert(xhr.responseText);
                    window.location.href = "/Login/Login";
                }
                else {
                    alert("Invalid data.Please check and Try again!")
                    //  window.location.href = "/Login/LogIn";
                }
            }
           
        });

        jQuery("#tbRunningAccountList").jqGrid('setGroupHeaders', {
            useColSpanStyle: false,
            groupHeaders: [
              { startColumnName: 'OB_BALANCE_AMT', numberOfColumns: 3, titleText: $("#Balance").val() == "C" ? "Credit Balances ( All Amounts are in Rs. )" : "Debit Balances ( All Amounts are in Rs. )" },
              { startColumnName: 'closed', numberOfColumns: 2, titleText: 'Shiping' }
            ]
        });
    }, 1000);

}

var prevCellVal0 = { cellId: undefined, value: undefined };

arrtSetting0 = function (rowId, val, rawObject, cm, rdata) {
    var result;

    if (prevCellVal0.value == val) {
        result = ' style="display: none" rowspanid="' + prevCellVal0.cellId + '"';
    }
    else {
        var cellId = this.id + '_row_' + rowId + '_' + cm.name;

        result = ' rowspan="1" id="' + cellId + '"';
        prevCellVal0 = { cellId: cellId, value: val };
    }

    return result;
}

var prevCellVal = { cellId: undefined, value: undefined };

arrtSetting= function (rowId, val, rawObject, cm, rdata) {
    var result;

    if (prevCellVal.value == val) {
        result = ' style="display: none" rowspanid="' + prevCellVal.cellId + '"';
    }
    else {
        var cellId = this.id + '_row_' + rowId + '_' + cm.name;

        result = ' rowspan="1" id="' + cellId + '"';
        prevCellVal = { cellId: cellId, value: val };
    }

    return result;
}
var prevCellVal1 = { cellId: undefined, value: undefined };

arrtSetting1 = function (rowId, val, rawObject, cm, rdata) {
    var result;
    if (val != '&#160;') {
        if (prevCellVal1.value == val) {
            result = ' style="display: none" rowspanid="' + prevCellVal1.cellId + '"';
        }
        else {
            var cellId = this.id + '_row_' + rowId + '_' + cm.name;

            result = ' rowspan="1" id="' + cellId + '"';
            prevCellVal1 = { cellId: cellId, value: val };
        }
    }

    return result;
}
