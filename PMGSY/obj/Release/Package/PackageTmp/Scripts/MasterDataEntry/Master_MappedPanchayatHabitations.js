$(document).ready(function () {

    LoadMappedPanchayatHabitation();


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

      
        if ($("#dvMappedPanchayatHabitationDetails").is(":visible")) {
            $('#dvMappedPanchayatHabitationDetails').hide('slow');
        }
        $('#btnSearchView').trigger('click');
        $('#tbPanchyatList').jqGrid("setGridState", "visible");
        $('#trAddNewSearch').show();
        $("#mainDiv").animate({
            scrollTop: 0
        });
        /*$('#dvSearchMLAConstituency').show();*/
    });




});

function LoadMappedPanchayatHabitation() {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#tbMappedPanchayatHabitationList").jqGrid({
        url: '/LocationMasterDataEntry/GetHabitationDetailsList_Mapped_Panchayat',
        datatype: "json",
        mtype: "POST",
        postData: { PanchayatCode: $('#EncryptedPanchayatCode_Mapped').val() },
        colNames: ['Habitation Name',  'Village Name', 'MP Contituency', 'Delete'],
        colModel: [
                             { name: 'HabitationName', index: 'HabitationName', height: 'auto', width: 200, align: "left", sortable: true },                                                      
                             { name: 'VillageName', index: 'VillageName', height: 'auto', width: 200, align: "left", sortable: true },
                             { name: 'MPContituency', index: 'MPContituency', height: 'auto', width: 150, sortable: false, align: "left", hidden: true },
                             //{ name: 'MLAContituency', index: 'MLAContituency', height: 'auto', width: 150, sortable: false, align: "left" },
                             //{ name: 'IsSchedule5', index: 'IsSchedule5', height: 'auto', width: 100, sortable: true, align: "left" },
                             { name: 'Delete', index: 'Delete', height: 'auto', width: 50, sortable: true, align: "center" },
                           
        ],
        pager: jQuery('#dvMappedPanchayatHabitationListPager'),
        rowNum: 15,
        //altRows: true,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'VillageName,HabitationName',
        sortorder: "asc",
        caption: "Habitation List",
        height: 'auto',
        //width: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false, 
        loadComplete: function () {
        
            $.unblockUI();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                $.unblockUI();
                window.location.href = "/Login/Login";
                
            }
            else {
                // alert(xhr.responseText);
                alert("Invalid data.Please check and Try again!")
                $.unblockUI();
                //  window.location.href = "/Login/LogIn";
            }
        }

    }); //end of grid
}
function DeleteMappedHabitation(urlparameter)
{
    if (confirm("Are you sure you want to delete mapped habitation?")) {
        $.ajax({
            type: 'POST',
            url: '/LocationMasterDataEntry/DeleteMappedHabitation/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert("Mapped habitation deleted successfully");
                    $("#tbMappedPanchayatHabitationList").trigger('reloadGrid');
                }
                else if (data.success == false) {
                    alert("Mapped habitation details is in use and can not be deleted.");
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