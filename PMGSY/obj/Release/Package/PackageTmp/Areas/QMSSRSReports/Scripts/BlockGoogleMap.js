$.getScript("https://maps.google.com/maps/api/js?sensor=true&region=nz&async=2&callback=MapApiLoaded", function () { });
$(document).ready(function () {
    //alert("Ready");

    $('select').css({ 'width': 100 });
    $("#btnView").click(function () {
        var state = $("#ddlState").val();
        var district = $("#ddlDistrict").val();
        var block = $("#ddlBlock").val();
        if (state == 0 || district == 0 || block == 0) {
            alert("Select State, District and Block.");
        }
        else {

            init();
        }
    });
    $("#ddlState").change(function () {
        var state = $(this).val();
        $("#ddlDistrict").empty();
        $("#ddlBlock").empty().append('<option value="0">Select Block</option>');


        $.post("/QMSSRSReports/QMSSRSReports/DistrictBlockList/", { state: state, district: 0 }, function (data) {
            var optionData = "";
            var selected = "";
            $.each(data, function (key, option) {
                selected = "";
                if (option.Selected == true) {
                    selected = 'selected="selected"';
                }
                optionData = optionData + '<option ' + selected + ' value="' + option.Value + '">' + option.Text + '</option>';
            });
            $("#ddlDistrict").append(optionData);
        });
    });

    $("#ddlDistrict").change(function () {
        var district = $(this).val();
        $("#ddlBlock").empty();



        $.post("/QMSSRSReports/QMSSRSReports/DistrictBlockList/", { state: 0, district: district }, function (data) {
            var optionData = "";
            var selected = "";
            $.each(data, function (key, option) {
                selected = "";
                if (option.Selected == true) {
                    selected = 'selected="selected"';
                }
                optionData = optionData + '<option ' + selected + ' value="' + option.Value + '">' + option.Text + '</option>';
            });
            $("#ddlBlock").append(optionData);
        });
    });
});

function init() {

    var state = $("#ddlState").val();
    var district = $("#ddlDistrict").val();
    var block = $("#ddlBlock").val();

    $.post("/QMSSRSReports/QMSSRSReports/GoogleMapshow/", { state: state, district: district, block: block }, function onSuccess(blockJSonData) {
        if (blockJSonData.length == 0) {
            alert("No road list available having Geo Tagging Data.");
        }
        var CenterLattitude = blockJSonData[0].GeoPosition[0].Lattitude;
        var CenterLongitude = blockJSonData[0].GeoPosition[0].Longitude;
        var mapDiv = document.getElementById('map-canvas');
        var myLatLng = new google.maps.LatLng(CenterLattitude, CenterLongitude);
        var mapOptions = {
            center: myLatLng,
            zoom: 12,
            mapTypeControl: true,
            zoomControl: false,
            scaleControl: false,
            streetViewControl: false,
            mapTypeId: google.maps.MapTypeId.SATELLITE
        }

        var map = new google.maps.Map(mapDiv, mapOptions);
        setMarkers(map, blockJSonData);
        RoadListGrid(blockJSonData);

    });



    // setMarkers(map, blockJSon);
}
function getIcon(color) {
    return MapIconMaker.createMarkerIcon({ width: 20, height: 34, primaryColor: color, cornercolor: color });
}
function setMarkers(map, blockJSon) {

    // var myJSON = '[{"Title":"foo","Icon":"bar","Position":[{"Lattitude":"28.613939","Longitude":"77.209021"},{"Lattitude":"28.535516","Longitude":"77.391026"},{"Lattitude":"28.533516","Longitude":"77.399026"}]}]';

    var title;
    var pinIcon
    var marker;
    var pinColor;
    var roadPhotoLatLong;
    var InspDate;
    var PhotoTakenTime;
    var content;


    $.each(blockJSon, function (key, value) {

        //InspDate = value.InspDate;
        pinColor = value.ColorCode;
        //title = value.RoadName;
        pinIcon = new google.maps.MarkerImage("http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=%E2%80%A2|" + pinColor, new google.maps.Size(21, 34), new google.maps.Point(0, 0), new google.maps.Point(10, 34));


        $("#SpanColorCode").css('background-color', "#" + value.ColorCode);



        var infowindow = new google.maps.InfoWindow();
        $.each(value.GeoPosition, function (key, LatLongValue) {
            //   alert(LatLongValue.monitor);
            content = "<table><tr><td colspan='2' width='200' align='center'><b><U>" + value.RoadName + "</U></b></td></tr>" +
                "<tr>" +
                "<td  width='100' align='center'><img width='100' height='100' src='" + LatLongValue.PhotoURLThumb + "' />" +
                "<br />" + LatLongValue.Description +
                "</td>" +
                 "<td    align='left' valign='top' nowrap>" + LatLongValue.monitor +
                    "<br /><b>Grade: </b>" + LatLongValue.Grade +
                    "<br /><b>Status: </b>" + LatLongValue.RoadStatus +
                    "<br /><b>Inspection Date: </b>" + LatLongValue.InspDate +
                     "<br /> <b>Lattitude: </b>" + LatLongValue.Lattitude +
                     "<br /> <b>Longitude: </b>" + LatLongValue.Longitude +
                    "<br /><a target='_blank' href='" + LatLongValue.PhotoURL + "'>Download Photo</a>" +
                 "</td></tr>" +
                "</table>";


            roadPhotoLatLong = new google.maps.LatLng(LatLongValue.Lattitude, LatLongValue.Longitude);

            marker = new google.maps.Marker({ position: roadPhotoLatLong, map: map, icon: pinIcon });


            google.maps.event.addListener(marker, 'click', (function (marker, content, infowindow) {
                return function () {
                    infowindow.setContent(content);
                    infowindow.open(map, marker);
                };
            })(marker, content, infowindow));

        });

    });


}





