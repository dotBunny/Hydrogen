var data = {
	labels : ["07/13","08/13", "09/13", "10/13", "11/13", "12/13", "01/14", "02/14","03/14", "04/14", "05/14","06/14","07/14","08/14","09/14"],
	datasets : [
		{
			fillColor : "rgba(66,139,202,0.5)",
			strokeColor : "rgba(66,139,202,1)",
			pointColor : "rgba(66,139,202,1)",
			pointStrokeColor : "#fff",			
			data : [1598, 1619, 1655,1662, 3820, 3933, 5639,6414,6421,6421,6421,6424,6424,6458,6458]
		},
	]
}

var myNewChart = new Chart($("#loc").get(0).getContext("2d")).Line(data);