let initialDueAmount = parseInt($("#InitialDueAmount").val());
let discount = parseFloat($('#Discount').val());
let VATCost = parseInt($('#VatCost').val());
let amountPaid = parseInt($('#AmountPaid').val());

// Get charge dola
function getFullCharge() {
    const callChargeInputs = document.querySelectorAll('.callChargeInput');
    let fullCharge = 0;
    callChargeInputs.forEach(item => {
        fullCharge += parseInt(item.dataset.charge) * item.value;
    })

    return fullCharge;
}


//
function applyCallCharge() {
    const charge = getFullCharge() / 100;

    const newInitialDueAmount = initialDueAmount + charge;

    const afterDueAmount = newInitialDueAmount * (1 - discount);

    const vatCost = afterDueAmount * 0.1224;

    $("#InitialDueAmount").val(newInitialDueAmount);
    $('#VatCost').val(vatCost)
    $('#AmountPaid').val(afterDueAmount + vatCost);

    // Add class changed
    $("#InitialDueAmount").addClass('changed');
    $("#VatCost").addClass('changed');
    $("#AmountPaid").addClass('changed');
}


// Listen event when call charge change
$('.callChargeInput').on({
    change: function (e) {
        applyCallCharge();
    }
})

//
function showApplyCallCharge() {

}

// Listen event when Amount change
$("#InitialDueAmount").on({
    focus: function (e) {
        if ($(this).hasClass('changed')) $(this).removeClass('changed')
    },
    change: function (e) {
        initialDueAmount = parseInt($(this).val());
        applyCallCharge();
    }
})
$("#AmountPaid").on({
    focus: function (e) {
        if ($(this).hasClass('changed')) $(this).removeClass('changed')
    },
    change: function (e) {
        amountPaid = parseInt($(this).val());
        applyCallCharge();
    }
})
$("#VatCost").on({
    focus: function (e) {
        if ($(this).hasClass('changed')) $(this).removeClass('changed')
    },
    change: function (e) {
        VATCost = parseInt($(this).val());
        applyCallCharge();
    }
})