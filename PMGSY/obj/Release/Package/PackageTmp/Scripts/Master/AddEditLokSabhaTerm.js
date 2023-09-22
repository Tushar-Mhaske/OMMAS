jQuery.validator.addMethod("datecomparefieldvalidator", function (value, element, param) {

    var fromDate = $('#MAST_LS_START_DATE').val()
    var toDate = $('#MAST_LS_END_DATE').val();

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

});

jQuery.validator.unobtrusive.adapters.addBool("datecomparefieldvalidator");

$(document).ready(function () {
   $.validator.unobtrusive.parse('#frmAddLokSabhaTerm');

  
    $('#MAST_LS_START_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a from date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Start Date',
        onSelect: function (selectedDate) {
            $("#MAST_LS_END_DATE").datepicker("option", "minDate", selectedDate);
            $(function () {
                $('#MAST_LS_START_DATE').focus();
                $('#MAST_LS_END_DATE').focus();
            })
        }
    });

    $('#MAST_LS_END_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a from date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'End Date',
        onSelect: function (selectedDate) {
            $("#MAST_LS_START_DATE").datepicker("option", "maxDate", selectedDate);
        }
    });


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });


    $("#btnSave").click(function (e) {

        
        if ($("#frmAddLokSabhaTerm").valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                type: 'POST',
                url: '/Master/AddLokSabhaTerm/',
                async: false,
                data: $("#frmAddLokSabhaTerm").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                        //$("#loksabhaDetails").load('/Master/AddLokSabhaTerm/');
                        //$('#loksabhaCategory').trigger('reloadGrid');
                        $("#btnCreateNew").show();
                        $('#loksabhaDetails').hide();
                        $('#loksabhaCategory').trigger('reloadGrid');
                        $.unblockUI();
                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                            $.unblockUI();
                        }
                       
                    }
                    else {
                        $("#loksabhaDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            })
        }
    });

    $("#MAST_LS_START_DATE").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#MAST_LS_END_DATE").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    //$("#dvhdAddNewLokSabhaDetails").click(function () {

    //    if ($("#dvAddNewLokSabhaDetails").is(":visible")) {

    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

    //        $(this).next("#dvAddNewLokSabhaDetails").slideToggle(300);
    //    }

    //    else {
    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

    //        $(this).next("#dvAddNewLokSabhaDetails").slideToggle(300);
    //    }
    //});

    $("#spCollapseIconCN").click(function () {

        if ($("#loksabhaDetails").is(":visible")) {
            $("#loksabhaDetails").hide("slow");

            $("#btnCreateNew").show();
        }
    });

    $("#btnCancel").click(function (e) {

        //$.ajax({
        //    url: "/Master/AddLokSabhaTerm",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
             
        //        $("#loksabhaDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});
        $("#btnCreateNew").show();
        $('#loksabhaDetails').hide();
     
    })

    var dates = $("input[id$='MAST_LS_START_DATE'],input[id$='MAST_LS_END_DATE']");
    $("#btnReset").click(function (e) {

        ClearDetails();

        dates.attr('value', '');
        dates.each(function () {
            $.datepicker._clearDate(this);
        });
    });

    $("#btnUpdate").click(function (e) {
        if ($("#frmAddLokSabhaTerm").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/EditLokSabhaTerm/',
                async: false,
                data: $("#frmAddLokSabhaTerm").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                   
                        //$('#loksabhaDetails').load("/Master/AddLokSabhaTerm");
                        //$('#loksabhaCategory').trigger('reloadGrid');
                        $("#btnCreateNew").show();
                        $('#loksabhaDetails').hide();
                        $('#loksabhaCategory').trigger('reloadGrid');
                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#loksabhaDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            })
        }
    });
});

function ClearDetails() {
    $('#MAST_LS_START_DATE').val('');
    $('#MAST_LS_END_DATE').val('');
    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}

