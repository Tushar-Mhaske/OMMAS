
jQuery.validator.addMethod("datecomparefieldvalidator", function (value, element, param) {

    var fromDate = $('#MAST_VS_START_DATE').val();
    var toDate = $('#MAST_VS_END_DATE').val();

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

    $.validator.unobtrusive.parse("#frmVidhanSabha");
    
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });


    $('#MAST_VS_START_DATE').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a start date',
        buttonImageOnly: true,
        buttonText: 'Start Date',
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
            $("#MAST_VS_END_DATE").datepicker("option", "minDate", selectedDate);
            $(function () {
                $('#MAST_VS_START_DATE').focus();
                $('#MAST_VS_END_DATE').focus();
            })
        }
    });

    $('#MAST_VS_END_DATE').datepicker({
        dateFormat:'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a end date',
        buttonImageOnly: true,
        buttonText: 'End Date',
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
            $("#MAST_VS_START_DATE").datepicker("option", "maxDate", selectedDate);
        }

    });

    if ($('#stateCode').val() > 0 && $('#btnSave').is(':visible')) {
        $("#ddlStateNames").val($('#stateCode').val());
        $("#ddlStateNames").attr("disabled", true);
        $("#ddlStateNames").trigger('change');
    }


    $('#btnSave').click(function (e) {
        if ($('#frmVidhanSabha').valid()) {
            
            $("#ddlStateNames").attr("disabled", false);
            var stateCode = $('#ddlStateNames option:selected').val();
            
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/AddMasterMasterVidhanSabhaTerm/",
                type: "POST",
             
                data: $("#frmVidhanSabha").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);                      
                     
                        //$('#btnReset').trigger('click');


                      
                        //if ($('#stateCode').val() > 0) {
                        //    $("#ddlStateNames").attr("disabled", true);
                        //}

                        //ClearDetails();
                        if ($("#dvVidhanSabhaDetails").is(":visible")) {
                            $('#dvVidhanSabhaDetails').hide('slow');

                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();
                        }

                        if (!$("#dvSearchVidhanSabha").is(":visible")) {
                            $("#dvSearchVidhanSabha").show('slow');
                        }
                        SearchDetails(stateCode);

                        $.unblockUI();
                

                    }
                    else if (data.success==false) {
                        if (data.message != "") {

                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                            if ($('#stateCode').val() > 0) {
                                $("#ddlStateNames").attr("disabled", true);
                            }
                            $.unblockUI();
                        }
                    }
                    else {
                        $("#dvVidhanSabhaDetails").html(data);
                        if ($('#stateCode').val() > 0) {
                            $("#ddlStateNames").attr("disabled", true);
                        }
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {                    
                    alert(xhr.responseText);
                    if ($('#stateCode').val() > 0) {
                        $("#ddlStateNames").attr("disabled", true);
                    }
                    $.unblockUI();
                }

            });

        }
    });




    $('#btnUpdate').click(function (e) {

        if ($('#frmVidhanSabha').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $('#ddlStateNames').attr('disabled', false);
            var stateCode = $('#ddlStateNames option:selected').val();

            $.ajax({
                url: "/Master/EditMasterVidhanSabhaTerm/",
                type: "POST",
            
                data: $("#frmVidhanSabha").serialize(),
                success: function (data) {
                 

                    if (data.success==true) {
                        alert(data.message);
                        //$('#tblVidhanSabhaList').trigger('reloadGrid');
                        //$("#dvVidhanSabhaDetails").load("/Master/AddEditMasterVidhanSabhaTerm");

                        if ($("#dvVidhanSabhaDetails").is(":visible")) {
                            $('#dvVidhanSabhaDetails').hide('slow');
                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();
                        }

                        if (!$("#dvSearchVidhanSabha").is(":visible")) {
                            $("#dvSearchVidhanSabha").show('slow');
                        }
                        SearchDetails(stateCode);
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#ddlStateNames').attr('disabled', true);
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvVidhanSabhaDetails").html(data);
                        $('#ddlStateNames').attr('disabled', true);
                    }

                    $.unblockUI();

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });
        }
    });

    $('#btnCancel').click(function (e) {

        //$.ajax({
        //    url: "/Master/AddEditMasterVidhanSabhaTerm",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        $("#dvVidhanSabhaDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }
        //});
        if ($("#dvVidhanSabhaDetails").is(":visible")) {
            $('#dvVidhanSabhaDetails').hide('slow');
            $('#btnSearchView').hide();
            $('#btnCreateNew').show();
        }

        if (!$("#dvSearchVidhanSabha").is(":visible")) {
            $("#dvSearchVidhanSabha").show('slow');
        }
    });

   
    $('#btnReset').click(function (e) {

        //Added By Abhishek kamble 25-Feb-2014 start
        $("input,select").removeClass("input-validation-error");
        $('.field-validation-error').html('');

        //$("#MAST_VS_START_DATE").removeClass("pmgsy-textbox hasDatepicker input-validation-error");
        //$("span MAST_VS_START_DATE").html('');
        e.preventDefault();

        ClearDetails();

    });
    



    //for expand and collpase Document Details 
    $("#dvhdCreateNewVidhanSabhaDetails").click(function () {

        if ($("#dvCreateNewVidhanSabhaDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");


            $(this).next("#dvCreateNewVidhanSabhaDetails").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvCreateNewVidhanSabhaDetails").slideToggle(300);
        }
    });

    $("#ddlStateNames").change(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');

        if ($('#ddlStateNames option:selected').val() > 0) {
            GetVidhanSabhaTerm();
        }
        else {
            $('#trVidhanSabhaTerm').hide('slow');
            $('#trBlankVidhanSabhaTerm').hide('slow');
        }

    });

    $("#MAST_VS_START_DATE").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#MAST_VS_END_DATE").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

});



function SearchDetails(stateCode) {
    $('#ddlSearchStates').val(stateCode);
    $('#tblVidhanSabhaList').setGridParam({
        url: '/Master/GetMasterVidhanSabhaTermList/'
    });

    $('#tblVidhanSabhaList').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStates option:selected').val() } });

    $('#tblVidhanSabhaList').trigger("reloadGrid", [{ page: 1 }]);

}

function GetVidhanSabhaTerm() {
    message = '<h4><label style="font-weight:normal"> Getting Vidhan Sabha Term... </label></h4>';


    $.blockUI({ message: message });
    $.ajax({
        url: "/Master/GetVidhanSabhaTerm/" + $('#ddlStateNames option:selected').val(),
        type: "POST",
        dataType: "json",
    
        success: function (data) { 
            if (data.status) {
                $('#lblVidhanSabhaTerm').text(data.vidhanSabhaTerm.toString());
                $('#trVidhanSabhaTerm').show('slow');
                $('#trBlankVidhanSabhaTerm').show('slow');
                
            }
           
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }
    });
}



function ClearDetails() {

    var dates = $("input[id$='MAST_VS_START_DATE'],input[id$='MAST_VS_END_DATE']");

    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
    $('#trVidhanSabhaTerm').hide('slow');
    $('#trBlankVidhanSabhaTerm').hide('slow');
    dates.attr('value', '');
    dates.each(function () {
        $.datepicker._clearDate(this);
    });

    if (!$('#ddlStateNames').is(':disabled')) {
        $('#ddlStateNames').val('0');
    }

    $('#MAST_VS_START_DATE').val('');
    $('#MAST_VS_END_DATE').val('');

}