// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function deleteTask(id) {
  window.location.href = "/Dashboard/DeleteTask?Id=" + id;
}

function cancelEditTask(id) {
  window.location.href = "/Dashboard/TaskDetails?Id=" + id;
}

function acceptReq(id) {
  window.location.href = "/Dashboard/AcceptRequest?Id=" + id;
}

function rejectReq(id) {
  window.location.href = "/Dashboard/RejectRequest?Id=" + id;
}

function progressBar(w) {
  document.getElementById("progress").style.width = w + "%";
}

function cancelEditProfile() {
  window.location.href = "/Dashboard/Profile";
}

function removeUser(id) {
  window.location.href = "/Dashboard/RemoveUser?Id=" + id;
}

$(function () {
  $(document).on("change", "#fileinput", function () {
    var files = !!this.files ? this.files : [];
    if (!files.length || !window.FileReader) return; // no file selected, or no FileReader support

    if (/^image/.test(files[0].type)) { // only image file
      var reader = new FileReader(); // instance of the FileReader
      reader.readAsDataURL(files[0]); // read the local file
      reader.onloadend = function () { // set image data as background of div
        //alert(uploadFile.closest(".upimage").find('.imagePreview').length);
        $("#pfp").css("background-image", "url(" + this.result + ")");
      }
    }

  });
});