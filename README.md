# Hellfire Miles
This project is licensed under the [GNU General Public License v3.0](https://www.gnu.org/licenses/gpl-3.0.txt). This basically means you can redistribute this product with modification, for patent or private use, as long as you accredit me and keep the license on your derivation of the project. You may NOT distribute the code as closed-source. Please follow the aforementioned link for more information.

## Why was this made?
Intended to complement Mike Wright's [Hellfire 3](http://www.railrover.co.uk/) this program was written as a personal project that allows for a more organized list of moves (aka, a journey behind a loco or in a unit from one destination to another) that the player has done over various games (or as the game calls them, weeks).
 
It was created with RailMiles in mind, following a very similar layout where the user can view their all-time journeys, as well as number of miles behind each loco/class, and a ranking of each class sorted by total miles. This program also features import/export, where the user can export their moves list/data as a .hfm file. Other users can import .hfm files to view journeys, or to compare their statistics with their own using the Compare Stats/Compare Miles feature.

This program was a huge learning experience for me during its creation, with multithreading and file reading/writing being the two main big things, as well as extra knowledge surrounding the Microsoft .NET Framework, especially on the data side of things. (I hope to never have to extensively use a dataGridView ever again.)

## Setup
To ensure that data gets recorded in the program, once you're done an in-game week, use Hellfire's Export Moves Book function, accessible through the Moves Book window. For program purposes, save all of these in the same folder.

All you really need to do to get the ball rolling, in terms of setting up the program itself, is to set the directory where all of your Hellfire moves files are. These are saved in .csv format. Once selected, restart the program and your moves list will show based on all .csv files in the folder. You can always change the directory later, if necessary.

Don't have any Hellfire moves files or just want to check the program out with some sample data? There is a .zip file containing a set of moves for you to import into the program, located in the Release section.

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
The traction viewer allows you to view your mileages behind each individual locomotive. These can be filtered by loco class, as well as a mile threshold that only display locomotives that you have had for less than or greater than a specified number of miles.
![HellfireMiles_aLouLevIff](https://user-images.githubusercontent.com/58154576/136306385-9274c511-3d76-4d11-a5c9-e01ae7aba695.png)

A Traction League is also available, accessible from the Traction Viewer, which ranks all haulages behind all classes:
![HellfireMiles_nTSddmHqDp](https://user-images.githubusercontent.com/58154576/136307278-07242eae-bb71-479d-888f-6af4480e0e6f.png)

### Import/Export Data
One big feature of this program is the ability to export your full moves lists/haulage data as .hfm files, accessible from the JourneyView.
These files can also be imported into other PCs' copies of the application. If a friend sends you their .hfm file, this allows for:
- Viewing of the complete moves list of other people's data (via the Import function)
- Comparing statistics (in the format of the Traction League) between two people's data
- Comparing mileages per loco between two people's data
![HellfireMiles_CF0pgDKytT](https://user-images.githubusercontent.com/58154576/136308490-44bab860-6d9a-4856-b495-f464a3341237.png)
![HellfireMiles_NisJkveVqE](https://user-images.githubusercontent.com/58154576/136308261-a80bc29b-313e-4226-a657-5dc566facc31.png)

## Issues
- Filtering haulages by loco using the mile threshold in the Traction Viewer will result in a NullReferenceException if ran in debug mode, regardless of the presence of the try-catch block. In VS2019, this can be ignored by pressing Continue once the exception is caught.
- Some DataGridView objects can be user edited. This doesn't modify existing move data permanently but it will be fixed in a future commit

## Potential Future Additions
- Conversion of moves to a neat, text format (button option)
- Loading screens while opening other forms
- Add loco depots/sort by depot

## Closing Remarks
### Credits
- Thank you to Mike Wright for the creation of Hellfire, an absolutely awesome, unique, and intricate game that has costed me hours and hours of free time - all in the name of chasing them locos. Totally worth it.
- Also thank you to Omar Diab and Jamie Sweetland for bearing with me as I used them as guinea pigs for testing the program!
- Thanks as well to some friends from college for giving me tips on better thread management. Without them, you'd be waiting ages for the program to respond while a window is being loaded.

Got any bugs to report? Feature suggestions? Toss them in the issues section.
