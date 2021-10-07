# Hellfire Miles

## Why was this made?
Intended to complement Mike Wright's [Hellfire 3](http://www.railrover.co.uk/) this program was written as a personal project that allows for a more organized list of moves (aka, a journey behind a loco or in a unit from one destination to another) that the player has done over various games (or as the game calls them, weeks).
 
It was created with RailMiles in mind, following a very similar layout where the user can view their all-time journeys, as well as number of miles behind each loco/class, and a ranking of each class sorted by total miles. This program also features import/export, where the user can export their moves list/data as a .hfm file. Other users can import .hfm files to view journeys, or to compare their statistics with their own using the Compare Stats/Compare Miles feature.

This program was a huge learning experience for me during its creation, with multithreading and file reading/writing being the two main big things.

## Setup
To ensure that data gets recorded in the program, once you're done an in-game week, use Hellfire's Export Moves Book function, accessible through the Moves Book window. For program purposes, save all of these in the same folder.

All you really need to do to get the ball rolling, in terms of setting up the program itself, is to set the directory where all of your Hellfire moves files are. These are saved in .csv format. Once selected, restart the program and your moves list will show based on all .csv files in the folder. You can always change the directory later, if necessary.

## Prerequisites
Technically none, although the program is intended to be used in supplement with Hellfire 3 (link above).

## Functionality
When the word "all" is used in the following sections, it means all data available from .csv files you have exported.

### Journey View
![HellfireMiles_CL0PjhrXzM](https://user-images.githubusercontent.com/58154576/136306005-88317fd0-1cc6-4275-beea-3835953eea06.png)
This is a list of all journeys you have embarked on. The week #, day of the week, locos powering, destinations, and service name/headcode are all recorded. Optionally, you can use the Filter Journeys button to filter trips by a week, loco class, and/or individual loco.

![HellfireMiles_YqoqDkRO4D](https://user-images.githubusercontent.com/58154576/136306230-0cec01bf-7714-4a88-9839-23f05f78fd74.png)
![HellfireMiles_s6pDYcoG6i](https://user-images.githubusercontent.com/58154576/136306247-e647a0f2-1833-478a-bf22-d24b9a381837.png)

### Traction Viewer & Traction League
The traction viewer allows you to 

### Import/Export Data

### Compare Data

## Issues
- Filtering haulages by loco using the mile threshold in the Traction Viewer will result in a NullReferenceException if ran in debug mode, regardless of the presence of the try-catch block. In VS2019, this can be ignored by pressing Continue once the exception is caught.

## Closing Remarks
Got any bugs to report? Feature suggestions? Toss them in the issues section.

This project is licensed under the [GNU General Public License v3.0](https://www.gnu.org/licenses/gpl-3.0.txt). This basically means you can redistribute this product with modification, for patent or private use, as long as you accredit me and keep the license on your derivation of the project. You may NOT distribute the code as closed-source. Please follow the aforementioned link for more information.
