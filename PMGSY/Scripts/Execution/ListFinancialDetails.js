var isValid;
$(document).ready(function () {

    $("#dvRoadDetails").dialog({
        autoOpen: false,
        // height:550,
        width: 950,
        modal: true,
        show: {
            effect: "blind",
            duration: 1000
        },
        hide: {
            effect: "explode",
            duration: 1000
        }

    });


    $(":input").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    })

    var IMS_ROAD_CODE = $("#prRoadCode").val();
    var PROGRESS = $("#prType").val();
    $("#tbFinancialList").jqGrid('GridUnload');
    LoadFinancialDetails(IMS_ROAD_CODE,PROGRESS);

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $('#imgCloseFinancetDetails').click(function () {


        if ($("#accordionFinance").is(":visible")) {
            $('#accordionFinance').hide('slow');
        }

        ViewSearchDiv();
        $('#tbProposedRoadList').jqGrid("setGridState", "visible");

        $("#dvAgreement").animate({
            scrollTop: 0
        });

    });

});

function ViewSearchDiv() {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    if (!$("#dvSearchProposedRoad").is(":visible")) {

        var data = $('#tbProposedRoadList').jqGrid("getGridParam", "postData");

        if (!(data === undefined)) {

            $('#ddlFinancialYears').val(data.sanctionedYear);
            $('#ddlBlocks').val(data.blockCode);
        }

        $("#dvSearchProposedRoad").show('slow');
        $.unblockUI();
    }
    $.unblockUI();

}

function ViewDetails()
{
    $("#tbRoadDetails").jqGrid('GridUnload');
    LoadAgreementDetails();

    $("#dvRoadDetails").dialog('open');
}

