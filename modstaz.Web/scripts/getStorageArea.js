isUserAuthenticated();
getStorageArea();
insertButtons();

function getStorageArea() {

    let params = getGetParameters();
    let jwt = localStorage.getItem('JWT');

    if (params.ID != undefined) {
        let storageAreaId = params.ID[0];
        var request = new XMLHttpRequest();
        request.open('POST', `${apiBaseUrl}/api/GetStorageArea`, true);

        let data = {
            "StorageAreaId": storageAreaId,
            "JWT": jwt
        };

        request.send(JSON.stringify(data));

        request.onload = function () {
            let resp = this.response;
            if (resp == "Invalid JWT") {
                redirectToLogin();
            }
            var html = storageAreaJsonToTable(JSON.parse(resp), "storageAreaList", storageAreaId);

            document.getElementById('storageAreaItems').innerHTML = html;
        };

        request.onerror = function () { };

    } else {

        window.location.replace("../getStorageAreas.html");
    }
}

function deleteRow(rowId) {
    let params = getGetParameters();
    let jwt = localStorage.getItem('JWT');

    if (params.ID != undefined) {
        let storageAreaId = params.ID[0];
        var request = new XMLHttpRequest();
        request.open('POST', `${apiBaseUrl}/api/DeleteRows`, true);

        let data = {
            "StorageAreaId": storageAreaId,
            "RowIds": [rowId],
            "JWT": jwt
        };

        request.send(JSON.stringify(data));

        request.onload = function () {
            let resp = this.response;
            if (resp == "Invalid JWT") {
                redirectToLogin();
            }
            getStorageArea();
            insertButtons();
        };

        request.onerror = function () { };

    } else {
        window.location.replace("../getStorageAreas.html");
    }




}

function storageAreaJsonToTable(json, className, storageAreaId) {
    let cols;
    let dateRegEx = /([0-9]+-[0-9]+-[0-9]+[T][0-9]+:[0-9]+:[0-9]+.[0-9]+)/;


    if (json[0] === undefined) {
        cols = [];
    } else {
        cols = Object.keys(json[0]);
    }

    let headerRow = '';
    let bodyRows = '';

    headerRow += `<th></th>`;

    cols.map(function (col) {
        headerRow += `<th>${col}</th>`;
    });

    headerRow += `<th></th>`;


    json.map(function (row) {
        bodyRows += '<tr>';
        bodyRows += `<td><a href="/editRow.html?StorageAreaId=${storageAreaId}&RowId=${row["Id"]}">Edit</a></td>`;
        cols.map(function (colName) {
            if (dateRegEx.test(row[colName])) {
                bodyRows += `<td>${moment(row[colName]).format("YYYY/MM/DD, h:mm:ss a")}</td>`;
            } else {

                bodyRows += `<td>${row[colName]}</td>`;
            }
        })

        bodyRows += `<td><button onclick="deleteRow(${row["Id"]})" type="button">Delete</button></td>`;
        bodyRows += '</tr>';
    });
    return `<table class="${className}"><tr>${headerRow}</tr>${bodyRows}</table>`;
}


function insertButtons() {
    let params = getGetParameters();

    if (params.ID != undefined) {
        let storageAreaId = params.ID[0];
        let html = "";
        html += `<a href="/insertRow.html?ID=${storageAreaId}">Insert Row</a><br>`;
        html += `<a href="/getStorageAreaColumns.html?ID=${storageAreaId}">Edit Columns</a><br>`;
        html += `<a href="/getStorageAreas.html">Storage Areas</a><br>`;
        html += `<a href="/index.html">Home</a><br>`;
        document.getElementById('storageAreaButtons').innerHTML = html;

    }

}