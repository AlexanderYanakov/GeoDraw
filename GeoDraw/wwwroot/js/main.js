var map = undefined;
var isPolygon = false;

var markers = [];
var lines = [];
var rectangles = [];
var polygons = [];



var test = undefined;
var test1 = undefined;

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

function separateLatLng(list) {
    var separatedLatLng = [];
    for (let i = 0; i < list.length; i++) {
        separatedLatLng.push(list[i].getLatLng())
    }
    return separatedLatLng;
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

function saveData() {
    map.off('click');
    
    var figureData = {
        markerList: separateLatLng(markers),
        lineList: separateLatLng(lines),
        rectangleList: separateLatLng(rectangles),
        polygonList: separateLatLng(polygon)
    };

    sendFiguresToBackend(figureData);
}

function sendFiguresToBackend(figureData) {
    var a = JSON.stringify(figureData);
    var backendURL = "http://localhost:5238/Home/CreateFigure";
    fetch(backendURL, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: a
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            console.log('Figures were successfully sent to the backend', data);
        })
        .catch(error => {
            console.error('There was a problem sending figures to the backend', error);
        });

}

// addMarkerFunction
function addMarker() {
    map.on('click', function (event) {
        var lon = event.latlng.lng;
        var lat = event.latlng.lat;
        markers.push(L.marker([lat, lon]).addTo(map));
        map.off("click");
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
                lines.push(L.polyline(drawnLines, { color: 'black' }).addTo(map));
                // Удаляем маркеры после построения линии
                setTimeout(clearMarkers(clickedPoints), 1);
                map.off('click');
            }
        }
    });
}

// addRectangleFunction
function addRectangle() {
    var clickedPoints = [];
    var drawnRectangleLines = [];

    map.on('click', function (event) {
        if (clickedPoints.length < 4) {
            var latLng = event.latlng;
            clickedPoints.push(L.circleMarker(latLng, { radius: 2, color: 'red', fillOpacity: 1 }).addTo(map));

            // Добавляем координаты в массив для создания линий
            drawnRectangleLines.push(latLng);

            // Если у нас уже есть как минимум две точки, рисуем линии
            if (drawnRectangleLines.length >= 2) {
                var lastPoint = drawnRectangleLines[drawnRectangleLines.length - 2];
                var currentPoint = drawnRectangleLines[drawnRectangleLines.length - 1];
                L.polyline([lastPoint, currentPoint], { color: 'black' }).addTo(map);
            }

            if (clickedPoints.length === 4) {
                // Замыкаем квадрат линиями
                var firstPoint = clickedPoints[0];
                var lastPoint = clickedPoints[clickedPoints.length - 1];

                L.polyline([lastPoint.getLatLng(), firstPoint.getLatLng()], { color: 'black' }).addTo(map);

                drawnRectangleLines.push(drawnRectangleLines[0]);
                rectangles.push(drawnRectangleLines);
                // Удаляем маркеры после завершения фигуры
                setTimeout(clearMarkers(clickedPoints), 1);
                map.off('click');
            }
        }
    });


    //L.tileLayer.wms('http://localhost:8080/geoserver/FigureDb/wms', {
    //     layers: 'lines',
    //     format: 'image/png',
    //     transparent: true
    // }).addTo(map);
    // var a = undefined;
}

var isDrawingPolygon = false;
var polygon = undefined;
// addPolygonFunction
function addPolygon() {
    if (isDrawingPolygon === true) {
        var result = stopDrawingPolygon();
        polygons.push(result);
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
