Slackypoo - Send email content to Slack

	- I use this at work to forward important build content to a team build chatroom
	- The system works beautifully

Third party dependency code/libraries (also included with this check in):

	- Simple Slack Client: https://gist.github.com/jogleasonjr/7121367
		- Because why write something someone already wrote?
	- ActiveUp Email library
		- Nice .net Email library for interacting with email

	
Recommended Usage:

	- Prerequisite #1: Get Slack API token
	- Prerequisite #2: Set up a common share email account
	- Email forwarding: 
		- Set up forwarding rules from 'some email account' to 'a shared content email account'
			- Q: Why do this?
			- A: So you don't have to share your password in the tool's configuration
	- Configure the Slackypoo tool's app.config:
		SlackApiToken
		SlackUsername	
		SlackChannel
		EmailServer
		EmailPort
		EmailUseSsl	
		EmailUser	
		EmailPassword
		EmailMailbox	
		EmailSendSubjectOnly
	- NOTE: Some email processing code may/WILL need to be edited to suit your organization's needs
		- See TransferEmailToSlack() in Program.cs to customize this method
	- Set up some sort of Task Scheduler process to run on your system/server to run this program every 5 minutes
		- See file (also included with this check in): !run_slackypoo.cmd


Support the developer
---
If you would like to support development of this software, you can contribute with a donation by clicking on the Donate Icon below. Thank you for your support!

[![Donate](https://www.paypalobjects.com/en_US/i/btn/btn_donate_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=PXV8MLB5KR5WG)


This work is licensed under a Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.
  - https://creativecommons.org/licenses/by-nc-sa/4.0/
