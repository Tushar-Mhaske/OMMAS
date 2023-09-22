
$(document).ready(function () {
   
    $("#tabs").tabs();
   

    
});

function LoadMordSanctionForm() {
    
     blockPage();
    $("#divMordSanctionDetails").load('/BuildingProposal/BuildingMordSanctionDetail/' + $("#IMS_PR_ROAD_CODE").val(), function () {
        //alert("Load Mord");
        $.validator.unobtrusive.parse($('#frmMordSanction'));
        unblockPage();
    });
    $('#divMordSanctionDetails').show('slow');
    unblockPage();

}


