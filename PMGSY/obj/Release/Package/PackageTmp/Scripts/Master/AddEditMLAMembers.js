jQuery.validator.addMethod("datecomparefieldvalidator", function (value, element, param) {

    var fromDate = $('#MAST_MEMBER_START_DATE').val();
    var toDate = $('#MAST_MEMBER_END_DATE').val();

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
    $.validator.unobtrusive.parse('#frmAddMembers');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

   




    $('#MAST_VS_TERM').append("<option value=0>--Select--</option>");
    $('#MAST_MEMBER_START_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a Start Date',
        buttonImageOnly: true,
        buttonText: 'Start Date',
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
            $("#MAST_MEMBER_END_DATE").datepicker("option", "minDate", selectedDate);
           
        }
    });

    $('#MAST_MEMBER_END_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a End Date',
        buttonText: 'End Date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
            $("#MAST_MEMBER_START_DATE").datepicker("option", "maxDate", selectedDate);
        }
    });


    $("#MAST_STATE_CODE").change(function () {


        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#message').html('');
        }

        FillInCascadeDropdown({ userType: $("#MAST_STATE_CODE").find(":selected").val() },
                    "#MAST_MLA_CONST_CODE", "/Master/GetConstituencyList?stateCode=" + $('#MAST_STATE_CODE option:selected').val());

        FillInCascadeDropdown({ userType: $("#MAST_STATE_CODE").find(":selected").val() },
                    "#MAST_VS_TERM", "/Master/GetTermList?stateCode=" + $('#MAST_STATE_CODE option:selected').val());

        $('#MAST_VS_TERM').empty();
        $('#MAST_VS_TERM').append("<option value=0>--Select--</option>");

    }); 

    if ($('#stateCode').val() > 0 && $('#btnSave').is(':visible')) {

        $("#MAST_STATE_CODE").val($('#stateCode').val());
        $("#MAST_STATE_CODE").attr("disabled", true);
        $("#MAST_STATE_CODE").trigger('change');
    }

    $("#btnSave").click(function (e) {

        if ($("#frmAddMembers").valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $("#MAST_STATE_CODE").attr("disabled", false);
            var stateCode = $("#MAST_STATE_CODE option:selected").val();
            var constCode = $('#MAST_MLA_CONST_CODE').val();
            var termCode = $('#MAST_VS_TERM').val();
           
            $.ajax({
                type: 'POST',
                url: '/Master/AddEditMLAMembers/',
                async: false,
                data: $("#frmAddMembers").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                        //  ClearDetails();

                        //$("#memberAddDetails").load('/Master/AddEditMLAMembers/');
                        if ($("#memberAddDetails").is(":visible")) {
                            $('#memberAddDetails').hide('slow');

                            $('#btnSearch').hide();
                            $('#btnAdd').show();

                        }

                        if (!$("#memberSearchDetails").is(":visible")) {
                            $('#memberSearchDetails').show('slow');
                        }
                        SearchDetails(stateCode,constCode,termCode);

                        if ($('#stateCode').val() > 0) {
                            $("#MAST_STATE_CODE").attr("disabled", true);
                        }

                        $.unblockUI();
                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                            if ($('#stateCode').val() > 0) {
                                $("#MAST_STATE_CODE").attr("disabled", true);
                            }
                            $.unblockUI();
                        }
                     
                    }
                    else {
                        $("#memberAddDetails").html(data);

                        if ($('#stateCode').val() > 0) {
                            $("#MAST_STATE_CODE").attr("disabled", true);
                        }
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    if ($('#stateCode').val() > 0) {
                        $("#MAST_STATE_CODE").attr("disabled", true);
                    }
                    $.unblockUI();
                }
            })
        }
    });

    $("#MAST_MEMBER").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

  //  var dates = $("input[id$='MAST_MEMBER_START_DATE'],input[id$='MAST_MEMBER_END_DATE']");

    $("#btnReset").click(function (e) {
        //dates.attr('value', '');
        //dates.each(function () {
        //    $.datepicker._clearDate(this);
        //});

        //Added By Abhishek kamble 20-Feb-2014
        $("input,select").removeClass("input-validation-error");
        $('.field-validation-error').html('');

        e.preventDefault();
        ClearDetails();
       // alert('a');

        //$('#dvErrorMessage').hide('slow');
        //$('#message').html('');
    });

    $('#MAST_VS_TERM').change(function () {

        if ($('#MAST_VS_TERM option:selected').val() > 0) {

            GetVidhanSabhaTermDates();
        }
        else {
            $("#MAST_MEMBER_START_DATE").val('');
            $("#MAST_MEMBER_END_DATE").val('');
        }

    });

    $("#dvhdAddNewMLADetails").click(function () {

        if ($("#dvAddNewMLADetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvAddNewMLADetails").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvAddNewMLADetails").slideToggle(300);
        }
    });



    $("#btnCancel").click(function (e) {

        //$.ajax({
        //    url: "/Master/AddEditMLAMembers",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        $("#memberAddDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }
        //});
        if ($("#memberAddDetails").is(":visible")) {
            $('#memberAddDetails').hide('slow');

            $('#btnSearch').hide();
            $('#btnAdd').show();

        }

        if (!$("#memberSearchDetails").is(":visible")) {
            $('#memberSearchDetails').show('slow');
        }
        
    })

    $("#btnUpdate").click(function (e) {

        if ($("#frmAddMembers").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $('#MAST_MLA_CONST_CODE').attr("disabled",false);
            $('#MAST_STATE_CODE').attr("disabled", false);
            $('#MAST_VS_TERM').attr("disabled", false);
            var stateCode = $("#MAST_STATE_CODE option:selected").val();
            var constCode = $('#MAST_MLA_CONST_CODE').val();
            var termCode = $('#MAST_VS_TERM').val();

            $.ajax({
                type: 'POST',
                url: '/Master/EditMLAMembers/',
                async: false,
                data: $("#frmAddMembers").serialize(),
                success: function (data) {

                 
                    if (data.success==true) {
                        alert(data.message);
                        
                        //$('#memberCategory').trigger('reloadGrid');
                        //$("#memberAddDetails").load('/Master/AddEditMLAMembers/');
                        if ($("#memberAddDetails").is(":visible")) {
                            $('#memberAddDetails').hide('slow');

                            $('#btnSearch').hide();
                            $('#btnAdd').show();

                        }

                        if (!$("#memberSearchDetails").is(":visible")) {
                            $('#memberSearchDetails').show('slow');
                        }
                        SearchDetails(stateCode, constCode, termCode)
                    }
                    else if (data.success==false) {
                        $('#MAST_MLA_CONST_CODE').attr("disabled", true);
                        $('#MAST_STATE_CODE').attr("disabled", true);
                        $('#MAST_VS_TERM').attr("disabled", true);
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#memberAddDetails").html(data);
                        $('#MAST_MLA_CONST_CODE').attr("disabled", true);
                        $('#MAST_STATE_CODE').attr("disabled", true);
                        $('#MAST_VS_TERM').attr("disabled", true);
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
    $('#MAST_MEMBER').val('');
    $('#MAST_MEMBER_END_DATE').val('');
    $('#MAST_MEMBER_PARTY').val('');
    $('#MAST_MEMBER_START_DATE').val('');
    $('#MAST_MLA_CONST_CODE').val('');

    if (!$('#MAST_STATE_CODE').is(':disabled')) {
        $('#MAST_STATE_CODE').val('0');
    }
    $('#MAST_VS_TERM').val('');
    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}

function FillInCascadeDropdown(map, dropdown, action) {


    var message = '';

    if (dropdown == '#MAST_STATE_CODE') {
        message = '<h4><label style="font-weight:normal"> Loading States... </label></h4>';
    }
    else if (dropdown == '#MAST_MLA_CONST_CODE') {
        message = '<h4><label style="font-weight:normal"> Loading Constituencies... </label></h4>';
    }
    else if (dropdown == '#MAST_VS_TERM') {
        message = '<h4><label style="font-weight:normal"> Loading Terms... </label></h4>';
    }
    $(dropdown).empty();
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
} 


function SearchDetails(stateCode,constCode,termCode) {
    $("#State").val(stateCode);
    $("#State").trigger('change');
    setTimeout(function () {
        $('#Constituency').val(constCode);
        $('#Term').val(termCode);
    }, 1000);
    setTimeout(function () {
        $('#memberCategory').setGridParam({
            url: '/Master/GetMemberList'
        });
        $('#memberCategory').jqGrid("setGridParam", { "postData": { stateCode: $("#State").val(), constituency: $('#Constituency').val(), term: $('#Term option:selected').val(), memberName: "" } });

        $('#memberCategory').trigger("reloadGrid", [{ page: 1 }]);
    }, 1500);

}


function GetVidhanSabhaTermDates() {
    message = '<h4><label style="font-weight:normal"> Getting Vidhan Sabha Term Dates... </label></h4>';


    $.blockUI({ message: message });
    $.ajax({
        url: "/Master/GetVidhanSabhaTermDates"  ,
        type: "POST",
        dataType: "json",
        data: { stateCode: $('#MAST_STATE_CODE option:selected').val(), vidhanSabhaTerm: $('#MAST_VS_TERM option:selected').val() },
        success: function (data) {
            if (data.status) {
                $('#MAST_MEMBER_START_DATE').val(data.vidhanSabhaStartDate.toString());
                $('#MAST_MEMBER_END_DATE').val(data.vidhanSabhaEndDate.toString());

                $("#MAST_MEMBER_START_DATE").datepicker("option", "minDate", $("#MAST_MEMBER_START_DATE").datepicker("getDate"));
                $("#MAST_MEMBER_START_DATE").datepicker("option", "maxDate", $("#MAST_MEMBER_END_DATE").datepicker("getDate"));
                $("#MAST_MEMBER_END_DATE").datepicker("option", "maxDate", $("#MAST_MEMBER_END_DATE").datepicker("getDate"));
                $("#MAST_MEMBER_END_DATE").datepicker("option", "minDate", $("#MAST_MEMBER_START_DATE").datepicker("getDate"));

            }
            
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }
    });
}