function columnsJsonToTable(json, className) {

    let cols;
    if (json[0] === undefined) {
        cols = [];
    } else {
        cols = Object.keys(json[0]);
    }
    let headerRow = '';
    let bodyRows = '';

    cols.map(function (col) {
        headerRow += `<th>${col}</th>`;
    });

    json.map(function (row) {
        bodyRows += '<tr>';
        cols.map(function (colName) {
            bodyRows += `<td>${row[colName]}</td>`;
        })
        if (row["IsEditable"] === true) {
            bodyRows += `<td><button onclick="deleteColumn(${row["Id"]})" type="button">Delete</button></td>`;
        } else {
            bodyRows += `<td></td>`;
        }

        bodyRows += '</tr>';
    });
    return `<table class="${className}"><tr>${headerRow}</tr>${bodyRows}</table>`;
}