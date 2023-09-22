var adminCode;

//$.validator.addMethod("isvalidreport", function (value, element, params) {

//    var reportType;
//    if ($("#rdbSrrda").is(':checked'))
//    {
//        reportType = 'S';
//        alert(reportType);
//    }
//    else if ($("#rdbDpiu").is(':checked'))
//    {
//        reportType = 'D';
//    }

//    if (reportType == 'S')
//    {
//        if ($("#ddlSrrda option:selected").val() == 0) {
//            return false;
//        }
//        else {
//            return true;
//        }
//    }
    
//    if (reportType == 'D') {
//        if ($("#ddlSrrda option:selected").val() == 0) {
//            return false;
//        }
//        else {
//            if ($("#ddlDpiu option:selected").val() == 0) {
//                return false;
//            }
//            else {
//                return true;
//            }
//        }
//    }

//    return false;
//});
//jQuery.validator.unobtrusive.adapters.addBool("isvalidreport");

$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmImprestSettlement');

    $(".trSrrdaDpiu").hide();
    $(".tdDpiu").hide();

    $("#rdbSrrda").click(function () {

        $(".trSrrdaDpiu").show('slow');
        if ($(".tdDpiu").is(":visible"))
        {
            $(".tdDpiu").hide('slow');
        }
    });

    $("#rdbDpiu").click(function () {
        $("#ddlSrrda").trigger('click');
        $(".tdDpiu").show('slow');
    });

    $("#ddlSrrda").change(function () {
        if ($("#LevelId").val() != 5) {
            FillInCascadeDropdown(null, "#ddlDpiu", "/Reports/PopulateDPIUOfSRRDA?id=" + $("#ddlSrrda").val());
        }

    });

    $("#btnView").click(function () {

        if ($("#frmImprestSettlement").valid()) {

            //added by abhishek kamble 1-jan-2014
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            if ($("#rdbSrrda").is(":checked")) {
                adminCode = $("#ddlSrrda option:selected").val();
            }
            else if ($("#rdbDpiu").is(":checked")) {
                adminCode = $("#ddlDpiu option:selected").val();
            }

            

            $("#ddlSrrda").attr('disabled',false);

            $.ajax({

                type: 'POST',
                url: '/Reports/GetImprestHeaderInfo',
                data: $("#frmImprestSettlement").serialize(),
                async: false,
                cache: false,
                success: function (data)
                {
                    $("#dvHeaderInfo").show();
                    $("#dvHeaderInfo").html(data);
                    if ($("#LevelId").val() == 4) {
                        $("#ddlSrrda").attr('disabled', 'disabled');
                    }
                    
                },
                error: function () {
                    
                }
            });

            LoadImprestSettlementDetails(adminCode);
            
        }
    });


    if ($("#LevelId").val() != 5) {
        $("#rdbSrrda").trigger('click');
    }

});
function LoadImprestSettlementDetails(adminCode)
{
   
    $("#tblListImprests").jqGrid('GridUnload');

    jQuery("#tblListImprests").jqGrid({
        url: '/Reports/ListImprestSettlements'  ,
        datatype: "json",
        mtype: "POST",
        postData: { Year: $("#ddlFinancialYear option:selected").val(), AdminCode: adminCode},
        //colNames: ['Month and Year From which transaction Dated', 'Name of Contractor/Supplier(Contractor Id)', 'Agreement No.', 'Opening Balance (in Rs.)', 'Voucher/Transfer Entry', 'Credit (in Rs.)', 'Debit (in Rs.)', 'Balance (in Rs.)'],
        colNames: ['Voucher No', 'Voucher Date', 'Imprest Amount', 'Payee Name', 'Settlement Voucher No', 'Settlement Voucher Date', 'Settlement Amount'],
        colModel: [
                            { name: 'P_Bill_No', index: 'P_Bill_No', height: 'auto', width: 40, align: "center", search: false, sortable: false, cellattr: arrtSetting0, },
                            { name: 'P_BILL_DATE', index: 'P_BILL_DATE', height: 'auto', width: 40, align: "center", search: false, sortable: false/*, cellattr: arrtSetting1,*/ },
                            { name: 'P_AMOUNT', index: 'P_AMOUNT', height: 'auto', width: 40, align: "right", sortable: false, cellattr: arrtSetting2, },
                            { name: 'Payee_Name', index: 'Payee_Name', height: 'auto', width: 60, align: "center", search: false, cellattr: arrtSetting3, },
                            { name: 'S_BILL_NO', index: 'S_BILL_NO', height: 'auto', width: 50, align: "center", search: false, sortable: false },
                            { name: 'S_BILL_DATE', index: 'S_BILL_DATE', height: 'auto', width: 50, align: "center", search: false, sortable: false },
                            { name: 'S_Amount', index: 'S_Amount', height: 'auto', width: 40, align: "right", search: false, sortable: false },

        ],
        pager: jQuery('#dvlstImprests').width(20),
        rowNum: 999999,
        //rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "P_Bill_No",
        sortorder: "asc",
        caption: "&nbsp;&nbsp;Imprest Details",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        gridview: true,
        hoverrows: false,
        autoencode: true,
        ignoreCase: true,
        pginput:false,
        cmTemplate: { title: false },
        beforeProcessing: function () {
            
        },
        loadComplete: function (data) {
            $("#tblListImprests").parents('div.ui-jqgrid-bdiv').css("max-height", "325px");
            $.unblockUI();
        },
        gridComplete: function () {
            var grid = this;

            $('td[rowspan="1"]', grid).each(function () {
                var spans = $('td[rowspanid="' + this.id + '"]', grid).length + 1;

                if (spans > 1) {
                    $(this).attr('rowspan', spans).attr('vertical-align','central');
                }
            });
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
            $.unblockUI();
        },
        resizeStop: function () {
            var $self = $(this),
                shrinkToFit = $self.jqGrid("getGridParam", "shrinkToFit");

            $self.jqGrid("setGridWidth", this.grid.newWidth, shrinkToFit);
            setHeaderWidth.call(this);
        }
    });
}
function FillInCascadeDropdown(map, dropdown, action) {

    $(dropdown).empty();
    $.post(action, map, function (data) {

        $.each(data, function () {
            if (this.Value != 0) {
                if (this.Selected == true) {
                    $(dropdown).append("<option value='" + this.Value + "' selected =" + this.Selected + ">" + this.Text + "</option>");
                }
                else { $(dropdown).append("<option value='" + this.Value + "'>" + this.Text + "</option>"); }
            }
        });
    }, "json");
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

var prevCellVal1 = { cellId: undefined, value: undefined };

arrtSetting1 = function (rowId, val, rawObject, cm, rdata) {
    var result;

    if (prevCellVal1.value == val) {
        result = ' style="display: none" rowspanid="' + prevCellVal1.cellId + '"';
    }
    else {
        var cellId = this.id + '_row_' + rowId + '_' + cm.name;

        result = ' rowspan="1" id="' + cellId + '"';
        prevCellVal1 = { cellId: cellId, value: val };
    }

    return result;
}
var prevCellVal2 = { cellId: undefined, value: undefined };

arrtSetting2 = function (rowId, val, rawObject, cm, rdata) {
    var result;

    if (prevCellVal2.value == val) {
        result = ' style="display: none" rowspanid="' + prevCellVal2.cellId + '"';
    }
    else {
        var cellId = this.id + '_row_' + rowId + '_' + cm.name;

        result = ' rowspan="1" id="' + cellId + '"';
        prevCellVal2 = { cellId: cellId, value: val };
    }

    return result;
}
var prevCellVal3 = { cellId: undefined, value: undefined };

arrtSetting3 = function (rowId, val, rawObject, cm, rdata) {
    var result;

    if (prevCellVal3.value == val) {
        result = ' style="display: none" rowspanid="' + prevCellVal3.cellId + '"';
    }
    else {
        var cellId = this.id + '_row_' + rowId + '_' + cm.name;

        result = ' rowspan="1" id="' + cellId + '"';
        prevCellVal3 = { cellId: cellId, value: val };
    }

    return result;
}