//cuadro de dialogo de una libreria JS.
function arrayBufferToBase64(buffer) {
    // https://stackoverflow.com/a/9458996
    var binary = '';
    var bytes = new Uint8Array(buffer);
    var len = bytes.byteLength;
    for (var i = 0; i < len; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return window.btoa(binary);
}

//guardar archivos en la maquina
function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

  
//cuadro de dialogo para confirmar de una libreria JS.
function CustomConfirm(titulo, mensaje, tipo) {
    return new Promise((resolve) => {
        Swal.fire({
            title: titulo,
            text: mensaje,
            type: tipo,
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Confirmar'
        }).then((result) => {
            if (result.value) {
                resolve(true);
            } else {
                resolve(false);
            }
        });
    });
}
 
//dar foco sobre un objeto html
function focusById(elementId) {
    var element = document.getElementById(elementId);
    if (element) {
        element.focus();
    }
}
//seleccionar el objeto por el id en html
function SelectById(elementId) {
    var element = document.getElementById(elementId);
    if (element) {
        element.select();
    }
}

//si esta inactivo deslogueamos al usuario
function timerInactivo(dotnetHelper) {
    var timer;
    document.onmousemove = resetTimer;
    document.onkeypress = resetTimer;

    function resetTimer() {
        clearTimeout(timer);
        timer = setTimeout(logout, 900000); //15 minutos en mili segundos
    }

    //invoca la funcion logout de c#
    function logout() {
        dotnetHelper.invokeMethodAsync("Logout");
    }
}
 