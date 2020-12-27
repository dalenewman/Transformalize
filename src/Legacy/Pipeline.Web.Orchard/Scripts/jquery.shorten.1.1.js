/*
 * jQuery Shorten plugin 1.1.0
 *
 * Copyright (c) 2013 Viral Patel
 * //viralpatel.net
 *
 * Dual licensed under the MIT license:
 *   //www.opensource.org/licenses/mit-license.php
 */

 (function($) {
	$.fn.shorten = function (settings) {
	
		var config = {
			showChars: 32,
			ellipsesText: "...",
			moreText: "more",
			lessText: "less"
		};

		if (settings) {
			$.extend(config, settings);
		}
		
		$(document).off("click", '.morelink');
		
		$(document).on({click: function () {

				var $this = $(this);
				if ($this.hasClass('less')) {
					$this.removeClass('less');
					$this.html(config.moreText);
				} else {
					$this.addClass('less');
					$this.html(config.lessText);
				}
				$this.parent().prev().toggle();
				$this.prev().toggle();
				return false;
			}
		}, '.morelink');

		return this.each(function () {
			var $this = $(this);
			if($this.hasClass("shortened")) return;
			
			$this.addClass("shortened");
         var content = $.trim($this.html());
			if (content.length > config.showChars) {
				var c = content.substr(0, config.showChars);
				var h = content.substr(config.showChars, content.length - config.showChars);
				var html = c + '<span class="moreellipses">' + config.ellipsesText + ' </span><span class="morecontent"><span>' + h + '</span> <a href="#" class="morelink">' + config.moreText + '</a></span>';
				$this.html(html);
				$(".morecontent span").hide();
			}
		});
		
	};

 })(jQuery);

