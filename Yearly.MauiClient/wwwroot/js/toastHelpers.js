window.toastError = (message = "") => {
    Toastify({
        text: message,
        duration: 3000,
        close: true,
        gravity: "bottom", // `top` or `bottom`
        position: "center", // `left`, `center` or `right`
        stopOnFocus: true, // Prevents dismissing of toast on hover
        style: {
            background: "var(--clr-danger)",
        },
        offset: {
            x: 0, // horizontal axis - can be a number or a string indicating unity. eg: '2em'
            y: 20 // vertical axis - can be a number or a string indicating unity. eg: '2em'
        },
    }).showToast();
}

window.toastSuccess = (message = "") => {
    Toastify({
        text: message,
        duration: 3000,
        close: true,
        gravity: "bottom", // `top` or `bottom`
        position: "center", // `left`, `center` or `right`
        stopOnFocus: true, // Prevents dismissing of toast on hover
        style: {
            background: "var(--clr-success)",
        },
        offset: {
            x: 0, // horizontal axis - can be a number or a string indicating unity. eg: '2em'
            y: 20 // vertical axis - can be a number or a string indicating unity. eg: '2em'
        },
    }).showToast();
}

window.toastInformation = (message = "", duration = 3000) => {
    Toastify({
        text: message,
        duration: duration, //-1 = permanent
        close: true,
        gravity: "bottom", // `top` or `bottom`
        position: "center", // `left`, `center` or `right`
        stopOnFocus: true, // Prevents dismissing of toast on hover
        style: {
            background: "var(--clr-primary)",
        },
        offset: {
            x: 0, // horizontal axis - can be a number or a string indicating unity. eg: '2em'
            y: 20 // vertical axis - can be a number or a string indicating unity. eg: '2em'
        },
    }).showToast();
}