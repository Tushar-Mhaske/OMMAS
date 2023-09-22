$(document).ready(function () {

    

    GetTrackingDetailsforMap();

});

var tableCount = 0;
function initGoogle() {
    
    //var startlatlong = 18.534963;
    //var endlatlong = 73.810874;

    map_initialize();

}
function LoadMap()
{
    
    var script = document.createElement('script');
    
    script.type = 'text/javascript';
    script.id = 'googleMaps';
     script.src = 'https://maps.googleapis.com/maps/api/js?key=AIzaSyDiZZz17Yi4SdQHVTljjYAzNLq-_84GwRo&sensor=false&callback=initGoogle';
    
  
    

    document.body.appendChild(script);
}

function GetTrackingDetailsforMap() {

    

    var roadId = $('#proposalCode').val();
    $('#divTrackingDetais').show('slow');


    
    debugger;
    var responsiveHelper;
    var breakpointDefinition = {
        tablet: 1024,
        phone: 480
    };

    var tableElement = $('#tbltrackingList');
    //tableElement.dataTable().fnDestroy();
    //tableElement.DataTable({


    
    $("#tbltrackingList").dataTable().fnDestroy();
    
   $("#tbltrackingList").DataTable({



 
       
        ajax: {
            url: "/Execution/GetTrackingForExecution",
            type: "POST",
            data: { roadCode: roadId },
        },
        serverSide: true,
        sScrollY: "300px",
        sScrollX: '100%',
        bSortable: true,
        scrollCollapse: true,
        bPaginate: false,
        bLengthChange: false,
        bInfo: true,

  


        language: {
            searchPlaceholder: "Enter Location To Search",
        },

        columns: [
            { sDefaultContent: "", bSortable: false, sClass: "text-center" },
            { data: "EXEC_LOC_FLAG", bSortable: false, sClass: "text-center" },
            { data: "EXEC_LOC_LAT", bSortable: false, sClass: "text-center" },
            { data: "EXEC_LOC_LONG", bSortable: false, sClass: "text-center" },

        ]
        ,
        autoWidth: false,
        preDrawCallback: function () {
            // Initialize the responsive datatables helper once.
            
            if (!responsiveHelper) {
                responsiveHelper = new ResponsiveDatatablesHelper(tableElement, breakpointDefinition);
            }
        },
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            responsiveHelper.createExpandIcon(nRow);

            

            $("td:first", nRow).html(iDisplayIndex + 1);
            $("td:first", nRow).css('width', '10px');
            $('#tbltrackingList_filter').parent().siblings().html("<h5><b>Location Details:</b></h5>");
            $('#tbltrackingList_filter').parent().siblings().removeClass('col-xs-6').addClass('col-xs-5');
            $('#tbltrackingList_filter >label').css('margin-right', '-22px');
            $('#tbltrackingList_filter > label > input').removeClass('form-control input-sm').addClass('form-control input');
            return nRow;
        },
        rowCallback: function (nRow) {

            responsiveHelper.createExpandIcon(nRow);
            

        },
        drawCallback: function (oSettings) {
            responsiveHelper.respond();
            


        }
        , fnInitComplete: function (oSettings, json) {
            tableCount = ($('#tbltrackingList tr').length);// for header row

        }
    });

}

