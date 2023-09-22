


$(document).ready(function () {
    
   // alert("OK");
    LoadDiv("/QualityMonitoring/GetQMComplainList");
});

function LoadDiv(url) {
   // $("#divQMComplainCreateForm").load(url);
    $("#divQMComplainContainer").load(url, function () {
        //$.validator.unobtrusive.parse($('#divProposalForm'));
       
    });
}


