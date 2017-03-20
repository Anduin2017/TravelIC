//var winWidth = $(window).width();
//$("html").css("fontSize", (winWidth / 640) * 40 + "px");

//var browser={
//    versions:function(){
//        var u = navigator.userAgent, app = navigator.appVersion;
//        return {//移动终端浏览器版本信息
//            trident: u.indexOf('Trident') > -1, //IE内核
//            presto: u.indexOf('Presto') > -1, //opera内核
//            webKit: u.indexOf('AppleWebKit') > -1, //苹果、谷歌内核
//            gecko: u.indexOf('Gecko') > -1 && u.indexOf('KHTML') == -1, //火狐内核
//            mobile: !!u.match(/AppleWebKit.*Mobile.*/)||u.indexOf('iPad') > -1, //是否为移动终端
//            ios: !!u.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/), //ios终端
//            android: u.indexOf('Android') > -1 || u.indexOf('Linux') > -1, //android终端或者uc浏览器
//            iPhone: u.indexOf('iPhone') > -1, //是否为iPhone或者QQHD浏览器
//            iPad: u.indexOf('iPad') > -1, //是否iPad
//            webApp: u.indexOf('Safari') == -1 //是否web应该程序，没有头部与底部
//        };
//    }(),
//    language:(navigator.browserLanguage || navigator.language).toLowerCase()
//};
//
//var detectBrowser = function(name) {
//	if(navigator.userAgent.toLowerCase().indexOf(name) > -1) {
//	    return true;
//	} else {
//	    return false;
//	}
//};
//var width = parseInt(window.screen.width);
//var scale = width/640;
//var userScalable = 'no';
//if(detectBrowser("qq/")) userScalable = 'yes';
//browser.versions.iPad ? document.getElementById('viewport').setAttribute( 'content', 'width=640, user-scalable=no, initial-scale=1, minimum-scale=1, maximum-scale=1') : document.getElementById('viewport').setAttribute( 'content', 'target-densitydpi=device-dpi,width=640,user-scalable='+userScalable+',initial-scale=' + scale);



