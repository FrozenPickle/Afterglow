# Afterglow

##Description
Afterglow is a plugin framework used to run ambient lighting behind a monitor or TV.

Afterglow has been designed to be a versatile plugin framework to perform image capturing, colour extraction, post processing, pre output processing, and colour output. This means with the addition of plugins Afterglow can provide heaps of settings for someone how likes to tune everything or just the basic setup to get it running. 

Plugins can be added to support many types of lighting hardware.

## Plugins
##### Light Setup Plugins
Map regions of a screen to lights, usually LED's or LED strips.

* Basic Light Setup

##### Capture Plugins

* .Net CopyScreen - This captures the screen on a windows desktop and in most media playing software
* Direct3D Capture  - This captures the screen in most games, has been tested in Eve Online and Titanfall

##### Colour Extraction Plugins
Methods to speeds up the colour extraction process

* Average Colour Extraction

##### Post Process Plugins

* Colour Correction Plugin - Adjust gamma, brightness, red, green, and blue to suit your room.

##### Pre Output Plugins
In Progress

##### Output Plugins
Sends the captured colour to lights

* Arduino Output - source code in this project

### Creating new plugins
* You will only need to know some C# the user interface is automatically generated.
* Copy an existing plugin of that type and en

##Running Afterglow for the first time
1. Install and run the Afterglow software on you pc
2. Open http://localhost:8888/ a browser
3. Click the Preview On button
4. You will now have the Afterglow preview capturing you desktop colours
5. Open and close windows and you will see the preview change

##Licence
Copyright (C) 2012  Jono C. and Justin S.

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
