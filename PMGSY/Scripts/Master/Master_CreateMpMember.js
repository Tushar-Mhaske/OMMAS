
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

    $.validator.unobtrusive.parse("#frmMasterMpMember");


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#MAST_MEMBER_START_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a start date',
        buttonImageOnly: true,
        buttonText: 'Start Date',
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
            $("#MAST_MEMBER_END_DATE").datepicker("option", "minDate", selectedDate);
            $(function () {
                $('#MAST_MEMBER_START_DATE').focus();
                $('#MAST_MEMBER_END_DATE').focus();
            })
        }
    });

    $('#MAST_MEMBER_END_DATE').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a end date',
        buttonImageOnly: true,
        buttonText: 'End Date',
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
            $("#MAST_MEMBER_START_DATE").datepicker("option", "maxDate", selectedDate);
        }

    });


    $('#btnSave').click(function (e) {

        if ($('#frmMasterMpMember').valid()) {
            var termCode = $('#MAST_LS_TERM option:selected').val();
            var constCode = $('#MAST_MP_CONST_CODE option:selected').val();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/AddMasterMpMember/",
                type: "POST",

                data: $("#frmMasterMpMember").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        ClearMpMemberDetails();
                        if ($("#dvMpMemberDetails").is(":visible")) {
                            $('#dvMpMemberDetails').hide('slow');

                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();
                        }

                        if (!$("#dvSearchMpMembers").is(":visible")) {
                            $("#dvSearchMpMembers").show('slow');
                        }
                        SearchMPCreateDetail(termCode, constCode);
                        $.unblockUI();
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvMpMemberDetails").html(data);
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



    $('#btnUpdate').click(function (e) {

        if ($('#frmMasterMpMember').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            var termCode = $('#MAST_LS_TERM option:selected').val();
            var constCode = $('#MAST_MP_CONST_CODE option:selected').val();
            $('#MAST_LS_TERM').attr('disabled', false);
            $('#MAST_MP_CONST_CODE').attr('disabled', false);

            $.ajax({
                url: "/Master/EditMasterMpMember/",
                type: "POST",

                data: $("#frmMasterMpMember").serialize(),
                success: function (data) {



                    if (data.success == true) {
                        alert(data.message);
                        //$('#tblMpMemberList').trigger('reloadGrid');
                        //$("#dvMpMemberDetails").load("/Master/AddEditMasterMpMember");
                        if ($("#dvMpMemberDetails").is(":visible")) {
                            $('#dvMpMemberDetails').hide('slow');

                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();
                        }

                        if (!$("#dvSearchMpMembers").is(":visible")) {
                            $("#dvSearchMpMembers").show('slow');
                        }
                        SearchMPCreateDetail(termCode, constCode);
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#MAST_LS_TERM').attr('disabled', true);
                            $('#MAST_MP_CONST_CODE').attr('disabled', true);
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvMpMemberDetails").html(data);
                        $('#MAST_LS_TERM').attr('disabled', true);
                        $('#MAST_MP_CONST_CODE').attr('disabled', true);
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
        //    url: "/Master/AddEditMasterMpMember",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {                
        //        $("#dvMpMemberDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }
        //});
        if ($("#dvMpMemberDetails").is(":visible")) {
            $('#dvMpMemberDetails').hide('slow');

            $('#btnSearchView').hide();
            $('#btnCreateNew').show();
        }

        if (!$("#dvSearchMpMembers").is(":visible")) {
            $("#dvSearchMpMembers").show('slow');
        }

    });

    var dates = $("input[id$='MAST_MEMBER_START_DATE'],input[id$='MAST_MEMBER_END_DATE']");

    $('#btnReset').click(function () {
        dates.attr('value', '');
        dates.each(function () {
            $.datepicker._clearDate(this);
        });
        ClearMpMemberDetails();
    });


    $("#dvhdCreateNewMpMemberDetails").click(function () {

        if ($("#dvCreateNewMpMemberDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvCreateNewMpMemberDetails").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvCreateNewMpMemberDetails").slideToggle(300);
        }
    });

    $("#MAST_LS_TERM").change(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');

        if ($('#MAST_LS_TERM option:selected').val() > 0) {

            GetLokSabhaTermDates();
        }
        else {
            $("#MAST_MEMBER_START_DATE").val('');
            $("#MAST_MEMBER_END_DATE").val('');
        }
    });

    $("#MAST_MP_CONST_CODE").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#MAST_MEMBER").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    $("#MAST_MEMBER_PARTY").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    $("#MAST_MEMBER_START_DATE").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    $("#MAST_MEMBER_END_DATE").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
});

function ClearMpMemberDetails() {
    $('#dvErrorMessage').hide('slow');

    $("#MAST_LS_TERM").val('');
    $("#MAST_MP_CONST_CODE").val('');
    $("#MAST_MEMBER").val('');
    $("#MAST_MEMBER_PARTY").val('');
    $("#MAST_MEMBER_START_DATE").val('');
    $("#MAST_MEMBER_END_DATE").val('');

}

function SearchMPCreateDetail(termCode, constCode) {
    //if ($("#StateCode").val() > 0) {
    //    $("#ddlSearchState").val($("#StateCode").val());
    //    FillInCascadeDropdown({ userType: $("#ddlSearchState").find(":selected").val() },
    //      "#ddlSearchConstituency", "/Master/GetMPConstituencyList?stateCode=" + $('#ddlSearchState option:selected').val());
    //    $("#ddlSearchConstituency").append("<option value='0'>All Constituencies </option>");       
    //    setTimeout(function () {
    //        if ($("#ddlSearchConstituency").length > 0) {
    //            $("#ddlSearchConstituency").val(constCode);
    //        }
    //        }, 800);    
    //}
    //else {
    //    FillInCascadeDropdown({ userType: $("#ddlSearchState").find(":selected").val() },
    //        "#ddlSearchConstituency", "/Master/GetMPConstituencyList?stateCode=" + $('#ddlSearchState option:selected').val());
    //    $("#ddlSearchConstituency").append("<option value='0'>All Constituencies </option>");
    //    setTimeout(function () {
    //        if ($("#ddlSearchConstituency").length > 0) {
    //            $("#ddlSearchConstituency").val(constCode);
    //        }
    //    }, 800);
    //}
    GetStateByMPConstCode(constCode);
    setTimeout(function () {
        if ($("#ddlSearchConstituency").length > 0) {
            $("#ddlSearchConstituency").val(constCode);
        }
        $('#txtSearchMember').val("");
        $('#ddlSearchTerm').val(termCode);

        $('#tblMpMemberList').setGridParam({
            url: '/Master/GetMasterMpMemberList'
        });

        $('#tblMpMemberList').jqGrid("setGridParam", { "postData": { termCode: $('#ddlSearchTerm option:selected').val(), stateCode: $('#ddlSearchState option:selected').val(), constituencyCode: $('#ddlSearchConstituency option:selected').val(), memberName: $('#txtSearchMember').val() } });
        $('#tblMpMemberList').trigger("reloadGrid", [{ page: 1 }]);
    }, 1500);

}

function GetLokSabhaTermDates() {
    message = '<h4><label style="font-weight:normal"> Getting Lok Sabha Term Dates... </label></h4>';


    $.blockUI({ message: message });
    $.ajax({
        url: "/Master/GetLokSabhaTermDates/" + $('#MAST_LS_TERM option:selected').val(),
        type: "POST",
        dataType: "json",

        success: function (data) {
            if (data.status) {
                $('#MAST_MEMBER_START_DATE').val(data.lokSabhaStartDate.toString());
                $('#MAST_MEMBER_END_DATE').val(data.lokSabhaEndDate.toString());

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

function GetStateByMPConstCode(constCode) {
    $.ajax({
        url: "/Master/GetStateCodeByMPConstiuencyCode?MpConstCode=" + constCode,
        cache: false,
        type: "POST",
        async: false,
        success: function (data) {
            $('#ddlSearchState').val(data.statCode);
            FillInCascadeDropdown({ userType: $("#ddlSearchState").find(":selected").val() },
           "#ddlSearchConstituency", "/Master/GetMPConstituencyList?stateCode=" + $('#ddlSearchState option:selected').val());
            $("#ddlSearchConstituency").append("<option value='0'>All Constituencies </option>");
        },
        error: function (xhr, ajaxOptions, thrownError) {

            if (xhr.responseText == "session expired") {

                alert(xhr.responseText);
                window.location.href = "/Login/LogIn";
            }
        }
    })

}