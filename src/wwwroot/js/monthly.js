const getDynamicExpenses = (startDate, endDate) => {
    return new Promise((resolve, reject) => {
        $.ajax({
            method: 'GET',
            url: `/api/dynamic/${startDate.toString("yyyy-MM-dd")}/${endDate.toString("yyyy-MM-dd")}`,

            success: resolve,
            error: reject
        });
    });
}
const getStaticExpenses = (startDate, endDate) => {
    return new Promise((resolve, reject) => {
        $.ajax({
            method: 'GET',
            url: `/api/static/${startDate.toString("yyyy-MM-dd")}/${endDate.toString("yyyy-MM-dd")}`,

            success: resolve,
            error: reject
        });
    });
}

const formatter = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD'
});

const startDateInput = document.getElementById("start-date");
const endDateInput = document.getElementById("end-date");

const dynamicTable = new DataTable("#dynamic", {
    serverSide: true,
    columns: [
        { data: 'payee',  name: 'Payee',  render: (payee) =>  payee.name },
        { data: 'vendor', name: 'Vendor', render: (vendor) => vendor.name },
        { data: 'date',   name: 'Date',   render: (date) =>   (new Date(date)).toLocaleDateString() },
        { data: 'amount', name: 'Amount', render: (amt) =>    formatter.format(amt) },
        { data: 'split',  name: 'Split',  render: (_data, _type, row) => formatter.format(row.amount / 2), orderable: false }
    ],
    ajax: {
        url: '/api/dynamic',
        data: (data) => {
            return {
                draw:        data.draw,
                column:      data.columns[data.order[0].column].name,
                order:       data.order[0].dir.toUpperCase(),
                search:      data.search.value,
                dateStart:   startDateInput.value,
                dateEnd:     endDateInput.value,
                resultStart: data.start,
                resultCount: data.length
            }
        }
    }
});

// Reload data on dat input change
startDateInput.addEventListener("change", () => {
    if (startDateInput.value != "")
        dynamicTable.ajax.reload();
});
endDateInput.addEventListener("change", () => {
    if (endDateInput.value != "")
        dynamicTable.ajax.reload();
})