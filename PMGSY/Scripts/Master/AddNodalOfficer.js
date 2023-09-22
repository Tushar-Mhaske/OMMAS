jQuery.validator.addMethod("enddatevalidator", function (value, element, param) {

    var IsDeactivate = $('#rdoDeactive').val();
    var EndDate = $('#ADMIN_ACTIVE_END_DATE').val();

     if ($("#rdoDeactive").is(":checked")) {
        if (IsDeactivate == "N" && EndDate == "") {
            return false;
        }
        else {
            return true;
        }
    } else {
        return true;
    }

});

jQuery.validator.unobtrusive.adapters.addBool("enddatevalidator");


jQuery.validator.addMethod("datecomparefieldvalidator", function (value, element, param) {
 
 var fromDate = $('#ADMIN_ACTIVE_START_DATE').val();
 var toDate = $('#ADMIN_ACTIVE_END_DATE').val();

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

   
    $.validator.unobtrusive.parse("#frmNodalOfficer");
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
    
    //if ($("#EncryptedOfficerCode").val() == "") {
    //    $("#tdlablEndDate").hide();
    //    $("#tdtxtEndDate").hide();
    //    $("#tdNew").show();
    //    $("#tdAdd").show();
    //}


    $(function(){
        if ($("#rdoActive").is(":checked")) {
            $("#tdlablEndDate").hide();
            $("#tdtxtEndDate").hide();
        }
    });
    $("#rdoActive").click(function () {
        $("#tdlablEndDate").hide();
        $("#tdtxtEndDate").hide();
        $("#tdNew").show();
        $("#tdAdd").show();
       
    });

    $("#rdoDeactive").click(function () {
        $("#tdlablEndDate").show("slow");
        $("#tdtxtEndDate").show("slow");
        $("#tdNew").hide();
        $("#tdAdd").hide();
    });
 

    if ($('#districtCode').val() > 0 && $('#btnSave').is(':visible')) {

        $("#MAST_DISTRICT_CODE").val($('#districtCode').val());
        //$("#MAST_DISTRICT_CODE").attr("disabled", true);
       // $("#MAST_DISTRICT_CODE").trigger('change');

        $("#ADMIN_ND_CODE").val($("#adminCode").val());
        // $("#ADMIN_ND_CODE").attr("disabled", true);
        $(function () {
            $("#ADMIN_ND_CODE").trigger('change')
        });
    }

    
    $("#ADMIN_ND_CODE").change(function () {
       // if ($("#rdoDPIU").is(":checked")) {
            $.blockUI({ message: '<h4><label style="font-weight:normal">loading District...</label> ' });
            var val = $("#ADMIN_ND_CODE").val();
           // alert(val);
            $.ajax({
                type: 'POST',
                url: "/Master/PopulateDistrict?id=" + val,
                async: false,
                success: function (data) {
                    $.unblockUI();
                    $("#MAST_DISTRICT_CODE").empty();
                    $.each(data, function () {
                        $("#MAST_DISTRICT_CODE").append("<option value=" + this.Value + ">" +
                                                                this.Text + "</option>");

                    });

                    $.unblockUI();
                }

            });
       // }

    });

    //This method is start date field datepicker
    $('#ADMIN_ACTIVE_START_DATE').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a start date',
        buttonImageOnly: true,
        buttonText: "Start Date",
        changeMonth: true,
        changeYear:true,
        onSelect: function (selectedDate) {
            $("#ADMIN_ACTIVE_END_DATE").datepicker("option", "minDate", selectedDate);
            $("#ADMIN_ACTIVE_START_DATE").focus();
            $("#ADMIN_NO_REMARKS").focus()
        }
    });

    //This method is end date field datepicker
    $('#ADMIN_ACTIVE_END_DATE').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a end date',
        buttonImageOnly: true,
        buttonText: "End Date",
        maxDate:new Date(),
        onSelect: function (selectedDate) {
            $("#ADMIN_ACTIVE_START_DATE").datepicker("option", "maxDate", selectedDate);
            $("#ADMIN_ACTIVE_END_DATE").focus();
            $("#btnUpdate").focus()
        }

    });

    var dates = $("input[id$='ADMIN_ACTIVE_START_DATE'],input[id$='ADMIN_ACTIVE_END_DATE']");

    //This method is for reset button click.
    $("#btnReset").click(function (e) {
        //e.preventDefault();
        $('#ADMIN_NO_EMAIL').prop('disabled', false);
        dates.attr('value', '');
        dates.each(function () {
            $.datepicker._clearDate(this);
        });
        $("#dvErrorMessage").hide('slow');
    });

    //This method call when mail flag filed being click.
    $('input[name=ADMIN_NO_MAIL_FLAG]').click(function () {
        if ($(this).val() == "N") {
          
            $('#ADMIN_NO_EMAIL').prop('disabled', true);
        } else {
         
            $('#ADMIN_NO_EMAIL').prop('disabled', false);
        }
    });

    //This method is for cancel button click.
    $("#btnCancel").click(function () {
        //$("#dvDetailsNodalOfficer").load('/Master/AddNodalOfficer');
        $("#dvErrorMessage").hide('slow');
        if ($("#dvDetailsNodalOfficer").is(":visible")) {
            $('#dvDetailsNodalOfficer').hide('slow');
            $('#btnSearchView').hide();
            $('#btnCreateNew').show();
        }
        if (!$("#dvSearchNodalOfficer").is(":visible")) {
            $("#dvSearchNodalOfficer").show();
        }

    });

    //This method is maximising/minimising the form and list.
    $("#dvhdCreateNewDetails").click(function () {
        if ($("#dvCreateNewDetails").is(":visible")) {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $("#dvCreateNewDetails").slideToggle(300);
        }
        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
            $("#dvCreateNewDetails").slideToggle(300);
        }
    });

    //This method is for save button click.
    $("#btnSave").click(function () {
        $("#ErrorMessage").show();


        $("#ADMIN_ND_CODE").attr("disabled", false);
        $("#MAST_DISTRICT_CODE").attr("disabled", false);
      
        if ($("#frmNodalOfficer").valid()) {
            var officeCode = $('#ADMIN_ND_CODE option:selected').val();
            var designation = $('#ADMIN_NO_DESIGNATION').val();
            var profiletype = $('#ADMIN_NO_TYPE').val();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/AddNodalOfficer",
                type: "POST",
                data: $("#frmNodalOfficer").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        if ($("#dvDetailsNodalOfficer").is(":visible")) {
                            $('#dvDetailsNodalOfficer').hide('slow');
                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();
                        }
                        if (!$("#dvSearchNodalOfficer").is(":visible")) {
                            $("#dvSearchNodalOfficer").show();
                        }
                        searchCreateNodalOfficerDetails(officeCode, designation,profiletype);
                        //$("#btnReset").trigger('click');
                        //if ($('#districtCode').val() > 0) {
                        //    $("#MAST_DISTRICT_CODE").attr("disabled", true);
                        //    $("#ADMIN_ND_CODE").attr("disabled", true);
                        //}
                        
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                            if ($('#districtCode').val() > 0) {
                                $("#MAST_DISTRICT_CODE").attr("disabled", true);
                                $("#ADMIN_ND_CODE").attr("disabled", true);
                            }
                        }
                        
                    }
                    else {
                        if ($('#districtCode').val() > 0) {
                            $("#MAST_DISTRICT_CODE").attr("disabled", true);
                            $("#ADMIN_ND_CODE").attr("disabled", true);
                        }
                        $("#dvDetailsNodalOfficer").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert(xhr.responseText);
                    $("#ADMIN_ND_CODE").attr("disabled", true);
                    $.unblockUI();
                }
            });
        }
    });

    //This method is for update button click.
    $("#btnUpdate").click(function () {
        if ($("#frmNodalOfficer").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            var officeCode = $('#ADMIN_ND_CODE option:selected').val();
            var designation = $('#ADMIN_NO_DESIGNATION').val();
            var profiletype = $('#ADMIN_NO_TYPE').val();
            $('#ADMIN_ND_CODE').attr("disabled", false);
            $('#ADMIN_NO_DESIGNATION').attr("disabled", false);
            $.ajax({
                url: "/Master/EditNodalOfficer",
                type: "POST",
                data: $("#frmNodalOfficer").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        //$('#tblList').trigger('reloadGrid');
                        //$("#dvDetailsNodalOfficer").load('/Master/AddNodalOfficer');
                        if ($("#dvDetailsNodalOfficer").is(":visible")) {
                            $('#dvDetailsNodalOfficer').hide('slow');
                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();
                        }
                        if (!$("#dvSearchNodalOfficer").is(":visible")) {
                            $("#dvSearchNodalOfficer").show();
                        }
                        searchCreateNodalOfficerDetails(officeCode,designation,profiletype);

                        
                    }
                    else if (data.success == false) {

                        $('#ADMIN_ND_CODE').attr("disabled", true);
                        $('#ADMIN_NO_DESIGNATION').attr("disabled", true);
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                        
                    }
                    else {
                        $("#dvDetailsNodalOfficer").html(data);
                        $('#ADMIN_ND_CODE').attr("disabled", true);
                        $('#ADMIN_NO_DESIGNATION').attr("disabled", true);
                        
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert(xhr.responseText);
                    $.unblockUI();
                }
            });
        }
    });

    //This method is for Back button click.
    $("#btnCancelView").click(function () {
      
        //  $("#dvDetailsNodalOfficer").hide('slow');
        if ($("#dvDetailsNodalOfficer").is(":visible")) {
            $('#dvDetailsNodalOfficer').hide('slow');
            $('#btnSearchView').hide();
            $('#btnCreateNew').show();
        }
        if (!$("#dvSearchNodalOfficer").is(":visible")) {
            $("#dvSearchNodalOfficer").show();
        }

    });

});


