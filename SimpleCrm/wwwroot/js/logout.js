window.addEventListener('beforeunload', function () {
    fetch('/Account/Checkout', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
    });
});
