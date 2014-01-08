var data = {
	labels : ["07/13","08/13", "09/13", "10/13", "11/13", "12/13"],
	datasets : [
		{
			fillColor : "rgba(66,139,202,0.5)",
			strokeColor : "rgba(66,139,202,1)",
			pointColor : "rgba(66,139,202,1)",
			pointStrokeColor : "#fff",			
			data : [1598, 1619, 1655,1662, 3820, 3933]
		},
	]
}

var myNewChart = new Chart($("#loc").get(0).getContext("2d")).Line(data);

