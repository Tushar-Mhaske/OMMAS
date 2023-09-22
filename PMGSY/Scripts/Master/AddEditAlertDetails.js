jQuery.validator.addMethod("datecomparefieldvalidator", function (value, element, param) {

    var fromDate = $('#DISPLAY_START_DATE').val();
    var toDate = $('#DISPLAY_END_DATE').val();

    if (toDate == "") {
        return true;
    }
    else {        
        var frommonthfield = fromDate.split("/")[1];
        var fromdayfield = fromDate.split("/")[0];
        var fromyearfield = fromDate.split("/")[2];

        var tomonthfield = toDate.split("/")[1];
        var todayfield = toDate.split("/")[0];
        var toyearfield = toDate.split("/")[2];

        var sDate = new Date(fromyearfield, frommonthfield, fromdayfield);
        var eDate = new Date(toyearfield, tomonthfield, todayfield);

        if (sDate > eDate) {
            return false;
        }
        else {
            return true;
        }
    }

});

jQuery.validator.unobtrusive.adapters.addBool("datecomparefieldvalidator");


$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmAlertDetails");

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#DISPLAY_START_DATE').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a start date',
        buttonImageOnly: true,
        buttonText: 'Start Date',
        changeMonth: true,
        changeYear: true,
        minDate:$("#SystemDate").val(),
        onSelect: function (selectedDate) {
            $("#DISPLAY_END_DATE").datepicker("option", "minDate", selectedDate);
            $(function () {
                $('#DISPLAY_START_DATE').focus();
                $('#DISPLAY_END_DATE').focus();
            })
        }
    });

    $('#DISPLAY_END_DATE').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a end date',
        buttonImageOnly: true,
        buttonText: 'End Date',
        changeMonth: true,
        changeYear: true,
        minDate:$("#SystemDate").val(),
        onSelect: function (selectedDate) {
            $("#DISPLAY_START_DATE").datepicker("option", "maxDate", selectedDate);
        }

    });




    $("#iconClose").click(function () {
        if ($("#dvAddAlertDetails").is(':visible')) {
            $("#dvAddAlertDetails").hide('slow');
            $("#btnCreateNew").show('slow');
            $("#dvSearchAlertDetails").show();
            $("#trSearchDetails").hide();

        }

    });

    $("#btnSaveAlertDetails").click(function (e) {
        
        if ($("#frmAlertDetails").valid()) {
        
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/AddAlertDetails/',
                async: false,
                data: $("#frmAlertDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        //$('#tblAlertDetails').trigger('reloadGrid');
                        //$("#dvAddAlertDetails").load("/Master/AddEditAlertDetails");
                        if ($("#dvSearchAlertDetails").is(':hidden')) {
                            $("#dvSearchAlertDetails").show();
                            $("#dvAddAlertDetails").hide('slow');
                            $("#btnCreateNew").show();
                            $("#trSearchDetails").hide();
                        }
                        $('#tblAlertDetails').trigger('reloadGrid');
                        $.unblockUI();
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                            $.unblockUI();
                        }
                    }
                    else {
                        $("#dvAddAlertDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert(xhr.responseText);
                    $.unblockUI();
                }
            })
        }
    });

    $('#btnUpdateAlertDetails').click(function (e) {

        if ($('#frmAlertDetails').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/Master/EditAlertDetails/",
                type: "POST",
                data: $("#frmAlertDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        //$('#tblAlertDetails').trigger('reloadGrid');
                        //$("#dvAddAlertDetails").load("/Master/AddEditAlertDetails");
                        if ($("#dvSearchAlertDetails").is(':hidden')) {
                            $("#dvSearchAlertDetails").show();
                            $("#dvAddAlertDetails").hide('slow');
                            $("#btnCreateNew").show();
                            $("#trSearchDetails").hide();
                        }
                        $('#tblAlertDetails').trigger('reloadGrid');
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvAddAlertDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();
                }
            });
        }
    });


    $("#btnResetAlertDetails").click(function () {
        $("#dvErrorMessage").hide('slow');

        //Clear Date Picker
        var dates = $("input[id$='DISPLAY_START_DATE'],input[id$='DISPLAY_END_DATE']");
        dates.attr('value', '');
        dates.each(function () {
            $.datepicker._clearDate(this);
        });
        $("#DISPLAY_END_DATE").datepicker("option", "minDate", $("#SystemDate").val());
    });

    $('#btnCancelAlertDetails').click(function (e) {
    //    $.ajax({
    //        url: "/Master/AddEditAlertDetails",
    //        type: "GET",
    //        dataType: "html",
    //        success: function (data) {
    //            $("#dvAddAlertDetails").html(data);
    //        },
    //        error: function (xhr, ajaxOptions, thrownError) {
    //            //alert(xhr.responseText);
    //        }
        //    });
        if ($("#dvSearchAlertDetails").is(':hidden')) {
            $("#dvSearchAlertDetails").show();
            $("#dvAddAlertDetails").hide('slow');
            $("#btnCreateNew").show();
            $("#trSearchDetails").hide();
        }

        });
       
});

function searchAlertCreateDetails() {
  
    $('#tblAlertDetails').setGridParam({
        url: '/Master/ListAlertsDetails', datatype: 'json'
    });
    $('#tblAlertDetails').jqGrid("setGridParam", { "postData": { Status: $('#ddlStatus option:selected').val() } });
    $('#tblAlertDetails').trigger("reloadGrid", [{ page: 1 }]);
}