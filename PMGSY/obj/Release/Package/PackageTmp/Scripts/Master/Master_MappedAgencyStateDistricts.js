$(document).ready(function () {

    //Added By Abhishek kamble 4-Apr-2014
    $("#btnShowDetails").click(function () {

        $("#tbMappedAgencyStateDistrictList").jqGrid('setGridParam', { postData: {AgencyCode: $('#EncryptedAgencyCode_Mapped').val(), StateCode: $("#StateCode").val() } }).trigger('reloadGrid');
        //$("#tbMappedAgencyStateDistrictList").trigger('reloadGrid');

    });
    $("#dvhdSearch_Mapped").click(function () {

        if ($("#dvSearchParameter_Mapped").is(":visible")) {

            $("#spCollapseIconS_Mapped").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

     

            $(this).next("#dvSearchParameter_Mapped").slideToggle(300);
        }

        else {
            $("#spCollapseIconS_Mapped").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvSearchParameter_Mapped").slideToggle(300);
        }
    });



    $('#btnMappedCancel').click(function (e) {


        if ($("#dvMappedAgencyStateDetails").is(":visible")) {
            $('#dvMappedAgencyStateDetails').hide('slow');
        }
        $('#btnSearchView').trigger('click');
        $('#tblList').jqGrid("setGridState", "visible");
        $('#trAddNewSearch').show();

        //Added By Abhishek kamble 26-Feb-2014        
        $('#btnSearchView').hide('slow');
        $('#btnAddAgency').show('slow');


        $("#mainDiv").animate({
            scrollTop: 0
        });
      
    });

   

});


