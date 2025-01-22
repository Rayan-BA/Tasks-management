// file changed.

var total;
var completed;
var labels;

$.ajax({
  url: "/Dashboard/ChartData",
  type: "GET",
  dataType: "json",
  success: function (res) {
    total = res.total;
    completed = res.completed;
    labels = res.labels;
    taskChart();
  }
});

function taskChart() {
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
          min: 0,
          max: Math.max(total),
          ticks: {
            stepSize: 50
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