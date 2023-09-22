var marker = null;
var map;
var infowindow = null;
var directionsDisplay;
var directionsService; // = new google.maps.DirectionsService();
var map2;
var latitude = $("#Latitude").val();
var longitude = $("#Longitude").val();
var uploadDate = $("#UploadDate").val();
var filename = $("#FileName").val();
var myLatlng;
var infowindow;
var contentString = "LATITUDE : " + String(latitude) + "<br>" + "LONGITUDE : " + String(longitude) + "<br>" + "UPLOAD DATE & TIME : " + String(uploadDate)

$(document).ready(function () {
    latitude = $("#Latitude").val();
    longitude = $("#Longitude").val();
    $('#dvPanchyatList').hide("slow");

    $('#btnCancel').click(function () {
        $('#divDisplayFacilityDetails').hide("slow");
        $('#dvPanchyatList').show();

    });

    // Added By Rohit on 04 Sept 2019
    $('#btnFinalize').click(function () {
        if ($("#IsFinalized").val() == "Yes") {
            alert("Facility details are already finalized.")
            return false;
        }
        var id = $("#EncryptedCode").val();
        finalizeProposal(id);
    });


    $("#dvhdCreateNewPanchayatDetails").click(function () {
        if ($("#dvCreateNewPanchayatDetails").is(':visible')) {
            $("#dvCreateNewPanchayatDetails").hide("slow");
        }
        else
        {
            $("#dvCreateNewPanchayatDetails").show("slow");
        }
    });



    $("#imageTag").attr('src', filename);
    LoadMap();
});

var tableCount = 0;

function initGoogle() {
    initialize2();

}


function LoadMap() {
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.id = 'googleMaps';
    script.src = 'https://maps.googleapis.com/maps/api/js?key=AIzaSyDiZZz17Yi4SdQHVTljjYAzNLq-_84GwRo&sensor=false&callback=initGoogle';
    document.body.appendChild(script);

    //  'https://bhuvan-app1.nrsc.gov.in/web_view/index.php?x=77.22&y=28.63&buff=0';
}

function initialize2() {

    latitude = $("#Latitude").val();
    longitude = $("#Longitude").val();
    uploadDate = $("#UploadDate").val();
    myLatlng = new google.maps.LatLng(latitude, longitude);
    var myOptions = {
        zoom: 12,
        center: myLatlng,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    }
    map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);
    contentString = "LATITUDE : " + String(latitude) + "<br>" + "LONGITUDE : " + String(longitude) + "<br>" + "UPLOAD DATE & TIME : " + String(uploadDate)
    infowindow = new google.maps.InfoWindow({
        content: contentString
    });

    TestMarker();

}

function addMarker(location) {

    latitude = $("#Latitude").val();
    longitude = $("#Longitude").val();
    marker = new google.maps.Marker({
        position: location,
        map: map,
        title: "Click Here"
    });
    marker.setIcon('http://maps.google.com/mapfiles/ms/icons/green-dot.png');
    marker.setAnimation(google.maps.Animation.BOUNCE);
    infowindow.open(map, marker);
    marker.addListener('click', function () {
        infowindow.open(map, marker);
    });
}

// Testing the addMarker function
function TestMarker() {
    latitude = $("#Latitude").val();
    longitude = $("#Longitude").val();
    FacilityLocation = new google.maps.LatLng(latitude, longitude);
    addMarker(FacilityLocation);
}

function DeleteImage(id) {
    if (confirm("Are you sure to delete the image ? ")) {

        $.ajax({
            url: "/LocationMasterDataEntry/DeleteImageLatLong/",
            type: "POST",
            cache: false,
            data: {
                FacilityID: id, value: Math.random()
            },
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                unblockPage();
                if (response.success) {
                    $("#spnImageDeleteButton").hide();
                    alert(response.message);
                    $('#tbPanchyatList').trigger('reloadGrid');
                    $("#btnCancel").trigger("click");
                }
                else {
                    alert(response.message);
                }
            }
        });

    }
    else {
        return;
    }
}


// Added By Rohit on 04 Sept 2019
function finalizeProposal(id) {
   
    if (confirm("Are you sure to finalize this facility details ?")) {
        $.ajax({
            url: '/LocationMasterDataEntry/FinalizeFacility/' + id,
            type: "POST",
            cache: false,
            async: false,
            success: function (response) {
                if (response.Success) {
                    alert("Facility Details Finalized successfully");
                    $('#divDisplayFacilityDetails').hide("slow");
                    $('#dvPanchyatList').show();

                    LoadGrid();
                    $('#tbPanchyatList').trigger("reloadGrid");
                    $('#tbPanchyatList').trigger('reloadGrid', { fromServer: true, page: 1 });
                    $('#tbPanchyatList').setGridParam({ datatype: 'json', page: 1 }).trigger('reloadGrid');
                }
                else {
                    
                    $("#divError").show("slow");
                    $("#spnError").html('<strong>Alert : </strong>' + response.ErrorMessage);
                }
                $.unblockUI();
            },
            error: function () {

                $.unblockUI();
                alert("Error : " + error);
                return false;
            }
        });

    }
}










