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