jQuery.validator.addMethod("customvalidationrequired", function (value, element, param) {

    if ($("#rdoIsActiveNo").is(":checked") && $('#ADMIN_ACTIVE_ENDDATE').val() == '')
        return false;
    else
        return true; 
});

jQuery.validator.unobtrusive.adapters.addBool("customvalidationrequired");

$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmSQC");

    
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    if ($("#rdoIsActiveYes").is(":checked")) {

        IsActiveYes();
    }

    if ($("#rdoIsActiveNo").is(":checked")) {

        IsActiveNo();
    }

    //$('#departmentDD').change(function () {
    //    var DDvalue = $('#departmentDD option:selected').val()
    //    if (DDvalue==0)
    //    $('#errormsg').show();
    //});

    //This method for datepicker functionality
    $('#ADMIN_ACTIVE_ENDDATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a from date',
        buttonImageOnly: true,
        title: "Expiry Date",
        minDate: new Date(),
        buttonText: "Valid Upto",
        
    });

    //This method gets call when radio button status change
    $("#rdoIsActiveYes").change(function () {
        IsActiveYes();//function call

    });
    
    //This method gets call when radio button status change
    $("#rdoIsActiveNo").change(function () {
        IsActiveNo();//function call
    });

    //This method for clearing error messages
    $("#btnReset").click(function (e) {

        //Added By Abhishek kamble 20-Feb-2014
        $("input,select").removeClass("input-validation-error");
        $('.field-validation-error').html('');

        e.preventDefault();
        
        ClearDetails();
    });

    //This method is for cancel button click
    $("#btnCancel").click(function () {

        //$("#dvDetails").load('/Master/AddAdminSqc');
        if ($("#dvDetails").is(":visible")) {
            $('#dvDetails').hide('slow');
            $('#btnSearchView').hide();
            $('#btnCreateNew').show();
        }
        if (!$("#dvSearchSQC").is(":visible")) {
            $("#dvSearchSQC").show();
        }
    });

    //This method is for maximising/minimising form and list.
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

  

    if ($('#stateCode').val() > 0) {
        $("#sqcStateCode").val($('#stateCode').val());
        $("#sqcStateCode").attr("disabled", true);

        $(function () {
            //alert("test");
            $("#sqcStateCode").trigger('change');
        });
        
    }


   
    //This method is for save button click.
    $("#btnSave").click(function () {
        if ($('#departmentDD option:selected').val() == 0) {
            return false;
        }
        if ($("#frmSQC").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            var stateCode = $("#sqcStateCode").val();
            $("#sqcStateCode").attr("disabled", false);
            var stateCode = $("#sqcStateCode option:selected").val();

            $.ajax({
                url: "/Master/AddAdminSqc",
                type: "POST",
                data: $("#frmSQC").serialize(),
                success: function (data) {
                        if (data.success==true) {
                        alert(data.message);
                        $('#tblList').trigger('reloadGrid');
                        ////$("#btnReset").trigger('click');

                        //if ($('#stateCode').val() > 0) {
                        //    $("#sqcStateCode").attr("disabled", true);
                            //}
                            ClearDetails();
                            if ($("#dvDetails").is(":visible")) {
                                $('#dvDetails').hide('slow');
                                $('#btnSearchView').hide();
                                $('#btnCreateNew').show();
                            }
                            if (!$("#dvSearchSQC").is(":visible")) {
                                $("#dvSearchSQC").show();
                            }
                            SearchCreatSQCDetails(stateCode);
                      
                        
                        $.unblockUI();
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');

                            if ($('#stateCode').val() > 0) {
                                $("#sqcStateCode").attr("disabled", true);
                            }
                            $.unblockUI();
                        }
                    }
                    else {
                        $("#dvDetails").html(data);
                        if ($('#stateCode').val() > 0) {
                            $("#sqcStateCode").attr("disabled", true);
                        }
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

    //This method is for update button click
    $("#btnUpdate").click(function () {
        //$("#sqcStateCode").attr('disabled', false);

        if ($("#frmSQC").valid()) {
            //alert("Y");
            if ($("#rdoIsActiveNo").is(":checked") && $("#ADMIN_ACTIVE_ENDDATE").val() != '') {
                if (confirm("Are you sure you want to 'DeActivate' quality controller details?")) {
                    UpdateSQC();
                }
                else {
                    return false;
                }
            }
            else {
                UpdateSQC();
            }
        }
        else {
            $("#sqcStateCode").attr('disabled', true);
        }
    });

    //This method is to populate district on change of state dropdown
    $("#sqcStateCode").change(function () {

        $.blockUI({ message: '<h4><label style="font-weight:normal">loading districts and Departments...</label> ' });
        var val = $("#sqcStateCode").val();
        $.ajax({
            type: 'POST',
            url: "/Master/getDistrictsName/",
            data: { id: val },
            async: false,
            success: function (data) {
                $("#MAST_DISTRICT_CODE").empty();
                $.each(data, function () {
                    $("#MAST_DISTRICT_CODE").append("<option value=" + this.Value + ">" +
                                                            this.Text + "</option>");
                });
                $.unblockUI();
            }

        });

        $("#dvErrorMessage").hide('slow');
        $('#message').html('');

    });

    //This method will load department according to state
    
    $("#sqcStateCode").change(function () {
        var adminNdCode = $('#sqcStateCode option:selected').val();
        $.ajax({
            url: '/Master/GetDepartmentOfStates/' + adminNdCode,
            type: 'GET',
            catche: false,
            error: function (xhr, status, error) {
                alert('An Error occured while processig your request.')
                return false;
                console.log(adminNdCode);
            },
            success: function (data) {
                $('#departmentDD').empty();
                $.each(data, function () {
                    $('#departmentDD').append("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            }
        });

    });

});

function IsActiveNo() {

    if ($("#rdoIsActiveNo").is(":checked")) {

        $("#tdIsActive").attr('colspan', '1');
        $("#tdlblEndDate").show('slow');
        $("#tdtxtEndDate").show('slow');
     
    }
}

function IsActiveYes() {

    if ($("#rdoIsActiveYes").is(":checked")) {

        $("#tdlblEndDate").hide();
        $("#tdtxtEndDate").hide();
        $("#ADMIN_ACTIVE_ENDDATE").val('');
        $("#tdIsActive").attr('colspan', '3');
    }
}
//function for updating the record
function UpdateSQC() {
    var stateCode = $("#sqcStateCode").val();
    $("#sqcStateCode").prop("disabled", false);
    //alert(stateCode);
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/Master/EditAdminSqc",
        type: "POST",
        dataType: "json",
        data: $("#frmSQC").serialize(),
        success: function (data) {

            if (data.success==true) {
                //alert(data.message);

                //$('#tblList').trigger('reloadGrid');
                //$("#dvDetails").load('/Master/AddAdminSqc');
                if ($("#dvDetails").is(":visible")) {
                    $('#dvDetails').hide('slow');
                    $('#btnSearchView').hide();
                    $('#btnCreateNew').show();
                }
                if (!$("#dvSearchSQC").is(":visible")) {
                    $("#dvSearchSQC").show();
                }
                SearchCreatSQCDetails(stateCode);
            }
            else if (data.success == false) {
                $("#sqcStateCode").attr('disabled', true)
                if (data.message != "") {
                    $('#message').html(data.message);
                    $('#dvErrorMessage').show('slow');

                }
            }
            else {
                $("#dvDetails").html(data);
                $("#sqcStateCode").attr('disabled', true)
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


    $("#dvErrorMessage").hide('slow');
    $('#message').html('');

    if (!$('#sqcStateCode').is(':disabled')) {
        $('#sqcStateCode').val('0');
    }

    $('#ADMIN_QC_NAME').val('');

    $('#ADMIN_QC_DESG').val('');
    $('#ADMIN_QC_ADDRESS1').val('');
    $('#MAST_DISTRICT_CODE').val('');
    $('#ADMIN_QC_ADDRESS2').val('');
    $('#ADMIN_QC_PIN').val('');
    $('#ADMIN_QC_MOBILE').val('');

    $('#ADMIN_QC_STD1').val('');
    $('#ADMIN_QC_PHONE1').val('');
    $('#ADMIN_QC_STD2').val('');
    $('#ADMIN_QC_PHONE2').val('');
    $('#ADMIN__QC_AADHAR_NO').val('');

    $('#ADMIN_QC_STD_FAX').val('');
    $('#ADMIN_QC_FAX').val('');
    $('#ADMIN_QC_EMAIL').val('');
    $('#ADMIN_QC_REMARKS').val('');
}

function SearchCreatSQCDetails(stateCode) {
 
    $('#ddlSearchStates').val(stateCode);
   // $('#ddlSearchStatus').val(status);

    $('#tblList').setGridParam({ url: '/Master/GetAdminSqcDetails', datatype: 'json' });

    $('#tblList').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStates option:selected').val(), status: $('#ddlSearchStatus option:selected').val(), adminNdCode: $('#departmentDD Option:selected').val() } });

    $('#tblList').trigger("reloadGrid", [{ page: 1 }]);

}