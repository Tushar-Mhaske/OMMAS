$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmDownload'));
 

    
    var ProposalCode = $("#proposalCode").val();
 //   alert(ProposalCode);

    $("#btnDownload").click(function () {
        window.location.href = '/Execution/DownloadExecLocationDetails?ProposalCode=' + ProposalCode;
    });

    


    //$("#btnDownload").click(function () {
    //    alert("File will be downloaded to C Drive.");

    //    if ($('#frmDownload').valid()) {
    //        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
    //        $.ajax({
    //            url: '/Execution/DownloadExecLocationDetails/',
    //            type: 'POST',
    //            catche: false,
    //            data: $("#frmDownload").serialize(),
    //            async: false,
    //            success: function (response) {
    //                $.unblockUI();
    //             //   $("#loadAnaProposal").html(response);
    //                alert("Location Details File Downloaded Succesfully.")
    //            },
    //            error: function () {
    //                $.unblockUI();
    //                alert("An Error");
    //                return false;
    //            },
    //        });

    //    }
    //    else {

    //    }
    //});


});

