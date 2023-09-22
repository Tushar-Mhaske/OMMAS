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

    $("#btnAddDetails").click(function () {
        var RoadCode = $("#ProposalCode").val();
        var ContractCode = $("#ContractCode").val();

        if ($("#frmAddTechnology").valid()) {
            $.ajax({
                type: 'POST',
                url: '/MaintenanceAgreement/AddTechnologyDetails/',
                data: $("#frmAddTechnology").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $("#tblistTechnologyDetails").trigger('reloadGrid');
                        $("#divAddTechnologyDetails").load('/MaintenanceAgreement/AddTechnologyDetails?RoadCode=' + RoadCode + '&ContractCode=' + ContractCode, function (response) {
                            $.validator.unobtrusive.parse($('#frmAddTechnology'));
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

            var RoadCode = $("#ProposalCode").val();
            var ContractCode = $("#ContractCode").val();


            $.ajax({
                type: 'POST',
                url: '/MaintenanceAgreement/EditTechnologyDetails/',
                data: $("#frmAddTechnology").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $("#tblistTechnologyDetails").trigger('reloadGrid');
                        $("#divAddTechnologyDetails").load('/MaintenanceAgreement/AddTechnologyDetails?RoadCode=' + RoadCode + '&ContractCode=' + ContractCode, function (response) {
                            $.validator.unobtrusive.parse($('#frmAddTechnology'));
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
        //CloseProposalDetails();
        if ($("#accordion").is(":visible")) {
            $('#accordion').hide('slow');
        }

        ViewSearchDiv();
        $('#tbProposedRoadList').jqGrid("setGridState", "visible");

        $("#dvAgreement").animate({
            scrollTop: 0
        });
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
        url: '/MaintenanceAgreement/GetStartChainage/' + ProposalCode + "$" + TechCode + "$" + LayerCode,
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