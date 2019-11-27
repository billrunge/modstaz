getColumnsToEdit();
insertLinks();

function getColumnsToEdit() {
    let params = getGetParameters();
    let jwt = localStorage.getItem('JWT');
    if (params.StorageAreaId[0] != undefined && params.RowId[0] != undefined) {
        let storageAreaId = params.StorageAreaId[0];
        let rowId = params.RowId[0];

        var request = new XMLHttpRequest();
        request.open('POST', `${apiBaseUrl}/api/GetRow`, true);

        let data = {
            "StorageAreaId": storageAreaId,
            "RowId": rowId,
            "OnlyShowEditable": true,
            "JWT": jwt
        };

        request.send(JSON.stringify(data));

        request.onload = function () {
            var resp = this.response;
            if (resp == "Invalid JWT") {
                redirectToLogin();
            }
            var json = JSON.parse(resp);
            var html = jsonToFormEdit(json, "editRow", "editRow");

            document.getElementById('columnsToUpdate').innerHTML = html;
        };

        request.onerror = function () { };
    }
}

function editRow(form) {
    let params = getGetParameters();
    let jwt = localStorage.getItem('JWT');
    if (params.StorageAreaId[0] != undefined && params.RowId[0] != undefined) {
        
        let fields = "";
        let storageAreaId = params.StorageAreaId[0];
        let rowId = params.RowId[0];

        for (var i = 0; i < form.length - 1; i++) {
            fields += `"${form[i].name}": "${form[i].value}",`;
        }
        fields = fields.slice(0, -1);

        data = {
            "StorageAreaId": storageAreaId,
            "JWT": jwt,
            "RowFieldsArray": [{
                RowId: rowId,
                Fields: JSON.parse(`{${fields}}`)
            }]
        };

        var request = new XMLHttpRequest();
        request.open('POST', `${apiBaseUrl}/api/EditRows`, true);

        request.send(JSON.stringify(data));

        request.onload = function () {
            var resp = this.response;
            if (resp == "Invalid JWT") {
                redirectToLogin();
            }
        };

        request.onerror = function () { };
    }
}

function jsonToFormEdit(json, className, functionName) {

    let html = "";

    for (var i = 0; i < json.length; i++) {
        let col = json[i];

        switch (col.ColumnTypeId) {
            case 1:
                html += `${col.DisplayName}: <select type_id="${col.ColumnTypeId}" name="${col.ColumnId}">
                <option value="NULL" ${populateSelected(col.Value, "NULL")}>Unset</option>
                <option value="1" ${populateSelected(col.Value, 1)}>Yes</option>
                <option value="0" ${populateSelected(col.Value, 0)}>No</option>
              </select><br>`
                break;
            case 2:
                html += `${col.DisplayName}: <input type="number" type_id="${col.ColumnTypeId}" name="${col.ColumnId}" value="${col.Value}"><br>`;
                break;
            case 3:
                html += `${col.DisplayName}: <input type="number" type_id="${col.ColumnTypeId}" name="${col.ColumnId}" value="${col.Value}"><br>`;
                break;
            case 4:
                html += `${col.DisplayName}: <input type="text" type_id="${col.ColumnTypeId}" name="${col.ColumnId}" value="${col.Value}"><br>`;
                break;
            case 5:
                html += `${col.DisplayName}: <input type="text" type_id="${col.ColumnTypeId}" name="${col.ColumnId}" value="${col.Value}"><br>`;
                break;
            case 6:
                html += `${col.DisplayName}: <input type="text" type_id="${col.ColumnTypeId}" name="${col.ColumnId}" value="${col.Value}"><br>`;
                break;
            case 7:
                html += `${col.DisplayName}: <input type="text" type_id="${col.ColumnTypeId}" name="${col.ColumnId}" value="${col.Value}"><br>`;
                break;
            case 8:
                // html += `${col.DisplayName}: <input type="date" type_id="${col.ColumnTypeId}" name="${col.ColumnId}" value="${col.Value.format("yyyy-MM-dd")}"><br>`;
                html += `${col.DisplayName}: <input type="date" type_id="${col.ColumnTypeId}" name="${col.ColumnId}" value="${moment(col.Value, "M/D/YYYY hh:mm:ss a").format("YYYY-MM-DD")}"><br>`;

                break;                
        }
    }

    return `<form id="${className}">${html}
    <br>
    <button onclick="${functionName}(this.form)"type="button">Save</button>
    </form>`;
}

function populateSelected(input, option){
    if (input == "NULL" && option == "NULL"){
        return "selected"
    } else if (input == "True" && option == 1){
        return "selected"
    } else if (input == "False" && option == 0){
        return "selected"
    } else {
        return "";
    }
}

function insertLinks() {
    let params = getGetParameters();

    if (params.StorageAreaId != undefined) {
        let storageAreaId = params.StorageAreaId[0];
        let html = "";

        html += `<a href="/createColumn.html?ID=${storageAreaId}">Create Column</a><br>`;
        html += `<a href="/getStorageArea.html?ID=${storageAreaId}">Return to Storage Area</a><br>`;
        html += `<a href="/getStorageAreas.html">Storage Areas</a><br>`;
        html += `<a href="/index.html">Home</a><br>`;
        document.getElementById("insertLinks").innerHTML = html;

    }
}