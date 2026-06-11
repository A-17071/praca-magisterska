/**
 * Triggers a browser file download with the given content.
 * @param {string} filename - suggested filename for the download
 * @param {string} content  - text content of the file
 * @param {string} mimeType - MIME type, e.g. "text/csv;charset=utf-8;"
 */
window.downloadFile = function (filename, content, mimeType) {
    const blob = new Blob([content], { type: mimeType });
    const url  = URL.createObjectURL(blob);
    const a    = document.createElement('a');
    a.href     = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
};