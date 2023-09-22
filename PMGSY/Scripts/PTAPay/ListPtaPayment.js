$(document).ready(function () {

    $("#idFilterDiv").click(function () {
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divFilterForm").toggle("slow");
    });

    $("#btnGo").click(function () {                
        blockPage();
        if (validate()) {
            $("#divPtaPayment").load('/PtaPay/PtaPaymentList/' + $("#ddlState").val() + "$" + $("#ddlImsYear").val() + "$" + $("#ddlImsBatch").val() + "$" + $("#ddlImsStreams").val() + "$" + $("#ddlImsProposalTypes").val() + "$" + $("#ddlImsProposalSchemes").val(), function () {
                unblockPage();
            });
        }
        unblockPage();
    });

    function validate()
    {
        if ($("#ddlState").val() == "0") {
            alert("Please Select State.");
            return false;
        }
        if ($("#ddlImsYear").val() == "0") {
            alert("Please Select Year.");
            return false;
        }
        if ($("#ddlImsBatch").val() == "0") {
            alert("Please Select Batch.");
            return false;
        }
        if ($("#ddlImsStreams").val() == "0") {
            alert("Please Select Funding Agency.");
            return false;
        }  
        return true;
    }
});