function LoadAgreementDetails()
{
    jQuery("#tbRoadDetails").jqGrid({
        url: '/Execution/GetRoadAgreementDetailsList',
        datatype: "json",
        mtype: "POST",
        postData: { roadCode: $("#prRoadCode").val(), progressType: $("#prType").val() },
        colNames: ['Agreement No.','Work Description','Split Length','Split Cost', 'Agreement Date','Agreement Amount' ,'Agreement Status', 'Value of Work Done'],
        colModel: [
                            { name: 'TEND_AGREEMENT_NUMBER', index: 'TEND_AGREEMENT_NUMBER', height: 'auto', width: 150, align: "left", search: false },
                            { name: 'IMS_WORK_DESC', index: 'IMS_WORK_DESC', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'IMS_PAV_LENGTH', index: 'IMS_PAV_LENGTH', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'IMS_PAV_EST_COST', index: 'IMS_PAV_EST_COST', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'TEND_DATE_OF_AGREEMENT', index: 'TEND_DATE_OF_AGREEMENT', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'TEND_AGREEMENT_AMOUNT', index: 'TEND_AGREEMENT_AMOUNT', height: 'auto', width: 100, align: "right", search: true },
                            { name: 'TEND_AGREEMENT_STATUS', index: 'TEND_AGREEMENT_STATUS', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'TEND_VALUE_WORK_DONE', index: 'TEND_VALUE_WORK_DONE', height: 'auto', width: 100, align: "center", search: false },

        ],
        pager: jQuery('#pgRoadDetails'),
        rowNum: 5,
        rowList: [10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "TEND_AGREEMENT_NUMBER",
        sortorder: "desc",
        caption: "&nbsp;&nbsp; Agreement list with details",
        height: 'auto',
        hidegrid: false,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {

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
}


function LoadFinancialDetails(IMS_ROAD_CODE,PROGRESS) {

    jQuery("#tbFinancialList").jqGrid({
        url: '/Execution/GetFinancialProgressList',
        datatype: "json",
        mtype: "POST",
        postData: { roadCode: IMS_ROAD_CODE ,progressType:PROGRESS},
        colNames: ['Year', 'Month', 'Upto Last Month', 'During This Month', 'Total', 'Upto Last Month', 'During This Month', 'Total', 'Is Final Payment Made', 'Date', 'Edit', 'Delete'],
        colModel: [
                            { name: 'EXEC_PROG_YEAR', index: 'EXEC_PROG_YEAR', height: 'auto', width: 55, align: "left", search: false },
                            { name: 'EXEC_PROG_MONTH', index: 'EXEC_PROG_MONTH', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_VALUEOFWORK_LASTMONTH', index: 'EXEC_VALUEOFWORK_LASTMONTH', height: 'auto', width: 100, align: "left", search: true },
                            { name: 'EXEC_VALUEOFWORK_THISMONTH', index: 'EXEC_VALUEOFWORK_THISMONTH', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'TOTAL', index: 'TOTAL', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'EXEC_PAYMENT_LASTMONTH', index: 'EXEC_PAYMENT_LASTMONTH', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'EXEC_PAYMENT_THISMONTH', index: 'EXEC_PAYMENT_THISMONTH', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'TOTAL_PAYMENT', index: 'TOTAL_PAYMENT', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'EXEC_FINAL_PAYMENT_FLAG', index: 'EXEC_FINAL_PAYMENT_FLAG', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'EXEC_FINAL_PAYMENT_DATE', index: 'EXEC_FINAL_PAYMENT_DATE', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'a', width: 50, align: "center", search: false },
                            { name: 'b', width: 50, align: "center", search: false, hidden: true },

        ],
        pager: jQuery('#pagerFinancialList'),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname:"EXEC_PROG_YEAR,EXEC_PROG_MONTH",
        sortorder: "desc",
        caption: "&nbsp;&nbsp; Financial Progress List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
            
            $("#tbFinancialList #pagerFinancialList").css({ height: '40px' });
            $("#pagerFinancialList_left").html("<input type='button' style='margin-left:27px' id='idAddFinancialProgress' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddFinancialProgress(" + IMS_ROAD_CODE+ ");return false;' value='Add Financial Progress'/>")

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

    jQuery("#tbFinancialList").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'EXEC_VALUEOFWORK_LASTMONTH', numberOfColumns: 3, titleText: 'Value of Work Done(Rs. in Lakh)' },
          { startColumnName: 'EXEC_PAYMENT_LASTMONTH', numberOfColumns: 3, titleText: 'Payment Made(Rs. in Lakh)' }
        ]
    });

}
function EditFinancialProgress(urlparameter) {
    $("#divAddFinancialProgress").load("/Execution/EditFinancialDetails/" + urlparameter, function (data) {
        if ((data.success == false)) {
            alert(data.message);
        }
        $("#divAddFinancialProgress").show();
        $.validator.unobtrusive.parse($('#frmAddFinancialProgress'));
    });
}

function DeleteFinancialProgress(urlparameter) {

    if (confirm("Are you sure you want to delete Financial details?")) {
        $.ajax({
            type: 'POST',
            url: '/Execution/DeleteFinancialDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert("Financial progress details deleted successfully");
                    $("#tbFinancialList").trigger('reloadGrid');
                    $("#divAddFinancialProgress").html('');
                }
                else if (data.success == false) {
                    alert("Financial progress details is in use and can not be deleted.");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.responseText);
            }
        });
    }
    else {
        return false;
    }

}
function AddFinancialProgress(IMS_ROAD_CODE) {

    var PROGRESS = $("#prType").val();

    var Code = IMS_ROAD_CODE + "," + PROGRESS

    $("#divAddFinancialProgress").load("/Execution/AddFinancialProgress?id=" + Code, function () {

        $("#divAddFinancialProgress").show();
        $.validator.unobtrusive.parse($('#frmAddFinancialProgress'));
    });

    /*

    var PROGRESS = $("#prType").val();

    var Code = IMS_ROAD_CODE + "," + PROGRESS

    ValidateRoadDetails(Code);

    if (isValid == true)
    {
        $("#divAddFinancialProgress").load("/Execution/AddFinancialProgress?id=" + Code, function () {

            $("#divAddFinancialProgress").show();
            $.validator.unobtrusive.parse($('#frmAddFinancialProgress'));
        });
    }
    else if (isValid == false)
    {
        return false;
    }
    */
}

function ValidateRoadDetails(IMS_PR_ROAD_CODE) {
    $.ajax({

        type: 'POST',
        url: '/Execution/CheckFinancialDetails/' + IMS_PR_ROAD_CODE,
        async: false,
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (data.success == false) {
                alert(data.message);
                isValid = false;
                return false;
            }
            else if (data.success == true) {
                isValid = true;
                return true;
            }
        },
        error: function () {
            alert('Error occurred while processing your request.');
        }
    });
}