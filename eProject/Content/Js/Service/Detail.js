$(document).ready(function() {
    $(".paymentPlanItem-childrenItem").click(function (e) {
        // Handle choose pld and set current
        $("#currentPayment-plan").text($(this).parents(".paymentPlanItem").find(".paymentPlanDetailItem-titleValue").text().trim());
        $("#currentPayment-planDetail").text($(this).find(".paymentPlanItem-childrenItemValue").text().trim());
        $(".registerBtn.disabled").removeClass("disabled");

        // Handle click register order
        const _this = this;
        $(".registerBtn:not(.disabled)").click(function (e) {
            e.preventDefault();
            window.location.href = `/Order/Prepare?serviceID=${$("#hiddenServiceID").data("service-id")}&paymentPlanDetailID=${$(_this).data("pld-id")}`;
        });

        // Toggle active
        $(".paymentPlanItem-childrenItem.active").removeClass("active");
        $(this).addClass("active");

        // Toggle parent active
        $(".paymentPlanItem-childrenItem:not(.active)").parents(".paymentPlanItem.active").removeClass("active")
        $(this).parents(".paymentPlanItem:not(.active)").addClass("active")
    })
})