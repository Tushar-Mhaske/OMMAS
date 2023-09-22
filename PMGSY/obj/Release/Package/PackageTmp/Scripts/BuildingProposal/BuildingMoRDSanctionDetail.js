$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmMordSanction'));

   

    $(function () {
        $("#IMS_SANCTIONED_DATE").datepicker(
        {
            dateFormat: "dd-M-yy",
            changeMonth: true,
            changeYear: true,
            //minDate: -365,
            maxDate: "+0M +0D",
            showOn: 'button',
            buttonImage: '../../Content/images/calendar_2.png',
            buttonImageOnly: true,
            onClose: function () {
                $(this).focus().blur();
            }
        });
        $("#IMS_SANCTIONED_DATE").datepicker().attr('readonly', 'readonly');
        $("#IMS_SANCTIONED_DATE").datepicker("option", "maxDate", new Date());
    });

   

    $("#btnUpdate").click(function (evt) {
        alert("OK");
        evt.preventDefault();


        if ($('#frmMordSanction').valid()) {
         
            $.ajax({
                    url: "/BuildingProposal/BuildingMordSanctionDetail/",
                type: "POST",
                cache: false,
                data: $("#frmMordSanction").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    Alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },
                success: function (response) {
                    unblockPage();
                    if (response.Success) {
                        $("#tblEditProposal").hide('slow');

                        if (response.Message != undefined && response.Message != ""){
                            alert(response.Message);
                        }
                        else {
                            alert("Proposal Details Updated Succesfully.");
                        }
                        
                        
                        CloseProposalDetails();
                            
                        $('#tbMoRDBuildingProposalList').trigger("reloadGrid");
                    }
                    else {                        
                        $("#divError").html(response.ErrorMessage);
                        $("#divError").show('slow');
                        return false;
                    }
                }
            });
          
        }

    });

});
