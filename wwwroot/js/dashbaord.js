// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function deleteTask(id) {
  window.location.href = "/Dashboard/DeleteTask?Id=" + id;
}

function cancelEdit(id) {
  window.location.href = "/Dashboard/TaskDetails?Id=" + id;
}