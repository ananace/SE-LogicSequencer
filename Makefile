.phony: all clean

LogicSequencer:
	cd Data/Scripts/LogicSequencer
	dotnet build

all: LogicSequencer

clean:
	find . -name 'bin' -or -name 'obj' -type d | xargs -rn1 rm -r
