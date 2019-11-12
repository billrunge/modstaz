var apiBaseUrl = "http://localhost:7071";

function createInstance() {
    console.log("Creating instance!");
    var request = new XMLHttpRequest();
    request.open('GET', `${apiBaseUrl}/api/DatabaseSetup`, true);

    request.onload = function () {
        var resp = this.response;
        console.log(resp);
    };

    request.onerror = function () { };
    request.send();
}

function destroyInstance() {
    console.log("Destroying instance!");
    var request = new XMLHttpRequest();
    request.open('GET', `${apiBaseUrl}/api/DatabaseDestroy`, true);

    request.onload = function () {
        var resp = this.response;
        console.log(resp);
    };

    request.onerror = function () { };
    request.send();
}

function createField(form) {
    var storageAreaId = form.storageAreaId.value;
    var displayName = form.displayName.value;
    var columnTypeId = form.columnTypeId.value;
    console.log(`storage area id = ${storageAreaId}, display name = ${displayName}, columnTypeID = ${columnTypeId}`);

    console.log("Creating Field!");
    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/DatabaseDestroy`, true);

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