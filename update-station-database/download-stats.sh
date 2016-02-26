#!/bin/bash

# Download stats from lower station
wget --user=E1071 --password='E1071' --directory-prefix=data/new/lower --no-passive-ftp ftp://85.117.200.132:10031/TRENDS/*

# Download stats from upper station
wget --user=E1071 --password='E1071' --directory-prefix=data/new/upper --no-passive-ftp ftp://85.117.200.132:10032/TRENDS/*

# Download alarms from lower station (always empty for some reason, ignoring until further information)
#wget --user=E1071 --password='E1071' --directory-prefix=LOWER/$TIME/ALARMS --restrict-file-names=nocontrol ftp://85.117.200.132:10031/ALARMS/Händelse.SKV
#wget --user=E1071 --password='E1071' --directory-prefix=LOWER/$TIME/ALARMS --restrict-file-names=nocontrol ftp://85.117.200.132:10031/ALARMS/Historik.SKV
#wget --user=E1071 --password='E1071' --directory-prefix=LOWER/$TIME/ALARMS --restrict-file-names=nocontrol ftp://85.117.200.132:10031/ALARMS/Aktuella.SKV

# Download alarms from upper station (always empty for some reason, ignoring until further information)
#wget --user=E1071 --password='E1071' --directory-prefix=UPPER/$TIME/ALARMS --restrict-file-names=nocontrol ftp://85.117.200.132:10032/ALARMS/Händelse.SKV
#wget --user=E1071 --password='E1071' --directory-prefix=UPPER/$TIME/ALARMS --restrict-file-names=nocontrol ftp://85.117.200.132:10032/ALARMS/Historik.SKV
#wget --user=E1071 --password='E1071' --directory-prefix=UPPER/$TIME/ALARMS --restrict-file-names=nocontrol ftp://85.117.200.132:10032/ALARMS/Aktuella.SKV