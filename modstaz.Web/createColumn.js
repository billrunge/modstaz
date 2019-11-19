let storageAreaId;
loadCreateColumn();

function createColumn(form) {
    var displayName = form.displayName.value;
    var columnTypeId = form.columnTypeId.value;
    console.log(`storage area id = ${storageAreaId}, display name = ${displayName}, columnTypeID = ${columnTypeId}`);

    console.log("Creating Field!");
    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/CreateColumns`, true);

    let data = {
        "StorageAreaID": storageAreaId,
        "ColumnArray": [
            {
                ColumnTypeID: columnTypeId,
                DisplayName: displayName
            }
        ],

    };

    request.send(JSON.stringify(data));

    request.onload = function () {
        var resp = this.response;
        console.log(resp);
    };

    request.onerror = function () { };
}

function loadCreateColumn() {
    let params = getGetParameters();

    if (params.ID != undefined) {
        storageAreaId = params.ID[0];

    } else {
        window.location.replace("../getStorageAreas.html");
    }
    insertCreateColumnLinks()

}

function insertCreateColumnLinks() {
    let params = getGetParameters();

    if (params.ID != undefined) {
        let storageAreaId = params.ID[0];
        let html = "";

        html += `<a href="/getStorageArea.html?ID=${storageAreaId}">Return to Storage Area</a><br>`;
        html += `<a href="/insertRow.html?ID=${storageAreaId}">Insert Row</a><br>`;
        html += `<a href="/index.html">Home</a><br>`;
        document.getElementById('createColumnLinks').innerHTML = html;

    }

};