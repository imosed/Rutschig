function shorten(url, pin, expiration) {
    fetch('Shorten/Create',
        {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({
                url: url,
                pin: pin.trim() !== '' ? pin : null,
                expiration: expiration.trim() !== '' ? new Date(expiration) : null
            })
        }
    )
}