$(document).ready(function () {

    

    //for expand and collpase Document Details 
    $("#dvhdSearch_Map_District").click(function () {
        if ($("#dvSearchParameter_Map_District").is(":visible")) {
            $("#spCollapseIconS_Map").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $(this).next("#dvSearchParameter_Map_District").slideToggle(300);
        }
        else {
            $("#spCollapseIconS_Map").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvSearchParameter_Map_District").slideToggle(300);
        }
    });

    LoadMapSRRDADistrict();

    $('#btnMapDistrict').click(function (e) {

        var districtCodes = $("#tbMapSRRDADistrictList").jqGrid('getGridParam', 'selarrrow');
        if (districtCodes != '') {
            $('#EncryptedDistrictCodes').val(districtCodes);
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/MapSRRDADistricts",
                type: "POST",
                dataType: "json",
                data: $("#frmSearchMapSRRDADistrict").serialize(),
                success: function (data) {

                    alert(data.message);
                    // $("#tbMapRegionDistrictList").jqGrid('resetSelection');
                    $("#tbMapSRRDADistrictList").trigger('reloadGrid');
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });
        }
        else {

            alert('Please select Districts to map with SRRDA.');
        }
    });
    //alert($('#EncryptedMapAdminCode').val());
    $('#btnMapDistrictCancel').click(function (e) {

        if ($("#dvMapAgencyDistrictsDetails").is(":visible")) {
            $('#dvMapAgencyDistrictsDetails').hide('slow');
        }
       // alert(stateCode);

        $('#trAddNewSearch').show();

        $('#State').val(stateCode);
        SearchMapSrrdaDetail();
        $('#adminSearchDetails').show('show');
        
       // $('#btnSearch').trigger('click');
        $('#adminCategory').jqGrid("setGridState", "visible");
        $('#btnSearch').hide();
        $('#btnAdd').show();
        
        $("#mainDiv").animate({
            scrollTop: 0
        });
    });

 
    

});//end document.ready


function LoadMapSRRDADistrict() {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    
    jQuery("#tbMapSRRDADistrictList").jqGrid({
        url: '/Master/GetDistrictDetailsList_Mapping_SRRDA',
        datatype: "json",
        mtype: "POST",
        postData: { AdminCode: $('#EncryptedMapAdminCode').val() },
        //postData: { AdminCode: encryptedCode },
        colNames: ['District Id','District Name', 'State Name', 'Is Included In PMGSY', 'Is IAP District', 'Shift District', 'Action'],
        colModel: [
                            { name: 'DistrictId', index: 'DistrictId', height: 'auto', width: 50, align: "left", sortable: true, hidden: true },
                            { name: 'DistrictName', index: 'DistrictName', height: 'auto', width: 430, align: "left", sortable: true },
                            { name: 'StateName', index: 'StateName', height: 'auto', width: 200, sortable: true, align: "left", hidden: false,hidden:true },
                            { name: 'IsPMGSYIncluded', index: 'IsPMGSYIncluded', width: 200, sortable: true },
                            { name: 'IsIAPDistrict', index: 'IsIAPDistrict', width: 200, sortable: true },
                            { name: 'Shift', /*width: 140,*/ sortable: false, resize: false, align: "center", hidden: true },
                            { name: 'a', /*width: 80,*/ sortable: false, resize: false, align: "center", sortable: false, hidden: true }
        ],
        pager: jQuery('#dvMapSRRDADistrictListPager'),
        pginput: false,
        pgbuttons: false,
        rowNum: 0,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'DistrictName',
        sortorder: "asc",
        caption: "District List",
        height: 'auto',
        // width: '100%',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        multiselect: true,
        loadComplete: function () {
            var recordCount = jQuery('#tbMapSRRDADistrictList').jqGrid('getGridParam', 'reccount');
            if (recordCount > 15) {

                $('#tbMapSRRDADistrictList').jqGrid('setGridHeight', '320');
            }
            else {
                $('#tbMapSRRDADistrictList').jqGrid('setGridHeight', 'auto');
            }
            $.unblockUI();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                     alert("Invalid data.Please check and Try again!")
              
            }
        }

    }); //end of  grid

}
function SearchMapSrrdaDetail(stateCode) {

    $('#adminCategory').setGridParam({
        url: '/Master/GetDepartmentList'
    });


    $('#adminCategory').jqGrid("setGridParam", { "postData": { stateCode: $('#State option:selected').val(), agency: $('#Agency option:selected').val() } });
    $('#adminCategory').trigger("reloadGrid", [{ page: 1 }]);
}