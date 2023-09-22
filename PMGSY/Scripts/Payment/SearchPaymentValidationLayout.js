$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmSearchPaymentValidationLayout')

    var date = new Date();
    var toDate = new Date((parseInt(date.getMonth()) == 11 ? parseInt(date.getFullYear() + 1) : date.getFullYear()),
                            (parseInt(date.getMonth()) == 11 ? 1 : parseInt(date.getMonth() + 2)),
                          0);
    //var toDate = parseInt(date.getMonth()) == 11 ? (new Date(parseInt(date.getFullYear() + 1), 0, 0)) : (new Date(parseInt(date.getFullYear() + 1), parseInt(date.getMonth() + 2), 0));
    console.log('date=' + date);
    console.log('toDate=' + toDate);
    console.log('getMonth=' + parseInt(date.getMonth()));
    console.log('getFullYear=' + parseInt(date.getFullYear()));

    PopulateDPIU();
    $("#ddlSRRDA").change(function () {
        $("#rdoAllRd").trigger('click');
        PopulateDPIU();
    });

    $('#txtFromDt').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a start date',
        buttonImageOnly: true,
        buttonText: 'Start Date',
        changeMonth: true,
        changeYear: true,
        //minDate: $('#agreementDate').val(),
        maxDate: new Date(),
        //minDate: new Date(),//startDate,
        //minDate: new Date(currentYear, currentMonth, currentDate),
        onSelect: function (selectedDate) {
            $("#txtToDt").datepicker("option", "minDate", selectedDate);
            $(function () {
                $('#txtFromDt').focus();
                $('#txtToDt').focus();
            })
            $('#txtFromDt').trigger('blur');
        }
    });

    $('#txtToDt').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a start date',
        buttonImageOnly: true,
        buttonText: 'Start Date',
        changeMonth: true,
        changeYear: true,
        //minDate: $('#agreementDate').val(),
        //maxDate: new Date(),
        maxDate: toDate,
        //minDate: new Date(),//startDate,
        //minDate: new Date(currentYear, currentMonth, currentDate),
        onSelect: function (selectedDate) {
            //$("#txtNewsPublishEnd").datepicker("option", "minDate", selectedDate);
            //$(function () {
            //    $('#txtNewsPublishSt').focus();
            //    $('#txtNewsPublishEnd').focus();
            //})
            $('#txtToDt').trigger('blur');
        }
    });

    if ($("#rdoSRRDA").is(":checked") /*&& ($("#levelId").val() != 5)*/) {
        //Add Validation rule for dpiu required
        $("#ddlDPIU").trigger('blur');

    }

    $("#rdoSRRDA").click(function () {
        $("#ddlDPIU").hide("slow");
        $("#ddlDPIU option:first").attr("selected", "selected");

        $("#ddlDPIU").trigger('blur');
        //$('#erDPIU').text('');

    });

    $("#rdoSRRDA").trigger('click');

    $("#rdoDPIU").click(function () {
        $("#ddlDPIU").show("slow");

    });

    $('#btnSearch1').click(function () {
        //alert($("#frmSearchPaymentValidationLayout").valid());
        if (!$("#frmSearchPaymentValidationLayout").valid()) {
            return false;
        }
        //alert($('#ddlDPIU').val());
        if ($("#rdoDPIU").is(":checked") && (parseInt($('#ddlDPIU').val()) < 0)) {
            alert('Please select DPIU');
            return false;
        }

        LoadValidationDetails();
    });

    $('#btnReset').click(function () {
        $("#rdoAllRd").trigger('click');
        $("#rdoSRRDA").trigger('click');
    });
});

function LoadValidationDetails() {
    //alert(1);
    jQuery("#tblValidationDetailsList").jqGrid('GridUnload');

    jQuery("#tblValidationDetailsList").jqGrid({
        url: '/Payment/GetValidationDetails',
        datatype: "json",
        mtype: "GET",
        postData: { AdminNdCode: $("#rdoSRRDA").is(":checked") ? $("#ddlSRRDA option:selected").val() : ($("#rdoDPIU").is(":checked") && $("#ddlDPIU option:selected").val() == 0 ? $("#ddlSRRDA option:selected").val() : $("#ddlDPIU option:selected").val()), frmDt: $('#txtFromDt').val(), toDt: $('#txtToDt').val() },
        colNames: ['Department', 'Road Name', 'From Date', 'To Date'],
        colModel: [
                            { name: 'ADMIN_ND_NAME', index: 'ADMIN_ND_NAME', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'RoadName', index: 'RoadName', height: 'auto', width: 250, align: "center", search: false },
                            { name: 'FROM_DATE', index: 'FROM_DATE', height: 'auto', width: 100, align: "center", search: true },
                            { name: 'TO_DATE', index: 'TO_DATE', height: 'auto', width: 100, align: "center", search: false },
                            //{ name: 'b', width: 50, align: "center", search: false },

        ],
        pager: jQuery('#divValidationDetailsListPager'),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "ADMIN_ND_NAME",
        sortorder: "desc",
        caption: "&nbsp;&nbsp; Validation Details List",
        height: 'auto',
        hidegrid: true,
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

    //jQuery("#tbFinancialRoadList").jqGrid('setGroupHeaders', {
    //    useColSpanStyle: false,
    //    groupHeaders: [
    //      { startColumnName: 'EXEC_VALUEOFWORK_LASTMONTH', numberOfColumns: 3, titleText: 'Value of Work Done(Rs. in Lakh)' },
    //      { startColumnName: 'EXEC_PAYMENT_LASTMONTH', numberOfColumns: 3, titleText: 'Payment Made(Rs. in Lakh)' }
    //    ]
    //});

}

function PopulateDPIU() {
    var adminNdCode = $('#ddlSRRDA option:selected').val();
    $.ajax({
        url: '/Payment/PopulateDPIUForCashBook/' + adminNdCode,
        type: 'GET',
        catche: false,
        error: function (xhr, status, error) {
            alert('An Error occured while processig your request.')
            return false;
        },
        success: function (data) {
            $('#ddlDPIU').empty();
            $.each(data, function () {
                $('#ddlDPIU').append("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
        }
    });
}