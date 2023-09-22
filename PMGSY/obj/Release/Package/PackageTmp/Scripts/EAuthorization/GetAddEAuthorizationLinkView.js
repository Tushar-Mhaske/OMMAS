$(document).ready(function () {

    $("#AUTHORIZATION_AMOUNT").focus();
    //Save eAuth Details in new Table for first Entry...i.e on Click on link
    $("#btnSavelnkEAuthorizationDetails").click(function (e) {
        
        
        $.validator.unobtrusive.parse($("#frmlnkAddEAuthorizationDetails"));

        if ($("#frmlnkAddEAuthorizationDetails").valid()) {
            if (confirm("Authorization Amount Entered Against Selected Contractor,Agreement,Package Cannot be change futher,Do you want to Save Details ? ")) {

                blockPage();
                $.ajax({
                    type: "POST",
                    url: "/EAuthorization/AddNewAuthorizationEntry/" + Bill_ID,
                    data: $("#frmlnkAddEAuthorizationDetails").serialize(),
                    error: function (xhr, status, error) {
                        unblockPage();
                        alert("Error while Processing");

                        return false;
                    },

                    success: function (data) {
                        unblockPage();
                        if (data.Success) {
                            alert("e-Authorization Details Saved Successfully");

                            $("#trAlreadyAuthAmount").show();
                            $("#txtAlreadyAutAmount").show();
                            $("#ALREADY_AUTHORISED_AMOUNT").val(data.AuthorisedAmount);
                            $("#ALREADY_AUTHORISED_AMOUNT").attr('readonly', 'readonly');
                            $("#lnkAlreadyAutAmount").hide();
                            $("#dialogAddAuthorizationDetails").dialog('close');

                            $("#AMOUNT_Q").focus();


                        } else {
                            alert(data.errMessage);
                        }
                    }
                });
            }
        }

        

    });
});