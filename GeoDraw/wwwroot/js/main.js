var map = undefined;
var isPolygon = false;

// load map
window.onload = (event) => {
    map = L.map('map').setView([41.505, 42.09], 5);
    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 20,
    }).addTo(map);
}

// stopDrawingPolygonFunction
function stopDrawingPolygon() {
    map.off('click');
    isDrawingPolygon = false;
    polygonPoints = [];
    map.eachLayer(function (layer) {
        if (layer instanceof L.CircleMarker) {
            map.removeLayer(layer);
        }
    });
    return polygon;
}

// clearMarkersFunction
function clearMarkers(clickedPoints) {
    for (var i = 0; i < clickedPoints.length; i++) {
        map.removeLayer(clickedPoints[i]);
    }
    clickedPoints = [];
}

// showPopupFunction
function showPopup() {
    const popup = document.getElementById("popup")
    popup.style.right = "10px";
    popup.style.top = "80px";
    popup.style.widows = "150px";
    popup.style.height = "50px";
}

// hidePopupFunction
function hidePopup() {
    const popup = document.getElementById("popup")
    popup.style.right = "-150px"
}

// addMarkerFunction
function addMarker() {
    map.on('click', function (event) {
        var lon = event.latlng.lng;
        var lat = event.latlng.lat;
        var marker = L.marker([lat, lon]).addTo(map);
        map.off("click");
        return marker;
    });

}

// addLineFunction
function addLine() {
    var clickedPoints = [];
    var drawnLines = [];

    map.on('click', function (event) {
        if (clickedPoints.length < 2) {
            var latLng = event.latlng;
            clickedPoints.push(L.circleMarker(latLng, { radius: 2, color: 'red', fillOpacity: 1 }).addTo(map));

            // Добавляем координаты в массив для создания линии
            drawnLines.push(latLng);

            // Если у нас уже есть как минимум две точки, рисуем линию
            if (drawnLines.length === 2) {
                var line = L.polyline(drawnLines, { color: 'black' }).addTo(map);
                // Удаляем маркеры и линию после построения линии
                setTimeout(clearMarkers(clickedPoints), 1);
                map.off('click');
                return line;
            }
        }
    });
}

// addRectangleFunction
function addRectangle() {
    var clickedPoints = [];
    var drawnLines = [];

    map.on('click', function (event) {
        if (clickedPoints.length < 4) {
            var latLng = event.latlng;
            clickedPoints.push(L.circleMarker(latLng, { radius: 2, color: 'red', fillOpacity: 1 }).addTo(map));

            // Добавляем координаты в массив для создания линий
            drawnLines.push(latLng);

            // Если у нас уже есть как минимум две точки, рисуем линии
            if (drawnLines.length >= 2) {
                var lastPoint = drawnLines[drawnLines.length - 2];
                var currentPoint = drawnLines[drawnLines.length - 1];
                L.polyline([lastPoint, currentPoint], { color: 'black' }).addTo(map);
            }

            if (clickedPoints.length === 4) {
                // Замыкаем квадрат линиями
                var firstPoint = clickedPoints[0];
                var lastPoint = clickedPoints[clickedPoints.length - 1];

                L.polyline([lastPoint.getLatLng(), firstPoint.getLatLng()], { color: 'black' }).addTo(map);
                drawnLines.push(drawnLines[0]);
                var rectangle = drawnLines;
                // Удаляем маркеры и линии после завершения фигуры
                setTimeout(clearMarkers(clickedPoints), 1);
                map.off('click');
                return rectangle;
            }
        }
    });


    //var geoServerLayer = L.tileLayer.wms('http://localhost:8080/geoserver/FigureDb/wms', {
    //    layers: 'lines',
    //    format: 'image/png',
    //    transparent: true
    //}).addTo(map);
    //var a = undefined;
}

var isDrawingPolygon = false;
var polygon = undefined;
// addPolygonFunction
function addPolygon() {
    if (isDrawingPolygon === true) {
        var result = stopDrawingPolygon();
        return result;
    }
    isDrawingPolygon = true;
    var polygonPoints = [];
    // Обработчик клика на карте
    map.on('click', function (event) {
        if (isDrawingPolygon) {
            var latLng = event.latlng;
            polygonPoints.push(latLng);

            L.circleMarker(latLng, { radius: 2, color: 'red', fillOpacity: 1 }).addTo(map);

            if (polygon) {
                map.removeLayer(polygon);
            }
            polygon = L.polygon(polygonPoints, { color: 'black' }).addTo(map);
        }
    });
}

