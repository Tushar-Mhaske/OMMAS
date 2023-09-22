jQuery.validator.addMethod("connectivitystatusvalidator", function (value, element, param) {

    var tt1 = parseInt($('#txtt11').val());
    var tt2 = parseInt($('#txtt12').val());
    var tt3 = parseInt($('#txtt13').val());
    var tt4 = parseInt($('#txtt14').val());
    var tt4 = parseInt($('#txtt14').val());
    var ttEl499 = parseInt($('#txtt1El499').val());
    var ttEl249 = parseInt($('#txtt1El249').val());

    var ct1 = parseInt($('#txtt21').val());
    var ct2 = parseInt($('#txtt22').val());
    var ct3 = parseInt($('#txtt23').val());
    var ct4 = parseInt($('#txtt24').val());
    var ctEl499 = parseInt($('#txtt2El499').val());
    var ctEl249 = parseInt($('#txtt2El249').val());

    if (tt1 < ct1) {
        //alert('ct1=' + ct1);
        //alert('tt1=' + tt1);
        //alert('Connected Habitations cannot be more than total habitations');
        return false;
    }
    else if (ct2 > tt2) {   
        return false;
    }
    else if (ct3 > tt3) {
        return false;
    }
    else if (ct4 > tt4) {
        return false;
    }
    else if (ctEl499 > ttEl499) {
        return false;
    }
    else if (ctEl249 > ttEl249) {
        return false;
    }
    else {
        return true;
    }
    return true;
});

jQuery.validator.unobtrusive.adapters.addBool("connectivitystatusvalidator");


