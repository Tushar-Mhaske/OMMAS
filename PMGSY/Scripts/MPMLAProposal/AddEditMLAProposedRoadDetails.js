/* 
     *  Name : AddEditMLAProposedRoadDetails.js
     *  Path : ~/Scripts/MPMLAProposal/AddEditMLAProposedRoadDetails.js
     *  Description : AddEditMLAProposedRoadDetails.js is used to add MLA Proposed Road details
     *  Author : Abhishek Kamlble(PE, e-gov)
     *  Company : C-DAC,E-GOV
     *  Dates of Creation : 05/Jul/2013
 */

$(document).ready(function () {

    $.validator.unobtrusive.parse($('frmCreateMLAProposal'));

    //add MLA Proposed Road Details
    $('#btnCreate').click(function (evt) {
        evt.preventDefault();

        year_code = $("#IMS_YEAR").val();
        const_code = $("#MAST_MLA_CONST_CODE").val();

        if ($('#frmCreateMLAProposal').valid()) {
            $.ajax({
                url: '/MPMLAProposal/AddMLAProposedRoadDetails',
                type: "POST",
                cache: false,
                data: $("#frmCreateMLAProposal").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    Alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },

                success: function (response) {
                    if (response.success == undefined) {
                        $("#divMLAProposedRoadForm").html(response);
                    }
                    else if (response.success) {
                        alert(response.message);
                        $("#btnReset").trigger("click");
                        CloseMLAProposedRoadDetails();

                        $('#tbMLAProposedRoadList').jqGrid("setGridParam", { "postData": { Year: year_code, Constituency: const_code } });
                        $('#tbMLAProposedRoadList').trigger("reloadGrid", [{ page: 1 }]);

                        unblockPage();
                    }
                    else {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.message);
                        unblockPage();
                    }
                    unblockPage();
                }
            });//end of ajax        
        }
    });

    //Update MLA Proposed Road Details
    $('#btnUpdate').click(function (evt) {
        evt.preventDefault();

        year_code = $("#IMS_YEAR").val();
        const_code = $("#MAST_MLA_CONST_CODE").val();

        if ($('#frmCreateMLAProposal').valid()) {
            $.ajax({
                url: '/MPMLAProposal/EditMLAProposedRoadDetails',
                type: "POST",
                cache: false,
                data: $("#frmCreateMLAProposal").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    Alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },
                success: function (response) {
                    if (response.success == undefined) {
                        $("#divMLAProposedRoadForm").html(response);
                    } else if (response.success) {
                        alert(response.message);
                        $("#btnReset").trigger("click");
                        $('#tbMLAProposedRoadList').jqGrid("setGridParam", { "postData": { Year: year_code, Constituency: const_code } });
                        $('#tbMLAProposedRoadList').trigger("reloadGrid", [{ page: 1 }]);
                        CloseMLAProposedRoadDetails();
                        unblockPage();
                    }
                    else {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.message);
                        unblockPage();
                    }
                    unblockPage();
                }
            });//end of ajax
        }
    });

    //btnCancel
    $("#btnCancel").click(function () {
        $("#accordion div").html("");
        $("#accordion h3").html(
                    "<a href='#' style= 'font-size:.9em;' >Add MLA Proposed Road Details</a>" +
                    '<a href="#" style="float: right;">' +
                    '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseMLAProposedRoadDetails();" /></a>'
                    );

        $('#accordion').show('fold', function () {
            blockPage();
            $("#divMLAProposedRoadForm").load('/MPMLAProposal/AddEditMLAProposedRoadDetails/', function () {
                $.validator.unobtrusive.parse($('#divMLAProposedRoadForm'));
                unblockPage();
            });

            $('#divMLAProposedRoadForm').show('slow');
            $("#divMLAProposedRoadForm").css('height', 'auto');
        });

        $("#tbMLAProposedRoadList").jqGrid('setGridState', 'hidden');

    });
});