$(function () {
    $(".flag-panel").each(function () {
        var flagText = $(this).text(),
		textArry = flagText.split(","),
		texthtml = "";
        $.each(textArry, function (i) {
            texthtml += "<span class='pro-flag c-base border-base'>" + textArry[i] + "</span>"
        });
        $(this).html(texthtml);
    });

    //清空输入框
    (function () {
        var inputs = $('.order-list input.order-text');
        if (inputs.size() <= 0) { return }
        inputs.each(function () {
            var t = $(this);
            if (!t.val() == "") {
                t.parent().find("i").removeClass("icon-iconfont-xie").addClass("icon-iconfont-32pxchaxian")
            } else {
                t.parent().find("i").removeClass("icon-iconfont-32pxchaxian").addClass("icon-iconfont-xie");
            }
        });
        inputs.focus(function () {
            $(this).parent().find("i").removeClass("icon-iconfont-xie").addClass("icon-iconfont-32pxchaxian");
        }).blur(function () {
            if ($(this).val() == "") {
                $(this).parent().find("i").removeClass("icon-iconfont-32pxchaxian").addClass("icon-iconfont-xie");
            }
        }).keyup(function () {
            if ($(this).val() == "") {
                $(this).parent().find("i").removeClass("icon-iconfont-32pxchaxian").addClass("icon-iconfont-xie");
            } else {
                $(this).parent().find("i").removeClass("icon-iconfont-xie").addClass("icon-iconfont-32pxchaxian");
            }
        });

        $(".order-list").on('click', '.icon-iconfont-32pxchaxian', function () {
            $(this).parent().find('input.order-text').val('');
            $(this).removeClass("icon-iconfont-32pxchaxian").addClass("icon-iconfont-xie");
        });
    })();
    //tab 切换
    (function () {
        if ($('.tab-swiper').size() > 0 && $('.details-tab-panel').size() > 0) {
            var tab_swiper = new Swiper('.tab-swiper', {
                wrapperClass: 'details-triple',
                slideClass: 'swiper-li',
                slidesPerView: 4,
                onTap: function (swiper) {
                    var i = swiper.clickedIndex;
                    content_swiper.slideTo(i);
                }
            });
            var content_swiper = new Swiper('.details-tab-panel', {
                wrapperClass: 'ticket-details-tabs',
                slideClass: 'details-tab-item',
                autoHeight: true,
                onSlideChangeStart: function (swiper) {
                    var i = swiper.activeIndex;
                    $('.details-triple li').eq(i).addClass('active').siblings().removeClass('active');
                    tab_swiper.slideTo(i)
                }
            });
        }
    })();

    //	拖动返回按钮
    if ($("#back-bar").size() > 0) {
        var tar = '#back-bar';  //按钮id
        var $tar = $(tar);
        //		touch.on(tar, 'tap', function(ev){
        //			window.history.back(); 
        //		});
        var start = new Date();
        var end;
        $tar.on('touchstart', function () {
            start = new Date();
        });
        $tar.on('touchend', function () {
            end = new Date();
            if ((end - start) < 300) {
                window.history.back();
            }
        });
        if (touch) {

            touch.on(tar, 'touchstart', function (ev) {
                ev.preventDefault();
            });
            //var target = document.getElementById("target");
            var ww = $(window).width();
            var wh = $(window).height();
            var dx, dy;
            var dot_w = $(tar).width(); // 按钮宽度
            var dot_h = $(tar).height(); // 按钮高度
            var pos_x = ww * .05; // 按钮初始位置x
            var pos_y = wh * .05; // 按钮初始位置y

            touch.on(tar, 'drag', function (ev) {
                dx = dx || 0;
                dy = dy || 0;
                var offx = dx + ev.x + "px";
                var offy = dy + ev.y + "px";
                var adsorb = 8; //距离屏幕边缘的间距，会被吸附到边缘
                //检测是否到4个屏幕边缘
                if (parseFloat(offx) + dot_w - pos_x + dot_w >= ww - adsorb) {
                    offx = ww - dot_w - pos_x + "px";
                }
                if (parseFloat(offx) + pos_x <= adsorb) {
                    offx = -pos_x + "px";
                }
                if (parseFloat(offy) + 2 * dot_h >= wh - adsorb) {
                    offy = wh - dot_h - pos_y + "px";
                }
                if (parseFloat(offy) + pos_y <= adsorb) {
                    offy = -pos_y + "px";
                }
                this.style.webkitTransform = "translate3d(" + offx + "," + offy + ",0)";
            });

            touch.on(tar, 'dragend', function (ev) {
                dx += ev.x;
                dy += ev.y;
            });
        }

    }
    //轮播
    (function () {
        var default_swiper = $('.swiper-container');
        if (default_swiper.size() <= 0) return false;
        var comSwiper = new Swiper('.swiper-container', {
            pagination: '.swiper-pagination',
            loop: true,
            paginationClickable: true,
            autoplay: 3000,
            speed: 600,
            autoplayDisableOnInteraction: false //操作完后不禁止动画
        });
    })();

    //返回顶部
    $(window).scroll(function () {
        if ($(document).scrollTop() > $(window).height() / 4) {
            $('.gotop').fadeIn(100);
        };
        if ($(document).scrollTop() < $(window).height() / 4) {
            $('.gotop').fadeOut(100);
        };
    });
    $('.gotop').on("touchstart", function () {
        $('html,body').animate({ scrollTop: '0px' }, 300);
    });

    if ($('#tab').size() > 0) {
        tabs.init("tab");
    }
    // if($('#details-tab').size()>0) {
    // 	tab("details-tab");
    // }
    if ($('.number').size() > 0) {
        if ($("#route-list").length > 0) {
            var totalp = 0;
            $(".route-price").each(function () {
                var price = $(this).find("strong").text();
                totalp = operation.accAdd(totalprice, price);
            });
            $("#totalprice").text(totalp);
        }
        else {
            totalprice(1);
        }
        $(".number").numSpinner({
            min: 1,
            onChange: function (evl, value) {
                if ($("#route-list").length > 0) {
                    routetotalprice();
                }
                else {
                    totalprice(value);
                }
            }
        });
    }
    $("#mask").height($(document).height());
    $(".tips-wrapper").css("min-height", $(window).height());
    //touch.on(".page-list",'tap',function(){
    //	$(this).find("a").removeClass("prevent");
    //})
    //$("#mask").on("touchstart",function(){
    //	$(".search-item").removeClass("search-show");
    //	$("#list-tab").find("a").removeClass("active");
    //	$("#mask").hide();
    //}).on("touchend",function(){
    //	$(".common-list,.order-list,.goods-list").find("a").addClass("pointer");
    //});
    //$(".common-list,.order-list,.goods-list").on("touchend",function(){
    //	$(".common-list,.order-list,.goods-list").find("a").removeClass("pointer");
    //});
});

