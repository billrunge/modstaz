function jsonToTable(json, className) {

    let cols = Object.keys(json[0]);
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
        bodyRows += '</tr>';
    });
    return `<table class="${className}"><tr>${headerRow}</tr>${bodyRows}</table>`;
}