function map_initialize() {
    

    var latitude = [];
    var longitude = [];
    var latcolumnData = "";
    var longColumnData = "";
    //Avinash
    var Flag = [];
    var FlagData = " ";
    var i = 0;
    var j = 0;
    jQuery('#tbltrackingList > tbody > tr').each(function (index, value) {
        //  alert("text " + $('td:eq(2)', this).text());
        latcolumnData += $('td:eq(2)', this).text() + ",";
        // alert(latcolumnData);
    });
    jQuery('#tbltrackingList > tbody > tr').each(function (index, value) {
        //alert("text " + $('td:eq(3)', this).text());
        longColumnData += $('td:eq(3)', this).text() + ",";

    });

    //Avinash
    jQuery('#tbltrackingList > tbody > tr').each(function (index, value) {
        FlagData += $('td:eq(1)', this).text() + ",";

    });



    //Avinash
    Flag = FlagData.split(",");

    latitude = latcolumnData.split(",");
    longitude = longColumnData.split(",");

    //for (var i = 0; i < longitude.length-1; i++) {

    //    alert("split data:  lat " +latitude[i] +"  long"+longitude[i]);

    //}


    //Setting Latutide And Longitude
    var vStartLatlLong = parseFloat(latitude[0]);
    var vEndLatlLong = parseFloat(longitude[0]);

    var directionsDisplay = new google.maps.DirectionsRenderer();;

    google.maps.visualRefresh = false;

    //vStartLatlLong = 73.81062833333334;
    //vEndLatlLong = 18.534196666666666;

    var mapCenter = new google.maps.LatLng(vStartLatlLong, vEndLatlLong); //Google map Coordinates
    var map;


    var googleMapOptions =
   {
       center: mapCenter, // map center
       zoom: 12,          //zoom level, 0 = earth view to higher value
       panControl: true, //enable pan Control
       zoomControl: true, //enable zoom control
       zoomControlOptions: {
           style: google.maps.ZoomControlStyle.SMALL //zoom control size
       },
       scaleControl: true, // enable scale control
       mapTypeId: google.maps.MapTypeId.ROADMAP // google map type
   };

    var map = new google.maps.Map(document.getElementById("map_canvas"), googleMapOptions);
    directionsDisplay.setMap(map);

    //start marker
    var marker = new google.maps.Marker({
        position: mapCenter, //map Coordinates where user right clicked
        map: map,
        title: "Start Point",
        //icon: icon //custom pin icon
    });
    marker.setIcon('http://maps.google.com/mapfiles/ms/icons/green-dot.png');
    marker.setAnimation(google.maps.Animation.BOUNCE);

    var infowindow = new google.maps.InfoWindow({
        content: "<div class='infoDiv'><h5>" +
          "Latitude :" + parseFloat(latitude[0]) + "</h5> <div><h5>Longitude: " +
          parseFloat(longitude[0]) + "</h5></div></div>"
    });

    google.maps.event.addListener(marker, 'click', function () {
        infowindow.open(map, marker);
    });

    var directionsService = new google.maps.DirectionsService();

    var start = mapCenter; //declared as centre
    var end = new google.maps.LatLng(latitude[latitude.length - 2], longitude[longitude.length - 2]);
    var request = {
        origin: start,
        destination: end,
        travelMode: google.maps.TravelMode.DRIVING
    };
    directionsService.route(request, function (result, status) {
        // alert(status);
        if (status == 'OK') {
            directionsDisplay.setDirections(result);
        }
    });



    $.each(longitude, function (i, item) {
        var marker = new google.maps.Marker({
            'position': new google.maps.LatLng(parseFloat(latitude[i]), parseFloat(longitude[i])),
            'map': map,
            'title': ""
        });


        //Avinash
        if (Flag[i] == "CD Work") {
            marker.setIcon('http://maps.google.com/mapfiles/ms/icons/yellow-dot.png');     //CD Work

        }

        else if (Flag[i] == "Mid Point") {
            marker.setIcon('http://maps.google.com/mapfiles/ms/icons/red-dot.png');     //Intermediate Point
        }
        //

        //marker.setIcon('http://maps.google.com/mapfiles/ms/icons/red-dot.png');

        if (i == (latitude.length - 2)) {
            marker.setIcon('http://maps.google.com/mapfiles/ms/icons/blue-dot.png');
            marker.setAnimation(google.maps.Animation.BOUNCE);
        }

        var infowindow = new google.maps.InfoWindow({
            content: "<div class='infoDiv'><h5>" +
              "Latitude :" + parseFloat(latitude[i]) + "</h5> <div><h5>Longitude: " +
              parseFloat(longitude[i]) + "</h5></div></div>"
        });

        google.maps.event.addListener(marker, 'click', function () {
            infowindow.open(map, marker);
        });


    });

    google.maps.event.trigger(map, "resize");
}