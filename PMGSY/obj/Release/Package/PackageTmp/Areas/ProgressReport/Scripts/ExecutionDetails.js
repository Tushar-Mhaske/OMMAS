$(document).ready(function () {

    loadMap();

    //$("#btnDisplayRoadPath").click(function () {
    //    getStartEndLatLong();
    //});

    $('#hrefHeaderQMGrading').click(function () {
        collapseExpandTbl('hrefHeaderQMGrading', 'divContentQMGrading', 'spnHeaderQMGrading0', 'spnHeaderQMGrading1');
    });

});


function rotateImage(index) {
    $('#qmimage' + index).rotateLeft(90);
    $('#qmimage' + index).width('580px');
    $('#qmimage' + index).height('500px');
}



//-------------- Google Map Code Starts here-------------------//
var marker = null;
var map;
var infowindow = null;
var directionsDisplay;
var directionsService; // = new google.maps.DirectionsService();
var map2;
function initGoogle() {
    infowindow = new google.maps.InfoWindow();
    directionsService = new google.maps.DirectionsService();
    initialize2();
}

function loadMap() {
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.id = 'googleMaps';
    script.src = 'https://maps.googleapis.com/maps/api/js?key=AIzaSyDiZZz17Yi4SdQHVTljjYAzNLq-_84GwRo&sensor=false&callback=initGoogle';
    document.body.appendChild(script);
}




function displayMap(imgLat, imgLon, inspDate) {
    var point = new google.maps.LatLng(imgLat, imgLon);

    if (imgLat != 0 && imgLon != 0) {

        if (marker == null) {
            marker = new google.maps.Marker({ position: point, map: map2, title: "", animation: google.maps.Animation.DROP });
        }
        else
            marker.setPosition(point);

        var descr = "<table style='background-color:#FFFFCC;border:1px;width:280px;'>" +
						"<tr class='row2'><td>Insp. Date </td><td>" + inspDate + "</td></tr>" +
						"<tr class='row2'><td>Latitude </td><td>" + imgLat + "</td></tr>" +
						"<tr class='row2'><td>Longitude </td><td>" + imgLon + "</td></tr>" +
						"</table>"

        infowindow.setContent(descr);

        infowindow.open(map2, marker);
        map2.panTo(point);
    }
}


function initialize2() {
    directionsDisplay = new google.maps.DirectionsRenderer();
    //var waypoints = [];
    var markers = [];

    // ---- Get Start and End LatLongs & display calculated route
    var latLongArrStartEnd = getStartEndLatLong();

    var arrStartEnd = latLongArrStartEnd.split("$$");
    var arrStartEndPoint = new Array();
    for (i = 0; i < arrStartEnd.length; i++) {
        var data = arrStartEnd[i].split("@");
        arrStartEndPoint[i] = new google.maps.LatLng(data[0], data[1]);
    }
    var start = arrStartEndPoint[0];
    var end = arrStartEndPoint[1];
    // ----------------------------------------------------------

    // ---- Fetch Lat Longs for all images
    var latLongArr = getLatLong();
    var arr1 = latLongArr.split("$$");
    var arr2 = "";
    var latLongPoints = new Array();
    for (i = 0; i < arr1.length - 1; i++) {
        arr2 = arr1[i].split("@");
        latLongPoints[i] = new google.maps.LatLng(arr2[0], arr2[1]);
        markers.push(latLongPoints[i]);
    }
    // ----------------------------------------------------------

    var mapOptions1 = {
        zoom: 16,
        center: latLongPoints[0],
        mapTypeId: google.maps.MapTypeId.SATELLITE
    }
    map2 = new google.maps.Map(document.getElementById('gmapDivRoadPath'), mapOptions1);

    var bounds = new google.maps.LatLngBounds();
    for (i = 0; i < markers.length; i++) {
        var position = latLongPoints[i];
        bounds.extend(position);
        marker = new google.maps.Marker({
            position: position,
            map: map2
        });

        map2.fitBounds(bounds);
    }
    // Override our map zoom level once our fitBounds function runs (Make sure it only runs once)
    var boundsListener = google.maps.event.addListener((map2), 'bounds_changed', function (event) {
        this.setZoom(16);
        google.maps.event.removeListener(boundsListener);
    });

    //var polyline = new google.maps.Polyline({
    //    path: waypoints,
    //    strokeColor: "#003e74",
    //    strokeWeight: 2,
    //    strokeOpacity: 1.0
    //});

    //polyline.setMap(map2);
    directionsDisplay.setMap(map2);

    calcRoute(start, end);
    console.info("initialize2 done");
}

