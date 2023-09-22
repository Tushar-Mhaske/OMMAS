var files;
$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmAddMaintenancePolicy');

    $("#ddlState").change(function () {
        loadAgencyList($("#ddlState").find(":selected").val());
        ClearMessage();
    });

    $("#dvhdAddNewPIUDetails").click(function () {

        if ($("#dvAddNewMaintenancePolicyDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvAddNewMaintenancePolicyDetails").slideToggle(300);
        }
        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvAddNewMaintenancePolicyDetails").slideToggle(300);
        }
    });


    $('#IMS_POLICY_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        maxDate: new Date(),
        onSelect: function (selectedDate) {
            $(this).focus().blur();
        }
    });


    $("#frmAddMaintenancePolicy").on('submit', function (event) {

        event.stopPropagation(); 
        event.preventDefault();

        if ($('#frmAddMaintenancePolicy').valid()) {


            var form_data = new FormData();

            var objFiles = $("input#file").prop("files");

            form_data.append("file", objFiles[0]);

            $("#fileDetails").val(form_data);

            var data = $("#frmAddMaintenancePolicy").serializeArray();

            for (var i = 0; i < data.length; i++) {
                form_data.append(data[i].name, data[i].value);
            }
            var stateCode = $("#ddlState option:selected").val();

            var agency = $("#ddlAgency option:selected").val();
            var fileType = $("#ddlImsFileType option:selected").val();
            $("#ddlState").attr("disabled", false);

            $.ajax({
                type: 'POST',
                url: '/Master/AddEditMaintenancePolicy/',
                cache: false,
                data: form_data,
                contentType: false,
                processData: false,
                success: function (data) {
                    
                    if (data.success == true) {
                        alert(data.message);
                        ClearDetails();
                        if ($("#ImsMaintenancePolicyAddDetails").is(":visible")) {
                            $('#ImsMaintenancePolicyAddDetails').hide('slow');
                            $('#btnSearch').hide();
                            $('#btnAdd').show();
                        }

                        if (!$("#ImsMaintenancePolicySearchDetails").is(":visible")) {
                            $("#ImsMaintenancePolicySearchDetails").show('slow');
                        }
                        SearchMaintenancePolicyDetails(stateCode, agency);

                    }
                    else if (data.success == false) {

                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');

                        }
                        if ($('#hdStatCode').val() > 0) {
                            $("#ddlState").val($('#hdStatCode').val());
                            $("#ddlState").attr("disabled", true);
                        }
                    }
                    else {
                        $("#ImsMaintenancePolicyAddDetails").html(data);
                    }

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                    if ($('#hdStatCode').val() > 0) {
                        $("#ddlState").val($('#hdStatCode').val());
                        $("#ddlState").attr("disabled", true);
                    }
                }
            })
        }
    });


    $("#btnCancel").click(function (e) {

        if ($("#ImsMaintenancePolicyAddDetails").is(":visible")) {
            $('#ImsMaintenancePolicyAddDetails').hide('slow');
            $('#btnSearch').hide();
            $('#btnAdd').show();
        }

        if (!$("#ImsMaintenancePolicySearchDetails").is(":visible")) {
            $("#ImsMaintenancePolicySearchDetails").show('slow');
        }

    })

    $("#btnReset").click(function (e) {
        
        $("input,select").removeClass("input-validation-error");
        $('.field-validation-error').html('');

        e.preventDefault();
        ClearDetails();
    });

    $("#btnUpdate").click(function (e) {


        if ($("#frmAddImsEcFileUpload").valid()) {

            $("#ddlState").attr("disabled", false);
            $("#ddlAgency").attr("disabled", false);
            $("#ddlImsFileType").attr("disabled", false);

            var stateCode = $("#ddlState option:selected").val();
            var agency = $("#ddlAgency option:selected").val();
            var fileType = $("#ddlImsFileType option:selected").val();

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/EditMaintenancePolicy/',
                async: false,
                data: $("#frmAddImsEcFileUpload").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);

                        ClearDetails();
                        if ($("#ImsMaintenancePolicyAddDetails").is(":visible")) {
                            $('#ImsMaintenancePolicyAddDetails').hide('slow');
                            $('#btnSearch').hide();
                            $('#btnAdd').show();
                        }

                        if (!$("#ImsMaintenancePolicySearchDetails").is(":visible")) {
                            $("#ImsMaintenancePolicySearchDetails").show('slow');
                        }
                        SearchMaintenancePolicyDetails(stateCode, agency);
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                            $("#ddlState").attr("disabled", true);
                            $("#ddlPhaseYear").attr("disabled", true);
                            $("#ddlBatch").attr("disabled", true);
                            $("#ddlAgency").attr("disabled", true);
                            $("#ddlImsFileType").attr("disabled", true);


                        }
                    }
                    else {
                        $("#ImsMaintenancePolicyAddDetails").html(data);
                        $("#ddlState").attr("disabled", true);
                        $("#ddlPhaseYear").attr("disabled", true);
                        $("#ddlBatch").attr("disabled", true);
                        $("#ddlAgency").attr("disabled", true);
                        $("#ddlImsFileType").attr("disabled", true);


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


    $("#frmAddImsEcFileUpload").bind('submitdone', function () {

    });

    $('input[type=file]').on('change', prepareUpload);
});
// Grab the files and set them to our variable
function prepareUpload(event) {
    files = event.target.files;
}


function SearchMaintenancePolicyDetails(stateCode, agency) {

    $('#ddlStateSerach').val(stateCode);
    $('#ddlStateSerach').trigger('change');

    setTimeout(function () {

        $('#ddlAgencySerach').val(agency);

        LoadMaintenancePolicyGrid();
        
        
        //$('#tblMaintenancePolicyUpload').setGridParam({
        //    url: '/Master/GetMaintenancePolicyFileUploadList'
        //});
        //$('#tblMaintenancePolicyUpload').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlStateSerach').val(), agency: $('#ddlAgencySerach').val() } });

        //$('#tblMaintenancePolicyUpload').trigger("reloadGrid", [{ page: 1 }]);
    }, 1000);
}

function ClearMessage() {

    if ($("#dvErrorMessage").is(":visible")) {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    }
}
function ClearDetails() {
    $('#ddlState').val('0');
    if ($('#hdStatCode').val() > 0) {
        $("#ddlState").val($('#hdStatCode').val());
        $("#ddlState").attr("disabled", true);
    }
    
    $('#ddlAgency').val('0');
    $('#ddlImsFileType').val('%');
    $('#file').val('');
    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}

function loadAgencyList(statCode) {
    $("#ddlAgency").val(0);
    $("#ddlAgency").empty();
    if (statCode > 0) {
        if ($("#ddlAgency").length > 0) {
            $.ajax({
                url: '/Master/GetAgencyListByState',
                type: 'POST',
                data: { "stateCode": statCode, "IsAllSelected": false },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlAgency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
    else {

        $("#ddlAgency").append("<option value='0'>--Select--</option>");

    }
}

function FileUploadSuccess(data) {
    alert(data.success);
    if (data.success == false) {
        $("#dvErrorMessage").show('fade');
        $("#message").html(data.message);
        return false;
    }
}
function Failure() {
    alert('fail');
}





