getStorageAreaColumns();

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