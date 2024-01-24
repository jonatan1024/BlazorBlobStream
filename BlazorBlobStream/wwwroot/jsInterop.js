export function getFileInfo(fileInputElement) {
    return Array.prototype.map.call(fileInputElement.files, (file, index) => ({
        name: file.name,
        lastModified: new Date(file.lastModified).toISOString(),
        size: file.size,
        contentType: file.type,
        index: index
    }));
}

export function getFile(fileInputElement, index) {
    return fileInputElement.files.item(index);
}