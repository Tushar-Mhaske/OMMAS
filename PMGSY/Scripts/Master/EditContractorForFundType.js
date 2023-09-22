
$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmMasterContReg");

    $('#btnUpdate').click(function (e) {

        $("#ddlContractors").attr('disabled', false);

        if ($('#frmMasterContReg').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            var stateCode = $('#ddlState').val();          

            $("#ddlState").attr("disabled", false);
            $.ajax({
                url: "/Master/EditMasterContractorRegFundType/",
                type: "POST",

                data: $("#frmMasterContReg").serialize(),
                success: function (data) {

                    if (data.success == true) {
                        alert(data.message);                      
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

        if ($("#dvDetailsContractorReg").is(":visible")) {
            $('#dvDetailsContractorReg').hide('slow');
            $('#btnSearch').hide();
            $('#btnCreateNew').show();
        }

        if (!$("#dvSearchContractorReg").is(":visible")) {
            $('#dvSearchContractorReg').show('slow');
        }
    });
   
});

function SearchCreateContractorRegisDetails(stateCode) {

    $('#State').val(stateCode);
    if ($('#stateCode').val() > 0) {

        $("#State").val($('#stateCode').val());
    }
 
    $('#tblstContractorReg').setGridParam({
        url: '/Master/GetContractorRegList'
    });

    $('#tblstContractorReg').jqGrid("setGridParam", { "postData": { stateCode: $('#State option:selected').val(), status: $('#Status option:selected').val(), contractorName: $('#txtContractor').val(), conStatus: $('#ContractorStatus option:selected').val(), panNo: $("#txtPan").val(), classType: $("#ClassType option:selected").val(), regNo: $("#txtRegNo").val(), companyName: $("#txtCompanyName").val() } });

    $('#tblstContractorReg').trigger("reloadGrid", [{ page: 1 }]);



}