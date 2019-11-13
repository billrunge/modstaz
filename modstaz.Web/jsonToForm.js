function jsonToForm(json, className, functionName) {

    let html = "";

    json.map(function (row) {
        if (row['IsEditable'] === true) {
            html += `${row['DisplayName']}: <input type="text" name="${row['ID']}"><br>`;
        }
    });

    return `<form id="${className}">${html}
    <br>
    <button onclick="${functionName}(this.form)"type="button">Click</button>
    </form>`;
}

function jsonToFormEdit(json, className, functionName) {

    let html = "";

    json = json[0];
    console.log(json);

    for (var col in json){
        html += `${col}: <input type="text" value="${json[col]}" name="${col}"><br>`;
    }

    return `<form id="${className}">${html}
    <br>
    <button onclick="${functionName}(this.form)"type="button">Click</button>
    </form>`;
}