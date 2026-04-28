$(document).ready(function () {

    // IMAGE PREVIEW
    $('#ProfileImage').on('change', function () {

        var file = this.files[0];
        if (!file) return;

        var ext = file.name.split('.').pop().toLowerCase();
        var allowed = ["jpg", "jpeg", "png", "gif", "bmp", "jfif"];

        if ($.inArray(ext, allowed) === -1) {
            alert("Only image files allowed!");
            $(this).val('');
            $('#imgPreview').attr('src', '/images/no-image.png');
            return;
        }

        var reader = new FileReader();

        reader.onload = function (e) {
            $('#imgPreview').attr('src', e.target.result);
        };

        reader.readAsDataURL(file);
    });


    // DOCUMENT PREVIEW
    $('#AddressProofFile').on('change', function () {

        var file = this.files[0];
        if (!file) return;

        var ext = file.name.split('.').pop().toLowerCase();
        var allowedDocs = ["pdf", "doc", "docx", "jpg", "png"];

        if ($.inArray(ext, allowedDocs) === -1) {
            alert("Invalid document format!");
            $(this).val('');
            $('#fileName').text('');
            return;
        }

        $('#fileName').text(file.name);
    });

});