// Create our number formatter.
const formatter = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD'
});

function getUserExpenses(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            method: 'GET',
            url: `/api/exp/${id}`,

            success: resolve,
            error: reject
        });
    });
}

getUserExpenses(1)
    .then(data => {
        document.getElementById('user1').innerHTML = formatter.format(data.balance);
    })
    .catch((xhr, status, err) => {
        document.getElementById('user1').innerHTML = '(Unable to get balance)';
    });

getUserExpenses(2)
    .then(data => {
        document.getElementById('user2').innerHTML = formatter.format(data.balance);
    })
    .catch((xhr, status, err) => {
        document.getElementById('user2').innerHTML = '(Unable to get balance)';
    });