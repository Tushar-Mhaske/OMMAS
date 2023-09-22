$(document).ready(function () {

    LoadMappedRegionDistricts();


    $("#dvhdSearch_Mapped").click(function () {

        if ($("#dvSearchParameter_Mapped").is(":visible")) {

            $("#spCollapseIconS_Mapped").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

           $(this).next("#dvSearchParameter_Mapped").slideToggle(300);
        }

        else {
            $("#spCollapseIconS_Mapped").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvSearchParameter_Mapped").slideToggle(300);
        }
    });

 $('#btnMappedCancel').click(function (e) {

       
        if ($("#dvMappedRegionDistrictsDetails").is(":visible")) {
            $('#dvMappedRegionDistrictsDetails').hide('slow');
        }
        $('#btnSearchView').trigger('click');
        $('#tblRegionList').jqGrid("setGridState", "visible");
        $('#trAddNewSearch').show();
        $("#mainDiv").animate({
            scrollTop: 0
        });
      
    });
});

function LoadMappedRegionDistricts() {

    jQuery("#tbMappedRegionDistrictList").jqGrid({
        url: '/Master/GetDistrictDetailsList_Mapped',
        datatype: "json",
        mtype: "POST",
        postData: { RegionCode: $('#EncryptedRegionCode_Mapped').val() },
        colNames: ['District Name',  'Is PMGSY Included', 'Is IAP District','Delete'],
        colModel: [
                            { name: 'DistrictName', index: 'DistrictName', height: 'auto', width: 430, align: "left", sortable: true }, 
                            { name: 'IsPMGSYIncluded', index: 'IsPMGSYIncluded', width: 200, sortable: true },
                            { name: 'IsIAPDistrict', index: 'IsIAPDistrict', width: 200, sortable: true },
                            { name: 'Delete', index: 'Delete', width: 50, sortable: false, align: "left" }

                            
        ],
        pager: jQuery('#dvMappedRegionDistrictListPager'),
        rowNum: 15,
    
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'DistrictName',
        sortorder: "asc",
        caption: "District List",
        height: 'auto',
     
        autowidth: true,
        rownumbers: true,
        hidegrid: false, 
        loadComplete: function () {
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

    }); 
}

//Added By Abhishek kamble 24-Feb-2014
function DeleteMappedDistrictRegion(param)
{   
        if (confirm("Are you sure you want to delete mapped district details?")) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/Master/DeleteMappedDistrictRegionDetails/" + param,
                type: "POST",
                dataType: "json",
                success: function (data) {

                    if (data.success) {
                        alert(data.message);

                        //if ($("#dvSearchMpMembers").is(":visible")) {
                        //    $('#btnSearch').trigger('click');
                        //}
                        //else {
                        //    $('#tbMappedRegionDistrictList').trigger('reloadGrid');
                        //}
                        //$("#dvMpMemberDetails").load("/Master/AddEditMasterMpMember/");
                        $('#tbMappedRegionDistrictList').trigger('reloadGrid');

                    }
                    else {
                        alert(data.message);
                    }
                    $.unblockUI();
                },
                error: function (xht, ajaxOptions, throwError)
                { alert(xht.responseText); }

            });
        }
        else {
            return false;
        }
}