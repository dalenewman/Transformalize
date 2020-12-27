window.fileHandler = function (alias, scan) {

   var container = $('#id_' + alias);
   var model = {
      expectingFile: container.find('.expecting-file'),
      hasFile: container.find('.has-file'),
      hidden: $('#id_' + alias + '_Old'),
      progressBar: container.find('.progress-bar'),
      removeButton: container.find('.removeButton'),
      fileReceived: function (result) {
         this.hidden.val(scan ? result.message : result.id);

         this.hasFile.find('input').val(result.message);
         this.hasFile.show();

         this.expectingFile.find('input').prop('disabled', true);
         this.expectingFile.hide();

         if (result.id > 0) {
            container.closest('div.form-group').find('.help-block').empty();
            this.hasFile.closest('div.form-group').removeClass('has-error');
         } else {
            container.closest('div.form-group').find('.help-container').append('<span class="help-block">unable to ' + scan ? 'scan' : 'save' + ' file</span>');
            this.hasFile.closest('div.form-group').addClass('has-error');
         }

      }
   };

   model.removeButton.click(function () {

      model.hidden.val('');
      model.progressBar.css('width', '0%');

      model.expectingFile.find('input').val('');
      model.expectingFile.find('input').prop('disabled', false);
      model.expectingFile.show();

      model.hasFile.find('input').val('');
      model.hasFile.hide();

   });

   var ajaxTime = new Date().getTime();
   model.expectingFile.fileupload({
      url: scan ? settings.scanUrl : settings.uploadUrl,
      maxNumberOfFiles: 1,
      disableImageResize: false,
      imageMaxWidth: 1920,
      imageMaxHeight: 1080,
      imageCrop: false,
      imageOrientation: true,
      done: function (e, data) {
         var totalTime = new Date().getTime() - ajaxTime;
         if (totalTime < 300) {
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
      },
      start: function () {
         ajaxTime = new Date().getTime();
         model.progressBar.addClass('active');
      },
      stop: function () {
         model.progressBar.removeClass('active');
      }
   });
};