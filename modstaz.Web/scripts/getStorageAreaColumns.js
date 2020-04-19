isUserAuthenticated();
getStorageAreaColumns();
insertGetColumnsLinks();

function getStorageAreaColumns(form) {
    let params = getGetParameters();
    let jwt = localStorage.getItem('JWT');

    if (params.ID != undefined) {
        let storageAreaId = params.ID[0];

        var request = new XMLHttpRequest();
        request.open('POST', `${apiBaseUrl}/api/GetStorageAreaColumns`, true);

        let data = {
            "StorageAreaId": storageAreaId,
            "JWT": jwt
        };

        request.send(JSON.stringify(data));

        request.onload = function () {
            let resp = this.response;
            if (resp == "Invalid JWT")
            {
                redirectToLogin();
            }
            var html = columnsJsonToTable(JSON.parse(resp), "storageAreaColumnsList");

            document.getElementById('storageAreaColumns').innerHTML = html;
        };

        request.onerror = function () { };
    }
}

function deleteColumn(columnId) {
    let params = getGetParameters();
    let jwt = localStorage.getItem('JWT');

    if (params.ID != undefined) {
        let storageAreaId = params.ID[0];

        var request = new XMLHttpRequest();
        request.open('POST', `${apiBaseUrl}/api/DeleteColumn`, true);

        let data = {
            "StorageAreaId": storageAreaId,
            "ColumnId": columnId,
            "JWT": jwt
        };

        request.send(JSON.stringify(data));

        request.onload = function () {
            let resp = this.response;
            if (resp == "Invalid JWT")
            {
                redirectToLogin();
            }
            getStorageAreaColumns();
        };

        request.onerror = function () { };
    }

}

function insertGetColumnsLinks() {
    let params = getGetParameters();

    if (params.ID != undefined) {
        let storageAreaId = params.ID[0];
        let html = "";

        html += `<a href="/createColumn.html?ID=${storageAreaId}">Create Column</a><br>`;
        html += `<a href="/getStorageArea.html?ID=${storageAreaId}">Return to Storage Area</a><br>`;
        html += `<a href="/insertRow.html?ID=${storageAreaId}">Insert Row</a><br>`;
        html += `<a href="/getStorageAreas.html">Storage Areas</a><br>`;
        html += `<a href="/index.html">Home</a><br>`;
        document.getElementById('getColumnsButtons').innerHTML = html;

    }

}