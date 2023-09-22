$(document).ready(function () {
    //alert("Ready");
    $("#btnUpdate").click(function () {

        alert("Lat: " + $("#lat").val() + "         Long:" + $("#lng").val());
    });


});

$(function () {
    var addresspicker = $("#addresspicker").addresspicker({
        componentsFilter: 'country:IN'
    });
    var addresspickerMap = $("#addresspicker_map").addresspicker({
        regionBias: "in",
        updateCallback: showCallback,
        mapOptions: {
            zoom: 15,
            center: new google.maps.LatLng(28.636171330905086, 77.10258197784424),
            scrollwheel: true,
            mapTypeId: google.maps.MapTypeId.SATELLITE,
            streetViewControl: false
        },
        elements: {
            map: "#map",
            lat: "#lat",
            lng: "#lng",

        }
    });

    var gmarker = addresspickerMap.addresspicker("marker");
    gmarker.setVisible(true);
    addresspickerMap.addresspicker("updatePosition");


   

});

function showCallback(geocodeResult, parsedGeocodeResult) {
    $('#callback_result').text(JSON.stringify(parsedGeocodeResult, null, 4));
}