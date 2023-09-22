$(document).ready(function () {
   
    
    if ($("#OPERATION").val() == "C") {
        $("#btnSave").attr("value", "Save");
    }
    else if ($("#OPERATION").val() == "U") {
        $("#btnReset").hide();
        $("#btnSave").attr("value", "Update");
    }



    //button Create Click
    $('#btnSave').click(function (evt) {
        

        evt.preventDefault();
        if ($('#frmLSBComponentDetails').valid()) {

            if ($("#OPERATION").val() == "C") {
                $.ajax({
                    url: '/LSBProposal/AddLSBComponentDetails',
                    type: "POST",
                    cache: false,
                    data: $("#frmLSBComponentDetails").serialize(),
                    beforeSend: function () {
                        blockPage();
                    },
                    error: function (xhr, status, error) {
                        unblockPage();
                        Alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },
                    success: function (response) {
                        if (response.success) {
                            alert("Component details saved succesfully.");
                            $("#divProposalForm").load("/LSBProposal/ShowLSBComponentList/" + $("#IMS_PR_ROAD_CODE").val());
                        }
                        else {
                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                        }
                        unblockPage();
                    }
                });
            }

            if ($("#OPERATION").val() == "U") {
                $.ajax({
                    url: '/LSBProposal/AddLSBComponentDetails',
                    type: "POST",
                    cache: false,
                    data: $("#frmLSBComponentDetails").serialize(),
                    beforeSend: function () {
                        blockPage();
                    },
                    error: function (xhr, status, error) {
                        unblockPage();
                        Alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },
                    success: function (response) {
                        if (response.success) {
                            alert("Component details updated succesfully.");
                            $("#divProposalForm").load("/LSBProposal/ShowLSBComponentList/" + $("#IMS_PR_ROAD_CODE").val());
                        }
                        else {

                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);

                            $.validator.unobtrusive.parse($('#mainDiv'));
                        }
                        unblockPage();
                    }
                });
            }
        }
        else {
            $('.qtip').show();
        }
    });//btnCreate ends here


});//doc.ready ends here


