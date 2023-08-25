const XlsxTemplate = require('xlsx-template');

module.exports = function (callback, byteString, metadata) {

    const parsedBytes = byteString.split('-').map(e => parseInt(`0x${e}`))
    const buffer = Buffer.from(parsedBytes);

    var t = new XlsxTemplate(buffer);

    t.sheets.forEach(sheet => {
        t.substitute(sheet.id, metadata);
    });

    var newData = t.generate({
        base64: true
    });

    callback(null, newData);
}
