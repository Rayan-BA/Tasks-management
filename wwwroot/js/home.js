
function loadGroup(Id, li) {
  $.ajax({
    url: "Groups",
    data: { Id: Id },
    success: function (res) {
      $("#group-partial").html(res);
      var grpsList = document.getElementById("groups-list").children;
      for (var i = 0; i < grpsList.length; i++) {
        if (grpsList[i].classList.contains("active")) {
          grpsList[i].classList.remove("active");
        }
      }
      li.classList.add("active");
    }
  });
}

function cancelEditProfile() {
  window.location.href = "/Home/Profile";
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