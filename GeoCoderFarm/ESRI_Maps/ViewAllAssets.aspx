<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewAllAssets.aspx.cs" Inherits="ESRI_Maps.ViewAllAssets" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>View All Assets</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=7, IE=9, IE=10" />
    <link rel="stylesheet" href="http://serverapi.arcgisonline.com/jsapi/arcgis/3.5/js/esri/css/esri.css" />
    <style>

    body 
    {
        margin:0px;
        padding: 0px;
    }

    #overlay 
    {
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background-color: #000;
        filter:alpha(opacity=50);
        opacity: 0.5;
        z-index: 10000;
    }

    #mapDiv 
    {
        width:1000px; 
        height:700px;
        border: 1px dashed black;
    }
    </style>

    <script type="text/javascript" src="http://serverapi.arcgisonline.com/jsapi/arcgis/3.5/"></script>
    <script type="text/javascript" src="js/jquery-1.8.2.min.js"></script>
</head>


<body>

    <div id="overlay"></div>
    <div id="mapDiv"></div>
    <br />
    <span id="msg" style="color:blue; font-size:x-large; font-weight:bold;"></span>

    <script type="text/javascript">

        $().ready(function myfunction()
        {
            dojo.require("esri.map");

            // Global vars
            var map, marker;
            var infoLookupTable = [];
            var xMax,yMax,xMin,yMin = 0;

            //
            // Map initialization
            function renderMap()
            {
                map = new esri.Map("mapDiv",
                {
                    center: new esri.geometry.Point(-98.35, 39.50), // center of USA
                    zoom: 10,
                    basemap: "streets"
                });

                map.on("load", function()
                {
                    map.infoWindow.resize(175, 300);
                    dojo.connect(map, "onClick", function () { map.infoWindow.hide(); });
                });

                // gets resolved LatLng list from Addresses list in the DB
                getWebServiveResponse();
            }


            //
            // Gets XML response from the Web Service
            function getWebServiveResponse()
            {
                $("#overlay").show();

                dojo.xhrGet({
                    url: "ArcGisService.svc/GetGeocodeFarmResponse", // web service URL
                    handleAs: "xml",
                    contentType: "application/xml; charset=utf-8",
                    load: function (response, args)
                    {
                        // parsing XML response to extract data from it
                        var responseText = response.documentElement.childNodes[0].nodeValue;

                        if (responseText.length > 0)
                        {
                            displayMarkers(responseText.split(";"));
                        }
                        else
                        {
                            alert("No results...");
                        }
                    },
                    error: function (error, args) { console.warn("error!", error); }
                });
            }

            //
            // Displays popup window above clicked Marker
            function showInfoWindow(event)
            {
                var xClick = event.graphic.geometry.x;
                var yClick = event.graphic.geometry.y;

                for (var i = 0; i < infoLookupTable.length; i++)
                {
                    if (xClick == infoLookupTable[i][0] && yClick == infoLookupTable[i][1])
                    {
                        var title = infoLookupTable[i][2];
                        var description = infoLookupTable[i][3];
                        var address = infoLookupTable[i][4];
                        var image = infoLookupTable[i][5];

                        var content = "<img src='images/" + image + "' /><br />";
                        content += "<span style='font-style:italic;'>" + description  + "</span><br /><br />";
                        content += "<span style='font-weight:bold;'>" + address + "</span><br />";

                        map.infoWindow.setTitle(title);
                        map.infoWindow.setContent(content);
                        map.infoWindow.show(new esri.geometry.Point(xClick, yClick));

                        break;
                    }
                }

                dojo.stopEvent(event); // stop Click event propagation
            }

            //
            // Displays Markers on the rendered Map
            function displayMarkers(responseArray)
            {
                if (responseArray.length > 0)
                {
                    var graphicLayer = new esri.layers.GraphicsLayer();
                    var iconSize = 22;
                    var icon1 = new esri.symbol.PictureMarkerSymbol({ "url": "/images/icons/telephone.png", "width": iconSize, "height": iconSize });
                    var icon2 = new esri.symbol.PictureMarkerSymbol({ "url": "/images/icons/edit.png", "width": iconSize, "height": iconSize });
                    var icon3 = new esri.symbol.PictureMarkerSymbol({ "url": "/images/icons/office_folders.png", "width": 26, "height": 26 });
                    var icon4 = new esri.symbol.PictureMarkerSymbol({ "url": "/images/icons/cloud_comment.png", "width": iconSize, "height": iconSize });
                    var icon5 = new esri.symbol.PictureMarkerSymbol({ "url": "/images/icons/male_user.png", "width": iconSize, "height": iconSize });

                    // assuming that the very first Asset has min/max {x, y}
                    var firstAsset = responseArray[0].split("|");
                    xMax = firstAsset[1]; // lat
                    xMin = firstAsset[1]; // lat
                    yMax = firstAsset[0]; // lng
                    yMin = firstAsset[0]; // lng

                    // AMOUNT OF REQUESTS USED AND REMAINING
                    // The very last item contains the most fresh data
                    // GeocodeFarm is 1 request behind (+/- 1)
                    var requestsData = responseArray[responseArray.length - 1].split("|");
                    var requestsUsed = parseInt(requestsData[6]) + 1;
                    var requestsRemaining = parseInt(requestsData[7]) - 1;
                    $("#msg").text(requestsUsed + " out of " + requestsRemaining + " geocoding requests being used today.");

                    // putting Markers onto the Map
                    for (var i = 0; i < responseArray.length; i++)
                    {
                        var assetData = responseArray[i].split("|");
                        var lng = assetData[0];
                        var lat = assetData[1];
                        var title = assetData[2];
                        var descr = assetData[3];
                        var address = assetData[4];
                        var image = assetData[5];

                        var icon;
                        if (i == 0)
                            icon = icon1;
                        if (i == 1)
                            icon = icon2;
                        if (i == 2)
                            icon = icon3;
                        if (i == 3)
                            icon = icon4;
                        if (i == 4)
                            icon = icon5;

                        graphic = new esri.Graphic(new esri.geometry.Point(lat, lng), icon);
                        graphicLayer.add(graphic);

                        // saving Assets data for further data retrieval and putting into a PopUp balloon
                        infoLookupTable.push([lat, lng, title, descr, address, image]);

                        // finding Lat/Lng extremes to zoom in/out Map to include all the Markers on it
                        findMinMaxLatLng(lat, lng);
                    }

                    map.addLayer(graphicLayer);
                    dojo.connect(graphicLayer, "onClick", showInfoWindow);
                    zoomToExtent();

                    $("#overlay").hide();
                }
                else
                {
                    alert("There are no markers to display...");
                }
            }

            //
            // Determines which X and Y are Min and Max
            // REMARK: sets GLOBAL variables!
            function findMinMaxLatLng(lat, lng)
            {
                if (Math.abs(lat) > Math.abs(xMin)) xMin = lat;
                if (Math.abs(lat) < Math.abs(xMax)) xMax = lat;
                if (lng > yMax) yMax = lng;
                if (lng < yMin) yMin = lng;
            }

            //
            // Zooming In/Out to Extent
            function zoomToExtent()
            {
                var extent = new esri.geometry.Extent(xMin, yMin, xMax, yMax, new esri.SpatialReference({ wkid: 4326 }));
                map.setExtent(extent, true);
            }

            // onLoad event handler
            dojo.ready(renderMap);

        });
    </script>
</body>
</html>