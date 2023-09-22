
$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmMasterAgency");

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

   
    $('#btnSave').click(function (e) {
        if ($('#frmMasterAgency').valid()) {
            var agencyCode = $("#ddlAgencyType option:selected").val();

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/AddMasterAgency/",
                type: "POST",

                data: $("#frmMasterAgency").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);

                        agencyDetails(agencyCode);
                        $('#tblMasterAgencyList').trigger('reloadGrid');
                        $("#dvAgencyDetails").load("/Master/AddEditMasterAgency");

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

});

function agencyDetails(agencyCode) {
    $('#tblMasterAgencyList').setGridParam({
        url: '/Master/GetMasterAgencyList', datatype: 'json'
    });
    $('#tblMasterAgencyList').jqGrid("setGridParam", { "postData": { AgencyType: agencyCode } });
    $('#tblMasterAgencyList').trigger("reloadGrid", [{ page: 1 }]);
}