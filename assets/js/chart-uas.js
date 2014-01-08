var data = {
	labels : ["07/13","08/13", "09/13", "10/13", "11/13", "12/13"],
	datasets : [
		{
			fillColor : "rgba(66,139,202,0.5)",
			strokeColor : "rgba(66,139,202,1)",
			pointColor : "rgba(66,139,202,1)",
			pointStrokeColor : "#fff",
			
			data : [0,0,0,11,38,70]
		},
		/*{
		fillColor : "rgba(220,220,220,0.5)",
			strokeColor : "rgba(220,220,220,1)",
			pointColor : "rgba(220,220,220,1)",
			pointStrokeColor : "#fff",
			
			
			fillColor : "rgba(151,187,205,0.5)",
			strokeColor : "rgba(151,187,205,1)",
			pointColor : "rgba(151,187,205,1)",
			pointStrokeColor : "#fff",
			data : [28,48,40,19,96,27,100]
		}*/
	]
}

var myNewChart = new Chart($("#uas").get(0).getContext("2d")).Line(data);

