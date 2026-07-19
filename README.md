Hi its my frist coding project so bugs are expected 

WHAT IT IS AND WHAT DOES IT DO

its plugin for Macro Deck 2 that shows your Truck Speed,actual speed limit, Fuel percentage , status of lights,
bliners,parking brake,status of cruise control and speed set on cruise controll
in configure you can change betwen KM/h and Mph 

HOW TO CONFIGURE MY PLUGIN
fro version 1.1.26 you need actin button to "Start" and "stop" connection with ETS2/ATS tlemetry server 
1.install Macro Deck 2

2.install my plugin Ets2Telemetry

3.install ETS 2

4.install ETS2/ATS telemertry server https://github.com/Funbit/ets2-telemetry-server

Now let's create buttons
all you need to do in macro deck is create two buttons first ACTION -> ETS2 TELEMETRY -> START ETS2/ATS TELEMETRY this will start connection between plugin and ets2/ats telemetry server and second button ACTION -> ETS2 TELEMETRY -> STOP ETS2/ATS TELEMETRY to stop that cnnction Otherwise, the plugin will keep trying to connect to the server all the time, which will cause errors to pop up in MD2 

then create buttons for indicators enter the appropriate line as button label "Label" 
you can edit them as you wish for e.g instead of using labels you can use "on event" and then set some nice icons to show status 

a list of labels and a description of what each label does
|variable | descryption|
|---------|------------|
|{ets2_speed}|  it shows your actual speed you can add Km/h or Mph at end and "speedometer" under it to make soure what it shows |
|{ets2_speed_limit}| it shows your actual speed limit as abowe you can add Km/h or MPh at end and "limit" |
|{ets2_cruise_on}| shows status of cruise controll|
|{ets2_cruise_speed}| shows speed set for cruise controll|
|{ets2_gear}| it shows your actual gear R N 1 2 3 etc  you can write "Gear" under it to now what it shows| 
|{ets2_park_brake}| it shows your parking brake status ON/OFF|
|{ets2_lights}| it shows lights status PARK LOW HIGH OFF or you can set them as 3 separate button|
|{ets2_lights_high}| for high ligits|
|{ets2_lights_low}|  for low lights|
|{ets2_lights_parking}| for parking lights|
|{ets2_blinker}| it shows blinkers status LEFT RIGHT HAZARD or you can set them as 3 separate button|
|{ets2_blinker_left}| for left blinker|
|{ets2_blinker_right}| for right blinker|
|{ets2_blinker_hazard}| for hazard lights|
 
to to change the conversion between Km/h and Mph go to the plugin configuration

when evry thing is done just start ETS2/ATS telemetry server and then start ETS2 

