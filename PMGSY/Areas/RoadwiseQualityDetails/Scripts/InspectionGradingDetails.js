$(document).ready(function () {
    loadMap();
    $("#btnDisplayRoadPath").click(function () {
        getStartEndLatLong();
    });

});
//-------------- Google Map Code Starts here-------------------//
var marker = null;
var map;
var infowindow = null;
var directionsDisplay;
var directionsService; // = new google.maps.DirectionsService();
var map2;
function initGoogle() {
    var latlng = new google.maps.LatLng(21.7679, 78.8718);
    infowindow = new google.maps.InfoWindow();
    var mapOptions = {
        center: latlng,
        zoom: 4
    };

    map = new google.maps.Map(document.getElementById("gmapView"),
        mapOptions);

    marker = new google.maps.Marker({
        position: latlng,
        map: map
    });
    directionsService = new google.maps.DirectionsService();
    initialize2();
}

// Change by Rohit Borse on 22-02-2022
function loadMap() {
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.id = 'googleMaps';
    // New Key Commented                                                         
    script.src = 'https://maps.googleapis.com/maps/api/js?key=AIzaSyDiZZz17Yi4SdQHVTljjYAzNLq-_84GwRo&sensor=false&callback=initGoogle';
    //script.src = 'https://maps.googleapis.com/maps/api/js?key=AIzaSyBQgrXAd_grdNnCtbPTqoXWqxyM2vdhEU0&sensor=false&callback=initGoogle';
    document.body.appendChild(script);
}




function displayMap(imgLat, imgLon, inspDate) {
    $('#gmapView').show();
    $('#gmapDivRoadPath').hide();

    var point = new google.maps.LatLng(imgLat, imgLon);

    if (imgLat != 0 && imgLon != 0) {

        if (marker == null) {
            marker = new google.maps.Marker({ position: point, map: map, title: "", animation: google.maps.Animation.DROP });
        }
        else
            marker.setPosition(point);

        var descr = "<table style='background-color:#FFFFCC;border:1px;width:280px;'>" +
            "<tr class='row2'><td>Insp. Date </td><td>" + inspDate + "</td></tr>" +
            "<tr class='row2'><td>Latitude </td><td>" + imgLat + "</td></tr>" +
            "<tr class='row2'><td>Longitude </td><td>" + imgLon + "</td></tr>" +
            "</table>"

        infowindow.setContent(descr);

        infowindow.open(map, marker);
        map.panTo(point);
        map.setZoom(8);

    }
}


function initialize2() {
    directionsDisplay = new google.maps.DirectionsRenderer();
    var india = new google.maps.LatLng(21.7679, 78.8718);
    var mapOptions1 = {
        zoom: 8,
        center: india
    }
    map2 = new google.maps.Map(document.getElementById('gmapDivRoadPath'), mapOptions1);
    directionsDisplay.setMap(map2);
    console.info("initialize2 done");
    $('#gmapDivRoadPath').hide();

}

function calcRoute(latLongArr) {

    console.info("calcRoute call start");
    $('#gmapView').hide();
    $('#gmapDivRoadPath').show();

    var latLngArr = new Array();
    var arr = latLongArr.split("$$");
    for (i = 0; i < arr.length; i++) {
        var data = arr[i].split("@");
        latLngArr[i] = new google.maps.LatLng(data[0], data[1]);
    }

    var start = latLngArr[0];
    var end = latLngArr[1];

    var request = {
        origin: start,
        destination: end,
        travelMode: google.maps.TravelMode.WALKING
    };
    directionsService.route(request, function (response, status) {
        if (status == google.maps.DirectionsStatus.OK) {
            console.info("calcRoute status" + response);
            directionsDisplay.setDirections(response);
            map2.panTo(start);
            map2.setCenter(start);
            google.maps.event.trigger(map2, 'resize');

            var totalDistance = 0;
            var legs = response.routes[0].legs;
            for (var i = 0; i < legs.length; ++i) {
                totalDistance += legs[i].distance.value;
            }
            document.getElementById('spnDistanceGMap').innerHTML = "<b> Distance Traveled : " + totalDistance / 1000 + " Km </b>";


        } else {
            console.info("calcRoute status fail " + response + "   ............" + status);
        }
    });


}

function getStartEndLatLong() {
    var startLatLongStrArr = "";
    $.ajax({
        url: '/RoadwiseQualityDetails/RoadwiseQualityInformation/GetStartEndLatLong/',
        type: "GET",
        cache: false,
        beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        },
        error: function (xhr, status, error) {
            $.unblockUI();
            alert("Request can not be processed at this time,please try after some time!!!");
            return false;
        },
        data: { obsId: $("#ObservationId").val() },
        success: function (response) {
            startLatLongStrArr = response.Message;
            calcRoute(startLatLongStrArr);
            $.unblockUI();
        }
    });

    return startLatLongStrArr;
}


//-------------- Google Map Code Ends Here-------------------//

function closeCurrentDiv(currentDivId, currentDivName) {

    if (confirm("Do You Want To Close " + currentDivName + "?")) {

        // to get element by id
        var el = document.getElementById(currentDivId);
        el.style.display = 'none';
    }
}