# JSIL Hacks

# License

MIT/X11

# Acknowledgements

This collection of hacks is using (and would be nothing without) JSIL by Kevin Gadd.

# Getting started

## Checking out the code with submodules: 

Using git bash:

```bash
git clone git://github.com/markusjohnsson/JSIL.Hacks.git
git submodule init
git submodule update
cd Dependencies/JSIL
git submodule init
git submodule update
```

## Building JSIL
Load Solution Dependencies\JSIL\JSIL.sln in Visual Studio and build it by hitting F6.

Close the solution.

Troubleshooting: if the build fails, try to unload the XNA projects by right 
clicking and hitting "Unload project".

## Building / running JSIL.Hacks
Open JSIL.Hacks.sln at the repository root. 

Right-click the example you want to run, for example HelloWorld and hit "Set as StartUp Project". 
Hit F5 to compile & transcode the application to HTML+JS. 

Right-click the HelloWorld project again and choose "Open Folder in Windows Explorer", enter the bin\Debug
subdirectory and open MainPage.html in your favorite web browser.

