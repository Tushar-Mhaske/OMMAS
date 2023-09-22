/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QualityATRFilters.js
        * Description   :   Handles events for ATRFilters
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/

$(document).ready(function () {
    $.validator.unobtrusive.parse($('#2TierATRFilterForm'));
    //Below line added on 05-12-2022 to enable searching in dropdown
    $('#monitorCodeATR').chosen();

    $("#id3TierATRFilterDiv").click(function () {
        $("#id3TierATRFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#div3TierATRFilterForm").toggle("slow");

    });

    selectedNameVal = 0;
    $("#stateCodeATR").change(function () {

        $("#monitorCodeATR").empty();

        //Below lines are commented on 13-12-2022
        //alert($("#stateCodeATR").val());

        //if ($(this).val() == 0) {
        //    $("#monitorCodeATR").append("<option value='0'>All Monitors</option>");

        //}

        if ($("#stateCodeATR").val() >= 0) {

            //if ($("#ADMIN_QM_CODE").length > 0)
            {
                $.ajax({
                    url: '/QualityMonitoring/GetNQMNames',
                    type: 'POST',
                    data: { selectedState: $("#stateCodeATR").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#monitorCodeATR").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                        $("#monitorCodeATR").trigger("chosen:updated");//use to reload chosen dropdown
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }
        //$("#monitorCodeATR").trigger("chosen:updated");
    });
    if ($('#PIU_OR_SQC').val() == "P") {
        viewATRDetails();
    }
    else {
       
        viewSQCATRDetails();
    }

    //ATRListGrid($("#stateCodeATR").val(), $("#monitorCodeATR").val(), $("#frmMonthATR").val(), $("#frmYearATR").val(), $("#toMonthATR").val(), $("#toYearATR").val(), $("#atrStatus").val(), $("#rdStatusATR").val());

    $('#btn2TierListPIUATRDetails').click(function () {
       // alert("viewATRDetails");
        viewATRDetails();
        //ATRListGrid($("#stateCodeATR").val(), $("#monitorCodeATR").val(), $("#frmMonthATR").val(), $("#frmYearATR").val(), $("#toMonthATR").val(), $("#toYearATR").val(), $("#atrStatus").val(), $("#rdStatusATR").val());
    });
    $('#btn2TierListSQCATRDetails').click(function () {
        // alert("viewATRDetails");
        viewSQCATRDetails();
        //ATRListGrid($("#stateCodeATR").val(), $("#monitorCodeATR").val(), $("#frmMonthATR").val(), $("#frmYearATR").val(), $("#toMonthATR").val(), $("#toYearATR").val(), $("#atrStatus").val(), $("#rdStatusATR").val());
    });

    $('#btnBulkRegrade').click(function () {
        ShowBulkATRDetails();
        $("#id3TierATRFilterDiv").trigger('click');
    });


});//doc.ready ends here

function viewATRDetails() {
   
    blockPage();
    $.ajax({
        url: '/QualityMonitoring/ATR2TierDetails',
        type: 'POST',
        data: $('#2TierATRFilterForm').serialize(),
        success: function (response) {
         
            $("#div2TierATRDetailsHtmlPage").html('');
            $("#div2TierATRDetailsHtmlPage").html(response);
           
            unblockPage();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            unblockPage();
            alert(xhr.status);
            alert(thrownError);

        }
    });
}


function viewSQCATRDetails() {
   
    blockPage();
    $.ajax({
        url: '/QualityMonitoring/ATR2TierDetails',
        type: 'POST',
        data: $('#2TierATRFilterForm').serialize(),
        success: function (response) {
            
            $("#div2TierATRDetailsHtml").html('');
            $("#div2TierATRDetailsHtml").html(response);
            
            unblockPage();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            unblockPage();
            alert(xhr.status);
            alert(thrownError);

        }
    });
}
