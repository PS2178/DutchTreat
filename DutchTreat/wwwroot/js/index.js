$(document).ready(function () { //load entire DOM into memory before executing code
var x = 0;
var s = "";


// jQuery uses a '$' as a selector with css class/id
console.log("Hello from the console");

var theForm = $("#theForm");

theForm.hide();

var button = $("#buyButton").on("click", function () {
    console.log("Buying Item");
});

var productInfo = $(".product-props li"); // slect container, and items
productInfo.on("click", function () { // add event listener on ALL ITEMS
       console.log("You clicked on " + $(this).text());
    });

    // lead with a '$' to indicate that this is a jquery object
    var $loginToggle = $("#loginToggle");
    var $popupForm = $(".popup-form");

    $loginToggle.on("click", function () {
        $popupForm.toggle(1000); // default animation, 1 sec
    });

}); // end jQuery. This helps with global scope