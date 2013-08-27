<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewAsset.aspx.cs" Inherits="ESRI_Maps.ViewAsset" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Popup Map</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=7, IE=9, IE=10" />
    <link rel="stylesheet" href="http://serverapi.arcgisonline.com/jsapi/arcgis/3.5/js/esri/css/esri.css" />
    <style>
    html, body, #mapDiv 
    {
        padding: 0;
        margin: 0;
        height: 100%;
    }
    </style>

    <script type="text/javascript" src="http://serverapi.arcgisonline.com/jsapi/arcgis/3.5/"></script>
    <script type="text/javascript">

        dojo.require("esri.map");

        //
        // Map initialization
        function init()
        {
            var lat = document.getElementById("hdnLat").value;
            var lng = document.getElementById("hdnLng").value;

            var point = new esri.geometry.Point(lng, lat);
            var marker = new esri.symbol.PictureMarkerSymbol({ "url": "http://static.arcgis.com/images/Symbols/Basic/RedShinyPin.png", "width": 28, "height": 28 });
            var map = new esri.Map("map", { center: point, zoom: 11, basemap: "streets" });
            var graphic = new esri.Graphic(point, marker);
            var layer = new esri.layers.GraphicsLayer();

            layer.add(graphic);
            map.addLayer(layer);
        }

        dojo.ready(init); // executed after all DOM objects are loaded
    </script>
</head>


<body>

    <div id="map" style="width:700px; height:650px;">
    </div>

    <form id="frmPopupMap" runat="server">
        <asp:HiddenField ID="hdnLat" EnableViewState="false" runat="server" />
        <asp:HiddenField ID="hdnLng" EnableViewState="false" runat="server" />    
    </form>
</body>
</html>