function searchCreateNodalOfficerDetails(officeCode,designation,profileType) {   
    GetStateByAdminNDCode(officeCode);
    setTimeout(function () {
        $('#ddlSearchOffice').val(officeCode);
        $('#ddlSearchDesignation').val(designation);
        $('#ddlSearchNOType').val(profileType);
        $('#tblList').setGridParam({
            url: '/Master/GetNodalOfficerDetails', datatype: 'json'
        });

        $('#tblList').jqGrid("setGridParam", { "postData": { StateCode: $('#ddlSearchState option:selected').val(), officeCode: $('#ddlSearchOffice option:selected').val(), districtCode: $('#ddlSearchDistrict option:selected').val(), designationCode: $('#ddlSearchDesignation option:selected').val(), NoTypeCode: $('#ddlSearchNOType option:selected').val(), ModuleType: $('#ddlModuleType option:selected').val(), Active: $('#ddlSearchActive option:selected').val() } });

        $('#tblList').trigger("reloadGrid", [{ page: 1 }]);

    }, 1500);
}

function GetStateByAdminNDCode(officeCode)
{   
    $.ajax({
        url: "/Master/GetStateCodeByAdminNdCode?AdminNdCode=" + officeCode,
        cache: false,
        type: "POST",
        async: false,
        success: function (data) {
     
            $('#ddlSearchState').val(data.statCode);
            FillInCascadeDropdown({ userType: $("#ddlSearchState").val() },
                   "#ddlSearchOffice", "/Master/PopulateAdminNd_ByStateCode?stateCode=" + $('#ddlSearchState option:selected').val());

        },
        error: function (xhr, ajaxOptions, thrownError) {

            if (xhr.responseText == "session expired") {

                alert(xhr.responseText);
                window.location.href = "/Login/LogIn";
            }
        }
    })
 
}