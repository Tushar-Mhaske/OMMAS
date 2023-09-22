$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmStateListRoadLayout'));
   
    $("#btnViewStateListRoad").click(function () {
        if ($('#frmStateListRoadLayout').valid()) {
            $("#dvloadSLRReport").html("");
         

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/PackageAgreementSanctionList/PackageAgreement/IssuesReport/',
                type: 'POST',
                catche: false,
                data: $("#frmStateListRoadLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvloadSLRReport").html(response);

                },
                error: function () {
                    $.unblockUI();
                    alert("An Error");
                    return false;
                },
            });

        }
        else {

        }
    });



    if ($('#Mast_State_Code').val() > 0) {
        $("#btnViewStateListRoad").trigger('click');
    }
    //this function call  on layout.js
    closableNoteDiv("divStateListRoad", "spnStateListRoad");


    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmStateListRoadLayout").toggle("slow");

    });
});

