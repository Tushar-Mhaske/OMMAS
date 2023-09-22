
var markers = [];
var arr1 = [];
var latLongPoints = new Array();


var addressNew = "";
$(document).ready(function () {
  
    loadMap();

   

    $('#prev').click(function () {
        //alert('previous');
    });

    
    $('#next').click(function () {
        //if($(".slideshow a").css('display') == 'block')
        //{
        //    alert($(".slideshow a").href);
        //}
       
        //var arrLink = $(".slideshow a").href;
        //alert(arrLink);
        ////activeSlide
        ////alert($("#slideshowContainer").attr('class'));
        //if ($("#slideshowContainer a").css('display') == 'block') {
            
            
        //}
        //var imgLat = 18.565901900000;
        //var imgLon = 73.807121300000;
        //displayMap(imgLat, imgLon, "");
    });

    setTimeout(function () {
        $('.slideshow').cycle({
            fx: 'fade',
            pause: 1,
            prev: '#prev',
            next: '#next'
        }).cycle('pause');
    }, 200);
});


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
    if ($('#hdnIsLatLongAvailable').val() == 1) {
        
    }
}

function loadMap() {

    

    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.id = 'googleMaps';
    script.src = 'https://maps.googleapis.com/maps/api/js?key=AIzaSyDiZZz17Yi4SdQHVTljjYAzNLq-_84GwRo&sensor=false&callback=initGoogle';
    //if (document.hasChildNodes()) {
    //    document.removeChild(script);
    //}
    document.body.appendChild(script);

   
}

function displayMap(imgLat, imgLon, inspDate) {
    var address = null;
    var point = new google.maps.LatLng(imgLat, imgLon);

    if (imgLat != 0 && imgLon != 0) {

        if (marker == null) {
            marker = new google.maps.Marker({ position: point, map: map2, title: "", animation: google.maps.Animation.DROP });
        }
        else
            marker.setPosition(point);
        
        //var latlng = new google.maps.LatLng(imgLat,imgLon);
        var geocoder = geocoder = new google.maps.Geocoder();
        address = geocoder.geocode({ 'latLng': point }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                if (results[1]) {
                    //alert("Location: " + results[1].formatted_address);
                    addressNew = results[1].formatted_address;
                    
                }
            }
        });
       
        var descr = "<table style='background-color:#FFFFCC;border:1px;width:280px;'>" +
						//"<tr class='row2'><td>Insp. Date </td><td>" + inspDate + "</td></tr>" +
						"<tr class='row2'><td>Latitude </td><td>" + imgLat + "</td></tr>" +
						"<tr class='row2'><td>Longitude </td><td>" + imgLon + "</td></tr>" +
                        "<tr class='row2'><td>Address </td><td>" + addressNew + "</td></tr>" +
						"</table>"

        infowindow.setContent(descr);

        $("#spnLatToShow").html("Latitude : " + imgLat);
        $("#spnLongToShow").html("Longitude : " + imgLon);
        $("#spnLocation").html("Location : " + addressNew);

        infowindow.open(map2, marker);
        map2.panTo(point);
    }
}


function initialize2() {
    directionsDisplay = new google.maps.DirectionsRenderer();
    //var waypoints = [];
    
    var latLongArr = $("#LatLong").val();
    arr1 = latLongArr.split("$$$");
    //alert(arr1[0]);
    var arr2 = "";
    //for (i = 0; i < arr1.length; i++) {
    //    arr2 = arr1[i].split("$$");
    //    latLongPoints[i] = new google.maps.LatLng(arr2[0], arr2[1]);
    //    markers.push(latLongPoints[i]);
    //}

    arr2 = arr1[0].split("$$");
    latLongPoints[0] = new google.maps.LatLng(arr2[0], arr2[1]);
    markers.push(latLongPoints[0]);
    
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

    //calcRoute(start, end);
    console.info("initialize2 done");
}
