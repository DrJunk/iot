import pyowm
import requests
import json
from time import gmtime, strftime, sleep

owm = pyowm.OWM('be353767dba614e5554d0401240408c1')  # You MUST provide a valid API key

location = 'dudinka,rus'

url = 'https://irpihandlemessages.azurewebsites.net/api/HttpGET-HandleApiCalls'
turnOnData = {'DeviceID' : 'MainDevice',
          'ProductName' : 'aircon',
          'ActionName' : 'chmod' }
		  
while True:
	print("Checking weather in " + location + " " + strftime("%Y-%m-%d %H:%M:%S", gmtime()))
	observation = owm.weather_at_place(location)
	w = observation.get_weather()
	temperature = w.get_temperature('celsius')  # {'temp_max': 10.5, 'temp': 9.7, 'temp_min': 9.0}
	print (temperature)
	if(temperature['temp'] < 20):
		r = requests.post(url, data = json.dumps(turnOnData))
		print(r.text)
	else:
		print("Don't turn on aircon")
	sleep(60)
