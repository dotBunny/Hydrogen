var data = {
	labels : ["07/13","08/13", "09/13", "10/13", "11/13", "12/13", "01/14", "02/14", "03/14", "04/14", "05/14","06/14","07/14","08/14","09/14"],
	datasets : [
		{
			fillColor : "rgba(66,139,202,0.5)",
			strokeColor : "rgba(66,139,202,1)",
			pointColor : "rgba(66,139,202,1)",
			pointStrokeColor : "#fff",
			
			data : [0,0,0,11,38,70,124,154,63,63,57,55,39,52,45]
		},
	]
}

var myNewChart = new Chart($("#uas").get(0).getContext("2d")).Line(data);

