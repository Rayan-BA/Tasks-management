// file changed.

function linechart() {
  var chart1 = document.getElementById("linechart");
  lineChartConfig = {
    type: 'line',
    data: {
      labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
      datasets: [{
        data: ['1150', '1160', '1160', '1165', '1160', '1170', '1175', '1165', '1170', '1180', '1185', '1190'],
        backgroundColor: "rgba(48, 164, 255, 0.2)",
        borderColor: "rgba(48, 164, 255, 0.8)",
        fill: true,
        borderWidth: 1
      }]
    },
    options: {
      animation: {
        duration: 2000,
        easing: 'easeOutQuart',
      },
      plugins: {
        legend: {
          display: false,
          position: 'right',
        },
        title: {
          display: true,
          text: 'Sales volume',
          position: 'left',
        },
      },
    }
  };

  if (myChart1) {
    myChart1.destroy();
    myChart1 = new Chart(chart1, lineChartConfig);
  }
  var myChart1 = new Chart(chart1, lineChartConfig);
}

function barchart() {
  var chart2 = document.getElementById('barchart');
  barChartConfig = {
    type: 'bar',
    data: {
      labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
      datasets: [{
        label: 'Income',
        backgroundColor: "rgba(54, 162, 235, 0.5)",
        borderColor: "rgb(54, 162, 235)",
        borderWidth: 1,
        data: ["20", "30", "40", "50", "60", "70", "80"],
      }, {
        label: 'Expenses',
        backgroundColor: "rgba(244, 67, 54, 0.5)",
        borderColor: "rgb(255, 99, 132)",
        borderWidth: 1,
        data: ["5", "15", "25", "35", "45", "35", "25"],
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
          position: 'top',
        },
        title: {
          display: true,
          text: 'Revenue',
          position: 'left',
        },
      },
    }
  };

  if (myChart2) {
    myChart2.destroy();
    myChart2 = new Chart(chart2, barChartConfig);
  }
  var myChart2 = new Chart(chart2, barChartConfig);
}

function piechart() {
  var chart3 = document.getElementById("piechart");
  pieChartConfig = {
    type: 'pie',
    data: {
      labels: ["Engineering", "Customer Support", "Operations", "Marketing", "R and D"],
      datasets: [{
        data: ["62", "80", "30", "25", "17"],
        backgroundColor: ["#009688", "#795548", "#673AB7", "#2196F3", "#6da252"],
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
  if (myChart3) {
    myChart3.destroy();
    myChart3 = new Chart(chart3, pieChartConfig);
  }
  var myChart3 = new Chart(chart3, pieChartConfig);
}

function doughnutchart() {
  var chart4 = document.getElementById("doughnutchart");
  doughnutChartConfig = {
    type: 'doughnut',
    data: {
      labels: ["You", "Group 1", "Group 2", "Group 3"],
      datasets: [{
        data: ["25", "45", "20", "10"],
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

  if (myChart4) {
    myChart4.destroy();
    myChart4 = new Chart(chart4, doughnutChartConfig);
  }
  var myChart4 = new Chart(chart4, doughnutChartConfig);
}

function stackedbarchart() {
  var chart5 = document.getElementById("stackedbarchart");
  stackedBarChartConfig = {
    type: 'bar',
    data: {
      labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
      datasets: [{
        label: 'Income',
        backgroundColor: "rgba(0, 150, 136, .5)",
        borderColor: "rgb(0, 150, 136)",
        borderWidth: 1,
        data: ["20", "30", "40", "50", "60", "70", "80"],
      }, {
        label: 'Liabilites',
        backgroundColor: "rgba(76, 175, 80, .5)",
        borderColor: "rgba(76, 175, 80)",
        borderWidth: 1,
        data: ["2", "5", "15", "25", "35", "25", "15"],
      }]
    },
    options: {
      animation: {
        duration: 2000,
        easing: 'easeOutQuart',
      },
      scales: {
        x: {
          stacked: true,
        },
        y: {
          stacked: true,
        }
      },
      plugins: {
        legend: {
          display: true,
          position: 'top',
        },
        title: {
          display: true,
          text: 'Volume',
          position: 'left',
        },
      },
    }
  };
  if (myChart5) {
    myChart5.destroy();
    myChart5 = new Chart(chart5, stackedBarChartConfig);
  }
  var myChart5 = new Chart(chart5, stackedBarChartConfig);
}

function radarchart() {
  var chart6 = document.getElementById("radarchart");
  radarChartConfig = {
    type: 'radar',
    data: {
      labels: ['Age 18-24', 'Age 25-31', 'Age 32-38', 'Age 39-45', 'Age 46-100+'],
      datasets: [{
        data: ["26", "35", "25", "28", "18"],
        backgroundColor: "rgba(48, 164, 255, 0.2)",
        borderColor: "rgba(48, 164, 255, 0.8)",
      }]
    },
    options: {
      animation: {
        duration: 2000,
        easing: 'easeOutQuart',
      },
      plugins: {
        legend: {
          display: false,
          position: 'right',
        },
        title: {
          display: true,
          text: 'Age Groups',
          position: 'top',
        },
      },
    }
  };
  if (myChart6) {
    myChart6.destroy();
    myChart6 = new Chart(chart6, radarChartConfig);
  }
  var myChart6 = new Chart(chart6, radarChartConfig);
}