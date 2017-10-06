# CATSBot2
An updated version of CATSBot.net's bot. This project was only made to improve my programming skills. I do not support any illegal activity by using this program. The bot uses [Tesseract](https://github.com/tesseract-ocr/tesseract) for the OCR technology.

## Table of contents
1. [Introduction](#introduction)
2. [Settings](#settings)
    1. [Quick fighting](#quick-fighting)
        1. [Quick Fight](#quick-fight)
        2. [Skip above health](#skip-above-health)
        3. [Stop at coins](#stop-at-coins)
    2. [Fight Championship](#fight-championship)
    3. [Box opening](#box-opening)
        1. [Open Boxes](#open-boxes)
        2. [Skip 30m](#skip-30m)
	3. [Crown Max](#crown-max)
    4. [Hibernation](#hibernation)
        1. [Exit at Stage Max](#exit-at-stage-max)
        2. [Hibernate in](#hibernate-in)
    5. [Latency settings](#latency-settings)
3. [Starting/Stopping the bot](#startingstopping-the-bot)
4. [General Advices](#general-advices)
	
## Introduction
This project was originally started by the team of [catsbot.net](https://catsbot.net), by [@p410n3](https://github.com/p410n3) and [@1n9i9c7om](https://github.com/1n9i9c7om). Then by rewriting the program, multiple new features were added into spectrum. This guide's goal is to make you feel more familiar with the different options and settings the program offers, because some of them are not evident at first sight (I'm not a good namer). My english is not the best, so keep in mind that I may make mistakes throughout the guide, for which I would like to apologize.
## Settings
Before starting the program you will face a lot of different options. I would advise you to set all the desired settings before starting the bot. I am not saying it will cause any problems if you change the settings while its running, but it can.
### Quick fighting
The bot will stop fighting if you reach stage max, unless other options let it fight.
#### Quick Fight
If checked, the bot will do quick fights, just as it did in the original bot.
#### Skip above health
Checking this option will add another step to the process. It will check the enemy's health, if its higher or less then the set amount, and then skips or attacks accordingly.
#### Stop at coins
The bot will check how many coins you have, and if its less than the set amount it will stop skipping opponents, and will work again just as it did in the original bot, however if you go back above the coin level (by opening boxes or defending your league position), it will start skipping again.
### Fight Championship
if checked, when starting the bot, it will try to read how much time is left from the championship, and if it successfully gets the value, it will compete in the championship 1 hour before it ends. Because the text, which shows the time, is so small, the program cannot always determine it correctly, so when it can't, it will disable the option, until next start. The detecting may take up to 2-5 minutes. Make sure, you select the loadout you want to quickfight with in the list box(loadout: 1, 2, 3), so the bot can set it back, before attacking again. The bot will also compete just before it will [hibernate (see below)](#hibernation), if the option is checked.
### Box opening
I would advise you to start the bot with a box to unlock, because if the bot unlocks the box it will know when will it be ready to open, thus it won't do all the unnecessary steps to determine if there are openable boxes. You could spare a lot of time with this.
#### Open Boxes
Checking this option will make the bot check if there are openable sponsor, regular or super boxes, it opens them and unlocks new ones.
#### Skip 30m
If you enable this setting will make the bot watch videos. But it only works if you have already reached stage max, or the quick fighting option is not set.
#### Crown Max
This setting will make the bot only fight quick fights until all the boxes get max crowns (12), it will then start unlocking boxes and watching videos, if all the boxes have been opened it will start fighting again.
### Hibernation
Before every hibernation, the bot will compete in the [championship](#fight-championship), if the Fight Championship option is checked.
#### Exit at Stage Max
If checked the bot will stop everything and close the emulator after you reach stage max.
#### Hibernate in
This setting means that no matter what happens, the bot will stop everything, close the emulator and hibernate the computer after it reaches the set time. If no time is set, or the time is less than 0.5. The bot will hibernate the computer when you reach stage max.
### Latency settings
Increasing the latency value, won't neccessary make the bot run slower. All it does is it lengthens the time the bot has, to detect a specific object. To make an example, when checking for the quick fight button, if the value is higher the bot will check not 3, but 6 times if is there. Why is this there then, you could ask. If you go back to the main screen, on slower networks with worse internet, it takes approximately 5 seconds to load the screen, until then it will be black. Now if, for example checking the screen 3 times takes 4 seconds, the bot won't find the button and it will think that something is wrong, and it will restart everything, you wouldn't want that to happen every time it checks the screen do you. The only reason it could be bad is if there was a real problem, because if the value would be higher, it would take more time for the bot the determine that and solve the problem.
## Starting/Stopping the bot
You can start the bot with the big toogle button at the bottom. When first starting the bot, it will take more time to initialize, because it will read how much time is left in the championship and until there is a box it didn't unlock it will check if its openable everytime the bot gets to that state. Make sure you always start the bot when the game is at the main screen, where the quick fight button is, otherwise it may not work properly and it could restart.

Stopping the bot, you first press the big toogle button which sets it into an intermediate state, which means that it will stop working after the current process is finishes, if the bot is stopped this way, it is guaranteed that it will be re-runnable again. If you would like to stop the bot immediately, you can press the button another time, which will stop everything where it was.
## General Advices
- Set the emulator's resolution to 720p HD (1280x720)
- Let the bot unlock a box when you start it.
- For quickfighting take a wooden bodypart, and attach a weapon with one-shot capable damage, the best one is the minigun.
- From my personal experience it is best to set the bot to crown maxing. You will get the money back you lost from skipping by opening boxes, and your rating will still go up.
- If you run out of videos to watch, you can get new videos by changing the emulated device (device, location, phone number and IMEI number). The given number of videos may differ but its around 20 to 50.
- Just a taste of what its capable of: from ~5540 attacks I've won ~5010, with a winrate of 90.4%. The avarage skips it does per attack is 1.18. Since implementing the box opening option (approx. 3days ago), it opened me ~150 boxes and watched ~650 videos.
- __Use the bot with full responsibility. From the moment you start your bot you are considered a cheater. Be prepared to get banned, because it is more than possible that the bot gets detected, although I've made everything that I could to evade just that.__
