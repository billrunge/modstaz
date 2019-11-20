getStorageAreaColumns();
insertGetColumnsLinks()

function getStorageAreaColumns(form) {
    let params = getGetParameters();

    if (params.ID != undefined) {
        let storageAreaId = params.ID[0];

        console.log(`storage area id = ${storageAreaId}`);

        console.log("Getting Storage Area Columns!");
        var request = new XMLHttpRequest();
        request.open('POST', `${apiBaseUrl}/api/GetStorageAreaColumns`, true);

        let data = {
            "StorageAreaId": storageAreaId
        };

        request.send(JSON.stringify(data));

        request.onload = function () {
            var html = columnsJsonToTable(JSON.parse(this.response), "storageAreaColumnsList");
            console.log(html);

            document.getElementById('storageAreaColumns').innerHTML = html;
        };

        request.onerror = function () { };
    }
}

function deleteColumn(columnId) {
    let params = getGetParameters();

    if (params.ID != undefined) {
        let storageAreaId = params.ID[0];

        var request = new XMLHttpRequest();
        request.open('POST', `${apiBaseUrl}/api/DeleteColumn`, true);

        let data = {
            "StorageAreaId": storageAreaId,
            "ColumnId": columnId
        };

        request.send(JSON.stringify(data));

        request.onload = function () {
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

        html += `<a href="/getStorageArea.html?ID=${storageAreaId}">Return to Storage Area</a><br>`;
        html += `<a href="/createColumn.html?ID=${storageAreaId}">Create Column</a><br>`;
        html += `<a href="/insertRow.html?ID=${storageAreaId}">Insert Row</a><br>`;
        html += `<a href="/getStorageAreas.html">Storage Areas</a><br>`;
        html += `<a href="/index.html">Home</a><br>`;
        document.getElementById('getColumnsButtons').innerHTML = html;

    }

}