//Lattitude":26.234030113555,"Longitude":87.513197455555
function loadPaths(gmap, RoadColorCode, StartLattitude, StartLongitude, EndLattitude, EndLongitude) {
    if (StartLattitude != 0 && StartLongitude != 0 && EndLattitude != 0 && EndLongitude != 0) {
        var latlngbounds = new google.maps.LatLngBounds(),
            infoWindow = new google.maps.InfoWindow(),
            pathPoints = [],
            index = 0,
            positions = [
                { latitude: StartLattitude, longitude: StartLongitude },
                { latitude: EndLattitude, longitude: EndLongitude }
            ];
        positions.reverse();
        $.each(positions, function (k, v) {
            var myLatlng = new google.maps.LatLng(v.latitude, v.longitude);
            pathPoints.push(myLatlng);
            index++;
        });

        // Intialize the Path Array
        var path = new google.maps.MVCArray();

        // Intialise the Direction Service
        var service = new google.maps.DirectionsService();


        // Set the Path Stroke Color
        var poly = new google.maps.Polyline({
            map: gmap,
            strokeColor: RoadColorCode
        });

        // Draw the path for this vehicle
        // We compute the path between each point to follow the road
        for (var i = 0; i < pathPoints.length; i++) {
            // If it's not the last point
            if ((i + 1) < pathPoints.length) {
                var src = pathPoints[i];
                var des = pathPoints[i + 1];

                // We had the starting point to the poly path
                path.push(src);

                // We compute the path between the 2 points
                service.route({
                    origin: src,
                    destination: des,
                    travelMode: google.maps.DirectionsTravelMode.DRIVING,
                    unitSystem: google.maps.UnitSystem.IMPERIAL
                }, function (result, status) {
                    if (status == google.maps.DirectionsStatus.OK) {
                        // We add the new computed points
                        for (var i = 0, len = result.routes[0].overview_path.length; i < len; i++) {
                            path.push(result.routes[0].overview_path[i]);
                        }
                    }
                });
            }
        }

        // Set the path of the polyline to draw it
        poly.setPath(path);
    }

}
//google.maps.event.addDomListener(window, 'load', init);



//Road List 
function RoadListGrid(BlockJsonData) {


    $("#jqGrid").jqGrid('GridUnload');
    $("#jqGrid").jqGrid({ //set your grid id
        data: BlockJsonData, //insert data from the data object we created above
        datatype: 'local',
        width: 600, //specify width; optional
        height: 380, //specify width; optional
        colNames: ['Road Name', 'Package', 'Year', 'Length', 'Status', 'Stipulated/Actual Completed Date', 'Color'], //define column names
        colModel: [
        { name: 'RoadName', index: 'RoadName', width: 220 },
        { name: 'Package', index: 'Package', width: 60 },
        { name: 'SYear', index: 'SYear', width: 70 },
        { name: 'RoadLength', index: 'RoadLength', width: 40 },
        { name: 'RoadStatus', index: 'RoadStatus', width: 70 },
        { name: 'StipulatedCompletedDate', index: 'StipulatedCompletedDate', width: 80 },
        { name: 'ColorCode', index: 'ColorCode', width: 70, align: "center", editable: true, formatter: SetColorFormatter }

        ], //define column models
        rownumbers: true,
        pager: '#pager', //set your pager div id
        sortname: 'id', //the column according to which data is to be sorted; optional
        viewrecords: true, //if true, displays the total number of records, etc. as: "View X to Y out of Z” optional
        sortorder: "asc", //sort order; optional
        caption: "List of Block Wise work Inspected by Monitors using QMS Mobile Application" //title of grid
    });

}//Schedule Road Grid Ends Here
function SetColorFormatter(cellValue, options, rowObject) {
    //  alert (cellValue);
    return '<span style="display:block;background-image:none;margin-right:-2px;margin-left:-2px;height:14px;padding:4px;background: #' + cellValue + ';">' + cellValue + '</span>';
}