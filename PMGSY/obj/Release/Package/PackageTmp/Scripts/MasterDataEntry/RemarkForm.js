$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmRemarkText'));

    $("#btnSubmitRemark").click(function () {
        DeleteImageWithRemark();
    });

    $("#btnRemarkCancel").click(function () {
        $("#dvhdRemark").hide("fast");
        $("#IdRemarkForm").hide("fast");

        $('#dvPanchyatList').show('slow');
    });

});


function DeleteImageWithRemark() {

    var id = $("#FacID").val();
    if ($('#frmRemarkText').valid()) {
        if (confirm("Are you sure to delete the photograph against the Habitation "+$("#HABNAME").val() + "?")){
        $.ajax({
            url: "/LocationMasterDataEntry/DeleteImageLatLong/" + id,
            type: "POST",
            cache: false,
            data: $("#frmRemarkText").serialize(),
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
                if (response.success) {
                    unblockPage();
                    $("#spnImageDeleteButton").hide();
                    alert(response.message);
                    $('#tbPanchyatList').trigger('reloadGrid');
                    $("#dvhdRemark").hide("fast");
                    
                    $("#IdRemarkForm").hide("fast");
                    $('#dvPanchyatList').show('slow');
                }
                else {
                    alert(response.message);
                }
            }
        });
    }
    }

}
