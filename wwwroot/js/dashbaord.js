// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function deleteTask(id) {
  window.location.href = "/Dashboard/DeleteTask?Id=" + id;
}

function cancelEditTask(id) {
  window.location.href = "/Dashboard/TaskDetails?Id=" + id;
}

function selectedMembers(s) {
  $("#selected-members").append(s.values);
}

function acceptReq(id) {
  window.location.href = "/Dashboard/AcceptRequest?Id=" + id;
}

function rejectReq(id) {
  window.location.href = "/Dashboard/RejectRequest?Id=" + id;
}