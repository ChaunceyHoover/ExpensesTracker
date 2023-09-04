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
    bAutoWidth: false,
    columns: [
        { data: 'payee',  name: 'Payee',  render: (payee) =>  payee.name },
        { data: 'vendor', name: 'Vendor', render: (vendor) => vendor.name },
        { data: 'date',   name: 'Date',   render: (date) =>   (new Date(date)).toLocaleDateString() },
        { data: 'amount', name: 'Amount', render: (amt) =>    formatter.format(amt) },
        { data: 'split',  name: 'Split',  render: (_data, _type, row) => formatter.format(row.amount / 2), orderable: false }
    ],
    ajax: {
        url: '/api/split',
        data: (data) => {
            return {
                draw: data.draw,
                column: data.columns[data.order[0].column].name,
                order: data.order[0].dir.toUpperCase(),
                search: data.search.value,
                dateStart: startDateInput.value,
                dateEnd: endDateInput.value,
                resultStart: data.start,
                resultCount: data.length
            }
        }
    }
});
const loansTable = new DataTable("#loans", {
    serverSide: true,
    bAutoWidth: false,
    columns: [
        { data: 'payee',  name: 'Lendor', render: (payee) => payee.name },
        { data: 'vendor', name: 'Vendor', render: (vendor) => vendor.name },
        { data: 'date',   name: 'Date',   render: (date) => (new Date(date)).toLocaleDateString() },
        { data: 'amount', name: 'Amount', render: (amt) => formatter.format(amt) }
    ],
    ajax: {
        url: '/api/loans',
        data: (data) => {
            return {
                draw: data.draw,
                column: data.columns[data.order[0].column].name,
                order: data.order[0].dir.toUpperCase(),
                search: data.search.value,
                dateStart: startDateInput.value,
                dateEnd: endDateInput.value,
                resultStart: data.start,
                resultCount: data.length
            }
        }
    }
});
const staticTable = new DataTable("#static", {
    serverSide: true,
    bAutoWidth: false,
    columns: [
        { data: 'name', name: 'Bill' },
        { data: 'issueDate', name: 'Issued', render: (date) => (new Date(date)).toLocaleDateString() },
        { data: 'amount', name: 'Amount', render: (amt) => formatter.format(amt) },
        { data: 'amount', name: 'Split', render: (amt) => formatter.format(amt / 2) },
        {
            data: 'notes', name: 'Notes', render: (note, _, row) =>
                note ?? `${row.name} for ${row.startDate.toLocaleDateString()} to ${row.endDate.toLocaleDateString()}`
        }
    ],
    ajax: {
        url: '/api/static',
        data: (data) => {
            return {
                draw: data.draw,
                column: data.columns[data.order[0].column].name,
                order: data.order[0].dir.toUpperCase(),
                search: data.search.value,
                dateStart: startDateInput.value,
                dateEnd: endDateInput.value,
                resultStart: data.start,
                resultCount: data.length
            }
        }
    }
});

const dynamicRemoveBtn = $('#btn-remove-purchase');
const loansRemoveBtn = $('#btn-remove-loan');
const staticRemoveBtn = $('#btn-remove-expense');

const dynamicModalTitle = $('#dynamic-modal-title');
const dynamicModalBtn = $('#btn-add-dynamic-expense');
const dynamicPayeeInput = $('#dynamic-payee-select');
const dynamicVendorInput = $('#dynamic-vendor-select');
const dynamicDateInput = $('#dynamic-date');
const dynamicAmountInput = $('#dynamic-amount');
var dynamicMode = "None";

// Reload data on dat input change
startDateInput.addEventListener("change", () => {
    if (startDateInput.value != "") {
        dynamicTable.ajax.reload();
        loansTable.ajax.reload();
        staticTable.ajax.reload();
    }
});
endDateInput.addEventListener("change", () => {
    if (endDateInput.value != "") {
        dynamicTable.ajax.reload();
        loansTable.ajax.reload();
        staticTable.ajax.reload();
    }
});

