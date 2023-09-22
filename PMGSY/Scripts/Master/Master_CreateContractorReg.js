
$(document).ready(function () {
  
    $.validator.unobtrusive.parse("#frmMasterContReg");
  
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#divPanSearch").dialog({
        autoOpen: false,
        height: '130',
        width: "370",
        modal: true,
        title: 'Contractor Search'
    });


    $('#MAST_CON_VALID_FROM').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a from date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
            $("#MAST_CON_VALID_TO").datepicker("option", "minDate", selectedDate);
        }
    });

    $('#MAST_CON_VALID_TO').datepicker({
        dateFormat:'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a to date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear:true,
        onSelect: function (selectedDate) {
            $("#MAST_CON_VALID_FROM").datepicker("option", "maxDate", selectedDate);
        }

    });

    if ($('#stateCode').val() > 0 && $('#btnSave').is(':visible')) {

        $("#ddlState").val($('#stateCode').val());
        $("#ddlState").attr("disabled", true);
        $("#ddlState").trigger('change');
    }

    $('#btnSave').click(function (e) {

        if ($('#frmMasterContReg').valid()) {

            if ($('input[type="radio"].radiofund:checked').val() == 'P' && $("#MAST_CON_REG_NO").val() == "") {
                alert("Registration Number is required");
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            var stateCode = $('#ddlState').val();
            $.ajax({
                url: "/Master/AddMasterContractorReg/",
                type: "POST",
                data: $("#frmMasterContReg").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);

                        $('#btnReset').trigger('click');
                        // $('#tblstContractorReg').trigger('reloadGrid');
                        if ($("#dvDetailsContractorReg").is(":visible")) {
                            $('#dvDetailsContractorReg').hide('slow');
                            $('#btnSearch').hide();
                            $('#btnCreateNew').show();
                        }

                        if (!$("#dvSearchContractorReg").is(":visible")) {
                            $('#dvSearchContractorReg').show('slow');
                        }
                        SearchCreateContractorRegisDetails(stateCode);
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }

                    }
                    else {
                        $("#ddlContractors").attr('disabled', true);
                        $("#dvDetailsContractorReg").html(data);
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

        $("#ddlContractors").attr('disabled', false);

        if ($('#frmMasterContReg').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            var stateCode = $('#ddlState').val();

            $("#ddlState").attr("disabled", false);           
            $.ajax({
                url: "/Master/EditMasterContractorReg/",
                type: "POST",
          
                data: $("#frmMasterContReg").serialize(),
                success: function (data) {

                    if (data.success==true) {
                        alert(data.message);
                        //$('#tblstContractorReg').trigger('reloadGrid');
                        //$("#dvDetailsContractorReg").load("/Master/AddEditMasterContractorReg");
                        if ($("#dvDetailsContractorReg").is(":visible")) {
                            $('#dvDetailsContractorReg').hide('slow');
                            $('#btnSearch').hide();
                            $('#btnCreateNew').show();
                        }

                        if (!$("#dvSearchContractorReg").is(":visible")) {
                            $('#dvSearchContractorReg').show('slow');
                        }
                        SearchCreateContractorRegisDetails(stateCode);
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $("#ddlState").attr("disabled", true);
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');

                        }
                        $("#ddlContractors").attr('disabled', true);
                    }
                    else {
                        $("#dvDetailsContractorReg").html(data);
                        $("#ddlState").attr("disabled", true);
                        $("#ddlContractors").attr('disabled', true);
                    }

                    $.unblockUI();

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $("#ddlState").attr("disabled", true);
                    alert(xhr.responseText);
                 
                    $.unblockUI();
                }

            });
        }
    });

    $('#btnCancel').click(function (e) {

        //$.ajax({
        //    url: "/Master/AddEditMasterContractorReg",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {                
        //        $("#dvDetailsContractorReg").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }
        //});
        if ($("#dvDetailsContractorReg").is(":visible")) {
            $('#dvDetailsContractorReg').hide('slow');
            $('#btnSearch').hide();
            $('#btnCreateNew').show();
        }

        if (!$("#dvSearchContractorReg").is(":visible")) {
            $('#dvSearchContractorReg').show('slow');
        }
    });

    $('#btnReset').click(function () {       
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    

   
    $("#dvhdCreateNewContRegDetails").click(function () {

        if ($("#dvCreateNewContRegDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvCreateNewContRegDetails").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvCreateNewContRegDetails").slideToggle(300);
        }
    });

    $("#MAST_CON_REG_NO").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#MAST_CON_CLASS").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#MAST_CON_VALID_FROM").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    $("#MAST_CON_VALID_TO").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
 
    $("#MAST_REG_OFFICE").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    

    $("#ddlState").change(function () {

        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#message').html('');
        }

        FillInCascadeDropdown({ userType: $("#ddlState").find(":selected").val() },
                    "#ddlClassTypes", "/Master/GetClassessByStateCode?stateCode=" + $('#ddlState option:selected').val());


        FillInCascadeDropdown({ userType: $("#ddlState").find(":selected").val() },
           "#ddlContractors", "/Master/GetContractorsByStateCode?stateCode=" + $('#ddlState option:selected').val());

    });

    
    $("#txtSearchPan").blur(function () {

        if ($("RoleCode").val() == 36) {
            FillInCascadeDropdown({ userType: $("#ddlState").find(":selected").val() },
           "#ddlContractors", "/Master/GetContractorsByPan?stateCode=" + $('#MAST_REG_STATE').val() + "$" + $("#txtSearchPan").val());
        }
        else {
            FillInCascadeDropdown({ userType: $("#ddlState").find(":selected").val() },
           "#ddlContractors", "/Master/GetContractorsByPan?stateCode=" + $('#ddlState option:selected').val() + "$" + $("#txtSearchPan").val());
        }
        

    });

    $("#searchContractor").click(function () {

        $("#divPanSearch").load('/Master/SearchContractorByPan');
        $("#divPanSearch").dialog('open');
    });

   


});



function FillInCascadeDropdown(map, dropdown, action) {

    var message = '';

    message = '<h4><label style="font-weight:normal"> Loading Class... </label></h4>';

    $(dropdown).empty();
    

    $.blockUI({ message: message });

    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
}

function SearchCreateContractorRegisDetails(stateCode) {

    $('#State').val(stateCode);
    if ($('#stateCode').val() > 0) {

        $("#State").val($('#stateCode').val());
    }
    //$('#State').trigger('change');
    
        $('#tblstContractorReg').setGridParam({
            url: '/Master/GetContractorRegList'
        });

        $('#tblstContractorReg').jqGrid("setGridParam", { "postData": { stateCode: $('#State option:selected').val(), status: $('#Status option:selected').val(), contractorName: $('#txtContractor').val(), conStatus: $('#ContractorStatus option:selected').val(), panNo: $("#txtPan").val(), classType: $("#ClassType option:selected").val(), regNo: $("#txtRegNo").val(), companyName: $("#txtCompanyName").val() } });

        $('#tblstContractorReg').trigger("reloadGrid", [{ page: 1 }]);
  


}