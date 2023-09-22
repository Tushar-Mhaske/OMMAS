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

    if ($("#LevelId").val() == 4 || $("#LevelId").val() == 6)
    {
        $('#spanPIU').hide();
    }

    if ($("#LevelId").val() == 4 || $("#LevelId").val()==5)
    {
        $('#spanState').hide();
        //$('#rdbSrrda').attr('checked', true);

        //Modified By Abhishek Kamle 24-dec-2013
        if (!($('#rdbSrrda').is(":checked")) && (!($('#rdbPiu').is(":checked")))) {
            $('#rdbSrrda').attr('checked', true);
        }
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

        //added by abhishek kamble 3-dec-2013
        if ($("#rdbMonth").is(":checked")) {
            if (($("#Month option:selected").val() == "0")) {
                return false;
            }
        }
        if (($("#Year option:selected").val() == "0")) {
            return false;
        }

        $("#lblAgency").text($("#Agency option:selected").text());

        if (($('#Piu option:selected').val() == '0'))
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
                data = { Month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndCode: $("#Agency option:selected").val(), rlevel: 4, allPiu: 0 };
            }
            else {
                data = { Month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndCode: $('#Agency option:selected').val(), rlevel: 4, allPiu: $("#Piu option:selected").val() };
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
                data = { Month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndCode: $("#Agency option:selected").val(), rlevel: 1, allPiu: 0 };            
            ValidateForm(data);
        }
        // selection- DPIU & Annual
        if ($("#rdbPiu").is(":checked") && $("#rdbAnnual").is(":checked")) {
            if ($("#Piu option:selected").val() == 0) {
                data = { month: 1, Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 2, AllPiu: 0, duration: 1 };
            }
            else {
                data = { month: 1, Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 2, AllPiu: $("#Piu option:selected").val(), duration: 1 };
            }
            ValidateForm(data);
        }

        // selection- DPIU & Monthly
        if ($("#rdbPiu").is(":checked") && $("#rdbMonth").is(":checked")) {
            if ($("#Piu option:selected").val() == 0) {
                data = { month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 2, AllPiu: 0 };
            }
            else {
                data = { month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), rlevel: 2, AllPiu: $("#Piu option:selected").val() };
            }
            ValidateForm(data);
        }

        // Login DPIU & selection- Annual
        if ($("#LevelId").val() == 5 && $("#rdbAnnual").is(":checked")) {           
            data = { month: 1, Year: $("#Year option:selected").val(), ndcode: $("#ParentNdCode").val(), rlevel: 2, AllPiu: $("#AdminNdCode").val(), duration: 1 };
            ValidateForm(data);
        }

        // Login DPIU & selection- Monthly
        if ($("#LevelId").val() == 5 && $("#rdbMonth").is(":checked")) {           
            data = { month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndcode: $("#ParentNdCode").val(), rlevel: 2, AllPiu: $("#AdminNdCode").val(), duration: 2 };
            ValidateForm(data);
        }
    });  
});


