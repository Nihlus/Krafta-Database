update-station-database
======

update-station-database is a simple program which updates database records of a power station.
It reads values from externally provided SKV (semicolon separated variables) files, which it
then parses and inserts.

Each record is uniquely identified by its timestamp (date and time), and is only entered into 
the database if it is unique.

All configuration options are available in krafta.cfg, and the download script is in the project's
root directory (download-stats.sh).

## Requirements
* A running MySQL database with a single database.
