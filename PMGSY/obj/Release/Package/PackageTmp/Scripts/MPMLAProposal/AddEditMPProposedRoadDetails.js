/* 
     *  Name : AddEditMPProposedRoadDetails.js
     *  Path : ~/Scripts/MPMLAProposal/AddEditMPProposedRoadDetails.js
     *  Description : AddEditMPProposedRoadDetails.js is used to add MLA Proposed Road details
     *  Author : Abhishek Kamlble(PE, e-gov)
     *  Company : C-DAC,E-GOV
     *  Dates of Creation : 05/Jul/2013
 */

$(document).ready(function () {

    $.validator.unobtrusive.parse($('frmCreateMPProposal'));

    //add MP Proposed Road Details
    $('#btnCreate').click(function (evt) {
        evt.preventDefault();

        year_code = $("#IMS_YEAR").val();
        const_code = $("#MAST_MP_CONST_CODE").val();

        if ($('#frmCreateMPProposal').valid()) {
                $.ajax({
                    url: '/MPMLAProposal/AddMPProposedRoadDetails',
                type: "POST",
                cache: false,
                data: $("#frmCreateMPProposal").serialize(),
                beforeSend: function () {
                            blockPage();
            },
                error: function (xhr, status, error) {
                    unblockPage();
                    Alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
            },

                success: function (response) {
                    if (response.success == undefined)
                    {
                        $("#divMPProposedRoadForm").html(response);
                    }
                    else if (response.success) {
                        alert(response.message);
                        $("#btnReset").trigger("click");                        
                        CloseMpProposedRoadDetails();

                        $('#tbMPProposedRoadList').jqGrid("setGridParam", { "postData": { Year: year_code, Constituency: const_code } });
                        $('#tbMPProposedRoadList').trigger("reloadGrid", [{ page: 1 }]);

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

    //Update MP Proposed Road Details
    $('#btnUpdate').click(function (evt) {
        evt.preventDefault();

        year_code = $("#IMS_YEAR").val();
        const_code = $("#MAST_MP_CONST_CODE").val();

        if ($('#frmCreateMPProposal').valid()) {
            $.ajax({
                url: '/MPMLAProposal/EditMPProposedRoadDetails',
                type: "POST",
                cache: false,
                data: $("#frmCreateMPProposal").serialize(),
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
                        $("#divMPProposedRoadForm").html(response);
                    }else if (response.success) {
                        alert(response.message);
                        $("#btnReset").trigger("click");                        
                        $('#tbMPProposedRoadList').jqGrid("setGridParam", { "postData": { Year: year_code, Constituency: const_code } });
                        $('#tbMPProposedRoadList').trigger("reloadGrid", [{ page: 1 }]);
                        CloseMpProposedRoadDetails();
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
                    "<a href='#' style= 'font-size:.9em;' >Add MP Proposed Road Details</a>" +
                    '<a href="#" style="float: right;">' +
                    '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseMpProposedRoadDetails();" /></a>'
                    );

        $('#accordion').show('fold', function () {
            blockPage();
            $("#divMPProposedRoadForm").load('/MPMLAProposal/AddEditMPProposedRoadDetails/', function () {
                $.validator.unobtrusive.parse($('#divMPProposedRoadForm'));
                unblockPage();
            });

            $('#divMPProposedRoadForm').show('slow');
            $("#divMPProposedRoadForm").css('height', 'auto');
        });

        $("#tbMPProposedRoadList").jqGrid('setGridState', 'hidden');

    });
});

//function ShowMPProposedRoads()
//{
//    $('#accordion').hide('slow');
//    $('#divExistingRoadsForm').hide('slow');
//    $("#tbExistingRoadsList").jqGrid('setGridState', 'visible');
//    showFilter();
//}
