﻿$(document).ready(function () {

    LoadMappedMLAConstituencyBlock();
    

    $("#dvhdSearch_Mapped").click(function () {

        if ($("#dvSearchParameter_Mapped").is(":visible")) {

            $("#spCollapseIconS_Mapped").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            //$("#dvDocumentDetails").css('margin-bottom','10px');

            $(this).next("#dvSearchParameter_Mapped").slideToggle(300);
        }

        else {
            $("#spCollapseIconS_Mapped").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvSearchParameter_Mapped").slideToggle(300);
        }
    });



    $('#btnMappedCancel').click(function (e) {

        //if ($("#dvMapMLAConstituencyBlockDetails").is(":visible")) {
        //    $('#dvMapMLAConstituencyBlockDetails').hide('slow');
        //}

        if ($("#dvMappedMLAConstituencyBlockDetails").is(":visible")) {
            $('#dvMappedMLAConstituencyBlockDetails').hide('slow');
        }
        // $('#btnSearchView').trigger('click');
        $("#dvSearchMLAConstituency").show('slow');
        $('#tbMLAConstituencyList').jqGrid("setGridState", "visible");
        $('#trAddNewSearch').show();
        $("#mainDiv").animate({
            scrollTop: 0
        });
        /*$('#dvSearchMLAConstituency').show();*/
    });




});

function LoadMappedMLAConstituencyBlock() {

    jQuery("#tbMappedMLAConstituencyBlockList").jqGrid({
        url: '/LocationMasterDataEntry/GetBlockDetailsList_Mapped_MLA',
        datatype: "json",
        mtype: "POST",
        postData: { MLAConstituencyCode: $('#EncryptedMLAConstituencyCode_Mapped').val() },
        colNames: ['Block Name',  'District Name', 'Is DESERT', 'Is TRIBAL', 'Is PMGSY Included', 'Is Schedule5','Delete'],
        colModel: [
                            { name: 'BlockName', index: 'BlockName', height: 'auto', width: 200, align: "left", sortable: true },                      
                            { name: 'DistrictName', index: 'DistrictName', height: 'auto', width: 190, sortable: true, align: "left" },
                            { name: 'IsDESERT', index: 'IsDESERT', width: 90, sortable: true },
                            { name: 'IsTRIBAL', index: 'IsTRIBAL', width: 90, sortable: true },
                            { name: 'IsPMGSYIncluded', index: 'IsPMGSYIncluded', width: 130, sortable: true },
                             { name: 'IsSchedule5', index: 'IsSchedule5', width: 110, sortable: true },
                             { name: 'Delete', index: 'Delete', width: 50, sortable: true }
                           
        ],
        pager: jQuery('#dvMappedMLAConstituencyBlockListPager'),
        rowNum: 15,
        //altRows: true,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Block List",
        height: 'auto',
        //width: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        sortname: 'DistrictName,BlockName',
        sortorder: "asc",
        loadComplete: function () {
            $.unblockUI();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                // alert(xhr.responseText);
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }

    }); //end of grid
}
function DeleteMappedBlock(urlparameter) {
    if (confirm("Are you sure you want to delete mapped state?")) {
        $.ajax({
            type: 'POST',
            url: '/LocationMasterDataEntry/DeleteMappedMLABlock/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert("Mapped block deleted successfully");
                    $("#tbMappedMLAConstituencyBlockList").trigger('reloadGrid');
                }
                else if (data.success == false) {
                    //alert("Mapped block details is in use and can not be deleted.");
                    alert(data.message);
                }
                else {

                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.responseText);
            }
        });
    }
    else {
        return false;
    }
}
