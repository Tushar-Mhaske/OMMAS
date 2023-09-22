$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmStateListRoadLayout'));
    $("#StateList_StateListRoadDetails").change(function ()
    {
        loadDistrict($("#StateList_StateListRoadDetails").val());

    });

    $("#DistrictList_StateListRoadDetails").change(function ()
    {
        loadBlock($("#StateList_StateListRoadDetails").val(), $("#DistrictList_StateListRoadDetails").val());

    });
    $("#btnViewStateListRoad").click(function ()
    {




        if ($('#frmStateListRoadLayout').valid()) {
            $("#dvloadSLRReport").html("");
          //  $("#BatchName").val($("#BatchList_StateListRoadDetails option:selected").text());
           // $("#CollaborationName").val($("#FundingAgencyList_StateListRoadDetails option:selected").text());
            //$("#StatusName").val($("#StatusList_StateListRoadDetails option:selected").text());

            if ($("#StateList_StateListRoadDetails").is(":visible")) {

                $("#StateName").val($("#StateList_StateListRoadDetails option:selected").text());
            }

            //if ($("#DistrictList_StateListRoadDetails").is(":visible")) {

            //    //$('#DistrictList_StateListRoadDetails').attr("disabled", false);
            //    $("#DistName").val($("#DistrictList_StateListRoadDetails option:selected").text());
            //}
            //if ($("#BlockList_StateListRoadDetails").is(":visible")) {

            //    $("#BlockName").val($("#BlockList_StateListRoadDetails option:selected").text());
            //}

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/PackageAgreementSanctionList/PackageAgreement/GepnicBidReport/',
                type: 'POST',
                catche: false,
                data: $("#frmStateListRoadLayout").serialize(),
                async: false,
                success: function (response)
                {
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

