// file changed.

if (window.location.pathname == "/Dashboard") {
  $.ajax({
    url: "/Dashboard/ChartData",
    type: "GET",
    dataType: "json",
    success: function (res) {
      if (res.labels.length > 0) {
        taskChart(res.labels, res.total, res.completed);
      }
    }
  });
}

function taskChart(labels, total, completed) {
  var tasksChart = document.getElementById("tasks-chart");
  tasksChartConfig = {
    type: 'bar',
    data: {
      labels: labels,
      datasets: [
        {
          label: 'Completed',
          data: completed,
          backgroundColor: "#7478ed",
        },
        {
          label: 'Total',
          data: total,
          backgroundColor: "#f24b86",
        },
      ]
    },
    options: {
      responsive: true,
      scales: {
        x: {
          stacked: true,
          ticks: {
            autoSkip: false,
            maxRotation: 60,
            minRotation: 60
          }
        },
        y: {
          ticks: {
            display: false
          }
        },
      },
      plugins: {
        legend: {
          display: true,
          position: 'top',
        },
        title: {
          display: true,
          text: 'Number of Tasks',
          position: 'left',
        },
      },
    }
  };

  if (chart2) {
    chart2.destroy();
    chart2 = new Chart(tasksChart, tasksChartConfig);
  }
  var chart2 = new Chart(tasksChart, tasksChartConfig);
}