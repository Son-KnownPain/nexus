$(document).ready(function() {
    $(".paymentPlanItem-childrenItem").click(function (e) {
        $(".paymentPlanItem-childrenItem.active").removeClass("active");
        $(this).addClass("active");

        // Parent
        $(".paymentPlanItem-childrenItem:not(.active)").parents(".paymentPlanItem.active").removeClass("active")
        $(this).parents(".paymentPlanItem:not(.active)").addClass("active")
    })
})