var tabs = {
    init: function (divid) {
        $("#" + divid).find("a").click(function (e) {
            var itmeId = $(this).attr("href");
            if (itmeId.substr(0, 1) == "#") {
                e.preventDefault();
            }
            $(itmeId).addClass('active').siblings().removeClass("active");
            $(this).parent().addClass('active').siblings().removeClass("active");
        });
    }
};

function totalprice(num) {
    var price = $("#price").text();
    $("#totalprice").text(operation.accMul(price, num));
}

function routetotalprice() {
    var totalprice = 0;
    $(".number").each(function () {
        var val = $(this).val();
        var price = $(this).parent().next().find("strong").text();
        totalprice = operation.accAdd(totalprice, operation.accMul(val, price));
    });
    $("#totalprice").text(totalprice);
}
//tab切换
function tab(id) {
    var touchObj = $("#" + id).find("a");
    $("#tab-panel").find(".details-tab-item:eq(0)").css("height", "auto");
    touch.on(touchObj, 'tap', function () {
        var index = $(this).parent().index(), divid = $(this).data("div");
        touchObj.removeClass("active");
        $(this).addClass("active");
        $("#tab-panel").css("margin-left", -(Math.round(index * 10000) / 100).toFixed(2) + '%').find(".details-tab-item").removeAttr("style");
        $("#" + divid).css("height", "auto");
    });
}
//四则运算
var operation = {
    accMul: function (arg1, arg2) {
        var m = 0, s1 = arg1.toString(), s2 = arg2.toString();
        try { m += s1.split(".")[1].length } catch (e) { }
        try { m += s2.split(".")[1].length } catch (e) { }
        return Number(s1.replace(".", "")) * Number(s2.replace(".", "")) / Math.pow(10, m);
    },
    accDiv: function (arg1, arg2) {
        var t1 = 0, t2 = 0, r1, r2;
        try { t1 = arg1.toString().split(".")[1].length } catch (e) { }
        try { t2 = arg2.toString().split(".")[1].length } catch (e) { }
        with (Math) {
            r1 = Number(arg1.toString().replace(".", ""));
            r2 = Number(arg2.toString().replace(".", ""));
            return (r1 / r2) * pow(10, t2 - t1);
        }
    },
    accAdd: function (arg1, arg2) {
        var r1, r2, m;
        try { r1 = arg1.toString().split(".")[1].length; } catch (e) { r1 = 0; }
        try { r2 = arg2.toString().split(".")[1].length; } catch (e) { r2 = 0; }
        m = Math.pow(10, Math.max(r1, r2));
        return (this.accMul(arg1, m) + this.accMul(arg2, m)) / m;
    },
    accSub: function (arg1, arg2) {
        var r1, r2, m, n;
        try {
            r1 = arg1.toString().split(".")[1].length;
        } catch (e) {
            r1 = 0;
        }
        try {
            r2 = arg2.toString().split(".")[1].length;
        } catch (e) {
            r2 = 0;
        }
        m = Math.pow(10, Math.max(r1, r2));
        //last modify by deeka
        //动态控制精度长度
        n = (r1 >= r2) ? r1 : r2;
        return ((this.accMul(arg2, m) - this.accMul(arg1, m)) / m).toFixed(n);
    }
};