$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    LoadMapPanchayatHabitation();

    LoadVillages();
    LoadHabitation();
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

    $('#btnMapHabitation').click(function (e) {

        var habCodes = $("#tbMapPanchayatHabitationList").jqGrid('getGridParam', 'selarrrow');

        if (habCodes != '') {
            $('#EncryptedHabCodes').val(habCodes);
          
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/LocationMasterDataEntry/MapPanchayatHabitations",
                type: "POST",
                dataType: "json",
                data: $("#frmSearchMapPanchayatHabitation").serialize(),
                success: function (data) {
                    
                    alert(data.message);
                    //$("#tbMapPanchayatHabitationList").jqGrid('resetSelection');
                    $("#tbMapPanchayatHabitationList").trigger('reloadGrid');
                    $("#txtSearchVillage").val('');
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });
        }
        else {

            alert('Please select Habitations to map with Panchayat.');
        }


    });


    $('#btnMapCancel').click(function (e) {

        if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
            $('#dvMapPanchayatHabitationsDetails').hide('slow');
        }

        $('#btnSearchView').trigger('click');
        $('#tbPanchyatList').jqGrid("setGridState", "visible");
        $('#trAddNewSearch').show();
        $("#mainDiv").animate({
            scrollTop: 0
        });
        /*  $('#dvSearchPanchayat').show();*/
    });



});

function LoadMapPanchayatHabitation() {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#tbMapPanchayatHabitationList").jqGrid({
        url: '/LocationMasterDataEntry/GetHabitationDetailsList_Mapping',
        datatype: "local",//"json",
        mtype: "POST",
        colNames: ['Habitation Name', 'Village Name', 'Block Name', 'District Name', 'State Name', 'MP Contituency', 'MLA Contituency', 'Is Schedule5', 'Action'],
        colModel: [
                             { name: 'HabitationName', index: 'HabitationName', height: 'auto', width: 220, align: "left", sortable: true },
                             { name: 'VillageName', index: 'VillageName', height: 'auto', width: 200, align: "left", sortable: true },                       
                             { name: 'BlockName', index: 'BlockName', height: 'auto', width: 220, align: "left", sortable: false, hidden: true },                       
                             { name: 'DistrictName', index: 'DistrictName', height: 'auto', width: 170, sortable: false, align: "left", hidden: true },
                             { name: 'StateName', index: 'DistrictName', height: 'auto', width: 170, sortable: false, align: "left", hidden: true },
                             { name: 'MPContituency', index: 'StateName', height: 'auto', width: 150, sortable: true, align: "left" },
                             { name: 'MLAContituency', index: 'MLAContituency', height: 'auto', width: 150, sortable: true, align: "left" },
                             { name: 'IsSchedule5', index: 'IsSchedule5', height: 'auto', width: 100, sortable: true, align: "left" },
                            { name: 'a', width: 80, sortable: false, resize: false, align: "center", hidden: true } /* formatter: FormatColumn*/
        ],
        pager: jQuery('#dvMapPanchayatHabitationListPager'), 
        pginput: false,
        pgbuttons: false,
        rowNum: 0,
       // altRows: true,
        //rowList: [5, 10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'VillageName,HabitationName',
        sortorder: "asc",
        caption: "Habitation List",
        height: 'auto',
        //width: 'auto',
        autowidth:true,
        rownumbers: true,
        hidegrid: false,
        multiselect:true,
        loadComplete: function () {
           
            var recordCount = jQuery('#tbMapPanchayatHabitationList').jqGrid('getGridParam', 'reccount');
          
            //alert(recordCount);
            if (recordCount > 15) {

                $('#tbMapPanchayatHabitationList').jqGrid('setGridHeight', '320');
                //$('#tbMapPanchayatHabitationList').jqGrid('setGridWidth', '540');

            }
            else {
                $('#tbMapPanchayatHabitationList').jqGrid('setGridHeight', 'auto');
               // $('#tbMapPanchayatHabitationList').jqGrid('setGridWidth', '530');
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

    $('#tbMapPanchayatHabitationList').setGridParam({
        url: '/LocationMasterDataEntry/GetHabitationDetailsList_Mapping', datatype: 'json'
    });

    var data = $('#tbMapPanchayatHabitationList').jqGrid("getGridParam", "postData");
    data._search = true;
    
    $('#tbMapPanchayatHabitationList').jqGrid("setGridParam", { "postData": { BlockCode: $('#EncryptedBlockCode').val(), VillageName: $('#txtSearchVillage').val(), HabitationName: $('#txtSearchHabitation').val() } });

    $('#tbMapPanchayatHabitationList').trigger("reloadGrid", [{ page: 1 }]);

   

}


function LoadVillages() {

    $("#txtSearchVillage").val('');

    $.ajax({
        url: "/LocationMasterDataEntry/GetVillagesForHabitationMapping/" + $('#EncryptedBlockCode').val(),
        cache: false,
        type: "POST",
        async: false,
        success: function (data) {
            
            var rows = new Array();
            for (var i = 0; i < data.length; i++) {
                
                rows[i] = { data: data[i].Text, value: data[i].Text, id: data[i].Value };
            }
            
            $('#txtSearchVillage').autocomplete({
                source: rows,
                dataType: 'json',
                formatItem: function (row, i, n) {
                    return row.Text;
                },
                width: 150,
                highlight: true,
                minChars: 3,
                selectFirst: true,
                max: 10,
                scroll: true,
                width: 100,
                maxItemsToShow: 10,
                maxCacheLength: 10,
                mustMatch: true
            })

        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert("An error occurred while executing this request.\n" + xhr.responseText);
            if (xhr.responseText == "session expired") {
                //$('#frmECApplication').submit();
                alert(xhr.responseText);
                window.location.href = "/Login/LogIn";
            }
        }
    })


}

function LoadHabitation() {

    $("#txtSearchHabitation").val('');

    $.ajax({
        url: "/LocationMasterDataEntry/GetHabitationNameByBlockCodeList/" + $('#EncryptedBlockCode').val(),
        cache: false,
        type: "POST",
        async: false,
        success: function (data) {

            var rows = new Array();
            for (var i = 0; i < data.length; i++) {

                rows[i] = { data: data[i].Text, value: data[i].Text, id: data[i].Value };
            }

            $('#txtSearchHabitation').autocomplete({
                source: rows,
                dataType: 'json',
                formatItem: function (row, i, n) {
                    return row.Text;
                },
                width: 150,
                highlight: true,
                minChars: 3,
                selectFirst: true,
                max: 10,
                scroll: true,
                width: 100,
                maxItemsToShow: 10,
                maxCacheLength: 10,
                mustMatch: true
            })

        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert("An error occurred while executing this request.\n" + xhr.responseText);
            if (xhr.responseText == "session expired") {
                //$('#frmECApplication').submit();
                alert(xhr.responseText);
                window.location.href = "/Login/LogIn";
            }
        }
    })


}