$(document).ready(function () {

    $('#btnSubmit').click(function (evt) {
        evt.preventDefault();
        debugger;

        if ($('#frmRegisterGrievance').valid()) {
            if (checkFilevalidation("add")) {
                var formdata = new FormData(document.getElementById("frmRegisterGrievance"));
                $.ajax({
                    url: '/ContractorGrievances/ContractorGrievances/SaveContractorGrievance',
                    type: "POST",
                    cache: false,
                    data: formdata,    //$("#frmRegisterGrievance").serialize(),
                    //dataType: 'json',
                    contentType: false,
                    processData: false,
                    //contentType: 'application/json; charset=utf-8',
                    beforeSend: function () {
                        blockPage();
                    },
                    error: function (xhr, status, error) {
                        unblockPage();
                        Alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },
                    success: function (response) {
                        unblockPage();
                        if (response.Success) {
                            alert("Grievance ID : " + response.ReferenceNo + "\nYour Grievance has been submitted.An email has also been sent to your registered email ID.");
                            //ResetForm();
                            CloseRegisterGrievenceForm();
                            unblockPage();
                            // LoadAgreementList();
                        }
                        else {
                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html(response.ErrorMessage);
                            $('#mainDiv').animate({ scrollTop: 0 }, 'slow');
                            unblockPage();
                        }
                    }
                });
            }
        }
        else {
            return false;
        }
    });

    $('#btnReset').click(function () {
        $('#frmRegisterGrievance').get(0).reset();
        $('#dvErrorMessage').hide();
    });

    $("#ddlType").change(function () {
        $("#ddlSubType").empty();
        $.ajax({
            url: '/ContractorGrievances/ContractorGrievances/PopulateGrievanceSubTypeList',
            type: 'GET',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { typeCode: $("#ddlType").val(), },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlSubType").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlSubType").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }
                $.unblockUI();
            },
            error: function (err) {
                $.unblockUI();
            }
        });
    });

    $("#btnCancel").click(function () {
        $('#accordion').hide('slow');
        $("#tbTrackGrievanceList").jqGrid('setGridState', 'visible');
        $('#divRegisterGrievanceListForm').hide('slow');
        $("#tbRegisterGrievanceList").jqGrid('setGridState', 'hidden');
        $('#divFilterForm').show('slow');
    });

    $('#btnEdit').click(function (evt) {
        evt.preventDefault();
        debugger;

        if ($('#frmRegisterGrievance').valid()) {
            if (checkFilevalidation("edit")) {
                var formdata = new FormData(document.getElementById("frmRegisterGrievance"));
                $.ajax({
                    url: '/ContractorGrievances/ContractorGrievances/EditContractorGrievance',
                    type: "POST",
                    cache: false,
                    data: formdata,
                    contentType: false,
                    processData: false,
                    beforeSend: function () {
                        blockPage();
                    },
                    error: function (xhr, status, error) {
                        unblockPage();
                        Alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },
                    success: function (response) {
                        unblockPage();
                        if (response.Success) {
                            alert("Grievance ID : " + response.ReferenceNo + "\nYour Grievance has been updated.");
                            $("#btnCancel").trigger('click');
                            //CloseRegisterGrievenceForm();
                            unblockPage();
                        }
                        else {
                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html(response.ErrorMessage);
                            $('#mainDiv').animate({ scrollTop: 0 }, 'slow');
                            unblockPage();
                        }
                    }
                });
            }
        }
        else {
            return false;
        }
    });
});

function MultiStepGrivanceForm(agreementCode) {
    debugger;

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Register Grievances</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseRegisterGrievenceForm();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordion').show('slow', function () {

        $("#divRegisterGrievanceListForm").load('/ContractorGrievances/ContractorGrievances/RegisterGrievanceForm?agreementCode=' + agreementCode, function () {
            $.validator.unobtrusive.parse($('#divRegisterGrievanceListForm'));

        });

        $('#divRegisterGrievanceListForm').show('slow');
        $("#divRegisterGrievanceListForm").css('height', 'auto');
    });

    $("#tbRegisterGrievanceList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');
}

function checkFilevalidation(operation) {
    debugger;
    var file = $("#grievanceFile").val();
    if (operation == "add") {
        if (file = "" || file == undefined || file.length == 0) {
            alert("Please select a file");
            return false;
        }
    }
    else {
        if (file = "" || file == undefined || file.length == 0) {
            return true;
        }
    }
    var ext = $("#grievanceFile").val().split('.').pop();
    if (ext.toLowerCase() != "pdf" && ext.toLowerCase() != "jpeg" && ext.toLowerCase() != "png" && ext.toLowerCase() != "jpg") {
        alert("only pdf , jpeg, jpg and png file is allowed.");
        $("#grievanceFile").val('');
        return false;
    }
    var fileSizeKb = $("#grievanceFile")[0].files[0].size;
    var fileSizeMb = fileSizeKb / 1048576;
    if (fileSizeMb > 4) {
        alert("File size should be less than or equal to 4 MB.")
        $(this).val('');
        return false;
    }
    return true;
}






