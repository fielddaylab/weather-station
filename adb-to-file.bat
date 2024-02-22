@echo off

set LOGCAT_OUTPUT_FILE="logcat_%date:/=-%_%time::=%.log"

@echo Outputting Unity logs to %LOGCAT_OUTPUT_FILE%
adb logcat Unity:D *:S > %LOGCAT_OUTPUT_FILE%