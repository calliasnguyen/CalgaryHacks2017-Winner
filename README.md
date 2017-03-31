# CalgaryHacks2017-Winner

##CalgaryHacks2017 - HealthCare Service##

[CalgaryHacks 2017 Best Xamarin Project/3rd Place Overall](http://calgaryhacks.com/)

####**Authors**: Callias Nguyen, Mo Xue, Mark Maillet, Craig Macritchie, Chris Billington ####

####**Theme**: Automation and Optimization ####

####**About:**####

This is a  healthcare service that monitors patient's vitals using IOT device that continually uploads data into the cloud,  and an ASP.NET web service that pulls sensor data from the cloud, and a mobile application that allows health care providers to monitor patient's vitals.  

####**Features**: 
Full Feedback Controllers/Monitoring Systems
Website & Web Service for viewing patient's vitals (ASP.NET)
Mobile Application for monitoring patient's vitals 


#####**1.) Monitoring Systems/Controllers:** ####

•	Heart Rate sensor (to call for emergency if heart rate is too high/low)
•	Level Controller (measures the proper amount of glucose required, when the level is below requirements, from empty reservoir, it sends a notification to staff)*
• Automatic Injection from the healthcare provider's mobile app

![Hackathon Setup ](https://lh3.googleusercontent.com/-z7z5LsytMC8/WN6sVMPPRbI/AAAAAAAAAM0/iZkt8X1VM-chrJ2z-GOExc-ktWlTlVmyQCLcB/s0/hackathon+controller.jpg "hackathon controller.jpg")

#####**2.) Service API using the .NET Framework:** 

•	Automatically gathers information from the Relayr cloud  and stores it into the database (locally)

•	Web Service that serves patient information and vitals to the mobile app clients

•	Service to allow for patient registration/modification via mobile app 
		 - Creates new devices for new patients on Relayr

			
•	Automatic monitoring system

*Allows mobile app user to set the monitor status of each patient. If the heart rate exceeds normal requirements, notify personnel*

*Notifications are done through the slack channel/text  messages/mobile app (multiple platforms)*

•	Webpage that shows  live data of heartrate for a selected patient 
*Graph also changes red show users abnormal readings*


####**3.) Mobile App:**
•	 Allows personnel to add/update patients
•	 Allows control of specific dosage fluids  to a patient on demand
•	 Remotely monitors heart rate  
•   Automatic notifications when patient's vitals turn critical
•   Automatic patient's vital syncing for up-to-date information