function calcRoute(start, end) {

    console.info("calcRoute call start");
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

//function initialize2() {
//    directionsDisplay = new google.maps.DirectionsRenderer();

//    //Fetch Lat Longs
//    var latLongArr = getLatLong();
//    getStartEndLatLong();           //display calculated Route
//    var arr1 = latLongArr.split("$$");
//    var arr2 = "";
//    var waypoints = new Array();
//    var markers = new Array();
//    var latLongPoints = new Array();
//    for (i = 0; i < arr1.length - 1; i++) {
//        arr2 = arr1[i].split("@");
//        latLongPoints[i] = new google.maps.LatLng(arr2[0], arr2[1]);
//        markers.push(latLongPoints[i]);
//        waypoints.push(latLongPoints[i]);
//    }

//    var mapOptions1 = {
//        zoom: 16,
//        center: latLongPoints[0],
//        mapTypeId: google.maps.MapTypeId.SATELLITE
//    }
//    map2 = new google.maps.Map(document.getElementById('gmapDivRoadPath'), mapOptions1);

//    var bounds = new google.maps.LatLngBounds();
//    for (i = 0; i < markers.length; i++) {
//        var position = latLongPoints[i];
//        bounds.extend(position);
//        marker = new google.maps.Marker({
//            position: position,
//            map: map2
//        });

//        map2.fitBounds(bounds);
//    }
//    // Override our map zoom level once our fitBounds function runs (Make sure it only runs once)
//    var boundsListener = google.maps.event.addListener((map2), 'bounds_changed', function (event) {
//        this.setZoom(16);
//        google.maps.event.removeListener(boundsListener);
//    });

//    var polyline = new google.maps.Polyline({
//        path: waypoints,
//        strokeColor: "#003e74",
//        strokeWeight: 2,
//        strokeOpacity: 1.0
//    });

//    polyline.setMap(map2);
//    console.info("initialize2 done");

//}

//function calcRoute(latLongArr) {

//    console.info("calcRoute call start");
//    //$('#gmapView').hide();
//    //$('#gmapDivRoadPath').show();

//    var latLngArr = new Array();
//    var arr = latLongArr.split("$$");
//    for (i = 0; i < arr.length; i++) {
//        var data = arr[i].split("@");
//        latLngArr[i] = new google.maps.LatLng(data[0], data[1]);
//    }

//    var start = latLngArr[0];
//    var end = latLngArr[1];

//    var request = {
//        origin: start,
//        destination: end,
//        travelMode: google.maps.TravelMode.WALKING
//    };
//    directionsService.route(request, function (response, status) {
//        if (status == google.maps.DirectionsStatus.OK) {
//            console.info("calcRoute status" + response);
//            directionsDisplay.setDirections(response);
//            map2.panTo(start);
//            map2.setCenter(start);
//            google.maps.event.trigger(map2, 'resize');

//            var totalDistance = 0;
//            var legs = response.routes[0].legs;
//            for (var i = 0; i < legs.length; ++i) {
//                totalDistance += legs[i].distance.value;
//            }
//            document.getElementById('spnDistanceGMap').innerHTML = "<b> Distance Traveled : " + totalDistance / 1000 + " Km </b>";


//        } else {
//            console.info("calcRoute status fail " + response + "   ............" + status);
//        }
//    });


//}

function getStartEndLatLong() {

    var startLatLongStrArr = "";
    $.ajax({
        url: '/ProgressReport/Progress/GetStartEndLatLong/',
        type: "GET",
        cache: false,
        beforeSend: function () {
            //blockPage();
        },
        error: function (xhr, status, error) {
            //unblockPage();
            alert("Request can not be processed at this time,please try after some time!!!");
            return false;
        },
        data: { obsId: $("#ObservationId").val() },
        success: function (response) {
            startLatLongStrArr = response.Message;
            //calcRoute(startLatLongStrArr);
            //unblockPage();
        }
    });

    return startLatLongStrArr;
}


function getLatLong() {
    var latLongStrArr = "";
    $.ajax({
        url: '/ProgressReport/Progress/GetLatLong/',
        type: "GET",
        cache: false,
        async: false,
        data: { proposalId: $("#ProposalCode").val() },
        beforeSend: function () {
            blockPage();
        },
        error: function (xhr, status, error) {
            unblockPage();
            alert("Request can not be processed at this time,please try after some time!!!");
            return false;
        },
        success: function (response) {
            latLongStrArr = response.Message;
            unblockPage();
        }
    });

    return latLongStrArr;
}

function doNothing() {
    return false;
}

//-------------- Google Map Code Ends Here-------------------//

