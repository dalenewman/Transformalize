window.fileHandler = function (alias) {

   var container = $('#id_' + alias);
   var model = {
      expectingFile: container.find('.expecting-file'),
      hidden: $('#id_' + alias + '_Old'),
      fileInput: container.find('input[type="file"]'),
      progressBar: container.find('.progress-bar'),
      addButton: container.find('.fileinput-button'),
      hasFile: container.find('.has-file'),
      removeButton: container.find('.removeButton'),
      fileReceived: function (result) {
         this.hidden.val(result.id);
         this.hasFile.find('input').val(result.message);
         this.hasFile.show();
         this.fileInput.prop('disabled', true);
         this.expectingFile.hide();
      }
   };

   model.removeButton.click(function () {
      model.progressBar.css('width', "0%");
      model.expectingFile.show();
      model.hidden.val('');
      model.hasFile.hide();
      model.fileInput.prop('disabled', false);
   });

   var ajaxTime = new Date().getTime();
   model.expectingFile.fileupload({
      url: settings.uploadUrl,
      maxNumberOfFiles: 1,
      disableImageResize: false,
      imageMaxWidth: 1920,
      imageMaxHeight: 1080,
      imageCrop: false,
      done: function (e, data) {
         var totalTime = new Date().getTime() - ajaxTime;
         if (totalTime < 200) {
            setTimeout(function () {
               model.fileReceived(data.result);
            }, 150);
         } else {
            model.fileReceived(data.result);
         }
      },
      progressall: function (e, data) {
         var progress = data.loaded / data.total * 100;
         model.progressBar.css('width', progress + "%");
      }/*,
      start: function () {
         ajaxTime = new Date().getTime();
         model.progressBar.addClass('active');
      },
      stop: function () {
         model.progressBar.removeClass('active');
      } */
   });
};