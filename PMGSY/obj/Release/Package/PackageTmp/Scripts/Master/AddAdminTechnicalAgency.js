$(document).ready(function () {
     $.validator.unobtrusive.parse("#frm");
     $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
     });

     $("#btnReset").click(function () {
         $("#dvErrorMessage").hide('slow');

     });
     $('#btnViewAgency').show('slow');
     //if ($("#roleCode").val() == 36 || $("#roleCode").val() == 25) {
     //    $('#btnViewAgency').show('slow');
     //}

    //$("#btnReset").click(function () {
    //     clearDetails();
    //     $("#ErrorMessage").hide();
    //     $("#dvErrorMessage").hide('slow');
    //    var validator = $("#frm").validate();
    //    validator.resetForm();
    // });

     $("#ADMIN_TA_NAME").focus(function () {

        $("#dvErrorMessage").hide(1000);
        $("#message").html('');
    });

     $("#chkAdd").change(function (e) {

        if (($("#chkAdd").is(':checked'))) {
            var text = $("#ADMIN_TA_ADDRESS1").val();
            $("#ADMIN_TA_ADDRESS2").val(text);
        }
        else {
            $("#ADMIN_TA_ADDRESS2").val("");
        }
     });

    $("#btnCancel").click(function () {
        // $("#dvAgencyDetails").load('/Master/AddAdminTechnicalAgency/');
        if ($("#dvAgencyDetails").is(":visible")) {
            $('#dvAgencyDetails').hide('slow');
            $('#btnSearchView').hide('slow');
            $('#btnAddAgency').show('slow');
        }
        if (!$("#dvSearchAgency").is(":visible")) {
            $("#dvSearchAgency").show('slow');
        }
        agencyDetails(agencyType);
    });

    $("#chkAdd").change(function () {
        if ($("#chkAdd").is(":checked")) {
            var address1 = $("#ADMIN_TA_ADDRESS1").val();
            if (address1 == "") {
                alert("Please enter address1");
                $("#chkAdd").attr("checked", false);
                return false;
            }
            else {
                $("#ADMIN_TA_ADDRESS2").val(address1);
                alert("Your Address1 and Address2 is same.");
            }
        }
        else {
            $("#ADMIN_TA_ADDRESS2").val();
        }
    });

    $("#dvhdCreateNewDetails").click(function () {

        if ($("#dvCreateNewDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $(this).next("#dvCreateNewDetails").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvCreateNewDetails").slideToggle(300);
        }
    });

    $("#btnSave").click(function () {
     $("#ErrorMessage").show();
        if ($("#frm").valid()) {
            var agencyType = $("input[name=ADMIN_TA_TYPE]:checked").val();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/AddAdminTechnicalAgency",
                type: "POST",
                data: $("#frm").serialize(),
                success: function (data) {
                if (data.success==true) {
                        alert(data.message);
                    //$("#dvAgencyDetails").load('/Master/AddAdminTechnicalAgency');
                        if ($("#dvAgencyDetails").is(":visible")) {
                            $('#dvAgencyDetails').hide('slow');

                            $('#btnSearchView').hide('slow');
                            $('#btnAddAgency').show('slow');

                        }

                        if (!$("#dvSearchAgency").is(":visible")) {
                            $("#dvSearchAgency").show('slow');
                        }
                        agencyDetails(agencyType);
                   }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvAgencyDetails").html(data);
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

     $("#btnUpdate").click(function () {
        $("#MAST_STATE_CODE").attr("disabled", false);
        $("#MAST_DISTRICT_CODE").attr("disabled", false);
        $("#ADMIN_TA_CONTACT_DESG").attr("disabled", false);
        var agencyType = $("input[name=ADMIN_TA_TYPE]:checked").val();
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/Master/EditAdminTechnicalAgency",
            type: "POST",
            data: $("#frm").serialize(),
            success: function (data) {
            if (data.success==true) {
                    alert(data.message);
                    //$('#tblList').trigger('reloadGrid');
                //$("#dvAgencyDetails").load("/Master/AddAdminTechnicalAgency");
                    if ($("#dvAgencyDetails").is(":visible")) {
                        $('#dvAgencyDetails').hide('slow');
                        $('#btnSearchView').hide('slow');
                        $('#btnAddAgency').show('slow');
                    }
                    if (!$("#dvSearchAgency").is(":visible")) {
                        $("#dvSearchAgency").show('slow');
                    }
                    agencyDetails(agencyType);
                }
                else if (data.success == false) {
                    $("#MAST_STATE_CODE").attr("disabled", true);
                    $("#MAST_DISTRICT_CODE").attr("disabled", true);
                    $("#ADMIN_TA_CONTACT_DESG").attr("disabled", true);
                    if (data.message != "") {
                        $('#message').html(data.message);
                        $('#dvErrorMessage').show('slow');
                        }
                }
                else {
                    $("#dvAgencyDetails").html(data);
                    $("#MAST_STATE_CODE").attr("disabled", true);
                    $("#MAST_DISTRICT_CODE").attr("disabled", true);
                    $("#ADMIN_TA_CONTACT_DESG").attr("disabled", true);
                }

                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }
        });
     });

    $("#MAST_STATE_CODE").change(function () {

        $.blockUI({ message: '<h4><label style="font-weight:normal"> Loading Districts...</label></h4>' });
        var val = $("#MAST_STATE_CODE").val();
        $.ajax({
            type: 'POST',
            url: "/Master/getDistrictsNameTA/",
            data: { id: val },
            async: false,
            success: function (data) {
                $("#MAST_DISTRICT_CODE").empty();
                $.each(data, function () {
                    $("#MAST_DISTRICT_CODE").append("<option value=" + this.Value + ">" +
                                                            this.Text + "</option>");
                });
            }
        });

        $.unblockUI();
    });
});


//function clearDetails()
//{
//    $("#ADMIN_TA_NAME").val("");
//    $("#ADMIN_TA_CONTACT_DESG").val("");
//    $("#ADMIN_TA_ADDRESS1").val("");
//    $("#ADMIN_TA_ADDRESS2").val("");
//    $("#MAST_STATE_CODE").val("");
//    $("#ADMIN_TA_PIN").val("");
//    $("#ADMIN_TA_WEBISTE").val("");
//    $("#ADMIN_TA_STD1").val("");
//    $("#ADMIN_TA_PHONE1").val("");
//    $("#ADMIN_TA_STD2").val("");
//    $("#ADMIN_TA_PHONE2").val("");
//    $("#ADMIN_TA_STD_FAX").val("");
//    $("#ADMIN_TA_FAX").val("");
//    $("#ADMIN_TA_MOBILE_NO").val("");
//    $("#ADMIN_TA_EMAIL").val("");
//    $("#ADMIN_TA_REMARKS").val("");
//    $("#ADMIN_TA_CONTACT_NAME").val("");
//    $("#MAST_DISTRICT_CODE").val('');
   
//    $('input:checkbox[name=Address Check]').attr('checked', false);
//}

function agencyDetails(agencyType) {
    $('#AgencyType').val(agencyType);
     $('#tblList').setGridParam({
        url: '/Master/GetAdminTechnicalAgencyDetails', datatype: 'json'
    });
     $('#tblList').jqGrid("setGridParam", { "postData": { AgencyType: $('#AgencyType option:selected').val(), AgencyName: $('#AgencyName').val() } });

    $('#tblList').trigger("reloadGrid", [{ page: 1 }]);
}