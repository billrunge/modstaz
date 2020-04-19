getViewColumns();

function getViewColumns(form) {
    let params = getGetParameters();
    let jwt = localStorage.getItem('JWT');

    if (params.ID != undefined) {
        let storageAreaId = params.ID[0];
        let viewId = params.ViewId[0];

        var request = new XMLHttpRequest();
        request.open('POST', `${apiBaseUrl}/api/GetViewColumns`, true);

        let data = {
            "StorageAreaId": storageAreaId,
            "ViewId": viewId,
            "JWT": jwt
        };

        request.send(JSON.stringify(data));

        request.onload = function () {
            let resp = this.response;
            if (resp == "Invalid JWT")
            {
                redirectToLogin();
            }
            var html = jsonToTable(JSON.parse(resp), "viewColumnsList");

            document.getElementById('viewColumns').innerHTML = html;
        };

        request.onerror = function () { };
    }
}

function jsonToTable(json, className) {

    let cols;
    let dateRegEx = /([0-9]+-[0-9]+-[0-9]+[T][0-9]+:[0-9]+:[0-9]+.[0-9]+)/;

    if (json[0] === undefined) {
        cols = [];
    } else {
        cols = Object.keys(json[0]);
    }

    let headerRow = '';
    let bodyRows = '';


    cols.map(function (col) {
        if (col != 'Id') {
            headerRow += `<th>${col}</th>`;
        }
    });

    json.map(function (row) {
        bodyRows += '<tr>';
        cols.map(function (colName) {
            if (colName == 'Name') {
                bodyRows += `<td><a href="../GetStorageArea.html?ID=${row["Id"]}">${row[colName]}</a></td>`;
            } else if (dateRegEx.test(row[colName])) {
                bodyRows += `<td>${moment(row[colName]).format("YYYY/MM/DD, h:mm:ss a")}</td>`;
            } else if (colName != 'Id') {
                bodyRows += `<td>${row[colName]}</td>`;
            } else {
                bodyRows += `<td></td>`;
            }
        })
        bodyRows += `<td><button onclick="deleteStorageArea(${row["Id"]})" type="button">Delete</button></td>`;
        bodyRows += '</tr>';
    });
    return `<table class="${className}"><tr>${headerRow}</tr>${bodyRows}</table>`;
}