
$(document).ready(function () {


    $('#btnCreateNew_AgreementDetails').click(function () {

        if (!$('#dvNewAgreement').is(':visible')) { 
            var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();
            //$("#dvAgreementDetails").load("/MaintenanceAgreement/MaintenanceAgreementDetails/" + encryptedIMSPRCode, function () {
            //    //$('#trAgreementType').show('slow');
            //    //$('#trEmptyAgreementType').show('slow');
            //    $('#dvNewAgreement').show('slow');
            //   // $('#tblContractor').show('slow');
            //    $('#dvNewExistingAgreement').hide();
            //    $('#dvAgreementDetails').show('slow');
            //    $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);

               
            //});


            $.ajax({
                url: "/MaintenanceAgreement/MaintenanceAgreementDetails/" + encryptedIMSPRCode,
                type: "GET",
                dataType: "html",
                async:false,
                success: function (data) {

                    $("#dvAgreementDetails").html(data);
                    $('#trAgreementType').show('slow');                     
                    $('#dvNewAgreement').show('slow'); 
                    $('#dvNewExistingAgreement').hide();
                    $('#dvAgreementDetails').show('slow');
                    $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);
                   
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }

            });

        }

        //if ($('#ddlProposalWorks option').length == 1) {

        //    $.ajax({
        //        url: "/MaintenanceAgreement/CheckForExistingorNewContractor",
        //        type: "GET",
        //        data: { IMSPRRoadCode: $('#EncryptedIMSPRRoadCode').val(), IMSWorkCode: $('#ddlProposalWorks option:selected').val() },
        //        //dataType: "html",
        //        success: function (data) {

        //            if (data.success == true) {
        //                $("#rdoIsNewContractorYes").attr('checked', true);
        //                $("#rdoIsNewContractorYes").attr('disabled', true);
        //                $("#rdoIsNewContractorNo").attr('disabled', true);
        //                IsNewContractorYes();
        //            }

        //        },
        //        error: function (xhr, ajaxOptions, thrownError) {
        //            alert(xhr.responseText);
        //        }

        //    });//end inner ajax

        //}//end   if ($('#ddlProposalWorks option').length == 1) 

      
    });

    $('#btnAgreementDetailsCancel').click(function () {


        if ($("#accordion").is(":visible")) {
            $('#accordion').hide('slow');
        }

        ViewSearchDiv();
        $('#tbProposedRoadList').jqGrid("setGridState", "visible");

        $("#dvAgreement").animate({
            scrollTop: 0
        });

    });

    $('#imgCloseAgreementDetails').click(function () {


        if ($("#accordion").is(":visible")) {
            $('#accordion').hide('slow');
        }

        ViewSearchDiv();
        $('#tbProposedRoadList').jqGrid("setGridState", "visible");

        $("#dvAgreement").animate({
            scrollTop: 0
        });

    });

    $("#dvIncompleteReason").dialog({
        autoOpen: false,
        height: '220',
        width: "420",
        modal: true,
        title: 'Incomplete Reason.'
    });

    $("#dvViewAgreementMaster").dialog({
        autoOpen: false,
        height: 'auto',
        width: '820',
        modal: true,
        title: 'Agreement Details'
    });

    $("#searchContractor").click(function () {

        $("#divPanSearch").load('/Agreement/SearchContractorByPan');
        $("#divPanSearch").dialog('open');
    });

});


function ViewSearchDiv() {
    
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    if (!$("#dvSearchProposedRoad").is(":visible")) {

        var data = $('#tbProposedRoadList').jqGrid("getGridParam", "postData");

        if (!(data === undefined)) {

            $('#ddlFinancialYears').val(data.sanctionedYear);
            $('#ddlBlocks').val(data.blockCode);        
        }

        $("#dvSearchProposedRoad").show('slow');       
        $.unblockUI();
    }
    $.unblockUI();

}
function AddTechnologyDetails(RoadCode, ContractCode) {
    
    

    $("#divTechnologyForm").load('/MaintenanceAgreement/ListTechnologyDetails', { RoadCode: RoadCode, ContractCode: ContractCode }, function (response) {
        $('#divMaintenance').hide('slow');
    });
    $('#divTechnologyForm').show('slow');
    $("#divTechnologyForm").css('height', 'auto');

}