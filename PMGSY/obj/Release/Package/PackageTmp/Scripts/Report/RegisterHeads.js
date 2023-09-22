
//added by abhishek kamble 4-dec-2013
jQuery.validator.addMethod("requireddependentmonthlyfield", function (value, element, param) {
    var month = $('#ddlMonths').val();

    if ($("#rdbMonth").is(":checked")) {
        if (month <= 0 || month > 12) {
            return false;
        }
        else {
            return true;
        }
    } else {
        return true;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("requireddependentmonthlyfield");

jQuery.validator.addMethod("requireddependentyearfield", function (value, element, param) {
    var year = $("#ddlYears").val();

    if ($("#rdbMonth").is(":checked")) {
        if (year < 2000 || year == 0) {
            return false;
        } else {
            return true;
        }
    } else {
        return true;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("requireddependentyearfield");

jQuery.validator.addMethod("requireddependentyearlyfield", function (value, element, param) {
    var year = $('#ddlFinancialYears').val();
    if ($("#rdbYear").is(":checked")) {
        if (year < 2000 || year == 0) {
            return false;
        }
        else {
            return true;
        }
    } else {
        return true;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("requireddependentyearlyfield");

//jQuery.validator.addMethod("dpiuvalidator", function (value, element, param) {

//    alert($("#LevelId").val());

//    if ($("#LevelId").val() == 4 || $("#LevelId").val() == 6) {

//        if( ($("#rdbDPIU").is(":checked")) && ())
//        {

        
//        }else{
//        return true;
//    }


//    }
//    else {
//        return true;
//    }

//    var year = $('#ddlFinancialYears').val();
//    if ($("#rdbYear").is(":checked")) {
//        if (year < 2000 || year == 0) {
//            return false;
//        }
//        else {
//            return true;
//        }
//    } else {
//        return true;
//    }
//});
//jQuery.validator.unobtrusive.adapters.addBool("dpiuvalidator");

var reportType;
var postData;
var fcol;
var lcol;
var monthly = false;
var yearly = false;
$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmRegisterHead');

    $(".yearly").hide();
    $(".tdsrrda").hide();


    $("#rdbMonth").click(function () {

        $(".monthly").show('slow');

        if ($(".yearly").is(':visible')) {
            $(".yearly").hide('slow');
        }
    });

    $("#rdbYear").click(function () {

        $(".yearly").show('slow');

        if ($(".monthly").is(':visible'))
        {
            $(".monthly").hide('slow');

            //Added By Abhishek kamble 3-July-2014 to reset month
            $("#ddlMonths").val(0);
        }

    });

    $("#rdbSRRDA").click(function () {

        $(".tdsrrda").show('slow');

        if ($(".tdsrrdadpiu").is(':visible')) {
            $(".tdsrrdadpiu").hide('slow');
        }
    });

    $("#rdbDPIU").click(function () {

        $(".tdsrrdadpiu").show('slow');

        if ($(".tdsrrda").is(':visible')) {
            $(".tdsrrda").hide('slow');
        }
    });

    fcol = $("#HeadCategoryId").val() == "6" ? "Debit" : "Credit";
    lcol = $("#HeadCategoryId").val() == "6" ? "Credit" : "Debit";

    $("#btnView").click(function () {
        //alert($("#frmRegisterHead").valid());

        //modified by abhishek kamble 4-dec-2013
        //if ($("#rdbMonth").is(':checked'))
        //{
        //    if ($("#ddlMonths").val() == 0)
        //    {
        //        //alert('Please select Month and Year');
        //        return false;
        //    }
        //    else if ($("#ddlYears").val() == 0) {
        //        //alert('Please select Month and Year');
        //        return false;
        //    }
        //}
        //else if ($("#rdbYear").is(':checked'))
        //{
        //    if ($("#ddlFinancialYears").val() == 0)
        //    {
        //        //alert('Please select Financial Year');
        //        return false;
        //    }
        //}
//
        //if (levelId == 4 )
        //{
        //    if ($("#rdbDPIU").is(':checked')) {
        //        if ($("#ddlSRRDAPIU").val() == 0)
        //        {
        //            //alert('Please select Nodal Agency and PIU');
        //            return false;
        //        }
        //        else if ($("#ddlPIU").val() == 0)
        //        {
        //            //alert('Please select Nodal Agency and PIU');
        //            return false;
        //        }
        //    }
        //    else if ($("#rdbSRRDA").is(':checked')) {
        //        if ($("#ddlSRRDA").val() == 0) {
        //            //alert('Please select Nodal Agency');
        //            return false;
        //        }
        //    }
        //}

        //if (levelId == 6) {
        //    if ($("#rdbDPIU").is(':checked')) {
        //        if ($("#ddlSRRDAPIU").val() == 0) {
        //           // alert('Please select Nodal Agency and PIU');
        //            return false;
        //        }
        //    }
        //}


        if ($("#rdState").is(':checked')) {
            reportType = "S";
        }
        else if ($("#rdDistrict").is(':checked')) {
            reportType = "D";
        }

        if ($("#frmRegisterHead").valid()) {

            $.ajax({
                type: 'POST',
                url: '/Reports/GetPostRegisterDetails/',
                async: false,
                cache: false,
                data: $("#frmRegisterHead").serialize(),
                success: function (data) {

                   // LoadRunningAccountList();
                    $("#mainDiv").html(data);
                        $("#rptHeaderDesc").show();
                        
                        if ($("#rdDistrict").is(':checked')) {
                            $("#rdDistrict").trigger('click');
                        }

                        if ($("#rdbYear").is(':checked')) {
                            $("#rdbYear").trigger('click');
                        }

                        if ($("#rdbSRRDA").is(':checked')) {
                            $("#rdbSRRDA").trigger('click');
                        }

                        if ($("#ddlSRRDAPIU").val() > 0) {
                            
                            //if (levelId == 6) {
                            //    $("#ddlSRRDAPIU").trigger('change');
                                
                            //}
                        }
                        if ($("#rdbMonth").is(':checked')) {
                            monthly = true;
                        }
                        else if ($("#rdbYear").is(':checked')) {
                            yearly = true;
                        }

                        LoadRunningAccountList();
                        $("#spnMonthlyStateSRRDA").trigger("click");
                },
                error: function () {
                    alert('Error occurred while processing your request.');
                }
            });
            //LoadRunningAccountList();

        }

    });

    $("#ddlSRRDAPIU").change(function () {

        FillInCascadeDropdown(null, "#ddlPIU", "/Reports/PopulateDPIUOfSRRDA?id=" + $("#ddlSRRDAPIU").val());

    });


    $("#spnMonthlyStateSRRDA").click(function () {
        $("#spnMonthlyStateSRRDA").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triagle-n");
        $("#dvFilterForm").slideToggle("slow");
    });

    var specialElementHandlers = {
        '#editor': function (element,renderer) {
            return true;
        }
    };

    $("#btnPDF").click(function () {
        
        var doc = new jsPDF('landscape');
        //doc.fromHTML($('#tblstRegister').html(), 10, 10, {
        doc.fromHTML($('#tblstRegister').html(), 10, 10, {
                'width': 'auto','elementHandlers': specialElementHandlers
            });
            
            //doc.output('C:\Users\vikramn\Downloads\sample-file.pdf');
            //doc.save('sample-file.pdf');
        doc.text($('#tblstRegister').html());
            doc.output("dataurlnewwindow")
    });

    

    $("#ddlMonths").change(function () {

        UpdateAccountSession($("#ddlMonths").val(), $("#ddlYears").val());

    });

    $("#ddlYears").change(function () {

        UpdateAccountSession($("#ddlMonths").val(), $("#ddlYears").val());

    });

});
function LoadRunningAccountList() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    
    //$("#tblstRegister").html('');
    //$("#pagerlstRegister").html('');

    //$("#tblstRegister").html('');

    //$("#tblstRegister").trigger('reloadGrid');

    //trigger("reloadGrid");
    $("#tblstRegister").jqGrid('GridUnload');
    //$("#tblstRegister").jqGrid('trigger');


    //Added by Abhishek kamble to set year 9-July-2014 start
    var selectedYear;
    if ($("#rdbMonth").is(":checked")) {
        selectedYear = $("#ddlYears option:selected").val();
    }
    else {
        selectedYear = $("#ddlFinancialYears option:selected").val();
    }
    //Added by Abhishek kamble to set year 9-July-2014 end


    jQuery("#tblstRegister").jqGrid({
        url: '/Reports/ListRegisterDetails'  ,
        datatype: "json",
        mtype: "POST",
        //postData:$("#frmRegisterHead").serialize(),
        postData: { Month: $("#ddlMonths option:selected").val(), Year: selectedYear, HeadId: $("#ddlHeads option:selected").val(), ReportType: "D", DPIUCode: $("#ddlPIU option:selected").val(), FundingAgencyCode: $("#ddlAgency option:selected").val() },
        //colNames: ['Month and Year From which transaction Dated', 'Name of Contractor/Supplier(Contractor Id)', 'Agreement No.', 'Opening Balance (in Rs.)', 'Voucher/Transfer Entry', 'Credit (in Rs.)', 'Debit (in Rs.)', 'Balance (in Rs.)'],
        colNames: ['Month and Year From which transaction Dated', 'Name of Contractor/Supplier(Contractor Id)', 'Agreement No.', 'Opening Balance (in Rs.)', 'No', 'Date', 'in Rs.', 'in Rs.','in Rs.','Contractor','CD'],
        colModel: [
                            { name: 'FIRST_DATE', index: 'FIRST_DATE', height: 'auto', width: 40, align: "center", search: false, sortable: false },
                            { name: 'CONT_SUP_NAME', index: 'CONT_SUP_NAME', height: 'auto', width: 100, align: "left", cellattr: arrtSetting0, search: false, sortable: false },
                            { name: 'AGREEMENT_NUMBER', index: 'AGREEMENT_NUMBER', height: 'auto', width: 40, cellattr: arrtSetting, align: "left", sortable: false },
                            { name: 'OPENING_BALANCE', index: 'OPENING_BALANCE', height: 'auto', width: 40, align: "right", search: false, hidden: $("#Balance option:selected").val() == "C" ? true : false, sortable: false, formatter: "number", formatoptions: { decimalPlaces: 2 } },
                            { name: 'OB_BALANCE_AMT', index: 'OB_BALANCE_AMT', height: 'auto', width: 50, align: "center", search: false, sortable: false },
                            { name: 'BILL_NO', index: 'BILL_NO', height: 'auto', width: 50, align: "center", search: false, sortable: false },
                            { name: 'BILL_DATE', index: 'BILL_DATE', height: 'auto', width: 50, align: "right", search: false,sortable:false },
                            { name: 'AMOUNT', index: 'AMOUNT', height: 'auto', width: 50, align: "right", search: false, sortable: false},
                            { name: 'AMOUNT1', index: 'AMOUNT', height: 'auto', width: 50, align: "right", search: false, sortable: false},//, formatter: "number", formatoptions: { decimalPlaces: 2 } },
                            { name: 'CONTRACTOR', index: 'CONTRACTOR', height: 'auto', width: 50, align: "right", search: false, hidden: true },
                            { name: 'CREDITDEBIT', index: 'CREDIT_DEBIT', height: 'auto', width: 50, align: "right", search: false, hidden: true }

        ],
        pager: jQuery('#pagerlstRegister').width(20),
        rowNum: 999999,
        //rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "HEAD_CODE",
        sortorder: "asc",
        caption: "&nbsp;&nbsp;" + $("#ReportTitle").val(),
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
            //$("#tblstRegister").trigger('reloadGrid');

            $("#tblstRegister").parents('div.ui-jqgrid-bdiv').css("max-height", "325px");
//            alert();

           // $("#tblstRegister").jqGrid('destroyGroupHeader');

            jQuery("#tblstRegister").jqGrid('setGroupHeaders', {
                useColSpanStyle: true,
                groupHeaders: [
                  //{ startColumnName: 'OB_BALANCE_AMT', numberOfColumns: 4, titleText: $("#ddlMonths option:selected").text() +"-"+ $("#ddlYears option:selected").val()},
                  {
                      startColumnName: 'OB_BALANCE_AMT', numberOfColumns: 5,
                      titleText: '<table style="width:100%;border-spacing:0px"' +
                                '<tr><td id="h0" colspan="5">' + (monthly == true ? ($("#ddlMonths option:selected").text() + " - " + $("#ddlYears option:selected").val()) : ($("#ddlFinancialYears option:selected").val() + " - " + (parseInt($("#ddlFinancialYears option:selected").val()) + 1))) + '</td></tr>' +
                                '<tr>' +
                                    '<td id="h1" colspan="2" style="width:40%">Voucher/Transfer Entry</td>' +
                                    '<td id="h2" style="width:20%">' + fcol + '</td>' +
                                    '<td id="h3" style="width:20%">' + lcol + '</td>' +
                                    '<td id="h4" style="width:20%">Balance</td>' +
                                '</tr>' +
                                '</table>'
                  },
                ]
            });

            
        },
        gridComplete: function () {
            var grid = this;

            $('td[rowspan="1"]', grid).each(function () {
                var spans = $('td[rowspanid="' + this.id + '"]', grid).length + 1;

                if (spans > 1) {
                    $(this).attr('rowspan', spans).attr('vertical-align','central');
                }
            });

            var ids = jQuery("#tblstRegister").jqGrid('getDataIDs');
            var previousId = 0;
            
            //for (var i = 0; i < ids.length; i++) {
            //    var rowId = ids[i];
            //    var rowData = jQuery('#tblstRegister').jqGrid('getRowData', rowId);
            //    var previousrowData = jQuery('#tblstRegister').jqGrid('getRowData', parseInt(rowId - 1));
            //    if (rowData.CONTRACTOR == previousrowData.CONTRACTOR) {
            //        $("#tblstRegister").jqGrid('setCell', rowId, 'OPENING_BALANCE', previousrowData.AMOUNT1);
            //        if (rowData.CREDITDEBIT == "C") {
            //            var newBalance = parseFloat(previousrowData.AMOUNT1) + parseFloat(rowData.BILL_DATE) - parseFloat(rowData.AMOUNT);
            //            $("#tblstRegister").jqGrid('setCell', rowId, 'AMOUNT1', parseFloat(newBalance).toFixed(2).toString().toLocaleString("en-IN"));
            //        }
            //        else if (rowData.CREDITDEBIT == "D")
            //        {
            //            var newBalance = parseFloat(previousrowData.AMOUNT1) + parseFloat(rowData.BILL_DATE) - parseFloat(rowData.AMOUNT);
            //            $("#tblstRegister").jqGrid('setCell', rowId, 'AMOUNT1', parseFloat(newBalance).toFixed(2).toString().toLocaleString("en-IN"));
            //        }
            //    }
            //}
           
            $.unblockUI();
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
        //,
        //grouping: true,
        //groupingView: {
        //    groupField: ['HEAD_CODE', 'HEAD_NAME', 'HEAD_COMP_PROGRESS', 'IMS_COLLABORATION'],
        //    groupText: ['<b>{0}</b>'],
        //    groupColumnShow: [false,false,false,false]
        //    //groupCollapse: true,
        //}
        resizeStop: function () {
        var $self = $(this),
            shrinkToFit = $self.jqGrid("getGridParam", "shrinkToFit");

        $self.jqGrid("setGridWidth", this.grid.newWidth, shrinkToFit);
        setHeaderWidth.call(this);
    }
    });

    

    $("th[title=CreditDebitBalance]").removeAttr("title");
    $("#h0").css({
        borderBottomWidth: "1px",
        borderBottomColor: "#c5dbec", // the color from jQuery UI which you use
        borderBottomStyle: "solid",
        padding: "4px 0 6px 0"
    });
    $("#h1").css({
        borderRightWidth: "1px",
        borderRightColor: "#c5dbec", // the color from jQuery UI which you use
        borderRightStyle: "solid",
        padding: "4px 0 4px 0"
    });
    $("#h2").css({
        borderRightWidth: "1px",
        borderRightColor: "#c5dbec", // the color from jQuery UI which you use
        borderRightStyle: "solid",
        padding: "4px 0 4px 0"
    });
    $("#h3").css({
        borderRightWidth: "1px",
        borderRightColor: "#c5dbec", // the color from jQuery UI which you use
        borderRightStyle: "solid",
        padding: "4px 0 4px 0"
    });
    $("#h4").css({
        padding: "4px 0 4px 0"
    });
    setHeaderWidth.call(grid[0]);

    $.unblockUI();
}

var prevCellVal = { cellId: undefined, value: undefined };

arrtSetting = function (rowId, val, rawObject, cm, rdata) {
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
function FillInCascadeDropdown(map, dropdown, action) {

    $(dropdown).empty();
    $.post(action, map, function (data) {

        $.each(data, function () {
            if (this.Selected == true)
            { $(dropdown).append("<option value='" + this.Value + "' selected =" + this.Selected + ">" + this.Text + "</option>"); }
            else { $(dropdown).append("<option value='" + this.Value + "'>" + this.Text + "</option>"); }
        });
    }, "json");
}
var grid = $("#tblstRegister"),
setHeaderWidth = function () {
    var $self = $(this),
        colModel = $self.jqGrid("getGridParam", "colModel"),
        cmByName = {},
        ths = this.grid.headers, // array with column headers
        cm,
        i,
        l = colModel.length;

    // save width of every column header in cmByName map
    // to make easy access there by name
    for (i = 0; i < l; i++) {
        cm = colModel[i];
        cmByName[cm.name] = $(ths[i].el).outerWidth();
    }
    // resize headers of additional columns based on the size of
    // the columns below the header
    //$("#h1").width(cmByName.No + cmByName.Date + cmByName.total - 1);
    $("#h1").width(cmByName.No + cmByName.Date - 1);
    //$("#h2").width(cmByName.in_Rs - 1);
    //$("#h3").width(cmByName.in_Rs - 1);
    //$("#h4").width(cmByName.in_Rs - 1);
};

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