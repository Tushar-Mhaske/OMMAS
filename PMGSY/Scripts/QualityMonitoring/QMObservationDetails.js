/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMObservationDetails.js
        * Description   :   Handles events, grids in Observation Details process
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/


this.imagePreview = function () {

    /* CONFIG */
    xOffset = 10;
    yOffset = 10;
    // these 2 variable determine popup's distance from the cursor
    // you might want to adjust to get the right result
    var Mx = 1000;// $(document).width();
    var My = 600;// $(document).height();

    //$("#preview").mouseover(function () {
    //    alert("hover : " + lat + "----" + long);
    //});

    /* END CONFIG */
    var callback = function (event, param) {
        var $img = $("#preview");

        // top-right corner coords' offset
        var trc_x = xOffset + $img.width();
        var trc_y = yOffset + $img.height();

        trc_x = Math.min(trc_x + event.pageX, Mx);
        trc_y = Math.min(trc_y + event.pageY, My);

        //alert("left: " + (trc_y - $img.height()) + "   Top " + (trc_x - $img.width()));

        $img
			.css("top", (trc_y - $img.height()) + "px")
			.css("left", (trc_x - $img.width()) + "px");


    };


    $("a.preview").hover(function (e) {

        Mx = $(this).offset().left + 400; // * 2;//600
        My = $(this).offset().top - 100; //600;

        //------------  Through Url split Latlongs & Href Link Part & Display Map as per appropriate LatLongs--------
        var arrLink = this.href.split("$$$");
        var lnkHref = arrLink[0];
        var latLong = arrLink[1];
        var inspDate = arrLink[2];

        var latLongArr = latLong.split("$$"); //0th is Latitude and 1st is Longitude
        if (latLongArr[0] != 0 && latLongArr[1] != 0) {
            displayMap(latLongArr[0], latLongArr[1], inspDate);
        }
        // ---------------------------------------------------------

        this.t = this.title;
        this.title = "";
        var c = (this.t != "") ? "<br/>" + this.t : "";
        $("body").append("<p id='preview'><img  style='height: 500px; width: 500px;' height='800' width='600' src='" + lnkHref + "' alt='Image Not Available' />" + c + "</p>");
        callback(e, 200);
        $("#preview").fadeIn("slow");
    },
		function () {
		    this.title = this.t;
		    $("#preview").remove();
		}
	)
    //.mousemove(callback);
};

