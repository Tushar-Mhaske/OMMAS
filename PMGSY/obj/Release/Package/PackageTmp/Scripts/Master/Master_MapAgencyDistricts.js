$(document).ready(function () {

  
    LoadMapAgencyDistrict();

 
    var startDateCheck = $('#strtDate').val();
    if (startDateCheck != '') {
        $("#message").hide();
    }
    else {
        $("#message").show();
    }
    $('#strtDate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "../../Content/Images/calendar_2.png",
        showButtonText: 'Choose a start date',
        buttonImageOnly: true,
        buttonText: 'Start Date',
        changeMonth: true,
        changeYear: true,
        maxDate: new Date(),
        onSelect: function (selectedDate) {
            $("#message").hide();
        }
    });

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


    $('#btnMapDistrict').click(function (e) {

        var districtCodes = $("#tbMapAgencyDistrictList").jqGrid('getGridParam', 'selarrrow');
        var startDateCheck = $('#strtDate').val();
        if (startDateCheck != '') {
            $("#message").hide();
        }
        else {
            $("#message").show();
            return false;
        }
        if (districtCodes != '') {
            $('#EncryptedDistrictCodes').val(districtCodes);
              $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/Master/MapAgencyDistricts",
                type: "POST",
                dataType: "json",
                data: $("#frmSearchMapAgencyDistrict, #frmStartDate").serialize(),
                success: function (data) {

                    
                    //document.getElementById('startDate').style.visibility = 'hidden';
                    $("#startDate").hide();
                    $("#message").hide();
                    alert(data.message);
               
                    $("#tbMapAgencyDistrictList").trigger('reloadGrid');
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });
        }
        else {

            alert('Please select Districts to map with Technical Agency.');
        }


    });

    $('#btnMapDistrictCancel').click(function (e) {

        if ($("#dvMapAgencyDistrictDetails").is(":visible")) {
            $('#dvMapAgencyDistrictDetails').hide('slow');
        }
        $('#btnSearchView').trigger('click');
        $('#tblList').jqGrid("setGridState", "visible");
        $('#trAddNewSearch').show();
        $("#mainDiv").animate({
            scrollTop: 0
        });

    });

    
    if ($('#stateCode').val() > 0) {

        $("#ddlSearchState").val($('#stateCode').val());
        $("#ddlSearchState").attr("disabled", true);
        $("#ddlSearchState").trigger('change');
    }
    

    $('#btnSearch_Map').click(function (e) {

        SearchDetails_Mapping();

    });

    
    setTimeout(function () {
        $('#btnSearch_Map').trigger('click');
    }, 500);

});

function SearchDetails_Mapping() {

    var isDisabled = false;

    $('#tbMapAgencyDistrictList').setGridParam({
        url: '/Master/GetDistrictDetailsList_Mapping_Agency', datatype: 'json'
    });
    
    $('#tbMapAgencyDistrictList').jqGrid("setGridParam", { "postData": { StateCode: $('#ddlSearchState').val() } });

    $('#tbMapAgencyDistrictList').trigger("reloadGrid", [{ page: 1 }]);

}

function LoadMapAgencyDistrict() {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#tbMapAgencyDistrictList").jqGrid({
        url: '/Master/GetDistrictDetailsList_Mapping_Agency',
        datatype: "json",
        mtype: "POST",
        postData: { StateCode: $('#ddlSearchState option:selected').val(), AgencyCode: $('#EncryptedAgencyCode').val() },
        colNames: ['District Id','District Name', 'State Name', 'Is Included In PMGSY', 'Is IAP District', 'Shift District', 'Action'],
        colModel: [
                            { name: 'DistrictId', index: 'DistrictId', height: 'auto', width: 50, align: "left", sortable: true, hidden: true },
                            { name: 'DistrictName', index: 'DistrictName', height: 'auto', width: 270, align: "left", sortable: true },
                            { name: 'StateName', index: 'StateName', height: 'auto', width: 200, sortable: true, align: "left", hidden: false },
                            { name: 'IsPMGSYIncluded', index: 'IsPMGSYIncluded', width: 180, sortable: true },
                            { name: 'IsIAPDistrict', index: 'IsIAPDistrict', width: 180, sortable: true },
                            { name: 'Shift', sortable: false, resize: false, align: "center", hidden: true },
                            { name: 'a',  sortable: false, resize: false, align: "center", sortable: false, hidden: true }
        ],
        pager: jQuery('#dvMapAgencyDistrictListPager'),
        pginput: false,
        pgbuttons: false,
        rowNum: 0,
        
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'StateName,DistrictName',
        sortorder: "asc",
        caption: "District List",
        height: 'auto',
     
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        multiselect: true,
        onSelectRow: function (id) {
            //alert("hi");
            var stateCodesCheck = $("#tbMapAgencyDistrictList").jqGrid('getGridParam', 'selarrrow');
            var startDate = document.getElementById('startDate');
            startDate.placeholder = "dd/mm/yyyy";

            if (stateCodesCheck != '') {
                //startDate.style.visibility = 'visible';
                $("#startDate").show();
            }
            else {
                //startDate.style.visibility = 'hidden';
                $("#startDate").hide();
            }
            var startDateCheck = $('#strtDate').val();
            if (startDateCheck != '') {
                $("#message").hide();
            }
            else {
                $("#message").show();
            }
        },
        onSelectAll: function (aRowids, status) {
            var startDate = document.getElementById('startDate');
            if (status) {
                $("#startDate").show();

            }
            else {
                $("#startDate").hide();
            }
            var startDateCheck = $('#strtDate').val();
            if (startDateCheck != '') {
                $("#message").hide();
            }
            else {
                $("#message").show();
            }
        },
        loadComplete: function () {

            var recordCount = jQuery('#tbMapAgencyDistrictList').jqGrid('getGridParam', 'reccount');

        
            if (recordCount > 15) {

                $('#tbMapAgencyDistrictList').jqGrid('setGridHeight', '320');

            }
            else {
                $('#tbMapAgencyDistrictList').jqGrid('setGridHeight', 'auto');
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

    }); 

}