/**
 * Created by jf on 2015/9/11.
 * Modified by bear on 2016/9/7.
 */
$(function () {
    var winH = $(window).height();
    var supportTouch = function(){
        try {
            document.createEvent("TouchEvent");
            return true;
        } catch (e) {
            return false;
        }
    }();
    var pageManager = {
        $container: $('#container'),
        _pageStack: [],
        _configs: [],
        _pageAppend: function(){},
        _defaultPage: null,
        _pageIndex: 1,
        setDefault: function (defaultPage) {
            this._defaultPage = this._find('name', defaultPage);
            return this;
        },
        setPageAppend: function (pageAppend) {
            this._pageAppend = pageAppend;
            return this;
        },
        init: function () {
            var self = this;

            $(window).on('hashchange', function () {
                var state = history.state || {};
                var url = location.hash.indexOf('#') === 0 ? location.hash : '#';
                var page = self._find('url', url) || self._defaultPage;
                if (state._pageIndex <= self._pageIndex || self._findInStack(url)) {
                    self._back(page);
                } else {
                    self._go(page);
                }
            });

            if (history.state && history.state._pageIndex) {
                this._pageIndex = history.state._pageIndex;
            }

            this._pageIndex--;

            var url = location.hash.indexOf('#') === 0 ? location.hash : '#';
            var page = self._find('url', url) || self._defaultPage;
            this._go(page);
            return this;
        },
        push: function (config) {
            this._configs.push(config);
            return this;
        },
        go: function (to) {
            var config = this._find('name', to);
            if (!config) {
                return;
            }
            location.hash = config.url;
        },
        _go: function (config) {
            this._pageIndex ++;

            history.replaceState && history.replaceState({_pageIndex: this._pageIndex}, '', location.href);

            var html = $(config.template).html();
            var $html = $(html).addClass('slideIn').addClass(config.name);
            $html.on('animationend webkitAnimationEnd', function(){
                $html.removeClass('slideIn').addClass('js_show');
            });
            this.$container.append($html);
            this._pageAppend.call(this, $html);
            this._pageStack.push({
                config: config,
                dom: $html
            });

            if (!config.isBind) {
                this._bind(config);
            }

            return this;
        },
        back: function () {
            history.back();
        },
        _back: function (config) {
            this._pageIndex --;

            var stack = this._pageStack.pop();
            if (!stack) {
                return;
            }

            var url = location.hash.indexOf('#') === 0 ? location.hash : '#';
            var found = this._findInStack(url);
            if (!found) {
                var html = $(config.template).html();
                var $html = $(html).css('opacity', 1).addClass(config.name);
                $html.insertBefore(stack.dom);

                if (!config.isBind) {
                    this._bind(config);
                }

                this._pageStack.push({
                    config: config,
                    dom: $html
                });
            }

            stack.dom.addClass('slideOut').on('animationend', function () {
                stack.dom.remove();
            }).on('webkitAnimationEnd', function () {
                stack.dom.remove();
            });

            return this;
        },
        _findInStack: function (url) {
            var found = null;
            for(var i = 0, len = this._pageStack.length; i < len; i++){
                var stack = this._pageStack[i];
                if (stack.config.url === url) {
                    found = stack;
                    break;
                }
            }
            return found;
        },
        _find: function (key, value) {
            var page = null;
            for (var i = 0, len = this._configs.length; i < len; i++) {
                if (this._configs[i][key] === value) {
                    page = this._configs[i];
                    break;
                }
            }
            return page;
        },
        _bind: function (page) {
            var events = page.events || {};
            for (var t in events) {
                for (var type in events[t]) {
                    var that = this;
                    if(type == 'click' && supportTouch){
                        (function(dom, event){
                            var touchStartY;
                            that.$container.on('touchstart', dom, function (e) {
                                touchStartY = e.changedTouches[0].clientY;
                            });
                            that.$container.on('touchend', dom, function (e) {
                                if (Math.abs(e.changedTouches[0].clientY - touchStartY) > 10) return;
                                e.preventDefault();

                                events[dom][event].call(this, e);
                            });
                        })(t, type);
                    }else{
                        this.$container.on(type, t, events[t][type]);
                    }
                }
            }
            page.isBind = true;
        }
    };
    var pages = {}, tpls = $('script[type="text/html"]');
    window.home = function(){
        location.hash = '';
    };

    for (var i = 0, len = tpls.length; i < len; ++i) {
        var tpl = tpls[i], name = tpl.id.replace(/tpl_/, '');
        pages[name] = {
            name: name,
            url: '#' + name,
            template: '#' + tpl.id
        };
    }

    pages.home.url = '#';
    pages.home.events = {
        '.js_item': {
            click: function (e) {
                var id = $(this).data('id');
                pageManager.go(id);
            }
        },
        '.js_category': {
            click: function(){
                var winH = $(window).height();
                var categorySpace = 10;
                return function(){
                    var $this = $(this),
                        $inner = $this.next('.js_categoryInner'),
                        $page = $this.parents('.page'),
                        $parent = $(this).parent('li');
                    var innerH = $inner.data('height');

                    if(!innerH){
                        $inner.css('height', 'auto');
                        innerH = $inner.height();
                        $inner.removeAttr('style');
                        $inner.data('height', innerH);
                    }

                    if($parent.hasClass('js_show')){
                        $parent.removeClass('js_show');
                    }else{
                        $parent.siblings().removeClass('js_show');

                        if(this.offsetTop + this.offsetHeight + innerH > $page.scrollTop() + winH){
                            $page.scrollTop(this.offsetTop + this.offsetHeight + innerH - winH + categorySpace);
                        }
                        $parent.addClass('js_show');
                    }
                };
            }()
        }
    };
    pages.input.events = {
        '#showTooltips': {
            click: function () {
                var $tooltips = $('.js_tooltips');
                if ($tooltips.css('display') != 'none') {
                    return;
                }

                // 如果有`animation`, `position: fixed`不生效
                $('.page.cell').removeClass('slideIn');
                $tooltips.css('display', 'block');
                setTimeout(function () {
                    $tooltips.css('display', 'none');
                }, 2000);
            }
        }
    };
    pages.toast.events = {
        '#showToast': {
            click: function (e) {
                var $toast = $('#toast');
                if ($toast.css('display') != 'none') {
                    return;
                }

                $toast.fadeIn(100);
                setTimeout(function () {
                    $toast.fadeOut(100);
                }, 2000);
            }
        },
        '#showLoadingToast': {
            click: function (e) {
                var $loadingToast = $('#loadingToast');
                if ($loadingToast.css('display') != 'none') {
                    return;
                }

                $loadingToast.fadeIn(100);
                setTimeout(function () {
                    $loadingToast.fadeOut(100);
                }, 2000);
            }
        }
    };
    pages.dialog.events = {
        '#showDialog1': {
            click: function (e) {
                var $dialog = $('#dialog1');
                $dialog.fadeIn(200);
                $dialog.find('.weui-dialog__btn').one('click', function () {
                    $dialog.fadeOut(200);
                });
            }
        },
        '#showDialog2': {
            click: function (e) {
                var $dialog = $('#dialog2');
                $dialog.fadeIn(200);
                $dialog.find('.weui-dialog__btn').one('click', function () {
                    $dialog.fadeOut(200);
                });
            }
        },
        '#showDialog3': {
            click: function (e) {
                var $dialog = $('#dialog3');
                $dialog.fadeIn(200);
                $dialog.find('.weui-dialog__btn').one('click', function () {
                    $dialog.fadeOut(200);
                });
            }
        },
        '#showDialog4': {
            click: function (e) {
                var $dialog = $('#dialog4');
                $dialog.fadeIn(200);
                $dialog.find('.weui-dialog__btn').one('click', function () {
                    $dialog.fadeOut(200);
                });
            }
        }
    };
    pages.progress.events = {
        '#btnStartProgress': {
            click: function () {

                if ($(this).hasClass('weui-btn_disabled')) {
                    return;
                }

                $(this).addClass('weui-btn_disabled');

                var progress = 0;
                var $progress = $('.js_progress');

                function next() {
                    $progress.css({width: progress + '%'});
                    progress = ++progress % 100;
                    setTimeout(next, 30);
                }

                next();
            }
        }
    };
    pages.actionsheet.events = {
        '#showIOSActionSheet': {
            click: (function(){
                function hideActionSheet(weuiActionsheet, mask) {
                    weuiActionsheet.removeClass('weui-actionsheet_toggle');
                    mask.removeClass('actionsheet__mask_show');
                    weuiActionsheet.on('transitionend', function () {
                        mask.css('display', 'none');
                    }).on('webkitTransitionEnd', function () {
                        mask.css('display', 'none');
                    })
                }
                return function () {
                    var mask = $('#mask');
                    var weuiActionsheet = $('#weui-actionsheet');
                    weuiActionsheet.addClass('weui-actionsheet_toggle');
                    mask.show().focus().addClass('actionsheet__mask_show').one('click', function () {
                        hideActionSheet(weuiActionsheet, mask);
                    });
                    $('#actionsheet_cancel').one('click', function () {
                        hideActionSheet(weuiActionsheet, mask);
                    });
                    weuiActionsheet.unbind('transitionend').unbind('webkitTransitionEnd');
                }
            })()
        },
        '#showAndroidActionSheet':{
            'click':function(){
                var $androidActionSheet = $('#weui-android-actionsheet');
                var $androidMask = $androidActionSheet.find('.weui-mask');
                $('#weui-android-actionsheet').fadeIn(200);
                $androidMask.one('click',function () {
                    $androidActionSheet.fadeOut(200);
                });
            }
        }
    };
    pages.searchbar.events = {
        '#search_input':{
            focus:function(){
                //searchBar
                var $weuiSearchBar = $('#search_bar');
                $weuiSearchBar.addClass('weui-search-bar_focusing');
            },
            blur:function(){
                var $weuiSearchBar = $('#search_bar');
                $weuiSearchBar.removeClass('weui-search-bar_focusing');
                if($(this).val()){
                    $('#search_text').hide();
                }else{
                    $('#search_text').show();
                }
            },
            input:function(){
                var $searchShow = $('#search_show');
                if($(this).val()){
                    $searchShow.show();
                }else{
                    $searchShow.hide();
                }
            }
        },
        '#search_cancel':{
            touchend:function(){
                $("#search_show").hide();
                $('#search_input').val('');
            }
        },
        '#search_clear':{
            touchend:function(){
                $("#search_show").hide();
                $('#search_input').val('');
            }
        }
    };

    for (var page in pages) {
        pageManager.push(pages[page]);
    }
    pageManager
        .setPageAppend(function($html){
            var $foot = $html.find('.page__ft');
            if($foot.length < 1) return;

            if($foot.position().top + $foot.height() < winH){
                $foot.addClass('j_bottom');
            }else{
                $foot.removeClass('j_bottom');
            }
        })
        .setDefault('home')
        .init();
});