XmlFix
======

A small project which removes nuget targets to allow projects to use automatic package restore.

A colleague of mine wrote this so I stole it and shared it with you.

Update the tests to point at the folder which contains your old fashioned nuget solution, run the tests, you're now ready to use new fangled automatic package restore.

The tests should have cleaned up all the nuget.exe and nuget targets from your project files. 

If this doesn't work for you I feel bad for you man, I've got 99 problems but that one is all yours. Use source control and you'll be fine.
