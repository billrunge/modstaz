getStorageArea();
insertButtons();

function getStorageArea() {

    let params = getGetParameters();

    if (params.ID != undefined) {
        console.log(params.ID[0]);
        let storageAreaId = params.ID[0];
        console.log(`storage area id = ${storageAreaId}`);

        console.log("Getting Storage Areas!");
        var request = new XMLHttpRequest();
        request.open('POST', `${apiBaseUrl}/api/GetStorageArea`, true);

        let data = {
            "StorageAreaId": storageAreaId
        };

        request.send(JSON.stringify(data));

        request.onload = function () {
            var html = jsonToTable(JSON.parse(this.response), "storageAreaList");
            console.log(html);

            document.getElementById('storageAreaItems').innerHTML = html;
        };

        request.onerror = function () { };

    } else {
        window.location.replace("../getStorageAreas.html");
    }
}

function insertButtons() {
    let params = getGetParameters();

    if (params.ID != undefined) {
        let storageAreaId = params.ID[0];
        let html = "";

        html += `<a href="/getStorageAreas.html">Storage Areas</a><br>`;
        html += `<a href="/insertRow.html?ID=${storageAreaId}">Insert Row</a><br>`;
        html += `<a href="/createColumn.html?ID=${storageAreaId}">Create Column</a><br>`;
        html += `<a href="/index.html">Home</a><br>`;
        document.getElementById('storageAreaButtons').innerHTML = html;

    }



}