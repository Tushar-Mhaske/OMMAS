/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QualityATRFilters.js
        * Description   :   Handles events for ATRFilters
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/

$(document).ready(function () {


    $("#id3TierATRFilterDiv").click(function () {
        $("#id3TierATRFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#div3TierATRFilterForm").toggle("slow");

    });

    selectedNameVal = 0;
    $("#stateCodeATR").change(function () {

        $("#monitorCodeATR").empty();

        if ($(this).val() == 0) {
            $("#monitorCodeATR").append("<option value='0'>All Monitors</option>");
        }

        if ($("#stateCodeATR").val() > 0) {

            if ($("#ADMIN_QM_CODE").length > 0) {

                $.ajax({
                    url: '/QualityMonitoring/GetNQMNames',
                    type: 'POST',
                    data: { selectedState: $("#stateCodeATR").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#monitorCodeATR").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }

    });

    viewATRDetails();

    $('#btn3TierListATRDetails').click(function () {
        viewATRDetails();
    });

    //$('#btnBulkRegrade').click(function () {
    //    ShowBulkATRDetails();
    //    $("#id3TierATRFilterDiv").trigger('click');
    //});


});//doc.ready ends here

function viewATRDetails()
{
    blockPage();
    $.ajax({
        url: '/Master/QualityATRDetails',
        type: 'POST',
        data: $('#3TierATRFilterForm').serialize(),
        success: function (response) {
            $("#div3TierATRDetailsHtml").html('');
            $("#div3TierATRDetailsHtml").html(response);
            unblockPage();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            unblockPage();
            alert(xhr.status);
            alert(thrownError);
            
        }
    });
}



