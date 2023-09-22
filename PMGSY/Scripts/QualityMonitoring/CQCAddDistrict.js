/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   CQCAddDistrict.js
        * Description   :   Add district to Monitor's schedule in CQC Login
        * Author        :   Shyam Yadav 
        * Creation Date :   10/Jun/2013
 **/

$(document).ready(function () {
    
    $.validator.unobtrusive.parse($('#frmCQCAddDistrict'));
    $.validator.unobtrusive.parse($('#frmQMDeleteDistrict'));

    $('#btnAddDistrict').click(function (evt) {
        evt.preventDefault();
    if ($('#frmCQCAddDistrict').valid()) {
            $.ajax({
                url: '/QualityMonitoring/CQCAddDistricts',
                type: "POST",
                cache: false,
                data: $("#frmCQCAddDistrict").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },
                success: function (response) {
                    if (response.Success) {
                        alert("District added successfully.");
                        $("#tb3TierScheduleList").trigger("reloadGrid");
                        //resetDistrictList();
                        CloseScheduleDetails();
                    }
                    else {
                        $("#divAddDistError").show("slow");
                        $("#divAddDistError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                    }
                    unblockPage();
                } 
            });
    }
    else {
        $('.qtip').show();
    }

    });//btnAddDistrict ends here


    $('#btnCancelAddDistrict').click(function () {
        CloseScheduleDetails();
    });

    $('#btnCancelDeleteDistrict').click(function () {
        CloseScheduleDetails();
    });

    $('#btnDeleteDistrict').click(function (evt) {

        evt.preventDefault();
        if ($('#frmQMDeleteDistrict').valid()) {
            if (confirm('Are you sure to delete?')) {
                $.ajax({
                    url: '/QualityMonitoring/QMDeleteDistricts',
                    type: "POST",
                    cache: false,
                    data: $("#frmQMDeleteDistrict").serialize(),
                    beforeSend: function () {
                        blockPage();
                    },
                    error: function (xhr, status, error) {
                        unblockPage();
                        alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },
                    success: function (response) {
                        if (response.Success) {
                            alert("District deleted successfully.");
                            $("#tb3TierScheduleList").trigger("reloadGrid");
                            CloseScheduleDetails();
                        }
                        else {
                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                        }
                        unblockPage();
                    }
                });//ajax ends here
            }//prompt ends here
        }
        else {
            $('.qtip').show();
        }

    });//btnDeleteDistrict ends here


}); //doc.ready ends ghere