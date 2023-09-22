
function AddDistrictDetails(evt, scheduleCode, showOption) {

    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(showOption).style.display = "block";
    evt.currentTarget.className += " active"
    $.ajax({
        url: "/TourClaim/ViewDistrictDetails",
        async: false,
        cache: false,
        data: { scheduleCode: scheduleCode },
        success: function (data) {

            $("#btncancelMainForm").show("slow");

            $("#showDistrictDetails").show("slow");
            $("#showDistrictDetails").html(data);

            $("#divDistrictDetailsList").show("slow");
            LoadDistrictDetailsList(scheduleCode);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
            alert(xhr.responseText);
            $.unblockUI();

        }
    });
}

$('#btnAddDistrictDetails').click(function (e) {

    if ($("#formDistrictDetails").valid()) {
        if (confirm("Are you sure you want to save district details ?")) {
            $.ajax({
                url: "/TourClaim/InsertDistrictDetails",
                type: "POST",
                async: false,
                cache: false,
                data: $("#formDistrictDetails").serialize(),
                success: function (response) {
                    alert(response.message);
                    if (response.success) {
                        $('#btnDistrictDetailsReset').trigger('click');
                        LoadDistrictDetailsList($('#scheduleCodeDistrict').val());
                    }
                    else {
                        //$('#btnDistrictDetailsReset').trigger('click');
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                    alert(xhr.responseText);
                    $.unblockUI();

                }
            });
        }
    }
});

function LoadDistrictDetailsList(scheduleCode) {

    jQuery("#tbDistrictDetailsList").jqGrid('GridUnload');
    jQuery("#tbDistrictDetailsList").jqGrid({
        url: '/TourClaim/GetTourDistrictList',
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        postData: { scheduleCode: scheduleCode },
        colNames: ['TourClaimId', 'District', 'Date From', 'Date To', 'Delete'],
        colModel: [
            { name: 'TOUR_CLAIM_ID', index: 'TOUR_CLAIM_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', width: 80, sortable: true, align: "center" },
            { name: 'DATE_FROM', index: 'DATE_FROM', width: 80, sortable: true, align: "center" },
            { name: 'DATE_TO', index: 'DATE_TO', height: 'auto', width: 80, align: "center", search: false },
            { name: 'DELETE', index: 'DELETE', height: 'auto', width: 80, align: "center", search: false },
        ],
        pager: jQuery('#pagerDistrictDetailsList').width(20),
        rowNum: 5,
        rowList: [5, 10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "DATE_FROM",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; District Details",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        onSelectRow: function (id) {
        },
        loadComplete: function (data) {

            $("#tbDistrictDetailsList #pagerDistrictDetailsList").css({ height: '40px' });
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

function DeleteDistrictDetails(id) {

    const myArray = id.split("$");
    var districtId = myArray[0];
    var scheduleCode = myArray[1];

    if (confirm("Are you sure to delete details ?")) {
        $.ajax({
            url: "/TourClaim/DeleteDistrictDetail/",
            type: "POST",
            async: false,
            cache: false,
            data: { districtId: districtId },
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    LoadDistrictDetailsList(scheduleCode);
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    else {
        return false;
    }
}

function EditDistrictDetails(id) {

    const myArray = id.split("$");
    var districtId = myArray[0];

    if (confirm("Are you sure to edit details ?")) {
        $.ajax({
            url: "/TourClaim/EditDistrictDetail/",
            async: false,
            cache: false,
            data: { districtId: districtId },
            success: function (data) {
                $("#showDistrictDetails").html(data);
                //$("#divDistrictDetailsList").hide("slow");
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    else {
        return false;
    }
}

$('#btnEditDistrictDetails').click(function (e) {

    var scheduleCode = $('#scheduleCodeDistrict').val();

    if ($("#formDistrictDetails").valid()) {
        if (confirm("Are you sure you want to update district details changes ?")) {
            $.ajax({
                url: "/TourClaim/UpdateDistrictDetails",
                type: "POST",
                async: false,
                cache: false,
                data: $("#formDistrictDetails").serialize(),
                success: function (response) {
                    alert(response.message);
                    if (response.success) {
                        $.ajax({
                            url: "/TourClaim/ViewDistrictDetails",
                            async: false,
                            cache: false,
                            data: { scheduleCode: scheduleCode },
                            success: function (data) {

                                $("#btncancelMainForm").show("slow");

                                $("#showDistrictDetails").show("slow");
                                $("#showDistrictDetails").html(data);

                                $("#divDistrictDetailsList").show("slow");
                                LoadDistrictDetailsList(scheduleCode);
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                alert("error");
                                alert(xhr.responseText);
                                $.unblockUI();

                            }
                        });
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                    alert(xhr.responseText);
                    $.unblockUI();

                }
            });
        }
    }
});

$('#btnBackDistrictDetails').click(function (e) {
    var scheduleCode = $("#scheduleCodeDistrict").val();
    $.ajax({
        url: "/TourClaim/ViewDistrictDetails",
        async: false,
        cache: false,
        data: { scheduleCode: scheduleCode },
        success: function (data) {

            $("#showDistrictDetails").show("slow");
            $("#showDistrictDetails").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
            alert(xhr.responseText);
            $.unblockUI();

        }
    });
});


// travel
function AddTravelClaim(evt, scheduleCode, showOption) {

    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(showOption).style.display = "block";
    evt.currentTarget.className += " active";
    $.ajax({
        url: "/TourClaim/AddTravelClaim",
        async: false,
        cache: false,
        data: { scheduleCode: scheduleCode },
        success: function (data) {
            $("#showTravelForm").show("slow");
            $("#showTravelForm").html(data);
            $("#divTravelClaimList").show("slow");

            LoadTravelClaimList(scheduleCode);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
            alert(xhr.responseText);
            $.unblockUI();

        }
    });
}

$('#btnSaveTravelClaim').click(function (e) {
    if ($("#formAddTravelClaim").valid()) {
        if (confirm("Are you sure you want to save travel claim details ?")) {
            var form = $('#formAddTravelClaim');
            var formadata = new FormData(form.get(0));
            var fileUpload = $("#BGFile").get(0);
            var FileBG = fileUpload.files[0];

            var fileUpload2 = $("#BGFile2").get(0);
            var FileBG2 = fileUpload2.files[0];

            formadata.append(FileBG, FileBG2);
           
            $.ajax({
                url: "/TourClaim/InsertTravelClaimDetails",
                type: "POST",
                async: false,
                cache: false,
                contentType: false,
                processData: false,
                data: formadata,
                success: function (response) {
                    alert(response.message);
                    if (response.success) {
                        $("#divTravelClaimList").show("slow");
                        LoadTravelClaimList($("#scheduleCodeTravel").val());
                        $('#btnResetTravelClaim').trigger('click');
                        $('#departureFrom').val('');
                    }
                    else {
                        //$('#btnResetTravelClaim').trigger('click');
                    }

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                    alert(xhr.responseText);
                    $.unblockUI();

                }
            });
        }
    }
    else {
        return false;
    }

})

function DeleteTravelDetails(id) {

    const myArray = id.split("$");
    var travelId = myArray[0];
    var scheduleCode = myArray[1];

    if (confirm("Are you sure to delete details ?")) {
        $.ajax({
            url: "/TourClaim/DeleteTravelDetails/",
            type: "POST",
            async: false,
            cache: false,
            data: { travelId: travelId },
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    LoadTravelClaimList(scheduleCode);
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    else {
        return false;
    }
}

function LoadTravelClaimList(scheduleCode) {

    jQuery("#tbTravelClaimList").jqGrid('GridUnload');
    jQuery("#tbTravelClaimList").jqGrid({
        url: '/TourClaim/GetTravelClaimList',
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        postData: { scheduleCode: scheduleCode },
        colNames: ['Travel Claim Id', 'Tour Claim Id', 'Start Date Time', 'End Date Time', 'Departure From', 'Arrival At', 'Mode Of Travel', 'Travel Class', 'Amount Claimed', 'Amount Passed', 'Date of amount passing', 'View Uploaded Ticket', 'View Uploaded Boarding Pass', 'Edit', 'Delete'],
        colModel: [
            { name: 'TRAVEL_CLAIM_ID', index: 'TRAVEL_CLAIM_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'TOUR_CLAIM_ID', index: 'TOUR_CLAIM_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'START_DATE_OF_TRAVEL', index: 'START_DATE_OF_TRAVEL', height: 'auto', width: 80, align: "center", search: false },
            //{ name: 'START_TIME', index: 'START_TIME', height: 'auto', width: 50, align: "center", search: false },
            { name: 'END_DATE_OF_TRAVEL', index: 'END_DATE_OF_TRAVEL', height: 'auto', width: 80, align: "center", search: false },
            //{ name: 'END_TIME', index: 'END_TIME', height: 'auto', width: 50, align: "center", search: false },
            { name: 'DEPARTURE_FROM', index: 'DEPARTURE_FROM', width: 80, sortable: true, align: "center" },
            { name: 'ARRIVAL_AT', index: 'ARRIVAL_AT', width: 80, sortable: true, align: "center" },
            { name: 'MODE_OF_TRAVEL', index: 'MODE_OF_TRAVEL', width: 80, sortable: true, align: "center" },
            { name: 'TRAVEL_CLASS', index: 'TRAVEL_CLASS', width: 80, sortable: true, align: "center" },
            { name: 'AMOUNT_CLAIMED', index: 'AMOUNT_CLAIMED', height: 'auto', width: 80, align: "center", search: false },
            { name: 'AMOUNT_PASSED', index: 'AMOUNT_PASSED', height: 'auto', width: 80, align: "center", search: false },
            { name: 'DATE_OF_PASSING', index: 'DATE_OF_PASSING', height: 'auto', width: 80, align: "center", search: false },
            { name: 'VIEW_UPLOADED_TICKET', index: 'VIEW_UPLOADED_TICKET', height: 'auto', width: 80, align: "center", search: false },
            { name: 'VIEW_UPLOADED_BOARDING_PASS', index: 'VIEW_UPLOADED_BOARDING_PASS', height: 'auto', width: 80, align: "center", search: false },
            { name: 'EDIT', index: 'EDIT', height: 'auto', width: 80, align: "center", search: false },
            { name: 'DELETE', index: 'DELETE', height: 'auto', width: 80, align: "center", search: false },
        ],
        pager: jQuery('#pagerTravelClaimList').width(20),
        rowNum: 5,
        rowList: [5, 10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "DATE_OF_TRAVEL",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Travel Claim Details",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        onSelectRow: function (id) {
        },
        loadComplete: function (data) {

            $("#tbTravelClaimList #pagerTravelClaimList").css({ height: '40px' });
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

$('#ddlModes').change(function () {

    $("#ddlClass").empty();

    if ($('#ddlModes').val() == 5) {
        $('#note').hide();
        $('.ticket').hide();
        $('#classVal').hide();
    }
    else {
        $('#classVal').show();
        $('#note').show();
        $('.ticket').show();
    }


    if ($('#ddlModes').val() == 4 || $('#ddlModes').val() == 5 || $('#ddlModes').val() == 6) {
        $('#ddlClassLabel').hide();
        $('#ddlClass').hide();
        $('#classVal').hide();
        $('.boardingPass').hide();
    }
    else {

        if ($('#ddlModes').val() == 2) {
            $('.boardingPass').show();
        }
        else {
            $('.boardingPass').hide();
        }

        $('#ddlClassLabel').show();
        $('#ddlClass').show();
        $('#classVal').show();

        $.ajax({
            url: '/TourClaim/PopulateTravelClass',
            type: 'POST',
            async: false,
            cache: false,
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { mode: $('#ddlModes').val() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddlClass").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }

                $.unblockUI();
            },
            error: function (err) {
                $.unblockUI();
            }
        });
    }
});

function EditTravelDetails(travelId) {

    if (confirm("Are you sure to edit details ?")) {
        $.ajax({
            url: "/TourClaim/EditTravelDetail/",
            async: false,
            cache: false,
            data: { travelId: travelId },
            success: function (data) {
                $("#showTravelForm").show("slow");
                $("#showTravelForm").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    else {
        return false;
    }
}

$('#btnUpdateTravelClaim').click(function (e) {

    var scheduleCode = $('#scheduleCodeTravel').val();

    if (confirm("Are you sure you want to update travel claim details ?")) {

        var form = $('#formAddTravelClaim');
        var formadata = new FormData(form.get(0));
        var fileUpload = $("#BGFile").get(0);
        var FileBG = fileUpload.files[0];
        var fileUpload2 = $("#BGFile2").get(0);
        var FileBG2 = fileUpload2.files[0];

        formadata.append(FileBG, FileBG2);
        
        $.ajax({
            url: "/TourClaim/UpdateTravelClaimDetails",
            type: "POST",
            async: false,
            cache: false,
            contentType: false,
            processData: false,
            data: formadata,
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    $.ajax({
                        url: "/TourClaim/AddTravelClaim",
                        async: false,
                        cache: false,
                        data: { scheduleCode: scheduleCode },
                        success: function (data) {
                            $("#showTravelForm").show("slow");
                            $("#showTravelForm").html(data);
                            $("#divTravelClaimList").show("slow");

                            LoadTravelClaimList(scheduleCode);
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert("error");
                            alert(xhr.responseText);
                            $.unblockUI();

                        }
                    });
                }
                
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
});

$('#btnBackTravel').click(function (e) {
    var scheduleCode = $("#scheduleCodeTravel").val();
    $.ajax({
        url: "/TourClaim/AddTravelClaim",
        async: false,
        cache: false,
        data: { scheduleCode: scheduleCode },
        success: function (data) {
            $("#showTravelForm").show("slow");
            $("#showTravelForm").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
            alert(xhr.responseText);
            $.unblockUI();

        }
    });
});


// Lodge
$("#typeOfClaim").change(function (e) {
    if ($("#typeOfClaim").val() == "H") {

        $('.hotel').show();
        $('.guest').hide();
    }
    else if ($("#typeOfClaim").val() == "G") {

        $('.guest').show();
        $('.hotel').hide();
    }
    else {
        $('.hotel').hide();
        $('.guest').hide();
    }
})

$('#ddlStatesForLodge').change(function () {
    $("#ddlDistrictForLodge").empty();

    $.ajax({
        url: '/QualityMonitoring/PopulateDistrictsbyStateCode',
        type: 'POST',
        async: false,
        cache: false,
        beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        },
        data: { stateCode: $("#ddlStatesForLodge").val(), },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                $("#ddlDistrictForLodge").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }

            $.unblockUI();
        },
        error: function (err) {
            $.unblockUI();
        }
    });
});

function AddLodgeClaim(evt, scheduleCode, showOption) {

    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(showOption).style.display = "block";
    evt.currentTarget.className += " active"

    $.ajax({
        url: "/TourClaim/AddLodgeClaim",
        async: false,
        cache: false,
        data: { scheduleCode: scheduleCode },
        success: function (data) {
            $("#showLodgeForm").show("slow");
            $("#showLodgeForm").html(data);
            $("#divLodgeClaimList").show("slow");
            LoadLodgeClaimList(scheduleCode);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
            alert(xhr.responseText);
            $.unblockUI();

        }
    });
}

$('#btnSaveLodgeClaim').click(function (e) {

    if ($("#formAddLodgeClaim").valid()) {
        if (confirm("Are you sure you want to save Lodge claim changes ?")) {
            var form = $('#formAddLodgeClaim');
            var formadata = new FormData(form.get(0));
            var fileUpload1 = $("#BGFile1").get(0); // for bill
            var FileBG1 = fileUpload1.files[0];
            var fileUpload2 = $("#BGFile2").get(0);  // for receipt
            var FileBG2 = fileUpload2.files[0];
            var fileUpload3 = $("#BGFile3").get(0);  // for guest house bill
            var FileBG3 = fileUpload3.files[0];

            $.ajax({
                url: "/TourClaim/InsertLodgeClaimDetails",
                type: "POST",
                async: false,
                cache: false,
                contentType: false,
                processData: false,
                data: formadata,
                success: function (response) {
                    alert(response.message);
                    if (response.success) {
                        $("#divLodgeClaimList").show("slow");
                        LoadLodgeClaimList($("#scheduleCodelLodge").val());
                        $('#btnResetLodgeClaim').trigger('click');
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                    alert(xhr.responseText);
                    $.unblockUI();

                }
            });
        }
        else {
            return false;
        }
    }

})

function LoadLodgeClaimList(scheduleCode) {

    jQuery("#tbLodgeClaimList").jqGrid('GridUnload');
    jQuery("#tbLodgeClaimList").jqGrid({
        url: '/TourClaim/GetLodgeClaimList',
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        postData: { scheduleCode: scheduleCode },
        colNames: ['Lodge Claim Id', 'Tour Claim Id', 'Date From', 'Date To', 'Type of Claim', 'Hotel Name', 'Daily', 'Hotel', 'Amount Passed', 'Date of amount passing', 'View Uploaded Bill', 'View Uploaded e-Receipt', 'Edit', 'Delete'],
        colModel: [
            { name: 'LODGE_CLAIM_ID', index: 'LODGE_CLAIM_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'TOUR_CLAIM_ID', index: 'TOUR_CLAIM_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'DATE_FROM', index: 'DATE_FROM', height: 'auto', width: 80, align: "center", search: false },
            { name: 'DATE_TO', index: 'DATE_TO', width: 80, sortable: true, align: "center" },
            { name: 'TYPE_OF_CLAIM', index: 'TYPE_OF_CLAIM', width: 80, sortable: true, align: "center" },
            { name: 'HOTEL_NAME', index: 'HOTEL_NAME', width: 80, sortable: true, align: "center" },
            { name: 'AMOUNT_CLAIMED_DAILY', index: 'AMOUNT_CLAIMED_DAILY', height: 'auto', width: 80, align: "center", search: false },
            { name: 'AMOUNT_CLAIMED_HOTEL', index: 'AMOUNT_CLAIMED_HOTEL', height: 'auto', width: 80, align: "center", search: false },
            { name: 'AMOUNT_PASSED', index: 'AMOUNT_PASSED', height: 'auto', width: 80, align: "center", search: false },
            { name: 'DATE_OF_PASSING', index: 'DATE_OF_PASSING', height: 'auto', width: 80, align: "center", search: false },
            { name: 'VIEW_UPLOADED_BILL', index: 'VIEW_UPLOADED_BILL', height: 'auto', width: 80, align: "center", search: false },
            { name: 'VIEW_UPLOADED_RECEIPT', index: 'VIEW_UPLOADED_RECEIPT', height: 'auto', width: 80, align: "center", search: false },
            { name: 'EDIT', index: 'EDIT', height: 'auto', width: 80, align: "center", search: false },
            { name: 'DELETE', index: 'DELETE', height: 'auto', width: 80, align: "center", search: false },
        ],
        pager: jQuery('#pagerLodgeClaimList').width(20),
        rowNum: 5,
        rowList: [5, 10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "DATE_FROM",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Lodge Claim and Daily Allowance Details",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        onSelectRow: function (id) {
        },
        loadComplete: function (data) {

            $("#tbLodgeClaimList #pagerLodgeClaimList").css({ height: '40px' });
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

    jQuery("#tbLodgeClaimList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
            { startColumnName: 'AMOUNT_CLAIMED_DAILY', numberOfColumns: 2, titleText: '<center>Amount Claimed</center>' },
        ]
    });

}

function EditLodgeDetails(lodgeId) {

    if (confirm("Are you sure to edit details ?")) {
        $.ajax({
            url: "/TourClaim/EditLodgeDetail/",
            async: false,
            cache: false,
            data: { lodgeId: lodgeId },
            success: function (data) {
                $("#showLodgeForm").show("slow");
                $("#showLodgeForm").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    else {
        return false;
    }
}

function DeleteLodgeDetails(id) {

    const myArray = id.split("$");
    var lodgeId = myArray[0];
    var scheduleCode = myArray[1];

    if (confirm("Are you sure to delete details ?")) {
        $.ajax({
            url: "/TourClaim/DeleteLodgeDetails/",
            type: "POST",
            async: false,
            cache: false,
            data: { lodgeId: lodgeId },
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    LoadLodgeClaimList(scheduleCode);
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    else {
        return false;
    }
}

$('#btnUpdateLodgeClaim').click(function (e) {

    var scheduleCode = $('#scheduleCodelLodge').val();

    if (confirm("Are you sure you want to update Lodge claim changes ?")) {
        var form = $('#formAddLodgeClaim');
        var formadata = new FormData(form.get(0));
        var fileUpload1 = $("#BGFile1").get(0); // for bill
        var FileBG1 = fileUpload1.files[0];
        var fileUpload2 = $("#BGFile2").get(0);  // for receipt
        var FileBG2 = fileUpload2.files[0];
        var fileUpload3 = $("#BGFile3").get(0);  // for guest house bill
        var FileBG3 = fileUpload3.files[0];

        $.ajax({
            url: "/TourClaim/UpdateLodgeClaimDetails",
            type: "POST",
            async: false,
            cache: false,
            contentType: false,
            processData: false,
            data: formadata,
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    $.ajax({
                        url: "/TourClaim/AddLodgeClaim",
                        async: false,
                        cache: false,
                        data: { scheduleCode: scheduleCode },
                        success: function (data) {
                            $("#showLodgeForm").show("slow");
                            $("#showLodgeForm").html(data);
                            $("#divLodgeClaimList").show("slow");
                            LoadLodgeClaimList(scheduleCode);
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert("error");
                            alert(xhr.responseText);
                            $.unblockUI();

                        }
                    });
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
});

$('#btnBackLodge').click(function (e) {
    var scheduleCode = $("#scheduleCodelLodge").val();
    $.ajax({
        url: "/TourClaim/AddLodgeClaim",
        async: false,
        cache: false,
        data: { scheduleCode: scheduleCode },
        success: function (data) {
            $("#showLodgeForm").show("slow");
            $("#showLodgeForm").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
            alert(xhr.responseText);
            $.unblockUI();

        }
    });
});

// Inspection

function PerformInspectionOfRoad(evt, scheduleCode, showOption) {

    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(showOption).style.display = "block";
    evt.currentTarget.className += " active"

    //LoadInspectionHonorariumList(scheduleCode);

    $.ajax({
        url: "/TourClaim/InspectionOfRoadsHonorarium",
        async: false,
        cache: false,
        data: { scheduleCode: scheduleCode },
        success: function (data) {

            $("#showInspectionOfRoadsHonorarium").hide();
            $("#showInspectionOfRoadsHonorarium").show("slow");
            $("#showInspectionOfRoadsHonorarium").html(data);

            $('#divInspectionOfRoadsHonorarium').show('slow');
            LoadInspectionHonorariumList(scheduleCode);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
            alert(xhr.responseText);
            $.unblockUI();

        }
    });
}

function PerformInspectionOfRoads() {
    $('#formAddInspectionHonorarium').show('slow');
}

$("#typeOfWork").change(function (e) {
    if ($("#typeOfWork").val() == "O") {
        $("#typeOfWork_other").empty();
        $("#typeOfWork_other").show('slow');
    }
    else {
        $("#typeOfWork_other").empty();
        $("#typeOfWork_other").hide('slow');
    }
})

$("#btnSaveInspectionHonorarium").click(function (e) {
    if ($("#formAddInspectionHonorarium").valid()) {
        if (confirm("Are you sure you wanted to save honorarium for inspection of roads changes ?")) {
            $.ajax({
                url: "/TourClaim/InsertInspectionHonorarium",
                type: "POST",
                async: false,
                cache: false,
                data: $("#formAddInspectionHonorarium").serialize(),
                success: function (response) {
                    alert(response.message);
                    if (response.success) {
                        $('#formAddInspectionHonorarium').hide('slow');
                        $('#divInspectionOfRoadsHonorarium').show('slow');
                        LoadInspectionHonorariumList($("#scheduleCodeInsp").val());
                        $('#btnResetInspectionHonorarium').trigger('click');
                    }
                    else {
                        //$('#btnResetInspectionHonorarium').trigger('click');
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                    alert(xhr.responseText);
                    $.unblockUI();

                }
            });
        }
    }

})

function LoadInspectionHonorariumList(scheduleCode) {

    jQuery("#tbInspectionOfRoadsHonorarium").jqGrid('GridUnload');
    jQuery("#tbInspectionOfRoadsHonorarium").jqGrid({
        url: '/TourClaim/GetInspectionHonorariumList',
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        postData: { scheduleCode: scheduleCode },
        colNames: ['HONORARIUM_INSPECTION_ID', 'TourClaimId', 'Date of Inspection', 'Work Type', 'Amount Claimed', 'Amount Passed', 'Date of amount passing', 'Edit', 'Delete'],
        colModel: [
            { name: 'HONORARIUM_INSPECTION_ID', index: 'HONORARIUM_INSPECTION_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'TOUR_CLAIM_ID', index: 'TOUR_CLAIM_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'DATE_OF_INSPECTION', index: 'DATE_OF_INSPECTION', width: 80, sortable: true, align: "center" },
            { name: 'TYPE_OF_WORK', index: 'TYPE_OF_WORK', width: 80, sortable: true, align: "center" },
            { name: 'AMOUNT_CLAIMED', index: 'AMOUNT_CLAIMED', height: 'auto', width: 80, align: "center", search: false },
            { name: 'AMOUNT_PASSED', index: 'AMOUNT_PASSED', height: 'auto', width: 80, align: "center", search: false },
            { name: 'DATE_OF_PASSING', index: 'DATE_OF_PASSING', height: 'auto', width: 80, align: "center", search: false },
            { name: 'EDIT', index: 'EDIT', height: 'auto', width: 80, align: "center", search: false },
            { name: 'DELETE', index: 'DELETE', height: 'auto', width: 80, align: "center", search: false },
        ],
        pager: jQuery('#pagerInspectionOfRoadsHonorarium').width(20),
        rowNum: 5,
        rowList: [5, 10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "DATE_OF_INSPECTION",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Honorarium for Inspection Of Roads Details",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        onSelectRow: function (id) {
        },
        loadComplete: function (data) {

            $("#tbInspectionOfRoadsHonorarium #pagerInspectionOfRoadsHonorarium").css({ height: '40px' });

            $("#pagerInspectionOfRoadsHonorarium_left").html("<input type='button' style='margin-left:1px; margin-top:1px' id='idAddInspection' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'PerformInspectionOfRoads();return false;' value='Add Inspection'/>");
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

function DeleteInspectionDetails(id) {

    const myArray = id.split("$");
    var inspectionId = myArray[0];
    var scheduleCode = myArray[1];

    if (confirm("Are you sure to delete details ?")) {
        $.ajax({
            url: "/TourClaim/DeleteInspectionDetails/",
            type: "POST",
            async: false,
            cache: false,
            data: { inspectionId: inspectionId },
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    LoadInspectionHonorariumList(scheduleCode);
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    else {
        return false;
    }
}

function EditInspectionDetails(id) {

    $('#formAddInspectionHonorarium').show('slow');

    if (confirm("Are you sure to edit details ?")) {
        $.ajax({
            url: "/TourClaim/EditInspectionDetails/",
            async: false,
            cache: false,
            data: { inspectionId: id },
            success: function (data) {
                $("#showInspectionOfRoadsHonorarium").html(data);
                $('#formAddInspectionHonorarium').show('slow');
                $("#showInspectionOfRoadsHonorarium").show("slow");
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    else {
        return false;
    }
}

$('#btnEditInspectionHonorarium').click(function (e) {

    var scheduleCode = $('#scheduleCodeInsp').val();

    if ($("#formAddInspectionHonorarium").valid()) {
        if (confirm("Are you sure you wanted to update honorarium for inspection of roads changes ?")) {
            $.ajax({
                url: "/TourClaim/UpdateInspectionHonorarium",
                type: "POST",
                async: false,
                cache: false,
                data: $("#formAddInspectionHonorarium").serialize(),
                success: function (response) {
                    alert(response.message);
                    if (response.success) {
                        $.ajax({
                            url: "/TourClaim/InspectionOfRoadsHonorarium",
                            async: false,
                            cache: false,
                            data: { scheduleCode: scheduleCode },
                            success: function (data) {

                                $("#showInspectionOfRoadsHonorarium").hide();
                                $("#showInspectionOfRoadsHonorarium").show("slow");
                                $("#showInspectionOfRoadsHonorarium").html(data);

                                $('#divInspectionOfRoadsHonorarium').show('slow');
                                LoadInspectionHonorariumList(scheduleCode);
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                alert("error");
                                alert(xhr.responseText);
                                $.unblockUI();

                            }
                        });
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                    alert(xhr.responseText);
                    $.unblockUI();

                }
            });
        }
    }
});

$("#btnBackInspection").click(function (e) {
    var scheduleCode = $("#scheduleCodeInsp").val();
    $.ajax({
        url: "/TourClaim/InspectionOfRoadsHonorarium",
        async: false,
        cache: false,
        data: { scheduleCode: scheduleCode },
        success: function (data) {

            $("#showInspectionOfRoadsHonorarium").hide();
            $("#showInspectionOfRoadsHonorarium").show("slow");
            $("#showInspectionOfRoadsHonorarium").html(data);

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
            alert(xhr.responseText);
            $.unblockUI();

        }
    });

})


// Meeting

function MeetingWithPIU(evt, scheduleCode, showOption) {

    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(showOption).style.display = "block";
    evt.currentTarget.className += " active"

    $.ajax({
        url: "/TourClaim/MeetingWihPIUHonorarium",
        async: false,
        cache: false,
        data: { scheduleCode: scheduleCode },
        success: function (data) {
            $("#showMeetingHonorarium").show("slow");
            $("#showMeetingHonorarium").html(data);

            $('#divMeetingHonorarium').show('slow');
            LoadMeetingHonorariumList(scheduleCode);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
            alert(xhr.responseText);
            $.unblockUI();

        }
    });
}

$('#meetingState').change(function () {
    $("#meetingDistrict").empty();

    $.ajax({
        url: '/QualityMonitoring/PopulateDistrictsbyStateCode',
        type: 'POST',
        async: false,
        cache: false,
        beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        },
        data: { stateCode: $("#meetingState").val(), },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                $("#meetingDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }

            $.unblockUI();
        },
        error: function (err) {
            $.unblockUI();
        }
    });
});

$("#btnSaveMeetingHonorarium").click(function (e) {

    if ($("#formAddMeetingHonorarium").valid()) {
        if (confirm("Are you sure you want to save honorarium for meeting with PIU changes ?")) {
            var form = $('#formAddMeetingHonorarium');
            var formadata = new FormData(form.get(0));
            var fileUpload = $("#BGFile").get(0);
            var FileBG = fileUpload.files[0]

            formadata.append("BGFile", FileBG);

            $.ajax({
                url: "/TourClaim/InsertMeetingHonorarium",
                type: "POST",
                async: false,
                cache: false,
                contentType: false,
                processData: false,
                data: formadata,
                success: function (response) {
                    alert(response.message);
                    if (response.success) {
                        $('#divMeetingHonorarium').show('slow');
                        LoadMeetingHonorariumList($("#scheduleCodeMeeting").val());
                        $('#btnResetMeetingHonorarium').trigger('click');
                    }
                    else {
                        //$('#btnResetMeetingHonorarium').trigger('click');
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                    alert(xhr.responseText);
                    $.unblockUI();

                }
            });
        }
        else {
            return false;
        }
    }

})

function LoadMeetingHonorariumList(scheduleCode) {

    jQuery("#tbMeetingHonorarium").jqGrid('GridUnload');
    jQuery("#tbMeetingHonorarium").jqGrid({
        url: '/TourClaim/GetMeetingHonorariumList',
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        postData: { scheduleCode: scheduleCode },
        colNames: ['HONORARIUM_MEETING_ID', 'TourClaimId', 'Date of Meeting', 'Place of Meeting', 'Amount Claimed', 'Amount Passed', 'Date of amount passing', 'View Uploaded attendance sheet', 'View Uploaded meeting details', 'View Uploaded Geotagged Photograph', 'Edit','Delete'],
        colModel: [
            { name: 'HONORARIUM_MEETING_ID', index: 'HONORARIUM_MEETING_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'TOUR_CLAIM_ID', index: 'TOUR_CLAIM_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'DATE_OF_MEETING', index: 'DATE_OF_MEETING', width: 80, sortable: true, align: "center" },
            { name: 'PLACE', index: 'PLACE', width: 80, sortable: true, align: "center" },
            { name: 'AMOUNT_CLAIMED', index: 'AMOUNT_CLAIMED', height: 'auto', width: 80, align: "center", search: false },
            { name: 'AMOUNT_PASSED', index: 'AMOUNT_PASSED', height: 'auto', width: 80, align: "center", search: false },
            { name: 'DATE_OF_PASSING', index: 'DATE_OF_PASSING', height: 'auto', width: 80, align: "center", search: false },
            { name: 'VIEW_UPLOADED_ATTENDANCE_SHEET', index: 'VIEW_UPLOADED_ATTENDANCE_SHEET', height: 'auto', width: 80, align: "center", search: false },
            { name: 'VIEW_UPLOADED_MEETING_DETAILS', index: 'VIEW_UPLOADED_MEETING_DETAILS', height: 'auto', width: 80, align: "center", search: false },
            { name: 'VIEW_UPLOADED_PHOTO', index: 'VIEW_UPLOADED_PHOTO', height: 'auto', width: 80, align: "center", search: false },
            { name: 'EDIT', index: 'EDIT', height: 'auto', width: 80, align: "center", search: false },
            { name: 'DELETE', index: 'DELETE', height: 'auto', width: 80, align: "center", search: false },
        ],
        pager: jQuery('#pagerMeetingHonorarium').width(20),
        rowNum: 5,
        rowList: [5, 10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "DATE_OF_MEETING",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Honorarium for Meeting with PIU",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        onSelectRow: function (id) {

        },
        loadComplete: function (data) {

            $("#tbMeetingHonorarium #pagerMeetingHonorarium").css({ height: '40px' });
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

function DeleteMeetingDetails(id) {

    const myArray = id.split("$");
    var meetingId = myArray[0];
    var scheduleCode = myArray[1];

    if (confirm("Are you sure to delete details ?")) {
        $.ajax({
            url: "/TourClaim/DeleteMeetingDetails/",
            type: "POST",
            async: false,
            cache: false,
            data: { meetingId: meetingId },
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    LoadMeetingHonorariumList(scheduleCode);
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    else {
        return false;
    }
}

function EditMeetingDetails(id) {

    if (confirm("Are you sure to edit details ?")) {
        $.ajax({
            url: "/TourClaim/EditMeetingDetails/",
            async: false,
            cache: false,
            data: { meetingId: id },
            success: function (data) {
                $("#showMeetingHonorarium").show("slow");
                $("#showMeetingHonorarium").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    else {
        return false;
    }
}

$('#btnUpdateMeetingHonorarium').click(function (e) {

    var scheduleCode = $('#scheduleCodeMeeting').val();

    if (confirm("Are you sure you want to save honorarium for meeting with PIU changes ?")) {
        var form = $('#formAddMeetingHonorarium');
        var formadata = new FormData(form.get(0));
        var fileUpload = $("#BGFile").get(0);
        var FileBG = fileUpload.files[0]

        formadata.append("BGFile", FileBG);

        $.ajax({
            url: "/TourClaim/UpdateMeetingHonorarium",
            type: "POST",
            async: false,
            cache: false,
            contentType: false,
            processData: false,
            data: formadata,
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    $.ajax({
                        url: "/TourClaim/MeetingWihPIUHonorarium",
                        async: false,
                        cache: false,
                        data: { scheduleCode: scheduleCode },
                        success: function (data) {
                            $("#showMeetingHonorarium").show("slow");
                            $("#showMeetingHonorarium").html(data);

                            $('#divMeetingHonorarium').show('slow');
                            LoadMeetingHonorariumList(scheduleCode);
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert("error");
                            alert(xhr.responseText);
                            $.unblockUI();

                        }
                    });
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    else {
        return false;
    }
});

$("#btnBackMeeting").click(function (e) {

    var scheduleCode = $("#scheduleCodeMeeting").val();

    $.ajax({
        url: "/TourClaim/MeetingWihPIUHonorarium",
        async: false,
        cache: false,
        data: { scheduleCode: scheduleCode },
        success: function (data) {
            $("#showMeetingHonorarium").show("slow");
            $("#showMeetingHonorarium").html(data);

            $('#divMeetingHonorarium').show('slow');
            LoadMeetingHonorariumList(scheduleCode);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
            alert(xhr.responseText);
            $.unblockUI();

        }
    });

});

// Miscellaneous

function AddMiscellaneousClaim(evt, scheduleCode, showOption) {

    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(showOption).style.display = "block";
    evt.currentTarget.className += " active";
    $.ajax({
        url: "/TourClaim/AddMiscellaneousClaim",
        async: false,
        cache: false,
        data: { scheduleCode: scheduleCode },
        success: function (data) {
            $("#showMiscellaneous").show("slow");
            $("#showMiscellaneous").html(data);
            $("#divMiscellaneousClaimList").show("slow");

            LoadMiscellaneousClaimList(scheduleCode);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
            alert(xhr.responseText);
            $.unblockUI();

        }
    });
}

$('#btnSaveMiscellaneousClaim').click(function (e) {
    if ($("#formAddMiscellaneousClaim").valid()) {
        if (confirm("Are you sure you want to save miscellaneous claim details ?")) {
            var form = $('#formAddMiscellaneousClaim');
            var formadata = new FormData(form.get(0));
            var fileUpload = $("#BGFile").get(0);
            var FileBG = fileUpload.files[0]
            formadata.append("BGFile", FileBG);

            $.ajax({
                url: "/TourClaim/InsertMiscellaneousClaimDetails",
                type: "POST",
                async: false,
                cache: false,
                contentType: false,
                processData: false,
                data: formadata,
                success: function (response) {
                    alert(response.message);
                    if (response.success) {
                        $("#divMiscellaneousClaimList").show("slow");
                        LoadMiscellaneousClaimList($("#scheduleCodeMiscellaneous").val());
                        $('#btnResetMiscellaneousClaim').trigger('click');
                    }
                    else {
                        //$('#btnResetMiscellaneousClaim').trigger('click');
                    }

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                    alert(xhr.responseText);
                    $.unblockUI();

                }
            });
        }
    }
    else {
        return false;
    }

})

function LoadMiscellaneousClaimList(scheduleCode) {

    jQuery("#tbMiscellaneousClaimList").jqGrid('GridUnload');
    jQuery("#tbMiscellaneousClaimList").jqGrid({
        url: '/TourClaim/GetMiscellaneousClaimList',
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        postData: { scheduleCode: scheduleCode },
        colNames: ['Mis Id', 'Tour Claim Id', 'Date', 'Description', 'Amount Claimed', 'Amount Passed', 'Date of amount passing', 'View Uploaded File', 'Edit', 'Delete'],
        colModel: [
            { name: 'MISCELLANEOUS_ID', index: 'MISCELLANEOUS_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'TOUR_CLAIM_ID', index: 'TOUR_CLAIM_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'DATE', index: 'DATE', height: 'auto', width: 80, align: "center", search: false },
            { name: 'DESCRIPTION', index: 'DESCRIPTION', height: 'auto', width: 100, align: "center", search: false },
            { name: 'AMOUNT_CLAIMED', index: 'AMOUNT_CLAIMED', height: 'auto', width: 80, align: "center", search: false },
            { name: 'AMOUNT_PASSED', index: 'AMOUNT_PASSED', height: 'auto', width: 80, align: "center", search: false },
            { name: 'DATE_OF_PASSING', index: 'DATE_OF_PASSING', height: 'auto', width: 80, align: "center", search: false },
            { name: 'VIEW_UPLOADED_FILE', index: 'VIEW_UPLOADED_FILE', height: 'auto', width: 80, align: "center", search: false },
            { name: 'EDIT', index: 'EDIT', height: 'auto', width: 80, align: "center", search: false },
            { name: 'DELETE', index: 'DELETE', height: 'auto', width: 80, align: "center", search: false },

        ],
        pager: jQuery('#pagerMiscellaneousClaimList').width(20),
        rowNum: 5,
        rowList: [5, 10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "MISCELLANEOUS_ID",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Miscellaneous Claim Details",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        onSelectRow: function (id) {
        },
        loadComplete: function (data) {

            $("#tbMiscellaneousClaimList #pagerMiscellaneousClaimList").css({ height: '40px' });
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

function EditMiscellaneousDetails(id) {
    
    const myArray = id.split("$");
    var miscellaneousId = myArray[0];
    var scheduleCode = myArray[1];

    if (confirm("Are you sure to edit details ?")) {
        $.ajax({
            url: "/TourClaim/EditMiscellaneousDetails/",
            type: "POST",
            async: false,
            cache: false,
            data: { miscellaneousId: miscellaneousId },
            success: function (data) {
                $("#showMiscellaneous").show("slow");
                $("#showMiscellaneous").html(data);
                $("#divMiscellaneousClaimList").show("slow");

                LoadMiscellaneousClaimList(scheduleCode);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    else {
        return false;
    }
}

$('#btnUpdateMiscellaneousClaim').click(function (e) {

    var scheduleCode = $('#scheduleCodeMiscellaneous').val();

    if ($("#formAddMiscellaneousClaim").valid()) {
        if (confirm("Are you sure you want to update miscellaneous claim details changes ?")) {
            var form = $('#formAddMiscellaneousClaim');
            var formadata = new FormData(form.get(0));
            var fileUpload = $("#BGFile").get(0);
            var FileBG = fileUpload.files[0]
            formadata.append("BGFile", FileBG);
            
            $.ajax({
                url: "/TourClaim/UpdateMiscellaneousClaimDetails",
                type: "POST",
                async: false,
                cache: false,
                contentType: false,
                processData: false,
                data: formadata,
                success: function (response) {
                    alert(response.message);
                    if (response.success) {
                        $.ajax({
                            url: "/TourClaim/AddMiscellaneousClaim",
                            async: false,
                            cache: false,
                            data: { scheduleCode: scheduleCode },
                            success: function (data) {
                                $("#showMiscellaneous").show("slow");
                                $("#showMiscellaneous").html(data);
                                $("#divMiscellaneousClaimList").show("slow");

                                LoadMiscellaneousClaimList(scheduleCode);
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                alert("error");
                                alert(xhr.responseText);
                                $.unblockUI();

                            }
                        });
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                    alert(xhr.responseText);
                    $.unblockUI();

                }
            });
        }
    }
});

function DeleteMiscellaneousDetails(id) {

    const myArray = id.split("$");
    var miscellaneousId = myArray[0];
    var scheduleCode = myArray[1];

    if (confirm("Are you sure to delete details ?")) {
        $.ajax({
            url: "/TourClaim/DeleteMiscellaneousDetails/",
            type: "POST",
            async: false,
            cache: false,
            data: { miscellaneousId: miscellaneousId },
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    LoadMiscellaneousClaimList(scheduleCode);
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    else {
        return false;
    }
}

function DeleteMiscellaneousDetailsFile() {

    var miscellaneousId = $('#miscellaneousId').val();

    if (confirm("Are you sure to delete the uplpoaded file ?")) {
        $.ajax({
            url: "/TourClaim/DeleteUploadedMisFile",
            type: "POST",
            async: false,
            cache: false,
            data: { miscellaneousId: miscellaneousId },
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    $('.viewAndDelete').hide();
                    $('#fileTextBox').show();
                    $('#fileNameLabel').hide();
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    
}

$("#btnBackMiscellaneous").click(function (e) {
    var scheduleCode = $("#scheduleCodeMiscellaneous").val();
    $.ajax({
        url: "/TourClaim/AddMiscellaneousClaim",
        async: false,
        cache: false,
        data: { scheduleCode: scheduleCode },
        success: function (data) {
            $("#showMiscellaneous").show("slow");
            $("#showMiscellaneous").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
            alert(xhr.responseText);
            $.unblockUI();

        }
    });

})

// Permission

function AddPermissionClaim(evt, scheduleCode, showOption) {

    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(showOption).style.display = "block";
    evt.currentTarget.className += " active";
    $.ajax({
        url: "/TourClaim/AddPermissionClaim",
        async: false,
        cache: false,
        data: { scheduleCode: scheduleCode },
        success: function (data) {
            $("#showPermission").show("slow");
            $("#showPermission").html(data);
            $("#divPermissionClaimList").show("slow");

            LoadPermissionClaimList(scheduleCode);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
            alert(xhr.responseText);
            $.unblockUI();

        }
    });
}

$('#btnSavePermissionClaim').click(function (e) {
    if ($("#formAddPermissionClaim").valid()) {
        if (confirm("Are you sure you want to save permission details ?")) {
            var form = $('#formAddPermissionClaim');
            var formadata = new FormData(form.get(0));
            var fileUpload = $("#BGFile").get(0);
            var FileBG = fileUpload.files[0]
            formadata.append("BGFile", FileBG);

            $.ajax({
                url: "/TourClaim/InsertPermissionClaimDetails",
                type: "POST",
                async: false,
                cache: false,
                contentType: false,
                processData: false,
                data: formadata,
                success: function (response) {
                    alert(response.message);
                    if (response.success) {
                        $("#divPermissionClaimList").show("slow");
                        LoadPermissionClaimList($("#scheduleCodePermission").val());
                        $('#btnResetPermissionClaim').trigger('click');
                    }
                    else {
                    }

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                    alert(xhr.responseText);
                    $.unblockUI();

                }
            });
        }
    }
    else {
        return false;
    }

})

function LoadPermissionClaimList(scheduleCode) {

    jQuery("#tbPermissionClaimList").jqGrid('GridUnload');
    jQuery("#tbPermissionClaimList").jqGrid({
        url: '/TourClaim/GetPermissionClaimList',
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        postData: { scheduleCode: scheduleCode },
        colNames: ['Per Id', 'Tour Claim Id', 'Date', 'Description', 'View Uploaded File', 'Edit', 'Delete'],
        colModel: [
            { name: 'PERMISSION_ID', index: 'PERMISSION_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'TOUR_CLAIM_ID', index: 'TOUR_CLAIM_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'DATE', index: 'DATE', height: 'auto', width: 80, align: "center", search: false },
            { name: 'DESCRIPTION', index: 'DESCRIPTION', height: 'auto', width: 100, align: "center", search: false },
            { name: 'VIEW_UPLOADED_FILE', index: 'VIEW_UPLOADED_FILE', height: 'auto', width: 80, align: "center", search: false },
            { name: 'EDIT', index: 'EDIT', height: 'auto', width: 80, align: "center", search: false },
            { name: 'DELETE', index: 'DELETE', height: 'auto', width: 80, align: "center", search: false },
        ],
        pager: jQuery('#pagerPermissionClaimList').width(20),
        rowNum: 5,
        rowList: [5, 10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "PERMISSION_ID",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Permissions Details",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        onSelectRow: function (id) {
        },
        loadComplete: function (data) {

            $("#tbPermissionClaimList #pagerPermissionClaimList").css({ height: '40px' });
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

function EditPermissionDetails(id) {

    const myArray = id.split("$");
    var permissionId = myArray[0];
    var scheduleCode = myArray[1];

    if (confirm("Are you sure to edit details ?")) {
        $.ajax({
            url: "/TourClaim/EditPermissionDetails/",
            type: "POST",
            async: false,
            cache: false,
            data: { permissionId: permissionId },
            success: function (data) {
                $("#showPermission").show("slow");
                $("#showPermission").html(data);
                $("#divPermissionClaimList").show("slow");

                LoadPermissionClaimList(scheduleCode);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    else {
        return false;
    }
}

$('#btnUpdatePermissionClaim').click(function (e) {

    var scheduleCode = $('#scheduleCodePermission').val();

    if ($("#formAddPermissionClaim").valid()) {
        if (confirm("Are you sure you want to update permission details ?")) {
            var form = $('#formAddPermissionClaim');
            var formadata = new FormData(form.get(0));
            var fileUpload = $("#BGFile").get(0);
            var FileBG = fileUpload.files[0]
            formadata.append("BGFile", FileBG);

            $.ajax({
                url: "/TourClaim/UpdatePermissionClaimDetails",
                type: "POST",
                async: false,
                cache: false,
                contentType: false,
                processData: false,
                data: formadata,
                success: function (response) {
                    alert(response.message);
                    if (response.success) {
                        $.ajax({
                            url: "/TourClaim/AddPermissionClaim",
                            async: false,
                            cache: false,
                            data: { scheduleCode: scheduleCode },
                            success: function (data) {
                                $("#showPermission").show("slow");
                                $("#showPermission").html(data);
                                $("#divPermissionClaimList").show("slow");

                                LoadPermissionClaimList(scheduleCode);
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                alert("error");
                                alert(xhr.responseText);
                                $.unblockUI();
                            }
                        });
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                    alert(xhr.responseText);
                    $.unblockUI();

                }
            });
        }
    }
});

function DeletePermissionDetails(id) {

    const myArray = id.split("$");
    var permissionId = myArray[0];
    var scheduleCode = myArray[1];

    if (confirm("Are you sure to delete details ?")) {
        $.ajax({
            url: "/TourClaim/DeletePermissionDetails/",
            type: "POST",
            async: false,
            cache: false,
            data: { permissionId: permissionId },
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    LoadPermissionClaimList(scheduleCode);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    }
    else {
        return false;
    }
}

$("#btnBackPermission").click(function (e) {
    var scheduleCode = $("#scheduleCodePermission").val();
    $.ajax({
        url: "/TourClaim/AddPermissionClaim",
        async: false,
        cache: false,
        data: { scheduleCode: scheduleCode },
        success: function (data) {
            $("#showPermission").show("slow");
            $("#showPermission").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
            alert(xhr.responseText);
            $.unblockUI();
        }
    });
})

// Common

function FinalizeTourDetails(id) {

    const myArray = id.split("$");
    var scheduleCode = myArray[0];
    var month = myArray[1];
    var year = myArray[2];

    var monthNum = getMonthFromString(month);

    if (confirm("Are you sure to finalize details ?")) {
        $.ajax({
            url: "/TourClaim/FinalizeTourDetail/",
            type: "POST",
            async: false,
            cache: false,
            data: { scheduleCode: scheduleCode },
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    closeTourClaimList();
                    $('#tbNQMScheduleList').trigger('reloadGrid');
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    else {
        return false;
    }
}

function getMonthFromString(mon) {
    return new Date(Date.parse(mon + " 1, 2022")).getMonth() + 1
}