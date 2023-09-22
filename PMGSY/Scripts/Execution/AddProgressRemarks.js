$(document).ready(function () {


    $("input").bind('keypress', function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    })


    //save the CDWorks details
    $("#btnAddRemarksDetails").click(function () {

        if ($("#frmAddProposalRemarks").valid()) {
            $.ajax({
                type: 'POST',
                url: '/Execution/AddProgressRemarks/',
                data: $("#frmAddProposalRemarks").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $("#tbListRemarks").trigger('reloadGrid');
                    }
                    else if(data.success==false) {
                        $("#divError").show();
                        $("#divError").html('<strong>Alert : </strong>' + data.message);
                    }
                },
                error: function () {
                    alert("Request can not be processed at this time.");
                }

            })
        }
    });


    $("#btnUpdateRemarksDetails").click(function () {
        
        if ($("#frmAddProposalRemarks").valid()) {
            $.ajax({
                type: 'POST',
                url: '/Execution/EditRemark/',
                data: $("#frmAddProposalRemarks").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $("#tbListRemarks").trigger('reloadGrid');
                    }
                    else if(data.success == false) {
                        $("#divError").show();
                        $("#divError").html('<strong>Alert : </strong>' + data.message);
                    }
                },
                error: function () {
                    alert("Request can not be processed at this time.");
                }

            })
        }
    });

    $("#btnCancelDetails").click(function () {

        $("#divAddRemarks").hide('slow');
    });
   
});

