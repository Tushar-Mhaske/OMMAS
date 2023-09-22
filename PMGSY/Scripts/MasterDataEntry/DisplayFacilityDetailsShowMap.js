var marker = null;
var map;
var infowindow = null;
var directionsDisplay;
var directionsService; // = new google.maps.DirectionsService();
var map2;
var LatitudeDetails = $("#LatitudeDetails").val();
var LongitudeDetails = $("#LongitudeDetails").val();
var UploadDateDetails = $("#UploadDateDetails").val();
//var FileNameDetails = $("#FileNameDetails").val();
var myLatlng;
var infowindow;
var contentString1 = "LATITUDE : " + String(LatitudeDetails) + "<br>" + "LONGITUDE : " + String(LongitudeDetails) + "<br>" + "UPLOAD DATE & TIME : " + String(UploadDateDetails)

$(document).ready(function () {

    LatitudeDetails = $("#LatitudeDetails").val();
    LongitudeDetails = $("#LongitudeDetails").val();
    var FileNameDetails = $("#FileNameDetails").val();

    $("#imageTag").attr('src', FileNameDetails);

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
}

function initialize2() {

    LatitudeDetails = $("#LatitudeDetails").val();
    LongitudeDetails = $("#LongitudeDetails").val();
    UploadDateDetails = $("#UploadDateDetails").val();
    myLatlng = new google.maps.LatLng(LatitudeDetails, LongitudeDetails);
    var myOptions = {
        zoom: 12,
        center: myLatlng,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    }
    map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);
    contentString1 = "LATITUDE : " + String(LatitudeDetails) + "<br>" + "LONGITUDE : " + String(LongitudeDetails) + "<br>" + "UPLOAD DATE & TIME : " + String(UploadDateDetails)
    infowindow = new google.maps.InfoWindow({
        content: contentString1
    });

    TestMarker();

}

function addMarker(location) {

    LatitudeDetails = $("#LatitudeDetails").val();
    LongitudeDetails = $("#LongitudeDetails").val();
    marker = new google.maps.Marker({
        position: location,
        map: map,
        title:"Click Here"
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
    LatitudeDetails = $("#LatitudeDetails").val();
    LongitudeDetails = $("#LongitudeDetails").val();
    FacilityLocation = new google.maps.LatLng(LatitudeDetails, LongitudeDetails);
    addMarker(FacilityLocation);
}