$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmConnectivityStatus'));

    //Removes validation from input-fields
    $('.input-validation-error').addClass('input-validation-valid');
    $('.input-validation-error').removeClass('input-validation-error');
    //Removes validation message after input-fields
    $('.field-validation-error').addClass('field-validation-valid');
    $('.field-validation-error').removeClass('field-validation-error');
    //Removes validation summary 
    $('.validation-summary-errors').addClass('validation-summary-valid');
    $('.validation-summary-errors').removeClass('validation-summary-errors');


    $("#spnFilterDiv").click(function () {

        $("#spnFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divConnectivityStatusFilter").toggle("slow");

    });

    $('#txtt14').blur(function () {
        $('#txtt1Tot').val(parseInt($('#txtt11').val()) + parseInt($('#txtt12').val()) + parseInt($('#txtt13').val()) + parseInt($('#txtt14').val()));
    });

    $('#txtt24').blur(function () {
        $('#txtt2Tot').val(parseInt($('#txtt21').val()) + parseInt($('#txtt22').val()) + parseInt($('#txtt23').val()) + parseInt($('#txtt24').val()));
    });

    $('#txtt21').blur(function () {
        if(parseInt($('#txtt11').val()) >= parseInt($('#txtt21').val()))
        {
            $('#txtt31').val(parseInt($('#txtt11').val()) - parseInt($('#txtt21').val()));

        }
    });
    $('#txtt22').blur(function () {
        if (parseInt($('#txtt12').val()) >= parseInt($('#txtt22').val())) {
            $('#txtt32').val(parseInt($('#txtt12').val()) - parseInt($('#txtt22').val()));
        }
    });
    $('#txtt23').blur(function () {
        if (parseInt($('#txtt13').val()) >= parseInt($('#txtt23').val())) {
            $('#txtt33').val(parseInt($('#txtt13').val()) - parseInt($('#txtt23').val()));
        }
    });

    $('#txtt2El499').blur(function () {
        if (parseInt($('#txtt1El499').val()) >= parseInt($('#txtt2El499').val())) {
            $('#txtt3El499').val(parseInt($('#txtt1El499').val()) - parseInt($('#txtt2El499').val()));
        }
    });

    $('#txtt2El249').blur(function () {
        if (parseInt($('#txtt1El249').val()) >= parseInt($('#txtt2El249').val())) {
            $('#txtt3El249').val(parseInt($('#txtt1El249').val()) - parseInt($('#txtt2El249').val()));
        }
    });

    $('#txtt24').blur(function () {
        if (parseInt($('#txtt14').val()) >= parseInt($('#txtt24').val())) {
            $('#txtt34').val(parseInt($('#txtt14').val()) - parseInt($('#txtt24').val()));
            $('#txtt3Tot').val(parseInt($('#txtt31').val()) + parseInt($('#txtt32').val()) + parseInt($('#txtt33').val()) + parseInt($('#txtt34').val()));
        }
    });

    $('#txtt44').blur(function () {
        $('#txtt4Tot').val(parseInt($('#txtt41').val()) + parseInt($('#txtt42').val()) + parseInt($('#txtt43').val()) + parseInt($('#txtt44').val()));
    });
    $('#txtt54').blur(function () {
        $('#txtt5Tot').val(parseInt($('#txtt51').val()) + parseInt($('#txtt52').val()) + parseInt($('#txtt53').val()) + parseInt($('#txtt54').val()));
    });

    $('#btnSubmit').click(function () {

        if ($("#frmConnectivityStatus").valid())
        {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

            $.ajax({
                url: '/Master/SaveConnectivityStatus/',
                type: 'POST',
                catche: false,
                data: $("#frmConnectivityStatus").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    
                    if (response.status == true) {
                        if ($('#dvLoadSaveButton').is(':visible')) {
                            alert('Records saved successfully');
                        }
                        else if ($('#dvLoadButtons').is(':visible')) {
                            alert('Records updated successfully');
                        }
                        $('#dvLoadButtons').show('slow');
                        $('#dvLoadSaveButton').hide('slow');
                        $('#btnReset').click();

                        $.ajax({
                            url: '/Master/Form3ConnectivityStatus/',
                            type: 'POST',
                            //catche: false,
                            data: $("#frmConnectivityStatusLayout").serialize(),
                            //async: false,
                            success: function (response) {
                                $.unblockUI();
                                //$('#dvLoadButtons').show('slow');


                                $("#dvLoadReport").html(response);
                                //alert($('#hdnflag').val());
                                if ($('#hdnflag').val() == "N") {
                                    $('#dvLoadButtons').show('slow');
                                    $('#dvLoadSaveButton').hide('slow');
                                }
                                else {
                                    $('#dvLoadSaveButton').show('slow');
                                    $('#dvLoadButtons').hide('slow');
                                }
                            },
                            error: function () {
                                $.unblockUI();
                                alert("Error ocurred");
                                return false;
                            },
                        });

                    }
                    else {
                        alert('Error occured');
                    }
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
            //if ($("#flag").val() == "N") {
            //    alert(1);
            //    $('#divConnectivityStatus1').show('slow');
            //    $('#divConnectivityStatus2').hide('slow');
            //}
            //else {
            //    $('#divConnectivityStatus1').hide('slow');
            //    $('#divConnectivityStatus2').show('slow');
            //}
            $.unblockUI();
        }
    });

    $('#btnCancel').click(function () {
        $.ajax({
            url: '/Master/Form3ConnectivityStatus/',
            type: 'POST',
            //catche: false,
            data: $("#frmConnectivityStatusLayout").serialize(),
            //async: false,
            success: function (response) {
                $.unblockUI();
                //$('#dvLoadButtons').show('slow');


                $("#dvLoadReport").html(response);
                //alert($('#hdnflag').val());
                if ($('#hdnflag').val() == "N") {
                    $('#dvLoadButtons').show('slow');
                    $('#dvLoadSaveButton').hide('slow');
                }
                else {
                    $('#dvLoadSaveButton').show('slow');
                    $('#dvLoadButtons').hide('slow');
                }
            },
            error: function () {
                $.unblockUI();
                alert("Error ocurred");
                return false;
            },
        });
    });
});