$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmPhyProgessWork'));

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#loadFilters").toggle("slow");

    });

    $("#btnViewPhyProgressWork").click(function () {
       
        if ($('#frmPhyProgessWork').valid()) {
            $("#loadReport").html("");

            if ($("#YearList_PhyProgressWorkDetails").is(":visible")) {
                
                $("#Year").val($("#YearList_PhyProgressWorkDetails option:selected").text()); 
            }
            if ($("#StateList_PhyProgressWorkDetails").is(":visible")) {
                
                $("#StateName").val($("#StateList_PhyProgressWorkDetails option:selected").text()); 
            }
            if ($("#BatchList_PhyProgressWorkDetails").is(":visible")) {
                
                $("#Batch").val($("#BatchList_PhyProgressWorkDetails option:selected").text()); 
            }
            
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProgressReport/Progress/GetCuplExclusionPost/',
                type: 'POST',
                catche: false,
                data: $("#frmPhyProgessWork").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadReport").html(response);

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
    
});
