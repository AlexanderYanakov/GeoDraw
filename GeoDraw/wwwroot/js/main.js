var map = undefined;
var isPolygon = false;

var markers = [];
var lines = [];
var rectangles = [];
var polygons = [];

// load map
window.onload = (event) => {
    map = L.map('map').setView([41.505, 42.09], 5);
    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 20,
    }).addTo(map);

    // showTile(lines)
    L.tileLayer.wms('http://localhost:8080/geoserver/FigureDb/wms', {
        layers: 'lines',
        format: 'image/png',
        transparent: true
    }).addTo(map);

    // showTile(markers)
    L.tileLayer.wms('http://localhost:8080/geoserver/FigureDb/wms', {
        layers: 'markers',
        format: 'image/png',
        transparent: true
    }).addTo(map);

    // showTile(rectangles)
    L.tileLayer.wms('http://localhost:8080/geoserver/FigureDb/wms', {
        layers: 'rectangles',
        format: 'image/png',
        transparent: true
    }).addTo(map);

    // showTile(polygons)
    L.tileLayer.wms('http://localhost:8080/geoserver/FigureDb/wms', {
        layers: 'polygons',
        format: 'image/png',
        transparent: true
    }).addTo(map);

    map.on('click', function (e) {
        var latLng = e.latlng;
        fetchDataFromBackend(latLng);
    });
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

// separateFunction
function separateLatLng(list) {
    var separatedLatLng = [];
    for (let i = 0; i < list.length; i++) {
        var a = list[i].getLatLng();
        separatedLatLng.push(a)
    }
    return separatedLatLng;
}

// separateFunction
function separateRectangleLatLng(list) {
    var separatedRectangleLatLng = [];
    for (let i = 0; i < list.length; i++) {
        var a = list[i].getLatLngs();
        separatedRectangleLatLng.push(a[0]);
    }
    return separatedRectangleLatLng;
}

// separateFunction
function separatePolygonLatLng(list) {

    var separatedPolygonLatLng = [];
    if (list !== undefined) {
        separatedPolygonLatLng = list.getLatLngs()
    }
    return separatedPolygonLatLng;
}

// separateFunction
function separateLineLatLng(list) {
    var separateLineLatLng = [];
    for (let i = 0; i < list.length; i++) {
        var latLngs = list[i].getLatLngs()
        var separatedLatLng = [];
        for (let j = 0; j < latLngs.length; j++) {
            separatedLatLng.push(latLngs[j]);
        }
        separateLineLatLng.push(separatedLatLng);
    }
    return separateLineLatLng;
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

// saveDataFunction(values in lists will be separated)
function saveData() {
    map.off('click');

    var figureData = {
        MarkerList: separateLatLng(markers),
        LineList: separateLineLatLng(lines),
        RectangleList: separateRectangleLatLng(rectangles),
        PolygonList: separatePolygonLatLng(polygon)
    };

    sendFiguresToBackend(figureData);
}

// sendDataToBackend
function sendFiguresToBackend(figureData) {
    var json = JSON.stringify(figureData);
    var backendURL = "http://localhost:5238/Home/CreateFigure";
    fetch(backendURL, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: json
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
function fetchDataFromBackend(latLng) {
    var latLngModel = {
        lat: latLng.lat,
        lng: latLng.lng
    };
    var backendURL = "http://localhost:5238/Home/Check";
    fetch(backendURL, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'LatLng-Model': JSON.stringify(latLngModel)
        },

    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            console.log('Data successfully fetched from the backend', data);
            var json = JSON.stringify(data, null, 2);
            L.popup()
                .setLatLng(latLng)
                .setContent('<pre>' + json + '</pre>')
                .openOn(map);
        })
        .catch(error => {
            console.error('There was a problem fetching data from the backend', error);
        });

}

// addMarkerFunction
function addMarker() {
    map.off('click');
    map.on('click', function (event) {
        var lon = event.latlng.lng;
        var lat = event.latlng.lat;
        markers.push(L.marker([lat, lon]).addTo(map));
        map.off("click");
    });
}

// addLineFunction
function addLine() {
    map.off('click');
    var lineClickedPoints = [];
    var drawnLines = [];

    map.on('click', function (event) {
        if (lineClickedPoints.length < 2) {
            var latLng = event.latlng;
            lineClickedPoints.push(L.circleMarker(latLng, { radius: 2, color: 'red', fillOpacity: 1 }).addTo(map));

            // Добавляем координаты в массив для создания линии
            drawnLines.push(latLng);

            if (drawnLines.length === 2) {
                lines.push(L.polyline(drawnLines, { color: 'black' }).addTo(map));
                // Удаляем маркеры после построения линии
                setTimeout(clearMarkers(lineClickedPoints), 1);
                map.off('click');
            }
        }
    });
}

// addRectangleFunction
function addRectangle() {
    map.off('click');
    var rectangleClickedPoints = [];
    var drawnRectanglePoints = [];

    map.on('click', function (event) {
        if (rectangleClickedPoints.length < 4) {
            var latLng = event.latlng;
            rectangleClickedPoints.push(L.circleMarker(latLng, { radius: 2, color: 'red', fillOpacity: 1 }).addTo(map));

            // Добавляем координаты в массив для создания полигона
            drawnRectanglePoints.push(latLng);

            if (rectangleClickedPoints.length === 4) {
                // Создаем полигон из 4 точек
                rectangles.push(L.polygon(drawnRectanglePoints, { color: 'black' }).addTo(map));

                setTimeout(clearMarkers(rectangleClickedPoints), 1);
                map.off('click');
            }
        }
    });
}




var isDrawingPolygon = false;
var polygon = undefined;
// addPolygonFunction
function addPolygon() {
    map.off('click');
    if (isDrawingPolygon === true) {
        var result = stopDrawingPolygon();
        polygons.push(result);
    }
    isDrawingPolygon = true;
    var polygonPoints = [];
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
