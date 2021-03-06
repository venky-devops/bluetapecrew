﻿export class App {
    static getPrice(id) {
        $.get(`Products/GetStylePrice/${id}`,
            function (data) {
                $('#price').html(data);
            });
    }

    static subscribe() {
        const subscriberEmail = document.getElementById('subscriber-email');
        if (subscriberEmail.value) {
            fetch(`api/subscribe?emailAddress=${subscriberEmail.value}`, { method: 'POST' })
                .then((response) => {
                    if (response.ok) {
                        alert('Thank you for subscribing');
                    } else {
                        alert('There was an error subscribing');
                    }
                    subscriberEmail.value = '';
                });
        }
    }
}
