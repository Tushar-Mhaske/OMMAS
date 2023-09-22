$(document).ready(function () {

   // alert("h");
    $("#btnCancel").click(function () {

        $("#tbUpdateProposalList").jqGrid('setGridState', 'visible');
        $('#accordion').show('slow');


        CloseDetails();
        $("#tbUpdateProposalList").trigger('reloadGrid');
        $("#divError").hide();
    });

    $("#btnUpdate").click(function () {
       

        if ($("#frmUpdateDetails").valid()) {
            if (confirm("Once Request is submitted, it can not be modified. Are you sure to submit Request?")) {
                $.ajax({
                    type: 'POST',
                    async: false,
                    url: '/ExistingRoads/MapRoadDetails/',
                    data: $("#frmUpdateDetails").serialize(),
                    beforeSend: function () {
                        blockPage();
                    },
                    success: function (data) {
                        if (data.success == true) {
                            alert(data.message);
                            CloseDetails();
                            $("#tbUpdateProposalList").trigger('reloadGrid');
                            $("#divError").hide();
                            unblockPage();
                        }
                        else if (data.success == false) {
                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                            $.validator.unobtrusive.parse($('#mainDiv'));
                            unblockPage();
                        }
                    },
                    error: function () {
                        alert('Error occurred while processing your request.');
                    },
                });
            }
        }
        else {
            return false;
        }

    });


    // btnSave

});