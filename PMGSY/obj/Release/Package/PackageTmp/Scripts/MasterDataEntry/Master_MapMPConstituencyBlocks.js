$(document).ready(function () {

    LoadMapMPConstituencyBlock();

    //for expand and collpase Document Details 
    $("#dvhdSearch_Map").click(function () {

        if ($("#dvSearchParameter_Map").is(":visible")) {

            $("#spCollapseIconS_Map").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            //$("#dvDocumentDetails").css('margin-bottom','10px');

            $(this).next("#dvSearchParameter_Map").slideToggle(300);
        }

        else {
            $("#spCollapseIconS_Map").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvSearchParameter_Map").slideToggle(300);
        }
    });


    $('#btnSearch_Map').click(function (e) {

        SearchDetails_Mapping();

    });

    $('#btnMapConstituency').click(function (e) {

        var blockCodes = $("#tbMapMPConstituencyBlockList").jqGrid('getGridParam', 'selarrrow');

        if (blockCodes != '') {
            $('#EncryptedBlockCodes').val(blockCodes);
            //        alert($('#EncryptedBlockCodes').val());


            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/LocationMasterDataEntry/MapMPConstituencyBlocks",
                type: "POST",
                dataType: "json",
                data: $("#frmSearchMapMPConstituencyBlock").serialize(),
                success: function (data) {

                    alert(data.message);
                    //$("#tbMapMPConstituencyBlockList").jqGrid('resetSelection');
                    $("#tbMapMPConstituencyBlockList").trigger('reloadGrid');
                    $('#ddlSearchDistrict').val('0');
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });
        }
        else {

            alert('Please select Blocks to map with MP Constituency.');
        }


    });

    $('#btnMapCancel').click(function (e) {

        if ($("#dvMapMPConstituencyBlockDetails").is(":visible")) {
            $('#dvMapMPConstituencyBlockDetails').hide('slow');
        }

        //  $('#btnSearchView').trigger('click');
        $("#dvSearchMPConstituency").show('slow');
        $('#tbMPConstituencyList').jqGrid("setGridState", "visible");
        $('#trAddNewSearch').show();
        $("#mainDiv").animate({
            scrollTop: 0
        });
        /* $('#dvSearchMPConstituency').show();*/
    });



});

function LoadMapMPConstituencyBlock() {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#tbMapMPConstituencyBlockList").jqGrid({
        url: '/LocationMasterDataEntry/GetBlockDetailsList_Mapping_MP',
        datatype: "local",//"json",
        mtype: "POST",
        colNames: ['Block Id','Block Name','District Name', 'State name',  'Is Desert', 'Is Tribal', 'Is Included In PMGSY', 'Is Schedule5', 'Action'],
        colModel: [
                            { name: 'MastBlockId', index: 'MastBlockId', height: 'auto', width: 90, align: "center", sortable: true, hidden: true },
                            { name: 'BlockName', index: 'BlockName', height: 'auto', width: 200, align: "left", sortable: true },
                            { name: 'DistrictName', index: 'DistrictName', height: 'auto', width: 190, sortable: true, align: "left" },
                            { name: 'StateName', index: 'StateName', height: 'auto', width: 170, sortable: false, align: "left", hidden: true },
                            { name: 'IsDESERT', index: 'IsDESERT', width: 90, sortable: true },
                            { name: 'IsTRIBAL', index: 'IsTRIBAL', width: 90, sortable: true },
                            { name: 'IsPMGSYIncluded', index: 'IsPMGSYIncluded', width: 130, sortable: true },
                            { name: 'IsSchedule5', index: 'IsSchedule5', width: 110, sortable: true },
                            { name: 'a', width: 80, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false, hidden: true }
        ],
        pager: jQuery('#dvMapMPConstituencyBlockListPager'),
        pginput: false,
        pgbuttons: false,
        rowNum: 0,
      //  altRows: true,
        // rowList: [5, 10, 20, 30],7
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Block List",
        height: 'auto',
        sortname: 'DistrictName,BlockName',
        sortorder: "asc",
        // width: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        multiselect: true,
        loadComplete: function () {

            var recordCount = jQuery('#tbMapMPConstituencyBlockList').jqGrid('getGridParam', 'reccount');

            if (recordCount > 14) {

                $('#tbMapMPConstituencyBlockList').jqGrid('setGridHeight', '320');
               // $('#tbMapMPConstituencyBlockList').jqGrid('setGridWidth', '840');

            }
            else {
                $('#tbMapMPConstituencyBlockList').jqGrid('setGridHeight', 'auto');
                //$('#tbMapMPConstituencyBlockList').jqGrid('setGridWidth', '830');
            }

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

function SearchDetails_Mapping() {

    $('#tbMapMPConstituencyBlockList').setGridParam({
        url: '/LocationMasterDataEntry/GetBlockDetailsList_Mapping_MP', datatype: 'json'
    });
    var data = $('#tbMapMPConstituencyBlockList').jqGrid("getGridParam", "postData");
    data._search = true;
    //  data.searchField = $("#frmSearchMapMLAConstituencyBlock").serialize();
    //$('#tbMapMPConstituencyBlockList').jqGrid("setGridParam", { "postData": data });

    $('#tbMapMPConstituencyBlockList').jqGrid("setGridParam", { "postData": { MLAStateCode: $('#EncryptedStateCode').val(), DistrictCode: $('#ddlSearchDistrict').val(), MPConstituencyCode: $('#MPConstituencyCode').val() } });

    $('#tbMapMPConstituencyBlockList').trigger("reloadGrid", [{ page: 1 }]);

}