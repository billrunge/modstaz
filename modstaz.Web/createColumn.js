let storageAreaId;
loadCreateColumn();
getColumnTypeList();

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

function getColumnTypeList() {

    console.log("Getting Column Type List!");
    var request = new XMLHttpRequest();
    request.open('GET', `${apiBaseUrl}/api/GetColumnTypes`, true);

    request.send();

    request.onload = function () {
        var resp = this.response;
        let respObj = JSON.parse(resp);

        let html = `<select name="columnTypeId">`;

        for (let i = 0; i < respObj.length; i++){
            html += `<option value="${respObj[i].id}">${respObj[i].name}</option>`;
        }
        html += "</select>";
        document.getElementById('columnTypeSelector').innerHTML = html;
    };

    request.onerror = function () { };
}




// let i = `<select name="columnTypeId">
//         <option value="1">Yes/No</option>
//         <option value="2">Integer</option>
//         <option value="3">Decimal</option>
//         <option value="4">Small Text</option>
//       </select>`
// }