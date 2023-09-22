/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   
        * Description   :   Handles click event.
        * Author        :   Rohit Jadhav. 
        * Creation Date :   
 **/

$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmQMCommenseInspDetails'));

    if ($("#hdnRoleCode").val() == "2")
    {
        $("#StateName").val($('#ddlStateCommence option:selected').text());
        loadReport();
    }

    $("#btnCommenceView").click(function () {

        if ($('#frmQMCommenseInspDetails').valid()) {

            $("#loadReport1").html("");
            $("#loadReport2").html("");
            $("#loadReport3").html("");
            $("#LoadQMInspDetailsReport").html("");
            $("#StateName").val($('#ddlStateCommence option:selected').text());
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            loadReport();
        }
    });
});

$("#spCollapseIconCN").click(function () {

    $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
    $("#dvSearchParameter").toggle("slow");

});

function loadReport() {
    $.ajax({
        url: '/ProposalSSRSReports/ProposalSSRSReports/WronglyMappedHabitationsReport/',
        type: 'POST',
        catche: false,
        data: $("#frmQMCommenseInspDetails").serialize(),
        async: false,
        success: function (response) {
            $.unblockUI();

            $("#loadReport1").html(response);

        },

        error: function () {
            $.unblockUI();
            alert("An Error");
            return false;
        },
    });
}