// Add ability to total selected rows
$('#static, #loans, #dynamic').on('click', 'tr', function () {
    $(this).toggleClass('selected');

    // update button inputs
    const dynamicSelected = $('#dynamic tr.selected').length,
        loansSelected = $('#loans tr.selected').length,
        staticSelected = $('#static tr.selected').length;

    dynamicRemoveBtn.attr('disabled', dynamicSelected == 0);
    dynamicRemoveBtn.text(dynamicSelected > 1 ? 'Remove Purchases' : 'Remove Purchase');

    loansRemoveBtn.attr('disabled', loansSelected == 0);
    loansRemoveBtn.text(loansSelected > 1 ? `Remove ${loansSelected} Loans` : 'Remove Loan');

    staticRemoveBtn.attr('disabled', staticSelected == 0);
    staticRemoveBtn.text(staticSelected > 1 ? 'Remove Expenses' : 'Remove Expense');

    // selected table = $(this).parent().parent()[0].id
    // TODO: Calculate all selected rows and show in a <span> above/below the table
});

$('#btn-add-purchase').on('click', () => {
    dynamicModalTitle.text('Add Split Expense');
    dynamicMode = "Split";
});
$('#btn-add-loan').on('click', () => {
    dynamicModalTitle.text('Add Loan');
    dynamicMode = "Loan";
});

dynamicModalBtn.on('click', () => {
    var route = "";
    if (dynamicMode == "Split") {
        route = "/api/split"
    } else if (dynamicMode == "Loan") {
        route = "/api/loan";
    } else {
        return;
    }

    // Add the "is-invalid" class on for 3 seconds, then remove it
    const flashWarning = (input) => {
        input.toggleClass('is-invalid', true);
        setTimeout(() => input.toggleClass('is-invalid', false), 3000);
    }

    if (dynamicPayeeInput.val() == -1) {
        flashWarning(dynamicPayeeInput);
        return;
    } else if (dynamicVendorInput.val() == -1) {
        flashWarning(dynamicVendorInput);
        return;
    } else if (dynamicDateInput.val() == null || dynamicDateInput.val() == "") {
        flashWarning(dynamicDateInput);
        return;
    } else if (dynamicAmountInput.val() == null || dynamicAmountInput.val() == "" || dynamicAmountInput.val() <= 0) {
        flashWarning(dynamicAmountInput);
        return;
    }

    dynamicModalBtn.attr('disabled', true);

    $.ajax({
        method: 'POST',
        url: route,

        contentType: "application/json",
        data: JSON.stringify({
            payeeId: dynamicPayeeInput.val(),
            vendorId: dynamicVendorInput.val(),
            date: dynamicDateInput.val(),
            amount: dynamicAmountInput.val(),
            notes: $('#dynamic-notes').val()
        }),

        success: function () {
            alert('Successfully added purchase');
            if (dynamicMode == "Split")
                dynamicTable.ajax.reload();
            else
                loansTable.ajax.reload();
        },
        error: function (a, b, c) {
            alert('Unable to add purchase');
            console.log(a, b, c);
        },
        complete: function () {
            dynamicModalBtn.attr('disabled', false);
        }
    })
})

// Populate dynamic modal
// TODO: Get selectize.js (or similar) to make Vendors dynamic
$.ajax({
    method: 'GET',
    url: '/api/payees',

    success: (result) => {
        const payeeSelect = $('#dynamic-payee-select');

        $.each(result, (_, payee) => {
            payeeSelect
                .append($('<option>', { value: payee.id, text: payee.name }));
        });
        payeeSelect.attr('disabled', null);
    },
    error: () => {
        alert('Unable to populate payees for expenses modal');
    }
});
$.ajax({
    method: 'GET',
    url: '/api/vendors',

    success: (result) => {
        const vendorSelect = $('#dynamic-vendor-select');

        $.each(result, (_, vendor) => {
            vendorSelect
                .append($('<option>', { value: vendor.id, text: vendor.name }));
        });
        vendorSelect.attr('disabled', null);
    },
    error: () => {
        alert('Unable to populate vendors for expenses modal');
    }
});