$.validator.addMethod("comparechainage", function (value, element, params) {

    var startChainage = parseFloat($("#IMS_START_CHAINAGE").val());
    var endChainage = parseFloat($("#IMS_END_CHAINAGE").val());
    if (startChainage < endChainage) {
        return true;
    }
    return false;
});
jQuery.validator.unobtrusive.adapters.addBool("comparechainage");

$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmAddTechnology'));

    $(":input").bind("keypress", function (e) {
        if (e.keyvalue == 13) {
            return false;
        }
    })

    // Added on 01-06-2023 for FDR changes    
    FDR_AdditionalFields_Visibility();

    $('#MAST_TECH_CODE').change(function () {

        $.ajax({
            type: 'POST',
            url: '/Proposal/PopulateLayersTechnologywise/',
            data: { techCode: $('#MAST_TECH_CODE').val() },
            async: false,
            cache: false,
            success: function (jsonData) {
                
                if (jsonData.length > 0) {
                    $("#MAST_LAYER_CODE").empty();
                    for (var i = 0; i < jsonData.length; i++) {
                        if (jsonData[i].Selected == true) {
                            $("#MAST_LAYER_CODE").append("<option value='" + jsonData[i].Value + "' selected=true>" + jsonData[i].Text + "</option>");
                        }
                        else {
                            $("#MAST_LAYER_CODE").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    }
                }
                //else //if (data.success == false)
                //{
                //    $("#divError").show();
                //    $("#divError").html('<strong>Alert : </strong>' + data.message);
                //}
            },
            error: function () {
                alert("Request can not be processed at this time.");
            }

        })

        // Added on 01-06-2023 for FDR changes
        // To show only for Technology: FDR Cement Stabilization
        FDR_AdditionalFields_Visibility();

    });


    // Added on 01-06-2023 for FDR changes
    // To show only for Technology: FDR Cement Stabilization
    function FDR_AdditionalFields_Visibility() {

        if ($('#MAST_TECH_CODE').val() != 64) {
            $('#additionalFields_FDR_cement').hide();
        }
        else {
            $('#additionalFields_FDR_cement').show();
        }
    }



    $("#btnAddDetails").click(function () {

        if ($("#frmAddTechnology").valid()) {
            $.ajax({
                type: 'POST',
                url: '/Proposal/AddTechnologyDetails/',
                data: $("#frmAddTechnology").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $("#tblistTechnologyDetails").trigger('reloadGrid');
                        $("#divAddTechnologyDetails").load('/Proposal/AddTechnologyDetails?id=' + $("#IMS_PR_ROAD_CODE").val(), function (response) {
                            $.validator.unobtrusive.parse($('#divProposalForm'));
                            unblockPage();
                        });
                        $("#divError").hide();
                        //CloseProposalDetails();
                    }
                    else if (data.success == false) {
                        $("#divError").show();
                        $("#divError").html('<strong>Alert : </strong>' + data.message);
                    }
                },
                error: function () {
                    alert("Request can not be processed at this time.");
                }

            })
        }
    });

    $("#btnUpdateDetails").click(function () {

        if ($("#frmAddTechnology").valid()) {

            $.ajax({
                type: 'POST',
                url: '/Proposal/EditTechnologyDetails/',
                data: $("#frmAddTechnology").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $("#tblistTechnologyDetails").trigger('reloadGrid');
                        $("#divAddTechnologyDetails").load('/Proposal/AddTechnologyDetails?id=' + $("#IMS_PR_ROAD_CODE").val(), function (response) {
                            $.validator.unobtrusive.parse($('#divProposalForm'));
                            unblockPage();
                        });
                        $("#divError").hide();
                    }
                    else if (data.success == false) {
                        $("#divError").show();
                        $("#divError").html('<strong>Alert : </strong>' + data.message);
                    }
                    if (data.success === undefined) {
                        alert("Request can not be processed at this time.Please Try Again.");
                    }
                },
                error: function () {
                    alert("Request can not be processed at this time.");
                }
            })
        }
    });



    $("#btnCancelDetails").click(function () {

       // alert("CancelDetails")
        $("#tblistTechnologyDetails").trigger('reloadGrid');
        $("#divAddTechnologyDetails").load('/Proposal/AddTechnologyDetails?id=' + $("#IMS_PR_ROAD_CODE").val(), function (response) {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });
        $("#divError").hide();
       // CloseProposalDetails();
    });

    //$("#MAST_TECH_CODE,#MAST_LAYER_CODE").change(function () {

    //    if ($("#MAST_LAYER_CODE option:selected").val() != 0 && $("#MAST_TECH_CODE option:selected").val() != 0)
    //    {
    //        GetStartChainage($("#IMS_PR_ROAD_CODE").val(), $("#MAST_LAYER_CODE option:selected").val(), $("#MAST_TECH_CODE option:selected").val());
    //    }

    //});


});

function GetStartChainage(ProposalCode,LayerCode,TechCode)
{
    $.ajax({

        type: 'POST',
        url: '/Proposal/GetStartChainage/' + ProposalCode + "$" + TechCode + "$" + LayerCode,
        async: false,
        cache: false,
        success: function (data) {
            if (data.Success == true) {
                if (data.StartChainage != null) {
                    $("#lblStartChainage").html(data.StartChainage);
                    $("#IMS_START_CHAINAGE").val(parseFloat($("#lblStartChainage").html()).toFixed(3));
                } else {
                    $("#lblStartChainage").html("0.00");
                    $("#IMS_START_CHAINAGE").val(parseFloat($("#lblStartChainage").html()).toFixed(3));
                }
            }
            
        },
        error: function () {
        }

    });
}