latLongArr = "";
$(document).ready(function () {

    ListQualityFiles($("#QM_OBSERVATION_ID").val());

    // Load Google Map
    loadMap();


});


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
    var waypoints = [];
    var markers = [];

    // ---- Get Start and End LatLongs & display calculated route
    var latLongArrStartEnd = getStartEndLatLong();

    var arrStartEnd = latLongArrStartEnd.split("$$");
    var arrStartEndPoint = new Array();
    for (i = 0; i < arrStartEnd.length; i++) {
        var data = arrStartEnd[i].split("@");
        arrStartEndPoint[i] = new google.maps.LatLng(data[0], data[1]);
        //waypoints.push(arrStartEndPoint[i]);
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
        waypoints.push(latLongPoints[i]);
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

function getStartEndLatLong() {
    var startLatLongStrArr = "";
    $.ajax({
        url: '/QualityMonitoring/GetStartEndLatLong/',
        type: "GET",
        async: false,
        cache: false,
        beforeSend: function () {
            blockPage();
        },
        error: function (xhr, status, error) {
            unblockPage();
            alert("Request can not be processed at this time,please try after some time!!!");
            return false;
        },
        data: { obsId: $("#QM_OBSERVATION_ID").val() },
        success: function (response) {
            startLatLongStrArr = response.Message;
            //calcRoute(startLatLongStrArr);
            unblockPage();
        }
    });

    return startLatLongStrArr;
}

function getLatLong() {
    var latLongStrArr = "";
    $.ajax({
        url: '/QualityMonitoring/GetLatLong/',
        type: "GET",
        cache: false,
        async: false,
        data: { obsId: $("#QM_OBSERVATION_ID").val() },
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


//-------------- Google Map Code Ends Here-------------------//



function imageFormatter(cellvalue, options, rowObject) {
    var PictureURL = cellvalue.replace('/thumbnails', '');

    var arrLinkSrc = cellvalue.split("$$$");
    var lnkHrefSrc = arrLinkSrc[0];
    return " <a href='" + PictureURL + "' onclick='doNothing(); return false;' class='preview'><img style='height: 75px; width: 100px; border:solid 1px black' src='" + lnkHrefSrc + "' alt='Image not Available' title=''  /> </a>";
}

function doNothing() {
    return false;
}

function ListQualityFiles(obsId) {

    jQuery("#tbFilesList").jqGrid({
        url: '/QualityMonitoring/ListFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["Image", "Description", "Download", "Delete"],
        colModel: [
                    { name: 'image', index: 'image', width: 125, sortable: false, align: "center", formatter: imageFormatter, search: false, editable: false },
                    { name: 'Description', index: 'Description', width: 200, sortable: false, align: "center", valign: "center", },
                    { name: 'download', index: 'download', width: 80, sortable: false, align: 'center', editable: false, valign: "center", },
                    { name: 'delete', index: 'delete', width: 80, sortable: false, align: 'center', editable: false, valign: "center", hidden: (($("#IS_ATR_PAGE").val() == "Y") ? true : false) },
        ],
        postData: { "QM_OBSERVATION_ID": obsId },
        pager: jQuery('#dvFilesListPager'),
        rowNum: 30,
        viewrecords: true,
        pgbuttons: false,
        pgtext: null,
        recordtext: '{2} records found',
        caption: "Image Details",
        height: '300',
        sortname: 'Image',
        rownumbers: true,
        loadComplete: function () {
            imagePreview();
        },
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Session Timeout !!!");
                window.location.href = "/Login/LogIn";
            }
        }
    }); //end of grid    
}

function AnchorFormatter(cellvalue, options, rowObject) {
    var url = "/QualityMonitoring/DownloadFile/" + cellvalue;
    return "<a href='#' onclick=downloadFileFromAction('" + url + "'); return false;> <img style='height:16px;width:16px' height='16' width='16' border=0 src='../../Content/images/PDF.ico' /> </a>";
}

function downloadFileFromAction(paramurl) {

    window.location = paramurl;

}

function DownLoadImage(cellvalue) {

    //var url = "/QualityMonitoring/DownloadFile/" + cellvalue;
    //downloadFileFromAction(url);

    $.ajax({
        url: '/QualityMonitoring/DownloadFile/' + cellvalue,
        type: "GET",
        cache: false,
        async: false,

        success: function (response) {

            if (response.Error) {
                alert("File Not Exist")
            }
            else {

                var url = "/QualityMonitoring/DownloadFile/" + cellvalue;
                downloadFileFromAction(url);
            }
        }


    });



}

function DeleteFileDetails(cellvalue) {
    var arrayDetails = cellvalue.split('$');
    // changes by saurabh starts here
    if (arrayDetails.length > 5) {
        var FileID = arrayDetails[0];
        var ObservationID = arrayDetails[1];
        var FileName = arrayDetails[2] + "$" + arrayDetails[3];
        var Year = arrayDetails[2];
    }
    else {
        var FileID = arrayDetails[0];
        var ObservationID = arrayDetails[1];
        var FileName = arrayDetails[2];
    }
    // changes by saurabh ended here


    if (confirm("Are you sure you want to delete this image?")) {
        $.ajax({
            url: '/QualityMonitoring/DeleteinspectionImageFiles/',
            type: "POST",
            cache: false,
            async: false,
            data: { observationID: ObservationID, filename: FileName, DictionYear: Year, fileID: FileID, "__RequestVerificationToken": $('[name=__RequestVerificationToken]').val() },
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                if (response) {
                    alert("Image Deleted Successfully.");

                    unblockPage();


                    //jQuery("#tbMonitorsInspectionList").trigger('reloadGrid');

                    InspectionListGrid($("#FROM_MONTH").val(), $("#FROM_YEAR").val(), $("#TO_MONTH").val(), $("#TO_YEAR").val());  //calling function from QualityMonitorsLayout.js to relode list for img count 
                    $("#tbMonitorsInspectionList").jqGrid('setGridState', 'hidden');  //code to minimise open grid

                }
                else {
                    alert("Image could not be Deleted");
                    unblockPage();
                }
                $("#tbFilesList").jqGrid('setGridParam', { datatype: 'json' })
                  .trigger('reloadGrid', [{ page: 1 }]);
                //ListQualityFiles($("#QM_OBSERVATION_ID").val());
                //$("#tbMonitorsInspectionList").trigger('reloadGrid');
                //$("#divMonitorsInspectionDetails").trigger('reloadGrid');
            }


        });

    }
}

