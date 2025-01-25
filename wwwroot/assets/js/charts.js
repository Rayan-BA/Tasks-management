// file changed.

if (window.location.pathname == "/") {
  $.ajax({
    url: "/Home/ChartData",
    type: "GET",
    dataType: "json",
    success: function (res) {
      if (res.data.length > 0) {
        doughnutchart(res.labels, res.data);
      } else {
        $(".canvas-wrapper").append("<p class='text-center text-muted'>No Data</p>")
      }
    }
  });
}

function doughnutchart(labels, data) {
  var chart4 = document.getElementById("doughnutchart");
  doughnutChartConfig = {
    type: 'doughnut',
    data: {
      labels: labels,
      datasets: [{
        data: data,
        backgroundColor: ["#F44336", "#2196F3", "#795548", "#6da252"],
        hoverOffset: 4
      }]
    },
    options: {
      animation: {
        duration: 2000,
        easing: 'easeOutQuart',
      },
      plugins: {
        legend: {
          display: true,
          position: 'right',
        },
        title: {
          display: false,
          text: 'Total Value',
          position: 'left',
        },
      },
    }
  };

  if (chart) {
    chart.destroy();
    chart = new Chart(chart4, doughnutChartConfig);
  }
  var chart = new Chart(chart4, doughnutChartConfig);
}
