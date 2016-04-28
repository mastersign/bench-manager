# BenchManager ToDo

## Basics

* ~~add support for custom uninstall scripts~~  
  _Existence of uninstall script inhibits default uninstall operation._
* ~~add support for global custom `setup.ps1`~~
* ~~add support for global custom `env.ps1`~~
* add support for global custom `clean.ps1`
* ~~add exception handling to all file system operations~~

## Reporting

* ~~Fix progress bar~~
* ~~Add app ID to progress to allow instant updates of individual apps~~
* ~~Add detailed exception info to `AppTaskError`~~
* ~~Catch output of external processes during task execution~~
* ~~Write log for individual apps~~

## UX

* ~~Integrated execution of command line processes via ConEmu embedded~~
* ~~Thorough checks for all tasks~~
	+ ~~Download Resource~~
	+ ~~Delete Resource~~
	+ ~~Install app~~
	+ ~~Uninstall app~~
	+ ~~Reinstall app~~
	+ ~~Upgrade app~~
* ~~Move checks into `AppFacade`~~
* ~~Allow cancelation by user~~3
* Prevent setup window from closing until cancelation is through
* Clean cancelation in case the BenchManager process is exited
* ~~Regard dependencies~~
	+ ~~Install all dependencies recursively before installing an app~~
	+ ~~Uninstall all responsibilities before uninstalling an app
	  (with the exception of execution environments like NodeJS and Python)~~
	+ inform the user when installing / uninstalling recursively
* About dialog with acknowledgments

## Command Line Interface

* ~~clean-up `bench-ctl` and PS actions~~

## Bootstrapping

* clear setup dialog for first configuration
* find elegant way to incorporate BenchManager binaries and libraries 
* alternative setup strategy without BenchManager (pure command line setup)
* make git SSH support optional
* make git optional

## Documentation

* Setup process
* Command line interface
* Graphical user interface

## Project Support

* project registry
* project actions
* project type based plugin system (facets)