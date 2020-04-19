# ZAxisLevelTool:

This is an old tool that I wrote while researching the level file-formats used in some of the games developed by Z-Axis.
The code is really janky... but thats just the way it goes when you start writing a converter before you fully know the format.

Many of the files used in this engine are ZIFF-files which is a format based on [IFF](https://en.wikipedia.org/wiki/Interchange_File_Format)

If you are interested in this project, I suggest that you just use this for reference.
It can give you some insight on the various chunk-types and the differences between the games and platforms.

However I started rewriting this project as a Python extension for Blender a while ago.
This might be a better starting point, if you want to continue development for this project.
I have not had any motivation to continue with this, but if anyone picks this up I will try to contribute.

https://github.com/Morten1337/io_thps_scene/blob/zaxis-importer/_zaxis_notes.txt
https://github.com/Morten1337/io_thps_scene/blob/zaxis-importer/import_zaxis.py

____

### Supported games (Xbox / NGC)
* Dave Mirra Freestyle BMX 2
* Aggressive Inline
* BMX XXX

____

### Notes

* The way mesh instancing and transforms are handled in this code is pretty bad.
* There are some know errors with reading the materials and/or texture passes.
* All the games use DDS texture format for NGC.
* Aggressive Inline and BMX XXX use XPR texture format for Xbox. (I have not looked much into this format, but I think its a already been documented).
* Some of the games have bonus levels that are only available on the Xbox disc.  
* Most of the levels have dynamic and interactable objects. I have not really looked into this, but my guess is that there is scripting going on here.

____

### Images

![alt text](https://github.com/Morten1337/ZAxisLevelTool/raw/master/Images/IMG_24042016_013014.png "screenshot01")
![alt text](https://github.com/Morten1337/ZAxisLevelTool/raw/master/Images/IMG_24042016_013015.png "screenshot02")
