$(function () {
    $(".text_div").each(function () {
        $(this).click(function () {
            if ($(this).children('span').css("color") == "rgb(0, 0, 0)") {
                $(this).children('span').css("color", "#e76740");
                $(this).css("background", "#eee");
                $(this).children('img').css("display", "none");
                $(this).next('ul').css("display", "");
            } else
             if ($(this).children('span').css("color") == "rgb(231, 103, 64)") {
                $(this).children('span').css("color", "rgb(0, 0, 0)");
                $(this).children('img').css("display", "");
                 $(this).css("background", "");
                 $(this).next('ul').css("display", "none");
            }
        });
    });
})