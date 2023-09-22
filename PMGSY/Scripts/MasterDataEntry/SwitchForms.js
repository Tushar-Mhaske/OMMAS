$(document).ready(function () {
    $("#dvhdSearchFacilityDetails").hide();

    $("#btnSearchNew").click(function ()
    {
        if ($("#dvPanchayatDetails").is(":visible")) {
            $("#dvPanchayatDetails").hide();
        }
        $.ajax({
            url: "/LocationMasterDataEntry/SearchFacilityForm",
            type: "GET",
            dataType: "html",
            success: function (data) {
                $("#dvSearchPanchayat").html(" ");
                $("#dvSearchPanchayat").html(data);
              
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("An Error Occurred");
            }
        });
        $("#dvSearchPanchayat").show();
    });


    $("#btnSearchNewForDefinalization").click(function () {
        if ($("#dvPanchayatDetails").is(":visible")) {
            $("#dvPanchayatDetails").hide();
        }
        $.ajax({
            url: "/LocationMasterDataEntry/SearchFacilityFormForDefinalization",
            type: "GET",
            dataType: "html",
            success: function (data) {
                $("#dvSearchPanchayat").html(" ");
                $("#dvSearchPanchayat").html(data);

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("An Error Occurred");
            }
        });
        $("#dvSearchPanchayat").show();
    });

    $("#btnCreateNew").click(function () {
        if ($("#dvSearchPanchayat").is(":visible")) {
            $("#dvSearchPanchayat").hide();
        }

        $.ajax({
            url: "/LocationMasterDataEntry/AddFacilityLayout",
            type: "GET",
            dataType: "html",
            success: function (data) {
                $("#dvPanchayatDetails").html(data);

                //added by abhinav pathak on 22-08-2019
                $('#tbPanchyatList').jqGrid('GridUnload');
                $('#tbPanchyatList').trigger('reloadGrid');

                //$("#dvPanchyatListPager").hide()
                //$("#gbox_tbPanchyatList").hide()
          },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("An Error Occurred");
            }

        });
    });

});