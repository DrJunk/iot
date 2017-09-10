import pyowm
import requests
import json
from time import gmtime, strftime, sleep

owm = pyowm.OWM('be353767dba614e5554d0401240408c1')  # Connecting to OWM, place a valid free key instead of XXX

location = 'Tel Aviv,isr'

url = 'https://irpihandlemessages.azurewebsites.net/api/HttpGET-HandleApiCalls'  	# API's url
turnOnData = 								# Building the data of the post request
		{'DeviceID' : 'MainDevice',
          'ProductName' : 'aircon',
          'ActionName' : 'on' }
		  
while True:
	print("Checking weather in " + location + " " + strftime("%Y-%m-%d %H:%M:%S", gmtime()))
	
	# Access the temperature using the pyowm lib
	observation = owm.weather_at_place(location)
	w = observation.get_weather()
	temperature = w.get_temperature('celsius')
	
	print (temperature)
	
	if(temperature['temp'] < 20):
		r = requests.post(url, data = json.dumps(turnOnData))	# Send a post requset the turns on the air conditioner		
		print(r.text)						# Print result
	else:
		print("Don't turn on aircon")
		
	sleep(60)
