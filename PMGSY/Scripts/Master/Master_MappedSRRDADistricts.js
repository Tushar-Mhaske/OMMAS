$(document).ready(function () {
    LoadMappedSRRDADistricts();
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

    if ($("#dvMappedAgencyDistrictsDetails").is(":visible")) {
            $('#dvMappedAgencyDistrictsDetails').hide('slow');
    }
    $('#State').val(stateCode);
    SearchMappedSrrdaDetail(stateCode);
    $('#btnSearch').hide();
    $('#btnAdd').show();
    $('#adminSearchDetails').show('show');
        $('#btnSearch').trigger('click');
        $('#adminCategory').jqGrid("setGridState", "visible");
        $('#trAddNewSearch').show();
        $("#mainDiv").animate({
            scrollTop: 0
        });
       
    });
});
function LoadMappedSRRDADistricts() {

    jQuery("#tbMappedSRRDADistrictList").jqGrid({
        url: '/Master/GetDistrictDetailsList_Mapped_SRRDA',
        datatype: "json",
        mtype: "POST",
        postData: { AdminCode: $('#EncryptedAdminCode_Mapped').val() },
        colNames: ['District Name', 'Is PMGSY Included', 'Is IAP District','Delete'],
        colModel: [
                            { name: 'DistrictName', index: 'DistrictName', height: 'auto', width: 350, align: "left", sortable: true },
                            { name: 'IsPMGSYIncluded', index: 'IsPMGSYIncluded', width: 200, sortable: true },
                            { name: 'IsIAPDistrict', index: 'IsIAPDistrict', width: 200, sortable: true },
                            { name: 'Delete', index: 'Delete', width: 50, sortable: true },

        ],
        pager: jQuery('#dvMappedSRRDADistrictListPager'),
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
        cmTemplate: { title: false },
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
function DeleteMappedDistrict(urlparameter) {
    if (confirm("Are you sure you want to delete mapped state?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteMappedSRRDADistrict/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert("Mapped district deleted successfully");
                    $("#tbMappedSRRDADistrictList").trigger('reloadGrid');
                }
                else if (data.success == false) {
                    alert("Mapped district details is in use and can not be deleted.");
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
function SearchMappedSrrdaDetail(stateCode) {

    $('#adminCategory').setGridParam({
        url: '/Master/GetDepartmentList'
    });


    $('#adminCategory').jqGrid("setGridParam", { "postData": { stateCode: $('#State option:selected').val(), agency: $('#Agency option:selected').val() } });
    $('#adminCategory').trigger("reloadGrid", [{ page: 1 }]);
}