function ValidateForm(param) {

    //alert(param);
    $.ajax({
        url: "/Reports/ValidateParameter",
        type: "POST",
        data: param,
        success: function (data) {
            if (data.success == false) {
                $("#divError").show();
                $("#divError").html('<strong>Alert : </strong>' + data.message);
                
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

    jQuery('#tbScheduleDurableAssetsList').jqGrid("GridUnload");

    $("#tbScheduleDurableAssetsList").jqGrid({

        url: '/Reports/GetDurableAssetsDetails',
        datatype: 'json',
        mtype: 'POST',
        colNames: ["Schedule Of Durable Assets", "Head Name", "Opening Balance ( Amount In Rs. )", "Addition During the year <br/>( Amount In Rs. )", "Total ( Amount In Rs. )", "Disposal During the year", "Balance ( Amount In Rs. )"],
        colModel: [
             { name: 'DurableAssets', index: 'DurableAssets', width: '80%', sortable: false, align: "left" },
             { name: 'HeadName', index: 'HeadName', width: '80%', sortable: false, align: "left"},
             { name: 'OB', index: 'OB', width: '80%', sortable: false, align: "right", formatter: 'number', summaryType: 'sum', summaryTpl: 'Total: {0}'  },
             { name: 'AdditionDuringTheYear', index: 'AdditionDuringTheYear', width: '80%', sortable: false, align: "right", formatter: 'number', summaryType: 'sum', summaryTpl: 'Total: {0}' },
             { name: 'Total', index: 'Total', width: '80%', sortable: false, align: "right", formatter: 'number', summaryType: 'sum', summaryTpl: 'Total: {0}' },
             { name: 'DisposalDuringTheYear', index: 'DisposalDuringTheYear', width: '80%', sortable: false, align: "right"},
             { name: 'Balance', index: 'Balance', width: '80%', sortable: false, align: "right", formatter: 'number', summaryType: 'sum', summaryTpl: 'Total: {0}' }
             //{ name: 'Amount', index: 'Amount', width: '20%', sortable: false, align: "right",formatter:formatAmountColumn},
             //{ name: 'This_Amt', index: 'This_Amt', width: 200, sortable: false, align: "right", editable: true, formatter: "number", formatoptions: { decimalPlaces: 2 } },
        ],
        
        //rowNum: 1020,
        viewrecords: true,
        postData: data,
        caption: "&nbsp;&nbsp;Schedule of Durable Assets Details",
        height: 'auto',
        //  maxheight: 100,
        pager: jQuery("#dvPagerScheduleDurableAssets"),
        pginput: false,
        pgbuttons:false,
        shrinktofit: true,
        width: '98%',
        autowidth: true,
        rownumbers: true,
        footerrow: true,
        grouping: true,
        groupingView: {
            groupSummary: true,
            groupField: ['DurableAssets'],
            groupColumnShow: false,
            groupText: ["<h3>{0} </h3>"]
        },


        //userDataOnFooter: true,
        //jsonReader: {
        //    repeatitems: false,
        //    total: "total",
        //    records: "records",
        //    page: "page",
        //    root: "rows",
        //    cell: "",
        //    id: "0"
        //},
        loadComplete: function (jsonData) {
           
            $("#tblHeading").show();

            //set header info
            $("#lblHeaderFormNumber").text(jsonData.reportHeader.formNumber);
            $("#lblHeaderFundType").text(jsonData.reportHeader.fundType);
            $("#lblReportHeader").text(jsonData.reportHeader.reportHeading);
            $("#lblReportReference").text(jsonData.reportHeader.refference);
                        
            ////bold grid information for PF
            //jQuery("#tbScheduleDurableAssetsList").jqGrid('setCell', 'Part I:Assets from the ...strative expenses Fund ', 'DurableAssets', "", { 'font-size': '13px', 'font-weight': 'bold' });
            //jQuery("#tbScheduleDurableAssetsList").jqGrid('setCell', 'Part II:Assets from the State administrative expenses Fund - (5) ', 'DurableAssets', "", { 'font-size': '13px', 'font-weight': 'bold' });
            //jQuery("#tbScheduleCurrentAssetsList").jqGrid('setCell', '(a) Against Contractors ', 'Particulars', "", { 'font-size': '12px', 'font-weight': 'bold' });
            //jQuery("#tbScheduleCurrentAssetsList").jqGrid('setCell', '(b) Against Staff ', 'Particulars', "", { 'font-size': '12px', 'font-weight': 'bold' });
            //jQuery("#tbScheduleCurrentAssetsList").jqGrid('setCell', '(c) Advances for DPR Preparation ', 'Particulars', "", { 'font-size': '12px', 'font-weight': 'bold' });
            //jQuery("#tbScheduleCurrentAssetsList").jqGrid('setCell', '(C) Administrative Expenses Recoverable from the State Government ', 'Particulars', "", { 'font-size': '13px', 'font-weight': 'bold' });
            //jQuery("#tbScheduleCurrentAssetsList").jqGrid('setCell', '(D) Other Items', 'Particulars', "", { 'font-size': '13px', 'font-weight': 'bold' });
            //jQuery("#tbScheduleCurrentAssetsList").jqGrid('setCell', 'Grand Total ( A +B + C +D) ', 'Particulars', "", { 'font-size': '13px', 'font-weight': 'bold' });
                        
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


//function formatAmountColumn(cellvalue, options, rowObject) {
    
//    if (cellvalue == null) {
//        return '';
//    } else {

//        if (cellvalue % 1 == 0)
//        {
//            cellvalue += ".00";
//        }

//        return cellvalue;
//    }
//}

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